using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class CourtController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Court>();
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                rep.Insert(t);
            }
        }
        public void CreateCourt(CourtViewModel t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var ctl = new CourtMotionController();
                var ctlEvent = new CourtEventTypeController();
                // Convert CourtViewModel to Court entity
                Court court = new Court
                {
                    county_id = t.county_id,
                    auto_extension = t.auto_extension,
                    calendar_weeks = t.calendar_weeks,
                    case_format_type = t.case_format_type,
                    case_num_format = t.case_num_format,
                    category_print = t.category_print,
                    custom_email_body = t.custom_email_body,
                    custom_header = t.custom_header,
                    defendant = t.defendant,
                    defendant_attorney_required = t.defendant_attorney_required,
                    defendant_required = t.defendant_required,
                    def_attorney_id = t.def_attorney_id.HasValue ? t.def_attorney_id.Value : -1,
                    description = t.description,
                    email_confirmations = t.email_confirmations,
                    lagtime = t.lagtime,
                    max_lagtime = t.max_lagtime,
                    opp_attorney_id = t.opp_attorney_id.HasValue ? t.opp_attorney_id.Value : -1,
                    plaintiff = t.plaintiff,
                    plaintiff_attorney_required = t.plaintiff_attorney_required,
                    plaintiff_required = t.plaintiff_required,
                    public_docket = t.public_docket,
                    public_docket_days = t.public_docket_days,
                    public_timeslot = t.public_timeslot,
                    scheduling = t.scheduling,
                    timeslot_header = t.timeslot_header,
                    web_policy = t.web_policy,
                    twitter_notification = t.twitter_notification,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now,
                };

                var rep = ctx.GetRepository<Court>();
                rep.Insert(court);
                if (court.id > 0)
                {
                    foreach (var motion in t.restricted_motions)
                    {
                        ctl.CreateCourtMotion(new CourtMotion { allowed = false, court_id = court.id, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                    }
                    foreach (var motion in t.available_motions)
                    {
                        ctl.CreateCourtMotion(new CourtMotion { allowed = true, court_id = court.id, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                    }
                    foreach (var eventType in t.available_hearing_types)
                    {
                        ctlEvent.CreateCourtEventType(new CourtEventType { court_id = court.id, event_type_id = eventType, created_at = DateTime.Now, updated_at = DateTime.Now });
                    }
                }
            }
        }

        public void DeleteCourt(long courtId)
        {
            var t = GetCourt(courtId);
            DeleteCourt(t);
        }

        public void DeleteCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Court>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Court> GetCourts()
        {
            IEnumerable<Court> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.GetRepository<Court>().Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetCourtDropDownItems(string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Court>();
                    var results = rep.Find("WHERE description LIKE @0", $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.description)).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }
        public Court GetCourt(long courtId)
        {
            Court t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.GetRepository<Court>().GetById(courtId);
            }
            return t;
        }

        public void UpdateCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Court>();
                t.updated_at = DateTime.Now;
                rep.Update(t);
            }
        }

        public void UpdateCourt(CourtViewModel t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var ctl = new CourtMotionController();
                var ctlEvent = new CourtEventTypeController();
                Court court = GetCourt(t.id);
                if (court != null)
                {
                    court.county_id = t.county_id;
                    court.auto_extension = t.auto_extension;
                    court.calendar_weeks = t.calendar_weeks;
                    court.case_format_type = t.case_format_type;
                    court.case_num_format = t.case_num_format;
                    court.category_print = t.category_print;
                    court.custom_email_body = t.custom_email_body;
                    court.custom_header = t.custom_header;
                    court.defendant = t.defendant;
                    court.defendant_attorney_required = t.defendant_attorney_required;
                    court.defendant_required = t.defendant_required;
                    court.def_attorney_id = t.def_attorney_id ?? -1;
                    court.description = t.description;
                    court.email_confirmations = t.email_confirmations;
                    court.lagtime = t.lagtime;
                    court.max_lagtime = t.max_lagtime;
                    court.opp_attorney_id = t.opp_attorney_id ?? -1;
                    court.plaintiff = t.plaintiff;
                    court.plaintiff_attorney_required = t.plaintiff_attorney_required;
                    court.plaintiff_required = t.plaintiff_required;
                    court.public_docket = t.public_docket;
                    court.public_docket_days = t.public_docket_days;
                    court.public_timeslot = t.public_timeslot;
                    court.scheduling = t.scheduling;
                    court.timeslot_header = t.timeslot_header;
                    court.web_policy = t.web_policy;
                    court.twitter_notification = t.twitter_notification;
                    court.updated_at = DateTime.Now;
                }
                var rep = ctx.GetRepository<Court>();

                rep.Update(court);
                ctl.DeleteCourtMotionsByCourtId(court.id);
                ctlEvent.DeleteCourtEventTypesByCourtId(court.id);
                foreach (var motion in t.restricted_motions)
                {
                    ctl.CreateCourtMotion(new CourtMotion { allowed = false, court_id = court.id, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                }
                foreach (var motion in t.available_motions)
                {
                    ctl.CreateCourtMotion(new CourtMotion { allowed = true, court_id = court.id, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                }
                foreach (var eventType in t.available_hearing_types)
                {
                    ctlEvent.CreateCourtEventType(new CourtEventType { court_id = court.id, event_type_id = eventType, created_at = DateTime.Now, updated_at = DateTime.Now });
                }
            }
        }

        public IEnumerable<CourtViewModel> GetCourtsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtViewModel>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetCourtsCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_count",
                    searchTerm ?? string.Empty
                );
            }
        }
        public Court GetCourtByEventId(long id)
        {
            Court t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select top 1 [courts].*
                from [courts] inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
                    inner join [timeslot_events] on [court_timeslots].timeslot_id=[timeslot_events].timeslot_id
                where [timeslot_events].[event_id] =@0";
                t = ctx.ExecuteSingleOrDefault<Court>(System.Data.CommandType.Text, query, id);
            }
            return t;
        }

        public DateTime? GetLastTimeslotDate(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT MAX(ts.start) 
                    FROM timeslots ts 
                    INNER JOIN court_timeslots ct ON ct.timeslot_id = ts.id 
                    WHERE ct.court_id = @0 AND ts.deleted_at IS NULL";
                return ctx.ExecuteScalar<DateTime?>(System.Data.CommandType.Text, query, courtId);
            }
        }

        public DateTime? GetLastHearingDate(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT MAX(ts.start) 
                    FROM timeslots ts 
                    INNER JOIN timeslot_events te ON te.timeslot_id = ts.id 
                    INNER JOIN court_timeslots ct ON ct.timeslot_id = ts.id 
                    WHERE ct.court_id = @0 AND ts.deleted_at IS NULL";
                return ctx.ExecuteScalar<DateTime?>(System.Data.CommandType.Text, query, courtId);
            }
        }
    }
}