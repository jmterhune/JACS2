using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_race")]
    [PrimaryKey("RaceId", AutoIncrement = true)]
    [Cacheable("tjc_employee_race", CacheItemPriority.Default, 20)]
    public class RaceInfo
    {
        public int RaceId { get; set; }
        public string RaceCode { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
