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
            var query = Request.GetQueryNameValuePairs()
                               .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
            string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : "";
            long courtId = query.ContainsKey("courtId") && long.TryParse(query["courtId"], out long cId) ? cId : 0;
            long courtroomId = query.ContainsKey("courtroomId") && long.TryParse(query["courtroomId"], out long catId) ? catId : 0;
            long statusId = query.ContainsKey("statusId") && long.TryParse(query["statusId"], out long statId) ? statId : 0;

            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "50", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "case_num";
            string sortDirection = "asc";
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
                events = ctl.GetEventListItems(userId, searchTerm, courtId, courtroomId, statusId,
                                               recordOffset, pageSize, sortColumn, sortDirection)
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
                var query = Request.GetQueryNameValuePairs()
                                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
                bool isJudge = query.ContainsKey("isJudge") && bool.TryParse(query["isJudge"], out bool judge) && judge;

                var ctl = new EventController();
                var events = new List<EventViewModel>();

                if (UserInfo.IsAdmin)
                    events = ctl.GetEventsForDashBoardByAdmin().Select(e => new EventViewModel(e)).ToList();
                else if (isJudge)
                    events = ctl.GetEventsForDashboardByJudge(userId).Select(e => new EventViewModel(e)).ToList();
                else
                    events = ctl.GetEventsForDashboard(userId).Select(e => new EventViewModel(e)).ToList();

                return Request.CreateResponse(new { data = events });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
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
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new EventSearchResult { data = null, error = "Event not found" });

                return Request.CreateResponse(HttpStatusCode.OK,
                    new EventSearchResult { data = new EventViewModel(evt), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new EventSearchResult { data = null, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventsForTimeslot(long p1)
        {
            try
            {
                var ctl = new EventController();
                var events = ctl.GetEventsByTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new EventsResult { data = events.Select(e => new EventViewModel(e)).ToList(), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEventListItemsForTimeslot(long p1)
        {
            try
            {
                var ctl = new EventController();
                var events = ctl.GetEventListItemsByTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new EventsResult { data = events.Select(e => new EventViewModel(e)).ToList(), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
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
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new { status = 404, message = "No timeslots found for event." });

                var timeslots = timeslotEvents.Select(te => tsCtl.GetTimeslot(te.timeslot_id.Value))
                                                  .OrderBy(ts => ts.start).ToList();
                var totalDuration = (timeslots.Last().end - timeslots.First().start).TotalMinutes;
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { status = 200, duration = totalDuration });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

        // -----------------------------------------------------------------------
        // CASE SEARCH ENDPOINTS
        // -----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SearchCaseNumber([FromBody] JObject p1)
        {
            try
            {
                var ctl = new EventController();
                var caseNumber = p1.ToObject<SearchTerm>();

                if (string.IsNullOrWhiteSpace(caseNumber.searchTerm))
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Case number is required." });

                Event eventData = ctl.GetEventByCaseNumber(caseNumber.searchTerm);
                if (eventData == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new EventSearchResult { data = null, error = "No Event Found" });

                return Request.CreateResponse(HttpStatusCode.OK,
                    new EventSearchResult { data = new EventViewModel(eventData), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new EventSearchResult { data = null, error = ex.Message });
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { message = "Case number is required" });

                var ctl = new EventController();
                var events = ctl.GetEventsByCasePattern(model.casePattern, model.userId, model.isJudge);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { data = events.Select(e => new EventViewModel(e)).ToList() });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { error = ex.Message });
            }
        }

        /// <summary>
        /// Calls the clerk's GetCase endpoint for a given case number and court.
        /// Called by the UI (evaluateCaseNumberFields in courtCalendar.js) when the user
        /// finishes entering a case number in the event tab.  The result list is presented
        /// to the user for selection; the chosen case populates the event form fields
        /// (plaintiff, defendant, emails, clerk_case_id, etc.) before the user saves.
        /// CreateEvent therefore receives all case data pre-populated and does NOT call
        /// this endpoint a second time.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> SearchCaseNumberDetails([FromBody] JObject p1)
        {
            try
            {
                string caseNum = p1["caseNum"]?.Value<string>();
                long courtId = p1["courtId"]?.Value<long>() ?? 0;

                if (string.IsNullOrWhiteSpace(caseNum) || courtId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure(
                            "caseNum and courtId are required in the request body"));

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
                        ClerkApiResult<List<ClerkCaseResult>>.Failure(
                            "GetCase API endpoint is not configured for this county"));

                string token = !string.IsNullOrWhiteSpace(county.decrypted_token)
                    ? county.decrypted_token
                    : await apiCtl.GetJwtToken(county);

                if (string.IsNullOrWhiteSpace(token))
                {
                    Exceptions.LogException(new Exception(
                        $"SearchCaseNumberDetails: failed to obtain token for county {county.id}, court {courtId}"));
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure(
                            "Failed to authenticate with the county clerk API"));
                }

                var payload = new { CaseNumber = caseNum, CaseId = 0 };
                var externalResponse = await apiCtl.CallExternalApi(api, token, payload, HttpMethod.Post,
                    BuildLogContext(action: ApiEndpointType.GetCase.ToString()));
                string responseBody = await externalResponse.Content.ReadAsStringAsync();

                if (!externalResponse.IsSuccessStatusCode)
                {
                    Exceptions.LogException(new Exception(
                        $"SearchCaseNumberDetails: clerk returned {(int)externalResponse.StatusCode} " +
                        $"for court {courtId}, case '{caseNum}': {responseBody}"));
                    return Request.CreateResponse(externalResponse.StatusCode,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure(
                            $"The clerk API returned an error ({(int)externalResponse.StatusCode}). {responseBody}"));
                }

                var rawItems = JsonConvert.DeserializeObject<List<ClerkCaseRaw>>(responseBody);
                if (rawItems == null || rawItems.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        ClerkApiResult<List<ClerkCaseResult>>.Failure(
                            "No cases found for the supplied case number"));

                var results = rawItems.Select(r => r.ToViewModel()).ToList();

                // Make sure every attorney bar number the Clerk returned is present
                // in the JACS attorneys table before the UI populates the dropdowns.
                // For any missing bar number, fetch from the Florida Bar API and
                // insert. The dropdown query runs after this returns and will see
                // the new rows. API failures are non-fatal — the dropdown just
                // won't have a match for that bar number.
                var attorneyCtl = new AttorneyController();
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var r in results)
                {
                    if (!string.IsNullOrWhiteSpace(r.PetitionerAttyBar) && seen.Add(r.PetitionerAttyBar))
                    {
                        await attorneyCtl.EnsureAttorneyByBarNumberAsync(r.PetitionerAttyBar);
                    }
                    if (!string.IsNullOrWhiteSpace(r.RespondentAttyBar) && seen.Add(r.RespondentAttyBar))
                    {
                        await attorneyCtl.EnsureAttorneyByBarNumberAsync(r.RespondentAttyBar);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    ClerkApiResult<List<ClerkCaseResult>>.Success(results));
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    ClerkApiResult<List<ClerkCaseResult>>.Failure(
                        "Internal server error while searching case details"));
            }
        }        // Order of operations:
        //   1. Validate the incoming EventViewModel.
        //      Case data (clerk_case_id, plaintiff, defendant, emails, telephone,
        //      notes) is already pre-populated by the UI: evaluateCaseNumberFields
        //      calls SearchCaseNumberDetails, the user selects a case, and
        //      populateEventFromClerkCase fills the form before Save is clicked.
        //      We trust and use that data directly — no second GetCase call needed.
        //   2. Call AddEvent on the clerk API using the pre-populated case data.
        //      Block the local save if the clerk rejects it.
        //   3. Persist locally only after the clerk call succeeds (or if no clerk
        //      endpoint is configured for this county).
        // -----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> CreateEvent(JObject p1)
        {
            try
            {
                var eventViewModel = p1.ToObject<EventViewModel>();
                var validationError = ValidateEventRequiredFields(eventViewModel);
                if (validationError != null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = validationError });

                eventViewModel.plaintiff_email = eventViewModel.plaintiff_email?.Replace(";", ",");
                eventViewModel.defendant_email = eventViewModel.defendant_email?.Replace(";", ",");

                var ctlStatus = new EventStatusController();
                var scheduledStatus = ctlStatus.GetEventStatusByName("Scheduled");

                // Build the local Event object from the view model (not yet saved).
                // clerk_case_id and all party fields are already populated by the UI
                // from the user's case selection via SearchCaseNumberDetails.
                Event evt = new Event
                {
                    case_num = eventViewModel.case_num,
                    notes = eventViewModel.notes,
                    plaintiff = eventViewModel.plaintiff,
                    defendant = eventViewModel.defendant,
                    motion_id = eventViewModel.motion_id > 0 ? (long?)eventViewModel.motion_id : null,
                    attorney_id = eventViewModel.attorney_id > 0 ? (long?)eventViewModel.attorney_id : null,
                    type_id = eventViewModel.type_id > 0 ? (long?)eventViewModel.type_id : null,
                    status_id = scheduledStatus?.id,
                    reminder = eventViewModel.reminder,
                    opp_attorney_id = eventViewModel.opp_attorney_id > 0 ? (long?)eventViewModel.opp_attorney_id : null,
                    owner_id = eventViewModel.owner_id > 0 ? (long?)eventViewModel.owner_id : null,
                    owner_type = eventViewModel.owner_type,
                    // Capture the current DNN user's username so "Edited By"
                    // can be shown across the public + internal portals (where
                    // owner_id isn't comparable but Username is the same login).
                    owner_username = UserInfo?.Username,
                    addon = eventViewModel.addon,
                    plaintiff_email = eventViewModel.plaintiff_email,
                    defendant_email = eventViewModel.defendant_email,
                    cancellation_reason = eventViewModel.cancellation_reason,
                    template = eventViewModel.template,
                    telephone = eventViewModel.telephone,
                    custom_motion = eventViewModel.custom_motion,
                    clerk_case_id = eventViewModel.clerk_case_id,   // set by populateEventFromClerkCase in UI
                    clerk_event_id = eventViewModel.clerk_event_id,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                // ------------------------------------------------------------------
                // Step 1 – If a timeslot and court are present, call the clerk's
                // AddEvent endpoint.  We skip the clerk entirely for courts that
                // have no configured clerk integration so those courts keep working.
                // ------------------------------------------------------------------
                if (eventViewModel.timeslot_id > 0 && eventViewModel.court_id > 0)
                {
                    // Courtroom is required for event creation — enforce via the timeslot.
                    var createTs = new TimeslotController().GetTimeslot(eventViewModel.timeslot_id);
                    if (createTs != null && (createTs.courtroom_id == null || createTs.courtroom_id <= 0))
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            new { status = 400, message = "Courtroom is required." });

                    ClerkContext ctx;
                    try
                    {
                        ctx = await ResolveClerkContextByCourt(eventViewModel.court_id);
                    }
                    catch (InvalidOperationException configEx)
                    {
                        Exceptions.LogException(configEx);
                        return Request.CreateResponse(HttpStatusCode.BadGateway,
                            new { status = 502, message = $"Clerk API configuration error: {configEx.Message} The hearing was not saved." });
                    }

                    var addEventApi = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)ApiEndpointType.AddEvent);
                    if (addEventApi != null)
                    {
                        // Fetch the timeslot so we can include start/end and duration in the payload.
                        var timeslot = new TimeslotController().GetTimeslot(eventViewModel.timeslot_id);

                        // Resolve all clerk-side ids and UDF via shared helper
                        var clerkEventData = await BuildClerkEventPayload(evt, eventViewModel.court_id, ctx.County.id,
                            overrideTimeslot: timeslot);

                        // Build the AddEvent payload with all required clerk fields.
                        var clerkPayload = new
                        {
                            CaseId         = evt.clerk_case_id > 0 ? (long?)evt.clerk_case_id : null,
                            JudgeId        = clerkEventData.JudgeId,
                            Action         = "New",
                            EventType      = clerkEventData.EventType,
                            OtherEventType = clerkEventData.OtherEventType,
                            EventDateTime  = clerkEventData.EventDateTime,
                            Duration       = clerkEventData.Duration,
                            CourtRoomId    = clerkEventData.CourtRoomId,
                            Notes          = evt.notes,
                            UDF            = clerkEventData.UDF
                        };

                        HttpResponseMessage clerkResponse;
                        string clerkBody;
                        try
                        {
                            clerkResponse = await ctx.ApiCtl.CallExternalApi(addEventApi, ctx.Token, clerkPayload, HttpMethod.Post,
                                BuildLogContext(caseId: evt.clerk_case_id, action: ApiEndpointType.AddEvent.ToString()));
                            clerkBody = await clerkResponse.Content.ReadAsStringAsync();
                        }
                        catch (Exception clerkEx)
                        {
                            Exceptions.LogException(new Exception("CreateEvent: AddEvent HTTP call failed.", clerkEx));
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                new { status = 502, message = "Could not reach the clerk API to add the hearing. The hearing was not saved.", details = clerkEx.Message });
                        }

                        if (!clerkResponse.IsSuccessStatusCode)
                        {
                            Exceptions.LogException(new Exception(
                                $"CreateEvent: AddEvent returned {(int)clerkResponse.StatusCode} for case '{evt.case_num}': {clerkBody}"));
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                ClerkApiResult<ClerkAddEventResponse>.Failure(
                                    $"The clerk API rejected the new hearing (HTTP {(int)clerkResponse.StatusCode}). The hearing was not saved. {clerkBody}"));
                        }

                        // AddEvent returns HTTP 201 with { "EventId": nnn, "error": "" }.
                        // An embedded error string on a 201 is also treated as a failure.
                        var addRaw = JsonConvert.DeserializeObject<ClerkAddEventRaw>(clerkBody);
                        if (!string.IsNullOrWhiteSpace(addRaw?.Error))
                        {
                            Exceptions.LogException(new Exception(
                                $"CreateEvent: AddEvent returned 201 but body contains error: {addRaw.Error}"));
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                ClerkApiResult<ClerkAddEventResponse>.Failure(
                                    $"The clerk API reported an error: {addRaw.Error}. The hearing was not saved."));
                        }

                        // Store the clerk-assigned EventId so future Update/Reschedule/Cancel calls can reference it.
                        if (addRaw?.EventId > 0)
                            evt.clerk_event_id = addRaw.EventId;
                    }
                }

                // ------------------------------------------------------------------
                // Step 2 – Clerk accepted (or no clerk endpoint is configured).
                // Persist locally.
                // ------------------------------------------------------------------
                var ctl = new EventController();
                ctl.CreateEvent(evt);

                if (evt.id > 0 && eventViewModel.timeslot_id > 0)
                {
                    new TimeslotEventController().CreateTimeslotEvent(new TimeslotEvent
                    {
                        event_id = evt.id,
                        timeslot_id = eventViewModel.timeslot_id,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { status = 200, message = "Event created successfully", clerk_event_id = evt.clerk_event_id, clerk_case_id = evt.clerk_case_id });
            }
            catch (ValidationException vex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { status = 400, message = vex.Message });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

        // -----------------------------------------------------------------------
        // UPDATE EVENT
        // Order of operations:
        //   1. Validate and load the existing record.
        //   2. Call UpdateEvent on the clerk API BEFORE saving locally.
        //      • On clerk success  → save locally and return 200.
        //      • On clerk failure  → call GetEvent to confirm whether the change
        //        was silently applied.  If confirmed, save locally; otherwise
        //        leave the local record unchanged and return 502.
        // -----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> UpdateEvent(JObject p1)
        {
            try
            {
                var evt = p1.ToObject<Event>();

                if (evt.id <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Event ID is required for update." });

                var updateValidationError = ValidateEventRequiredFields(evt);
                if (updateValidationError != null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = updateValidationError });

                var ctl = new EventController();
                var existingEvent = ctl.GetEvent(evt.id);
                if (existingEvent == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new { status = 404, message = "Event not found." });

                // Verify the current timeslot has a courtroom assigned.
                var teCtl = new TimeslotEventController();
                var tsCtlVerify = new TimeslotController();
                var evtTimeslotEvt = teCtl.GetTimeslotEventsByEvent(evt.id).FirstOrDefault();
                if (evtTimeslotEvt?.timeslot_id != null)
                {
                    var evtTimeslot = tsCtlVerify.GetTimeslot(evtTimeslotEvt.timeslot_id.Value);
                    if (evtTimeslot != null && (evtTimeslot.courtroom_id == null || evtTimeslot.courtroom_id <= 0))
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            new { status = 400, message = "Courtroom is required." });
                }

                // Normalise email separators and template before the clerk call
                // so the payload matches what we will persist.
                evt.plaintiff_email = p1["plaintiff_email"]?.ToString().Replace(";", ",");
                evt.defendant_email = p1["defendant_email"]?.ToString().Replace(";", ",");
                evt.template = p1["template"]?.ToString();
                evt.updated_at = DateTime.Now;
                // Stamp the editor's DNN username so "Edited By" reflects whichever
                // portal the change came from. The browser-supplied payload's
                // owner_username is intentionally ignored.
                evt.owner_username = UserInfo?.Username;

                // ------------------------------------------------------------------
                // Step 1 – Call the clerk's UpdateEvent endpoint BEFORE saving locally.
                // ------------------------------------------------------------------
                try
                {
                    var ctx = await ResolveClerkContext(evt.id);

                    // Resolve xref ids for this county
                    long courtId = new EventController().GetCourtIdByEventId(evt.id);
                    var clerkEventData = await BuildClerkEventPayload(evt, courtId, ctx.County.id);

                    var clerkPayload = new
                    {
                        EventId        = evt.clerk_event_id,
                        CaseId         = evt.clerk_case_id > 0 ? (long?)evt.clerk_case_id : null,
                        JudgeId        = clerkEventData.JudgeId,
                        Action         = "Update",
                        EventType      = clerkEventData.EventType,
                        OtherEventType = clerkEventData.OtherEventType,
                        EventDateTime  = clerkEventData.EventDateTime,
                        Duration       = clerkEventData.Duration,
                        CourtRoomId    = clerkEventData.CourtRoomId,
                        Notes          = evt.notes,
                        UDF            = clerkEventData.UDF,
                        Reason         = p1["cancellation_reason"]?.ToString()
                                         ?? p1["reason"]?.ToString()
                                         ?? string.Empty
                    };

                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.UpdateEvent, clerkPayload, HttpMethod.Post,
                        BuildLogContext(eventId: evt.id, caseId: evt.clerk_case_id));
                    string clerkBody = await clerkResponse.Content.ReadAsStringAsync();

                    var clerkWriteResult = string.IsNullOrWhiteSpace(clerkBody)
                        ? new ClerkWriteErrorRaw()
                        : JsonConvert.DeserializeObject<ClerkWriteErrorRaw>(clerkBody) ?? new ClerkWriteErrorRaw();

                    if (!clerkResponse.IsSuccessStatusCode || !clerkWriteResult.IsSuccess)
                    {
                        string clerkError = !clerkWriteResult.IsSuccess ? clerkWriteResult.Error : clerkBody;
                        Exceptions.LogException(new Exception(
                            $"UpdateEvent: clerk returned {(int)clerkResponse.StatusCode} for event {evt.id}: {clerkError}"));

                        // Recovery: call GetEvent to see if the change landed anyway.
                        bool confirmed = await VerifyClerkEventChange(
                            ctx,
                            evt.clerk_event_id,
                            body => string.Equals(body["Notes"]?.Value<string>(), evt.notes, StringComparison.Ordinal));

                        if (!confirmed)
                        {
                            // Clerk rejected — do NOT save locally.
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                ClerkApiResult<ClerkWriteAckResponse>.Failure(
                                    $"The clerk API rejected the update: {clerkError}. Your changes were not saved."));
                        }

                        // Change confirmed on clerk side despite the error — proceed to local save.
                        Exceptions.LogException(new Exception(
                            $"UpdateEvent: clerk returned an error but GetEvent confirms the change was applied " +
                            $"for event {evt.id}. Proceeding with local save."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    // Clerk not configured — block the save.
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway,
                        ClerkApiResult<ClerkWriteAckResponse>.Failure(
                            $"Clerk API configuration error: {configEx.Message} Your changes were not saved."));
                }

                // ------------------------------------------------------------------
                // Step 2 – Clerk accepted (or confirmed via GetEvent). Save locally.
                // ------------------------------------------------------------------
                ctl.UpdateEvent(evt);

                return Request.CreateResponse(HttpStatusCode.OK,
                    ClerkApiResult<ClerkWriteAckResponse>.Success(
                        new ClerkWriteAckResponse { Message = "Event updated successfully" }));
            }
            catch (ValidationException vex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { status = 400, message = vex.Message });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

        // -----------------------------------------------------------------------
        // RESCHEDULE EVENT
        // Order of operations:
        //   1. Validate the target timeslot (capacity, duration match).
        //   2. Call RescheduleEvent on the clerk API BEFORE saving locally.
        //      • On clerk success  → update timeslot link and status locally.
        //      • On clerk failure  → call GetEvent to confirm.
        //        If confirmed, save locally.
        //        Otherwise return 502 — no local changes are made.
        // -----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> RescheduleEvent(RescheduleModel p1)
        {
            try
            {
                var ctl = new EventController();
                var teCtl = new TimeslotEventController();
                var tsCtl = new TimeslotController();
                var eventStatusCtl = new EventStatusController();

                var eventToReschedule = ctl.GetEvent(p1.event_id);
                if (eventToReschedule == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new { status = 404, message = "Event not found." });

                var timeslotEvent = teCtl.GetTimeslotEventsByEvent(p1.event_id).FirstOrDefault();
                if (timeslotEvent == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "No timeslots found for event." });

                var currentTimeslot = tsCtl.GetTimeslot(timeslotEvent.timeslot_id.Value);
                var selectedTimeslot = tsCtl.GetTimeslot(p1.timeslot_id);

                // Capacity check.
                int eventCount = teCtl.GetTimeslotEventsByTimeslot(selectedTimeslot.id).Count();
                if (eventCount >= selectedTimeslot.quantity)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "The selected timeslot has no space available for event assignment." });

                // Duration match check.
                if (selectedTimeslot.duration != currentTimeslot.duration)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Selected duration must match original hearing duration." });

                // Courtroom is required for a rescheduled hearing — enforce on the
                // target timeslot so we never move an event to a courtroom-less slot.
                if (selectedTimeslot.courtroom_id == null || selectedTimeslot.courtroom_id <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Courtroom is required on the selected timeslot." });

                var rescheduledStatus = eventStatusCtl.GetEventStatusByName("Rescheduled");

                // ------------------------------------------------------------------
                // Step 1 – Call the clerk's RescheduleEvent endpoint BEFORE any
                // local changes are made.
                // ------------------------------------------------------------------
                try
                {
                    var ctx = await ResolveClerkContext(p1.event_id);

                    // Resolve xref ids for both new and current timeslot
                    long courtId = new EventController().GetCourtIdByEventId(p1.event_id);
                    var clerkNew = await BuildClerkEventPayload(eventToReschedule, courtId, ctx.County.id,
                        overrideTimeslot: selectedTimeslot);
                    var clerkCurrent = await BuildClerkEventPayload(eventToReschedule, courtId, ctx.County.id,
                        overrideTimeslot: currentTimeslot);

                    var clerkPayload = new
                    {
                        NewEvent = new
                        {
                            EventId        = eventToReschedule.clerk_event_id,
                            CaseId         = eventToReschedule.clerk_case_id > 0 ? (long?)eventToReschedule.clerk_case_id : null,
                            JudgeId        = clerkNew.JudgeId,
                            Action         = "Modify",
                            EventType      = clerkNew.EventType,
                            OtherEventType = clerkNew.OtherEventType,
                            EventDateTime  = clerkNew.EventDateTime,
                            Duration       = clerkNew.Duration,
                            CourtRoomId    = clerkNew.CourtRoomId,
                            Notes          = eventToReschedule.notes,
                            UDF            = clerkNew.UDF
                        },
                        CurrentEvent = new
                        {
                            EventDateTime  = clerkCurrent.EventDateTime,
                            CourtRoomId    = clerkCurrent.CourtRoomId,
                            Duration       = clerkCurrent.Duration,
                            JudgeId        = clerkCurrent.JudgeId,
                            EventType      = clerkCurrent.EventType,
                            Notes          = eventToReschedule.notes,
                            UDF            = clerkCurrent.UDF
                        },
                        Reason = "Rescheduled"
                    };

                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.RescheduleEvent, clerkPayload, HttpMethod.Post,
                        BuildLogContext(eventId: eventToReschedule.id, caseId: eventToReschedule.clerk_case_id));

                    if (!clerkResponse.IsSuccessStatusCode)
                    {
                        string clerkError = await clerkResponse.Content.ReadAsStringAsync();
                        Exceptions.LogException(new Exception(
                            $"RescheduleEvent: clerk returned {(int)clerkResponse.StatusCode} " +
                            $"for event {p1.event_id}: {clerkError}"));

                        // Recovery: verify via GetEvent whether the reschedule landed anyway.
                        bool confirmed = await VerifyClerkEventChange(
                            ctx,
                            eventToReschedule.clerk_event_id,
                            body =>
                            {
                                var clerkStart = body["EventDateTime"]?.Value<DateTime?>();
                                return clerkStart.HasValue &&
                                       Math.Abs((clerkStart.Value - selectedTimeslot.start).TotalSeconds) < 60;
                            });

                        if (!confirmed)
                        {
                            // Clerk rejected — return without touching local data.
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                new { status = 502, message = $"The clerk API rejected the reschedule (HTTP {(int)clerkResponse.StatusCode}). The hearing was not rescheduled.", details = clerkError });
                        }

                        Exceptions.LogException(new Exception(
                            $"RescheduleEvent: clerk returned an error but GetEvent confirms reschedule was " +
                            $"applied for event {p1.event_id}. Proceeding with local save."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway,
                        new { status = 502, message = $"Clerk API configuration error: {configEx.Message} The hearing was not rescheduled." });
                }

                // ------------------------------------------------------------------
                // Step 2 – Clerk accepted (or confirmed via GetEvent). Save locally.
                // ------------------------------------------------------------------
                timeslotEvent.timeslot_id = selectedTimeslot.id;
                teCtl.UpdateTimeslotEvent(timeslotEvent);

                eventToReschedule.status_id = rescheduledStatus?.id;
                eventToReschedule.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToReschedule);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { status = 200, message = "Event rescheduled successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

        // -----------------------------------------------------------------------
        // CANCEL EVENT
        // Order of operations:
        //   1. Load the event and snapshot timeslot links.
        //   2. Call CancelEvent on the clerk API BEFORE saving locally.
        //      • On clerk success  → soft-delete timeslot links, set status locally.
        //      • On clerk failure  → call GetEvent to confirm.
        //        If confirmed, proceed with local changes.
        //        Otherwise return 502 — no local changes are made.
        // -----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> CancelEvent(long p1)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs()
                                   .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string reason = query.ContainsKey("cancellation_reason") ? query["cancellation_reason"] : string.Empty;

                if (p1 <= 0)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new EventCancelResult { cancelled = false, error = "Event ID is required" });

                var ctl = new EventController();
                var teCtl = new TimeslotEventController();
                var eventStatusCtl = new EventStatusController();

                var eventToCancel = ctl.GetEvent(p1);
                if (eventToCancel == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new EventCancelResult { cancelled = false, error = "Event not found" });

                // Snapshot timeslot links — used only for reference (no pre-deletion).
                var timeslotEvents = teCtl.GetTimeslotEventsByEvent(p1).ToList();
                var cancelledStatus = eventStatusCtl.GetEventStatusByName("Cancelled");

                // ------------------------------------------------------------------
                // Step 1 – Call the clerk's CancelEvent endpoint BEFORE any
                // local changes are made.
                // ------------------------------------------------------------------
                try
                {
                    var ctx = await ResolveClerkContext(p1);

                    var clerkPayload = new
                    {
                        EventId = eventToCancel.clerk_event_id,
                        Reason  = reason
                    };

                    var clerkResponse = await CallClerkApi(ctx, ApiEndpointType.CancelEvent, clerkPayload, HttpMethod.Post,
                        BuildLogContext(eventId: eventToCancel.id, caseId: eventToCancel.clerk_case_id));

                    if (!clerkResponse.IsSuccessStatusCode)
                    {
                        string clerkError = await clerkResponse.Content.ReadAsStringAsync();
                        Exceptions.LogException(new Exception(
                            $"CancelEvent: clerk returned {(int)clerkResponse.StatusCode} for event {p1}: {clerkError}"));

                        // Recovery: verify via GetEvent whether the cancellation landed anyway.
                        bool confirmed = await VerifyClerkEventChange(
                            ctx,
                            eventToCancel.clerk_event_id,
                            body => string.Equals(body["Status"]?.Value<string>(), "Cancelled", StringComparison.OrdinalIgnoreCase));

                        if (!confirmed)
                        {
                            // Clerk rejected — return without touching local data.
                            return Request.CreateResponse(HttpStatusCode.BadGateway,
                                new EventCancelResult
                                {
                                    cancelled = false,
                                    error = $"The clerk API rejected the cancellation (HTTP {(int)clerkResponse.StatusCode}). The hearing was not cancelled. Details: {clerkError}"
                                });
                        }

                        Exceptions.LogException(new Exception(
                            $"CancelEvent: clerk returned an error but GetEvent confirms cancellation was " +
                            $"applied for event {p1}. Proceeding with local save."));
                    }
                }
                catch (InvalidOperationException configEx)
                {
                    Exceptions.LogException(configEx);
                    return Request.CreateResponse(HttpStatusCode.BadGateway,
                        new EventCancelResult
                        {
                            cancelled = false,
                            error = $"Clerk API configuration error: {configEx.Message} The hearing was not cancelled."
                        });
                }

                // ------------------------------------------------------------------
                // Step 2 – Clerk accepted (or confirmed via GetEvent). Save locally.
                // ------------------------------------------------------------------
                foreach (var te in timeslotEvents)
                    teCtl.DeleteTimeslotEvent(te.id, false);

                eventToCancel.cancellation_reason = reason;
                eventToCancel.status_id = cancelledStatus?.id;
                eventToCancel.updated_at = DateTime.Now;
                ctl.UpdateEvent(eventToCancel);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new EventCancelResult { cancelled = true, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new EventCancelResult { cancelled = false, error = ex.Message });
            }
        }


        // -----------------------------------------------------------------------
        // HELPERS
        // -----------------------------------------------------------------------

        /// <summary>
        /// Server-side guard for the business-rule required fields: case number,
        /// event type, and motion.  Returns null when valid, or an error message
        /// string otherwise. Courtroom is enforced via the linked timeslot's
        /// courtroom_id and checked in the CreateEvent/UpdateEvent paths.
        /// </summary>
        private static string ValidateEventRequiredFields(EventViewModel evm)
        {
            if (evm == null) return "Event data is required.";
            if (string.IsNullOrWhiteSpace(evm.case_num)) return "Case number is required.";
            if (evm.type_id <= 0) return "Event type is required.";
            if (evm.motion_id <= 0) return "Motion is required.";
            return null;
        }

        private static string ValidateEventRequiredFields(Event evt)
        {
            if (evt == null) return "Event data is required.";
            if (string.IsNullOrWhiteSpace(evt.case_num)) return "Case number is required.";
            if (!evt.type_id.HasValue || evt.type_id.Value <= 0) return "Event type is required.";
            if (!evt.motion_id.HasValue || evt.motion_id.Value <= 0) return "Motion is required.";
            return null;
        }

        // DataTable column index → event_list view column name. Indices match
        // the columns array in js/event.js (column 0 is the edit-icon button).
        // The names returned MUST be real columns in the event_list view, since
        // tjc_jacs_get_event_list_paged appends them straight into ORDER BY.
        private string GetSortColumn(string columnIndex)
        {
            switch (columnIndex)
            {
                case "1": return "case_num";
                case "2": return "motion_name";
                case "3": return "start";
                case "4": return "duration";
                case "5": return "court_name";
                case "6": return "status_name";
                case "7": return "attorney_name";
                case "8": return "opp_attorney_name";
                case "9": return "plaintiff";
                case "10": return "defendant";
                case "11": return "courtroom_name";
                default: return "case_num";
            }
        }
        /// <summary>
        /// Resolves the court → county chain for an existing event, obtains the auth token,
        /// and returns a ClerkContext ready for API calls.
        /// Throws <see cref="InvalidOperationException"/> when any required configuration is missing.
        /// </summary>
        private async System.Threading.Tasks.Task<ClerkContext> ResolveClerkContext(long eventId)
        {
            long courtId = new EventController().GetCourtIdByEventId(eventId);
            if (courtId <= 0)
                throw new InvalidOperationException($"Could not resolve a court for event {eventId}.");

            return await ResolveClerkContextByCourt(courtId);
        }

        /// <summary>
        /// Resolves the county / auth token for a court id.  Used when creating a new event
        /// where no local event id exists yet.
        /// Throws <see cref="InvalidOperationException"/> when any required configuration is missing.
        /// </summary>
        private async System.Threading.Tasks.Task<ClerkContext> ResolveClerkContextByCourt(long courtId)
        {
            var court = new CourtController().GetCourt(courtId);
            if (court == null)
                throw new InvalidOperationException($"Court {courtId} not found.");

            var county = new CountyController().GetCounty(court.county_id);
            if (county == null)
                throw new InvalidOperationException($"County {court.county_id} not found for court {courtId}.");

            var apiCtl = new ApiEndpointController();
            string token = !string.IsNullOrWhiteSpace(county.decrypted_token)
                ? county.decrypted_token
                : await apiCtl.GetJwtToken(county);

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    $"Failed to obtain an auth token for county {county.id}, court {courtId}.");

            return new ClerkContext { County = county, Token = token, ApiCtl = apiCtl };
        }

        /// <summary>
        /// Calls the clerk API for the given endpoint type.
        /// Throws <see cref="InvalidOperationException"/> when the endpoint is not configured.
        /// <paramref name="logContext"/> is forwarded to CallExternalApi so the
        /// request/response are persisted to the api_log table — pass null to opt out.
        /// </summary>
        private async System.Threading.Tasks.Task<HttpResponseMessage> CallClerkApi(
            ClerkContext ctx,
            ApiEndpointType endpointType,
            object payload,
            HttpMethod method,
            ApiLogContext logContext = null)
        {
            var api = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)endpointType);
            if (api == null)
                throw new InvalidOperationException(
                    $"No {endpointType} endpoint configured for county {ctx.County.id}.");

            if (logContext != null && string.IsNullOrWhiteSpace(logContext.Action))
                logContext.Action = endpointType.ToString();

            return await ctx.ApiCtl.CallExternalApi(api, ctx.Token, payload, method, logContext);
        }

        /// <summary>
        /// Builds an <see cref="ApiLogContext"/> populated with the current DNN user
        /// id plus any optional event/case identifiers the caller wants recorded.
        /// Action defaults are supplied by CallClerkApi when the endpoint type is
        /// known, but may be overridden here for endpoints called directly.
        /// </summary>
        private ApiLogContext BuildLogContext(
            long? eventId = null,
            long? caseId = null,
            string action = null)
        {
            return new ApiLogContext
            {
                UserId = UserInfo != null && UserInfo.UserID > 0 ? (int?)UserInfo.UserID : null,
                EventId = eventId,
                CaseId = caseId,
                Action = action,
                Application = ApiLogApplication.JACS,
            };
        }

        /// <summary>
        /// After a clerk write call fails, calls the clerk's GetEvent endpoint to check
        /// whether the change was actually applied despite the error response.
        /// Returns true if the clerk's record reflects the expected post-change state.
        /// Any exception during verification is swallowed and logged so it never masks
        /// the original error.
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

                var getPayload = new { EventId = clerkEventId };
                var getResponse = await ctx.ApiCtl.CallExternalApi(getApi, ctx.Token, getPayload, HttpMethod.Post,
                    BuildLogContext(action: ApiEndpointType.GetEvent.ToString()));

                if (getResponse == null || !getResponse.IsSuccessStatusCode)
                    return false;

                var body = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                // Spec response: { "data": { ... }, "error": "" } — unwrap the data object
                var data = body["data"] as JObject ?? body;
                return stateMatches(data);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"VerifyClerkEventChange: exception during verification for clerk_event_id {clerkEventId}.", ex));
                return false;
            }
        }
        /// <summary>
        /// Resolves all clerk-side ids (judge, courtroom, event type) and UDF data
        /// for an event, returning a flat DTO ready to embed in any clerk API payload.
        /// Pass <paramref name="overrideTimeslot"/> to use a specific timeslot's start,
        /// duration, and courtroom instead of looking them up from the event's own
        /// timeslot link (needed for reschedule where new and current timeslots differ).
        /// </summary>
        private System.Threading.Tasks.Task<ClerkEventPayload> BuildClerkEventPayload(
            Event evt, long courtId, long countyId, Timeslot overrideTimeslot = null)
        {
            var judgeCtl = new JudgeController();
            var courtroomCtl = new CourtroomController();
            var motionCtl = new MotionController();

            // Judge xref
            var judge = judgeCtl.GetJudgeByCourt(courtId);
            var judgeXref = judge != null
                ? judgeCtl.GetJudgeXref(judge.id, countyId).FirstOrDefault()
                : null;
            long clerkJudgeId = judgeXref?.clerk_judge_id ?? 0;

            // Timeslot — use override when provided (reschedule), otherwise load from event
            Timeslot timeslot = overrideTimeslot;
            if (timeslot == null)
            {
                var te = new TimeslotEventController().GetTimeslotEventsByEvent(evt.id).FirstOrDefault();
                timeslot = te != null ? new TimeslotController().GetTimeslot(te.timeslot_id.Value) : null;
            }

            // Courtroom xref
            long clerkCourtroomId = 0;
            if (timeslot?.courtroom_id != null)
            {
                var courtroomXref = courtroomCtl.GetCourtroomXref(timeslot.courtroom_id.Value, countyId).FirstOrDefault();
                clerkCourtroomId = courtroomXref?.clerk_courtroom_id ?? 0;
            }

            // Motion → EventType string: just use the motion description directly.
            // No xref lookup needed — the clerk accepts the motion name as a string.
            string eventType = null;
            string otherEventType = null;
            if (evt.motion_id.HasValue)
            {
                if (evt.motion_id.Value == 221) // "Other" motion
                {
                    otherEventType = evt.custom_motion;
                }
                else
                {
                    var motion = motionCtl.GetMotion(evt.motion_id.Value);
                    eventType = motion?.description;
                }
            }

            // UDF: parse composite-key template JSON → human-readable field names
            var udf = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(evt.template))
            {
                try
                {
                    var templateObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(evt.template);
                    if (templateObj != null)
                    {
                        foreach (var kvp in templateObj)
                        {
                            string fieldName = kvp.Key.Contains("_|")
                                ? kvp.Key.Split(new[] { "_|" }, StringSplitOptions.None)[0]
                                : kvp.Key;
                            if (!string.IsNullOrWhiteSpace(kvp.Value))
                                udf[fieldName] = kvp.Value;
                        }
                    }
                }
                catch (Exception udfEx)
                {
                    Exceptions.LogException(new Exception("BuildClerkEventPayload: failed to parse UDF template JSON.", udfEx));
                }
            }

            return System.Threading.Tasks.Task.FromResult(new ClerkEventPayload
            {
                JudgeId        = clerkJudgeId,
                EventType      = eventType,
                OtherEventType = otherEventType,
                EventDateTime  = timeslot?.start.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration       = timeslot?.duration ?? 0,
                CourtRoomId    = clerkCourtroomId,
                UDF            = udf
            });
        }

        /// <summary>Flattened clerk event fields shared by Create, Update, and Reschedule payloads.</summary>
        private class ClerkEventPayload
        {
            public long JudgeId { get; set; }
            public string EventType { get; set; }
            public string OtherEventType { get; set; }
            public string EventDateTime { get; set; }
            public int Duration { get; set; }
            public long CourtRoomId { get; set; }
            public Dictionary<string, string> UDF { get; set; }
        }

        /// <summary>
        /// Shared context resolved once per request: county, decrypted token, and ApiEndpointController.
        /// </summary>
        private class ClerkContext
        {
            public County County { get; set; }
            public string Token { get; set; }
            public ApiEndpointController ApiCtl { get; set; }
        }

    }
}
