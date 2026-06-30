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
        public int CalendarID // int
        {
            get; set;
        }
        public int JudgeID // int
        {
            get; set;
        }
        public DateTime StartDate // date
        {
            get; set;
        }
        public DateTime EndDate // date
        {
            get; set;
        }
        public int VacationDays // int
        {
            get; set;
        }
        // PROD table tjc_vacation_vacations also has: Email nvarchar(200) (not mapped in this model)
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