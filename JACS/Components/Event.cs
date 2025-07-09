using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("events")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Events", CacheItemPriority.Default, 20)]
    internal class Event
    {
        public long id { get; set; }
        public string case_num { get; set; }
        public string notes { get; set; }
        public string plaintiff { get; set; }
        public string defendant { get; set; }
        public long? motion_id { get; set; }
        public long? attorney_id { get; set; }
        public long? type_id { get; set; }
        public long? status_id { get; set; }
        public bool reminder { get; set; }
        public long? opp_attorney_id { get; set; }
        public long? owner_id { get; set; }
        public string owner_type { get; set; }
        public bool? addon { get; set; }
        public string plaintiff_email { get; set; }
        public string defendant_email { get; set; }
        public string cancellation_reason { get; set; }
        public string template { get; set; }
        public string telephone { get; set; }
        public string custom_motion { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public string motion_name
        {
            get
            {
                if (motion_id.HasValue)
                {
                    var ctl = new MotionController();
                    var m = ctl.GetMotion(motion_id.Value);
                    if (m != null)
                        return m.description;
                }
                return string.Empty;
            }
        }
        [IgnoreColumn]
        public string attorney_name
        {
            get
            {
                if (attorney_id.HasValue)
                {
                    var ctl = new AttorneyController();
                    var a = ctl.GetAttorney(attorney_id.Value);
                    if (a != null)
                        return a.name;
                }
                return string.Empty;
            }
        }
        [IgnoreColumn]
        public string opp_attorney_name
        {
            get
            {
                if (opp_attorney_id.HasValue)
                {
                    var ctl = new AttorneyController();
                    var a = ctl.GetAttorney(opp_attorney_id.Value);
                    if (a != null)
                        return a.name;
                }
                return string.Empty;
            }
        }
        [IgnoreColumn]
        public string status_name
        {
            get
            {
                if (status_id.HasValue)
                {
                    var ctl = new EventStatusController();
                    var s = ctl.GetEventStatus(status_id.Value);
                    if (s != null)
                        return s.name;
                }
                return string.Empty;
            }
        }
        [IgnoreColumn]
        public string timeslot_desc
        {
            get
            {
                var ctl = new TimeslotController();
                var tslot = ctl.GetTimeslotByEventId(id);
                if (tslot != null)
                    return tslot.FormattedStart;
                return string.Empty;
            }
        }
        [IgnoreColumn]
        public string court_name
        {
            get
            {
                var ctl = new CourtController();
                var court = ctl.GetCourtByEventId(id);
                if (court != null)
                {
                    return court.description;
                }
                return string.Empty;
            }
        }
    }
    [TableName("event_list")]
    [PrimaryKey("id", AutoIncrement = false)]
    [Cacheable("EventsListItem", CacheItemPriority.Default, 20)]
    internal class EventListItem
    {
        public long id { get; set; }
        public string case_num { get; set; }
        public string notes { get; set; }
        public string plaintiff { get; set; }
        public string defendant { get; set; }
        public long? motion_id { get; set; }
        public long? attorney_id { get; set; }
        public long? type_id { get; set; }
        public long? status_id { get; set; }
        public bool reminder { get; set; }
        public long? opp_attorney_id { get; set; }
        public long? owner_id { get; set; }
        public string owner_type { get; set; }
        public bool? addon { get; set; }
        public string plaintiff_email { get; set; }
        public string defendant_email { get; set; }
        public string cancellation_reason { get; set; }
        public string template { get; set; }
        public string telephone { get; set; }
        public string custom_motion { get; set; }
        public string motion_name { get; set; }
        public string attorney_name { get; set; }
        public string opp_attorney_name { get; set; }
        public string status_name { get; set; }
        public string timeslot_desc { get; set; }
        public string court_name { get; set; }
        public string category_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}