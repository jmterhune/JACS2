using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Services.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Intranet.API.Components.Employee
{
    [TableName("Emp_Phones")]
    [PrimaryKey("PhoneId", AutoIncrement = true)]
    public class Phone
    {
        public long PhoneId { get; set; }
        public long EmployeeId { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string  PhoneType { get; set; }
        public string Extension { get; set; }
        public string PhoneCascade { get; set; }
        public bool SWNText { get; set; }
        public bool SWNCall { get; set; }
        public bool SWNExcludeExtension { get; set; }

    }
}