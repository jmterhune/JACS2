using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace tjc.Modules.jacs.Components
{
    internal class TimeslotController
    {
        private const string CONN_JACS = "jacs"; // Connection
        private static readonly int[] AllowedDurations = { 5, 10, 15, 20, 30, 45, 60, 90, 120, 150, 165, 180, 210, 240, 300, 360, 480, 1440 };

        public void CreateTimeslot(Timeslot t)
        {
            ValidateTimeslot(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<Timeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslot(long timeslotId)
        {
            var t = GetTimeslot(timeslotId);
            if (t != null)
            {
                DeleteTimeslot(t);
            }
        }
        public void DeleteTimeslot(Timeslot t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM court_timeslots WHERE timeslot_id = @0", t.id);
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM timeslot_motions WHERE timeslotable_type = 'Timeslot' AND timeslotable_id = @0", t.id);
                var rep = ctx.GetRepository<Timeslot>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Timeslot> GetTimeslots()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                return rep.Get();
            }
        }
        public IEnumerable<Timeslot> GetTimeslotsForDashboardByJudge(long judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 15 * FROM [timeslots] 
                    WHERE EXISTS (
                        SELECT * FROM [courts] 
                        INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                        WHERE [timeslots].[id] = [court_timeslots].[timeslot_id] AND 
                        EXISTS (
                            SELECT * FROM [judges] 
                            WHERE [courts].[id] = [judges].[court_id] AND [judges].[id] = @0
                        )
                    ) 
                    AND [start] >= GETDATE() AND [timeslots].[deleted_at] IS NULL 
                    ORDER BY [start] DESC";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, judgeId);
            }
        }
        public IEnumerable<Timeslot> GetTimeslotsForDashboard(long userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 15 * FROM [timeslots] 
                    WHERE EXISTS (
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
                    AND [start] >= GETDATE() AND [timeslots].[deleted_at] IS NULL 
                    ORDER BY [start] DESC";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, userId);
            }
        }
        public IEnumerable<Timeslot> GetTimeslotsByCourtIdAfterDate(long courtId, DateTime date)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT ts.*
                    FROM timeslots ts
                    INNER JOIN court_timeslots ct ON ct.timeslot_id = ts.id
                    WHERE ct.court_id = @0 AND ts.start >= @1 AND ts.deleted_at IS NULL";
                var timeslots = ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, courtId, date);
                // Load related TimeslotEvents if needed for filtering
                var teCtl = new TimeslotEventController();
                foreach (var ts in timeslots)
                {
                    ts.TimeslotEvents = teCtl.GetTimeslotEventsByTimeslot(ts.id).ToList();
                }
                return timeslots;
            }
        }
        public Court GetCourtByTimeslot(long id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT [courts].* 
                    FROM [courts] 
                    INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                    WHERE [court_timeslots].[timeslot_id] = @0";
                return ctx.ExecuteSingleOrDefault<Court>(System.Data.CommandType.Text, query, id);
            }
        }
        public Timeslot GetTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                return rep.GetById(timeslotId);
            }
        }
        public Timeslot GetTimeslotByEventId(long id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 1 [timeslots].*
                    FROM [timeslots] 
                    INNER JOIN [timeslot_events] ON [timeslot_events].[timeslot_id] = [timeslots].[id] 
                    WHERE [timeslot_events].[event_id] = @0";
                return ctx.ExecuteSingleOrDefault<Timeslot>(System.Data.CommandType.Text, query, id);
            }
        }
        public void UpdateTimeslot(Timeslot t)
        {
            ValidateTimeslot(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<Timeslot>();
                rep.Update(t);
            }
        }
        public IEnumerable<Timeslot> GetTimeslotsForDashBoardByAdmin()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT TOP 15 * FROM [timeslots] 
                    WHERE EXISTS (
                        SELECT * FROM [courts] 
                        INNER JOIN [court_timeslots] ON [court_timeslots].[court_id] = [courts].[id] 
                        WHERE [timeslots].[id] = [court_timeslots].[timeslot_id] AND 
                        EXISTS (
                            SELECT * FROM [judges] 
                            WHERE [courts].[id] = [judges].[court_id]
                        )
                    ) 
                    AND [start] >= GETDATE() AND [timeslots].[deleted_at] IS NULL 
                    ORDER BY [start] DESC";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query);
            }
        }
        public IEnumerable<CustomTimeslot> GetTimeslotsByCourtId(long courtId, DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT t.*, (SELECT COUNT(*) FROM timeslot_events te WHERE te.timeslot_id = t.id) AS eventCount 
                    FROM [timeslots] t 
                    INNER JOIN [court_timeslots] ct ON ct.timeslot_id = t.id 
                    WHERE ct.court_id = @0 AND t.start >= @1 AND t.[end] < @2 AND t.deleted_at IS NULL 
                    ORDER BY t.start";
                return ctx.ExecuteQuery<CustomTimeslot>(System.Data.CommandType.Text, query, courtId, start, end);
            }
        }
        public IEnumerable<Timeslot> GetOverlappingTimeslots(long courtId, DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT t.* FROM [timeslots] t 
                    INNER JOIN [court_timeslots] ct ON ct.timeslot_id = t.id 
                    WHERE ct.court_id = @0 
                    AND t.deleted_at IS NULL 
                    AND NOT (t.[end] <= @1 OR t.start >= @2)";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, courtId, start, end);
            }
        }
        public long[] GetRestrictedMotionsForTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<TimeslotMotion>(
                    System.Data.CommandType.Text,
                    "SELECT motion_id FROM timeslot_motions WHERE timeslotable_type = 'Timeslot' AND timeslotable_id = @0",
                    timeslotId).Select(m => m.motion_id).ToArray();
            }
        }
        public int GetEventCountForTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM timeslot_events 
                    WHERE timeslot_id = @0";
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text, query, timeslotId);
            }
        }
        public TimeslotMotion GetTimeslotMotion(long timeslotMotionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                return rep.GetById(timeslotMotionId);
            }
        }
        public void CreateTimeslotMotion(TimeslotMotion timeslotMotion)
        {
            ValidateTimeslotMotion(timeslotMotion);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.GetRepository<TimeslotMotion>().Insert(timeslotMotion);
            }
        }
        public void UpdateTimeslotMotion(TimeslotMotion timeslotMotion)
        {
            ValidateTimeslotMotion(timeslotMotion);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var existing = ctx.GetRepository<TimeslotMotion>().GetById(timeslotMotion.id);
                if (existing == null)
                {
                    throw new ValidationException("Timeslot motion not found.");
                }
                ctx.GetRepository<TimeslotMotion>().Update(timeslotMotion);
            }
        }
        public void DeleteTimeslotMotion(long timeslotMotionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var timeslotMotion = ctx.GetRepository<TimeslotMotion>().GetById(timeslotMotionId);
                if (timeslotMotion == null)
                {
                    throw new ValidationException("Timeslot motion not found.");
                }
                ctx.GetRepository<TimeslotMotion>().Delete(timeslotMotion);
            }
        }
        public void DeleteTimeslotMotionsForTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM timeslot_motions WHERE timeslotable_type = 'Timeslot' AND timeslotable_id = @0",
                    timeslotId);
            }
        }
        public IEnumerable<TimeslotMotion> GetTimeslotMotions(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<TimeslotMotion>(
                    System.Data.CommandType.Text,
                    "SELECT * FROM timeslot_motions WHERE timeslotable_type = 'Timeslot' AND timeslotable_id = @0",
                    timeslotId);
            }
        }
        private void ValidateTimeslot(Timeslot t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (t.start >= t.end)
                throw new ValidationException("End time must be after start time.");
            if (t.duration <= 0 || !Array.Exists(AllowedDurations, d => d == t.duration))
                throw new ValidationException($"Invalid duration. Must be one of: {string.Join(",", AllowedDurations)} minutes.");
            if (t.quantity < 1)
                throw new ValidationException("Quantity must be at least 1.");

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                if (t.category_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM categories WHERE id = @0", t.category_id.Value) == 0)
                    throw new ValidationException("Invalid category ID.");

                var overlaps = ctx.ExecuteScalar<long>(System.Data.CommandType.Text,
                    "SELECT COUNT(*) FROM timeslots ts JOIN court_timeslots ct ON ts.id = ct.timeslot_id " +
                    "WHERE ct.court_id = (SELECT court_id FROM court_timeslots WHERE timeslot_id = @0) " +
                    "AND ts.id != @0 " +
                    "AND (ts.start < @2 AND ts.[end] > @1 OR ts.start BETWEEN @1 AND @2 OR ts.[end] BETWEEN @1 AND @2)",
                    t.id, t.start, t.end);

                if (overlaps > 0 && !t.allDay)
                    throw new ValidationException("Timeslot overlaps with existing timeslots.");
            }
        }
        private void ValidateTimeslotMotion(TimeslotMotion timeslotMotion)
        {
            if (timeslotMotion == null) throw new ArgumentNullException(nameof(timeslotMotion));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM timeslots WHERE id = @0", timeslotMotion.timeslotable_id) == 0)
                    throw new ValidationException("Invalid timeslot ID for motion.");
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM motions WHERE id = @0", timeslotMotion.motion_id) == 0)
                    throw new ValidationException("Invalid motion ID.");
                var courtId = ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT court_id FROM court_timeslots WHERE timeslot_id = @0", timeslotMotion.timeslotable_id);
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM court_motions WHERE court_id = @0 AND motion_id = @1 AND allowed = 1", courtId, timeslotMotion.motion_id) == 0)
                    throw new ValidationException("Motion is not allowed in this court.");
            }
        }
        private long[] GetUserCourts(int userId)
        {
            using (IDataContext ctx = DataContext.Instance("jacs"))
            {
                string query = @"
                    SELECT DISTINCT c.id 
                    FROM courts c 
                    INNER JOIN judges j ON j.court_id = c.id
                    INNER JOIN court_permissions cp ON cp.judge_id = j.id
                    WHERE cp.user_id = @0 AND cp.active = 1";
                var courts = ctx.ExecuteQuery<long>(System.Data.CommandType.Text, query, userId);
                return courts.ToArray();
            }
        }
        public IEnumerable<TimeslotListItem> GetTimeslotListItems(long userId, string searchTerm, long court_id, DateTime startDate, DateTime endDate, int offset, int pageSize, string sortOrder, string direction)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<TimeslotListItem>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_timeslot_list_paged",
                    userId,
                    searchTerm ?? string.Empty,
                    court_id,
                    startDate.ToShortDateString(),
                    endDate.ToShortDateString(),
                    offset,
                    pageSize,
                    sortOrder ?? "ts.start",
                    direction ?? "asc"
                );
            }
        }
        public int GetTimeslotListCount(long userId, string searchTerm, long court_id, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_timeslot_list_count",
                    userId,
                    searchTerm ?? string.Empty,
                    court_id,
                    startDate.ToShortDateString(),
                    endDate.ToShortDateString()
                );
            }
        }
    }
}