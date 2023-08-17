using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class RegionController
    {
        public void CreateRegion(Region t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Region>();
                rep.Insert(t);
            }
        }

        public void DeleteRegion(int regionId)
        {
            var t = GetRegion(regionId);
            DeleteRegion(t);
        }

        public void DeleteRegion(Region t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Region>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Region> GetRegions()
        {
            IEnumerable<Region> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Region>();
                t = rep.Get();
            }
            return t;
        }

        public Region GetRegion(int regionId)
        {
            Region t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Region>();
                t = rep.GetById(regionId);
            }
            return t;
        }

        public void UpdateRegion(Region t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Region>();
                rep.Update(t);
            }
        }

    }
}
