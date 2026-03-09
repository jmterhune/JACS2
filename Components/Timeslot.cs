// Filename: Timeslot.cs
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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
        public long? courtroom_id { get; set; }
        public long? template_id { get; set; }
        public long? court_template_order_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [IgnoreColumn]
        public int template_week_order
        {
            get
            {
                int weekDay = 0;
                if (court_template_order_id.HasValue)
                {
                    var ctl = new CourtTemplateOrderController();
                    var cto = ctl.GetCourtTemplateOrder(court_template_order_id.Value);
                    if (cto != null && cto.auto && cto.order.HasValue)
                    {
                        weekDay = cto.order.Value;
                    }
                    else if (cto != null && !cto.auto && cto.date.HasValue)
                    {
                        weekDay = DateTimeExtensions.GetWeekOfMonth(cto.date.Value);
                    }
                }
                return weekDay;
            }
        }
        [IgnoreColumn]
        public IEnumerable<Event> events
        {
            get
            {
                var ctl = new EventController();
                return ctl.GetEventsByTimeslot(id);
            }
        }
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
        public string formatted_start
        {
            get
            {
                return start.ToString("MM/dd/yyyy @ hh:mm tt");
            }
        }
        [IgnoreColumn]
        public ICollection<TimeslotEvent> timeslot_events { get; set; } = new List<TimeslotEvent>();
        [IgnoreColumn]
        public ICollection<TimeslotMotion> motions { get; set; } = new List<TimeslotMotion>();
        [IgnoreColumn]
        public Courtroom Courtroom
        {
            get
            {

                if (this.courtroom_id.HasValue)
                {
                    var ctl = new CourtroomController();
                    var et = ctl.GetCourtroom(courtroom_id.Value);
                    if (et != null)
                        return et;
                }
                return new Courtroom();
            }
        }
        [IgnoreColumn]
        public string title
        {
            get
            {
                var start = this.start;
                var end = this.end;
                double diff = (end - start).TotalMinutes;
                int available = quantity * duration;
                int eventsCount = events?.Count() ?? 0;
                string title = string.Empty;

                if (eventsCount * duration > diff && eventsCount != quantity)
                {
                    if (blocked)
                    {
                        if (public_block)
                        {
                            title = "Public Blocked <br>" + (!string.IsNullOrEmpty(block_reason) ? block_reason : description);
                        }
                        else
                        {
                            title = "Blocked <br>" + (!string.IsNullOrEmpty(block_reason) ? block_reason : description);
                        }
                    }
                    else
                    {
                        int availableCount = (int)Math.Floor(diff / duration) - eventsCount;
                        string countStr = availableCount > 0
                            ? availableCount + " Available <br> " + (quantity - (int)Math.Floor(diff / duration)) + " Overbooked"
                            : (eventsCount - (int)Math.Floor(diff / duration)) + " Overbooked";
                    }
                    title += "<br>";
                    if (events != null)
                    {
                        foreach (var evt in events)
                        {
                            title += evt.case_num + "<br>";
                        }
                    }
                }
                else
                {
                    if (eventsCount == quantity)
                    {
                        if (events != null)
                        {
                            foreach (var evt in events)
                            {
                                title += evt.case_num + "<br>";
                            }
                        }
                    }
                    else
                    {
                        if (blocked)
                        {
                            if (public_block)
                            {
                                title = "Public blocked <br>" + (!string.IsNullOrEmpty(block_reason) ? block_reason : description);
                            }
                            else
                            {
                                title = "blocked <br>" + (!string.IsNullOrEmpty(block_reason) ? block_reason : description);
                            }
                        }
                        else
                        {
                            if (quantity - eventsCount < 1)
                            {
                                if (events != null)
                                {
                                    foreach (var evt in events)
                                    {
                                        title += evt.case_num + "<br>";
                                    }
                                }
                            }
                            else
                            {
                                title = (quantity - eventsCount) + " Available";
                                if (Courtroom != null && !string.IsNullOrEmpty(Courtroom.description))
                                {
                                    title += " (" + Courtroom.description + ")";
                                }
                                if (description != null && !string.IsNullOrEmpty(description))
                                {
                                    title += " (" + description + ")";
                                }
                            }
                        }
                        title += "<br>";
                        if (events != null)
                        {
                            foreach (var evt in events)
                            {
                                title += evt.case_num + "<br>";
                            }
                        }
                    }
                }

                return title;
            }
        }
        [IgnoreColumn]
        public string total_length
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMinutes(duration * quantity);
                List<string> parts = new List<string>();
                if (ts.Days > 0) parts.Add($"{ts.Days} day{(ts.Days > 1 ? "s" : "")}");
                if (ts.Hours > 0) parts.Add($"{ts.Hours} hour{(ts.Hours > 1 ? "s" : "")}");
                if (ts.Minutes > 0) parts.Add($"{ts.Minutes} minute{(ts.Minutes > 1 ? "s" : "")}");
                return string.Join(" ", parts);
            }
        }
        [IgnoreColumn]
        public string display => "auto";

        [IgnoreColumn]
        public string color
        {
            get
            {
                string color = null;
                var start = this.start;
                var end = this.end;
                double diff = (end - start).TotalMinutes;
                int available = quantity * duration;
                int eventsCount = events?.Count() ?? 0;

                if (available > diff && eventsCount != quantity)
                {
                    color = blocked ? "#808080" : "#dc3545";
                }
                else
                {
                    if (eventsCount == quantity)
                    {
                        color = "#28a745";
                    }
                    else
                    {
                        if (blocked)
                        {
                            color = blocked && public_block ? "rgba(0, 0, 255, 0.5)" : "#808080";
                        }
                        else
                        {
                            color = (quantity - eventsCount < 1) ? "#dc3545" : "#007bff";
                        }
                    }
                }

                return color;
            }
        }

        [IgnoreColumn]
        public string date => start.ToString("MM/dd/yyyy");

        [IgnoreColumn]
        public string startTime => start.ToString("h:mm tt").ToLower();

        [IgnoreColumn]
        public string endTime => end.ToString("h:mm tt").ToLower();

        [IgnoreColumn]
        public string length
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMinutes(duration);
                List<string> parts = new List<string>();
                if (ts.Days > 0) parts.Add($"{ts.Days} day{(ts.Days > 1 ? "s" : "")}");
                if (ts.Hours > 0) parts.Add($"{ts.Hours} hour{(ts.Hours > 1 ? "s" : "")}");
                if (ts.Minutes > 0) parts.Add($"{ts.Minutes} minute{(ts.Minutes > 1 ? "s" : "")}");
                return string.Join(" ", parts.Take(2));
            }
        }
        //[IgnoreColumn]
        //public string TableDisplay => $"{this.start.ToString("MM/dd/yyyy")} @ {this.start.ToString("h:mm tt").ToLower()}";

        [IgnoreColumn]
        public string CourtroomTable => Courtroom?.description ?? "-";

        // Scope not directly translatable to C# property; approximate as a method
        // Assuming Court relation is has-one, count is 0 or 1; adapt based on context
        public static IEnumerable<Timeslot> Active(IEnumerable<Timeslot> timeslots, int courtCount)
        {
            return timeslots.Where(t => t.quantity <= courtCount);
        }

        [IgnoreColumn]
        public bool Clickable => !public_block;
    }

    internal class CustomTimeslot : Timeslot
    {
        public int eventCount { get { return this.events.Count(); } }
        public string reschedule_title { get; set; }
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
        public long court_id { get; set; }
        public bool editable { get; set; }

    }
    internal class MonthlySummaryItem
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool allDay { get; set; }
        public string title { get; set; }
        public int tCount { get; set; }
        public string color { get; set; }
        public int order { get; set; }
        public string timeslotDescription { get; set; }
    }
}