using Newtonsoft.Json;
using System.Collections.Generic;
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
}