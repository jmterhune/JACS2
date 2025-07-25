using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
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
        public IEnumerable<CourtMotion> GetCourtMotions(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtMotion>();
                return rep.Find("Where court_id=@0");
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

        internal CourtMotion GetCourtMotionByCourtAndMotion(long courtId, long motionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"SELECT * FROM court_motions WHERE court_id = @0 AND motion_id = @1";
                return ctx.ExecuteSingleOrDefault<CourtMotion>(System.Data.CommandType.Text, query, courtId, motionId);
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

        public List<KeyValuePair<long, string>> GetCourtMotionDropDownItems(long courtId, bool allowed)
        {
            try
            {
                // Validate input
                if (courtId <= 0)
                {
                    throw new ArgumentException("Court ID must be greater than zero.", nameof(courtId));
                }

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var query = @"
                SELECT m.*
                FROM court_motions cm
                INNER JOIN motions m ON cm.motion_id = m.id
                WHERE cm.court_id = @0 AND cm.allowed = @1";

                    IEnumerable<Motion> results = ctx.ExecuteQuery<Motion>(System.Data.CommandType.Text, query, courtId, allowed);
                    if (results.Count() > 0)
                    {
                        return results.Select(m => new KeyValuePair<long, string>(m.id, m.description)).ToList();
                    }
                    return new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception using DNN's exception logging
                Exceptions.LogException(ex);
                // Return an empty list to prevent downstream errors
                return new List<KeyValuePair<long, string>>();
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

        public List<KeyValuePair<long, string>> GetAvailableMotionDropDownItems(long courtId, List<long> excludedIds)
        {
            try
            {
                // Validate input  
                if (courtId <= 0)
                {
                    throw new ArgumentException("Court ID must be greater than zero.", nameof(courtId));
                }

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var query = @"  
                        SELECT m.*
                        FROM court_motions cm  
                        INNER JOIN motions m ON cm.motion_id = m.id  
                        WHERE cm.court_id = @0 AND cm.allowed = 1";

                    // Use parameterized query to prevent SQL injection  
                    var parameters = new List<object> { courtId };
                    if (excludedIds != null && excludedIds.Any())
                    {
                        query += " AND m.id NOT IN (" + string.Join(",", Enumerable.Range(0, excludedIds.Count).Select(i => "@" + (i + 1))) + ")";
                        parameters.AddRange(excludedIds.Cast<object>()); // Fix: Cast excludedIds to IEnumerable<object>  
                    }
                    IEnumerable<Motion> results = ctx.ExecuteQuery<Motion>(System.Data.CommandType.Text, query, parameters.ToArray());
                    if (results == null || !results.Any())
                        return new List<KeyValuePair<long, string>>();
                    return results.Select(m => new KeyValuePair<long, string>(m.id, m.description)).ToList();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
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