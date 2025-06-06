using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtMotionController
    {
        public void CreateCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtMotion(int courtmotionId)
        {
            var t = GetCourtMotion(courtmotionId);
            DeleteCourtMotion(t);
        }
        public void DeleteCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtMotion> GetCourtMotions()
        {
            IEnumerable<CourtMotion> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtMotion>();
                t = rep.Get();
            }
            return t;
        }
        public CourtMotion GetCourtMotion(int courtmotionId)
        {
            CourtMotion t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtMotion>();
                t = rep.GetById(courtmotionId);
            }
            return t;
        }
        public void UpdateCourtMotion(CourtMotion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtMotion>();
                rep.Update(t);
            }
        }
    }
}