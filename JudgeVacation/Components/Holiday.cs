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
        public int HolidayID
        {
            get; set;
        }

        public DateTime HolidayDate
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }
}