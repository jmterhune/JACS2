using Newtonsoft.Json;
using System;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EventViewModel
    {
        public EventViewModel(Event eventData)
        {
            id = eventData.id;
            clerk_case_id = eventData.clerk_case_id;
            clerk_event_id  = eventData.clerk_event_id;
            case_num = eventData.case_num;
            notes = eventData.notes;
            plaintiff = eventData.plaintiff;
            defendant = eventData.defendant;
            motion_id = eventData.motion_id ?? -1;
            attorney_id = eventData.attorney_id ?? -1;
            type_id = eventData.type_id ?? -1;
            status_id = eventData.status_id ?? -1;
            reminder = eventData.reminder;
            opp_attorney_id = eventData.opp_attorney_id ?? -1;
            owner_id = eventData.owner_id ?? -1;
            owner_type = eventData.owner_type;
            owner_username = eventData.owner_username;
            addon = eventData.addon ?? false;
            plaintiff_email = eventData.plaintiff_email;
            defendant_email = eventData.defendant_email;
            cancellation_reason = eventData.cancellation_reason;
            start_formatted = eventData.start_formatted;
            template = eventData.template;
            telephone = eventData.telephone;
            custom_motion = eventData.custom_motion;
            created_at = eventData.created_at ?? DateTime.Now;
            updated_at = eventData.updated_at ?? DateTime.Now;
            motion_name = eventData.motion_name;
            attorney_name = eventData.attorney_name;
            opp_attorney_name = eventData.opp_attorney_name;
            status_name = eventData.status_name;
            timeslot_desc = eventData.timeslot_desc;
            court_name = eventData.court_name;
            updated_by_name = eventData.owner_id.HasValue ? GetUserName(eventData.owner_id.Value) : "";
            duration = GetTimeslotDuration(eventData.id);
            editable = true; // Assume true for Event object
            if (eventData.attorney_id.HasValue && eventData.attorney_id.Value > 0)
                attorney_bar_num = GetAttorneyBarNum(eventData.attorney_id.Value);
            if (eventData.opp_attorney_id.HasValue && eventData.opp_attorney_id.Value > 0)
                opp_attorney_bar_num = GetAttorneyBarNum(eventData.opp_attorney_id.Value);
        }
        public EventViewModel(EventListItem eventData)
        {
            id = eventData.id;
            clerk_case_id = eventData.clerk_case_id; 
            clerk_event_id = eventData.clerk_event_id;
            case_num = eventData.case_num;
            notes = eventData.notes;
            plaintiff = eventData.plaintiff;
            defendant = eventData.defendant;
            motion_id = eventData.motion_id ?? -1;
            attorney_id = eventData.attorney_id ?? -1;
            type_id = eventData.type_id ?? -1;
            status_id = eventData.status_id ?? -1;
            reminder = eventData.reminder;
            opp_attorney_id = eventData.opp_attorney_id ?? -1;
            owner_id = eventData.owner_id ?? -1;
            owner_type = eventData.owner_type;
            owner_username = eventData.owner_username;
            addon = eventData.addon ?? false;
            plaintiff_email = eventData.plaintiff_email;
            defendant_email = eventData.defendant_email;
            cancellation_reason = eventData.cancellation_reason;
            template = eventData.template;
            start_formatted = eventData.start_formatted;
            telephone = eventData.telephone;
            custom_motion = eventData.custom_motion;
            created_at = eventData.created_at ?? DateTime.Now;
            updated_at = eventData.updated_at ?? DateTime.Now;
            motion_name = eventData.motion_name;
            attorney_name = eventData.attorney_name;
            opp_attorney_name = eventData.opp_attorney_name;
            status_name = eventData.status_name;
            timeslot_desc = eventData.timeslot_desc;
            court_name = eventData.court_name;
            court_id = eventData.court_id ?? -1;
            courtroom_name = eventData.courtroom_name;
            start_date = eventData.start.HasValue ? eventData.start.Value.ToShortDateString() : "";
            start_time = eventData.start.HasValue ? eventData.start.Value.ToShortTimeString() : "";
            duration = eventData.duration;
            // List rows: skip the per-row DNN UserController and Attorney lookups
            // — the list grid never displays these fields, and the lookups were
            // making the page do ~150 extra DB round-trips for a 50-row page.
            // updated_by_name now reads owner_username straight off the row.
            updated_by_name = eventData.owner_username ?? "";
            editable = true; // Assume true for EventListItem
        }
        public EventViewModel(EventListItemPaged eventData)
        {
            id = eventData.id;
            clerk_case_id = eventData.clerk_case_id;
            clerk_event_id = eventData.clerk_event_id;
            case_num = eventData.case_num;
            notes = eventData.notes;
            plaintiff = eventData.plaintiff;
            defendant = eventData.defendant;
            motion_id = eventData.motion_id ?? -1;
            attorney_id = eventData.attorney_id ?? -1;
            type_id = eventData.type_id ?? -1;
            status_id = eventData.status_id ?? -1;
            reminder = eventData.reminder;
            opp_attorney_id = eventData.opp_attorney_id ?? -1;
            owner_id = eventData.owner_id ?? -1;
            owner_type = eventData.owner_type;
            owner_username = eventData.owner_username;
            addon = eventData.addon ?? false;
            plaintiff_email = eventData.plaintiff_email;
            defendant_email = eventData.defendant_email;
            cancellation_reason = eventData.cancellation_reason;
            start_formatted = eventData.start_formatted;
            template = eventData.template;
            telephone = eventData.telephone;
            custom_motion = eventData.custom_motion;
            created_at = eventData.created_at ?? DateTime.Now;
            updated_at = eventData.updated_at ?? DateTime.Now;
            motion_name = eventData.motion_name;
            attorney_name = eventData.attorney_name;
            opp_attorney_name = eventData.opp_attorney_name;
            status_name = eventData.status_name;
            timeslot_desc = eventData.timeslot_desc;
            court_name = eventData.court_name;
            court_id = eventData.court_id ?? -1;
            courtroom_name = eventData.courtroom_name;
            start_date = eventData.start.HasValue ? eventData.start.Value.ToShortDateString() : "";
            start_time = eventData.start.HasValue ? eventData.start.Value.ToShortTimeString() : "";
            duration = eventData.duration;
            // Same N+1 cleanup as the EventListItem constructor — see comment there.
            updated_by_name = eventData.owner_username ?? "";
            editable = eventData.editable;
        }
        public EventViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("case_num")]
        public string case_num { get; set; }
        [JsonProperty("notes")]
        public string notes { get; set; }
        [JsonProperty("plaintiff")]
        public string plaintiff { get; set; }
        [JsonProperty("defendant")]
        public string defendant { get; set; }
        [JsonProperty("motion_id")]
        public long motion_id { get; set; }
        [JsonProperty("attorney_id")]
        public long attorney_id { get; set; }
        [JsonProperty("type_id")]
        public long type_id { get; set; }
        [JsonProperty("status_id")]
        public long status_id { get; set; }
        [JsonProperty("reminder")]
        public bool reminder { get; set; }
        [JsonProperty("opp_attorney_id")]
        public long opp_attorney_id { get; set; }
        [JsonProperty("owner_id")]
        public long owner_id { get; set; }
        [JsonProperty("owner_type")]
        public string owner_type { get; set; }
        [JsonProperty("owner_username")]
        public string owner_username { get; set; }
        [JsonProperty("addon")]
        public bool addon { get; set; }
        [JsonProperty("plaintiff_email")]
        public string plaintiff_email { get; set; }
        [JsonProperty("defendant_email")]
        public string defendant_email { get; set; }
        [JsonProperty("cancellation_reason")]
        public string cancellation_reason { get; set; }
        [JsonProperty("template")]
        public string template { get; set; }
        [JsonProperty("start_formatted")]
        public string start_formatted { get; set; }
        [JsonProperty("telephone")]
        public string telephone { get; set; }
        [JsonProperty("custom_motion")]
        public string custom_motion { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("motion_name")]
        public string motion_name { get; set; }
        [JsonProperty("attorney_name")]
        public string attorney_name { get; set; }
        [JsonProperty("attorney_bar_num")]
        public string attorney_bar_num { get; set; }
        [JsonProperty("opp_attorney_name")]
        public string opp_attorney_name { get; set; }
        [JsonProperty("opp_attorney_bar_num")]
        public string opp_attorney_bar_num { get; set; }
        [JsonProperty("status_name")]
        public string status_name { get; set; }
        [JsonProperty("timeslot_desc")]
        public string timeslot_desc { get; set; }
        [JsonProperty("timeslot_id")]
        public long timeslot_id { get; set; }
        [JsonProperty("court_name")]
        public string court_name { get; set; }
        [JsonProperty("court_id")]
        public long court_id { get; set; }
        [JsonProperty("courtroom_name")]
        public string courtroom_name { get; set; }
        [JsonProperty("duration")]
        public int duration { get; set; }
        [JsonProperty("start_date")]
        public string start_date { get; set; }
        [JsonProperty("start_time")]
        public string start_time { get; set; }
        [JsonProperty("updated_by_name")]
        public string updated_by_name { get; set; }
        [JsonProperty("editable")]
        public bool editable { get; set; }
        [JsonProperty("clerk_event_id")]
        public long clerk_event_id { get; set; }
        [JsonProperty("clerk_case_id")]
        public long clerk_case_id { get; set; }
        private string GetUserName(long userId)
        {
            try
            {
                var user = DotNetNuke.Entities.Users.UserController.GetUserById(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, (int)userId);
                if (user != null)
                {
                    return user.DisplayName;
                }
            }
            catch
            {
                // Log exception if necessary
            }
            return "";
        }
        private string GetAttorneyBarNum(long attorneyId)
        {
            try
            {
                var attorney = new AttorneyController().GetAttorney(attorneyId);
                return attorney?.bar_num;
            }
            catch { return null; }
        }
        private int GetTimeslotDuration(long eventId)
        {
            try
            {
                var ctl = new TimeslotController();
                var timeslot = ctl.GetTimeslotByEventId(eventId);
                if (timeslot != null)
                {
                    return timeslot.duration;
                }
            }
            catch
            {
                // Log exception if necessary
            }
            return 0;
        }
    } 
}