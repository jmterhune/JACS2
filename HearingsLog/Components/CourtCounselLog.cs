using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using System.Web.Caching;

namespace tjc.Modules.HearingLog.Components
{
    [TableName("tjc_hearing_cc")]
    [Cacheable("CourtCounselLogs", CacheItemPriority.Default, 20)]
    public class CourtCounselLog
    {
        public int LogID { get; set; }
        public int JudgeID { get; set; }
        public string JudgeName { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public string CaseType { get; set; }
        public string CaseStatus { get; set; }
        public string County { get; set; }
        public string Attorney { get; set; }
        public string Description { get; set; }
        public DateTime? MotionFiled { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DateCompleted { get; set; }
        [IgnoreColumn]
        public DateTime? SixtiethDayDate
        {
            get
            {
                if (MotionFiled.HasValue)
                    return MotionFiled.Value.AddDays(60);
                return null;
            }
        }
    }

}
