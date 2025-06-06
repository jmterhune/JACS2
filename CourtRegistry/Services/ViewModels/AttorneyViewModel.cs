using Newtonsoft.Json;
using tjc.Modules.CourtRegistry.Components;
namespace tjc.Modules.CourtRegistry.Services
{
    [JsonObject(MemberSerialization.OptIn)]

    internal class AttorneyViewModel
    {
        public AttorneyViewModel(Attorney attorney)
        {
            AttorneyID = attorney.AttorneyID;
            BarNumber = attorney.BarNumber;
            LastName = attorney.LastName;
            FirstName = attorney.FirstName;
            Email=attorney.Email;
            Phone=attorney.Phone;
            Cell =attorney.Cell;
            Fax =attorney.Fax;
            LawFirm =attorney.LawFirm;
        }
        public AttorneyViewModel() { }
        [JsonProperty("attorneyid")]
        public int AttorneyID { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("barnumber")]
        public int BarNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("cell")]
        public string   Cell { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("lawfirm")]
        public string LawFirm { get; set; }
    }
}
