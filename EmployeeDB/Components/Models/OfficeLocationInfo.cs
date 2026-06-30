using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_office_location")]
    [PrimaryKey("OfficeLocationId", AutoIncrement = true)]
    [Cacheable("tjc_employee_office_location", CacheItemPriority.Default, 20)]
    public class OfficeLocationInfo
    {
        public int OfficeLocationId { get; set; }  // int
        public string Description { get; set; }  // nvarchar(100)
        public string Address { get; set; }  // nvarchar(300)
        public string City { get; set; }  // nvarchar(50)
        public string State { get; set; }  // char(2)
        public string Zip { get; set; }  // nvarchar(12)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
