// Filename: CourtEventsAPIController.cs
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CourtEventsAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            try
            {
                // Placeholder for index logic; PHP returns a view, so this would typically return a list of events
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Index view not implemented in API." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage Show(long courtId)
        {
            try
            {
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var eventCtl = new EventController();
                var events = eventCtl.GetEventsByCourtId(courtId);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    events = events.Select(e => new
                    {
                        id = e.id,
                        case_num = e.case_num,
                        motion_id = e.motion_id,
                        type_id = e.type_id,
                        attorney_id = e.attorney_id,
                        opp_attorney_id = e.opp_attorney_id,
                        plaintiff = e.plaintiff,
                        defendant = e.defendant,
                        plaintiff_email = e.plaintiff_email,
                        defendant_email = e.defendant_email,
                        notes = e.notes,
                        addon = e.addon,
                        reminder = e.reminder
                    })
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Store(JObject p1)
        {
            try
            {
                var eventData = p1.ToObject<Event>();
                if (string.IsNullOrWhiteSpace(eventData.case_num))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Case number is required." });
                }
                var eventCtl = new EventController();
                eventData.created_at = DateTime.Now;
                eventData.updated_at = DateTime.Now;
                eventCtl.CreateEvent(eventData);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Event created successfully",
                    id = eventData.id
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage Create()
        {
            try
            {
                // Placeholder for create logic; PHP returns a view, so this would typically return form data or options
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Create view not implemented in API." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
    }
}