using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Intranet.API.Components.Employee;
namespace tjc.Intranet.API.Services.ViewModels.Employee
{
    [JsonObject(MemberSerialization.OptIn)]

    public class EmployeeViewModel
    {
        public EmployeeViewModel(Components.Employee.Employee employee)
        {
            EmployeeId = employee.EmployeeId;
            Address1 = employee.Address1;
            Address2 = employee.Address2;
            City = employee.City;
            State = employee.State;
            Zip = employee.Zip;
            Location = employee.Location;
            EmailHome = employee.EmailHome;
            Phones = employee.Phones;
            EmergencyContacts = employee.EmergencyContacts;
        }
        public EmployeeViewModel() { }

        [JsonProperty("employeeId")]
        public long EmployeeId { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("emailHome")]
        public string EmailHome { get; set; }

        [JsonProperty("phones")]
        public IEnumerable<Phone> Phones { get; set; }

        [JsonProperty("emergencyContacts")]
        public IEnumerable<EmergencyContact> EmergencyContacts { get; set; }
    }
}
