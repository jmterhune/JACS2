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
    /// Read + write access to <c>tjc_supervisor</c>. The Supervisors admin
    /// tab on EmployeeList calls into this via the SupervisorsController
    /// API layer; the EditEmployee dropdown calls
    /// <see cref="EmployeeController.GetSupervisors"/> which joins this
    /// table to <c>tjc_employee</c> for display.
    /// </summary>
    public class SupervisorController
    {
        /// <summary>Roster join used by both the admin tab grid and the
        /// EditEmployee dropdown. Returns one row per supervisor with the
        /// employee's name, the row's IsActive flag, and a count of how
        /// many <c>tjc_employee</c> rows currently point at this
        /// supervisor — that count powers the "Assignees" column on the
        /// admin tab (clickable to open a modal listing the names).
        /// Sorted by name.</summary>
        public IEnumerable<SupervisorRow> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var sql = @"SELECT s.SupervisorId, s.EmployeeId,
                                   e.FirstName, e.LastName,
                                   s.IsActive,
                                   ISNULL(e.IsActive, 0) AS IsEmployeeActive,
                                   (SELECT COUNT(*)
                                    FROM   tjc_employee a
                                    WHERE  a.SupervisorId = s.EmployeeId
                                      AND  a.IsEmployee   = 1) AS AssigneeCount
                            FROM   tjc_supervisor s
                            INNER JOIN tjc_employee e ON e.EmployeeId = s.EmployeeId
                            WHERE  e.IsEmployee = 1
                            ORDER BY e.LastName, e.FirstName";
                return ctx.ExecuteQuery<SupervisorRow>(CommandType.Text, sql);
            }
        }

        /// <summary>Returns the employees currently assigned to a supervisor —
        /// every <c>tjc_employee</c> row whose <c>SupervisorId</c> matches the
        /// supervisor's <c>EmployeeId</c>. Used by the assignees modal on the
        /// Supervisors admin tab. Terminated employees are included (the
        /// modal flags them with a badge) since "who used to report to this
        /// supervisor" is part of the picture HR needs.</summary>
        public IEnumerable<EmployeeInfo> GetAssignees(int supervisorEmployeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text,
                    @"SELECT *
                      FROM   tjc_employee
                      WHERE  SupervisorId = @0
                        AND  IsEmployee   = 1
                      ORDER BY LastName, FirstName",
                    supervisorEmployeeId);
            }
        }

        public SupervisorInfo GetById(int supervisorId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupervisorInfo>();
                return rep.GetById(supervisorId);
            }
        }

        public SupervisorInfo GetByEmployeeId(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupervisorInfo>();
                return rep.Find("WHERE EmployeeId = @0", employeeId).FirstOrDefault();
            }
        }

        public int Create(SupervisorInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupervisorInfo>();
                rep.Insert(item);
            }
            return item.SupervisorId;
        }

        public void Update(SupervisorInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve the audit columns from the existing row (the JSON-bound
            // payload from the API layer comes in with DateTime.MinValue / 0
            // for these, which SQL Server datetime rejects).
            var existing = GetById(item.SupervisorId);
            if (existing != null)
            {
                item.CreatedDate = existing.CreatedDate;
                item.CreatedById = existing.CreatedById;
                // EmployeeId is intentionally immutable post-create — the only
                // mutable field on a supervisor row is IsActive. Pin it here
                // so a stray payload can't move an existing row to point at a
                // different employee.
                item.EmployeeId = existing.EmployeeId;
            }
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupervisorInfo>();
                rep.Update(item);
            }
        }

        /// <summary>How many employees currently have this supervisor's
        /// EmployeeId set as their <c>SupervisorId</c>. The API layer uses
        /// this to refuse a delete with a clean explanation rather than
        /// failing with an obscure FK / orphaning the references.</summary>
        public int CountAssignedEmployees(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteScalar<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM tjc_employee WHERE SupervisorId = @0",
                    employeeId);
            }
        }

        public void Delete(int supervisorId)
        {
            var item = GetById(supervisorId);
            if (item == null) return;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupervisorInfo>();
                rep.Delete(item);
            }
        }

        /// <summary>Marks the supervisor row for the given employee inactive
        /// (no-op if the row doesn't exist or is already inactive). Called
        /// from <see cref="EmployeeController.UpdateEmployee"/> when an
        /// employee transitions from active to inactive so the Supervisor
        /// dropdown moves them into the disabled "Inactive" group
        /// automatically.</summary>
        public void DeactivateForEmployee(int employeeId, int userId)
        {
            var existing = GetByEmployeeId(employeeId);
            if (existing == null || !existing.IsActive) return;
            existing.IsActive = false;
            Update(existing, userId);
        }
    }
}
