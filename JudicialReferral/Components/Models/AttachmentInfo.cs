using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.JudicialReferral.Components.Models
{
    [TableName("tjc_jr_attachments")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    public class AttachmentInfo
    {
        public int AttachmentID { get; set; }  // int
        public int ReferralID { get; set; }  // int
        public int FileID { get; set; }  // int
        public string Path { get; set; }  // nvarchar(2000)
        public string FileName { get; set; }  // nvarchar(500)
    }
}
