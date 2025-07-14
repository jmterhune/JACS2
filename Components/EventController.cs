using DotNetNuke.Data;
using DotNetNuke.UI.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;
using static tjc.Modules.jacs.Services.EventAPIController;
namespace tjc.Modules.jacs.Components
{
    internal class EventController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Insert(t);
            }
        }
        public void DeleteEvent(int eventId)
        {
            var t = GetEvent(eventId);
            DeleteEvent(t);
        }
        public void DeleteEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Event> GetEvents()
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<EventListItem> GetEventsByCourtId(long courtId)
        {
            IEnumerable<EventListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventListItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Event> GetEventsByCourtId(long courtId, DateTime start, DateTime end)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<EventListItem> GetEventListItems(string court, string category, string status)
        {
            IEnumerable<EventListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventListItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<EventListItem> GetEventListItems(string searchTerm, long court_id, long category_id, long status_id, int offset, int pageSize, string sortOrder, string direction)
        {
            IEnumerable<EventListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteQuery<EventListItem>(
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
            return t;
        }
        public int GetEventListItemCount(string searchTerm, long court_id, long category_id, long status_id)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_list_count",
                    searchTerm ?? string.Empty,
                    court_id,
                    category_id,
                    status_id);
            }
            return t;
        }
        public IEnumerable<Event> GetEventsForDashboardByJudge(int judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
            select top 10 * from [events] where 
                exists (select * from [timeslots] inner join [timeslot_events] on [timeslot_events].[timeslot_id] = [timeslots].[id] 
	            where [events].[id] = [timeslot_events].[event_id] and 
		        exists (select * from [courts] inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id] and [judges].[id] = @0))) order by [created_at] desc";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, judgeId);
            }
        }
        public IEnumerable<Event> GetEventsForDashboard(int userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"select top 10 * from [events] where 
                exists (select * from [timeslots] inner join [timeslot_events] on [timeslot_events].[timeslot_id] = [timeslots].[id] 
	            where [events].[id] = [timeslot_events].[event_id] and 
		        exists (select * from [courts] inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id] and 
		        exists (select * from [users] inner join [court_permissions] on [court_permissions].[user_id] = [users].[id]
                where [judges].[id] = [court_permissions].[judge_id] and [user_id]=@0 and active=1 and editiable=1)))) order by [created_at] desc";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, userId);
            }
        }
        public Event GetEvent(long eventId)
        {
            Event t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.GetById(eventId);
            }
            return t;
        }
        public void UpdateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                rep.Update(t);
            }
        }

        internal object GetEventsForDashBoardByAdmin()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"select top 10 * from [events] where 
                exists (select * from [timeslots] inner join [timeslot_events] on [timeslot_events].[timeslot_id] = [timeslots].[id] 
	            where [events].[id] = [timeslot_events].[event_id] and 
		        exists (select * from [courts] inner join [court_timeslots] on [court_timeslots].[court_id] = [courts].[id] 
		        where [timeslots].[id] = [court_timeslots].[timeslot_id] and 
		        exists (select * from [judges] where [courts].[id] = [judges].[court_id]))) order by [created_at] desc";
                return ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query);
            }
        }

        internal Event GetEventByCaseNumber(string caseNumber)
        {
            string searchTerm = $"%{caseNumber}%";
            Event t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Find("Where case_num like @0", searchTerm).FirstOrDefault();
            }
            return t;
        }
        internal bool CancelEvent(long eventId)
        {
            throw new NotImplementedException();
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

        public IEnumerable<Event> GetEventsByTimeslot(long timeslotId)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"select e.* from [events] e inner join [timeslot_events] te on te.event_id = e.id where te.timeslot_id = @0";
                t = ctx.ExecuteQuery<Event>(System.Data.CommandType.Text, query, timeslotId);
            }
            return t;
        }
    }
}