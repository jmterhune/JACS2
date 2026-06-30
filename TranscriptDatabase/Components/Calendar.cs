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
        public int EventID { get; set; }  // int

        public string Subject { get; set; }  // nvarchar(255)

        public DateTime StartTime { get; set; }  // smalldatetime

        public DateTime EndTime { get; set; }  // smalldatetime

        public int EventTypeID { get; set; }  // int

        public int DesignationID { get; set; }  // int

        public bool RequestOutstanding { get; set; }  // bit
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
        public int EventID { get; set; }  // int

        public string Subject { get; set; }  // nvarchar(255)

        public DateTime StartTime { get; set; }  // smalldatetime

        public DateTime EndTime { get; set; }  // smalldatetime

        public int EventTypeID { get; set; }  // int
        public string County { get; set; }  // nvarchar(50)

        public int DesignationID { get; set; }  // int
        public string CourtReporterName { get; set; }  // int (view column is int)
        public bool RequestOutstanding { get; set; }  // bit
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