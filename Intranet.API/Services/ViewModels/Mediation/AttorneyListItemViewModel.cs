using Newtonsoft.Json;
using tjc.Intranet.API.Components.Mediation;
namespace tjc.Intranet.API.Services.ViewModels.Mediation
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AttorneyListItemViewModel
    {
        public AttorneyListItemViewModel(AttorneyListItem attorneyListItem)
        {
            AttorneyId = attorneyListItem.AttorneyId;
            FirstName = attorneyListItem.FirstName;
            LastName = attorneyListItem.LastName;
            Firm = attorneyListItem.Firm;
            Phone=attorneyListItem.Phone;
            Extension = attorneyListItem.Extension;
            Email = attorneyListItem.Email;
            AttorneyName=attorneyListItem.AttorneyName;

        }
        public AttorneyListItemViewModel() { }
        [JsonProperty("attorneyid")]
        public int AttorneyId { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("firm")]
        public string Firm { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("extenstion")]
        public string Extension { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("attorneyname")]
        public string AttorneyName { get; set; }
    }
}
