using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Text.RegularExpressions;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_phone")]
    //setup the primary key for table
    [PrimaryKey("PhoneId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Phones", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Phone : EmployeeBase
    {
        public int PhoneId { get; set; }

        public int EmployeeId { get; set; }

        public int? OfficeLocationId { get; set; }

        public string PhoneType { get; set; }

        public string PhoneNumber { get; set; }

        public string Extension { get; set; }

        public bool IsMain { get; set; }

        public string PhoneCascade { get; set; }

        public bool SwnText { get; set; }

        public bool SwnCall { get; set; }

        public bool SwnExcludeExtension { get; set; }
        [IgnoreColumn]
        public string FormattedPhone { get { return FormatPhone(); } }
        [IgnoreColumn]
        public string OfficeLocationName { get; set; }

        #region Methods
        private string FormatPhone()
        {
            string tempPhone = "";
            string phoneFormatted = "";
            string phoneUrl = "<a class=\"{3}\" data-original-title=\"{2}\" data-plugin-tooltip=\"tooltip\" href=\"tel:{0}\">{1}</a>";
            string tempPhoneExtention = "";
            if (PhoneType.ToLower().Contains("cell"))
            {
                tempPhone = Regex.Replace(PhoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3") + " <em>Cell Phone</em>";
                phoneFormatted += string.Format(phoneUrl, Helper.CleanPhone(PhoneNumber), tempPhone, "Work Issued Cell Phone", "phone cell-phone");
            }
            else
            {
                if (Extension.Trim() != "")
                {
                    tempPhone = Regex.Replace(PhoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3") + " x" + Extension;
                    tempPhoneExtention = string.Format("{0},{1}", Helper.CleanPhone(PhoneNumber), Helper.CleanPhone(Extension));
                }
                else
                {
                    tempPhone = Regex.Replace(PhoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                    tempPhoneExtention = Helper.CleanPhone(PhoneNumber);
                }
                phoneFormatted = OfficeLocationName != "" ?  string.Format(phoneUrl, tempPhoneExtention, tempPhone, OfficeLocationName, "phone phone-location") : string.Format(phoneUrl, tempPhoneExtention, tempPhone,"Office Phone","phone") ;
            }

            return phoneFormatted;
        }
        #endregion

    }
}
