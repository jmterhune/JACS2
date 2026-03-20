using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ApiEndpointViewModel
    {
        public ApiEndpointViewModel(ApiEndpoint apiEndpoint)
        {
            id = apiEndpoint.id;
            county_id = apiEndpoint.county_id;
            end_point_url = apiEndpoint.end_point_url;
            type = (int)apiEndpoint.type;
            county_name = apiEndpoint.county_name;
            type_desc = apiEndpoint.type_desc;
        }

        public ApiEndpointViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("county_id")]
        public long county_id { get; set; }
        [JsonProperty("type")]
        public int type { get; set; }

        [JsonProperty("end_point_url")]
        public string end_point_url { get; set; }

        [JsonProperty("county_name")]
        public string county_name { get; set; }

        [JsonProperty("type_desc")]
        public string type_desc { get; set; }
    }
}