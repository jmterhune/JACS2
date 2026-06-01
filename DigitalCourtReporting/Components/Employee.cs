using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_employee")]
    [PrimaryKey("EmployeeID", AutoIncrement = true)]
    [Cacheable("Employees", CacheItemPriority.Default, 20)]
    internal class Employee:EntityBase
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
