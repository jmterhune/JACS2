using Microsoft.VisualBasic.CompilerServices;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class SwnPhone:Phone
    {
        public string CountryCode
        {
            get; set;
        }
        public string SmsLabel
        {
            get; set;
        }

        public string Sms
        {
            get; set;
        }
    }
}