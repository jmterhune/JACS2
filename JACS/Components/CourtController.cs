using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtController
    {
        public void CreateCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Court>();
                rep.Insert(t);
            }
        }
        public void DeleteCourt(int courtId)
        {
            var t = GetCourt(courtId);
            DeleteCourt(t);
        }
        public void DeleteCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Court>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Court> GetCourts()
        {
            IEnumerable<Court> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Court>();
                t = rep.Get();
            }
            return t;
        }
        public Court GetCourt(int courtId)
        {
            Court t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Court>();
                t = rep.GetById(courtId);
            }
            return t;
        }
        public void UpdateCourt(Court t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Court>();
                rep.Update(t);
            }
        }
    }
}