using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class EventStatusController
    {
        public void CreateEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventStatus>();
                rep.Insert(t);
            }
        }
        public void DeleteEventStatus(int eventstatusId)
        {
            var t = GetEventStatus(eventstatusId);
            DeleteEventStatus(t);
        }
        public void DeleteEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventStatus>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventStatus> GetEventStatuss()
        {
            IEnumerable<EventStatus> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventStatus>();
                t = rep.Get();
            }
            return t;
        }
        public EventStatus GetEventStatus(int eventstatusId)
        {
            EventStatus t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventStatus>();
                t = rep.GetById(eventstatusId);
            }
            return t;
        }
        public void UpdateEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventStatus>();
                rep.Update(t);
            }
        }
    }
}
