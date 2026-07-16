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
        public int LogID { get; set; } // int
        public string Description { get; set; } // nvarchar(2000)
        public int GroupID { get; set; } // int
        public int RecordTypeID { get; set; } // int
        public int RetentionPeriodID { get; set; } // int
        public int DestructionMethodID { get; set; } // int
        public int YearCreated { get; set; } // int
        public DateTime? DateDestroyed { get; set; } // datetime
        public int FileID { get; set; } // int
    }
}
