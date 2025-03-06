using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using tjc.Modules.TranscriptDatabase.Components;
namespace tjc.Modules.TranscriptDatabase.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class EventViewModel
    {
        public EventViewModel(Event eventItem)
        {
            EventId = eventItem.EventID;
            PresidingJudgeId = eventItem.PresidingJudgeID;
            HearingType = eventItem.HearingType;
            HearingDate = eventItem.HearingDate.Value;
            CreatedByUserID = eventItem.CreatedByUserID;
            DesignationId = eventItem.DesignationID;
        }
        public EventViewModel() { }
        [JsonProperty("eventid")]
        public int EventId { get; set; }
        [JsonProperty("designationid")]
        public int DesignationId { get; set; }

        [JsonProperty("presidingjudgeid")]
        public int PresidingJudgeId { get; set; }
        [JsonProperty("presidingjudgename")]
        public string PresidingJudgeName { get; set; }


        [JsonProperty("hearingtype")]
        public string HearingType { get; set; }

        [JsonProperty("hearingdate")]
        public DateTime HearingDate { get; set; }

        [JsonProperty("createdbyuserid")]
        public int CreatedByUserID { get; set; }
    }
    public class EventsViewModel
    {
        public List<EventViewModel> events { get; set; }
    }
}
