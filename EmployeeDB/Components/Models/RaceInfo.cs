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
        public int RaceId { get; set; }  // int
        public string RaceCode { get; set; }  // nvarchar(10)
        public string Description { get; set; }  // nvarchar(100)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
