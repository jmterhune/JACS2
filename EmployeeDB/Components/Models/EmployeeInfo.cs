using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee")]
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    public class EmployeeInfo
    {
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobGroupId { get; set; }
        public int? ClassId { get; set; }
        public string BadgeNumber { get; set; }
        public string Position { get; set; }
        public string EmploymentType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int? OfficeLocationId { get; set; }
        public int? CountyId { get; set; }
        public int? FileId { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public decimal? Salary { get; set; }
        public decimal? AnnualLeaveBalance { get; set; }
        public decimal? SickLeaveBalance { get; set; }
        [DigitsOnly] public string SocialSecurityNumber { get; set; }
        public string AgencyOfEmployment { get; set; }
        public bool? IsActive { get; set; }
        public bool IsEmployee { get; set; }
        public bool? ManateeAccess { get; set; }
        public string SarasotaAccess { get; set; }
        public string DesotoAccess { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }

        // Added in EmployeeDB 0.0.3 to support the HR reports that used to
        // live in Documentation\DROP Participants.xlsx and
        // Documentation\JA seniority.xlsx → Certified Interpreters sheet.
        public DateTime? DropEntryDate { get; set; }
        public DateTime? DropExitDate { get; set; }
        public decimal? DropLeavePayout { get; set; }
        public DateTime? CertificationDate { get; set; }

        [IgnoreColumn]
        public string DisplayName
        {
            get
            {
                var first = FirstName ?? string.Empty;
                var last = LastName ?? string.Empty;
                return (last + ", " + first).Trim(',', ' ');
            }
        }
    }
}
