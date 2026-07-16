using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_phone")]
    [PrimaryKey("PhoneId", AutoIncrement = true)]
    public class PhoneInfo
    {
        public long PhoneId { get; set; }  // bigint
        public int EmployeeId { get; set; }  // int
        public int? OfficeLocationId { get; set; }  // int
        public string PhoneType { get; set; }  // nvarchar(20)
        [DigitsOnly] public string PhoneNumber { get; set; }  // nvarchar(20) — stored as raw digits (mask stripped)
        public string Extension { get; set; }  // nvarchar(10)
        public bool IsMain { get; set; }  // bit
        public int? PhoneCascade { get; set; }  // int
        public bool SwnText { get; set; }  // bit
        public bool SwnCall { get; set; }  // bit
        public bool SwnExcludeExtension { get; set; }  // bit
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int

        /// <summary>Description of the OfficeLocation row, populated by the
        /// API layer on read so the JS can render the column directly without
        /// a separate id-to-name lookup. Excluded from PetaPoco INSERT / UPDATE
        /// / SELECT — there is no LocationName column in tjc_employee_phone.</summary>
        [IgnoreColumn]
        public string LocationName { get; set; }
    }
}
