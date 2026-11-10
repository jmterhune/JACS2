/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tjc.Modules.Arbitrators.Components
{
    /// <summary>
    /// Verifies an uploaded file's actual binary signature ("magic bytes") rather than trusting its
    /// extension or the browser-supplied Content-Type header. Same allow list as the public Arbitrators
    /// module (tjc.Modules\Arbitrators\Components\FileSignature.cs): PDF, JPEG and PNG.
    /// </summary>
    internal static class FileSignature
    {
        public static readonly Dictionary<string, string> AllowedExtensionContentTypes =
            new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
            };

        /// <summary>
        /// Reads just the header bytes needed to identify the file type, then restores the stream's
        /// original position. Returns null when the header matches none of the allowed types.
        /// </summary>
        public static string Detect(Stream stream)
        {
            long originalPosition = stream.Position;
            try
            {
                stream.Position = 0;
                byte[] header = new byte[8];
                int read = stream.Read(header, 0, header.Length);

                if (read >= 4 && header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
                    return "application/pdf"; // %PDF
                if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                    return "image/jpeg";
                if (read >= 8 && header.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                    return "image/png";
                return null;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>True only when the extension is on the allow list AND the file's signature matches it.</summary>
        public static bool MatchesExtension(Stream stream, string extension)
        {
            string expectedContentType;
            if (!AllowedExtensionContentTypes.TryGetValue(extension, out expectedContentType))
                return false;
            return Detect(stream) == expectedContentType;
        }
    }
}
