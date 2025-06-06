using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CountyViewModel
    {
        public CountyViewModel(County county)
        {
            id = county.id;
            name = county.name;
            code = county.code;
        }

        public CountyViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }
    }
}
