using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_position_history")]
    [PrimaryKey("PositionId", AutoIncrement = true)]
    public class PositionHistoryInfo
    {
        public int PositionId { get; set; }
        [DigitsOnly] public string SocialSecurityNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsInternal { get; set; }
        public string EntryType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
