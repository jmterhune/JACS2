using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class GroupMembershipController
    {
        public void AddMembership(int groupId, int employeeId, int userId = -1)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"IF NOT EXISTS (SELECT 1 FROM tjc_employee_group_membership WHERE GroupId = @0 AND EmployeeId = @1)
                               INSERT INTO tjc_employee_group_membership
                                   (GroupId, EmployeeId, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
                               VALUES (@0, @1, @2, @3, @2, @3)";
                ctx.Execute(CommandType.Text, sql, groupId, employeeId, DateTime.Now, userId);
            }
        }

        public void DeleteMembership(int groupId, int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "DELETE FROM tjc_employee_group_membership WHERE GroupId = @0 AND EmployeeId = @1",
                    groupId, employeeId);
            }
        }

        public void ClearMembership(int groupId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "DELETE FROM tjc_employee_group_membership WHERE GroupId = @0",
                    groupId);
            }
        }

        public IEnumerable<GroupMembershipInfo> GetForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupMembershipInfo>();
                return rep.Find("WHERE EmployeeId = @0", employeeId);
            }
        }

        public IEnumerable<GroupMembershipInfo> GetForGroup(int groupId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupMembershipInfo>();
                return rep.Find("WHERE GroupId = @0", groupId);
            }
        }
    }
}
