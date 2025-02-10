using Newtonsoft.Json;
using tjc.Intranet.API.Components.Employee;
namespace tjc.Intranet.API.Services.ViewModels.Employee
{
    [JsonObject(MemberSerialization.OptIn)]

    public class PhoneViewModel
    {
        public PhoneViewModel(Phone phone)
        {
            EmployeeId = phone.EmployeeId;
            PhoneId = phone.PhoneId;
            PhoneNumber = phone.PhoneNumber;
            PhoneType = phone.PhoneType;
            Extension = phone.Extension;
            OfficeLocationId = phone.OfficeLocationId;
        }
        public PhoneViewModel() { }
        [JsonProperty("employeeId")]
        public long EmployeeId { get; set; }

        [JsonProperty("phoneId")]
        public long PhoneId { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phoneType")]
        public string PhoneType { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("officelocationid")]
        public int OfficeLocationId { get; set; }

    }
}
