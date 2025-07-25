using DotNetNuke.Data;
using DotNetNuke.UI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class EventController
    {
        private const string CONN_JACS = "jacs"; // Connection

        public void CreateEvent(Event t)
        {
            ValidateEvent(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Insert(t);
            }
        }

        public void DeleteEvent(int eventId)
        {
            var t = GetEvent(eventId);
            if (t != null)
            {
                DeleteEvent(t);
            }
        }

        public void DeleteEvent(Event t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Event> GetEvents()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                return rep.Get();
            }
        }

        public IEnumerable<EventListItem> GetEventsByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT e.* 
                    FROM [events] e
                    INNER JOIN [timeslot_events] te ON te.event_id = e.id
                    INNER JOIN [court_timeslots] ct ON ct.timeslot_id = te.timeslot_id
                    WHERE ct.court_id = @0";
                return ctx.ExecuteQuery<EventListItem>(System.Data.CommandType.Text, query, courtId);
            }
        }

        public IEnumerable<Event> GetEventsByCourtId(long courtId, DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT e.*
                    FROM [events] e
                    INNER JOIN [timeslot_events] te ON te.event_id = e.id
                    INNER JOIN [timeslots] t ON t.id = te.timeslot_id
                    INNER JOIN [court_timeslots] ct ON ct.timeslot_id = t.id
                    WHERE ct.court_id = @0 AND t.start >= @1 AND t.[end] < @2 AND t.deleted_at IS NULL";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, courtId, start, end);
            }
        }

        public IEnumerable<EventListItem> GetEventListItems(string court, string category, string status)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventListItem>();
                return rep.Get();
            }
        }

        public IEnumerable<EventListItem> GetEventListItems(string searchTerm, long court_id, long category_id, long status_id, int offset, int pageSize, string sortOrder, string direction)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<EventListItem>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_list_paged",
                    searchTerm ?? string.Empty,
                    court_id,
                    category_id,
                    status_id,
                    offset,
                    pageSize,
                    sortOrder ?? "case_num",
                    direction ?? "asc"
                );
            }
        }

        public int GetEventListItemCount(string searchTerm, long court_id, long category_id, long status_id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_list_count",
                    searchTerm ?? string.Empty,
                    court_id,
                    category_id,
                    status_id
                );
            }
        }

        public IEnumerable<Event> GetEventsForDashboardByJudge(int judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 10 * FROM [events] WHERE 
                    EXISTS (
                        SELECT * FROM [timeslots] 
                        INNER JOIN [timeslot_events] ON [timeslot_events].[timeslot_id] = [timeslots].[id] 
                        WHERE [events].[id] = [timeslot_events].[event_id] AND 
                        EXISTS (
                            SELECT * FROM [courts] 
                            INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                            WHERE [timeslots].[id] = [court_timeslots].[timeslot_id] AND 
                            EXISTS (
                                SELECT * FROM [judges] 
                                WHERE [courts].[id] = [judges].[court_id] AND [judges].[id] = @0
                            )
                        )
                    ) 
                    ORDER BY [created_at] DESC";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, judgeId);
            }
        }

        public IEnumerable<Event> GetEventsForDashboard(int userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 10 * FROM [events] WHERE 
                    EXISTS (
                        SELECT * FROM [timeslots] 
                        INNER JOIN [timeslot_events] ON [timeslot_events].[timeslot_id] = [timeslots].[id] 
                        WHERE [events].[id] = [timeslot_events].[event_id] AND 
                        EXISTS (
                            SELECT * FROM [courts] 
                            INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                            WHERE [timeslots].[id] = [court_timeslots].[timeslot_id] AND 
                            EXISTS (
                                SELECT * FROM [judges] 
                                WHERE [courts].[id] = [judges].[court_id] AND 
                                EXISTS (
                                    SELECT * FROM [users] 
                                    INNER JOIN [court_permissions] ON [court_permissions].[user_id] = [users].[id]
                                    WHERE [judges].[id] = [court_permissions].[judge_id] AND [user_id] = @0 AND active = 1 AND editable = 1
                                )
                            )
                        )
                    ) 
                    ORDER BY [created_at] DESC";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, userId);
            }
        }

        public Event GetEvent(long eventId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                return rep.GetById(eventId);
            }
        }

        public void UpdateEvent(Event t)
        {
            ValidateEvent(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Update(t);
            }
        }

        public IEnumerable<Event> GetEventsForDashBoardByAdmin()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 10 * FROM [events] WHERE 
                    EXISTS (
                        SELECT * FROM [timeslots] 
                        INNER JOIN [timeslot_events] ON [timeslot_events].[timeslot_id] = [timeslots].[id] 
                        WHERE [events].[id] = [timeslot_events].[event_id] AND 
                        EXISTS (
                            SELECT * FROM [courts] 
                            INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                            WHERE [timeslots].[id] = [court_timeslots].[timeslot_id] AND 
                            EXISTS (
                                SELECT * FROM [judges] 
                                WHERE [courts].[id] = [judges].[court_id]
                            )
                        )
                    ) 
                    ORDER BY [created_at] DESC";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query);
            }
        }

        public Event GetEventByCaseNumber(string caseNumber)
        {
            string searchTerm = $"%{caseNumber}%";
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                return rep.Find("WHERE case_num LIKE @0", searchTerm).FirstOrDefault();
            }
        }

        public bool CancelEvent(long eventId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var evt = GetEvent(eventId);
                if (evt == null) return false;
                evt.status_id = ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT id FROM event_statuses WHERE name = 'Cancelled'");
                evt.cancellation_reason = "Cancelled via API";
                UpdateEvent(evt);
                return true;
            }
        }

        public IEnumerable<Event> GetEventsByTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"SELECT e.* FROM [events] e INNER JOIN [timeslot_events] te ON te.event_id = e.id WHERE te.timeslot_id = @0";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, timeslotId);
            }
        }

        public static string ValidateCaseNumber(string caseNumber, string caseFormat)
        {
            string caseNumberError = null;

            if (!string.IsNullOrEmpty(caseFormat))
            {
                var caseFormatArray = caseFormat.Split('-');
                var caseNumberArray = caseNumber.Split('-');

                if (caseFormatArray.Length == 1)
                {
                    caseNumberError = string.IsNullOrEmpty(caseNumber) ? "Please provide Full UCN!" : null;
                }
                else if (caseFormatArray.Length == 2)
                {
                    if (caseNumberArray.Length < 1 || caseNumberArray[0].Length != 4)
                    {
                        caseNumberError = "Please provide complete year!";
                    }
                    else if (caseNumberArray.Length < 2 || caseNumberArray[1].Length < 1 || caseNumberArray[1].Length > 8)
                    {
                        caseNumberError = "Please provide valid case number!";
                    }
                }
                else if (caseFormatArray.Length == 3)
                {
                    if (caseNumberArray.Length < 1 || !int.TryParse(caseNumberArray[0], out _) || caseNumberArray[0].Length != 4)
                    {
                        caseNumberError = "Please provide complete year!";
                    }
                    else if (caseNumberArray.Length < 2 || caseNumberArray[1].Length < 1)
                    {
                        caseNumberError = "Please select Case Code!";
                    }
                    else if (caseNumberArray.Length < 3 || !int.TryParse(caseNumberArray[2], out _) || caseNumberArray[2].Length < 3 || caseNumberArray[2].Length > 7)
                    {
                        caseNumberError = "Please provide valid case number!";
                    }
                    else if (caseFormatArray[1].Length == 2 || caseFormatArray[1] == "0")
                    {
                        if (caseNumberArray.Length < 1 || caseNumberArray[0].Length != 4)
                        {
                            caseNumberError = "Please provide complete year!";
                        }
                        else if (caseNumberArray.Length < 2 || caseNumberArray[1].Length < 1)
                        {
                            caseNumberError = "Please select Case Code!";
                        }
                        else if (caseNumberArray.Length < 3 || caseNumberArray[2].Length < 1 || caseNumberArray[2].Length > 7)
                        {
                            caseNumberError = "Please Enter case number!";
                        }
                    }
                    else
                    {
                        if (caseNumberArray.Length < 1 || caseNumberArray[0].Length != 4)
                        {
                            caseNumberError = "Please provide complete year!";
                        }
                        else if (caseNumberArray.Length < 2 || caseNumberArray[1].Length < 1 || (caseNumberArray.Length >= 3 && caseNumberArray[2].Length > 7))
                        {
                            caseNumberError = "Please Enter case number!";
                        }
                        else if (caseNumberArray.Length < 3 || caseNumberArray[2].Length < 1 || caseNumberArray[2].Length > 4)
                        {
                            caseNumberError = "Please provide Party/Defendant Identifier!";
                        }
                    }
                }
                else if (caseFormatArray.Length >= 4 && caseFormatArray.Length <= 6)
                {
                    if (caseNumberArray.Length < 1 || caseNumberArray[0].Length < 1 || caseNumberArray[0].Length > 2)
                    {
                        caseNumberError = "Please provide valid county number!";
                    }
                    else if (caseNumberArray.Length < 2 || !int.TryParse(caseNumberArray[1], out _) || caseNumberArray[1].Length != 4)
                    {
                        caseNumberError = "Please provide complete year with Numeric only!";
                    }
                    else if (caseNumberArray.Length < 3 || caseNumberArray[2].Length < 1)
                    {
                        caseNumberError = "Please select Case Code!";
                    }
                    else if (caseNumberArray.Length < 4 || !int.TryParse(caseNumberArray[3], out _) || caseNumberArray[3].Length < 3 || caseNumberArray[3].Length > 6)
                    {
                        caseNumberError = "Please Enter case number!";
                    }
                    else if (caseNumberArray.Length < 5 || caseNumberArray[4].Length < 1 || caseNumberArray[4].Length > 4)
                    {
                        caseNumberError = "Please provide Party/Defendant Identifier!";
                    }
                    else if (caseNumberArray.Length >= 6 && (caseNumberArray[5].Length < 1 || caseNumberArray[5].Length > 2))
                    {
                        caseNumberError = "Please provide Branch Location";
                    }
                }
            }
            else
            {
                caseNumberError = string.IsNullOrEmpty(caseNumber) ? "Please Enter case number!" : null;
            }

            return caseNumberError;
        }

        private void ValidateEvent(Event evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));
            if (string.IsNullOrWhiteSpace(evt.case_num))
                throw new ValidationException("Case number is required.");

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // Validate foreign keys
                if (evt.motion_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM motions WHERE id = @0", evt.motion_id.Value) == 0)
                    throw new ValidationException("Invalid motion ID.");
                if (evt.type_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM event_types WHERE id = @0", evt.type_id.Value) == 0)
                    throw new ValidationException("Invalid type ID.");
                if (evt.status_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM event_statuses WHERE id = @0", evt.status_id.Value) == 0)
                    throw new ValidationException("Invalid status ID.");
                if (evt.attorney_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM attorneys WHERE id = @0", evt.attorney_id.Value) == 0)
                    throw new ValidationException("Invalid attorney ID.");
                if (evt.opp_attorney_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM attorneys WHERE id = @0", evt.opp_attorney_id.Value) == 0)
                    throw new ValidationException("Invalid opposing attorney ID.");

                // Validate emails
                var emailValidator = new EmailAddressAttribute();
                if (!string.IsNullOrEmpty(evt.plaintiff_email) && !emailValidator.IsValid(evt.plaintiff_email))
                    throw new ValidationException("Invalid plaintiff email format.");
                if (!string.IsNullOrEmpty(evt.defendant_email) && !emailValidator.IsValid(evt.defendant_email))
                    throw new ValidationException("Invalid defendant email format.");

                // Court-specific validation
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourtByEventId(evt.id);
                if (court != null)
                {
                    if (court.plaintiff_required && string.IsNullOrWhiteSpace(evt.plaintiff))
                        throw new ValidationException("Plaintiff is required for this court.");
                    if (court.defendant_required && string.IsNullOrWhiteSpace(evt.defendant))
                        throw new ValidationException("Defendant is required for this court.");
                    string caseNumberError = ValidateCaseNumber(evt.case_num, court.case_num_format);
                    if (!string.IsNullOrEmpty(caseNumberError))
                        throw new ValidationException(caseNumberError);
                }
            }
        }
    }
}