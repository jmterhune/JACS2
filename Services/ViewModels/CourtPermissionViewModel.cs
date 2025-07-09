using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtPermissionViewModel
    {
        public CourtPermissionViewModel(CourtPermission courtPermission, string userDisplayName, string judgeName)
        {
            id = courtPermission.id;
            user_id = courtPermission.user_id;
            judge_id = courtPermission.judge_id;
            user_display_name = userDisplayName;
            judge_name = judgeName;
            active = courtPermission.active;
            editable = courtPermission.editable;
            permission = courtPermission.editable ? "View and Edit" : "View";
        }

        public CourtPermissionViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("user_id")]
        public long user_id { get; set; }

        [JsonProperty("judge_id")]
        public long judge_id { get; set; }

        [JsonProperty("user_display_name")]
        public string user_display_name { get; set; }

        [JsonProperty("judge_name")]
        public string judge_name { get; set; }

        [JsonProperty("active")]
        public bool active { get; set; }

        [JsonProperty("editable")]
        public bool editable { get; set; }

        [JsonProperty("permission")]
        public string permission { get; set; }
    }
}