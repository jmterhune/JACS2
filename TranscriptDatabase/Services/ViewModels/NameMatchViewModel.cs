using Newtonsoft.Json;
namespace tjc.Modules.TranscriptDatabase.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class NameMatchViewModel
    {
        public NameMatchViewModel() { }
        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("hearingdate")]
        public string HearingDate { get; set; }

    }
}
