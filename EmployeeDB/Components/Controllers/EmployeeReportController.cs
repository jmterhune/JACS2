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
        // Every report query restricts to IsEmployee = 1. Non-employee rows
        // (vendors, contractors, terminated user shells) are filtered out
        // at the data layer so each view doesn't have to remember.

        public IEnumerable<EmployeeInfo> GetBirthdays(int month, int countyId)
        {
            using (IDataContext ctx = DataContext.Instance())
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
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE TerminationDate BETWEEN @0 AND @1
                                 AND IsEmployee = 1
                               ORDER BY TerminationDate DESC";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql, startDate, endDate);
            }
        }

        public IEnumerable<EmployeeInfo> GetActiveEmployeesForSwn()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT * FROM tjc_employee
                               WHERE IsActive = 1 AND IsEmployee = 1
                               ORDER BY LastName, FirstName";
                return ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, sql);
            }
        }

        public IEnumerable<EmployeeInfo> GetSupervisorList()
        {
            using (IDataContext ctx = DataContext.Instance())
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
