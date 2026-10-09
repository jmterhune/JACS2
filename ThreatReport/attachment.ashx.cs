using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var ctl = new AttachmentController(context.GetScope().ServiceProvider.GetRequiredService<IHostSettings>());
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                int id;
                if (!Int32.TryParse(context.Request.QueryString["id"], out id))
                {
                    response.StatusCode = 400;
                    response.Write("Missing or invalid attachment id.");
                    //Response.End() raises a ThreadAbortException by design, which the catch at the
                    //bottom then swallowed on every request; CompleteRequest ends the request without
                    //aborting the thread, so the return below is what actually exits
                    response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Only members of the Incident Viewer role (as set on the module that links here) may open files.
                UserInfo user = UserController.Instance.GetCurrentUserInfo();
                if (user == null || user.UserID <= 0)
                {
                    response.StatusCode = 401;
                    response.Write("You must be signed in.");
                    response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }
                int moduleId;
                int.TryParse(context.Request.QueryString["mid"], out moduleId);
                ModuleInfo module = moduleId > 0 ? ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false) : null;
                if (module == null || !IncidentAccess.CanView(user, module.PortalID, module.TabModuleSettings))
                {
                    response.StatusCode = 403;
                    response.Write("You do not have permission to view this file.");
                    response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Attachment attachment = ctl.GetAttachment(id);
                if (attachment == null)
                {
                    response.StatusCode = 404;
                    response.Write("Attachment not found.");
                    response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // The file bytes are stored in the shared database by the external site. Attachments
                // uploaded before that change only exist on the external server's disk; send the
                // browser to the external copy for those.
                byte[] data = ctl.GetFileData(id);
                DateTime lastModified = attachment.UploadedDate;
                if (data == null)
                {
                    if (!string.IsNullOrEmpty(attachment.URL) && attachment.URL.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Redirect(attachment.URL, false);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    response.StatusCode = 404;
                    response.Write("The requested file is not stored in the database.");
                    response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                long fileLength = data.Length;

                response.ClearContent();
                response.Clear();

                // The site adds X-Frame-Options: SAMEORIGIN and a strict Content-Security-Policy
                // to every response via <httpProtocol><customHeaders> in web.config. The browser's
                // built-in PDF viewer renders the file inside a frame owned by its own (extension)
                // origin, so those headers make it fail with "Failed to load PDF document". Strip
                // them for this standalone file response so the PDF/image displays inline.
                try { response.Headers.Remove("X-Frame-Options"); } catch { }
                try { response.Headers.Remove("Content-Security-Policy"); } catch { }

                response.ContentType = string.IsNullOrEmpty(attachment.ContentType) ? AttachmentFiles.GetContentType(attachment.FileName) : attachment.ContentType;
                response.AddHeader("Content-Disposition",
                                   "inline; filename=\"" + attachment.FileName + "\";");
                // Advertise byte-range support. The Chromium/PDFium inline viewer issues
                // Range requests to stream a PDF and shows "Failed to load PDF document"
                // if the server ignores them and returns 200 with the whole body.
                response.AddHeader("Accept-Ranges", "bytes");

                long start = 0;
                long end = fileLength - 1;
                string rangeHeader = context.Request.Headers["Range"];
                bool isPartial = false;
                if (!string.IsNullOrEmpty(rangeHeader) &&
                    rangeHeader.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
                {
                    // Handle the three single-range forms per RFC 7233:
                    //   "bytes=a-b"  explicit start and end
                    //   "bytes=a-"   from a to end of file
                    //   "bytes=-n"   suffix: the LAST n bytes of the file
                    // The suffix form matters: PDF viewers read the xref/trailer from the
                    // end of the file with "bytes=-n". Mishandling it returns the file header
                    // instead of the trailer, so the viewer fails with "Failed to load PDF".
                    string rangeValue = rangeHeader.Substring("bytes=".Length).Trim();
                    int dash = rangeValue.IndexOf('-');
                    if (dash >= 0)
                    {
                        string startStr = rangeValue.Substring(0, dash).Trim();
                        string endStr = rangeValue.Substring(dash + 1).Trim();
                        if (startStr.Length == 0)
                        {
                            // Suffix range: last N bytes.
                            long suffix;
                            if (long.TryParse(endStr, out suffix) && suffix > 0)
                            {
                                if (suffix > fileLength) suffix = fileLength;
                                start = fileLength - suffix;
                                end = fileLength - 1;
                            }
                        }
                        else
                        {
                            long parsedStart;
                            if (long.TryParse(startStr, out parsedStart)) start = parsedStart;
                            long parsedEnd;
                            if (endStr.Length > 0 && long.TryParse(endStr, out parsedEnd)) end = parsedEnd;
                            if (end > fileLength - 1) end = fileLength - 1;
                        }
                    }

                    // Unsatisfiable range -> 416.
                    if (start < 0 || start > end || start >= fileLength)
                    {
                        response.StatusCode = 416;
                        response.AddHeader("Content-Range", "bytes */" + fileLength);
                        response.Flush();
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    isPartial = true;
                }

                long length = end - start + 1;
                if (isPartial)
                {
                    response.StatusCode = 206;
                    response.AddHeader("Content-Range",
                                       "bytes " + start + "-" + end + "/" + fileLength);
                }
                response.AddHeader("Content-Length", length.ToString());
                response.AddHeader("Last-Modified",
                                   lastModified.ToUniversalTime().ToString("R"));

                // Write the requested byte range as a single buffered response. Chrome's PDF viewer
                // streams with HTTP/2 Range requests and needs a well-formed, known-length response;
                // unbuffered chunked streaming makes it fail with "Failed to load PDF document".
                response.OutputStream.Write(data, (int)start, (int)length);
                response.Flush();
                context.ApplicationInstance.CompleteRequest();

            }
            catch (Exception exc)
            {

                context.Response.Write(" <html><body><h1>Error Processing File</h2><p>An error occurred processing the requested file.</p><div style='color:red'>");
                context.Response.Write(exc.Message);
                context.Response.Write("</div></body></html>");
                //log the real failure: this catch previously fired on every request because the
                //Response.End() calls above raise ThreadAbortException by design, so a genuine
                //error was indistinguishable from a normal download and was never recorded
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                context.Response.Flush();
                context.ApplicationInstance.CompleteRequest();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}