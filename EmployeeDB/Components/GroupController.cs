/*
' Copyright (c) 2023 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class GroupController
    {
        public void CreateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Insert(t);
            }
        }

        public void DeleteGroup(int groupId)
        {
            var t = GetGroup(groupId);
            DeleteGroup(t);
        }

        public void DeleteGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Group> GetGroups()
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Group> GetSwnGroups()
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.Find("Where IsSwnGroup = 1");
            }
            return t;
        }
        
        public IEnumerable<Group> GetGroups(int groupType)
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.Find("Where GroupType=@0",groupType);
            }
            return t;
        }
        public IEnumerable<Group> GetEmployeeSwnGroups(long employeeId)
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t= ctx.ExecuteQuery<Group>(System.Data.CommandType.Text, "select g.* from tjc_gl_group g inner join tjc_employee_group_membership gm on g.GroupId=gm.GroupId Where g.IsSwnGroup=1 And gm.EmployeeId=@0", employeeId);
            }
            return t;
        }
        public int GetMaxGroup()
        {
            int groupCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                groupCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "Select Max(GroupCount) from (select count(employeeId) as GroupCount from tjc_gl_group g inner join tjc_employee_group_membership gm on g.GroupId=gm.GroupId Where g.IsSwnGroup=1 Group by gm.EmployeeId) t");

            }
            return groupCount;
        }
        public Group GetGroup(int groupId)
        {
            Group t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.GetById(groupId);
            }
            return t;
        }

        public void UpdateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Update(t);
            }
        }

        public IEnumerable<Group> GetGroupMemberships(long employeeId)
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Group>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_group_memberships", employeeId);
            }
            return t;
        }
        public IEnumerable<string> GetSwnGroupMembers(int groupId)
        {
            IEnumerable<string> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<string>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_swn_group_members", groupId);
            }
            return t;
        }
        public IEnumerable<Group> GetGroupsExcludingMembership(long employeeId)
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Group>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_groups_membership_excluded", employeeId);
            }
            return t;
        }
        public GroupMembership GetGroupMembership(long employeeId,int groupId)
        {
            GroupMembership t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<GroupMembership>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_group_membership", employeeId,groupId).FirstOrDefault();
            }
            return t;
        }
        public void CreateGroupMembership(GroupMembership groupMembership)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_employee_create_group_membership", groupMembership.EmployeeId, groupMembership.GroupId,groupMembership.CreatedById);
            }
        }
        public void DeleteGroupMembership(GroupMembership groupMembership)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_employee_delete_group_membership", groupMembership.EmployeeId, groupMembership.GroupId);
            }
        }

    }
}
