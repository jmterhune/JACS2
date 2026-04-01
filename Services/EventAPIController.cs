using DocumentFormat.OpenXml.EMMA;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
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
            long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
            string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : "";
            long courtId = query.ContainsKey("courtId") && long.TryParse(query["courtId"], out long cId) ? cId : 0;
            long courtroomId = query.ContainsKey("courtroomId") && long.TryParse(query["courtroomId"], out long catId) ? catId : 0;
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
                filteredCount = ctl.GetEventListItemCount(userId, searchTerm, courtId, courtroomId, statusId);
                if (p1 == 0) { recordCount = filteredCount; }
                events = ctl.GetEventListItems(userId, searchTerm, courtId, courtroomId, statusId, recordOffset, pageSize, sortColumn, sortDirection)
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
                if (UserInfo.IsAdmin)
                {
                    events = ctl.GetEventsForDashBoardByAdmin().Select(evt => new EventViewModel(evt)).ToList();
                }
                else if (isJudge)
                {
                    events = ctl.GetEventsForDashboardByJudge(userId).Select(evt => new EventViewModel(evt)).ToList();
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
        public HttpResponseMessage SearchCaseNumber([FromBody] JObject p1)
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
        public HttpResponseMessage GetEventsByCaseNumber([FromBody] JObject p1)
        {
            try
            {
                var model = p1.ToObject<CaseSearchModel>();
                if (string.IsNullOrWhiteSpace(model.casePattern))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Case number is required" });

                var ctl = new EventController();
                var events = ctl.GetEventsByCasePattern(model.casePattern, model.userId, model.isJudge);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    data = events.Select(e => new EventViewModel(e)).ToList()
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SearchCaseNumberDetails([FromBody] JObject request)
        {
            try
            {
                string caseNum = request["caseNum"]?.Value<string>();
                long courtId = request["courtId"]?.Value<long>() ?? 0;

                if (string.IsNullOrWhiteSpace(caseNum) || courtId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("caseNum and courtId are required in the request body"));

                var court = new CourtController().GetCourt(courtId);
                if (court == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("Court not found"));

                var county = new CountyController().GetCounty(court.county_id);
                if (county == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("County not found"));

                var apiCtl = new ApiEndpointController();
                var api = apiCtl.GetApiEndpointByCountyAndType(county.id, (int)ApiEndpointType.GetCase);
                if (api == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("GetCase API endpoint is not configured for this county"));

                string token = !string.IsNullOrWhiteSpace(county.decrypted_token)
                    ? county.decrypted_token
                    : apiCtl.GetJwtToken(county).Result;

                if (string.IsNullOrWhiteSpace(token))
                {
                    Exceptions.LogException(new Exception($"SearchCaseNumberDetails: failed to obtain token for county {county.id}, court {courtId}"));
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("Failed to authenticate with the county clerk API"));
                }

                var payload = new { case_number = caseNum, case_id = 0 };
                var externalResponse = apiCtl.CallExternalApi(api, token, payload, HttpMethod.Post).Result;
                string responseBody = externalResponse.Content.ReadAsStringAsync().Result;

                if (!externalResponse.IsSuccessStatusCode)
                {
                    Exceptions.LogException(new Exception(
                        $"SearchCaseNumberDetails: clerk returned {(int)externalResponse.StatusCode} for court {courtId}, case '{caseNum}': {responseBody}"));
                    return Request.CreateResponse(externalResponse.StatusCode,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure($"The clerk API returned an error ({(int)externalResponse.StatusCode}). {responseBody}"));
                }

                // The clerk returns a JSON array; deserialise and project to our normalised model.
                var rawItems = JsonConvert.DeserializeObject<List<ClerkCaseRaw>>(responseBody);
                if (rawItems == null || rawItems.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure("No cases found for the supplied case number"));

                var results = rawItems.Select(r => r.ToViewModel()).ToList();
                return Request.CreateResponse(HttpStatusCode.OK,
                    ClerkApiResult<List<ClerkCaseResult>>.Success(results));
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    ClerkApiResult<List<ClerkCaseResult>>.Failure("Internal server error while searching case details"));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> CancelEvent(long p1)
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

                // Snapshot the timeslot links before deletion so we can restore them on rollback.
                var timeslotEvents = teCtl.GetTimeslotEventsByEvent(p1).ToList();

                foreach (var te in timeslotEvents)
                {
                    teCtl.DeleteTimeslotEvent(te.id, false);
                }
                eventToCancel.cancellation_reason = reason;
                eventToCancel.status_id = cancelledStatus != null ? cancelledStatus.id : (long?)null;
                eventToCancel.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToCancel);

                // Call the clerk API and block the response on the result.
                try
                {
                    var ctx = await ResolveClerkContext(p1);
                    var clerkPayload = new
                    {
                        clerk_case_id = eventToCancel.clerk_case_id,
                        clerk_event_id = eventToCancel.clerk_event_id,
                        cancellation_reason = reason
                    };
                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.CancelEvent, clerkPayload, HttpMethod.Post);
                    if (!clerkResponse.IsSuccessStatusCode)
                    {
                        string clerkError = await clerkResponse.Content.ReadAsStringAsync();
                        Exceptions.LogException(new Exception(
                            $"CancelEvent: clerk API returned {(int)clerkResponse.StatusCode} for event {p1}: {clerkError}"));

                        // Verify whether the clerk applied the cancellation despite the error.
                        bool clerkAlreadyCancelled = await VerifyClerkEventChange(
                            ctx,
                            eventToCancel.clerk_event_id,
                            body => string.Equals(body["status"]?.Value<string>(), "Cancelled", StringComparison.OrdinalIgnoreCase));

                        if (!clerkAlreadyCancelled)
                        {
                            // Roll back: restore the event to its pre-cancel state and re-link timeslots.
                            var originalEvent = ctl.GetEvent(p1); // re-fetch to avoid mutation issues
                            if (originalEvent != null)
                            {
                                originalEvent.cancellation_reason = null;
                                originalEvent.status_id = null; // restore to no-status; caller may want a specific rollback status
                                originalEvent.updated_at = DateTime.Now;
                                ctl.UpdateEvent(originalEvent);
                            }
                            foreach (var te in timeslotEvents)
                            {
                                te.id = 0; // force insert
                                teCtl.CreateTimeslotEvent(te);
                            }
                            return Request.CreateResponse(HttpStatusCode.BadGateway, new EventCancelResult
                            {
                                cancelled = false,
                                error = $"The clerk API rejected the cancellation (HTTP {(int)clerkResponse.StatusCode}). The hearing was not cancelled. Details: {clerkError}"
                            });
                        }
                        Exceptions.LogException(new Exception(
                            $"CancelEvent: clerk returned an error but GetEvent confirms cancellation was applied for event {p1}. Local save kept."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    // Roll back both the event and the timeslot deletions.
                    var originalEvent = ctl.GetEvent(p1);
                    if (originalEvent != null)
                    {
                        originalEvent.cancellation_reason = null;
                        originalEvent.status_id = null;
                        originalEvent.updated_at = DateTime.Now;
                        ctl.UpdateEvent(originalEvent);
                    }
                    foreach (var te in timeslotEvents)
                    {
                        te.id = 0;
                        teCtl.CreateTimeslotEvent(te);
                    }
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway, new EventCancelResult
                    {
                        cancelled = false,
                        error = $"Clerk API configuration error: {configEx.Message} The hearing was not cancelled."
                    });
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
        public async System.Threading.Tasks.Task<HttpResponseMessage> CreateEvent(JObject p1)
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
                    updated_at = eventViewModel.updated_at,
                    clerk_case_id = eventViewModel.clerk_case_id,
                    clerk_event_id = eventViewModel.clerk_event_id
                };

                // For a new event there is no local record yet, so we resolve the clerk context
                // from the timeslot's court rather than the event id.  Build a temporary context
                // using the court_id carried in the view model's timeslot.
                if (eventViewModel.timeslot_id > 0)
                {
                    try
                    {
                        var tsCtl = new TimeslotController();
                        var courtCtl = new CourtController();
                        var timeslot = tsCtl.GetTimeslot(eventViewModel.timeslot_id);
                        var courtId = eventViewModel.court_id > 0 ? eventViewModel.court_id : 0;
                        var court = courtId > 0 ? courtCtl.GetCourt(courtId) : null;
                        var county = court != null ? new CountyController().GetCounty(court.county_id) : null;

                        if (county != null)
                        {
                            var apiCtl = new ApiEndpointController();
                            string token = !string.IsNullOrWhiteSpace(county.decrypted_token)
                                ? county.decrypted_token
                                : await apiCtl.GetJwtToken(county);

                            if (!string.IsNullOrWhiteSpace(token))
                            {
                                var api = apiCtl.GetApiEndpointByCountyAndType(county.id, (int)ApiEndpointType.AddEvent);
                                if (api != null)
                                {
                                    var clerkPayload = new
                                    {
                                        clerk_case_id = evt.clerk_case_id,
                                        clerk_event_id = evt.clerk_event_id,
                                        case_num = evt.case_num,
                                        notes = evt.notes
                                    };
                                    var clerkResponse = await apiCtl.CallExternalApi(api, token, clerkPayload, HttpMethod.Post);
                                    string clerkBody = await clerkResponse.Content.ReadAsStringAsync();

                                    // AddEvent returns HTTP 201 on success with { "EventId": nnn, "error": "" }
                                    if (!clerkResponse.IsSuccessStatusCode)
                                    {
                                        Exceptions.LogException(new Exception(
                                            $"CreateEvent: clerk AddEvent returned {(int)clerkResponse.StatusCode}: {clerkBody}"));
                                        return Request.CreateResponse(HttpStatusCode.BadGateway,
                                            ClerkApiResult<ClerkAddEventResponse>.Failure(
                                                $"The clerk API rejected the new hearing (HTTP {(int)clerkResponse.StatusCode}). The hearing was not saved. {clerkBody}"));
                                    }

                                    // Parse the clerk's EventId and carry it into our local record.
                                    var addRaw = JsonConvert.DeserializeObject<ClerkAddEventRaw>(clerkBody);
                                    if (!string.IsNullOrWhiteSpace(addRaw?.Error))
                                    {
                                        // Clerk returned 201 but embedded an error message in the body.
                                        Exceptions.LogException(new Exception(
                                            $"CreateEvent: clerk AddEvent returned 201 but body contains error: {addRaw.Error}"));
                                        return Request.CreateResponse(HttpStatusCode.BadGateway,
                                            ClerkApiResult<ClerkAddEventResponse>.Failure(
                                                $"The clerk API reported an error: {addRaw.Error}. The hearing was not saved."));
                                    }
                                    if (addRaw?.EventId > 0)
                                    {
                                        // Store the clerk's assigned EventId on our event so future
                                        // UpdateEvent / CancelEvent / RescheduleEvent calls can reference it.
                                        evt.clerk_event_id = addRaw.EventId;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception clerkEx)
                    {
                        Exceptions.LogException(new Exception("CreateEvent: clerk pre-check failed.", clerkEx));
                        return Request.CreateResponse(HttpStatusCode.BadGateway, new
                        {
                            status = 502,
                            message = "Could not reach the clerk API to add the hearing. The hearing was not saved.",
                            details = clerkEx.Message
                        });
                    }
                }

                // Clerk accepted (or no clerk endpoint configured) — commit locally.
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
        public async System.Threading.Tasks.Task<HttpResponseMessage> UpdateEvent(JObject p1)
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

                // Call the clerk API and block the response on the result.
                try
                {
                    var ctx = await ResolveClerkContext(evt.id);
                    var clerkPayload = new
                    {
                        clerk_case_id = evt.clerk_case_id,
                        clerk_event_id = evt.clerk_event_id,
                        case_num = evt.case_num,
                        notes = evt.notes
                    };
                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.UpdateEvent, clerkPayload, HttpMethod.Post);
                    string clerkBody = await clerkResponse.Content.ReadAsStringAsync();

                    // UpdateEvent returns HTTP 200 on success; on failure the body is { "error": "..." }.
                    // Treat both a non-2xx HTTP status AND a non-empty "error" field as failures.
                    var clerkWriteResult = string.IsNullOrWhiteSpace(clerkBody)
                        ? new ClerkWriteErrorRaw()
                        : JsonConvert.DeserializeObject<ClerkWriteErrorRaw>(clerkBody) ?? new ClerkWriteErrorRaw();

                    if (!clerkResponse.IsSuccessStatusCode || !clerkWriteResult.IsSuccess)
                    {
                        string clerkError = !clerkWriteResult.IsSuccess ? clerkWriteResult.Error : clerkBody;
                        Exceptions.LogException(new Exception(
                            $"UpdateEvent: clerk API returned {(int)clerkResponse.StatusCode} for event {evt.id}: {clerkError}"));

                        // Verify whether the clerk applied the change despite returning an error.
                        bool clerkAlreadyUpdated = await VerifyClerkEventChange(
                            ctx,
                            evt.clerk_event_id,
                            body => string.Equals(body["Notes"]?.Value<string>(), evt.notes, StringComparison.Ordinal));

                        if (!clerkAlreadyUpdated)
                        {
                            // Roll back local change.
                            ctl.UpdateEvent(existingEvent);
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                ClerkApiResult<ClerkWriteAckResponse>.Failure(
                                    $"The clerk API rejected the update: {clerkError}. Your changes were not saved."));
                        }
                        // Change verified on clerk side — local save stands; log and continue.
                        Exceptions.LogException(new Exception(
                            $"UpdateEvent: clerk returned an error but GetEvent confirms the change was applied for event {evt.id}. Local save kept."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    // Clerk configuration missing — roll back and block.
                    ctl.UpdateEvent(existingEvent);
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway,
                        ClerkApiResult<ClerkWriteAckResponse>.Failure(
                            $"Clerk API configuration error: {configEx.Message} Your changes were not saved."));
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    ClerkApiResult<ClerkWriteAckResponse>.Success(new ClerkWriteAckResponse { Message = "Event updated successfully" }));
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
        public async System.Threading.Tasks.Task<HttpResponseMessage> RescheduleEvent(RescheduleModel p1)
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

                if (timeslotEvent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "No timeslots found for event." });
                }

                var currentTimeslot = tsCtl.GetTimeslot(timeslotEvent.timeslot_id.Value);
                var selectedtimeslot = tsCtl.GetTimeslot(p1.timeslot_id);
                var selectedtimeslotEvents = teCtl.GetTimeslotEventsByTimeslot(selectedtimeslot.id);
                int eventCount = selectedtimeslotEvents.Count();
                if (eventCount >= selectedtimeslot.quantity)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "The selected timeslot has no space available for event assignment." });
                }
                var currentDuration = currentTimeslot.duration;
                var selectedDuration = (selectedtimeslot.duration);
                if (selectedDuration != currentDuration)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Selected duration must match original hearing duration." });
                }

                // Snapshot the original timeslot_id for rollback.
                long originalTimeslotId = timeslotEvent.timeslot_id.Value;
                long originalStatusId = eventToReschedule.status_id ?? 0;

                timeslotEvent.timeslot_id = selectedtimeslot.id;
                teCtl.UpdateTimeslotEvent(timeslotEvent);
                eventToReschedule.status_id = rescheduledStatus != null ? rescheduledStatus.id : (long?)null;
                eventToReschedule.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToReschedule);

                // Call the clerk API and block the response on the result.
                try
                {
                    var ctx = await ResolveClerkContext(p1.event_id);
                    var clerkPayload = new
                    {
                        clerk_case_id = eventToReschedule.clerk_case_id,
                        clerk_event_id = eventToReschedule.clerk_event_id,
                        new_start = selectedtimeslot.start,
                        new_end = selectedtimeslot.end
                    };
                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.RescheduleEvent, clerkPayload, HttpMethod.Post);
                    if (!clerkResponse.IsSuccessStatusCode)
                    {
                        string clerkError = await clerkResponse.Content.ReadAsStringAsync();
                        Exceptions.LogException(new Exception(
                            $"RescheduleEvent: clerk API returned {(int)clerkResponse.StatusCode} for event {p1.event_id}: {clerkError}"));

                        // Verify whether the clerk applied the reschedule despite the error.
                        bool clerkAlreadyRescheduled = await VerifyClerkEventChange(
                            ctx,
                            eventToReschedule.clerk_event_id,
                            body =>
                            {
                                var clerkStart = body["start"]?.Value<DateTime?>();
                                return clerkStart.HasValue &&
                                       Math.Abs((clerkStart.Value - selectedtimeslot.start).TotalSeconds) < 60;
                            });

                        if (!clerkAlreadyRescheduled)
                        {
                            // Roll back: restore the timeslot link and the event status.
                            timeslotEvent.timeslot_id = originalTimeslotId;
                            teCtl.UpdateTimeslotEvent(timeslotEvent);
                            eventToReschedule.status_id = originalStatusId > 0 ? originalStatusId : (long?)null;
                            eventToReschedule.updated_at = DateTime.Now;
                            ctl.UpdateEvent(eventToReschedule);
                            return Request.CreateResponse(HttpStatusCode.BadGateway, new
                            {
                                status = 502,
                                message = $"The clerk API rejected the reschedule (HTTP {(int)clerkResponse.StatusCode}). The hearing was not rescheduled.",
                                details = clerkError
                            });
                        }
                        Exceptions.LogException(new Exception(
                            $"RescheduleEvent: clerk returned an error but GetEvent confirms reschedule was applied for event {p1.event_id}. Local save kept."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    // Roll back.
                    timeslotEvent.timeslot_id = originalTimeslotId;
                    teCtl.UpdateTimeslotEvent(timeslotEvent);
                    eventToReschedule.status_id = originalStatusId > 0 ? originalStatusId : (long?)null;
                    eventToReschedule.updated_at = DateTime.Now;
                    ctl.UpdateEvent(eventToReschedule);
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway, new
                    {
                        status = 502,
                        message = $"Clerk API configuration error: {configEx.Message} The hearing was not rescheduled."
                    });
                }

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
        internal class CaseSearchModel
        {
            public string casePattern { get; set; }
            public int userId { get; set; }
            public bool isJudge { get; set; }
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
                case "12": return "courtroom";
                default: return "case_num";
            }
        }

        /// <summary>
        /// Shared context resolved once per request: county, decrypted token, and ApiEndpointController.
        /// Passed between helpers so the DB/decryption work is not repeated.
        /// </summary>
        private class ClerkContext
        {
            public County County { get; set; }
            public string Token { get; set; }
            public ApiEndpointController ApiCtl { get; set; }
        }

        /// <summary>
        /// Resolves the court → county chain for a given event id, obtains the decrypted token,
        /// and returns a ClerkContext ready for API calls.  Throws InvalidOperationException with
        /// a descriptive message when any required piece of configuration is missing.
        /// </summary>
        private async System.Threading.Tasks.Task<ClerkContext> ResolveClerkContext(long eventId)
        {
            var evtCtl = new EventController();
            var courtCtl = new CourtController();

            long courtId = evtCtl.GetCourtIdByEventId(eventId);
            if (courtId <= 0)
                throw new InvalidOperationException(
                    $"Could not resolve a court for event {eventId}.");

            var court = courtCtl.GetCourt(courtId);
            if (court == null)
                throw new InvalidOperationException(
                    $"Court {courtId} not found for event {eventId}.");

            var county = new CountyController().GetCounty(court.county_id);
            if (county == null)
                throw new InvalidOperationException(
                    $"County {court.county_id} not found for court {courtId}.");

            var apiCtl = new ApiEndpointController();

            // Prefer the stored static token (decrypted_token); fall back to JWT auth.
            string token = !string.IsNullOrWhiteSpace(county.decrypted_token)
                ? county.decrypted_token
                : await apiCtl.GetJwtToken(county);

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    $"Failed to obtain an auth token for county {county.id}, event {eventId}.");

            return new ClerkContext { County = county, Token = token, ApiCtl = apiCtl };
        }

        /// <summary>
        /// Calls the clerk API for the given endpoint type.  Throws InvalidOperationException
        /// when the endpoint is not configured or the HTTP call itself fails to send.
        /// Returns the HttpResponseMessage (which may be a non-2xx clerk error) so callers
        /// can inspect the status and body.
        /// </summary>
        private async System.Threading.Tasks.Task<HttpResponseMessage> CallClerkApi(
            ClerkContext ctx,
            ApiEndpointType endpointType,
            object payload,
            HttpMethod method)
        {
            var api = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)endpointType);
            if (api == null)
                throw new InvalidOperationException(
                    $"No {endpointType} endpoint configured for county {ctx.County.id}.");

            return await ctx.ApiCtl.CallExternalApi(api, ctx.Token, payload, method);
        }

        /// <summary>
        /// After a clerk write call fails, calls GetEvent on the clerk API to check whether the
        /// change was actually applied despite the error response.  Returns true if the clerk's
        /// record reflects the expected state (change silently succeeded), false otherwise.
        /// Any exception during the verification is swallowed and logged — verification failure
        /// is never allowed to mask the original error.
        /// </summary>
        private async System.Threading.Tasks.Task<bool> VerifyClerkEventChange(
            ClerkContext ctx,
            long clerkEventId,
            Func<JObject, bool> stateMatches)
        {
            try
            {
                var getApi = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)ApiEndpointType.GetEvent);
                if (getApi == null)
                {
                    Exceptions.LogException(new Exception(
                        $"VerifyClerkEventChange: no GetEvent endpoint configured for county {ctx.County.id}."));
                    return false;
                }

                var getPayload = new { clerk_event_id = clerkEventId };
                var getResponse = await ctx.ApiCtl.CallExternalApi(getApi, ctx.Token, getPayload, HttpMethod.Post);

                if (getResponse == null || !getResponse.IsSuccessStatusCode)
                    return false;

                var body = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                return stateMatches(body);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"VerifyClerkEventChange: exception during verification for clerk_event_id {clerkEventId}.", ex));
                return false;
            }
        }
    }
}