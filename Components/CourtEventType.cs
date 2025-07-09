using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_event_types")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtEventTypes", CacheItemPriority.Default, 20)]
    internal class CourtEventType
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public long event_type_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
