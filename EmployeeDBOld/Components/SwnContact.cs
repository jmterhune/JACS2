using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class SwnContact
    {
        public string TimeZone
        {
            get;set;
        }

        public bool IsEmployee
        {
            get; set;
        }

        public string PreferredLanguage
        {
            get; set;
        }

        public string CustomLabel1
        {
            get; set;
        }

        public string CustomValue1
        {
            get; set;
        }

        public string CustomLabel2
        {
            get; set;
        }

        public string CustomValue2
        {
            get; set;
        }

        public string CustomLabel3
        {
            get; set;
        }

        public string CustomValue3
        {
            get; set;
        }

        public string CustomLabel4
        {
            get; set;
        }

        public string CustomValue4
        {
            get; set;
        }

        public string CustomLabel5
        {
            get; set;
        }

        public string CustomValue5
        {
            get; set;
        }

        public string CustomLabel6
        {
            get; set;
        }

        public string CustomValue6
        {
            get; set;
        }

        public string Country
        {
            get; set;
        }

        public string ZipPostalCode
        {
            get; set;
        }

        public string StateProvince
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public string Address2
        {
            get; set;
        }

        public string Address1
        {
            get; set;
        }

        public long UniqueID
        {
            get; set;
        }

        public string FirstName
        {
            get; set;
        }

        public string LastName
        {
            get; set;
        }

        public string MiddleInitial
        {
            get; set;
        }

        public string EmailLabel1
        {
            get; set;
        }

        public string Email1
        {
            get; set;
        }

        public string EmailLabel2
        {
            get; set;
        }

        public string Email2
        {
            get; set;
        }

        public string BBPinLabel
        {
            get; set;
        }

        public string BBPin
        {
            get; set;
        }

        public List<SwnPhone> PhoneList
        {
            get; set;
        }

        public List<Group> GroupList
        {
            get; set;
        }

    }
    public class ContactExists
    {
        public bool is_contact_in_account { get; set; }
    }
    public class ContactIdList
    {
        public List<string> contacts { get; set; }
    }
    public class CustomField
    {
        public string custom_field_name { get; set; }
        public string custom_field_value { get; set; }
    }

    public class ContactPoint
    {
        public string type { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string country_code { get; set; }
        public int cascade_order { get; set; }
        public string extension { get; set; }
        public string carrier { get; set; }
    }

    public class Address
    {
        public string address_type { get; set; }
        public string facility_location { get; set; }
        public string first_address { get; set; }
        public string second_address { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string province { get; set; }
        public string country { get; set; }
    }

    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
        public string quick_send_code { get; set; }
        public List<string> access_group_list { get; set; }
        public string status { get; set; }
    }

    public class EmployeeData
    {
        public string id { get; set; }
        public string employee_id { get; set; }
        public string full_name { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string time_zone { get; set; }
        public string pin { get; set; }
        public string language { get; set; }
        public string division { get; set; }
        public string company { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public List<ContactPoint> contact_points { get; set; }
        public List<Address> addresses { get; set; }
        public Login login { get; set; }
    }
}