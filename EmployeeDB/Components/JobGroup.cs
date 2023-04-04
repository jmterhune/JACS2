using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_job_group")]
    //setup the primary key for table
    [PrimaryKey("JobGroupId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("JobGroups", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class JobGroup : EmployeeBase
    {
        public int JobGroupId { get; set; }

        public string Description { get; set; }
    }
}
