using DotNetNuke.ComponentModel.DataAnnotations;
using System.IO;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_attachments")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    [Scope("ModuleID")]
    internal class Attachment
    {
        public int ModuleID { get; set; }
        public int AttachmentID { get; set; }
        public int FileID { get; set; }
        public int OrderID { get; set; }
        public int FormID { get; set; }
    }

    internal class AttachmentListItem : Attachment
    {
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string FullPath { get { return string.Format("/{0}{1}", Folder, FileName); } }
    }
}