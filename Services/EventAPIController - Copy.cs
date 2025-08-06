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
                filteredCount = ctl.GetEventListItemCount(searchTerm, courtId, categoryId, statusId);
                if (p1 == 0) { recordCount = filteredCount; }
                events = ctl.GetEventListItems(searchTerm, courtId, categoryId, statusId, recordOffset, pageSize, sortColumn, sortDirection)
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
                var ctl = new EventController();
                bool result = ctl.CancelEvent(p1);
                if (!result)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventCancelResult { cancelled = false, error = "Event not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventCancelResult { cancelled = true, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventCancelResult { cancelled = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CancelHearing(long p1)
        {
            try
            {
                var ctl = new EventController();
                bool result = ctl.CancelEvent(p1);
                if (!result)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventCancelResult { cancelled = false, error = "Event not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventCancelResult { cancelled = true, error = null });
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
                var evt = p1.ToObject<Event>();
                if (evt == null || string.IsNullOrWhiteSpace(evt.case_num))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                evt.created_at = DateTime.Now;
                evt.updated_at = DateTime.Now;
                evt.plaintiff_email = p1["plaintiff_email"]?.ToString().Replace(";", ",");
                evt.defendant_email = p1["defendant_email"]?.ToString().Replace(";", ",");
                evt.template = p1["template"]?.ToString();
                var ctl = new EventController();
                ctl.CreateEvent(evt);
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
                return Request.CreateResponse(HttpStatusCode.OK, new EventsResult{ data = events.Select(e => new EventViewModel(e)).ToList(), error = null });
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