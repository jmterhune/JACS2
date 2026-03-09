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
}