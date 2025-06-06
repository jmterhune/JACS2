using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotEventController
    {
        public void CreateTimeslotEvent(TimeslotEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslotEvent(int timesloteventId)
        {
            var t = GetTimeslotEvent(timesloteventId);
            DeleteTimeslotEvent(t);
        }
        public void DeleteTimeslotEvent(TimeslotEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TimeslotEvent> GetTimeslotEvents()
        {
            IEnumerable<TimeslotEvent> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.Get();
            }
            return t;
        }
        public TimeslotEvent GetTimeslotEvent(int timesloteventId)
        {
            TimeslotEvent t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.GetById(timesloteventId);
            }
            return t;
        }
        public void UpdateTimeslotEvent(TimeslotEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Update(t);
            }
        }
    }
}