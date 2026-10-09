using System.IO;

namespace tjc.Modules.ThreatReport.Components
{
    static class AttachmentFiles
    {
        public static string GetContentType(string fileName)
        {
            string ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
            switch (ext)
            {
                case ".pdf": return "application/pdf";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".txt": return "text/plain";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default: return "application/octet-stream";
            }
        }
    }
}
