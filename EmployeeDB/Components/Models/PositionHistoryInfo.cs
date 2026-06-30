using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_position_history")]
    [PrimaryKey("PositionId", AutoIncrement = true)]
    public class PositionHistoryInfo
    {
        public int PositionId { get; set; }  // int
        [DigitsOnly] public string SocialSecurityNumber { get; set; }  // nvarchar(9) — stored as raw digits (mask stripped)
        public DateTime? StartDate { get; set; }  // datetime
        public DateTime? EndDate { get; set; }  // datetime
        public string Description { get; set; }  // nvarchar(2000)
        public bool IsInternal { get; set; }  // bit
        public string EntryType { get; set; }  // char(1)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
