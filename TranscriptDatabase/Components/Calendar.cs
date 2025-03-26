using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_calendar")]
    [PrimaryKey("EventID", AutoIncrement = true)]
    [Cacheable("Calendars", CacheItemPriority.Default, 20)]
    internal class Calendar:EntityBase
    {
        public int EventID { get; set; }

        public string Subject { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int EventTypeID { get; set; }

        public int DesignationID { get; set; }

        public bool RequestOutstanding { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(EventTypes))]
        public EventTypes EventType
        {
            get
            {
                return (EventTypes)this.EventTypeID;
            }
            set
            {
                this.EventTypeID = (int)value;
            }
        }
        [IgnoreColumn]
        public string EventTypeName
        {
            get
            {
                    return Enumerations.GetEnumDescription(EventType);
            }
        }      
    }
    [TableName("tjc_rec_calendar_events")]
    [PrimaryKey("EventID", AutoIncrement = true)]
    internal class CalendarListItem : EntityBase
    {
        public int EventID { get; set; }

        public string Subject { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int EventTypeID { get; set; }
        public string County { get; set; }

        public int DesignationID { get; set; }
        public string CourtReporterName { get; set; }
        public bool RequestOutstanding { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(EventTypes))]
        public EventTypes EventType
        {
            get
            {
                return (EventTypes)this.EventTypeID;
            }
            set
            {
                this.EventTypeID = (int)value;
            }
        }
        [IgnoreColumn]
        public string EventTypeName
        {
            get
            {
                return Enumerations.GetEnumDescription(EventType);
            }
        }
    }
    internal class CalendarEvent
    {
        public string EventList { get; set; }
        public bool Muted { get; set; }
        public int Day { get; set; }
        public string DayOfWeek { get; set; }
        public string WeekBreak
        {
            get
            {
                if (DayOfWeek != "Saturday")
                { return ""; }
                else { return "<div class=\"w-100\"></div>"; }
            }
        }
    }
}