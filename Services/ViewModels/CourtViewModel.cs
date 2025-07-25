using DotNetNuke.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtViewModel
    {
        public CourtViewModel(Court court)
        {
            id = court.id;
            description = court.description;
            case_num_format = court.case_num_format;
            county_id = court.county_id;
            def_attorney_id = court.def_attorney_id;
            plaintiff = court.plaintiff;
            opp_attorney_id = court.opp_attorney_id;
            defendant = court.defendant;
            scheduling = court.scheduling;
            web_policy = court.web_policy;
            public_timeslot = court.public_timeslot;
            public_docket = court.public_docket;
            public_docket_days = court.public_docket_days;
            email_confirmations = court.email_confirmations;
            lagtime = court.lagtime;
            custom_email_body = court.custom_email_body;
            twitter_notification = court.twitter_notification;
            calendar_weeks = court.calendar_weeks;
            auto_extension = court.auto_extension;
            plaintiff_required = court.plaintiff_required;
            defendant_required = court.defendant_required;
            defendant_attorney_required = court.defendant_attorney_required;
            plaintiff_attorney_required = court.plaintiff_attorney_required;
            category_print = court.category_print;
            max_lagtime = court.max_lagtime;
            custom_header = court.custom_header;
            timeslot_header = court.timeslot_header;
            case_format_type = court.case_format_type;
            county_name = court.county_name;
            judge_name = GetJudgeName(court.id);
            def_attorney_item = GetAttorneyItem(court.def_attorney_id);
            opp_attorney_item = GetAttorneyItem(court.opp_attorney_id);
            available_motions = court.GetCourtMotionValues(true);
            restricted_motions = court.GetCourtMotionValues(false);
            available_hearing_types = court.GetCourtEventTypeValues();
            restricted_motion_items = court.GetCourtMotionDropDownItems(court.id,false);
            available_motion_items = court.GetCourtMotionDropDownItems(court.id, true);
            available_hearing_type_items = court.GetCourtEventTypes();
        }

        public CourtViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("case_num_format")]
        public string case_num_format { get; set; }
        [JsonProperty("county_id")]
        public long county_id { get; set; }
        [JsonProperty("def_attorney_id")]
        public long? def_attorney_id { get; set; }        
        [JsonProperty("plaintiff")]
        public string plaintiff { get; set; }
        [JsonProperty("opp_attorney_id")]
        public long? opp_attorney_id { get; set; }
        [JsonProperty("defendant")]
        public string defendant { get; set; }
        [JsonProperty("scheduling")]
        public bool scheduling { get; set; }
        [JsonProperty("web_policy")]
        public string web_policy { get; set; }
        [JsonProperty("public_timeslot")]
        public bool public_timeslot { get; set; }
        [JsonProperty("public_docket")]
        public bool public_docket { get; set; }
        [JsonProperty("public_docket_days")]
        public int? public_docket_days { get; set; }
        [JsonProperty("email_confirmations")]
        public bool email_confirmations { get; set; }
        [JsonProperty("lagtime")]
        public int? lagtime { get; set; }
        [JsonProperty("custom_email_body")]
        public string custom_email_body { get; set; }
        [JsonProperty("twitter_notification")]
        public int twitter_notification { get; set; }
        [JsonProperty("calendar_weeks")]
        public int calendar_weeks { get; set; }
        [JsonProperty("auto_extension")]
        public bool auto_extension { get; set; }
        [JsonProperty("plaintiff_required")]
        public bool plaintiff_required { get; set; }
        [JsonProperty("defendant_required")]
        public bool defendant_required { get; set; }
        [JsonProperty("defendant_attorney_required")]
        public bool defendant_attorney_required { get; set; }
        [JsonProperty("plaintiff_attorney_required")]
        public bool plaintiff_attorney_required { get; set; }
        [JsonProperty("category_print")]
        public bool category_print { get; set; }
        [JsonProperty("max_lagtime")]
        public int? max_lagtime { get; set; }
        [JsonProperty("custom_header")]
        public string custom_header { get; set; }
        [JsonProperty("timeslot_header")]
        public string timeslot_header { get; set; }
        [JsonProperty("case_format_type")]
        public int case_format_type { get; set; }
        [JsonProperty("county_name")]
        public string county_name { get; set; }
        [JsonProperty("judge_name")]
        public string judge_name { get; set; }
        [JsonProperty("def_attorney_item")]
        public KeyValuePair<long, string> def_attorney_item { get; set; }
        [JsonProperty("opp_attorney_item")]
        public KeyValuePair<long, string> opp_attorney_item { get; set; }
        [JsonProperty("available_motions")]
        public List<int> available_motions { get; set; }
        [JsonProperty("restricted_motions")]
        public List<int> restricted_motions { get; set; }
        [JsonProperty("available_hearing_types")]
        public List<int> available_hearing_types { get; set; }
        [JsonProperty("available_motion_items")]
        public List<KeyValuePair<long, string>> available_motion_items { get; set; }
        [JsonProperty("restricted_motion_items")]
        public List<KeyValuePair<long, string>> restricted_motion_items { get; set; }
        [JsonProperty("available_hearing_type_items")]
        public List<KeyValuePair<long, string>> available_hearing_type_items { get; set; }
        [JsonProperty("has_revisions")]
        public bool has_revisions { get; set; }

        private KeyValuePair<long, string> GetAttorneyItem(long attorneyId) { 
        var controller = new AttorneyController();
            return controller.GetAttorneyListItem(attorneyId);
        }
        private string GetJudgeName(long courtId)
        {
            var controller = new JudgeController();
            var judge= controller.GetJudgeByCourt(courtId);
            if (judge != null)
            {
                return judge.name;
            }
            return string.Empty;
        }
    }
}