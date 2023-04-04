using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_group_membership")]
    internal class GroupMembership:EmployeeBase
    {
        public int GroupId { get; set; }

        public int EmployeeId { get; set; }
    }
}
