using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_emergency_contact")]
    [PrimaryKey("ContactId", AutoIncrement = true)]
    public class EmergencyContactInfo
    {
        public int ContactId { get; set; }  // int
        public int? EmployeeId { get; set; }  // int
        public string FirstName { get; set; }  // nvarchar(50)
        public string LastName { get; set; }  // nvarchar(50)
        public string Relationship { get; set; }  // nvarchar(50)
        [DigitsOnly] public string PhoneHome { get; set; }  // nvarchar(20) — stored as raw digits (mask stripped)
        [DigitsOnly] public string PhoneWork { get; set; }  // nvarchar(20) — stored as raw digits (mask stripped)
        [DigitsOnly] public string PhoneMobile { get; set; }  // nvarchar(20) — stored as raw digits (mask stripped)
        public int? CallOrder { get; set; }  // int
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
