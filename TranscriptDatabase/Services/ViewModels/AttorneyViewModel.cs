using Newtonsoft.Json;
using tjc.Modules.TranscriptDatabase.Components;
namespace tjc.Modules.TranscriptDatabase.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class AttorneyViewModel
    {
        public AttorneyViewModel(Attorney attorney)
        {
            AttorneyId = attorney.AttorneyID;
            LastName = attorney.LastName;
            FirstName = attorney.FirstName;
            MiddleName = attorney.MiddleName;
            OfficeId = attorney.OfficeID;
            Address1 = attorney.Address1;
            Address2 = attorney.Address2;
            City = attorney.City;
            State = attorney.State;
            ZipCode = attorney.ZipCode;
            OfficeName = attorney.OfficeName;
            ListName = attorney.ListName;
        }
        public AttorneyViewModel(Attorney attorney, int designationId)
        {
            AttorneyId = attorney.AttorneyID;
            LastName = attorney.LastName;
            FirstName = attorney.FirstName;
            MiddleName = attorney.MiddleName;
            OfficeId = attorney.OfficeID;
            Address1 = attorney.Address1;
            Address2 = attorney.Address2;
            City = attorney.City;
            State = attorney.State;
            ZipCode = attorney.ZipCode;
            OfficeName = attorney.OfficeName;
            ListName = attorney.ListName;
            DesignationId = designationId;
        }
        public AttorneyViewModel() { }
        [JsonProperty("attorneyid")]
        public int AttorneyId { get; set; }
        [JsonProperty("designationid")]
        public int DesignationId { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("middlename")]
        public string MiddleName { get; set; }
        [JsonProperty("listname")]
        public string ListName { get; set; }
        [JsonProperty("officeid")]
        public int OfficeId { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string ZipCode { get; set; }

        [JsonProperty("officename")]
        public string OfficeName { get; set; }

        [JsonProperty("createdbyuserid")]
        public int CreatedByUserID
        {
            get; set;
        }
    }
    public class DropDownViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("office")]
        public string Office { get; set; }
    }
}
