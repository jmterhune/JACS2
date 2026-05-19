using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("events")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Events", CacheItemPriority.Default, 20)]
    internal class Event
    {
        public long id { get; set; }
        public long clerk_event_id { get; set; } // from clerk's event_list view
        public long clerk_case_id { get; set; } // from clerk's case table
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
        // DNN username of the user who created/last-saved this event. Populated
        // by EventAPIController on Create and Update. Used to surface "Edited By"
        // on the event form across the public (attorney) and internal (judge/JA)
        // DNN sites, where DNN UserIDs aren't shared but usernames are.
        public string owner_username { get; set; }
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
                    return Motion.description;
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
               var tslot = timeslot;
                if (tslot != null)
                    return tslot.description;
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
        [IgnoreColumn]
        public Motion Motion
        {
            get
            {
                if (motion_id.HasValue)
                {
                    var ctl = new MotionController();
                    return ctl.GetMotion(motion_id.Value);
                }
                return null;
            }
        }
        [IgnoreColumn]
        public Attorney Attorney
        {
            get
            {
                if (attorney_id.HasValue)
                {
                    var ctl = new AttorneyController();
                    var a = ctl.GetAttorney(attorney_id.Value);
                    if (a != null)
                        return a;
                }
                return new Attorney();
            }
        }
        [IgnoreColumn]
        public Attorney opposing_attorney
        {
            get
            {
                if (opp_attorney_id.HasValue)
                {
                    var ctl = new AttorneyController();
                    var a = ctl.GetAttorney(opp_attorney_id.Value);
                    if (a != null)
                        return a;
                }
                return new Attorney();
            }
        }
        [IgnoreColumn]
        public EventType EventType
        {
            get
            {

                if (type_id.HasValue)
                {
                    var ctl = new EventTypeController();
                    var et = ctl.GetEventType(type_id.Value);
                    if (et != null)
                        return et;
                }
                return new EventType();
            }
        }
        [IgnoreColumn]
        public Timeslot timeslot
        {
            get
            {
                var ctl = new TimeslotController();
                return ctl.GetTimeslotByEventId(id);
            }
        }
        [IgnoreColumn]
        public string start_formatted
        {
            get
            {
                var tslot = timeslot;
                if (tslot != null)
                    return tslot.formatted_start;
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
        public long clerk_event_id { get; set; } // from clerk's event_list view
        public long clerk_case_id { get; set; } // from clerk's case table
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
        public string owner_username { get; set; }
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
        public long ? court_id { get; set; }
        public string courtroom_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? start { get; set; }
        public int duration { get; set; }
        public long? timeslot_id { get; set; }
        public string event_type { get; set; }
          [IgnoreColumn]
        public string start_formatted
        {
            get
            {
                return start.HasValue ? start.Value.ToString("MM/dd/yyyy @ hh:mm tt") : string.Empty;
            }
        }

    }
    internal class EventListItemPaged :EventListItem{ 
        public bool editable { get; set; }

    }
    public class CalendarItem
    {
        public long calendarId { get; set; }
        public long timeslotId { get; set; }
        public long eventId { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
    internal class EventListItemResult
    {
        public List<EventViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class EventsResult
    {
        public List<EventViewModel> data { get; set; }
        public string error { get; set; }
    }

    internal class EventSearchResult
    {
        public EventViewModel data { get; set; }
        public string error { get; set; }
    }

    internal class EventCancelResult
    {
        public bool cancelled { get; set; }
        public string error { get; set; }
    }

    internal class CaseSearchModel
    {
        public string casePattern { get; set; }
        public int userId { get; set; }
        public bool isJudge { get; set; }
    }

    internal class SearchTerm
    {
        public string searchTerm { get; set; }
    }

}