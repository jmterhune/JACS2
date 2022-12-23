using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Intranet.API.Components
{
    public class File : EntityBase
    {
        public long FileId { get; set; }
        public long AssignmentId { get; set; }
        public string FileName { get; set; }
        public string DriveId { get; set; }
        public string ItemId { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }

    }
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
