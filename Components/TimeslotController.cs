using DotNetNuke.Data;
using DotNetNuke.UI.UserControls;
using System;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslot(int timeslotId)
        {
            var t = GetTimeslot(timeslotId);
            DeleteTimeslot(t);
        }
        public void DeleteTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Timeslot> GetTimeslots()
        {
            IEnumerable<Timeslot> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Timeslot> GetTimeslotsForDashboardByJudge(int judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select top 15 * from [timeslots] 
	            where exists (select * from [courts] 
	            inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id] and 
                    [judges].[id] = @0)) and [start] >= GetDate() and [timeslots].[deleted_at] 
		            is null order by [start] desc";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, judgeId);
            }
        }
        public IEnumerable<Timeslot> GetTimeslotsForDashboard(int userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select top 15 * from [timeslots] 
	            where exists (select * from [courts] 
	            inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id] and 
                exists (select * from [users] inner join [court_permissions] on [court_permissions].[user_id] = [users].[id] 
		        where [judges].[id] = [court_permissions].[judge_id] and [user_id] =@0 and [active]=1 
                    and editable=1))) and [start] >= GetDate() and [timeslots].[deleted_at] 
		            is null order by [start] desc";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, userId);
            }
        }
        public Court GetCourtByTimeslot(long id)
        {
            Court court = null;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select [courts].* 
                from [courts] inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
                where [court_timeslots].[timeslot_id] = @0";
                court = ctx.ExecuteSingleOrDefault<Court>(System.Data.CommandType.Text, query, id);
            }
            return court;
        }
        public Timeslot GetTimeslot(long timeslotId)
        {
            Timeslot t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                t = rep.GetById(timeslotId);
            }
            return t;
        }
        public Timeslot GetTimeslotByEventId(long id)
        {
            Timeslot t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select top 1 [timeslots].*
                from [timeslots] inner join [timeslot_events] on [timeslot_events].[timeslot_id] = [timeslots].[id] 
                where [timeslot_events].[event_id] = @0";
                t = ctx.ExecuteSingleOrDefault<Timeslot>(System.Data.CommandType.Text, query, id);
            }
            return t;
        }
        public void UpdateTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Update(t);
            }
        }

        internal object GetTimeslotsForDashBoardByAdmin()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select top 15 * from [timeslots] 
	            where exists (select * from [courts] 
	            inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id])) and [start] >= GetDate() and [timeslots].[deleted_at] 
		        is null order by [start] desc";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query);
            }
        }

        public IEnumerable<CustomTimeslot> GetTimeslotsByCourtId(long courtId, DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select t.*, (select count(*) from timeslot_events te where te.timeslot_id = t.id) as eventCount 
                from [timeslots] t 
                inner join [court_timeslots] ct on ct.timeslot_id = t.id 
                where ct.court_id = @0 and t.start >= @1 and t.[end] < @2 and t.deleted_at is null 
                order by t.start";
                return ctx.ExecuteQuery<CustomTimeslot>(System.Data.CommandType.Text, query, courtId, start, end);
            }
        }

        public IEnumerable<Timeslot> GetOverlappingTimeslots(long courtId, DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                select t.* from [timeslots] t 
                inner join [court_timeslots] ct on ct.timeslot_id = t.id 
                where ct.court_id = @0 
                and t.deleted_at is null 
                and not (t.[end] <= @1 or t.start >= @2)";
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text, query, courtId, start, end);
            }
        }
    }
}