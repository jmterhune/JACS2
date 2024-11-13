using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.JudgeVacation.Components
{
    [TableName("tjc_vacation_vacations")]
    [PrimaryKey("CalendarID", AutoIncrement = true)]
    [Cacheable("JudgeVacations", CacheItemPriority.Default, 20)]
    public class JudgeVacation
    {
        public int CalendarID
        {
            get; set;
        }
        public int JudgeID
        {
            get; set;
        }
        public DateTime StartDate
        {
            get; set;
        }
        public DateTime EndDate
        {
            get; set;
        }
        public int VacationDays
        {
            get; set;
        }
    }
    public class JudgeVacationReport:JudgeVacation
    {
        public string JudgeName
        {
            get; set;
        }
        public int SubTotal
        {
            get; set;
        }
    }
    public class AvailableYears
    {
        public AvailableYears() { }
        public AvailableYears(int year)
        {
            Years = year;
        }
        public int Years
        {
            get; set;
        }

    }
}