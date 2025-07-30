using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
using tjc.Modules.jacs.Components;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_timeslots")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTimeslots", CacheItemPriority.Default, 20)]
    internal class CourtTimeslot
    {
        public long id { get; set; }
        public long? court_id { get; set; }
        public long? timeslot_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public Timeslot Timeslot
        {
            get
            {
                var ctl = new TimeslotController();
                if (timeslot_id.HasValue)
                {
                    return ctl.GetTimeslot(timeslot_id.Value);
                }
                return new Timeslot();
            }
        }
    }
}
