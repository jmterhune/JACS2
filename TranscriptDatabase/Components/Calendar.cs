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
}