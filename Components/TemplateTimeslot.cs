using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    [TableName("template_timeslots")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("TemplateTimeslots", CacheItemPriority.Default, 20)]
    internal class TemplateTimeslot
    {
        public long id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int duration { get; set; }
        public int quantity { get; set; }
        public bool allDay { get; set; }
        public int day { get; set; }
        public long? court_template_id { get; set; }
        public string description { get; set; }
        public long? category_id { get; set; }
        public bool blocked { get; set; }
        public bool public_block { get; set; }
        public string block_reason { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public string total_length { get { return string.Format("{0} minutes", duration); } }
        [IgnoreColumn]
        public string title
        {
            get
            {
                var startTime = start.ToLocalTime(); // Adjusted for timezone if needed
                var endTime = end.ToLocalTime();
                var diff = (int)(endTime - startTime).TotalMinutes;
                var available = quantity * duration;

                string title = "";
                if (available > diff)
                {
                    var ctl = new TimeslotEventController();
                    var eventCount = ctl.GetTimeslotEventsByTimeSlot(id)?.Count() ?? 0;
                    title = $"{description ?? ""}<br>{Math.Floor((double)diff / duration) - eventCount} Available <br>{quantity - Math.Floor((double)diff / duration)} Overbooked";
                }
                else
                {
                    title = (description ?? block_reason ?? "") + "<br>";
                    if (category_id.HasValue)
                    {
                        title += $"({category.description}) <br>";
                    }
                    title += $"{quantity} Available";
                }
                if (public_block)
                {
                    title += (title != "" ? "<br>" : "") + "Public Blocked";
                }
                return title;
            }
        }

        [IgnoreColumn]
        public string color
        {
            get
            {
                var startTime = start.ToLocalTime();
                var endTime = end.ToLocalTime();
                var diff = (int)(endTime - startTime).TotalMinutes;
                var available = quantity * duration;

                if (available > diff)
                {
                    return blocked ? "#808080" : "#dc3545";
                }
                return blocked ? "#808080" : "#007bff";
            }
        }
        [IgnoreColumn]
        public IEnumerable<Motion> motions
        {
            get
            {
                var ctl = new TimeslotMotionController();
                return ctl.GetMotionsByTemplateTimeslot(id);
            }
        }
        [IgnoreColumn]
        public Category category
        {
            get
            {
                if (category_id.HasValue)
                {
                    var ctl = new CategoryController();
                    return ctl.GetCategory(category_id.Value);
                }
                return null;
            }
        }
    }
}