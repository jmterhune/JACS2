using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.RecordDestruction.Components
{
    [TableName("tjc_gl_group")]
    //setup the primary key for table
    [PrimaryKey("GroupID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Departments", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Group : AuditBase
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

    }
    
}
