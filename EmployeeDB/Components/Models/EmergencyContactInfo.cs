using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_emergency_contact")]
    [PrimaryKey("ContactId", AutoIncrement = true)]
    public class EmergencyContactInfo
    {
        public int ContactId { get; set; }
        public int? EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        [DigitsOnly] public string PhoneHome { get; set; }
        [DigitsOnly] public string PhoneWork { get; set; }
        [DigitsOnly] public string PhoneMobile { get; set; }
        public int? CallOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
