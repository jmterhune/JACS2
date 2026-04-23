using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_assigned_item")]
    [PrimaryKey("ItemId", AutoIncrement = true)]
    public class AssignedItemInfo
    {
        public int ItemId { get; set; }
        public int EmployeeId { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
