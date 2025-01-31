using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_calendar")]
    [PrimaryKey("CalendarID", AutoIncrement = true)]
    [Cacheable("Calendars", CacheItemPriority.Default, 20)]
    internal class Calendar:EntityBase
    {
        public int EventID { get; set; }

        public string Subject { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string RecurrenceRule { get; set; }

        public int RecurrenceParentID { get; set; }

        public int EventTypeID { get; set; }

        public int DesignationID { get; set; }

        public bool RequestOutstanding { get; set; }
    }
}