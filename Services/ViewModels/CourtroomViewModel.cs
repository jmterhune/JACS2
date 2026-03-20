using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtroomViewModel
    {
        public CourtroomViewModel(Courtroom courtroom)
        {
            id = courtroom.id;
            description = courtroom.description;
        }

        public CourtroomViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtroomClerkXrefViewModel
    {
        public CourtroomClerkXrefViewModel(CourtroomClerkXref xref)
        {
            county_id = xref.county_id;
            courtroom_id = xref.courtroom_id;
            clerk_courtroom_id = xref.clerk_courtroom_id;
            clerk_courtroom_name = xref.clerk_courtroom_name;
        }
        public CourtroomClerkXrefViewModel(CourtroomClerkXrefListItem xref)
        {
            county_id = xref.county_id;
            courtroom_id = xref.courtroom_id;
            clerk_courtroom_id = xref.clerk_courtroom_id;
            clerk_courtroom_name = xref.clerk_courtroom_name;
            county_name = xref.county_name ?? string.Empty;
            courtroom_name = xref.courtroom_name ?? string.Empty;
        }
        public CourtroomClerkXrefViewModel() { }

        [JsonProperty("county_id")]
        public long county_id { get; set; }
        [JsonProperty("courtroom_id")]
        public long courtroom_id { get; set; }
        [JsonProperty("clerk_courtroom_id")]
        public long clerk_courtroom_id { get; set; }
        [JsonProperty("clerk_courtroom_name")]
        public string clerk_courtroom_name { get; set; }
        [JsonProperty("county_name")]
        public string county_name { get; set; }
        [JsonProperty("courtroom_name")]
        public string courtroom_name { get; set; }
    }

}