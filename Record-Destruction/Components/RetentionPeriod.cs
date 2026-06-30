using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_rdl_retention_periods")]
    //setup the primary key for table
    [PrimaryKey("RetentionPeriodID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("RetentionPeriods", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class RetentionPeriod : AuditBase
    {
        public int RetentionPeriodID { get; set; } // int
        public string Description { get; set; } // nvarchar(100)
    }
}
