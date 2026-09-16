using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// Shared clerk-side event operations used by both the event-management API
    /// (EventAPIController) and bulk operations such as the calendar truncate
    /// flow. Centralising the logic keeps the clerk call + GetEvent verify
    /// recovery in one place and lets the truncate path cancel hundreds of
    /// events with the same correctness guarantees as a single-event cancel.
    /// </summary>
    internal class ClerkEventService
    {
        /// <summary>
        /// Per-call context — county + auth token + the underlying
        /// ApiEndpointController. Built by ResolveContextByCourtAsync /
        /// ResolveContextByEventAsync; reuse across many calls for the same
        /// court to avoid re-fetching the JWT.
        /// </summary>
        internal class ClerkContext
        {
            public County County { get; set; }
            public string Token { get; set; }
            public ApiEndpointController ApiCtl { get; set; }
        }

        /// <summary>
        /// Result of a single CancelEventAsync call. Success means the clerk
        /// either accepted the cancel or GetEvent verified it landed despite an
        /// error response. LocallyApplied indicates whether this service also
        /// updated the local event row + soft-deleted timeslot_events links
        /// (true unless the caller set persistLocally=false).
        /// </summary>
        internal class CancelResult
        {
            public bool Success { get; set; }
            public bool LocallyApplied { get; set; }
            public string Error { get; set; }
            public long EventId { get; set; }
            public long ClerkEventId { get; set; }
            public string CaseNum { get; set; }
        }

        /// <summary>
        /// Resolves court → county → JWT for an existing event.
        /// </summary>
        public async Task<ClerkContext> ResolveContextByEventAsync(long eventId)
        {
            long courtId = new EventController().GetCourtIdByEventId(eventId);
            if (courtId <= 0)
                throw new InvalidOperationException($"Could not resolve a court for event {eventId}.");
            return await ResolveContextByCourtAsync(courtId);
        }

        /// <summary>
        /// Resolves county + JWT for a court id. Use this when batching many
        /// events for the same court — calling once and reusing the context
        /// avoids one JWT round-trip per event.
        /// </summary>
        public async Task<ClerkContext> ResolveContextByCourtAsync(long courtId)
        {
            var court = new CourtController().GetCourt(courtId);
            if (court == null)
                throw new InvalidOperationException($"Court {courtId} not found.");

            var county = new CountyController().GetCounty(court.county_id);
            if (county == null)
                throw new InvalidOperationException($"County {court.county_id} not found for court {courtId}.");

            var apiCtl = new ApiEndpointController();
            string token = await apiCtl.EnsureValidTokenAsync(county);

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    $"Failed to obtain an auth token for county {county.id}, court {courtId}.");

            return new ClerkContext { County = county, Token = token, ApiCtl = apiCtl };
        }

        /// <summary>
        /// Cancels an event on the clerk side and, when persistLocally is true
        /// (default), updates the local event row + soft-deletes its
        /// timeslot_events links. On clerk failure, calls GetEvent to confirm
        /// whether the cancel landed anyway — if confirmed, the cancel is
        /// treated as successful. Returns a structured result for the caller
        /// to surface or aggregate.
        /// </summary>
        public async Task<CancelResult> CancelEventAsync(
            ClerkContext ctx,
            Event evt,
            string reason,
            int? userIdForLog = null,
            bool persistLocally = true)
        {
            var result = new CancelResult
            {
                EventId = evt.id,
                ClerkEventId = evt.clerk_event_id,
                CaseNum = evt.case_num,
            };

            // --------------------------------------------------------------
            // Step 1 – Talk to the clerk.
            // --------------------------------------------------------------
            try
            {
                var api = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)ApiEndpointType.CancelEvent);
                if (api == null)
                {
                    result.Error = $"No CancelEvent endpoint configured for county {ctx.County.id}.";
                    return result;
                }

                var clerkPayload = new
                {
                    EventId = evt.clerk_event_id,
                    Reason = reason ?? string.Empty,
                };

                var logCtx = new ApiLogContext
                {
                    UserId = userIdForLog,
                    EventId = evt.clerk_event_id,
                    CaseId = evt.clerk_case_id,
                    Action = ApiEndpointType.CancelEvent.ToString(),
                    Application = ApiLogApplication.JACS,
                };

                var clerkResponse = await ctx.ApiCtl.CallExternalApi(api, ctx.Token, clerkPayload, HttpMethod.Post, logCtx);

                if (!clerkResponse.IsSuccessStatusCode)
                {
                    string clerkError = await clerkResponse.Content.ReadAsStringAsync();
                    Exceptions.LogException(new Exception(
                        $"ClerkEventService.CancelEvent: clerk returned {(int)clerkResponse.StatusCode} for event {evt.id} (clerk_event_id={evt.clerk_event_id}): {clerkError}"));

                    // Recovery: GetEvent to see if the cancel landed anyway.
                    bool confirmed = await VerifyCancelLandedAsync(ctx, evt.clerk_event_id, userIdForLog);
                    if (!confirmed)
                    {
                        result.Error = $"Clerk rejected the cancel (HTTP {(int)clerkResponse.StatusCode}). {clerkError}";
                        return result;
                    }

                    Exceptions.LogException(new Exception(
                        $"ClerkEventService.CancelEvent: clerk returned error but GetEvent confirms cancel was applied for event {evt.id}."));
                }
            }
            catch (InvalidOperationException configEx)
            {
                Exceptions.LogException(configEx);
                result.Error = $"Clerk configuration error: {configEx.Message}";
                return result;
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"ClerkEventService.CancelEvent: unhandled exception for event {evt.id}.", ex));
                result.Error = $"Clerk call failed: {ex.Message}";
                return result;
            }

            // --------------------------------------------------------------
            // Step 2 – Persist local cancel state if requested.
            // --------------------------------------------------------------
            if (persistLocally)
            {
                try
                {
                    var teCtl = new TimeslotEventController();
                    foreach (var te in teCtl.GetTimeslotEventsByEvent(evt.id))
                        teCtl.DeleteTimeslotEvent(te.id, false); // soft delete

                    var cancelledStatus = new EventStatusController().GetEventStatusByName("Cancelled");
                    evt.status_id = cancelledStatus?.id;
                    evt.cancellation_reason = reason;
                    evt.updated_at = DateTime.Now;
                    new EventController().UpdateEvent(evt, validate: false);
                    result.LocallyApplied = true;
                }
                catch (Exception persistEx)
                {
                    Exceptions.LogException(new Exception(
                        $"ClerkEventService.CancelEvent: clerk succeeded but local save failed for event {evt.id}.", persistEx));
                    result.Error = $"Clerk accepted the cancel but local save failed: {persistEx.Message}";
                    return result;
                }

                // Notify attorneys (best-effort — mailer swallows its own errors
                // so a failed send never masks a successful cancel).
                new EventNotificationMailer().SendCancellation(evt, reason);
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// Calls the clerk's GetEvent endpoint and reports whether the event
        /// has been marked Cancelled. Used as a recovery probe after a
        /// CancelEvent error response so a flaky network failure doesn't make
        /// us refuse to apply a cancel that the clerk actually committed.
        /// </summary>
        private async Task<bool> VerifyCancelLandedAsync(ClerkContext ctx, long clerkEventId, int? userIdForLog)
        {
            try
            {
                var getApi = ctx.ApiCtl.GetApiEndpointByCountyAndType(ctx.County.id, (int)ApiEndpointType.GetEvent);
                if (getApi == null) return false;

                var logCtx = new ApiLogContext
                {
                    UserId = userIdForLog,
                    EventId = clerkEventId,
                    Action = ApiEndpointType.GetEvent.ToString(),
                    Application = ApiLogApplication.JACS,
                };

                var getResponse = await ctx.ApiCtl.CallExternalApi(getApi, ctx.Token,
                    new { EventId = clerkEventId }, HttpMethod.Post, logCtx);
                if (getResponse == null || !getResponse.IsSuccessStatusCode) return false;

                var body = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                var data = body["data"] as JObject ?? body;
                return string.Equals(data["Status"]?.Value<string>(), "Cancelled", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"ClerkEventService.VerifyCancelLandedAsync: probe failed for clerk_event_id {clerkEventId}.", ex));
                return false;
            }
        }
    }

    /// <summary>
    /// One failed event entry returned to the truncate UI so the user can see
    /// which hearings still need attention.
    /// </summary>
    public class CancelFailureItem
    {
        [JsonProperty("event_id")]
        public long EventId { get; set; }
        [JsonProperty("clerk_event_id")]
        public long ClerkEventId { get; set; }
        [JsonProperty("case_num")]
        public string CaseNum { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
