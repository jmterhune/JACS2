using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtTypeController
    {
        public void CreateCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtType>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtType(int courttypeId)
        {
            var t = GetCourtType(courttypeId);
            DeleteCourtType(t);
        }
        public void DeleteCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtType>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtType> GetCourtTypes()
        {
            IEnumerable<CourtType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtType>();
                t = rep.Get();
            }
            return t;
        }
        public CourtType GetCourtType(int courttypeId)
        {
            CourtType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtType>();
                t = rep.GetById(courttypeId);
            }
            return t;
        }
        public void UpdateCourtType(CourtType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtType>();
                rep.Update(t);
            }
        }
    }
}