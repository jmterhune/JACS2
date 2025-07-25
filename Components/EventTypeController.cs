using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class EventTypeController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventType>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteEventType(long eventtypeId)
        {
            var t = GetEventType(eventtypeId);
            DeleteEventType(t);
        }
        public void DeleteEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventType>();
                rep.Delete(t);
            }
        }
        public IEnumerable<EventType> GetEventTypes()
        {
            IEnumerable<EventType> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventType>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetEventTypeDropDownItems(string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<EventType>();
                    var results = rep.Find("WHERE name LIKE @0", $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.name)).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }
        public EventType GetEventType(long eventtypeId)
        {
            EventType t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventType>();
                t = rep.GetById(eventtypeId);
            }
            return t;
        }
        public void UpdateEventType(EventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<EventType>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        // Add to EventTypeController.cs
        public IEnumerable<EventType> GetEventTypesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<EventType>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_type_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetEventTypesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_event_type_count",
                    searchTerm ?? string.Empty
                );
            }
        }

       
    }
}