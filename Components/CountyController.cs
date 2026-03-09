using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
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
                if (!string.IsNullOrWhiteSpace(t.password))
                {
                    t.password = EncryptionHelper.Encrypt(t.password);
                }
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
                foreach (var county in t)
                {
                    if (!string.IsNullOrWhiteSpace(county.password))
                    {
                        county.password = EncryptionHelper.Decrypt(county.password);
                    }
                }
            }
            return t;
        }
        public List<KeyValuePair<long,string>> GetCountyDropDownItems()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t.Select(c=>new KeyValuePair<long, string>(c.id,c.name)).OrderBy(c=>c.Value).ToList();
        }
        public List<KeyValuePair<string, string>> GetCountyCodeDropDownItems()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t.Select(c => new KeyValuePair<string, string>(c.code, c.name)).OrderBy(c => c.Value).ToList();
        }
        public County GetCounty(long countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
                if (t != null && !string.IsNullOrWhiteSpace(t.password))
                {
                    t.password = EncryptionHelper.Decrypt(t.password);
                }
            }
            return t;
        }
        public void UpdateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t.updated_at = System.DateTime.Now;
                if (!string.IsNullOrWhiteSpace(t.password))
                {
                    t.password = EncryptionHelper.Encrypt(t.password);
                }
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