using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class EventReminderController
    {
        public void CreateEventReminder(EventReminder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
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
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventReminder>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventReminder> GetEventReminders()
        {
            IEnumerable<EventReminder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventReminder>();
                t = rep.Get();
            }
            return t;
        }
        public EventReminder GetEventReminder(int eventreminderId)
        {
            EventReminder t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventReminder>();
                t = rep.GetById(eventreminderId);
            }
            return t;
        }
        public void UpdateEventReminder(EventReminder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EventReminder>();
                rep.Update(t);
            }
        }
    }
}