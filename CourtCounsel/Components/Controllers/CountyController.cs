using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class CountyController
    {
        public void CreateCounty(CountyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CountyInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteCounty(int countyId)
        {
            var t = GetCounty(countyId);
            if (t != null) DeleteCounty(t);
        }

        public void DeleteCounty(CountyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CountyInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CountyInfo> GetCounties()
        {
            IEnumerable<CountyInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CountyInfo>();
                t = rep.Get();
            }
            return t;
        }

        public CountyInfo GetCounty(int countyId)
        {
            CountyInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CountyInfo>();
                t = rep.GetById(countyId);
            }
            return t;
        }

        public void UpdateCounty(CountyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CountyInfo>();
                rep.Update(t);
            }
        }
    }
}
