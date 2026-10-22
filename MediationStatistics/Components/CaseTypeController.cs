using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class CaseTypeController
    {
        private readonly IHostSettings _hostSettings;

        public CaseTypeController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Insert(t);
            }
        }

        public void DeleteCaseType(int caseTypeId)
        {
            var t = GetCaseType(caseTypeId);
            DeleteCaseType(t);
        }

        public void DeleteCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CaseType> GetCaseTypes()
        {
            IEnumerable<CaseType> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.Get();
            }
            return t;
        }

        public CaseType GetCaseType(int caseTypeId)
        {
            CaseType t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.GetById(caseTypeId);
            }
            return t;
        }

        public void UpdateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Update(t);
            }
        }

    }
}
