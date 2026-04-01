
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
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
}

