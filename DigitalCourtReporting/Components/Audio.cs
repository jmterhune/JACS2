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
        public int AudioID { get; set; }                // int
        public int? ProceedingID { get; set; }          // int
        public string Juvenile { get; set; }            // nvarchar(4)
        public string Indigence { get; set; }           // nvarchar(4)
        public string CDType { get; set; }              // nvarchar(25)
        public string Employee { get; set; }            // nvarchar(50)
        public string Tracking { get; set; }            // nvarchar(50)
        public DateTime? CDBurnDate { get; set; }       // smalldatetime
        public DateTime? DateotCA { get; set; }         // smalldatetime
        public int? TotalMinutes { get; set; }          // int
        public int? CDCount { get; set; }               // int
        public string Notes { get; set; }               // nvarchar(max)
        public bool? UTP { get; set; }                  // bit
        public int? CreatedByID { get; set; }           // int
        public DateTime? CreatedDate { get; set; }      // date
        public int? LastModifiedByID { get; set; }      // int
        public DateTime? LastModifiedDate { get; set; } // date
    }
}