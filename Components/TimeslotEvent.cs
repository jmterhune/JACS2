using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("timeslot_events")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("TimeslotEvents", CacheItemPriority.Default, 20)]
    internal class TimeslotEvent
    {
        public long id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public long? event_id { get; set; }
        public long? timeslot_id { get; set; }
        [IgnoreColumn]
        public Event Event { get; set; } = new Event();
        [IgnoreColumn]
        public Timeslot Timeslot { get; set; } = new Timeslot();
    }
}