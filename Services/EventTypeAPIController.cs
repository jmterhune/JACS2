using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class EventTypeAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetEventTypes(int p1)
        {
            List<EventTypeViewModel> eventTypes = new List<EventTypeViewModel>();
            try
            {
                var ctl = new EventTypeController();
                var allTypes = ctl.GetEventTypes().Select(t => new EventTypeViewModel(t)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new EventTypeSearchResult
                {
                    data = allTypes,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventTypeSearchResult
                {
                    data = eventTypes,
                    error = $"Failed to retrieve Event Types: {ex.Message}"
                });
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
        
        [HttpDelete]
        public HttpResponseMessage DeleteEventType(long p1)
        {
            try
            {
                var ctl = new EventTypeController();
                ctl.DeleteEventType(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Type deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventType(long p1)
        {
            try
            {
                var ctl = new EventTypeController();
                EventType eventType = ctl.GetEventType(p1);
                if (eventType == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventTypeResult { data = null, error = "Event Type not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventTypeResult { data = eventType, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventTypeResult { data = null, error = ex.Message });
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
                if (string.IsNullOrWhiteSpace(eventType.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name is required." });
                }
                eventType.created_at = DateTime.Now;
                eventType.updated_at = DateTime.Now;
                ctl.CreateEventType(eventType);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Type created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                if (eventType.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Event Type ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(eventType.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name is required." });
                }
                var existingEventType = ctl.GetEventType(eventType.id);
                if (existingEventType == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Event Type not found." });
                }
                eventType.updated_at = DateTime.Now;
                ctl.UpdateEventType(eventType);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Type updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
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