using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_job_group")]
    [PrimaryKey("JobGroupId", AutoIncrement = true)]
    [Cacheable("tjc_employee_job_group", CacheItemPriority.Default, 20)]
    public class JobGroupInfo
    {
        public int JobGroupId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
