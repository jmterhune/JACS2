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
        public int LogID { get; set; }            // int (logId)
        public int JudgeID { get; set; }          // int
        public string JudgeName { get; set; }     // varchar(25)
        public string CaseName { get; set; }      // varchar(100)
        public string CaseNumber { get; set; }    // varchar(18)
        public string CaseType { get; set; }      // varchar(50)
        public string CaseStatus { get; set; }    // nvarchar(50)
        public string County { get; set; }        // varchar(10)
        public string Attorney { get; set; }      // varchar(25)
        public string Description { get; set; }   // varchar(100)
        public DateTime? MotionFiled { get; set; } // smalldatetime
        public DateTime DateReceived { get; set; } // smalldatetime
        public DateTime DateCompleted { get; set; } // smalldatetime
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
