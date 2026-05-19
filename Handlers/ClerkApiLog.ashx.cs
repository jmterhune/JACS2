using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Handlers
{
    /// <summary>
    /// Anonymous HTTP handler that lets an external clerk view the JACS ↔ Clerk
    /// API log for their county. The caller authenticates with the same Bearer
    /// token they already use for their clerk API (the value stored on the
    /// <c>counties.token</c> row and decrypted server-side). That token uniquely
    /// identifies the county, so the response is automatically scoped to the
    /// matching county — callers cannot see another county's logs even by
    /// guessing the <c>countyId</c>.
    ///
    /// Supported routes (all GET):
    ///   /Handlers/ClerkApiLog.ashx
    ///       → paged search (defaults + optional filters below)
    ///   /Handlers/ClerkApiLog.ashx?logId=123
    ///       → single-row detail (must belong to the caller's county)
    ///
    /// Query-string filters (search):
    ///   fromDate, toDate   — ISO or yyyy-MM-dd, inclusive of time
    ///   action             — e.g. AddEvent, UpdateEvent, CancelEvent, RescheduleEvent, GetCase, GetEvent
    ///   caseId, eventId    — numeric
    ///   search             — free-text across endpoint, request, response, error
    ///   page, pageSize     — default 1 / 50 (pageSize capped at 500)
    /// </summary>
    public class ClerkApiLog : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "application/json";

                // 1) Authenticate via Bearer token → resolve to a county
                var county = ResolveCountyFromAuthHeader(context.Request);
                if (county == null)
                {
                    WriteJson(context, HttpStatusCode.Unauthorized, new
                    {
                        error = "Missing or invalid Bearer token."
                    });
                    return;
                }

                // 2) Detail vs. search
                string logIdRaw = context.Request.QueryString["logId"];
                if (!string.IsNullOrWhiteSpace(logIdRaw))
                {
                    HandleDetail(context, county, logIdRaw);
                    return;
                }

                HandleSearch(context, county);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception("ClerkApiLog handler: unhandled exception.", ex));
                WriteJson(context, HttpStatusCode.InternalServerError, new
                {
                    error = "Internal server error."
                });
            }
        }

        private static void HandleDetail(HttpContext context, County county, string logIdRaw)
        {
            if (!long.TryParse(logIdRaw, out long logId) || logId <= 0)
            {
                WriteJson(context, HttpStatusCode.BadRequest, new { error = "Invalid logId." });
                return;
            }

            var ctl = new ApiLogController();
            var row = ctl.GetApiLog(logId);
            if (row == null || row.county_id != county.id)
            {
                // Same response for "not found" and "not yours" so we don't leak
                // whether a given logId exists in another county.
                WriteJson(context, HttpStatusCode.NotFound, new { error = "Log not found." });
                return;
            }

            WriteJson(context, HttpStatusCode.OK, new
            {
                data = ToViewModel(row, county.name),
                error = (string)null
            });
        }

        private static void HandleSearch(HttpContext context, County county)
        {
            var q = context.Request.QueryString;
            DateTime? fromDate = ParseDate(q["fromDate"]);
            DateTime? toDate = ParseDate(q["toDate"]);
            string action = q["action"];
            string search = q["search"];
            long? caseId = ParseLong(q["caseId"]);
            long? eventId = ParseLong(q["eventId"]);

            int page = ParseInt(q["page"], 1);
            if (page < 1) page = 1;
            int pageSize = ParseInt(q["pageSize"], 50);
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;
            int offset = (page - 1) * pageSize;

            var ctl = new ApiLogController();
            var rows = ctl.SearchPaged(
                countyId: county.id,
                eventId: eventId,
                caseId: caseId,
                action: action,
                search: search,
                fromDate: fromDate,
                toDate: toDate,
                application: null,
                offset: offset,
                pageSize: pageSize);

            int total = ctl.SearchCount(
                countyId: county.id,
                eventId: eventId,
                caseId: caseId,
                action: action,
                search: search,
                fromDate: fromDate,
                toDate: toDate,
                application: null);

            var data = rows.Select(r => ToViewModel(r, county.name)).ToList();
            WriteJson(context, HttpStatusCode.OK, new
            {
                data,
                paging = new
                {
                    page,
                    pageSize,
                    total,
                    totalPages = pageSize > 0 ? (int)Math.Ceiling((double)total / pageSize) : 1
                },
                error = (string)null
            });
        }

        /// <summary>
        /// Looks at the Authorization header on the incoming request and returns
        /// the County whose decrypted token matches. Small N (one row per
        /// county), so we scan linearly; if this table ever grows to hundreds of
        /// rows, add a token → county_id lookup table.
        /// </summary>
        private static County ResolveCountyFromAuthHeader(HttpRequest request)
        {
            string raw = request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(raw)) return null;
            const string bearerPrefix = "Bearer ";
            if (!raw.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)) return null;

            string token = raw.Substring(bearerPrefix.Length).Trim();
            if (string.IsNullOrWhiteSpace(token)) return null;

            var countyCtl = new CountyController();
            foreach (var c in countyCtl.GetCountys())
            {
                // County.decrypted_token computes on the raw token field.
                string decrypted = c.decrypted_token;
                if (!string.IsNullOrWhiteSpace(decrypted) && string.Equals(decrypted, token, StringComparison.Ordinal))
                    return c;
            }
            return null;
        }

        private static ApiLogViewModel ToViewModel(ApiLog row, string countyName) =>
            new ApiLogViewModel(row) { county_name = countyName };

        private static DateTime? ParseDate(string v) =>
            DateTime.TryParse(v, out var dt) ? dt : (DateTime?)null;

        private static long? ParseLong(string v) =>
            long.TryParse(v, out var n) ? n : (long?)null;

        private static int ParseInt(string v, int fallback) =>
            int.TryParse(v, out var n) ? n : fallback;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
        };

        private static void WriteJson(HttpContext context, HttpStatusCode status, object payload)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(payload, _jsonSettings));
        }
    }
}
