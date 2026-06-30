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
        public int ClassId { get; set; }  // int
        public string ClassName { get; set; }  // nvarchar(50)
        public int ClassCode { get; set; }  // int
        public int? PayGrade { get; set; }  // int
        public string FLSA { get; set; }  // nvarchar(2)
        public int? EEO { get; set; }  // int
        public decimal? MMax { get; set; }  // money
        public decimal? MMin { get; set; }  // money
        public decimal? AMax { get; set; }  // money
        public decimal? AMin { get; set; }  // money
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int
    }
}
