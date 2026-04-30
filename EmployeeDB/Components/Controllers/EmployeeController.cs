using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
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

        // ------------------------------------------------------------------
        // Bulk-fetch methods filter to IsEmployee = 1.
        //
        // tjc_employee can carry non-employee rows (legacy vendor / contractor
        // records, terminated user shells, etc.). All employee-facing views,
        // dropdowns, and SWN sync feeds want actual employees only — this
        // restriction is enforced in the data layer so every caller benefits
        // without each having to remember the flag.
        //
        // GetEmployee(id) and GetByUserId(id) are intentionally NOT filtered:
        // they are point lookups by primary / foreign key and the caller
        // already knows which row it wants.
        // ------------------------------------------------------------------

        public IEnumerable<EmployeeInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.Find("WHERE IsEmployee = 1");
            }
        }

        public IEnumerable<EmployeeInfo> GetActive()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeInfo>();
                return rep.Find("WHERE IsActive = 1 AND IsEmployee = 1");
            }
        }

        public IEnumerable<EmployeeInfo> Search(string firstName, string lastName, int? departmentId, int? countyId)
        {
            IEnumerable<EmployeeInfo> items;
            using (IDataContext ctx = DataContext.Instance())
            {
                // Search is always scoped to actual employees.
                var conditions = new List<string> { "IsEmployee = 1" };
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

                string sql = "SELECT * FROM tjc_employee WHERE "
                             + string.Join(" AND ", conditions)
                             + " ORDER BY LastName, FirstName";

                items = ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, args.ToArray());
            }
            return items;
        }

        public int CreateEmployee(EmployeeInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
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
            ModelNormalizer.Normalize(item);
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

        /// <summary>Single-column UPDATE used by the Photo tab API endpoint.
        /// Avoids round-tripping the whole row when all we want to change is
        /// the FileId pointer (or clear it back to NULL).</summary>
        public void SetFileId(int employeeId, int? fileId, int actorId = -1)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "UPDATE tjc_employee SET FileId = @0, LastModifiedDate = @1, LastModifiedById = @2 WHERE EmployeeId = @3",
                    (object)fileId ?? DBNull.Value, DateTime.Now, actorId, employeeId);
            }
        }

        public IEnumerable<EmployeeInfo> GetSupervisors()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND (Position LIKE '%Supervisor%'
                                      OR EmployeeId IN (SELECT DISTINCT SupervisorId FROM tjc_employee WHERE SupervisorId IS NOT NULL))
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
