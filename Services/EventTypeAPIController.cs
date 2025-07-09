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
using static tjc.Modules.jacs.Services.CategoryAPIController;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class EventTypeAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetEventTypes(int p1)
        {
            List<EventTypeViewModel> eventTypes = new List<EventTypeViewModel>();
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
                var ctl = new EventTypeController();
                filteredCount = ctl.GetEventTypesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                eventTypes = ctl.GetEventTypesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(eventType => new EventTypeViewModel(eventType)).ToList();
                return Request.CreateResponse(new EventTypeSearchResult { data = eventTypes, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventTypeSearchResult { data = eventTypes, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetEventTypeDropDownItems()
        {
            List<KeyValuePair<long, string>> eventTypes = new List<KeyValuePair<long, string>>();
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = !query.ContainsKey("q") ? "" : query["q"].ToString();

                var ctl = new EventTypeController();
                eventTypes = ctl.GetEventTypeDropDownItems(searchTerm);
                return Request.CreateResponse(new EventTypeListItemResult { data = eventTypes, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventTypeListItemResult { data = null, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage DeleteEventType(long p1)
        {
            try
            {
                var ctl = new EventTypeController();
                ctl.DeleteEventType(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventType(long p1)
        {
            try
            {
                var ctl = new EventTypeController();
                EventType eventType = ctl.GetEventType(p1);
                return Request.CreateResponse(new EventTypeResult { data = eventType, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventTypeResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateEventType(JObject p1)
        {
            try
            {
                var ctl = new EventTypeController();
                var eventType = p1.ToObject<EventType>();
                ctl.CreateEventType(eventType);
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
        public HttpResponseMessage UpdateEventType(JObject p1)
        {
            try
            {
                var ctl = new EventTypeController();
                var eventType = p1.ToObject<EventType>();
                ctl.UpdateEventType(eventType);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class EventTypeSearchResult
        {
            public List<EventTypeViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }
        internal class EventTypeListItemResult
        {
            public List<KeyValuePair<long, string>> data { get; set; }
            public string error { get; set; }
        }

        internal class EventTypeResult
        {
            public EventType data { get; set; }
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