using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
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
            long courtId = 0;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var ctl = new CourtMotionController();
                var ctlEvent = new CourtEventTypeController();
                var rep = ctx.GetRepository<Court>();
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
                    def_attorney_id = t.def_attorney_id.HasValue ? t.def_attorney_id.Value : (long?)null,
                    opp_attorney_id = t.opp_attorney_id.HasValue ? t.opp_attorney_id.Value : (long?)null,
                    defendant_attorney_required = t.defendant_attorney_required,
                    defendant_required = t.defendant_required,
                    description = t.description,
                    email_confirmations = t.email_confirmations,
                    lagtime = t.lagtime,
                    max_lagtime = t.max_lagtime,
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
                rep.Insert(court);
                courtId = court.id;
                if (t.id > 0)
                {
                    foreach (var motion in t.restricted_motions)
                    {
                        ctl.CreateCourtMotion(new CourtMotion { allowed = false, court_id = courtId, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                    }
                    foreach (var motion in t.available_motions)
                    {
                        ctl.CreateCourtMotion(new CourtMotion { allowed = true, court_id = courtId, motion_id = motion, updated_at = DateTime.Now, created_at = DateTime.Now });
                    }
                    foreach (var eventType in t.available_hearing_types)
                    {
                        ctlEvent.CreateCourtEventType(new CourtEventType { court_id = courtId, event_type_id = eventType, created_at = DateTime.Now, updated_at = DateTime.Now });
                    }
                }
            }
        }

        public void DeleteCourt(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // stored procedure removes the court and associated data
                ctx.Execute(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_delete_court", courtId
                );
                DataCache.ClearCache("Courts");
            }
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
        public IEnumerable<long> GetPermittedCourts(long userId)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                // Full SELECT query against the TVF with parameter
                var sql = "SELECT * FROM dbo.getUserCourtViewPermissions(@0)";
                return context.ExecuteQuery<long>(CommandType.Text, sql, userId);
            }
        }
        public IEnumerable<long> GetEditableCourts(long userId)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                // Full SELECT query against the TVF with parameter
                var sql = "SELECT * FROM dbo.getUserCourtEditPermissions(@0)";
                return context.ExecuteQuery<long>(CommandType.Text, sql, userId);
            }
        }
        public List<KeyValuePair<long, string>> GetCourtDropDownItems(long userId, string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Court>();
                    var results = rep.Find("WHERE id in (select court_id from dbo.getUserCourtViewPermissions(@0)) AND description LIKE @1", userId, $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.description)).OrderBy(c => c.Value).ToList();
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
                    if (t.def_attorney_id.HasValue)
                        court.def_attorney_id = t.def_attorney_id.Value;
                    court.description = t.description;
                    court.email_confirmations = t.email_confirmations;
                    court.lagtime = t.lagtime;
                    court.max_lagtime = t.max_lagtime;
                    if (t.opp_attorney_id.HasValue)
                        court.opp_attorney_id = t.opp_attorney_id.Value;
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
                }
                var rep = ctx.GetRepository<Court>();
                court.updated_at = DateTime.Now;
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

        public IEnumerable<CourtViewModel> GetCourtsPaged(long userId, string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // Pass userId to the stored procedure, update the procedure to filter by user if needed
                return ctx.ExecuteQuery<CourtViewModel>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_paged",
                    userId,
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
        public int GetCourtsCount(long userId, string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_count",
                    userId,
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
        public TruncateResponse TruncateTimeslots(long courtId, DateTime startDate, string filter)
        {
            var timeslotSQL = "select [timeslots].* from [timeslots] inner join [court_timeslots] on [court_timeslots].[timeslot_id] = [timeslots].[id] where [court_timeslots].[court_id] = @0 and [start] >= @1";
            bool handleEvents = false;
            switch (filter)
            {
                case "all":
                    timeslotSQL += " and [timeslots].[deleted_at] is null";
                    handleEvents = true;
                    break;
                case "hearings":
                    timeslotSQL += " and not exists (select * from [events] inner join [timeslot_events] on [timeslot_events].[event_id] = [events].[id] where [timeslots].[id] = [timeslot_events].[timeslot_id] and [timeslot_events].[deleted_at] is null) and [timeslots].[deleted_at] is null";

                    break;
                case "templates":
                    handleEvents = true;
                    timeslotSQL += " and [template_id] is not null and [blocked] = 0 and [timeslots].[deleted_at] is null";
                    break;
                case "both":
                    timeslotSQL += " and not exists (select * from [events] inner join [timeslot_events] on [timeslot_events].[event_id] = [events].[id] where [timeslots].[id] = [timeslot_events].[timeslot_id] and [timeslot_events].[deleted_at] is null) and [template_id] is not null and [timeslots].[deleted_at] is null";

                    break;
                default:
                    break;
            }
            bool success = true;
            string error = string.Empty;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var timeslots = ctx.ExecuteQuery<Timeslot>(CommandType.Text, timeslotSQL, courtId, startDate);
                foreach (var timeslot in timeslots)
                {
                    if (handleEvents)
                    {
                        var ctlEvent = new EventController();
                        var events = ctlEvent.GetEventsByTimeslot(timeslot.id);
                        foreach (var ev in events)
                        {
                            // in clerk interface, when truncating timeslots, we cancel events then delete the timeslot
                            ev.status_id = 1;// cancelled
                            ev.updated_at = DateTime.Now;
                            ev.cancellation_reason = "Calendar Truncated.";
                            // if(!ClerkAPI.CancelEvent(ev.clerkeventId){success=false; error+="Clerk event number " + ev.clerkeventId + " could not be cancelled" + environment.newline; }
                        }
                    }
                    var ctl = new TimeslotController();
                    timeslot.deleted_at = DateTime.Now;
                    timeslot.updated_at = DateTime.Now;
                    ctl.UpdateTimeslot(timeslot);
                }
            }
            return new TruncateResponse { Success = success, Error = error };
        }
        public ExtendResponse AutoExtendCalendar(ExtendRequest request, Court court)
        {
            try
            {
                // --------------------------------------------------------------------
                // Controllers & data
                // --------------------------------------------------------------------
                var holidayCtl = new HolidayController();
                var courtTemplateCtl = new CourtTemplateController();
                var courtTemplateOrderCtl = new CourtTemplateOrderController();
                var timeslotCtl = new TimeslotController();
                var courtTimeslotCtl = new CourtTimeslotController();

                if (court == null)
                    return new ExtendResponse { success = false, message = "Court does not exist" };

                var holidays = holidayCtl.GetHolidays().ToList();
                var orderedTemplates = courtTemplateOrderCtl.GetAutoCourtTemplateOrders(request.CourtId)
                                                              .OrderBy(t => t.order)
                                                              .ToList();

                if (!orderedTemplates.Any())
                    return new ExtendResponse { success = false, message = "No auto-templates defined for this court" };

                // --------------------------------------------------------------------
                // Determine the *first* day we will write timeslots for
                // --------------------------------------------------------------------
                DateTime startDate = request.StartDate.Date;               // user-selected start
                DateTime endDate = startDate.AddDays(request.Weeks * 7); // exclusive upper bound

                // Find the first template that matches the requested order
                var firstTemplateOrder = orderedTemplates.FirstOrDefault(t => t.order == request.StartTemplateOrder);
                if (firstTemplateOrder?.template_id == null)
                    return new ExtendResponse { success = false, message = "Requested start-template order not found" };

                // --------------------------------------------------------------------
                // Walk day-by-day from startDate → endDate
                // --------------------------------------------------------------------
                long currentTemplateIdx = orderedTemplates.IndexOf(firstTemplateOrder);
                int createdCount = 0;

                for (DateTime day = startDate; day < endDate; day = day.AddDays(1))
                {
                    // ----- pick the template for this day (round-robin by order) -----
                    var orderItem = orderedTemplates[(int)currentTemplateIdx];
                    var template = courtTemplateCtl.GetCourtTemplate(orderItem.template_id.Value);
                    var tmplSlots = courtTemplateCtl.GetTemplateTimeslots(template.id)
                                  .Where(ts => ts.day == (int)day.DayOfWeek)
                                  .ToList();
                    bool isHoliday = holidays.Any(h => h.date.Date == day);

                    foreach (var ts in tmplSlots)
                    {
                        var newTs = new Timeslot
                        {
                            start = day.Add(ts.start.TimeOfDay),
                            end = day.Add(ts.end.TimeOfDay),
                            description = ts.description,
                            allDay = ts.allDay,
                            duration = ts.duration,
                            quantity = ts.quantity,
                            blocked = isHoliday ? true : ts.blocked,
                            block_reason = string.IsNullOrEmpty(ts.block_reason) ? null : ts.block_reason,
                            public_block = ts.public_block,
                            category_id = ts.category_id,
                            template_id = template.id,
                            court_template_order_id = orderItem.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };

                        timeslotCtl.CreateTimeslot(newTs);
                        courtTimeslotCtl.CreateCourtTimeslot(new CourtTimeslot
                        {
                            court_id = court.id,
                            timeslot_id = newTs.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });

                        createdCount++;
                    }
                    if (day.DayOfWeek == DayOfWeek.Saturday)
                        currentTemplateIdx = (currentTemplateIdx + 1) % orderedTemplates.Count;

                }
                return new ExtendResponse
                {
                    success = true,
                    message = "Auto Extension Successful",
                    extendedCount = createdCount
                };
            }
            catch (Exception ex)
            {
                return new ExtendResponse
                {
                    success = false,
                    message = $"Auto Extend Failed! Error: {ex.Message}"
                };
            }
        }
    }
}