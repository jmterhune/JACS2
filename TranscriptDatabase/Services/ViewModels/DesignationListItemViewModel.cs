using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Services.Description;
using tjc.Modules.TranscriptDatabase.Components;
using static DotNetNuke.Web.InternalServices.MessagingServiceController;
namespace tjc.Modules.TranscriptDatabase.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class DesignationListItemViewModel
    {
        public DesignationListItemViewModel(DesignationListItem designationItem)
        {
            DesignationID = designationItem.DesignationID;
            LastName = designationItem.dLastName;
            FirstName = designationItem.dFirstName;
            CaseNumber = designationItem.CaseNumber;
            County = designationItem.County;
            if(designationItem.ServiceDate.HasValue)
            ServiceDate = designationItem.ServiceDate.Value.ToShortDateString();
            if(designationItem.DueDate.HasValue)
            DueDate = designationItem.DueDate.Value.ToShortDateString();
            if (designationItem.TranscriptFiled.HasValue)
            TranscriptFiled = designationItem.TranscriptFiled.Value.ToShortDateString();
            AcknowledgmentFiled = designationItem.AcknowledgmentFiled;
            Archived=designationItem.Archived;
            CreatedByUsername = designationItem.CreatedByUsername;

        }
        public DesignationListItemViewModel() { }
        [JsonProperty("designationid")]
        public int DesignationID { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("casenumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("servicedate")]
        public string ServiceDate { get; set; }

        [JsonProperty("duedate")]
        public string DueDate { get; set; }

        [JsonProperty("transcriptfiled")]
        public string TranscriptFiled { get; set; }

        [JsonProperty("acknowledgmentfiled")]
        public bool AcknowledgmentFiled { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("createdbyusername")]
        public string CreatedByUsername { get; set; }
    }
}
