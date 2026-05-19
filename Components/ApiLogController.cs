using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// Data-access layer for the api_log table. Provides a high-level Log helper
    /// designed to be called from anywhere in the application (including
    /// ApiEndpointController.CallExternalApi) plus search helpers for the
    /// admin UI and the external clerk log endpoint.
    /// </summary>
    internal class ApiLogController
    {
        private const string CONN_JACS = "jacs";

        #region Basic CRUD

        public void CreateApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiLog>();
                if (!t.created_at.HasValue) t.created_at = DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteApiLog(long apilogId)
        {
            var t = GetApiLog(apilogId);
            if (t != null) DeleteApiLog(t);
        }

        public void DeleteApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiLog>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ApiLog> GetApiLogs()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.GetRepository<ApiLog>().Get();
            }
        }

        public ApiLog GetApiLog(long apilogId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.GetRepository<ApiLog>().GetById(apilogId);
            }
        }

        public void UpdateApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.GetRepository<ApiLog>().Update(t);
            }
        }

        #endregion

        #region High-level Log helper

        /// <summary>
        /// Convenience entry point the rest of the app calls after an outbound
        /// API call. Serialises request/response payloads if given as objects,
        /// swallows logging failures (logging must never break the caller), and
        /// always records created_at server-side.
        ///
        /// Pass <paramref name="requestPayload"/> and <paramref name="responsePayload"/>
        /// either as already-serialised JSON strings or as objects that can be
        /// JsonConvert.SerializeObject'd.
        /// </summary>
        public long Log(
            string apiEndpointUrl,
            object requestPayload,
            object responsePayload,
            string error = null,
            long? countyId = null,
            long? eventId = null,
            long? caseId = null,
            int? userId = null,
            string action = null,
            ApiLogApplication application = ApiLogApplication.JACS)
        {
            try
            {
                var row = new ApiLog
                {
                    user_id = userId,
                    event_id = eventId,
                    case_id = caseId,
                    county_id = countyId,
                    action = Truncate(action, 50),
                    api_end_point = Truncate(apiEndpointUrl, 2000),
                    request_json = SerializeIfObject(requestPayload),
                    response_json = SerializeIfObject(responsePayload),
                    error = error,
                    created_at = DateTime.Now,
                    application = (byte)application,
                };
                CreateApiLog(row);
                return row.log_id;
            }
            catch (Exception ex)
            {
                // Logging must never break the caller. Surface to the DNN event
                // log and keep going.
                Exceptions.LogException(new Exception(
                    "ApiLogController.Log: failed to persist api_log row.", ex));
                return 0;
            }
        }

        private static string SerializeIfObject(object value)
        {
            if (value == null) return null;
            if (value is string s) return s;
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                return $"[serialization failed: {ex.Message}]";
            }
        }

        private static string Truncate(string value, int max)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= max) return value;
            return value.Substring(0, max);
        }

        #endregion

        #region Search

        /// <summary>
        /// Filtered, paged list of log rows ordered by created_at DESC.
        /// Null filter arguments mean "no restriction on this column".
        /// </summary>
        public IEnumerable<ApiLog> SearchPaged(
            long? countyId,
            long? eventId,
            long? caseId,
            string action,
            string search,
            DateTime? fromDate,
            DateTime? toDate,
            byte? application,
            int offset,
            int pageSize)
            => SearchPaged(countyId, eventId,
                caseId.HasValue ? new[] { caseId.Value } : null,
                action, search, fromDate, toDate, application, offset, pageSize);

        /// <summary>
        /// Overload that accepts multiple clerk case_ids — used when the UI
        /// resolves a typed case number to one-or-more clerk_case_id values
        /// from our events table. A non-null empty list short-circuits to no
        /// results (no case_id matched the typed case number).
        /// </summary>
        public IEnumerable<ApiLog> SearchPaged(
            long? countyId,
            long? eventId,
            IEnumerable<long> caseIds,
            string action,
            string search,
            DateTime? fromDate,
            DateTime? toDate,
            byte? application,
            int offset,
            int pageSize)
        {
            var caseIdList = caseIds?.ToList();
            if (caseIdList != null && caseIdList.Count == 0)
                return Enumerable.Empty<ApiLog>();

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var args = BuildSearchArgs(countyId, eventId, caseIdList, action, search,
                    fromDate, toDate, application, offset, pageSize);
                return ctx.ExecuteQuery<ApiLog>(
                    CommandType.Text,
                    BuildSearchSql(args.Where,
                        $"ORDER BY created_at DESC OFFSET {args.Offset} ROWS FETCH NEXT {args.PageSize} ROWS ONLY"),
                    args.Params.ToArray());
            }
        }

        /// <summary>
        /// Count of rows matching the same filters accepted by SearchPaged.
        /// </summary>
        public int SearchCount(
            long? countyId,
            long? eventId,
            long? caseId,
            string action,
            string search,
            DateTime? fromDate,
            DateTime? toDate,
            byte? application)
            => SearchCount(countyId, eventId,
                caseId.HasValue ? new[] { caseId.Value } : null,
                action, search, fromDate, toDate, application);

        public int SearchCount(
            long? countyId,
            long? eventId,
            IEnumerable<long> caseIds,
            string action,
            string search,
            DateTime? fromDate,
            DateTime? toDate,
            byte? application)
        {
            var caseIdList = caseIds?.ToList();
            if (caseIdList != null && caseIdList.Count == 0) return 0;

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var args = BuildSearchArgs(countyId, eventId, caseIdList, action, search,
                    fromDate, toDate, application, 0, 0);
                return ctx.ExecuteScalar<int>(
                    CommandType.Text,
                    BuildSearchSql(args.Where, null, isCount: true),
                    args.Params.ToArray());
            }
        }

        private static string BuildSearchSql(string whereClause, string trailer, bool isCount = false)
        {
            string select = isCount ? "COUNT(*)" : "*";
            string sql = $"SELECT {select} FROM api_log";
            if (!string.IsNullOrEmpty(whereClause)) sql += " WHERE " + whereClause;
            if (!string.IsNullOrEmpty(trailer)) sql += " " + trailer;
            return sql;
        }

        private class SearchArgs
        {
            public string Where;
            public List<object> Params;
            public int Offset;
            public int PageSize;
        }

        private static SearchArgs BuildSearchArgs(
            long? countyId, long? eventId, IReadOnlyList<long> caseIds, string action, string search,
            DateTime? fromDate, DateTime? toDate, byte? application,
            int offset, int pageSize)
        {
            var where = new List<string>();
            var parms = new List<object>();
            int i = 0;

            if (countyId.HasValue)      { where.Add($"county_id = @{i++}"); parms.Add(countyId.Value); }
            if (eventId.HasValue)       { where.Add($"event_id = @{i++}");  parms.Add(eventId.Value); }
            if (caseIds != null && caseIds.Count > 0)
            {
                if (caseIds.Count == 1)
                {
                    where.Add($"case_id = @{i++}");
                    parms.Add(caseIds[0]);
                }
                else
                {
                    var placeholders = new List<string>(caseIds.Count);
                    foreach (var id in caseIds)
                    {
                        placeholders.Add($"@{i++}");
                        parms.Add(id);
                    }
                    where.Add($"case_id IN ({string.Join(",", placeholders)})");
                }
            }
            if (application.HasValue)   { where.Add($"application = @{i++}"); parms.Add(application.Value); }
            if (!string.IsNullOrWhiteSpace(action))
            {
                where.Add($"action = @{i++}");
                parms.Add(action.Trim());
            }
            if (fromDate.HasValue)      { where.Add($"created_at >= @{i++}"); parms.Add(fromDate.Value); }
            if (toDate.HasValue)        { where.Add($"created_at <= @{i++}"); parms.Add(toDate.Value); }
            if (!string.IsNullOrWhiteSpace(search))
            {
                // free-text across endpoint, request, response, and error columns
                where.Add($"(api_end_point LIKE @{i} OR request_json LIKE @{i} OR response_json LIKE @{i} OR error LIKE @{i})");
                parms.Add("%" + search.Trim() + "%");
                i++;
            }

            return new SearchArgs
            {
                Where = string.Join(" AND ", where),
                Params = parms,
                Offset = Math.Max(offset, 0),
                PageSize = pageSize > 0 ? pageSize : 50
            };
        }

        #endregion
    }
}
