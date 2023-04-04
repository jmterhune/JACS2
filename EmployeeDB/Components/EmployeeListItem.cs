using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_list")]
    internal class EmployeeListItem 
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public DateTime? HireDate { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool IsActive { get; set; }

        public string PhoneList { get; set; }
        public int DepartmentId { get; set; }
        

        [IgnoreColumn]
        public string Phones { get { return GetPhoneListFromString(); } }
        #region Methods
        private string GetPhoneListFromString()
        {
            string phoneList=PhoneList.TrimEnd('|');
            string outputList = "";
            List<Phone> phoneObjectList = new List<Phone>();
            string[] phoneArray = phoneList.Split('|');
            foreach (string phone in phoneArray)
            {
                string[] phoneFields = phone.Split(',');
                phoneObjectList.Add(new Phone { PhoneType = phoneFields[0], PhoneNumber = phoneFields[1], Extension = phoneFields[2], OfficeLocationName = phoneFields[3] });
            }
            foreach (Phone phone in phoneObjectList) {
                outputList += phone.FormattedPhone + "<br />";
            }
            outputList = outputList.Substring(0, outputList.LastIndexOf("<br />"));
            return outputList;
        }

        #endregion
    }
}
