using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CountyViewModel
    {
        public CountyViewModel(County county)
        {
            id = county.id;
            name = county.name;
            code = county.code;
            auth_end_point_url = county.auth_end_point_url;
            user_name = county.user_name;
            password = county.decrypted_password;
            token = county.decrypted_token;

        }

        public CountyViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("auth_end_point_url")]
        public string auth_end_point_url { get; set; }

        [JsonProperty("user_name")]
        public string user_name { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }

    }
}
