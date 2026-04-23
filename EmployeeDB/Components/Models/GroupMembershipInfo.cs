using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    // Composite PK (GroupId, EmployeeId) - PetaPoco supports this via comma-separated primary keys.
    [TableName("tjc_employee_group_membership")]
    [PrimaryKey("GroupId", AutoIncrement = false)]
    public class GroupMembershipInfo
    {
        public int GroupId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
