using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// Row in the api_log table. Persisted for every outbound Clerk API call, and
    /// may be persisted by additional applications that share this log table.
    /// </summary>
    [TableName("api_log")]
    [PrimaryKey("log_id", AutoIncrement = true)]
    [Cacheable("ApiLogs", CacheItemPriority.Default, 20)]
    internal class ApiLog
    {
        public long log_id { get; set; }
        public int? user_id { get; set; }
        public long? event_id { get; set; }
        public long? case_id { get; set; }
        public long? county_id { get; set; }
        public string action { get; set; }
        public string api_end_point { get; set; }
        public string request_json { get; set; }
        public string response_json { get; set; }
        public string error { get; set; }
        public DateTime? created_at { get; set; }
        public byte? application { get; set; }
    }

    /// <summary>
    /// Identifier for the application that emitted a log entry. Stored in
    /// api_log.application (tinyint) so multiple apps can share the same table.
    /// </summary>
    public enum ApiLogApplication : byte
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("JACS")]
        JACS = 1,
        // Add additional applications here as they begin using the api_log table.
    }

    /// <summary>
    /// Per-call context passed into ApiEndpointController.CallExternalApi so the
    /// logger can record which user, event, case, and action the HTTP call belonged
    /// to. All fields are optional — callers populate what they know.
    /// </summary>
    internal class ApiLogContext
    {
        public int? UserId { get; set; }
        public long? EventId { get; set; }
        public long? CaseId { get; set; }
        public string Action { get; set; }
        public ApiLogApplication Application { get; set; } = ApiLogApplication.JACS;
    }
}
