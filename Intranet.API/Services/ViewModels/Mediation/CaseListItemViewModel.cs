using Newtonsoft.Json;
using tjc.Intranet.API.Components.Mediation;
namespace tjc.Intranet.API.Services.ViewModels.Mediation
{
    [JsonObject(MemberSerialization.OptIn)]

    public class CaseListItemViewModel
    {
        public CaseListItemViewModel(CaseListItem caseListItem)
        {
            Region = caseListItem.Region;
            Group = caseListItem.Group;
            PartyOne = caseListItem.PartyOne;
            PartyTwo = caseListItem.PartyTwo;
            ListNumber = string.IsNullOrEmpty(caseListItem.ListNumber)?" ":caseListItem.ListNumber;
            CaseId = caseListItem.CaseId;
            CreatedDate = caseListItem.CreatedDate.ToShortDateString();
            GroupName = caseListItem.GroupTypeName;
            Comments=caseListItem.FormattedComments;
            
        }        
        [JsonProperty("caseid")]
        public int CaseId { get; set; }

        public CaseListItemViewModel() { }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("partyone")]
        public string PartyOne { get; set; }

        [JsonProperty("partytwo")]
        public string PartyTwo { get; set; }

        [JsonProperty("listnumber")]
        public string ListNumber { get; set; }

        [JsonProperty("createddate")]
        public string CreatedDate { get; set; }

        [JsonProperty("groupname")]
        public string GroupName { get; set; }
        [JsonProperty("comments")]
        public string Comments { get; set; }
    }   
}
