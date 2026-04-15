using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class PhaseController
    {
        public void CreatePhase(PhaseInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhaseInfo>();
                rep.Insert(t);
            }
        }

        public void DeletePhase(int phaseId)
        {
            var t = GetPhase(phaseId);
            if (t != null) DeletePhase(t);
        }

        public void DeletePhase(PhaseInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhaseInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<PhaseInfo> GetPhases()
        {
            IEnumerable<PhaseInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhaseInfo>();
                t = rep.Get();
            }
            return t;
        }

        public PhaseInfo GetPhase(int phaseId)
        {
            PhaseInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhaseInfo>();
                t = rep.GetById(phaseId);
            }
            return t;
        }

        public void UpdatePhase(PhaseInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhaseInfo>();
                rep.Update(t);
            }
        }
    }
}
