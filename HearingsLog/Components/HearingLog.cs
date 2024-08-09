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
        public int LogID { get; set; }
        public int CalendarID { get; set; }
        public string JudgeID { get; set; }
        public string County { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public string DIN { get; set; }
        public string MotionTitle { get; set; }
        public string DraftedBy { get; set; }
        public string CourtNotes { get; set; }
        public string DelayReason { get; set; }
        public StatusType Status { get; set; }
        public DateTime? OrderSigned { get; set; }
        public DateTime HearingDate { get; set; }
        public int CreatedByID { get; set; }
        public int LastModifiedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
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
