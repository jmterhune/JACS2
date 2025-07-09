using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class CourtMotionController
    {
        private const string CONN_JACS = "jacs";

        public void CreateCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Insert(t);
            }
        }

        public void DeleteCourtMotion(long courtMotionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                var motion = rep.GetById(courtMotionId);
                if (motion != null)
                {
                    rep.Delete(motion);
                }
            }
        }

        public void DeleteCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtMotion> GetCourtMotions()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                return rep.Get();
            }
        }

        internal CourtMotion GetCourtMotion(long courtMotionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                return rep.GetById(courtMotionId);
            }
        }

        public void UpdateCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Update(t);
            }
        }

        public IEnumerable<CourtMotion> GetCourtMotionsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtMotion>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_motion_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetCourtMotionsCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_motion_count",
                    searchTerm ?? string.Empty
                );
            }
        }

        public List<CourtListItem> GetCourtMotionsByCourtId(long courtId, bool allowed)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
            SELECT m.id as value, m.description as text
            FROM court_motions cm
            INNER JOIN motions m ON cm.motion_id = m.id
            WHERE cm.court_id = @0 AND cm.allowed = @1";
                return ctx.ExecuteQuery<CourtListItem>(System.Data.CommandType.Text, query, courtId, allowed).ToList();
            }
        }
        public List<int> GetCourtMotionValuesByCourtId(long courtId, bool allowed)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
            SELECT m.id
            FROM court_motions cm
            INNER JOIN motions m ON cm.motion_id = m.id
            WHERE cm.court_id = @0 AND cm.allowed = @1";
                return ctx.ExecuteQuery<int>(System.Data.CommandType.Text, query, courtId, allowed).ToList();
            }
        }

        public void DeleteCourtMotionsByCourtId(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                var motions = rep.Find("Where court_id=@0", courtId);
                foreach (var motion in motions)
                {
                    DeleteCourtMotion(motion);
                }
            }
        }
    }
}