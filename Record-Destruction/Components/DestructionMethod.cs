using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_rdl_destruction_methods")]
    //setup the primary key for table
    [PrimaryKey("DestructionMethodID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("DestructionMethods", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class DestructionMethod : AuditBase
    {
        public int DestructionMethodID { get; set; }
        public string Description { get; set; }
    }
}
