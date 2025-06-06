using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class EventTypeController
    {
        public void CreateEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventType>();
                rep.Insert(t);
            }
        }
        public void DeleteEventType(int eventtypeId)
        {
            var t = GetEventType(eventtypeId);
            DeleteEventType(t);
        }
        public void DeleteEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventType>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventType> GetEventTypes()
        {
            IEnumerable<EventType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventType>();
                t = rep.Get();
            }
            return t;
        }
        public EventType GetEventType(int eventtypeId)
        {
            EventType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventType>();
                t = rep.GetById(eventtypeId);
            }
            return t;
        }
        public void UpdateEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventType>();
                rep.Update(t);
            }
        }
    }
}