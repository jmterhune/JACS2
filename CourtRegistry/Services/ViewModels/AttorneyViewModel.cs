using Newtonsoft.Json;
using tjc.Modules.CourtRegistry.Components;
namespace tjc.Modules.CourtRegistry.Services
{
    [JsonObject(MemberSerialization.OptIn)]

    public class AttorneyViewModel
    {
        internal AttorneyViewModel(Attorney attorney)
        {
            AttorneyID = attorney.AttorneyID;
            BarNumber = attorney.BarNumber;
            LastName = attorney.LastName;
            FirstName = attorney.FirstName;
            Email = attorney.Email;
            Phone = attorney.Phone;
            Cell = attorney.Cell;
            Fax = attorney.Fax;
            LawFirm = attorney.LawFirm;
            Street = attorney.Address;
            City = attorney.City;
            State = attorney.State;
            ZipCode = attorney.Zip;
            Languages = attorney.Language;
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
        public string Cell { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("lawfirm")]
        public string LawFirm { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zipcode")]
        public string ZipCode { get; set; }

        [JsonProperty("languages")]
        public string Languages { get; set; }
    }
}
