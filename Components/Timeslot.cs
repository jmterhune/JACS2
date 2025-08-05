// Filename: Timeslot.cs
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

namespace tjc.Modules.jacs.Components
{
    [TableName("timeslots")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Timeslots", CacheItemPriority.Default, 20)]
    internal class Timeslot
    {
        public long id { get; set; }
        public DateTime end { get; set; }
        public DateTime start { get; set; }
        public string description { get; set; }
        public bool allDay { get; set; }
        public int quantity { get; set; }
        public int duration { get; set; }
        public bool blocked { get; set; }
        public bool public_block { get; set; }
        public string block_reason { get; set; }
        public long? category_id { get; set; }
        public long? template_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [IgnoreColumn]
        public bool available
        {
            get
            {
                if (blocked || public_block)
                    return false;
                else
                {
                    var ctl = new TimeslotEventController();
                    int eventCount = ctl.GetTimeslotEventsByTimeslot(id).Count();
                    return quantity > eventCount;
                }
            }
        }
        [IgnoreColumn]
        public string FormattedStart
        {
            get
            {
                return start.ToString("MM/dd/yyyy @ hh:mm tt");
            }
        }
        [IgnoreColumn]
        public ICollection<TimeslotEvent> TimeslotEvents { get; set; } = new List<TimeslotEvent>();
        [IgnoreColumn]
        public ICollection<TimeslotMotion> Motions { get; set; } = new List<TimeslotMotion>();
    }

    internal class CustomTimeslot : Timeslot
    {
        public int eventCount { get; set; }
    }
    internal class TimeslotListResult
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<TimeslotListItem> Data { get; set; }
    }

    internal class TimeslotListItem : CustomTimeslot
    {
        public string court_name { get; set; }
    }
}