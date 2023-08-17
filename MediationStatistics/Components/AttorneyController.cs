using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class AttorneyController
    {
        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Insert(t);
            }
        }

        public void DeleteAttorney(int attorneyId)
        {
            var t = GetAttorney(attorneyId);
            DeleteAttorney(t);
        }

        public void DeleteAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Attorney> GetAttorneys()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }

        public Attorney GetAttorney(int attorneyId)
        {
            Attorney t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }

        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Update(t);
            }
        }

    }
}
