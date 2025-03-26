using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class CalendarController
    {
        public void CreateCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Insert(t);
            }
        }
        public void DeleteCalendar(int calendarId)
        {
            var t = GetCalendar(calendarId);
            DeleteCalendar(t);
        }
        public void DeleteCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Calendar> GetCalendars()
        {
            IEnumerable<Calendar> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.Get();
            }
            return t;
        }
        public Calendar GetCalendar(int calendarId)
        {
            Calendar t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.GetById(calendarId);
            }
            return t;
        }
        public Calendar GetCalendarByDesignation(int designationId)
        {
            Calendar t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.Find("Where DesignationID=@0", designationId).FirstOrDefault();
            }
            return t;
        }
        public void UpdateCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Update(t);
            }
        }
        public IEnumerable<CalendarEvent> GetCalendarEvents(DateTime currentDate, string county, List<int> courtReporterIds, string url)
        {
            DateTime monthBegins = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime monthEnds = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            monthBegins = monthBegins.AddDays(-((int)monthBegins.DayOfWeek));
            int endDayofWeek = (int)monthEnds.DayOfWeek;
            monthEnds = monthEnds.AddDays(6 - endDayofWeek);
            List<CalendarEvent> calendarEvents = new List<CalendarEvent>();
            IEnumerable<CalendarListItem> t;
            int reporterId = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CalendarListItem>();
                if (courtReporterIds == null || courtReporterIds.Count == 0)
                {
                    if (county != "")
                        t = rep.Find("Where (StartTime Between @0 And @1) AND County = @2", monthBegins, monthEnds, county);
                    else
                        t = rep.Find("Where (StartTime Between @0 And @1)", monthBegins, monthEnds);
                }
                else if (courtReporterIds.Count == 1)
                {
                    reporterId = courtReporterIds.FirstOrDefault();
                    if (county != "")
                        t = rep.Find("Where (StartTime Between @0 And @1) And CreatedByUserID = @2 AND County=@3", monthBegins, monthEnds, reporterId, county);
                    else
                        t = rep.Find("Where (StartTime Between @0 And @1) And CreatedByUserID = @2", monthBegins, monthEnds, reporterId);
                }
                else
                {
                    string sqlQuery = $@"Select * From tjc_rec_calendar_events Where (StartTime Between '{monthBegins.ToShortDateString()}' And '{monthEnds.ToShortDateString()}') And CreatedByUserID in ({string.Join(",", courtReporterIds)})";
                    if (county != "")
                        sqlQuery += string.Format(" AND County={0}",county);
                    t = ctx.ExecuteQuery<CalendarListItem>(System.Data.CommandType.Text, sqlQuery);
                }
            }
            do
            {
                string eventList = "";
                if (t != null && t.Count() > 0)
                    eventList = GetEventListItems(t.Where(ev => ev.StartTime == monthBegins), url, reporterId);
                CalendarEvent dayEvent = new CalendarEvent { Day = monthBegins.Day, DayOfWeek = monthBegins.ToString("dddd"), Muted = (monthBegins.Month != currentDate.Month), EventList = eventList };
                calendarEvents.Add(dayEvent);
                monthBegins = monthBegins.AddDays(1);
            } while (monthBegins <= monthEnds);
            return calendarEvents;
        }
        private string GetEventListItems(IEnumerable<CalendarListItem> t, string url, int reporterId)
        {
            string eventList = "";
            var ctl = new DesignationController();
            foreach (CalendarListItem @event in t)
            {
                Designation designation = ctl.GetDesignation(@event.DesignationID);

                string link = string.Format("{0}/did/{1}", url, @event.DesignationID);
                string evtTypeClass = "";
                string subject = "";
                string tooltip = "";
                string courtReporterName = "";
                int estimatedPages = designation.EstimatedPages(reporterId);
                if (@event.RequestOutstanding)
                    subject = "<em>(r)</em> ";
                subject += @event.Subject;
                subject += " - ";
                subject += estimatedPages.ToString();
                tooltip = subject;
                if (@event.EventTypeID >= 0)
                {
                    tooltip = string.Format("{0}<br /><strong>{1}</strong>", tooltip, Enumerations.GetEnumDescription(@event.EventType));
                }
                if (!string.IsNullOrEmpty(@event.CourtReporterName))
                {
                    courtReporterName = @event.CourtReporterName;
                    tooltip = string.Format("{0}<br />{1}", tooltip, courtReporterName);
                }
                switch (@event.EventType)
                {
                    case EventTypes.firstExtension:
                        evtTypeClass = "first-extension";
                        break;
                    case EventTypes.secondExtension:
                        evtTypeClass = "second-extension";
                        break;
                    case EventTypes.thirdExtension:
                        evtTypeClass = "third-extension";
                        break;
                    case EventTypes.dueDate:
                        evtTypeClass = "due-date";
                        break;
                    case EventTypes.transcriptFiled:
                        evtTypeClass = "transcript-filed";
                        break;
                    default:
                        break;
                }
                // eventList += string.Format("<div title=\"{0}\n\r{3}\" data-designationId=\"{1}\" data-plugin-tooltip=\"tooltip\" data-toggle=\"tooltip\" class=\"event-item {5}\" data-subject=\"{2}\" data-body=\"{0}\" data-user=\"{3}\"><strong><a href=\"{4}\">{2}</a></strong></div>", tooltip, @event.DesignationID, subject, @event.CourtReporterName, link,evtTypeClass);
                eventList += string.Format("<div title=\"{0}\" data-designationId=\"{1}\" data-bs-toggle=\"tooltip\" data-bs-html=\"true\" data-subject=\"{2}\" data-body=\"{0}\" data-user=\"{3}\" class=\"border rounded event-item {4}\"><strong><a href=\"{5}\">{2}</a></strong></div>", tooltip, @event.DesignationID, subject, courtReporterName, evtTypeClass, link);

            }
            return eventList;
        }
    }
}
