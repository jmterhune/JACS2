using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee")]
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    public class EmployeeInfo
    {
        // Column sizes below reflect production (judmansql03.intranet.jud12.local.dbo.tjc_employee).
        public int EmployeeId { get; set; }                            // int (identity PK)
        public int? UserId { get; set; }                               // int
        public int? SupervisorId { get; set; }                         // int
        public int? DepartmentId { get; set; }                         // int
        public int? JobGroupId { get; set; }                           // int
        public int? ClassId { get; set; }                              // int
        public string BadgeNumber { get; set; }                        // nvarchar(50)
        public string Position { get; set; }                           // nvarchar(150)
        public string EmploymentType { get; set; }                     // nvarchar(20)
        public string FirstName { get; set; }                          // nvarchar(50)
        public string LastName { get; set; }                           // nvarchar(50)
        public string MiddleInitial { get; set; }                      // nvarchar(1)
        public string Email { get; set; }                              // nvarchar(250)
        public string PersonalEmail { get; set; }                      // nvarchar(250)
        public string Address { get; set; }                            // nvarchar(300) - AddressLine1 + "\n" + AddressLine2
        public string City { get; set; }                               // nvarchar(50)
        public string State { get; set; }                              // nvarchar(50)
        public string Zip { get; set; }                                // nvarchar(12)
        public int? OfficeLocationId { get; set; }                     // int
        public int? CountyId { get; set; }                             // int
        public int? FileId { get; set; }                               // int
        public DateTime? HireDate { get; set; }                        // datetime
        public DateTime? TerminationDate { get; set; }                 // datetime
        public DateTime? ServiceDate { get; set; }                     // datetime
        public DateTime? BirthDate { get; set; }                       // datetime
        public string Race { get; set; }                               // char(1)
        public string Gender { get; set; }                             // char(1)
        public string JobTitle { get; set; }                           // nvarchar(150)
        public decimal? Salary { get; set; }                           // money
        public decimal? AnnualLeaveBalance { get; set; }               // decimal(18,2)
        public decimal? SickLeaveBalance { get; set; }                 // decimal(18,2)
        [DigitsOnly] public string SocialSecurityNumber { get; set; }  // char(9) - digits only (form mask 999-99-9999)
        public string AgencyOfEmployment { get; set; }                 // char(1)
        public bool? IsActive { get; set; }                            // bit
        public bool IsEmployee { get; set; }                           // bit
        public bool? ManateeAccess { get; set; }                       // bit
        public string SarasotaAccess { get; set; }                     // nvarchar(50)
        public string DesotoAccess { get; set; }                       // nvarchar(50)
        public DateTime CreatedDate { get; set; }                      // datetime
        public int CreatedById { get; set; }                           // int
        public DateTime LastModifiedDate { get; set; }                 // datetime
        public int LastModifiedById { get; set; }                      // int

        // Added in EmployeeDB 0.0.3 to support the HR reports that used to
        // live in Documentation\DROP Participants.xlsx and
        // Documentation\JA seniority.xlsx → Certified Interpreters sheet.
        public DateTime? DropEntryDate { get; set; }                   // datetime
        public DateTime? DropExitDate { get; set; }                    // datetime
        public decimal? DropLeavePayout { get; set; }                  // decimal(8,2)
        public DateTime? CertificationDate { get; set; }               // datetime

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
