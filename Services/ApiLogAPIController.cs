using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
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
    /// <summary>
    /// Internal (DNN-authenticated) API that powers the admin API Log search page.
    /// All reads are scoped to the same <c>api_log</c> table used by the external
    /// clerk handler but without the single-county restriction — admins may view
    /// entries across counties.
    /// </summary>
    [DnnAuthorize]
    public class ApiLogAPIController : DnnApiController
    {
        /// <summary>
        /// DataTables-style search. p1 is the cached total record count (0 on
        /// first call so we recompute). Filters come from query string: countyId,
        /// eventId, caseId, action, application, fromDate, toDate, searchText.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetApiLogs(int p1)
        {
            var result = new ApiLogSearchResult();
            int recordCount = p1;
            int filteredCount = 0;

            var query = Request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            long? countyId = ParseLong(query, "countyId");
            long? eventId  = ParseLong(query, "eventId");
            long? caseId   = ParseLong(query, "caseId");
            string caseNumber = query.ContainsKey("caseNumber") ? query["caseNumber"]?.Trim() : null;
            byte? app      = ParseByte(query, "application");
            string action  = query.ContainsKey("action") ? query["action"] : null;
            string search  = query.ContainsKey("searchText") ? query["searchText"] : null;
            DateTime? fromDate = ParseDate(query, "fromDate");
            DateTime? toDate   = ParseDate(query, "toDate");

            // If the caller supplied a case number, resolve it to clerk_case_id(s)
            // via the events table. An empty resolution short-circuits to no
            // results inside the controller.
            List<long> caseIds = null;
            if (!string.IsNullOrWhiteSpace(caseNumber))
            {
                caseIds = new EventController().GetClerkCaseIdsByCaseNumber(caseNumber).ToList();
                if (caseId.HasValue) caseIds.Add(caseId.Value);
                caseIds = caseIds.Distinct().ToList();
            }
            else if (caseId.HasValue)
            {
                caseIds = new List<long> { caseId.Value };
            }

            int.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            int.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            int.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int offset);

            try
            {
                var ctl = new ApiLogController();
                filteredCount = ctl.SearchCount(countyId, eventId, caseIds, action, search, fromDate, toDate, app);
                if (p1 == 0) recordCount = filteredCount;

                var rows = ctl.SearchPaged(countyId, eventId, caseIds, action, search,
                    fromDate, toDate, app, offset, pageSize);

                // Resolve county names once to avoid N+1 lookups
                var countyNames = new CountyController().GetCountys()
                    .ToDictionary(c => c.id, c => c.name);

                result.data = rows
                    .Select(r =>
                    {
                        var vm = new ApiLogViewModel(r);
                        if (r.county_id.HasValue && countyNames.TryGetValue(r.county_id.Value, out var name))
                            vm.county_name = name;
                        return vm;
                    })
                    .ToList();
                result.draw = draw;
                result.recordsFiltered = filteredCount;
                result.recordsTotal = recordCount;
                result.error = null;
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                result.data = new List<ApiLogViewModel>();
                result.draw = draw;
                result.recordsFiltered = filteredCount;
                result.recordsTotal = recordCount;
                result.error = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }
        }

        /// <summary>Single log row detail (for the row-expand modal).</summary>
        [HttpGet]
        public HttpResponseMessage GetApiLog(long p1)
        {
            try
            {
                if (p1 <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { data = (object)null, error = "Invalid log id." });

                var row = new ApiLogController().GetApiLog(p1);
                if (row == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        new { data = (object)null, error = "Log not found." });

                var vm = new ApiLogViewModel(row);
                if (row.county_id.HasValue)
                {
                    var county = new CountyController().GetCounty(row.county_id.Value);
                    vm.county_name = county?.name ?? string.Empty;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { data = vm, error = (string)null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { data = (object)null, error = ex.Message });
            }
        }

        /// <summary>
        /// Returns the list of known action names for the filter dropdown.
        /// We derive them from the <see cref="ApiEndpointType"/> enum — which is
        /// the same source auto-logging uses for <c>api_log.action</c> — so the
        /// dropdown is populated even when no rows have been logged yet. We
        /// union any distinct actions actually present in the log as well, so
        /// historical or external rows with non-standard names still appear.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetActions()
        {
            try
            {
                var fromEnum = Enum.GetNames(typeof(ApiEndpointType));
                var fromLog = new ApiLogController().GetApiLogs()
                    .Where(r => !string.IsNullOrWhiteSpace(r.action))
                    .Select(r => r.action);

                var actions = fromEnum
                    .Concat(fromLog)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { data = actions, error = (string)null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { data = new List<string>(), error = ex.Message });
            }
        }

        #region helpers / DTOs

        internal class ApiLogSearchResult
        {
            public List<ApiLogViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        private static long? ParseLong(Dictionary<string, string> q, string key) =>
            q.ContainsKey(key) && long.TryParse(q[key], out var n) && n > 0 ? n : (long?)null;

        private static byte? ParseByte(Dictionary<string, string> q, string key) =>
            q.ContainsKey(key) && byte.TryParse(q[key], out var n) ? n : (byte?)null;

        private static DateTime? ParseDate(Dictionary<string, string> q, string key) =>
            q.ContainsKey(key) && DateTime.TryParse(q[key], out var dt) ? dt : (DateTime?)null;

        #endregion
    }
}
