using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
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
    }
}
