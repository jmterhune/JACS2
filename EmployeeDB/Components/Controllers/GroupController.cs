using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    /// <summary>
    /// Read + write access to <c>tjc_gl_group</c> (the global Groups /
    /// Departments table). The table is shared across modules; modifications
    /// from this module are gated to DNN site administrators in the API
    /// controller layer (see <c>DepartmentsController</c>).
    /// </summary>
    public class GroupController
    {
        public IEnumerable<GroupInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.Get();
            }
        }

        public GroupInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<GroupInfo> GetSwnGroups()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.Find("WHERE IsSwnGroup = 1");
            }
        }

        public int Create(GroupInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedByID = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedByID = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                rep.Insert(item);
            }
            return item.GroupID;
        }

        public void Update(GroupInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve audit columns from the existing row (JSON payloads come
            // in with DateTime.MinValue / 0 which SQL Server datetime rejects).
            var existing = GetById(item.GroupID);
            if (existing != null)
            {
                item.CreatedDate = existing.CreatedDate;
                item.CreatedByID = existing.CreatedByID;
            }
            else
            {
                item.CreatedDate = DateTime.Now;
                item.CreatedByID = userId;
            }
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedByID = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                rep.Update(item);
            }
        }

        /// <summary>Returns the count of dependent records that would be
        /// orphaned by deleting the given group — used by the Departments tab
        /// to refuse a delete cleanly with an explanation.</summary>
        public int CountDependents(int groupId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var inEmployees = ctx.ExecuteScalar<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM tjc_employee WHERE DepartmentId = @0", groupId);
                var inMembership = ctx.ExecuteScalar<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM tjc_employee_group_membership WHERE GroupId = @0", groupId);
                return inEmployees + inMembership;
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                rep.Delete(item);
            }
        }
    }
}
