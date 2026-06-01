using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    // Global groups table, read-only from this module.
    [TableName("tjc_gl_group")]
    [PrimaryKey("GroupID", AutoIncrement = true)]
    [Cacheable("tjc_gl_group", CacheItemPriority.Default, 20)]
    public class GroupInfo
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int? GroupType { get; set; }
        public bool? IsSwnGroup { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByID { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedByID { get; set; }
    }
}
