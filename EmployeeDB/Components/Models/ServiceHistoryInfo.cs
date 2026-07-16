using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_service_history")]
    [PrimaryKey("ServiceId", AutoIncrement = true)]
    public class ServiceHistoryInfo
    {
        public int ServiceId { get; set; }  // int
        [DigitsOnly] public string SocialSecurityNumber { get; set; }  // char(9) — stored as raw digits (mask stripped)
        public DateTime? HireDate { get; set; }  // datetime
        public DateTime? TerminationDate { get; set; }  // datetime
        public decimal? LastPayRate { get; set; }  // money
        public string CompanyName { get; set; }  // nvarchar(200)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
