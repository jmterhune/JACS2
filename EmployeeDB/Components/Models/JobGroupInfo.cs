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
        public int JobGroupId { get; set; }  // int
        public string Description { get; set; }  // nvarchar(100)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
