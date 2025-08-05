using Newtonsoft.Json;
using System;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TimeslotViewModel
    {
        public TimeslotViewModel(TimeslotListItem timeslotData)
        {
            id = timeslotData.id;
            end = timeslotData.end;
            start = timeslotData.start;
            FormattedStart= timeslotData.FormattedStart;
            allDay = timeslotData.allDay;
            quantity = timeslotData.quantity;
            duration = timeslotData.duration;
            blocked = timeslotData.blocked;
            public_block = timeslotData.public_block;
            block_reason = timeslotData.block_reason;
            category_id = timeslotData.category_id;
            template_id = timeslotData.template_id;
            created_at = timeslotData.created_at;
            updated_at = timeslotData.updated_at;
            deleted_at = timeslotData.deleted_at;
            court_name = timeslotData.court_name;
            event_count = timeslotData.TimeslotEvents.Count;
            available=timeslotData.available;
            description = timeslotData.description ?? string.Empty;

        }
        public TimeslotViewModel(Timeslot timeslotData)
        {
            id = timeslotData.id;
            end = timeslotData.end;
            start = timeslotData.start;
            FormattedStart = timeslotData.FormattedStart;
            allDay = timeslotData.allDay;
            quantity = timeslotData.quantity;
            duration = timeslotData.duration;
            blocked = timeslotData.blocked;
            public_block = timeslotData.public_block;
            block_reason = timeslotData.block_reason;
            category_id = timeslotData.category_id;
            template_id = timeslotData.template_id;
            created_at = timeslotData.created_at;
            updated_at = timeslotData.updated_at;
            deleted_at = timeslotData.deleted_at;
            event_count = timeslotData.TimeslotEvents.Count;
            available = timeslotData.available;
            description = timeslotData.description ?? string.Empty;
        }
        public TimeslotViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("end")]
        public DateTime end { get; set; }
        [JsonProperty("start")]
        public DateTime start { get; set; }
        [JsonProperty("FormattedStart")]
        public string FormattedStart { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("allDay")]
        public bool allDay { get; set; }
        [JsonProperty("quantity")]
        public int quantity { get; set; }
        [JsonProperty("duration")]
        public int duration { get; set; }
        [JsonProperty("blocked")]
        public bool blocked { get; set; }
        [JsonProperty("public_block")]
        public bool public_block { get; set; }
        [JsonProperty("block_reason")]
        public string block_reason { get; set; }
        [JsonProperty("category_id")]
        public long? category_id { get; set; }
        [JsonProperty("template_id")]
        public long? template_id { get; set; }
        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }
        [JsonProperty("deleted_at")]
        public DateTime? deleted_at { get; set; }
        [JsonProperty("available")]
        public bool available { get; set; }
        [JsonProperty("court_name")]
        public string court_name { get; set; }
        [JsonProperty("event_count")]
        public int event_count { get; set; }
    }
}