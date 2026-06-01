using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_files")]
    //setup the primary key for table
    [PrimaryKey("FileId", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class File : EntityBase
    {
        public long FileId { get; set; }
        public long AssignmentId { get; set; }
        public string FileName { get; set; }
        public string DriveId { get; set; }
        public string ItemId { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }
        [IgnoreColumn]
        public Stream FileStream{ get; set; }

    }
    [TableName("court_counsel_file_list")]
    public class FileListItem
    {
        public long FileId { get; set; }
        public long AssignmentId { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public string CaseTypeName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime DateReceived { get; set; }
        public string FileName { get; set; }
        public string ItemId { get; set; }
        public string Url { get; set; }
        public string ModifiedBy { get; set; }
    }
}
