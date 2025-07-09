using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtTemplateViewModel
    {
        public CourtTemplateViewModel(CourtTemplate courtTemplate)
        {
            id = courtTemplate.id;
            name = courtTemplate.name;
            court_id = courtTemplate.court_id;
            judge_name = courtTemplate.judge_name; // Assuming this is populated via a join in the controller
            court_description = courtTemplate.court_description; // Assuming this is populated via a join
        }

        public CourtTemplateViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("court_id")]
        public long court_id { get; set; }

        [JsonProperty("judge_name")]
        public string judge_name { get; set; }

        [JsonProperty("court_description")]
        public string court_description { get; set; }
    }
}