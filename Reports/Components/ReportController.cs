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

namespace tjc.Modules.Reports.Components
{
    internal class ReportController
    {
        private const string CONN_INTRANET = "Intranet"; //Connection
        private const string CONN_JACS_DESOTO = "JacsDesoto";
        public IEnumerable<BirthDayEmployees> GetBirthDates(int month, string county)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                return ctx.ExecuteQuery<BirthDayEmployees>(System.Data.CommandType.StoredProcedure, "Emp_GetBirthdays", month, county);
            }
        }
        public IEnumerable<ServiceAwardEmployees> GetServiceDates(int month, int reportType, int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                return ctx.ExecuteQuery<ServiceAwardEmployees>(System.Data.CommandType.StoredProcedure, "Emp_GetMonthlyServiceReport", month, reportType, year);
            }
        }
        public IEnumerable<TerminatedEmployees> GetTerminationDates(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                return ctx.ExecuteQuery<TerminatedEmployees>(System.Data.CommandType.StoredProcedure, "Emp_GetTerminatedEmployeesReport", startDate,endDate);
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
    }
}
