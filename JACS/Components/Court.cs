using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
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
        public long? def_attorney_id { get; set; }
        public string plaintiff { get; set; }
        public long? opp_attorney_id { get; set; }
        public string defendant { get; set; }
        public bool scheduling { get; set; }
        public string web_policy { get; set; }
        public bool public_timeslot { get; set; }
        public bool public_docket { get; set; }
        public byte? public_docket_days { get; set; }
        public bool email_confirmations { get; set; }
        public byte? lagtime { get; set; }
        public string custom_email_body { get; set; }
        public byte twitter_notification { get; set; }
        public int calendar_weeks { get; set; }
        public bool auto_extension { get; set; }
        public bool plaintiff_required { get; set; }
        public bool defendant_required { get; set; }
        public bool defendant_attorney_required { get; set; }
        public bool plaintiff_attorney_required { get; set; }
        public bool category_print { get; set; }
        public byte? max_lagtime { get; set; }
        public string custom_header { get; set; }
        public string timeslot_header { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int case_format_type { get; set; }
    }
}
