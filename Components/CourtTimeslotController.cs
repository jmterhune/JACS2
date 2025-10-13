// Filename: CourtTimeslotController.cs
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;
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
       
        public CourtTimeslot GetCourtTimeslotByTimeslotId(long timeslotId)
        {
            return GetCourtTimeslots().FirstOrDefault(ct => ct.timeslot_id == timeslotId);
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
                DataCache.ClearCache("TimeslotMotions");
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

        public IEnumerable<CourtTimeslot> GetCourtTimeslotsByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                return rep.Find("WHERE court_id = @0", courtId);
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
                t.updated_at = DateTime.Now;
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
                if (t.court_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM courts WHERE id = @0", t.court_id.Value) == 0)
                    throw new ValidationException("Invalid court ID.");
                if (t.timeslot_id.HasValue && ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM timeslots WHERE id = @0", t.timeslot_id.Value) == 0)
                    throw new ValidationException("Invalid timeslot ID.");
                if (ctx.ExecuteScalar<long>(System.Data.CommandType.Text, "SELECT COUNT(*) FROM court_timeslots WHERE court_id = @0 AND timeslot_id = @1 AND id != @2", t.court_id, t.timeslot_id, t.id) > 0)
                    throw new ValidationException("Duplicate court-timeslot association.");
            }
        }
        public  DateTime? GetLastTimeslotStart(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<DateTime?>(System.Data.CommandType.Text,
                    @"SELECT TOP 1 t.start FROM timeslots t 
              INNER JOIN court_timeslots ct ON ct.timeslot_id = t.id 
              WHERE ct.court_id = @0 AND t.deleted_at IS NULL 
              ORDER BY t.start DESC",
                    courtId);
            }
        }

        public  DateTime? GetLastHearingStart(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<DateTime?>(System.Data.CommandType.Text,
                    @"SELECT TOP 1 t.start FROM timeslots t 
              INNER JOIN court_timeslots ct ON ct.timeslot_id = t.id 
              WHERE ct.court_id = @0 AND EXISTS (SELECT 1 FROM events e INNER JOIN timeslot_events te ON e.id=te.event_id 
                    WHERE te.timeslot_id = t.id AND te.deleted_at IS NULL)
                    AND t.deleted_at IS NULL
              ORDER BY t.start DESC",
                    courtId);
            }
        }

        public  Timeslot GetLastTemplateTimeslot(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text,
                    @"SELECT TOP 1 t.* FROM timeslots t 
              INNER JOIN court_timeslots ct ON ct.timeslot_id = t.id 
              WHERE ct.court_id = @0 AND t.template_id IS NOT NULL AND t.deleted_at IS NULL 
              ORDER BY t.start DESC",
                    courtId).FirstOrDefault();
            }
        }
    }
    //select top 1 [start] from [timeslots] inner join [court_timeslots] on
    //[court_timeslots].[timeslot_id] = [timeslots].[id] where [court_timeslots].[court_id] = @P1 and
    //[timeslots].[deleted_at] is null order by [start] desc
}