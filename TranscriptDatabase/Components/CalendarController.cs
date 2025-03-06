using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class CalendarController
    {
        public void CreateCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Insert(t);
            }
        }
        public void DeleteCalendar(int calendarId)
        {
            var t = GetCalendar(calendarId);
            DeleteCalendar(t);
        }
        public void DeleteCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Calendar> GetCalendars()
        {
            IEnumerable<Calendar> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.Get();
            }
            return t;
        }
        public Calendar GetCalendar(int calendarId)
        {
            Calendar t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.GetById(calendarId);
            }
            return t;
        }
        public Calendar GetCalendarByDesignation(int designationId)
        {
            Calendar t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                t = rep.Find("Where DesignationID=@0",designationId).FirstOrDefault();
            }
            return t;
        }
        public void UpdateCalendar(Calendar t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Calendar>();
                rep.Update(t);
            }
        }
    }
}
