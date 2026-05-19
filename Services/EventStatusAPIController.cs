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
    public class EventStatusAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetEventStatuses(int p1)
        {
            List<EventStatusViewModel> eventStatuses = new List<EventStatusViewModel>();
            try
            {
                var ctl = new EventStatusController();
                var allStatuses = ctl.GetEventStatuses().Select(t => new EventStatusViewModel(t)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new EventStatusSearchResult
                {
                    data = allStatuses,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventStatusSearchResult
                {
                    data = eventStatuses,
                    error = $"Failed to retrieve Event Statuses: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Key/value list for the Status filter on the Event list page.
        /// Mirrors the shape returned by EventTypeAPI/GetEventTypeDropDownItems
        /// so the Select2 wiring works without changes on the JS side.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetEventStatusDropDownItems()
        {
            List<KeyValuePair<long, string>> eventStatuses = new List<KeyValuePair<long, string>>();
            try
            {
                var ctl = new EventStatusController();
                eventStatuses = ctl.GetEventStatusDropDownItems();
                return Request.CreateResponse(new { data = eventStatuses, error = (string)null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new { data = eventStatuses, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteEventStatus(long p1)
        {
            try
            {
                var ctl = new EventStatusController();
                ctl.DeleteEventStatus(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Status deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventStatus(long p1)
        {
            try
            {
                var ctl = new EventStatusController();
                EventStatus eventStatus = ctl.GetEventStatus(p1);
                if (eventStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new EventStatusResult { data = null, error = "Event Status not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new EventStatusResult { data = eventStatus, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new EventStatusResult { data = null, error = ex.Message });
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
                if (string.IsNullOrWhiteSpace(eventStatus.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name is required." });
                }
                eventStatus.created_at = DateTime.Now;
                eventStatus.updated_at = DateTime.Now;
                ctl.CreateEventStatus(eventStatus);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Status created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                if (eventStatus.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Event Status ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(eventStatus.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name is required." });
                }
                var existingEventStatus = ctl.GetEventStatus(eventStatus.id);
                if (existingEventStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Event Status not found." });
                }
                eventStatus.updated_at = DateTime.Now;
                ctl.UpdateEventStatus(eventStatus);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Event Status updated successfully" });
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