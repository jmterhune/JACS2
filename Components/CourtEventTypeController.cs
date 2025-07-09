using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    internal class CourtEventTypeController
    {
        private const string CONN_JACS = "jacs";

        public void CreateCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Insert(t);
            }
        }

        public void DeleteCourtEventType(int courteventtypeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                var entity = rep.GetById(courteventtypeId);
                if (entity != null)
                {
                    rep.Delete(entity);
                }
            }
        }

        public void DeleteCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtEventType> GetCourtEventTypes()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                return rep.Get();
            }
        }

        public CourtEventType GetCourtEventType(int courteventtypeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                return rep.GetById(courteventtypeId);
            }
        }

        public void UpdateCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Update(t);
            }
        }

        internal List<CourtListItem> GetCourtEventTypesByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
            SELECT e.id as value, e.name as text
            FROM court_event_types ce
            INNER JOIN event_types e ON ce.event_type_id = e.id
            WHERE ce.court_id = @0";
                return ctx.ExecuteQuery<CourtListItem>(System.Data.CommandType.Text, query, courtId).ToList();
            }
        }
        internal List<int> GetCourtEventTypeValuesByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
            SELECT e.id
            FROM court_event_types ce
            INNER JOIN event_types e ON ce.event_type_id = e.id
            WHERE ce.court_id = @0";
                return ctx.ExecuteQuery<int>(System.Data.CommandType.Text, query, courtId).ToList();
            }
        }

        public void DeleteCourtEventTypesByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtEventType>();
                var eventTypes = rep.Find("WHERE court_id = @0", courtId);
                foreach (var eventType in eventTypes)
                {
                    rep.Delete(eventType);
                }
            }
        }
    }
}