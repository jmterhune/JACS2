using DotNetNuke.Data;
using System;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotEventController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTimeslotEvent(TimeslotEvent t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
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
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TimeslotEvent> GetTimeslotEvents()
        {
            IEnumerable<TimeslotEvent> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.Get();
            }
            return t;
        }
        public TimeslotEvent GetTimeslotEvent(int timesloteventId)
        {
            TimeslotEvent t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.GetById(timesloteventId);
            }
            return t;
        }
        public void UpdateTimeslotEvent(TimeslotEvent t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Update(t);
            }
        }

        public IEnumerable<TimeslotEvent> GetTimeslotEventsByTimeslot(long id)
        {
            IEnumerable<TimeslotEvent> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.Find("Where timeslot_id=@0",id);
            }
            return t;
        }
        public IEnumerable<TimeslotEvent> GetTimeslotEventsByEvent(long id)
        {
            IEnumerable<TimeslotEvent> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                t = rep.Find("Where event_id=@0", id);
            }
            return t;
        }
    }
}