using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_phone")]
    [PrimaryKey("PhoneId", AutoIncrement = true)]
    public class PhoneInfo
    {
        public long PhoneId { get; set; }
        public int EmployeeId { get; set; }
        public int? OfficeLocationId { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public bool IsMain { get; set; }
        public int? PhoneCascade { get; set; }
        public bool SwnText { get; set; }
        public bool SwnCall { get; set; }
        public bool SwnExcludeExtension { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
