using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationCaseController
    {
        public void CreateMediationCase(MediationCase t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCase>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationCase(int mediationcaseId)
        {
            var t = GetMediationCase(mediationcaseId);
            DeleteMediationCase(t);
        }
        public void DeleteMediationCase(MediationCase t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCase>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationCase> GetMediationCases()
        {
            IEnumerable<MediationCase> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCase>();
                t = rep.Get();
            }
            return t;
        }
        public MediationCase GetMediationCase(int mediationcaseId)
        {
            MediationCase t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCase>();
                t = rep.GetById(mediationcaseId);
            }
            return t;
        }
        public void UpdateMediationCase(MediationCase t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCase>();
                rep.Update(t);
            }
        }
    }
}