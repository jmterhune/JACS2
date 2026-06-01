using Newtonsoft.Json;
using System;
using tjc.Modules.DigitalCourtReporting.Components;
namespace tjc.Modules.DigitalCourtReporting.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class ProceedingListItemViewModel
    {
        public ProceedingListItemViewModel(ProceedingListItem proceedignListItem)
        {
            ProceedingID = proceedignListItem.ProceedingID;
            Requestor = proceedignListItem.Requestor;
            CaseName = proceedignListItem.CaseName;
            CaseNumber = proceedignListItem.CaseNumber;
            RequestedDate=proceedignListItem.RequestedDate;
            RequestDateFormatted = proceedignListItem.RequestDateFormatted;
            Location = proceedignListItem.Location;
            Jurisdiction = proceedignListItem.Jurisdiction;
            ProceedingDate = proceedignListItem.ProceedingDate;
        }
        public ProceedingListItemViewModel() { }
        [JsonProperty("proceedingid")]
        public int ProceedingID { get; set; }

        [JsonProperty("requestor")]
        public string Requestor { get; set; }

        [JsonProperty("casename")]
        public string CaseName { get; set; }

        [JsonProperty("casenumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("requesteddate")]
        public DateTime RequestedDate { get; set; }

        [JsonProperty("requestdateformatted")]
        public string RequestDateFormatted { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("jurisdiction")]
        public string Jurisdiction { get; set; }

        [JsonProperty("proceedingdate")]
        public string ProceedingDate { get; set; }

    }
}
