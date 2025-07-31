using DotNetNuke.Data;
using System;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class EventReminderController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateEventReminder(EventReminder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<EventReminder>();
                rep.Insert(t);
            }
        }
        public void DeleteEventReminder(int eventreminderId)
        {
            var t = GetEventReminder(eventreminderId);
            DeleteEventReminder(t);
        }
        public void DeleteEventReminder(EventReminder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventReminder>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventReminder> GetEventReminders()
        {
            IEnumerable<EventReminder> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventReminder>();
                t = rep.Get();
            }
            return t;
        }
        public EventReminder GetEventReminder(int eventreminderId)
        {
            EventReminder t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventReminder>();
                t = rep.GetById(eventreminderId);
            }
            return t;
        }
        public void UpdateEventReminder(EventReminder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;
                var rep = ctx.GetRepository<EventReminder>();
                rep.Update(t);
            }
        }
    }
}