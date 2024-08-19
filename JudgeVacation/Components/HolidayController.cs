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
    internal class HolidayController
    {
        public void CreateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Insert(t);
            }
        }

        public void DeleteHoliday(int holidayId)
        {
            var t = GetHoliday(holidayId);
            DeleteHoliday(t);
        }

        public void DeleteHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Holiday> GetHolidays()
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Holiday> GetHolidays(int year)
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Find("WHERE YEAR(HolidayDate)=@0",year);
            }
            return t;
        }
        public IEnumerable<Holiday> GetReportHolidays(int startYear,int endYear)
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Find("WHERE YEAR(HolidayDate)>=@0 AND YEAR(HolidayDate)<=@1", startYear,endYear);
            }
            return t;
        }
        public IEnumerable<Holiday> GetHolidaysByRange(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Find("WHERE HolidayDate BETWEEN @0 and @1", startDate, endDate);
            }
            return t;
        }
        public Holiday GetHoliday(int holidayId)
        {
            Holiday t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.GetById(holidayId);
            }
            return t;
        }

        public void UpdateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Update(t);
            }
        }

        public IEnumerable<AvailableYears> GetYearsAvailable(int judgeId)
        {
            IEnumerable<AvailableYears> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<AvailableYears>(System.Data.CommandType.StoredProcedure, "tjc_vacation_get_judge_years",judgeId);
            }
            return t;
        }
        public IEnumerable<AvailableYears> GetYearsAvailable()
        {
            IEnumerable<AvailableYears> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<AvailableYears>(System.Data.CommandType.StoredProcedure, "tjc_vacation_get_years");
            }
            return t;
        }
        public int GetActualVacationDays(DateTime firstDay, DateTime lastDay)
        {
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay.ToString());
            var span = lastDay - firstDay;
            int actualVacationDays = span.Days + 1;
            var currentDay = firstDay;
            var holidayDates = GetHolidaysByRange(firstDay, lastDay).Select(h => h.HolidayDate);
            while (currentDay <= lastDay)
            {
                if ((int)currentDay.DayOfWeek == 6 | currentDay.DayOfWeek == 0 | holidayDates.Contains(currentDay))
                {
                    actualVacationDays -= 1;
                }
                currentDay = currentDay.AddDays(1d);
            }
            return actualVacationDays;
        }
    }
}
