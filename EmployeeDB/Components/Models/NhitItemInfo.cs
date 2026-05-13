using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// One row in the New Hire IT Worksheet's admin-managed checkbox catalog.
    /// Items are grouped by Category — exactly three: "Software", "Intranet",
    /// "Judicial" — and rendered as checkboxes in the corresponding section
    /// of the form.
    ///
    /// Notes (when present) appear next to the item name as guidance text,
    /// e.g. "Check with Mike", "HR to send request to Tiffany Hammill".
    /// </summary>
    [TableName("tjc_nhit_item")]
    [PrimaryKey("NhitItemId", AutoIncrement = true)]
    public class NhitItemInfo
    {
        public int NhitItemId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
