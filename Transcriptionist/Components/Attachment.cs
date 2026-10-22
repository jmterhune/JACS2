using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.IO;

namespace tjc.Modules.Transcriptionist.Components
{
    // Read-only here: the files are DNN files on the jud12 site, so this site can list and link them but
    // cannot add or remove them.
    [TableName("tjc_cat_attachments")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    internal class Attachment
    {
        public long AttachmentID { get; set; }
        public long? TranscriptionistID { get; set; }
        public int? FileID { get; set; }
        public DateTime? UploadDate { get; set; }
        public string FullPath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserID { get; set; }

        [IgnoreColumn]
        public string Filename { get { return Path.GetFileName(FullPath); } }
    }
}
