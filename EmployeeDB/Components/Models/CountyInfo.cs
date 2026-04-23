using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    // Global counties table, read-only from this module.
    [TableName("tjc_gl_counties")]
    [PrimaryKey("CountyId", AutoIncrement = true)]
    [Cacheable("tjc_gl_counties", CacheItemPriority.Default, 20)]
    public class CountyInfo
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public string CreatedById { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
