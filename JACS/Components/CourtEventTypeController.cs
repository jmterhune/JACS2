using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtEventTypeController
    {
        public void CreateCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtEventType(int courteventtypeId)
        {
            var t = GetCourtEventType(courteventtypeId);
            DeleteCourtEventType(t);
        }
        public void DeleteCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtEventType> GetCourtEventTypes()
        {
            IEnumerable<CourtEventType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtEventType>();
                t = rep.Get();
            }
            return t;
        }
        public CourtEventType GetCourtEventType(int courteventtypeId)
        {
            CourtEventType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtEventType>();
                t = rep.GetById(courteventtypeId);
            }
            return t;
        }
        public void UpdateCourtEventType(CourtEventType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtEventType>();
                rep.Update(t);
            }
        }
    }
}