using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using System.Web.Caching;

namespace tjc.Modules.HearingLog.Components
{
    [TableName("tjc_hearing_log")]
    [PrimaryKey("LogID", AutoIncrement = true)]
    [Cacheable("Hearings", CacheItemPriority.Default, 20)]
    public class HearingLog
    {
        public int LogID { get; set; }              // int
        public int CalendarID { get; set; }         // int
        public string JudgeID { get; set; }         // nvarchar(20)
        public string County { get; set; }          // nvarchar(50)
        public string CaseName { get; set; }        // nvarchar(500)
        public string CaseNumber { get; set; }      // nvarchar(200)
        public string DIN { get; set; }             // nvarchar(200)
        public string MotionTitle { get; set; }     // nvarchar(500)
        public string DraftedBy { get; set; }       // nvarchar(500)
        public string CourtNotes { get; set; }      // nvarchar(max)
        public string DelayReason { get; set; }     // nvarchar(max)
        public StatusType Status { get; set; }      // int (Status)
        public DateTime? OrderSigned { get; set; }  // datetime
        public DateTime HearingDate { get; set; }   // datetime
        public int CreatedByID { get; set; }        // int
        public int LastModifiedByID { get; set; }   // int
        public DateTime CreatedDate { get; set; }   // datetime
        public DateTime LastModifiedDate { get; set; } // datetime
        [IgnoreColumn]
        public DateTime SixtiethDayDate { get { return HearingDate.AddDays(60); } }
    }
    public enum StatusType
    {
        [Description("New Hearing")]
        New = 0,
        [Description("Archived Hearing")]
        Archived = 1,
        [Description("Excluded Hearing")]
        Excluded = 2
    }
}
