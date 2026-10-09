using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_swn_interface_log")]
    [PrimaryKey("LogId", AutoIncrement = true)]
    public class SwnInterfaceLogInfo
    {
        public long LogId { get; set; }
        public string Process { get; set; }
        public string Exception { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
