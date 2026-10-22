/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.Arbitrators.Components
{
    public enum AttachmentType
    {
        TrainingCertificate = 0,
        BarCard = 1
    }

    /// <summary>
    /// Maps to tjc_arbitrators_attachments in the jud12.flcourts.org database
    /// (reached via the "Jud12" connection). Attachments are stored as DB blobs
    /// (not on disk) specifically so this internal review app - on a different
    /// server/site than the public Arbitrators module - can read them directly
    /// from the shared SQL Server database without needing a shared file path.
    /// </summary>
    [TableName("tjc_arbitrators_attachments")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    public class ArbitratorAttachment
    {
        public int AttachmentID { get; set; }
        public int ArbitratorApplicationID { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileData { get; set; }
        public DateTime UploadedOnDate { get; set; }
    }
}
