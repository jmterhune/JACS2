using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_audio")]
    [PrimaryKey("AudioID", AutoIncrement = true)]
    [Cacheable("Audios", CacheItemPriority.Default, 20)]
    internal class Audio
    {
        public int AudioID { get; set; }
        public int? ProceedingID { get; set; }
        public string Juvenile { get; set; }
        public string Indigence { get; set; }
        public string CDType { get; set; }
        public string Employee { get; set; }
        public string Tracking { get; set; }
        public DateTime? CDBurnDate { get; set; }
        public DateTime? DateotCA { get; set; }
        public int? TotalMinutes { get; set; }
        public int? CDCount { get; set; }
        public string Notes { get; set; }
        public bool? UTP { get; set; }
        public int? CreatedByID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}