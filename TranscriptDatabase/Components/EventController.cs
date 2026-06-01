using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class EventController
    {
        public void CreateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                rep.Insert(t);
            }
        }
        public void DeleteEvent(int eventId)
        {
            var t = GetEvent(eventId);
            DeleteEvent(t);
        }
        public void DeleteEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Event> GetEvents(int designationId)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Find("Where DesignationID = @0",designationId);
            }
            return t;
        }
       
        public IEnumerable<EventViewModel> GetEventViewModels(int designationId)
        {
            IEnumerable<EventViewModel> t = Enumerable.Empty<EventViewModel>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
               IEnumerable<Event> events = GetEvents(designationId);
                foreach (Event evt in events)
                {
                    t.Append(new EventViewModel(evt));
                }
            }
            return t;
        }
        public Event GetEvent(int eventId)
        {
            Event t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.GetById(eventId);
            }
            return t;
        }
        public void UpdateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                rep.Update(t);
            }
        }
        public IEnumerable<EventListItem> GetEventListItemsByDesignation(int designationId)
        {
            IEnumerable<EventListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventListItem>();
                t = rep.Find("Where DesignationID = @0", designationId);
            }
            return t;
        }
        public EventListItem GetEventListItem(int eventId)
        {
            EventListItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventListItem>();
                t = rep.GetById(eventId) ;
            }
            return t;
        }

        //public IEnumerable<Calendar> GetCalendarEvents(DateTime currentDate, List<int> userIds, string url)
        //{
        //    DateTime monthBegins = new DateTime(currentDate.Year, currentDate.Month, 1);
        //    DateTime monthEnds = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
        //    monthBegins = monthBegins.AddDays(-((int)monthBegins.DayOfWeek));
        //    int endDayofWeek = (int)monthEnds.DayOfWeek;
        //    monthEnds = monthEnds.AddDays(6 - endDayofWeek);
        //    IEnumerable<Calendar> calendarEvents = new List<Calendar>();
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<Calendar>();
        //        if (userIds == null || userIds.Count == 1)
        //        {
        //            calendarEvents = rep.Find("Where (StartTime Between @0 And @1) OR (EndTime BETWEEN @0 AND @1) And CourtReporterID=@2", monthBegins, monthEnds);
        //        }
        //        else
        //        {
        //            string sqlQuery = $@"Select * From tjc_rec_event Where (HearingDate Between '{monthBegins.ToShortDateString()}' And '{monthEnds.ToShortDateString()}') And CourtReporterID in ({string.Join(",", userIds)})";
        //            t = ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, sqlQuery);
        //        }
        //    }
        //    do
        //    {
        //        string eventList = "";
        //        if (t != null && t.Count() > 0)
        //            eventList = GetEventListItems(t.Where(ev => ev.HearingDate == monthBegins), url);
        //        Calendar dayEvent = new Calendar { StartTime = monthBegins.Day, DayOfWeek = monthBegins.ToString("dddd"), Muted = (monthBegins.Month != currentDate.Month), EventList = eventList };
        //        calendarEvents.Add(dayEvent);
        //        monthBegins = monthBegins.AddDays(1);
        //    } while (monthBegins <= monthEnds);
        //    return calendarEvents;
        //}

        //private string GetEventListItems(IEnumerable<Event> t, string url)
        //{
        //    string eventList = "";
        //    foreach (Event @event in t)
        //    {
        //        string link = string.Format("{0}/aid/{1}", url, @event.AssignmentId);
        //        eventList += string.Format("<div title=\"{0}\" data-assignmentid=\"{1}\" data-plugin-tooltip=\"tooltip\" data-toggle=\"tooltip\" class=\"event-item\" data-subject=\"{2}\" data-body=\"{0}\" data-user=\"{3}\"><strong><a href=\"{4}\">{2}</a></strong> <span class=\"d-block\">{3}</span></div>", @event.Body, @event.AssignmentId, @event.Subject, @event.UserName.Replace("azure-", "").Replace("@jud12.flcourts.org", ""), link);
        //    }
        //    return eventList;
        //}
    }
}
