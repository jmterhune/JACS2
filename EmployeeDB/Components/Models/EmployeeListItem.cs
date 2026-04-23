using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    // Denormalized read-only view tjc_employee_list.
    // Mapped loosely; only commonly-used columns are exposed. PetaPoco will set
    // matching properties and ignore the rest.
    [TableName("tjc_employee_list")]
    [PrimaryKey("EmployeeId", AutoIncrement = false)]
    public class EmployeeListItem
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public string ClassName { get; set; }
        public string JobGroupName { get; set; }
        public string LocationName { get; set; }
        public string CountyName { get; set; }
        public string SupervisorName { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
