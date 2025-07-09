using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
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
    }
}
