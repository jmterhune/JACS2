using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.jacs.Components
{
    
    public class ListItemOptionResult
    {
        [JsonProperty("data")]
        public List<KeyValuePair<long, string>> data { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
    }
}