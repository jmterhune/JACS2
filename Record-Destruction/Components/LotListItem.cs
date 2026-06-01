using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_rdl_log_items")]
    //setup the primary key for table
    [PrimaryKey("LogID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class LogListItem
    {
        public int LogID { get; set; }
        public string Description { get; set; }
        public string DestructionMethod { get; set; }
        public string RecordType { get; set; }
        public string RetentionPeriod { get; set; }
        public string GroupName { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public int YearCreated { get; set; }
        public int PortalId { get; set; }
        public DateTime? DateDestroyed { get; set; }
        [IgnoreColumn]
        public string FileLink
        {
            get
            {
                if (Folder == null | FileName == null)
                    return "";
                return string.Format("<a href='/Portals/{0}/{1}'>{2}</a>", PortalId, Path.Combine(Folder, FileName),FileName);
            }
        }
    }
}
