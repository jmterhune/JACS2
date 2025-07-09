using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AttorneyViewModel
    {
        public AttorneyViewModel(Attorney attorney)
        {
            id = attorney.id;
            UserId = attorney.UserId;
            name = attorney.name;
            bar_num = attorney.bar_num;
            enabled = attorney.enabled.HasValue ? attorney.enabled.Value : false;
            scheduling = attorney.scheduling.HasValue ? attorney.scheduling.Value : false;
            phone = attorney.phone;
            notes = attorney.notes;
            emails = attorney.emails;
            email_list = attorney.email_list;
        }

        public AttorneyViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("bar_num")]
        public string bar_num { get; set; }

        [JsonProperty("enabled")]
        public bool enabled { get; set; }

        [JsonProperty("phone")]
        public string phone { get; set; }

        [JsonProperty("scheduling")]
        public bool scheduling { get; set; }

        [JsonProperty("notes")]
        public string notes { get; set; }

        [JsonProperty("emails")]
        public List<string> emails { get; set; }

        [JsonProperty("email_list")]
        public List<string> email_list { get; set; }
    }
}