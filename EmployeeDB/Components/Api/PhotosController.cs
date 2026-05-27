using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

        /// <summary>Resize uploaded employee photos so the long edge (height)
        /// fits this many pixels. Smaller uploads are left alone — no upscaling.
        /// 640px gives a clean 320x240 hover preview on EmployeeList and still
        /// shows well on the EditEmployee tab without blowing up storage. </summary>
        private const int MaxPhotoHeight = 640;

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

                // Slurp the upload into a byte[] (the HTTP stream is one-shot
                // and can't be reset; we need to be able to inspect dimensions
                // and re-encode the resized version).
                byte[] originalBytes;
                using (var inputStream = await filePart.ReadAsStreamAsync())
                using (var buffer = new MemoryStream())
                {
                    await inputStream.CopyToAsync(buffer);
                    originalBytes = buffer.ToArray();
                }
                var processed = ResizeToMaxHeight(originalBytes, MaxPhotoHeight, fileName);

                // Rename the file so it's easy to identify in the asset folder:
                // {employeeId}-{LastName}-{FirstName}.{ext}. Falls back to the
                // upload's original filename if the employee row can't be
                // resolved (shouldn't happen — employeeId was already validated).
                var savedName = BuildPhotoFileName(employeeId, processed.Extension) ?? fileName;

                IFileInfo file;
                using (var saveStream = new MemoryStream(processed.Bytes, writable: false))
                {
                    file = FileManager.Instance.AddFile(folder, savedName, saveStream, true);
                }
                if (file == null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not save uploaded file");

                // Stamp the new FileId onto the employee row (single-column UPDATE,
                // doesn't disturb anything else on the page).
                new EmployeeController().SetFileId(employeeId, file.FileId, UserInfo.UserID);

                // Best-effort helpdesk notification — wraps any mail / lookup
                // failure so the photo save still returns 200 even if the
                // notification pipeline is down.
                try { SendPhotoUpdateEmail(employeeId, file.FileName); }
                catch (Exception mailEx)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(mailEx);
                }

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

        /// <summary>Holds the bytes we'll write to disk plus the file
        /// extension they correspond to (so the caller can name the asset
        /// {id}-Last-First.{ext}).</summary>
        private sealed class ProcessedImage
        {
            public byte[] Bytes;
            public string Extension; // "png" | "gif" | "jpg" | original fallback (no leading dot)
        }

        /// <summary>Returns a JPEG/PNG/GIF byte stream where the image's height
        /// is at most <paramref name="maxHeight"/> px (width scaled to
        /// preserve aspect ratio). Smaller images are returned untouched —
        /// we never upscale, both to avoid quality loss and so an HR Admin's
        /// already-trimmed thumb isn't re-saved at lower JPEG quality.
        ///
        /// Also reports back the correct extension for the returned bytes:
        ///   - source PNG  -> "png" (preserved, lossless)
        ///   - source GIF  -> "gif" (preserved)
        ///   - source JPEG -> "jpg"
        ///   - any other format (BMP/TIFF/etc.) when resized -> "jpg" (we re-encode)
        ///   - any other format when NOT resized -> the original filename's extension
        ///
        /// If anything in the imaging pipeline throws (corrupt file, unsupported
        /// format, GDI+ failure on the IIS app pool), the original bytes are
        /// returned so the user's upload still saves — better a too-tall photo
        /// than a 500 error during HR's daily workflow.</summary>
        private static ProcessedImage ResizeToMaxHeight(byte[] originalBytes, int maxHeight, string originalFileName)
        {
            var result = new ProcessedImage { Bytes = originalBytes };
            try
            {
                using (var inputStream = new MemoryStream(originalBytes, writable: false))
                using (var image = Image.FromStream(inputStream))
                {
                    var isPng = image.RawFormat.Guid == ImageFormat.Png.Guid;
                    var isGif = image.RawFormat.Guid == ImageFormat.Gif.Guid;
                    var isJpg = image.RawFormat.Guid == ImageFormat.Jpeg.Guid;

                    if (image.Height <= maxHeight)
                    {
                        // No resize -> preserve original bytes AND extension. Use
                        // the original filename's ext if known; otherwise infer
                        // from RawFormat.
                        result.Extension = SafeExt(Path.GetExtension(originalFileName));
                        if (string.IsNullOrEmpty(result.Extension))
                        {
                            result.Extension = isPng ? "png" : isGif ? "gif" : "jpg";
                        }
                        return result;
                    }

                    int newHeight = maxHeight;
                    int newWidth = (int)Math.Round((double)image.Width * newHeight / image.Height);
                    if (newWidth < 1) newWidth = 1;

                    using (var resized = new Bitmap(newWidth, newHeight))
                    {
                        resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                        using (var g = Graphics.FromImage(resized))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.DrawImage(image, 0, 0, newWidth, newHeight);
                        }

                        using (var output = new MemoryStream())
                        {
                            // Preserve PNG / GIF (lossless) when the original was
                            // one of those; everything else round-trips as JPEG
                            // at quality 85.
                            if (isPng)
                            {
                                resized.Save(output, ImageFormat.Png);
                                result.Extension = "png";
                            }
                            else if (isGif)
                            {
                                resized.Save(output, ImageFormat.Gif);
                                result.Extension = "gif";
                            }
                            else
                            {
                                var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                                    .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                if (jpegEncoder == null)
                                {
                                    resized.Save(output, ImageFormat.Jpeg);
                                }
                                else
                                {
                                    var encoderParams = new EncoderParameters(1);
                                    // Fully qualify -- 'Encoder' is ambiguous with System.Text.Encoder
                                    // now that we import System.Text for StringBuilder.
                                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);
                                    resized.Save(output, jpegEncoder, encoderParams);
                                    encoderParams.Dispose();
                                }
                                result.Extension = "jpg";
                            }
                            result.Bytes = output.ToArray();
                            return result;
                        }
                    }
                }
            }
            catch
            {
                // Imaging pipeline failed — keep original bytes; do best-effort
                // extension detection from the original filename.
                result.Extension = SafeExt(Path.GetExtension(originalFileName));
                if (string.IsNullOrEmpty(result.Extension)) result.Extension = "jpg";
                return result;
            }
        }

        /// <summary>Normalize a Path.GetExtension result ("." prefix, mixed case)
        /// into a bare lowercase token, or empty if none.</summary>
        private static string SafeExt(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            return raw.TrimStart('.').ToLowerInvariant();
        }

        /// <summary>Build the asset filename for an employee photo:
        ///   {employeeId}-{LastName}-{FirstName}.{ext}
        /// Non-alphanumeric characters in the names are dropped (apostrophes,
        /// periods, etc.); internal spaces / hyphens / underscores collapse
        /// to a single underscore so the format's "-" separators stay
        /// unambiguous. Returns null if the employee row can't be loaded —
        /// the caller falls back to the original upload filename.</summary>
        private static string BuildPhotoFileName(int employeeId, string extension)
        {
            try
            {
                var emp = new EmployeeController().GetEmployee(employeeId);
                if (emp == null) return null;
                var last = SanitizeForFilename(emp.LastName);
                var first = SanitizeForFilename(emp.FirstName);
                if (string.IsNullOrEmpty(last)) last = "Unknown";
                if (string.IsNullOrEmpty(first)) first = "Unknown";
                var ext = string.IsNullOrWhiteSpace(extension) ? "jpg" : extension.TrimStart('.').ToLowerInvariant();
                return employeeId + "-" + last + "-" + first + "." + ext;
            }
            catch
            {
                return null;
            }
        }

        // ---------- Helpdesk notification on photo update --------------------

        /// <summary>Sends a plaintext email to the helpdesk after a successful
        /// photo upload. Format mirrors the change-summary email emitted by
        /// EditEmployee.SendChangeNotification — same "**** ... ****" header,
        /// "---" rule, and "Saved by: ..." footer — so helpdesk tickets land
        /// in the same visual shape regardless of which page triggered them.
        ///
        /// Settings (read off ActiveModule.ModuleSettings — same keys the
        /// EditEmployee page uses):
        ///   Notify_OnSave     true|false (default false; opt-in per environment)
        ///   Notify_FromEmail  (default hr@jud12.flcourts.org)
        ///   Notify_ToEmail    (default helpdesk@jud12.flcourts.org)
        /// </summary>
        private void SendPhotoUpdateEmail(int employeeId, string fileName)
        {
            if (!NotifyOnSave()) return;

            var emp = new EmployeeController().GetEmployee(employeeId);
            if (emp == null) return; // shouldn't happen — Upload validated the id
            var displayName = (emp.DisplayName ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = ((emp.LastName ?? string.Empty).Trim()
                              + ", " + (emp.FirstName ?? string.Empty).Trim())
                              .Trim(',', ' ');
            }

            var subject = "Employee Photo Updated: " + displayName;

            var sb = new StringBuilder();
            sb.AppendLine("**** Employee Photo Updated: " + displayName + " ****");
            sb.AppendLine();
            sb.AppendLine("Photo: " + (fileName ?? string.Empty));
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine("Saved by: "
                + (UserInfo?.DisplayName ?? "(unknown)")
                + " (UserId " + (UserInfo != null ? UserInfo.UserID.ToString() : "?") + ")");

            DotNetNuke.Services.Mail.Mail.SendEmail(
                NotifyFromEmail(), NotifyToEmail(), subject, sb.ToString());
        }

        private bool NotifyOnSave()
        {
            var s = ActiveModule?.ModuleSettings;
            if (s == null || !s.Contains("Notify_OnSave")) return false;
            return bool.TryParse(s["Notify_OnSave"]?.ToString(), out var v) && v;
        }

        private string NotifyFromEmail()
        {
            var s = ActiveModule?.ModuleSettings;
            if (s != null && s.Contains("Notify_FromEmail"))
            {
                var v = s["Notify_FromEmail"]?.ToString();
                if (!string.IsNullOrWhiteSpace(v)) return v;
            }
            return "hr@jud12.flcourts.org";
        }

        private string NotifyToEmail()
        {
            var s = ActiveModule?.ModuleSettings;
            if (s != null && s.Contains("Notify_ToEmail"))
            {
                var v = s["Notify_ToEmail"]?.ToString();
                if (!string.IsNullOrWhiteSpace(v)) return v;
            }
            return "helpdesk@jud12.flcourts.org";
        }

        /// <summary>Strip filename-unsafe characters from a name component.
        /// Letters / digits pass through; spaces / hyphens / underscores
        /// collapse to "_"; everything else (apostrophes, periods, slashes,
        /// punctuation, accented Unicode forms, etc.) is dropped.</summary>
        private static string SanitizeForFilename(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var sb = new StringBuilder(s.Length);
            bool lastWasSeparator = false;
            foreach (var c in s.Trim())
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    lastWasSeparator = false;
                }
                else if (c == ' ' || c == '-' || c == '_')
                {
                    if (!lastWasSeparator && sb.Length > 0)
                    {
                        sb.Append('_');
                        lastWasSeparator = true;
                    }
                }
                // Anything else (apostrophes, periods, punctuation, etc.) is dropped.
            }
            // Trim any trailing separator we may have added.
            while (sb.Length > 0 && sb[sb.Length - 1] == '_') sb.Length--;
            return sb.ToString();
        }
    }
}
