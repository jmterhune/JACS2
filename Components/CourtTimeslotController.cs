using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tjc.Modules.jacs.Components
{
    internal class CourtTimeslotController
    {
        private const string CONN_JACS = "jacs"; // Connection

        public void CreateCourtTimeslot(CourtTimeslot t)
        {
            ValidateCourtTimeslot(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Insert(t);
            }
        }

        public void DeleteCourtTimeslot(long courtTimeslotId)
        {
            var t = GetCourtTimeslot(courtTimeslotId);
            if (t != null)
            {
                DeleteCourtTimeslot(t);
            }
        }

        public void DeleteCourtTimeslot(CourtTimeslot t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM timeslot_motions WHERE timeslotable_type = 'Timeslot' AND timeslotable_id = @0", t.timeslot_id);
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtTimeslot> GetCourtTimeslots()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                return rep.Get();
            }
        }

        public CourtTimeslot GetCourtTimeslot(long courtTimeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                return rep.GetById(courtTimeslotId);
            }
        }

        public CourtTimeslot GetCourtTimeslotByCourtAndTimeslot(long courtId, long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteSingleOrDefault<CourtTimeslot>(
                    System.Data.CommandType.Text,
                    "SELECT * FROM court_timeslots WHERE court_id = @0 AND timeslot_id = @1",
                    courtId, timeslotId);
            }
        }

        public void UpdateCourtTimeslot(CourtTimeslot t)
        {
            ValidateCourtTimeslot(t);
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var existing = ctx.GetRepository<CourtTimeslot>().GetById(t.id);
                if (existing == null)
                {
                    throw new ValidationException("Court-timeslot mapping not found.");
                }
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Update(t);
            }
        }

        private void ValidateCourtTimeslot(CourtTimeslot t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                if (t.court_id.HasValue && ctx.ExecuteScalar<long>( System.Data.CommandType.Text,"SELECT COUNT(*) FROM courts WHERE id = @0", t.court_id.Value) == 0)
                    throw new ValidationException("Invalid court ID.");
                if (t.timeslot_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM timeslots WHERE id = @0", t.timeslot_id.Value) == 0)
                    throw new ValidationException("Invalid timeslot ID.");
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM court_timeslots WHERE court_id = @0 AND timeslot_id = @1", t.court_id, t.timeslot_id) > 0)
                    throw new ValidationException("Duplicate court-timeslot association.");
            }
        }
    }
}