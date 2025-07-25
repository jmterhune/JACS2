using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class EventStatusController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteEventStatus(long eventStatusId)
        {
            var t = GetEventStatus(eventStatusId);
            DeleteEventStatus(t);
        }
        public void DeleteEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventStatus> GetEventStatuses()
        {
            IEnumerable<EventStatus> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long,string>> GetEventStatusDropDownItems()
        {
            IEnumerable<EventStatus> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                t = rep.Get();
            }
            return t.Select(es=>new KeyValuePair<long, string>(es.id,es.name)).ToList();
        }
        public EventStatus GetEventStatus(long eventStatusId)
        {
            EventStatus t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                t = rep.GetById(eventStatusId);
            }
            return t;
        }
        public void UpdateEventStatus(EventStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventStatus>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<EventStatus> GetEventStatusesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<EventStatus>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_status_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetEventStatusesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_status_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}