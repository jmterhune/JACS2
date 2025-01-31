using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_rdl_log")]
    //setup the primary key for table
    [PrimaryKey("LogID", AutoIncrement = true)]
    //configure caching using PetaPoco
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Log : AuditBase
    {
        public int LogID { get; set; }
        public string Description { get; set; }
        public int GroupID { get; set; }
        public int RecordTypeID { get; set; }
        public int RetentionPeriodID { get; set; }
        public int DestructionMethodID { get; set; }
        public int YearCreated { get; set; }
        public DateTime? DateDestroyed { get; set; }
        public int FileID { get; set; }
    }
}
