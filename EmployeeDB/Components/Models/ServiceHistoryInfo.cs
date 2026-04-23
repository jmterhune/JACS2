using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_service_history")]
    [PrimaryKey("ServiceId", AutoIncrement = true)]
    public class ServiceHistoryInfo
    {
        public int ServiceId { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public decimal? LastPayRate { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
