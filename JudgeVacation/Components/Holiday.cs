using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.JudgeVacation.Components
{
    [TableName("tjc_vacation_holidays")]
    [PrimaryKey("HolidayID", AutoIncrement = true)]
    [Cacheable("Holidays", CacheItemPriority.Default, 20)]

    internal class Holiday
    {
        public int HolidayID // int
        {
            get; set;
        }

        public DateTime HolidayDate // date
        {
            get; set;
        }

        public string Description // nvarchar(50)
        {
            get; set;
        }
    }
}