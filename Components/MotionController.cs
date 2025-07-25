using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.jacs.Components
{
    internal class MotionController
    {
        private const string CONN_JACS = "jacs"; // Connection
        public void CreateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteMotion(long motionId)
        {
            var t = GetMotion(motionId);
            DeleteMotion(t);
        }

        public void DeleteMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Motion> GetMotions()
        {
            IEnumerable<Motion> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetMotionDropDownItems()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Motion>();
                    var results = rep.Get()
                        .Select(m => new KeyValuePair<long, string>(m.id, m.description)).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }

        public Motion GetMotion(long motionId)
        {
            Motion t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.GetById(motionId);
            }
            return t;
        }

        public void UpdateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<Motion> GetMotionsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Motion>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_motion_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetMotionsCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_motion_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}