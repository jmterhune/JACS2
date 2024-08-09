using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Users;
using System.Web.Caching;

namespace tjc.Modules.HearingLog.Components
{
    [TableName("tjc_hearing_jacs_judges")]
    [PrimaryKey("JacsUserID", AutoIncrement = true)]
    [Cacheable("JacsJudges", CacheItemPriority.Default, 20)]
    internal class JacsJudge
    {
        public int JacsUserID { get; set; }
        public string JudgeID { get; set; }
        public string County { get; set; }
        public string JudgeName { get; set; }
    }
    internal class UserJacsJudge : JacsJudge
    {
        public int UserID { get; set; }
    }
    [TableName("tjc_hearing_jacs_userid_by_userid")]
    internal class RefJacsUserByAppUser
    {
        public int JACSUserID { get; set; }
        public int UserID { get; set; }
    }
    internal class ExistingJacsJudges
    {
        public int JACSUserID { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
