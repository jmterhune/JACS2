using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("courts")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Courts", CacheItemPriority.Default, 20)]
    internal class Court
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public string case_num_format { get; set; }
        public long county_id { get; set; }
        public long def_attorney_id { get; set; }
        public string plaintiff { get; set; }
        public long opp_attorney_id { get; set; }
        public string defendant { get; set; }
        public bool scheduling { get; set; }
        public string web_policy { get; set; }
        public bool public_timeslot { get; set; }
        public bool public_docket { get; set; }
        public int? public_docket_days { get; set; }
        public bool email_confirmations { get; set; }
        public int? lagtime { get; set; }
        public string custom_email_body { get; set; }
        public int twitter_notification { get; set; }
        public int calendar_weeks { get; set; }
        public bool auto_extension { get; set; }
        public bool plaintiff_required { get; set; }
        public bool defendant_required { get; set; }
        public bool defendant_attorney_required { get; set; }
        public bool plaintiff_attorney_required { get; set; }
        public bool category_print { get; set; }
        public int? max_lagtime { get; set; }
        public string custom_header { get; set; }
        public string timeslot_header { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int case_format_type { get; set; }
        [IgnoreColumn]
        public string county_name { get; set; }
        [IgnoreColumn]
        public string judge_name { get; set; }
        [IgnoreColumn]
        public KeyValuePair<long, string> def_attorney_item { get; set; }
        [IgnoreColumn]
        public KeyValuePair<long, string> opp_attorney_item { get; set; }
       
        public List<KeyValuePair<long, string>> GetCourtMotionDropDownItems(long id,bool allowed)
        {
            var ctl = new CourtMotionController();
            return ctl.GetCourtMotionDropDownItems(id,allowed);
        }
        public IEnumerable<CourtMotion> GetCourtMotions(long id)
        {
            var ctl = new CourtMotionController();
            return ctl.GetCourtMotions(id);
        }
        public List<long> GetCourtMotionValues(bool allowed)
        {
            var ctl = new CourtMotionController();
            return ctl.GetCourtMotionValuesByCourtId(id, allowed);
        }
        public List<KeyValuePair<long, string>> GetCourtEventTypes()
        {
            var ctl = new CourtEventTypeController();
            return ctl.GetCourtEventTypesByCourtId(id);
        }
        public List<long> GetCourtEventTypeValues()
        {
            var ctl = new CourtEventTypeController();
            return ctl.GetCourtEventTypeValuesByCourtId(id);
        }
        public Judge GetJudge()
        {
            var ctl = new JudgeController();
            var judge= ctl.GetJudgeByCourt(id);
            if (judge!=null)
            {
                return judge;
            }
            return new Judge { id = 0, name = string.Empty };
        }
    }
    [TableName("getUserCourtViewPermissions")] // Optional: Specify if not inferring from class name
    public class MyResultModel
    {
        public long court_id { get; set; }
    }
}
