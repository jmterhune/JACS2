using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// One row per employee designated as a supervisor on the Supervisors
    /// admin tab. Drives the Active/Inactive grouping in the Supervisor
    /// dropdown on the Edit Employee form.
    ///
    /// <c>EmployeeId</c> is UNIQUE in the DB — a person is either on the
    /// supervisor list or not. <c>IsActive = false</c> keeps the row but
    /// removes them from the "selectable" group in the dropdown.
    /// </summary>
    [TableName("tjc_supervisor")]
    [PrimaryKey("SupervisorId", AutoIncrement = true)]
    [Cacheable("tjc_supervisor", CacheItemPriority.Default, 20)]
    public class SupervisorInfo
    {
        public int SupervisorId { get; set; }  // int
        public int EmployeeId { get; set; }  // int
        public bool IsActive { get; set; }  // bit
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }

    /// <summary>
    /// Read-side row shape used for the EditEmployee Supervisor dropdown
    /// and the Supervisors admin tab. Joins <c>tjc_supervisor</c> to
    /// <c>tjc_employee</c> so the consumer gets the name and the active
    /// flag in one trip without having to handle FK lookups itself.
    /// </summary>
    public class SupervisorRow
    {
        public int SupervisorId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmployeeActive { get; set; }
        /// <summary>How many <c>tjc_employee</c> rows currently point at this
        /// supervisor via <c>SupervisorId</c>. Drives the "Assigned" column
        /// on the Supervisors admin tab (clickable count → modal).</summary>
        public int AssigneeCount { get; set; }

        // Computed convenience for the admin tab's row rendering. Not
        // mapped — JSON serializers will pick it up automatically.
        public string DisplayName
        {
            get
            {
                var last = (LastName ?? string.Empty).Trim();
                var first = (FirstName ?? string.Empty).Trim();
                if (last.Length == 0 && first.Length == 0) return string.Empty;
                if (last.Length == 0) return first;
                if (first.Length == 0) return last;
                return last + ", " + first;
            }
        }
    }
}
