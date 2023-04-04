using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Intranet.API.Components.Employee
{
    [TableName("Emp_EmergencyContact")]
    [PrimaryKey("ContactId", AutoIncrement = true)]
    public class EmergencyContact
    {
        public long ContactId { get; set; }
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneWork { get; set; }
        public string PhoneMobile { get; set; }
        public int CallOrder { get; set; }
    }
}