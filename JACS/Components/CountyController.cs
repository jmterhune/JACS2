using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CountyController
    {
        public void CreateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance())
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
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<County>();
                rep.Delete(t);
            }
        }
        public IEnumerable<County> GetCountys()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t;
        }
        public County GetCounty(long countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
            }
            return t;
        }
        public void UpdateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<County>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
    }
}