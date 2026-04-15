using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class CaseTypeController
    {
        public void CreateCaseType(CaseTypeInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteCaseType(int caseTypeId)
        {
            var t = GetCaseType(caseTypeId);
            if (t != null) DeleteCaseType(t);
        }

        public void DeleteCaseType(CaseTypeInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CaseTypeInfo> GetCaseTypes()
        {
            IEnumerable<CaseTypeInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeInfo>();
                t = rep.Get();
            }
            return t;
        }

        public CaseTypeInfo GetCaseType(int caseTypeId)
        {
            CaseTypeInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeInfo>();
                t = rep.GetById(caseTypeId);
            }
            return t;
        }

        public void UpdateCaseType(CaseTypeInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeInfo>();
                rep.Update(t);
            }
        }
    }
}
