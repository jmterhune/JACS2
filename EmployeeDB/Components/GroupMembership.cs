using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_group_membership")]
    internal class GroupMembership:EmployeeBase
    {
        public int GroupID { get; set; }

        public int EmployeeId { get; set; }
    }
}
