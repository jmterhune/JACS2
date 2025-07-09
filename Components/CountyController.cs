using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CountyController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteCounty(long countyId)
        {
            var t = GetCounty(countyId);
            DeleteCounty(t);
        }
        public void DeleteCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                rep.Delete(t);
            }
        }
        public IEnumerable<County> GetCountys()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t;
        }
        public County GetCounty(long countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
            }
            return t;
        }
        public void UpdateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<County> GetCountiesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<County>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_county_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetCountiesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_county_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}