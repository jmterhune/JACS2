using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationOutcomeController
    {
        public void CreateMediationOutcome(MediationOutcome t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationOutcome>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationOutcome(int mediationoutcomeId)
        {
            var t = GetMediationOutcome(mediationoutcomeId);
            DeleteMediationOutcome(t);
        }
        public void DeleteMediationOutcome(MediationOutcome t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationOutcome>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationOutcome> GetMediationOutcomes()
        {
            IEnumerable<MediationOutcome> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationOutcome>();
                t = rep.Get();
            }
            return t;
        }
        public MediationOutcome GetMediationOutcome(int mediationoutcomeId)
        {
            MediationOutcome t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationOutcome>();
                t = rep.GetById(mediationoutcomeId);
            }
            return t;
        }
        public void UpdateMediationOutcome(MediationOutcome t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationOutcome>();
                rep.Update(t);
            }
        }
    }
}