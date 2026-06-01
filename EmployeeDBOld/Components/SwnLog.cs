using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_swn_interface_log")]
    [PrimaryKey("LogId", AutoIncrement = true)]
    [Cacheable("SwnLog", CacheItemPriority.Default, 20)]
    internal class SwnLog 
    {
        public long LogId { get; set; }

        public string Exception { get; set; }

        public string Process { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
    [TableName("tjc_employee_swn_interface_log_list")]
    [PrimaryKey("LogId", AutoIncrement = false)]
    internal class SwnLogListItem:SwnLog        
    {
        public string CreatedByName { get; set; }
    }
}
