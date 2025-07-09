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
    public class EventStatusAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetEventStatuses(int p1)
        {
            List<EventStatusViewModel> eventStatuses = new List<EventStatusViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new EventStatusController();
                filteredCount = ctl.GetEventStatusesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                eventStatuses = ctl.GetEventStatusesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(eventStatus => new EventStatusViewModel(eventStatus)).ToList();
                return Request.CreateResponse(new EventStatusSearchResult { data = eventStatuses, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventStatusSearchResult { data = eventStatuses, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteEventStatus(long p1)
        {
            try
            {
                var ctl = new EventStatusController();
                ctl.DeleteEventStatus(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventStatus(long p1)
        {
            try
            {
                var ctl = new EventStatusController();
                EventStatus eventStatus = ctl.GetEventStatus(p1);
                return Request.CreateResponse(new EventStatusResult { data = eventStatus, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventStatusResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateEventStatus(JObject p1)
        {
            try
            {
                var ctl = new EventStatusController();
                var eventStatus = p1.ToObject<EventStatus>();
                ctl.CreateEventStatus(eventStatus);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateEventStatus(JObject p1)
        {
            try
            {
                var ctl = new EventStatusController();
                var eventStatus = p1.ToObject<EventStatus>();
                ctl.UpdateEventStatus(eventStatus);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class EventStatusSearchResult
        {
            public List<EventStatusViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class EventStatusResult
        {
            public EventStatus data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "name";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}