using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Receives the attachment uploads (and deletes) from the incident entry form. The file bytes are
    /// written to the FileData column of the shared jud12 database, the same place the external site stores them.
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private const int MaxFileBytes = 20 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".wpd", ".jpg", ".jpeg" };

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            if (!context.Request.IsAuthenticated)
            {
                context.Response.Write(Json(0, "File Rejected. You must be Logged in to Upload Files"));
                return;
            }

            // Who can reach the submit page is set with DNN permissions, so any signed-in user may submit here.
            // Files can only be added to, or removed from, an incident the user submitted themselves
            // (the blank incident the form creates when it opens already carries their email).
            int moduleId;
            int.TryParse(context.Request.Params["moduleId"], out moduleId);
            ModuleInfo module = moduleId > 0 ? ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false) : null;
            UserInfo user = UserController.Instance.GetCurrentUserInfo();
            if (module == null || !IncidentAccess.CanSubmit(user, module.PortalID, module.TabModuleSettings))
            {
                context.Response.Write(Json(0, "File Rejected. You do not have permission to submit incidents."));
                return;
            }

            var hostSettings = context.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
            var ctl = new AttachmentController(hostSettings);
            var incidents = new IncidentController(hostSettings);
            try
            {
                if (context.Request.Files.Count > 0)
                {
                    int incidentId;
                    if (!int.TryParse(context.Request.Params["incidentId"], out incidentId) || incidentId <= 0)
                    {
                        context.Response.Write(Json(0, "File Rejected. Missing incident."));
                        return;
                    }
                    Incident target = incidents.GetIncident(incidentId);
                    if (target != null && !IncidentAccess.Owns(user, target) && !IncidentAccess.IsAdministrator(user, module.PortalID))
                    {
                        context.Response.Write(Json(0, "File Rejected. This incident was submitted by someone else."));
                        return;
                    }
                    string error;
                    int fileId = Insert(ctl, incidents, context.Request.Files[0], incidentId, out error);
                    context.Response.Write(Json(fileId, error));
                }
                else
                {
                    int fileId;
                    if (!int.TryParse(context.Request.Params["fileId"], out fileId) || fileId <= 0)
                    {
                        context.Response.Write(Json(0, "Missing file id."));
                        return;
                    }
                    Attachment existing = ctl.GetAttachment(fileId);
                    Incident owner = existing == null ? null : incidents.GetIncident(existing.IncidentID);
                    if (existing == null || owner == null
                        || (!IncidentAccess.Owns(user, owner) && !IncidentAccess.IsAdministrator(user, module.PortalID)))
                    {
                        context.Response.Write(Json(0, "File not found, or it belongs to an incident submitted by someone else."));
                        return;
                    }
                    ctl.DeleteAttachment(fileId);
                    context.Response.Write(Json(fileId, "File Deleted"));
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                context.Response.Write(Json(0, "Unexpected error processing file: " + ex.Message));
            }
        }

        private static int Insert(AttachmentController ctl, IncidentController incidents, HttpPostedFile file, int incidentId, out string error)
        {
            error = "";
            string filename = Path.GetFileName(file.FileName);
            if (!AllowedExtensions.Contains(Path.GetExtension(filename), StringComparer.OrdinalIgnoreCase))
            {
                error = "File rejected. Allowed file types: " + string.Join(", ", AllowedExtensions);
                return 0;
            }
            if (file.ContentLength <= 0)
            {
                error = "File rejected. The file is empty.";
                return 0;
            }
            if (file.ContentLength > MaxFileBytes)
            {
                error = "File rejected. Files can be at most " + (MaxFileBytes / (1024 * 1024)) + " MB.";
                return 0;
            }
            if (incidents.GetIncident(incidentId) == null)
            {
                error = "File rejected. Incident " + incidentId + " was not found.";
                return 0;
            }

            byte[] data = new byte[file.ContentLength];
            int read = 0;
            while (read < data.Length)
            {
                int n = file.InputStream.Read(data, read, data.Length - read);
                if (n <= 0) break;
                read += n;
            }

            var attachment = new Attachment
            {
                UploadedDate = DateTime.Now,
                IncidentID = incidentId,
                ContentType = AttachmentFiles.GetContentType(filename),
            };
            ctl.CreateAttachment(attachment);
            int fileId = attachment.AttachmentID;
            try
            {
                // Same stored-name convention as the external site: incident_attachment_filename.
                attachment.FileName = incidentId + "_" + fileId + "_" + filename.Replace(" ", "_");
                ctl.UpdateAttachment(attachment);
                ctl.SaveFileData(fileId, data, attachment.ContentType);
            }
            catch
            {
                try { ctl.DeleteAttachment(fileId); } catch { /* best-effort row cleanup */ }
                throw;
            }
            return fileId;
        }

        private static string Json(int fileId, string error)
        {
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { fileId = fileId, error = error });
        }
    }
}
