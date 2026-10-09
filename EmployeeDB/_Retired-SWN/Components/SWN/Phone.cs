namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.Phone from D:\websites\Intranet\App_Code\EmployeeDB\SWN.vb.
    /// Internal phone DTO used when shaping a Contact for SWN. Defaults match
    /// the VB original: PhoneCountryCode = "1", SMSLabel = "SMS".
    /// </summary>
    public class Phone
    {
        private string _phoneCountryCode;
        private string _smsLabel;

        public Phone()
        {
        }

        public Phone(
            string phonelabel,
            string phonecountrycode,
            string phone,
            string phoneextension,
            string cascade,
            string smslabel,
            bool swncall,
            bool swntext,
            bool swnexcludeExtension)
        {
            PhoneLabel = phonelabel;
            PhoneCountryCode = phonecountrycode;
            PhoneNumber = phone;
            PhoneExtension = phoneextension;
            Cascade = cascade;
            SMSLabel = smslabel;
            SWNCall = swncall;
            SWNText = swntext;
            SWNexcludeExtension = swnexcludeExtension;
        }

        public string PhoneLabel { get; set; }

        public string PhoneCountryCode
        {
            get
            {
                if (string.IsNullOrEmpty(_phoneCountryCode))
                {
                    _phoneCountryCode = "1";
                }
                return _phoneCountryCode;
            }
            set { _phoneCountryCode = value; }
        }

        /// <summary>
        /// The phone number. Maps to the VB "Phone" property; renamed to
        /// PhoneNumber here to avoid C# conflict with the containing class name.
        /// </summary>
        public string PhoneNumber { get; set; }

        public string PhoneExtension { get; set; }

        public string Cascade { get; set; }

        public string SMSLabel
        {
            get
            {
                if (string.IsNullOrEmpty(_smsLabel))
                {
                    _smsLabel = "SMS";
                }
                return _smsLabel;
            }
            set { _smsLabel = value; }
        }

        public bool SWNCall { get; set; }
        public bool SWNText { get; set; }
        public bool SWNexcludeExtension { get; set; }

        /// <summary>
        /// Returns the SMS address for mobile/cell phones:
        /// "countrycode+phonenumber@sms.sendwordnow.com". Empty for other labels.
        /// </summary>
        public string SMS
        {
            get
            {
                if (!string.IsNullOrEmpty(PhoneLabel))
                {
                    var label = PhoneLabel.ToLower().Trim();
                    if (label.Contains("mobile") || PhoneLabel.ToLower().Contains("cell"))
                    {
                        return PhoneCountryCode + PhoneNumber + "@sms.sendwordnow.com";
                    }
                }
                return string.Empty;
            }
        }
    }
}
