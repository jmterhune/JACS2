using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.HearingLog.Components
{
    [TableName("tjc_hearing_judge_ja")]
    internal class JudgeJa
    {
        public int JudgeUserID { get; set; }
        public int JaUserID { get; set; }
    }
}
