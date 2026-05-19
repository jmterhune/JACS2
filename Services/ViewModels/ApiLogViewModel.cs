using Newtonsoft.Json;
using System;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    /// <summary>
    /// Row shape returned to the admin log-search UI and to the external clerk
    /// log endpoint. We surface a resolved county name and a friendly application
    /// label so the caller doesn't have to join anything client-side.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class ApiLogViewModel
    {
        public ApiLogViewModel() { }

        public ApiLogViewModel(ApiLog log)
        {
            log_id = log.log_id;
            user_id = log.user_id;
            event_id = log.event_id;
            case_id = log.case_id;
            county_id = log.county_id;
            action = log.action;
            api_end_point = log.api_end_point;
            request_json = log.request_json;
            response_json = log.response_json;
            error = log.error;
            created_at = log.created_at;
            application = log.application;
            application_name = log.application.HasValue
                ? ((ApiLogApplication)log.application.Value).ToString()
                : null;
        }

        [JsonProperty("log_id")]         public long log_id { get; set; }
        [JsonProperty("user_id")]        public int? user_id { get; set; }
        [JsonProperty("event_id")]       public long? event_id { get; set; }
        [JsonProperty("case_id")]        public long? case_id { get; set; }
        [JsonProperty("county_id")]      public long? county_id { get; set; }
        [JsonProperty("county_name")]    public string county_name { get; set; }
        [JsonProperty("action")]         public string action { get; set; }
        [JsonProperty("api_end_point")]  public string api_end_point { get; set; }
        [JsonProperty("request_json")]   public string request_json { get; set; }
        [JsonProperty("response_json")]  public string response_json { get; set; }
        [JsonProperty("error")]          public string error { get; set; }
        [JsonProperty("created_at")]     public DateTime? created_at { get; set; }
        [JsonProperty("application")]    public byte? application { get; set; }
        [JsonProperty("application_name")] public string application_name { get; set; }
    }
}
