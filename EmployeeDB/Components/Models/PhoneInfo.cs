using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

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
        [DigitsOnly] public string PhoneNumber { get; set; }
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

        /// <summary>Description of the OfficeLocation row, populated by the
        /// API layer on read so the JS can render the column directly without
        /// a separate id-to-name lookup. Excluded from PetaPoco INSERT / UPDATE
        /// / SELECT — there is no LocationName column in tjc_employee_phone.</summary>
        [IgnoreColumn]
        public string LocationName { get; set; }
    }
}
