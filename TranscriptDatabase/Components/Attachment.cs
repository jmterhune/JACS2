using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_attachment")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    [Cacheable("Attachments", CacheItemPriority.Default, 20)]
    internal class Attachment:EntityBase
    {
        public int AttachmentID { get; set; }
        public int DesignationID { get; set; }
        public int FileID { get; set; }
        public string Path { get; set; }
        public string DisplayName { get; set; }
        public int UserID { get; set; }
    }
}
