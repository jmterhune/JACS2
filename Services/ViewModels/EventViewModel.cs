    using DotNetNuke.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EventViewModel
    {
        public EventViewModel(Event eventData)
        {
            id = eventData.id;
            case_num = eventData.case_num;
            notes = eventData.notes;
            plaintiff = eventData.plaintiff;
            defendant = eventData.defendant;
            motion_id = eventData.motion_id ?? -1;
            attorney_id = eventData.attorney_id ?? -1;
            type_id = eventData.type_id ?? -1;
            status_id = eventData.status_id ?? -1;
            reminder = eventData.reminder;
            opp_attorney_id = eventData.opp_attorney_id ?? -1;
            owner_id = eventData.owner_id ?? -1;
            owner_type = eventData.owner_type;
            addon = eventData.addon ?? false;
            plaintiff_email = eventData.plaintiff_email;
            defendant_email = eventData.defendant_email;
            cancellation_reason = eventData.cancellation_reason;
            template = eventData.template;
            telephone = eventData.telephone;
            custom_motion = eventData.custom_motion;
            created_at = eventData.created_at ?? DateTime.Now;
            updated_at = eventData.updated_at ?? DateTime.Now;
            motion_name = eventData.motion_name;
            attorney_name = eventData.attorney_name;
            opp_attorney_name = eventData.opp_attorney_name;
            status_name = eventData.status_name;
            timeslot_desc = eventData.timeslot_desc;
            court_name = eventData.court_name;
        }
        public EventViewModel(EventListItem eventData)
        {
            id = eventData.id;
            case_num = eventData.case_num;
            notes = eventData.notes;
            plaintiff = eventData.plaintiff;
            defendant = eventData.defendant;
            motion_id = eventData.motion_id ?? -1;
            attorney_id = eventData.attorney_id ?? -1;
            type_id = eventData.type_id ?? -1;
            status_id = eventData.status_id ?? -1;
            reminder = eventData.reminder;
            opp_attorney_id = eventData.opp_attorney_id ?? -1;
            owner_id = eventData.owner_id ?? -1;
            owner_type = eventData.owner_type;
            addon = eventData.addon ?? false;
            plaintiff_email = eventData.plaintiff_email;
            defendant_email = eventData.defendant_email;
            cancellation_reason = eventData.cancellation_reason;
            template = eventData.template;
            telephone = eventData.telephone;
            custom_motion = eventData.custom_motion;
            created_at = eventData.created_at ?? DateTime.Now;
            updated_at = eventData.updated_at ?? DateTime.Now;
            motion_name = eventData.motion_name;
            attorney_name = eventData.attorney_name;
            opp_attorney_name = eventData.opp_attorney_name;
            status_name = eventData.status_name;
            timeslot_desc = eventData.timeslot_desc;
            court_name = eventData.court_name;
            category_name = eventData.category_name;
        }
        public EventViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("case_num")]
        public string case_num { get; set; }
        [JsonProperty("notes")]
        public string notes { get; set; }
        [JsonProperty("plaintiff")]
        public string plaintiff { get; set; }
        [JsonProperty("defendant")]
        public string defendant { get; set; }
        [JsonProperty("motion_id")]
        public long motion_id { get; set; }
        [JsonProperty("attorney_id")]
        public long attorney_id { get; set; }
        [JsonProperty("type_id")]
        public long type_id { get; set; }
        [JsonProperty("status_id")]
        public long status_id { get; set; }
        [JsonProperty("reminder")]
        public bool reminder { get; set; }
        [JsonProperty("opp_attorney_id")]
        public long opp_attorney_id { get; set; }
        [JsonProperty("owner_id")]
        public long owner_id { get; set; }
        [JsonProperty("owner_type")]
        public string owner_type { get; set; }
        [JsonProperty("addon")]
        public bool addon { get; set; }
        [JsonProperty("plaintiff_email")]
        public string plaintiff_email { get; set; }
        [JsonProperty("defendant_email")]
        public string defendant_email { get; set; }
        [JsonProperty("cancellation_reason")]
        public string cancellation_reason { get; set; }
        [JsonProperty("template")]
        public string template { get; set; }
        [JsonProperty("telephone")]
        public string telephone { get; set; }
        [JsonProperty("custom_motion")]
        public string custom_motion { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("motion_name")]
        public string motion_name { get; set; }
        [JsonProperty("attorney_name")]
        public string attorney_name { get; set; }
        [JsonProperty("opp_attorney_name")]
        public string opp_attorney_name { get; set; }
        [JsonProperty("status_name")]
        public string status_name { get; set; }
        [JsonProperty("timeslot_desc")]
        public string timeslot_desc { get; set; }
        [JsonProperty("court_name")]
        public string court_name { get; set; }
        [JsonProperty("category_name")]
        public string category_name { get; set; }
    }
}