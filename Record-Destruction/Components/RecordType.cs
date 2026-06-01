using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_rdl_record_types")]
    //setup the primary key for table
    [PrimaryKey("RecordTypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("RecordTypes", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class RecordType : AuditBase
    {
        public int RecordTypeID { get; set; }
        public string Description { get; set; }
    }
}
