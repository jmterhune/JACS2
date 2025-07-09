using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EventStatusViewModel
    {
        public EventStatusViewModel(EventStatus eventStatus)
        {
            id = eventStatus.id;
            name = eventStatus.name;
        }

        public EventStatusViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }
    }
}