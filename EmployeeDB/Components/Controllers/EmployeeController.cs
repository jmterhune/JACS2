using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class EmployeeController
    {
        public EmployeeInfo GetEmployee(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<EmployeeInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.Get();
            }
        }

        public IEnumerable<EmployeeInfo> GetActive()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.Find("WHERE IsActive = 1");
            }
        }

        public IEnumerable<EmployeeInfo> Search(string firstName, string lastName, int? departmentId, int? countyId)
        {
            IEnumerable<EmployeeInfo> items;
            using (IDataContext ctx = DataContext.Instance())
            {
                var conditions = new List<string>();
                var args = new List<object>();
                int paramIndex = 0;

                if (!string.IsNullOrEmpty(firstName))
                {
                    conditions.Add(string.Format("FirstName LIKE @{0}", paramIndex++));
                    args.Add("%" + firstName + "%");
                }
                if (!string.IsNullOrEmpty(lastName))
                {
                    conditions.Add(string.Format("LastName LIKE @{0}", paramIndex++));
                    args.Add("%" + lastName + "%");
                }
                if (departmentId.HasValue && departmentId.Value > 0)
                {
                    conditions.Add(string.Format("DepartmentId = @{0}", paramIndex++));
                    args.Add(departmentId.Value);
                }
                if (countyId.HasValue && countyId.Value > 0)
                {
                    conditions.Add(string.Format("CountyId = @{0}", paramIndex++));
                    args.Add(countyId.Value);
                }

                string sql = "SELECT * FROM tjc_employee";
                if (conditions.Any())
                    sql += " WHERE " + string.Join(" AND ", conditions);
                sql += " ORDER BY LastName, FirstName";

                items = ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, args.ToArray());
            }
            return items;
        }

        public int CreateEmployee(EmployeeInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                rep.Insert(item);
            }
            return item.EmployeeId;
        }

        public void UpdateEmployee(EmployeeInfo item, int userId = -1)
        {
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                rep.Update(item);
            }
        }

        public void DeleteEmployee(int id)
        {
            var item = GetEmployee(id);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<EmployeeInfo>();
                    rep.Delete(item);
                }
            }
        }

        public void ChangeStatus(int employeeId, bool active, int userId = -1)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "UPDATE tjc_employee SET IsActive = @0, LastModifiedDate = @1, LastModifiedById = @2 WHERE EmployeeId = @3",
                    active, DateTime.Now, userId, employeeId);
            }
        }

        public void SetUserId(int employeeId, int userId, int actorId = -1)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "UPDATE tjc_employee SET UserId = @0, LastModifiedDate = @1, LastModifiedById = @2 WHERE EmployeeId = @3",
                    userId, DateTime.Now, actorId, employeeId);
            }
        }

        public IEnumerable<EmployeeInfo> GetSupervisors()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE Position LIKE '%Supervisor%'
                                  OR EmployeeId IN (SELECT DISTINCT SupervisorId FROM tjc_employee WHERE SupervisorId IS NOT NULL)
                               ORDER BY LastName, FirstName";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql);
            }
        }

        public EmployeeInfo GetByUserId(int userId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.Find("WHERE UserId = @0", userId).FirstOrDefault();
            }
        }
    }
}
