using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("template_timeslots")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("TemplateTimeslots", CacheItemPriority.Default, 20)]
    internal class TemplateTimeslot
    {
        public long id { get; set; }
        public DateTime end { get; set; }
        public DateTime start { get; set; }
        public int day { get; set; }
        public string description { get; set; }
        public bool allDay { get; set; }
        public int quantity { get; set; }
        public int duration { get; set; }
        public bool blocked { get; set; }
        public bool public_block { get; set; }
        public string block_reason { get; set; }
        public long? category_id { get; set; }
        public long? court_template_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}