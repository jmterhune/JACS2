using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("event_statuses")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("EventStatuss", CacheItemPriority.Default, 20)]
    internal class EventStatus
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}