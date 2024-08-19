/*
' Copyright (c) 2024 Joe Terhune
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

namespace tjc.Modules.JudgeVacation.Components
{
    internal class JudgeVacationController
    {
        public void CreateJudgeVacation(JudgeVacation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                rep.Insert(t);
            }
        }

        public void DeleteJudgeVacation(int calendarId)
        {
            var t = GetJudgeVacation(calendarId);
            DeleteJudgeVacation(t);
        }

        public void DeleteJudgeVacation(JudgeVacation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JudgeVacation> GetJudgeVacations()
        {
            IEnumerable<JudgeVacation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<JudgeVacation> GetJudgeVacations(int judgeId,int year)
        {
            IEnumerable<JudgeVacation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                t = rep.Find("WHERE JudgeID=@0 AND YEAR(StartDate)=@1",judgeId,year);
            }
            return t;
        }

        public JudgeVacation GetJudgeVacation(int calendarId)
        {
            JudgeVacation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                t = rep.GetById(calendarId);
            }
            return t;
        }

        public void UpdateJudgeVacation(JudgeVacation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeVacation>();
                rep.Update(t);
            }
        }
        public IEnumerable<JudgeVacation> GetVacationReport(DateTime startDate,DateTime endDate)
        {
            IEnumerable<JudgeVacation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<JudgeVacation>(System.Data.CommandType.StoredProcedure, "tjc_vacation_judge_report", startDate,endDate);
            }
            return t;
        }
        public IEnumerable<JudgeVacation> GetVacationReportByJudge(DateTime startDate, DateTime endDate,int judgeId)
        {
            IEnumerable<JudgeVacation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<JudgeVacation>(System.Data.CommandType.StoredProcedure, "tjc_vacation_report_by_judge", startDate, endDate,judgeId);
            }
            return t;
        }
      
    }
}
