using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class AttorneyController
    {
        public void CreateAttorney(AttorneyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteAttorney(int attorneyId)
        {
            var t = GetAttorney(attorneyId);
            if (t != null) DeleteAttorney(t);
        }

        public void DeleteAttorney(AttorneyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<AttorneyInfo> GetAttorneys()
        {
            IEnumerable<AttorneyInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<AttorneyInfo> GetActiveAttorneys()
        {
            IEnumerable<AttorneyInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                t = rep.Find("WHERE IsActive = 1");
            }
            return t;
        }

        public AttorneyInfo GetAttorney(int attorneyId)
        {
            AttorneyInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }

        public void UpdateAttorney(AttorneyInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyInfo>();
                rep.Update(t);
            }
        }
    }
}
