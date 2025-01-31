using DotNetNuke.ComponentModel.DataAnnotations;
using System.IO;

namespace tjc.Modules.Purchasing.Components
{
    internal class Attachment
    {
        public int AttachmentID { get; set; }
        public int FileID { get; set; }
        public int OrderID { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }

    [TableName("tjc_purchasing_form_order_attachments")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    internal class FormOrderAttachment: Attachment
    {
        public int FormID { get; set; }
    }

    internal class AttachmentListItem : FormOrderAttachment
    {
        public string Folder { get; set; }
        public string FullPath { get { return string.Format("/{0}{1}", Folder, FileName); } }
    }
    [TableName("tjc_purchasing_stamp_order_attachments")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    internal class StampOrderAttachment: Attachment
    {
    }
    [TableName("tjc_purchasing_supply_order_attachments")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    internal class SupplyOrderAttachment: Attachment
    {
    }

}