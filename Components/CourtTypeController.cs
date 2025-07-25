using DotNetNuke.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class CourtTypeController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteCourtType(long courtTypeId)
        {
            var t = GetCourtType(courtTypeId);
            DeleteCourtType(t);
        }

        public void DeleteCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtType> GetCourtTypes()
        {
            IEnumerable<CourtType> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                t = rep.Get();
            }
            return t;
        }

        public List<KeyValuePair<long,string>> GetCourtTypeDropDownItems()
        {
            IEnumerable<CourtType> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                t = rep.Get();
            }
            return t.Select(ct=>new KeyValuePair<long, string>(ct.id,ct.code)).ToList();
        }

        public CourtType GetCourtType(long courtTypeId)
        {
            CourtType t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                t = rep.GetById(courtTypeId);
            }
            return t;
        }

        public void UpdateCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtType>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<CourtType> GetCourtTypesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtType>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_type_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetCourtTypesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_type_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}