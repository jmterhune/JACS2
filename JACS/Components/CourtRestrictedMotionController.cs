using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtRestrictedMotionController
    {
        public void CreateCourtRestrictedMotion(CourtRestrictedMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtRestrictedMotion>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtRestrictedMotion(int courtrestrictedmotionId)
        {
            var t = GetCourtRestrictedMotion(courtrestrictedmotionId);
            DeleteCourtRestrictedMotion(t);
        }
        public void DeleteCourtRestrictedMotion(CourtRestrictedMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtRestrictedMotion>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtRestrictedMotion> GetCourtRestrictedMotions()
        {
            IEnumerable<CourtRestrictedMotion> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtRestrictedMotion>();
                t = rep.Get();
            }
            return t;
        }
        public CourtRestrictedMotion GetCourtRestrictedMotion(int courtrestrictedmotionId)
        {
            CourtRestrictedMotion t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtRestrictedMotion>();
                t = rep.GetById(courtrestrictedmotionId);
            }
            return t;
        }
        public void UpdateCourtRestrictedMotion(CourtRestrictedMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtRestrictedMotion>();
                rep.Update(t);
            }
        }
    }
}