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
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.Reports.Components
{
    internal class ReportController
    {
        private const string CONN_JACS_DESOTO = "JacsDesoto";

        /// <summary>
        /// Returns Birthday Report rows for the given month and county.
        /// <paramref name="countyId"/> = 0 means "all counties".
        ///
        /// The SP signature was migrated from (@month int, @county varchar)
        /// to (@month int, @countyId int) when tjc_employee gained the
        /// CountyId FK. The old call passed a county-name string here,
        /// which silently coerced to 0 and made the report always empty —
        /// then the .ascx blew up on grdReport.HeaderRow being null.
        /// </summary>
        public IEnumerable<BirthDayEmployees> GetBirthDates(int month, int countyId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<BirthDayEmployees>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_employee_birthdays",
                    month, countyId);
            }
        }

        /// <summary>Lookup list for the Birthday Report's County dropdown.</summary>
        public IEnumerable<CountyLookup> GetCounties()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<CountyLookup>()
                    .Get()
                    .OrderBy(c => c.CountyName)
                    .ToList();
            }
        }
        public IEnumerable<ServiceAwardEmployees> GetServiceDates(int month, int reportType, int year)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<ServiceAwardEmployees>(System.Data.CommandType.StoredProcedure, "tjc_employee_monthly_service_report", month, reportType, year);
            }
        }
        public IEnumerable<TerminatedEmployees> GetTerminationDates(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<TerminatedEmployees>(System.Data.CommandType.StoredProcedure, "tjc_employee_terminated_employees_report", startDate,endDate);
            }
        }
        public IEnumerable<JacsJudge> GetJacsJudges(string county)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS_DESOTO))
            {
                return ctx.ExecuteQuery<JacsJudge>(System.Data.CommandType.StoredProcedure, "tjc_get_judges", county);
            }
        }
        public IEnumerable<WeekdayHearing> GetWeekdayHearingCounts(string county,DateTime startDate,DateTime endDate,string judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS_DESOTO))
            {
                return ctx.ExecuteQuery<WeekdayHearing>(System.Data.CommandType.StoredProcedure, "jacs.tjc_get_most_popular_day_schedule", county,startDate,endDate,judgeId);
            }
        }

        // ----------------------------------------------------------------
        //  Employee Reports (new in v1.0.x — replaces HR Excel sheets that
        //  used to live in EmployeeDB\Documentation\)
        //
        //  These queries hit tjc_employee directly via inline SQL rather
        //  than going through stored procedures so the Reports module
        //  doesn't need a coordinated SP deploy alongside this change.
        // ----------------------------------------------------------------

        /// <summary>DROP (Deferred Retirement Option Program) participants —
        /// any employee with DropEntryDate set. Active filter optional;
        /// the legacy Excel includes both still-in-DROP and already-exited
        /// rows so we default to all participants and let the view sort. </summary>
        public IEnumerable<DropParticipantRow> GetDropParticipants(bool includeInactive = true)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var sql = @"SELECT FirstName, LastName, DropEntryDate, DropExitDate,
                                   TerminationDate, DropLeavePayout, JobTitle, IsActive
                            FROM tjc_employee
                            WHERE DropEntryDate IS NOT NULL
                              AND IsEmployee = 1"
                          + (includeInactive ? string.Empty : " AND IsActive = 1")
                          + " ORDER BY DropEntryDate, LastName, FirstName";
                return ctx.ExecuteQuery<DropParticipantRow>(System.Data.CommandType.Text, sql);
            }
        }

        /// <summary>Judicial Assistants ordered by HireDate (seniority).
        /// Filter is JobTitle containing 'Judicial Assistant' OR Class /
        /// Department name containing same. Active employees only by
        /// default — terminated JAs aren't seniority candidates.</summary>
        public IEnumerable<SeniorityRow> GetJudicialAssistantSeniority(bool includeInactive = false)
        {
            return GetSeniorityRoster("Judicial Assistant", includeInactive);
        }

        /// <summary>Trial Court Staff Attorneys (Law Clerks) ordered by HireDate.
        /// Matches JobTitle / Class containing 'Law Clerk' or 'Staff Attorney'.</summary>
        public IEnumerable<SeniorityRow> GetStaffAttorneySeniority(bool includeInactive = false)
        {
            return GetSeniorityRoster("Law Clerk|Staff Attorney", includeInactive);
        }

        /// <summary>Certified Court Interpreters ordered by CertificationDate.
        /// Filter requires CertificationDate IS NOT NULL — that's the
        /// canonical "they're a certified interpreter" signal.</summary>
        public IEnumerable<SeniorityRow> GetCertifiedInterpreterSeniority(bool includeInactive = false)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var sql = @"SELECT e.EmployeeId, e.FirstName, e.LastName, e.JobTitle,
                                   c.ClassName, g.GroupName AS DepartmentName,
                                   e.HireDate AS StartDate, e.CertificationDate,
                                   e.IsActive
                            FROM tjc_employee e
                            LEFT JOIN tjc_employee_class c ON c.ClassId = e.ClassId
                            LEFT JOIN tjc_gl_group       g ON g.GroupID = e.DepartmentId
                            WHERE e.CertificationDate IS NOT NULL
                              AND e.IsEmployee = 1"
                          + (includeInactive ? string.Empty : " AND e.IsActive = 1")
                          + " ORDER BY e.CertificationDate, e.LastName, e.FirstName";
                return ctx.ExecuteQuery<SeniorityRow>(System.Data.CommandType.Text, sql);
            }
        }

        /// <summary>Helper for the JA / Staff Attorney roster reports.
        /// <paramref name="rolePattern"/> is a pipe-delimited list of
        /// substrings to match against JobTitle / ClassName / DepartmentName
        /// (case-insensitive).</summary>
        private IEnumerable<SeniorityRow> GetSeniorityRoster(string rolePattern, bool includeInactive)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var tokens = (rolePattern ?? string.Empty).Split('|');
                // Build a WHERE that matches if ANY of the tokens appears in
                // JobTitle / ClassName / DepartmentName. PetaPoco @0..@N
                // parameter binding keeps it safe.
                var ors = new List<string>();
                var args = new List<object>();
                int p = 0;
                foreach (var t in tokens)
                {
                    var like = "%" + (t ?? string.Empty).Trim() + "%";
                    ors.Add("(e.JobTitle LIKE @" + p + " OR c.ClassName LIKE @" + (p + 1) + " OR g.GroupName LIKE @" + (p + 2) + ")");
                    args.Add(like); args.Add(like); args.Add(like);
                    p += 3;
                }
                var orClause = ors.Count > 0 ? string.Join(" OR ", ors) : "1=0";

                var sql = @"SELECT e.EmployeeId, e.FirstName, e.LastName, e.JobTitle,
                                   c.ClassName, g.GroupName AS DepartmentName,
                                   e.HireDate AS StartDate, e.CertificationDate,
                                   e.IsActive
                            FROM tjc_employee e
                            LEFT JOIN tjc_employee_class c ON c.ClassId = e.ClassId
                            LEFT JOIN tjc_gl_group       g ON g.GroupID = e.DepartmentId
                            WHERE e.IsEmployee = 1
                              AND (" + orClause + ")"
                          + (includeInactive ? string.Empty : " AND e.IsActive = 1")
                          + " ORDER BY e.HireDate, e.LastName, e.FirstName";
                return ctx.ExecuteQuery<SeniorityRow>(System.Data.CommandType.Text, sql, args.ToArray());
            }
        }
    }
}
