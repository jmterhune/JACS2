/*
' Copyright (c) 2022 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class EventController
    {
        private const string AuthSystemApplicationName = "Azure";
        private const string CONN_INTRANET = "Intranet.API"; //Connection
        public EventController() // 
        {

        }
        public void CreateEvent(Event t, string username, int portalId)
        {
            string externalId = AsyncHelper.RunSync(() => CreateGraphEventAsync(t, username, portalId));
            if (!string.IsNullOrEmpty(externalId))
            {
                t.ExternalId = externalId;
                using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
                {
                    var rep = ctx.GetRepository<Event>();
                    rep.Insert(t);
                }
            }
        }

        public bool DeleteEvent(long eventId, string username, int portalId)
        {
            var t = GetEvent(eventId);
            bool deleted = AsyncHelper.RunSync(() => DeleteGraphEventAsync(t.ExternalId, username, portalId));
            if (deleted)
                DeleteEvent(t);
            return deleted;
        }

        public void DeleteEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Event> GetEvents()
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Event> GetEventPage(int pageIndex, int pageSize)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.GetPage(pageIndex, pageSize);
            }
            return t;
        }
        public IEnumerable<CalendarEvent> GetCalendarEvents(DateTime currentDate, List<string> usernames,string url)
        {
            DateTime monthBegins = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime monthEnds = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            monthBegins = monthBegins.AddDays(-((int)monthBegins.DayOfWeek));
            int endDayofWeek = (int)monthEnds.DayOfWeek;
            monthEnds = monthEnds.AddDays(6- endDayofWeek);
            List<CalendarEvent> calendarEvents = new List<CalendarEvent>();
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                if (usernames == null || usernames.Count == 1)
                {
                    t = rep.Find("Where (StartDate Between @0 And @1) And username=@2", monthBegins, monthEnds, usernames.FirstOrDefault());
                }
                else
                {
                    string sqlQuery = $@"Select * From court_counsel_events Where (StartDate Between '{monthBegins.ToShortDateString()}' And '{monthEnds.ToShortDateString()}') And username in ('{string.Join("','", usernames).Replace("@","@@")}')";
                    t = ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, sqlQuery);
                }
            }
            do
            {
                string eventList = "";
                if (t != null && t.Count() > 0)
                    eventList = GetEventListItems(t.Where(ev => ev.StartDate == monthBegins),url);
                CalendarEvent dayEvent = new CalendarEvent { Day = monthBegins.Day, DayOfWeek = monthBegins.ToString("dddd"), Muted = (monthBegins.Month != currentDate.Month), EventList = eventList };
                calendarEvents.Add(dayEvent);
                monthBegins=monthBegins.AddDays(1);
            } while (monthBegins <= monthEnds);
            return calendarEvents;
        }

        private string GetEventListItems(IEnumerable<Event> t,string url)
        {
            string eventList = "";
            foreach (Event @event in t)
            {
              string  link = string.Format("{0}/aid/{1}",url,@event.AssignmentId); 
                eventList += string.Format("<div title=\"{0}\" data-assignmentid=\"{1}\" data-plugin-tooltip=\"tooltip\" data-toggle=\"tooltip\" class=\"event-item\" data-subject=\"{2}\" data-body=\"{0}\" data-user=\"{3}\"><strong><a href=\"{4}\">{2}</a></strong> <span class=\"d-block\">{3}</span></div>", @event.Body, @event.AssignmentId, @event.Subject, @event.UserName.Replace("azure-","").Replace("@jud12.flcourts.org",""), link);
            }
            return eventList;
        }

        public IEnumerable<Event> GetEventsByAssignment(long assignmentId)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Find("Where AssignmentId = @0", assignmentId);
            }
            return t;
        }
        public IEnumerable<EventListItem> GetCalendarEventItems(SearchQueryParameters searchQueryParameters, int pageIndex, int pageSize)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                string sqlQuery = $@"SELECT e.EventId, a.AssignmentId, l.CaseNumber, l.Description as CaseName,
                                                        ct.CaseTypeName, a.DateReceived, e.Subject, e.StartDate, e.UserName 
                                                    FROM tjc_cc_log_entries l
                                                        inner join tjc_cc_assignments a on l.LogId = a.LogId
                                                        inner join tjc_cc_events e on e.AssignmentId = a.AssignmentId
                                                        inner join tjc_cc_case_types ct on a.CaseTypeId = ct.CaseTypeId
                                                    WHERE e.StartDate BETWEEN @0 AND @1";
                if (searchQueryParameters.StartDate == searchQueryParameters.EndDate)
                {
                    searchQueryParameters.EndDate = searchQueryParameters.EndDate.AddDays(1);
                }

                if (!string.IsNullOrEmpty(searchQueryParameters.CaseNumber) && searchQueryParameters.CaseNumber != null)
                {
                    sqlQuery += string.Format(" AND l.CaseNumber like %{0}%", searchQueryParameters.CaseNumber);
                }
                if (!string.IsNullOrEmpty(searchQueryParameters.CaseName) && searchQueryParameters.CaseName != null)
                {
                    sqlQuery += string.Format(" AND l.Description like %{0}%", searchQueryParameters.CaseName);
                }
                if (!string.IsNullOrEmpty(searchQueryParameters.UserName) && searchQueryParameters.UserName != null)
                {
                    string userNameList = "";
                    string[] UserNames = searchQueryParameters.UserName.Split(',');
                    foreach (var item in UserNames)
                    {
                        userNameList += string.Format("'{0}',", item);
                    }
                    userNameList = userNameList.Trim(',');

                    sqlQuery += $" AND e.UserName In ({userNameList})";
                }

                sqlQuery += String.Format(" ORDER BY e.StartDate OFFSET {0} * ({1} -1) ROWS FETCH NEXT {0} ROWS ONLY", pageSize, pageIndex);

                IEnumerable<EventListItem> eventListItems = ctx.ExecuteQuery<EventListItem>(System.Data.CommandType.Text, sqlQuery, searchQueryParameters.StartDate, searchQueryParameters.EndDate);
                return eventListItems;

            }
        }
        public IEnumerable<Event> GetEventListItems(SearchQueryParameters searchQueryParameters, int pageIndex, int pageSize)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                string sqlQuery = $@"SELECT EventId, AssignmentId, Subject, Body, StartDate, EndDate, UserName 
                                                    FROM tjc_cc_events 
                                                    WHERE StartDate BETWEEN @0 AND @1";

                var firstDayOfMonth = new DateTime(searchQueryParameters.StartDate.Year, searchQueryParameters.StartDate.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                if (!string.IsNullOrEmpty(searchQueryParameters.UserName) && searchQueryParameters.UserName != null)
                {
                    string userNameList = "";
                    string[] UserNames = searchQueryParameters.UserName.Split(',');
                    foreach (var item in UserNames)
                    {
                        userNameList += string.Format("'{0}',", item);
                    }
                    userNameList = userNameList.Trim(',');

                    sqlQuery += $" AND UserName In ({userNameList})";
                }
                sqlQuery += String.Format(" ORDER BY e.StartDate OFFSET {0} * ({1} -1) ROWS FETCH NEXT {0} ROWS ONLY", pageSize, pageIndex);

                IEnumerable<Event> events = ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, sqlQuery, searchQueryParameters.StartDate, searchQueryParameters.EndDate);
                return events;
            }
        }
        public Event GetEvent(long eventId)
        {
            Event t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.GetById(eventId);
            }
            return t;
        }
        public bool UpdateEvent(Event t, string username, int portalId)
        {
            bool updated = AsyncHelper.RunSync(() => UpdateGraphEventAsync(t, username, portalId));
            if (updated)
                using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
                {
                    var rep = ctx.GetRepository<Event>();
                    rep.Update(t);
                }
            return updated;
        }

        public async Task<string> CreateGraphEventAsync(Event t, string userName, int portalId)
        {
            try
            {
                Microsoft.Graph.Event graphEvent = new Microsoft.Graph.Event
                {
                    Start = new DateTimeTimeZone
                    {
                        DateTime = t.StartDate.ToString("o"),
                        TimeZone = "Eastern Standard Time"
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = t.EndDate.ToString("o"),
                        TimeZone = "Eastern Standard Time"
                    },
                    Subject = t.Subject,
                    Body = new ItemBody { Content = t.Body, ContentType = BodyType.Html },
                    IsAllDay = true,
                    ReminderMinutesBeforeStart = (int)t.ReminderMinutesBeforeStart,
                    IsReminderOn = t.IsReminderOn
                };
                DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
                var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
                DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);
                var myEvent = await graphClient.CreateGraphEventAsync(graphEvent, userName);
                if (myEvent != null)
                    return myEvent.Id;
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
            return "";
        }
        public async Task<bool> UpdateGraphEventAsync(Event t, string userName, int portalId)
        {
            try
            {
                Microsoft.Graph.Event graphEvent = new Microsoft.Graph.Event
                {
                    Start = new DateTimeTimeZone
                    {
                        DateTime = t.StartDate.ToString("o"),
                        TimeZone = "Eastern Standard Time"
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = t.EndDate.ToString("o"),
                        TimeZone = "Eastern Standard Time"
                    },
                    Subject = t.Subject,
                    Body = new ItemBody { Content = t.Body, ContentType = BodyType.Html },
                    IsAllDay = true,
                    ReminderMinutesBeforeStart = (int)t.ReminderMinutesBeforeStart,
                    IsReminderOn = t.IsReminderOn
                };
                DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
                var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
                DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);

                var myEvent = await graphClient.UpdateGraphEventAsync(graphEvent, userName, t.ExternalId);
                if (myEvent != null)
                    return true;
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
            return false;
        }
        public async Task<bool> DeleteGraphEventAsync(string graphEventId, string userName, int portalId)
        {

            DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
            var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
            DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);

            return await graphClient.DeleteGraphEventAsync(graphEventId, userName);
        }
    }
}
