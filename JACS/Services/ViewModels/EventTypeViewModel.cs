// File: EventTypeViewModel.cs
using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EventTypeViewModel
    {
        public EventTypeViewModel(EventType eventType)
        {
            id = eventType.id;
            name = eventType.name;
        }

        public EventTypeViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }
    }
}