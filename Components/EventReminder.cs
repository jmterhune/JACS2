using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("event_reminders")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("EventReminders", CacheItemPriority.Default, 20)]
    internal class EventReminder
    {
        public int id { get; set; }
        public string email { get; set; }
        public DateTime event_start { get; set; }
        public long event_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}