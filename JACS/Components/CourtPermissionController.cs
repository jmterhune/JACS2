using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtPermissionController
    {
        public void CreateCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtPermission>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtPermission(int courtpermissionId)
        {
            var t = GetCourtPermission(courtpermissionId);
            DeleteCourtPermission(t);
        }
        public void DeleteCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtPermission>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtPermission> GetCourtPermissions()
        {
            IEnumerable<CourtPermission> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t = rep.Get();
            }
            return t;
        }
        public CourtPermission GetCourtPermission(int courtpermissionId)
        {
            CourtPermission t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t = rep.GetById(courtpermissionId);
            }
            return t;
        }
        public void UpdateCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtPermission>();
                rep.Update(t);
            }
        }
    }
}