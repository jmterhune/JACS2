using System;

namespace tjc.Modules.jacs.Components
{
    internal class DocketDataItem
    {
        public long timeslot_id { get; set; }
        public DateTime timeslot_start { get; set; }
        public DateTime timeslot_end { get; set; }
        public int duration { get; set; }
        public bool blocked { get; set; }
        public bool public_block { get; set; }
        public string timeslot_description { get; set; }
        public long? courtroom_id { get; set; }
        public int has_event { get; set; }
        public long? event_id { get; set; }
        public string case_num { get; set; }
        public string plaintiff { get; set; }
        public string defendant { get; set; }
        public string notes { get; set; }
        public long? motion_id { get; set; }
        public string custom_motion { get; set; }
        public long? attorney_id { get; set; }
        public long? opp_attorney_id { get; set; }
        public string motion_description { get; set; }
        public string attorney_name { get; set; }
        public string attorney_phone { get; set; }
        public string opp_attorney_name { get; set; }
        public string opp_attorney_phone { get; set; }
    }
}