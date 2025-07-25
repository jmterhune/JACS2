using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tjc.Modules.jacs.Components
{
    internal class TimeslotEventController
    {
        private const string CONN_JACS = "jacs"; // Connection

        public void CreateTimeslotEvent(TimeslotEvent t)
        {
            ValidateTimeslotEvent(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Insert(t);
            }
        }

        public void DeleteTimeslotEvent(int timeslotEventId)
        {
            var t = GetTimeslotEvent(timeslotEventId);
            if (t != null)
            {
                DeleteTimeslotEvent(t);
            }
        }

        public void DeleteTimeslotEvent(TimeslotEvent t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Delete(t);
            }
        }

        public IEnumerable<TimeslotEvent> GetTimeslotEvents()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                return rep.Get();
            }
        }

        public TimeslotEvent GetTimeslotEvent(int timeslotEventId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                return rep.GetById(timeslotEventId);
            }
        }

        public void UpdateTimeslotEvent(TimeslotEvent t)
        {
            ValidateTimeslotEvent(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                rep.Update(t);
            }
        }

        public IEnumerable<TimeslotEvent> GetTimeslotEventsByTimeslot(long id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                return rep.Find("WHERE timeslot_id = @0", id);
            }
        }

        public IEnumerable<TimeslotEvent> GetTimeslotEventsByEvent(long id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotEvent>();
                return rep.Find("WHERE event_id = @0", id);
            }
        }

        private void ValidateTimeslotEvent(TimeslotEvent t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                if (t.event_id.HasValue && ctx.ExecuteScalar<long>( System.Data.CommandType.Text,"SELECT COUNT(*) FROM events WHERE id = @0", t.event_id.Value) == 0)
                    throw new ValidationException("Invalid event ID.");
                if (t.timeslot_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM timeslots WHERE id = @0", t.timeslot_id.Value) == 0)
                    throw new ValidationException("Invalid timeslot ID.");
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM timeslot_events WHERE event_id = @0 AND timeslot_id = @1", t.event_id, t.timeslot_id) > 0)
                    throw new ValidationException("Duplicate timeslot-event association.");
            }
        }
    }
}