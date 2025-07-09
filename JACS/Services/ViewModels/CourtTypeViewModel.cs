using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtTypeViewModel
    {
        public CourtTypeViewModel(CourtType courtType)
        {
            id = courtType.id;
            description = courtType.description;
        }

        public CourtTypeViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }
}