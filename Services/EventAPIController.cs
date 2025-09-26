using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class EventAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetEvents(int p1)
        {
            List<EventViewModel> events = new List<EventViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
            string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : "";
            long courtId = query.ContainsKey("courtId") && long.TryParse(query["courtId"], out long cId) ? cId : 0;
            long categoryId = query.ContainsKey("categoryId") && long.TryParse(query["categoryId"], out long catId) ? catId : 0;
            long statusId = query.ContainsKey("statusId") && long.TryParse(query["statusId"], out long statId) ? statId : 0;
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "50", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "case_num"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                sortColumn = GetSortColumn(query["order[0].column"]);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new EventController();
                filteredCount = ctl.GetEventListItemCount(userId, searchTerm, courtId, categoryId, statusId);
                if (p1 == 0) { recordCount = filteredCount; }
                events = ctl.GetEventListItems(userId, searchTerm, courtId, categoryId, statusId, recordOffset, pageSize, sortColumn, sortDirection)
                           .Select(evt => new EventViewModel(evt)).ToList();
                return Request.CreateResponse(new EventListItemResult
                {
                    data = events,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventListItemResult
                {
                    data = events,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = ex.Message
                });
            }
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetDashsboardEvents()
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
                bool isJudge = query.ContainsKey("isJudge") && bool.TryParse(query["isJudge"], out bool judge) ? judge : false;
                var ctl = new EventController();
                var events = new List<EventViewModel>();
                if (isJudge)
                {
                    events = ctl.GetEventsForDashboardByJudge(userId).Select(evt => new EventViewModel(evt)).ToList();
                }
                else if (UserInfo.IsAdmin)
                {
                    events = ctl.GetEventsForDashBoardByAdmin().Select(evt => new EventViewModel(evt)).ToList();
                }
                else
                {
                    events = ctl.GetEventsForDashboard(userId).Select(evt => new EventViewModel(evt)).ToList();
                }
                return Request.CreateResponse(new { data = events });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SearchCaseNumber(JObject p1)
        {
            try
            {
                var ctl = new EventController();
                var caseNumber = p1.ToObject<SearchTerm>();
                if (string.IsNullOrWhiteSpace(caseNumber.searchTerm))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                Event eventData = ctl.GetEventByCaseNumber(caseNumber.searchTerm);
                if (eventData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventSearchResult { data = null, error = "No Event Found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventSearchResult { data = new EventViewModel(eventData), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventSearchResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SearchCaseNumberDetails(JObject p1)
        {
            try
            {
                var caseNumber = p1.ToObject<SearchTerm>();
                if (string.IsNullOrWhiteSpace(caseNumber.searchTerm))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                var ctl = new EventController();
                var eventData = ctl.GetEventByCaseNumber(caseNumber.searchTerm);
                if (eventData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventSearchResult { data = null, error = "No Event Found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventSearchResult { data = new EventViewModel(eventData), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventSearchResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CancelEvent(long p1)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string reason = query.ContainsKey("cancellation_reason") ? query["cancellation_reason"] : string.Empty;

                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventCancelResult { cancelled = false, error = "Event ID is required" });

                }
                var ctl = new EventController();
                var teCtl = new TimeslotEventController();
                var tsCtl = new TimeslotController();
                var eventStatusCtl = new EventStatusController();
                var cancelledStatus = eventStatusCtl.GetEventStatusByName("Cancelled");
                var eventToCancel = ctl.GetEvent(p1);
                if (eventToCancel == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventCancelResult { cancelled = false, error = "Event not found" });
                }
                var timeslotEvents = teCtl.GetTimeslotEventsByEvent(p1);
                foreach (var te in timeslotEvents)
                {
                    teCtl.DeleteTimeslotEvent(te.id);
                }
                eventToCancel.cancellation_reason = reason;
                eventToCancel.status_id =  cancelledStatus != null ? cancelledStatus.id : (long?)null;
                eventToCancel.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToCancel);
                return Request.CreateResponse(HttpStatusCode.OK, new EventCancelResult { cancelled = true, error = null});

            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventCancelResult { cancelled = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateEvent(JObject p1)
        {
            try
            {
                var eventViewModel = p1.ToObject<EventViewModel>();
                if (eventViewModel == null || string.IsNullOrWhiteSpace(eventViewModel.case_num))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                eventViewModel.created_at = DateTime.Now;
                eventViewModel.updated_at = DateTime.Now;
                eventViewModel.plaintiff_email = eventViewModel.plaintiff_email?.ToString().Replace(";", ",");
                eventViewModel.defendant_email = eventViewModel.defendant_email?.ToString().Replace(";", ",");
                eventViewModel.template = eventViewModel.template?.ToString();

                var ctl = new EventController();
                var ctlStatus = new EventStatusController();
                var status = ctlStatus.GetEventStatusByName("Scheduled");
                Event evt = new Event
                {
                    case_num = eventViewModel.case_num,
                    notes = eventViewModel.notes,
                    plaintiff = eventViewModel.plaintiff,
                    defendant = eventViewModel.defendant,
                    motion_id = eventViewModel.motion_id,
                    attorney_id = eventViewModel.attorney_id,
                    type_id = eventViewModel.type_id,
                    status_id = status != null ? status.id : (long?)null,
                    reminder = eventViewModel.reminder,
                    opp_attorney_id = eventViewModel.opp_attorney_id,
                    owner_id = eventViewModel.owner_id,
                    owner_type = eventViewModel.owner_type,
                    addon = eventViewModel.addon,
                    plaintiff_email = eventViewModel.plaintiff_email,
                    defendant_email = eventViewModel.defendant_email,
                    cancellation_reason = eventViewModel.cancellation_reason,
                    template = eventViewModel.template,
                    telephone = eventViewModel.telephone,
                    custom_motion = eventViewModel.custom_motion,
                    created_at = eventViewModel.created_at,
                    updated_at = eventViewModel.updated_at
                };
                ctl.CreateEvent(evt);
                if (evt.id > 0)
                {
                    var timeslotCtl = new TimeslotEventController();
                    TimeslotEvent timeslotEvent = new TimeslotEvent
                    {
                        event_id = evt.id,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        timeslot_id = eventViewModel.timeslot_id > 0 ? eventViewModel.timeslot_id : (long?)null
                    };
                    timeslotCtl.CreateTimeslotEvent(timeslotEvent);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event created successfully" });
            }
            catch (ValidationException vex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = vex.Message });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateEvent(JObject p1)
        {
            try
            {
                var evt = p1.ToObject<Event>();
                if (evt.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Event ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(evt.case_num))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                var ctl = new EventController();
                var existingEvent = ctl.GetEvent(evt.id);
                if (existingEvent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Event not found." });
                }
                evt.updated_at = DateTime.Now;
                evt.plaintiff_email = p1["plaintiff_email"]?.ToString().Replace(";", ",");
                evt.defendant_email = p1["defendant_email"]?.ToString().Replace(";", ",");
                evt.template = p1["template"]?.ToString();
                ctl.UpdateEvent(evt);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event updated successfully" });
            }
            catch (ValidationException vex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = vex.Message });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEvent(long p1)
        {
            try
            {
                var ctl = new EventController();
                var evt = ctl.GetEvent(p1);
                if (evt == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventSearchResult { data = null, error = "Event not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventSearchResult { data = new EventViewModel(evt), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventSearchResult { data = null, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventsForTimeslot(long p1)
        {
            try
            {
                var ctl = new EventController();
                var events = ctl.GetEventsByTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new EventsResult { data = events.Select(e => new EventViewModel(e)).ToList(), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetEventListItemsForTimeslot(long p1)
        {
            try
            {
                var ctl = new EventController();
                var events = ctl.GetEventListItemsByTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new EventsResult { data = events.Select(e => new EventViewModel(e)).ToList(), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetEventDuration(long p1)
        {
            try
            {
                var teCtl = new TimeslotEventController();
                var tsCtl = new TimeslotController();
                var timeslotEvents = teCtl.GetTimeslotEventsByEvent(p1);
                if (!timeslotEvents.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "No timeslots found for event." });
                }
                var timeslots = timeslotEvents.Select(te => tsCtl.GetTimeslot(te.timeslot_id.Value)).OrderBy(ts => ts.start).ToList();
                var firstStart = timeslots.First().start;
                var lastEnd = timeslots.Last().end;
                var totalDuration = (lastEnd - firstStart).TotalMinutes;
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, duration = totalDuration });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage RescheduleEvent(RescheduleModel p1)
        {
            try
            {
                var ctl = new EventController();
                var teCtl = new TimeslotEventController();
                var tsCtl = new TimeslotController();
                var courtCtl = new CourtController();
                var eventToReschedule = ctl.GetEvent(p1.event_id);
                var eventStatusCtl = new EventStatusController();
                var rescheduledStatus = eventStatusCtl.GetEventStatusByName("Rescheduled");

                if (eventToReschedule == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Event not found." });
                }
                var timeslotEvent = teCtl.GetTimeslotEventsByEvent(p1.event_id).FirstOrDefault();
                
                if (timeslotEvent==null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "No timeslots found for event." });
                }
                
                var currentTimeslot = tsCtl.GetTimeslot(timeslotEvent.timeslot_id.Value);
                var selectedtimeslot = tsCtl.GetTimeslot(p1.timeslot_id);
                var selectedtimeslotEvents = teCtl.GetTimeslotEventsByTimeslot(selectedtimeslot.id);
                int eventCount = selectedtimeslotEvents.Count();
                if (eventCount>=selectedtimeslot.quantity)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "The selected timeslot has no space available for event assignment." });
                }
                var currentDuration = currentTimeslot.duration;
                var selectedDuration = (selectedtimeslot.duration);
                if (selectedDuration != currentDuration)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Selected duration must match original hearing duration." });
                }
                //var oldIds = timeslots.Select(ts => ts.id).ToArray();
                //using (IDataContext ctx = DataContext.Instance("jacs"))
                //{
                //    var overlapQuery = @"
                //        SELECT COUNT(*) FROM timeslots ts
                //        INNER JOIN court_timeslots ct ON ct.timeslot_id = ts.id
                //        WHERE ct.court_id = @0
                //        AND ts.deleted_at IS NULL
                //        AND ts.id NOT IN (" + string.Join(",", oldIds) + @")
                //        AND (ts.start < @2 AND ts.end > @1)
                //    ";
                //    var overlaps = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, overlapQuery, courtId, p1.start_new, p1.end_new);
                //    if (overlaps > 0)
                //    {
                //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "The selected time range overlaps with existing timeslots." });
                //    }
                //}
                timeslotEvent.timeslot_id = selectedtimeslot.id;
                teCtl.UpdateTimeslotEvent(timeslotEvent);
                eventToReschedule.status_id =  rescheduledStatus != null ? rescheduledStatus.id : (long?)null;
                eventToReschedule.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToReschedule);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event rescheduled successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        internal class EventListItemResult
        {
            public List<EventViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }
        internal class EventsResult
        {
            public List<EventViewModel> data { get; set; }
            public string error { get; set; }
        }

        internal class EventSearchResult
        {
            public EventViewModel data { get; set; }
            public string error { get; set; }
        }

        internal class EventCancelResult
        {
            public bool cancelled { get; set; }
            public string error { get; set; }
        }

        internal class SearchTerm
        {
            public string searchTerm { get; set; }
        }

        private string GetSortColumn(string columnIndex)
        {
            switch (columnIndex)
            {
                case "2": return "case_num";
                case "3": return "motion";
                case "4": return "timeslot";
                case "5": return "duration";
                case "6": return "court";
                case "7": return "status";
                case "8": return "attorney";
                case "9": return "opposing_attorney";
                case "10": return "plaintiff";
                case "11": return "defendant";
                case "12": return "category";
                default: return "case_num";
            }
        }
    }
}