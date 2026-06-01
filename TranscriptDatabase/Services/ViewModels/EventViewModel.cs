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
    public class ExtensionViewModel
    {
        public ExtensionViewModel(ExtensionRequest extensionRequest)
        {
            ExtensionId = extensionRequest.ExtensionID;
            DesignationId = extensionRequest.DesignationID;
            EventTypeId = extensionRequest.EventTypeID;
            RequestedDate = extensionRequest.RequestedDate.Value;
            GrantedDate = extensionRequest.GrantedDate.Value;
            SubmittedDate = extensionRequest.SubmittedDate.Value;
            CreatedDate = extensionRequest.CreatedDate;
            CreatedByUserId = extensionRequest.CreatedByUserID;
            LastModifiedByUserId = extensionRequest.LastModifiedByUserID;
            LastModifiedDate=extensionRequest.LastModifiedDate;
            Approved=extensionRequest.Approved;
        }
        public ExtensionViewModel() { }
        [JsonProperty("extensionid")] 
        public int ExtensionId { get; set; }
        [JsonProperty("designationid")] 
        public int DesignationId { get; set; }
        [JsonProperty("eventtypeid")] 
        public int EventTypeId { get; set; }
        [JsonProperty("requesteddate")] 
        public DateTime RequestedDate { get; set; }
        [JsonProperty("granteddate")] 
        public DateTime GrantedDate { get; set; }
        [JsonProperty("submitteddate")] 
        public DateTime SubmittedDate { get; set; }
        [JsonProperty("createddate")] 
        public DateTime CreatedDate { get; set; }
        [JsonProperty("createdbyid")] 
        public int CreatedByUserId { get; set; }
        [JsonProperty("lastmodifieddate")] 
        public DateTime LastModifiedDate { get; set; }
        [JsonProperty("lastmodifiedbyuserid")] 
        public int LastModifiedByUserId { get; set; }
        [JsonProperty("approved")]
        public bool Approved { get; set; }
        [JsonProperty("documenttypeid")]
        public int DocumentTypeId { get; set; }
        [JsonProperty("hasduedate")]
        public bool HasDueDate { get; set; }
        [JsonProperty("portalid")]
        public int PortalId { get; set; }
        [JsonProperty("adminrole")]
        public string AdminRole { get; set; }
        [JsonProperty("countyname")]
        public string CountyName { get; set; }
    }
}
