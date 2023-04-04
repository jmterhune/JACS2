using Newtonsoft.Json;
using tjc.Intranet.API.Components.Employee;
namespace tjc.Intranet.API.Services.ViewModels.Employee
{
    [JsonObject(MemberSerialization.OptIn)]

    public class EmergencyContactViewModel
    {
        public EmergencyContactViewModel(EmergencyContact emergencyContact)
        {
            EmployeeId = emergencyContact.EmployeeId;
            ContactId = emergencyContact.ContactId;
            FirstName = emergencyContact.FirstName;
            LastName = emergencyContact.LastName;
            Relationship = emergencyContact.Relationship;
            PhoneHome = emergencyContact.PhoneHome;
            PhoneWork = emergencyContact.PhoneWork;
            PhoneMobile = emergencyContact.PhoneMobile;
            CallOrder= emergencyContact.CallOrder;
        }
        public EmergencyContactViewModel() { }

        [JsonProperty("employeeId")]
        public long EmployeeId { get; set; }

        [JsonProperty("contactId")]
        public long ContactId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("relationship")]
        public string Relationship { get; set; }

        [JsonProperty("phoneHome")]
        public string PhoneHome { get; set; }

        [JsonProperty("phoneWork")]
        public string PhoneWork { get; set; }

        [JsonProperty("phoneMobile")]
        public string PhoneMobile { get; set; }

        [JsonProperty("callOrder")]
        public int CallOrder { get; set; }
    }
}
