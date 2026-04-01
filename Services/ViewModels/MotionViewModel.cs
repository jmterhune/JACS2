using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class MotionViewModel
    {
        public MotionViewModel(Motion motion)
        {
            id = motion.id;
            description = motion.description;
            lag = motion.lag;
            lead = motion.lead;
        }

        public MotionViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("lag")]
        public int? lag { get; set; }

        [JsonProperty("lead")]
        public int? lead { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class MotionClerkXrefViewModel
    {
        public MotionClerkXrefViewModel(MotionClerkXref xref)
        {
            county_id = xref.county_id;
            motion_id = xref.motion_id;
            clerk_motion_id = xref.clerk_motion_id;
            clerk_motion_name = xref.clerk_motion_name;
        }
        public MotionClerkXrefViewModel(MotionClerkXrefListItem xref)
        {
            county_id = xref.county_id;
            motion_id = xref.motion_id;
            clerk_motion_id = xref.clerk_motion_id;
            clerk_motion_name = xref.clerk_motion_name;
            county_name = xref.county_name ?? string.Empty;
            motion_name = xref.motion_name ?? string.Empty;
        }
        public MotionClerkXrefViewModel() { }

        [JsonProperty("county_id")]
        public long county_id { get; set; }
        [JsonProperty("motion_id")]
        public long motion_id { get; set; }
        [JsonProperty("clerk_motion_id")]
        public long clerk_motion_id { get; set; }
        [JsonProperty("clerk_motion_name")]
        public string clerk_motion_name { get; set; }
        [JsonProperty("county_name")]
        public string county_name { get; set; }
        [JsonProperty("motion_name")]
        public string motion_name { get; set; }
    }
}