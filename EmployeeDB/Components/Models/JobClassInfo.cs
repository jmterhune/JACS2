using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_class")]
    [PrimaryKey("ClassId", AutoIncrement = true)]
    [Cacheable("tjc_employee_class", CacheItemPriority.Default, 20)]
    public class JobClassInfo
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int ClassCode { get; set; }
        public int? PayGrade { get; set; }
        public string FLSA { get; set; }
        public int? EEO { get; set; }
        public decimal? MMax { get; set; }
        public decimal? MMin { get; set; }
        public decimal? AMax { get; set; }
        public decimal? AMin { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }
    }
}
