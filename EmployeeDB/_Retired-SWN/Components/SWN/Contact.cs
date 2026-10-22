using System.Collections.Generic;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.Contact from D:\websites\Intranet\App_Code\EmployeeDB\SWN.vb.
    /// Simple DTO used for internal contact representation before translating into
    /// the SWN SOAP service ContactDetailRequest.
    /// </summary>
    public class Contact
    {
        public Contact()
        {
        }

        public Contact(
            string uniqueid,
            bool isEmployee,
            string lastname,
            string firstname,
            string middleinitial,
            string address1,
            string address2,
            string city,
            string state,
            string zip,
            string country,
            string timezone,
            string preferredlanguage,
            string customlabel1,
            string customvalue1,
            string customlabel2,
            string customvalue2,
            string customlabel3,
            string customvalue3,
            string customlabel4,
            string customvalue4,
            string customlabel5,
            string customvalue5,
            string customlabel6,
            string customvalue6,
            string emaillabel1,
            string email1,
            string emaillabel2,
            string email2,
            string bbpinlabel,
            string bbpin)
        {
            UniqueID = uniqueid;
            IsEmployee = isEmployee;
            LastName = lastname;
            FirstName = firstname;
            MiddleInitial = middleinitial;
            Address1 = address1;
            Address2 = address2;
            City = city;
            StateProvince = state;
            ZipPostalCode = zip;
            Country = country;
            TimeZone = timezone;
            PreferredLanguage = preferredlanguage;
            CustomLabel1 = customlabel1;
            CustomValue1 = customvalue1;
            CustomLabel2 = customlabel2;
            CustomValue2 = customvalue2;
            CustomLabel3 = customlabel3;
            CustomValue3 = customvalue3;
            CustomLabel4 = customlabel4;
            CustomValue4 = customvalue4;
            CustomLabel5 = customlabel5;
            CustomValue5 = customvalue5;
            CustomLabel6 = customlabel6;
            CustomValue6 = customvalue6;
            EmailLabel1 = emaillabel1;
            Email1 = email1;
            EmailLabel2 = emaillabel2;
            Email2 = email2;
            BBPinLabel = bbpinlabel;
            BBPin = bbpin;
        }

        public string UniqueID { get; set; }
        public bool IsEmployee { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string Country { get; set; }
        public string TimeZone { get; set; }
        public string PreferredLanguage { get; set; }

        public string CustomLabel1 { get; set; }
        public string CustomValue1 { get; set; }
        public string CustomLabel2 { get; set; }
        public string CustomValue2 { get; set; }
        public string CustomLabel3 { get; set; }
        public string CustomValue3 { get; set; }
        public string CustomLabel4 { get; set; }
        public string CustomValue4 { get; set; }
        public string CustomLabel5 { get; set; }
        public string CustomValue5 { get; set; }
        public string CustomLabel6 { get; set; }
        public string CustomValue6 { get; set; }

        public string EmailLabel1 { get; set; }
        public string Email1 { get; set; }
        public string EmailLabel2 { get; set; }
        public string Email2 { get; set; }

        public string BBPinLabel { get; set; }
        public string BBPin { get; set; }

        public List<Phone> PhoneList { get; set; }

        /// <summary>
        /// Placeholder for the employee Group list. In the VB source this was
        /// List(Of AWS.Employee.Group). Typed as object so the SWN namespace
        /// does not have to take a hard dependency on module model types.
        /// </summary>
        public object GroupList { get; set; }
    }
}
