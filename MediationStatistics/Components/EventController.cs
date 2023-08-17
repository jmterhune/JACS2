using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class EventController
    {
        public void CreateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance())
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
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Event> GetEvents()
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Event> GetEventsBySession(int sessionId)
        {
            IEnumerable<Event> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.Find("Where SessionId = @0",sessionId);
            }
            return t;
        }

        public Event GetEvent(int eventId)
        {
            Event t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                t = rep.GetById(eventId);
            }
            return t;
        }

        public void UpdateEvent(Event t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Event>();
                rep.Update(t);
            }
        }
        public void CreateEventAppearance(EventAppearance eventAppearance)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_add_event_appearance", eventAppearance.EventId, eventAppearance.AppearanceId,  eventAppearance.CreatedById);
            }
        }
        public void DeleteEventAppearance(EventAppearance eventAppearance)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_event_appearance", eventAppearance.EventId, eventAppearance.AppearanceId);
            }
        }
        public void DeleteAllEventAppearances(int eventId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_all_event_appearances", eventId);
            }
        }

    }
}
