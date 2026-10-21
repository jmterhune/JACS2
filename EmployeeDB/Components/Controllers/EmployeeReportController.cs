using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class EmployeeReportController
    {
        private readonly IHostSettings _hostSettings;

        public EmployeeReportController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        // Every report query restricts to IsEmployee = 1. Non-employee rows
        // (vendors, contractors, terminated user shells) are filtered out
        // at the data layer so each view doesn't have to remember.

        public IEnumerable<EmployeeInfo> GetBirthdays(int month, int countyId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE MONTH(BirthDate) = @0
                                 AND CountyId = @1
                                 AND IsActive = 1
                                 AND IsEmployee = 1
                               ORDER BY DAY(BirthDate)";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, month, countyId);
            }
        }

        public IEnumerable<EmployeeInfo> GetTerminated(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE TerminationDate BETWEEN @0 AND @1
                                 AND IsEmployee = 1
                               ORDER BY TerminationDate DESC";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, startDate, endDate);
            }
        }

        /// <summary>Roster for the Crisis24 Person/HR feed — every active
        /// employee, plus only the inactive ones terminated since
        /// <paramref name="inactiveSinceDate"/> (the last successful export —
        /// see Crisis24ExportLogController). An inactive employee already
        /// reported once drops off the feed on the next run rather than being
        /// resent every time; <paramref name="inactiveSinceDate"/> null (no
        /// export has ever succeeded) includes every inactive employee that
        /// has a termination date, to seed Crisis24 with a baseline. An
        /// inactive employee with no termination date at all never qualifies
        /// — there's no "since" to compare, so they're left off entirely.
        /// See Components\Helpers\Crisis24ExportBuilder.cs, which maps
        /// IsActive onto the feed's Status column ("A"/"I").
        ///
        /// Also called from the monthly Crisis24ExportSchedule task, which
        /// has no HttpContext to resolve an IHostSettings from, so
        /// _hostSettings can be null there — fall back to the classic no-arg
        /// overload rather than risk passing null into
        /// DataContext.Instance(IHostSettings).</summary>
        public IEnumerable<EmployeeInfo> GetAllEmployeesForExport(DateTime? inactiveSinceDate)
        {
            using (IDataContext ctx = _hostSettings != null ? DataContext.Instance(_hostSettings) : DataContext.Instance())
            {
                // ISNULL(@0, '19000101') makes a null cutoff (first-ever
                // export) match every real termination date, rather than
                // "TerminationDate > NULL" silently excluding everyone.
                string sql = @"SELECT * FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND (IsActive = 1
                                      OR (TerminationDate IS NOT NULL
                                          AND TerminationDate > ISNULL(@0, '19000101')))
                               ORDER BY LastName, FirstName";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, (object)inactiveSinceDate ?? DBNull.Value);
            }
        }

        public IEnumerable<EmployeeInfo> GetSupervisorList()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                string sql = @"SELECT DISTINCT s.*
                               FROM tjc_employee s
                               INNER JOIN tjc_employee e ON e.SupervisorId = s.EmployeeId
                               WHERE s.IsEmployee = 1 AND e.IsEmployee = 1
                               ORDER BY s.LastName, s.FirstName";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql);
            }
        }
    }
}
