using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoints for the Photo tab on the Edit Employee page.
    ///
    ///   POST   Photos/Upload   (multipart/form-data: employeeId + file)
    ///   DELETE Photos/{id}     — clears the FileId pointer on the employee
    ///                            (the underlying file in the asset folder is
    ///                            left in place; deleting it requires DNN
    ///                            permissions outside our HR Admin scope).
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class PhotosController : DnnApiController
    {
        private const string PhotoFolderPath = "Employee-Photos";

        [HttpPost]
        [ActionName("Upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "multipart/form-data required");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int employeeId = ReadIntPart(provider, "employeeId");
                if (employeeId <= 0)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "employeeId required");

                var filePart = provider.Contents.FirstOrDefault(c =>
                    c.Headers.ContentDisposition != null
                    && !string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName));
                if (filePart == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "file part required");

                var fileName = (filePart.Headers.ContentDisposition.FileName ?? "photo").Trim('"');
                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = "photo-" + DateTime.UtcNow.Ticks;

                var portalId = PortalSettings.PortalId;
                var folder = FolderManager.Instance.GetFolder(portalId, PhotoFolderPath)
                             ?? FolderManager.Instance.AddFolder(portalId, PhotoFolderPath);

                IFileInfo file;
                using (var stream = await filePart.ReadAsStreamAsync())
                {
                    file = FileManager.Instance.AddFile(folder, fileName, stream, true);
                }
                if (file == null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not save uploaded file");

                // Stamp the new FileId onto the employee row (single-column UPDATE,
                // doesn't disturb anything else on the page).
                new EmployeeController().SetFileId(employeeId, file.FileId, UserInfo.UserID);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    FileId = file.FileId,
                    Url = FileManager.Instance.GetUrl(file),
                    FileName = file.FileName
                });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Clears the FileId pointer on the employee row. The
        /// underlying DNN file is left in place — clean-up of orphaned files
        /// is the file-manager's job.</summary>
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                if (id <= 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Employee id required");
                new EmployeeController().SetFileId(id, null, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Helper: pulls a named text part out of the multipart provider.</summary>
        private static int ReadIntPart(MultipartMemoryStreamProvider provider, string name)
        {
            var part = provider.Contents.FirstOrDefault(c =>
                c.Headers.ContentDisposition != null
                && string.Equals(
                    (c.Headers.ContentDisposition.Name ?? "").Trim('"'),
                    name,
                    StringComparison.OrdinalIgnoreCase));
            if (part == null) return 0;
            var s = part.ReadAsStringAsync().Result;
            return int.TryParse(s, out var v) ? v : 0;
        }
    }
}
