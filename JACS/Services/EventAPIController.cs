using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [ValidateAntiForgeryToken]
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
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "50", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            string sortColumn = GetSortColumn(query.ContainsKey("order[0][column]") ? query["order[0][column]"] : "2");
            string sortDirection = query.ContainsKey("order[0][dir]") ? query["order[0][dir]"] : "asc";

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
                Event eventData = ctl.GetEventByCaseNumber(caseNumber.searchTerm);
                if (eventData != null)
                    return Request.CreateResponse(new EventSearchResult { data = new EventViewModel(eventData), error = null });
                return Request.CreateResponse(new EventSearchResult { data = null, error = "No Event Found" });

            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventSearchResult { data = null, error = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CancelEvent(long p1)
        {
            try
            {
                var ctl = new EventController();
                var eventId = p1;
                bool result = ctl.CancelEvent(eventId);
                return Request.CreateResponse(new EventCancelResult { cancelled = result, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventCancelResult { cancelled = false, error = ex.Message });
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