using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.JudicialReferral.Components.Models
{
    [TableName("tjc_jr_attachments")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    public class AttachmentInfo
    {
        public int AttachmentID { get; set; }
        public int ReferralID { get; set; }
        public int FileID { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }
}
