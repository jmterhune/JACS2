using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtTemplateController
    {
        public void CreateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtTemplate(int courttemplateId)
        {
            var t = GetCourtTemplate(courttemplateId);
            DeleteCourtTemplate(t);
        }
        public void DeleteCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtTemplate> GetCourtTemplates()
        {
            IEnumerable<CourtTemplate> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t = rep.Get();
            }
            return t;
        }
        public CourtTemplate GetCourtTemplate(int courttemplateId)
        {
            CourtTemplate t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t = rep.GetById(courttemplateId);
            }
            return t;
        }
        public void UpdateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                rep.Update(t);
            }
        }
    }
}