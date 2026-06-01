using DocumentFormat.OpenXml.Drawing;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
using static tjc.Modules.TranscriptDatabase.Services.AttorneyController;

namespace tjc.Modules.TranscriptDatabase.Services
{
    [DnnAuthorize]
    public class EventController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDesignationEvents(int designationId)
        {
            IEnumerable<EventViewModel> events = Enumerable.Empty<EventViewModel>();
            try
            {
                var ctl = new Components.EventController();
                events = ctl.GetEventViewModels(designationId);
                return Request.CreateResponse(new EventResult { data = events, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new EventResult { data = events, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("DeleteEvent")]
        public HttpResponseMessage DeleteEvent(int eventId)
        {
            try
            {
                var ctl = new EventController();
                ctl.DeleteEvent(eventId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("CreateEvent")]
        public HttpResponseMessage CreateEvent(EventViewModel eventViewModel)
        {
            var ctl = new Components.EventController();
            Event eventItem = new Event
            {
                DesignationID = eventViewModel.DesignationId,
                PresidingJudgeID = eventViewModel.PresidingJudgeId,
                HearingType = eventViewModel.HearingType,
                HearingDate = eventViewModel.HearingDate,
                CreatedByUserID = eventViewModel.CreatedByUserID,
                CreatedDate = DateTime.Now,
                LastModifiedByUserID = eventViewModel.CreatedByUserID,
                LastModifiedDate = DateTime.Now
            };
            try
            {
                ctl.CreateEvent(eventItem);
                bool result = eventItem.EventID > 0;
                if (result)
                {
                    return Request.CreateResponse(new EventAddedResult { EventId = eventItem.EventID });
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public class EventResult
        {
            public IEnumerable<EventViewModel> data { get; set; }
            public string error { get; set; }

        }
        public class EventAddedResult
        {
            public int EventId { get; set; }

        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("CreateExtension")]
        public HttpResponseMessage CreateExtension(ExtensionViewModel extensionRequest)
        {
            var ctl = new Components.ExtensionRequestController();
            var cCtl = new Components.CalendarController();
            ExtensionRequest extension = new ExtensionRequest
            {
                DesignationID = extensionRequest.DesignationId,
                EventTypeID = extensionRequest.EventTypeId,
                RequestedDate = extensionRequest.RequestedDate,
                SubmittedDate = extensionRequest.SubmittedDate,
                CreatedDate = DateTime.Now,
                CreatedByUserID = extensionRequest.CreatedByUserId,
                LastModifiedByUserID = extensionRequest.CreatedByUserId,
                LastModifiedDate = DateTime.Now,
            };
            try
            {
            
                ctl.CreateExtensionRequest(extension);
                bool hasExtentionId = extension.ExtensionID > 0;
                ExtensionAddedResult extensionresult = new ExtensionAddedResult();
                string returnMessage = string.Empty;
                if (hasExtentionId)
                {
                    Components.Calendar calendar = cCtl.GetCalendarByDesignation(extension.DesignationID);
                    if (calendar != null)
                    {
                        calendar.EventTypeID = extension.EventTypeID;
                        calendar.RequestOutstanding = true;
                        cCtl.UpdateCalendar(calendar);
                        extensionresult = new ExtensionAddedResult { ExtensionId = extension.ExtensionID };
                    }
                    else
                    {
                        returnMessage = "Could not retrieve calendar information for designation. Update the due date to recreate the calendar event.";
                        extensionresult = new ExtensionAddedResult { ExtensionId = extension.ExtensionID, Message = returnMessage };
                    }
                    string subject = string.Format("Extension Request for Designation: {0}", extensionRequest.DesignationId);
                    Notifications.NotifiyRecordingManager(subject, "A new extension request has been submitted", extensionRequest.PortalId, extensionRequest.AdminRole, extensionRequest.CountyName);
                    return Request.CreateResponse(extensionresult);
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(new ExtensionAddedResult { ExtensionId = extension.ExtensionID, Message = exc.Message });
            }
        }

        public class ExtensionAddedResult
        {
            public int ExtensionId { get; set; }
            public string Message { get; set; }

        }
    }
}
