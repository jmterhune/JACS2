using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using tjc.Intranet.API.Components.Mediation;
namespace tjc.Intranet.API.Services.ViewModels.Mediation
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MediatorListItemViewModel
    {
        public MediatorListItemViewModel(MediatorListItem mediatorListItem)
        {
            MediatorId = mediatorListItem.MediatorId;
            FirstName = mediatorListItem.FirstName;
            LastName = mediatorListItem.LastName;
            MediatorName = mediatorListItem.MediatorName;
            Email = mediatorListItem.Email;
            Phone = mediatorListItem.Phone;
        }
        public MediatorListItemViewModel() { }
        [JsonProperty("mediatorid")]
        public int MediatorId { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("mediatorname")]
        public string MediatorName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
