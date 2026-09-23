using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// Endpoint behind the "Crisis24 Export" button on EmployeeList.
    ///
    /// Builds the Crisis24 Person/HR Feed CSV and pushes it straight to the
    /// Crisis24 SFTP site — the file is never handed to the browser, so
    /// employee PII doesn't take a detour through the workstation.
    ///
    /// This replaces the old Send Word Now endpoints (Sync / AddMissing /
    /// MissingContacts / Export). Crisis24 has no write API on our side: the
    /// feed is a one-way file drop, which is why there's a single action here
    /// where SWN needed four.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class Crisis24Controller : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        /// <summary>
        /// Response shape shared with the JS layer: Success drives the
        /// SweetAlert icon, Html is the ready-to-render (server-encoded) body.
        /// </summary>
        public class Crisis24Result
        {
            public bool Success { get; set; }
            public string Title { get; set; }
            public string Html { get; set; }
        }

        /// <summary>True for HR Admins (configurable via the HrAdminRole
        /// module setting, default "HR Admin") plus site admins and super
        /// users. Mirrors the gate on the HR-only admin tabs — this endpoint
        /// ships the whole active roster off-site, so View access on the
        /// module is not enough on its own.</summary>
        private bool IsHrAdmin
        {
            get
            {
                if (UserInfo == null) return false;
                if (UserInfo.IsSuperUser) return true;

                var settings = ActiveModule == null ? null : ActiveModule.ModuleSettings;
                var roleName = "HR Admin";
                if (settings != null && settings.Contains("HrAdminRole"))
                {
                    var v = settings["HrAdminRole"] as string;
                    if (!string.IsNullOrWhiteSpace(v)) roleName = v.Trim();
                }
                if (UserInfo.IsInRole(roleName)) return true;

                var portalAdmin = PortalSettings?.AdministratorRoleName;
                return !string.IsNullOrEmpty(portalAdmin) && UserInfo.IsInRole(portalAdmin);
            }
        }

        /// <summary>
        /// Builds the Person/HR feed file and returns it as a download WITHOUT
        /// uploading it. Same builder, same filename, and the same
        /// <see cref="Crisis24Uploader.Encode"/> call the transfer uses, so the
        /// downloaded bytes are exactly what a real transmission would put on
        /// Crisis24's server.
        ///
        /// This exists for the implementation hand-off — Crisis24 reviews and
        /// approves the file layout before the feed goes live — and stays
        /// useful afterwards as a dry run before a go-live or a schema change.
        ///
        /// Deliberately does NOT require Crisis24FTP to be configured: the
        /// whole point is to produce the file before the SFTP account exists.
        /// </summary>
        [HttpGet]
        [ActionName("Preview")]
        public HttpResponseMessage Preview()
        {
            if (!IsHrAdmin)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "HR Admin access required.");

            try
            {
                var file = Crisis24ExportBuilder.Build(_hostSettings);

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Crisis24Uploader.Encode(file.Content))
                };
                resp.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("text/csv") { CharSet = "utf-8" };
                resp.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment") { FileName = file.FileName };
                // Surfaced in a toast so the user can sanity-check the scope
                // without opening the file.
                resp.Headers.TryAddWithoutValidation("X-Crisis24-Rows",
                    file.RowCount.ToString(CultureInfo.InvariantCulture));
                return resp;
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Builds the Person/HR feed file and uploads it to Crisis24.
        ///
        /// POST rather than GET: it has an outward-facing side effect (a file
        /// lands on Crisis24's server and updates their person database), so
        /// it must not be reachable by a stray link or prefetch.
        /// </summary>
        [HttpPost]
        [ActionName("Export")]
        public HttpResponseMessage Export()
        {
            if (!IsHrAdmin)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "HR Admin access required.");

            const string title = "Crisis24 Export";

            // Check the destination before building anything: the builder walks
            // the whole active roster one employee's phones at a time, and
            // there's no point paying for that when we already know the file
            // has nowhere to go.
            if (!Crisis24Uploader.IsConfigured)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Crisis24Result
                {
                    Success = false,
                    Title = title,
                    Html = "<p>Crisis24 hasn't been set up on this site yet.</p>"
                         + "<p class=\"mb-0\">Once Crisis24 provides the SFTP host and account, put them in "
                         + "the <code>Crisis24FTP</code> appSetting in <code>web.config</code> "
                         + "(plus a password or <code>Crisis24SshPrivateKeyPath</code>) and this button "
                         + "will start sending the feed.</p>"
                });
            }

            try
            {
                var file = Crisis24ExportBuilder.Build(_hostSettings);

                if (file.RowCount == 0)
                {
                    // A full file is expected on every transmission, so an
                    // empty roster is a data problem, not a valid feed —
                    // sending it would look like a mass termination.
                    return Request.CreateResponse(HttpStatusCode.OK, new Crisis24Result
                    {
                        Success = false,
                        Title = title,
                        Html = "<p>No active employees were found, so nothing was sent to Crisis24.</p>"
                    });
                }

                var upload = Crisis24Uploader.Upload(file.Content, file.FileName);

                var sb = new StringBuilder();
                sb.Append("<p><strong>")
                  .Append(file.RowCount.ToString("N0", CultureInfo.CurrentCulture))
                  .Append(file.RowCount == 1 ? " employee record" : " employee records")
                  .Append("</strong> sent to Crisis24.</p>")
                  .Append("<ul class=\"list-unstyled mb-0\">")
                  .Append("<li><strong>File:</strong> ")
                  .Append(HttpUtility.HtmlEncode(file.FileName))
                  .Append("</li><li><strong>Host:</strong> ")
                  .Append(HttpUtility.HtmlEncode(upload.Host))
                  .Append("</li><li><strong>Remote path:</strong> ")
                  .Append(HttpUtility.HtmlEncode(upload.RemotePath))
                  .Append("</li><li><strong>Size:</strong> ")
                  .Append(upload.ByteCount.ToString("N0", CultureInfo.CurrentCulture))
                  .Append(" bytes</li></ul>");

                return Request.CreateResponse(HttpStatusCode.OK, new Crisis24Result
                {
                    Success = true,
                    Title = title,
                    Html = sb.ToString()
                });
            }
            catch (Exception ex)
            {
                // Surface the message rather than a bare 500 — the common
                // failures here are configuration (missing Crisis24FTP, bad
                // credentials, unreachable host) and the HR Admin needs to
                // read what went wrong to get it fixed.
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.OK, new Crisis24Result
                {
                    Success = false,
                    Title = title,
                    Html = "<p>The export could not be sent to Crisis24.</p><p class=\"text-danger mb-0\">"
                           + HttpUtility.HtmlEncode(ex.Message) + "</p>"
                });
            }
        }
    }
}
