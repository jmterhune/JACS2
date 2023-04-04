using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Intranet.API.Components.Employee
{
    [TableName("Emp_Employees")]
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Location { get; set; }
        public string EmailHome { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [IgnoreColumn]
        public IEnumerable<Phone> Phones { get; set; }
        [IgnoreColumn]
        public IEnumerable<EmergencyContact> EmergencyContacts { get; set; }
    }
}