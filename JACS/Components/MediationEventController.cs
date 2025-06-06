using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationEventController
    {
        public void CreateMediationEvent(MediationEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationEvent>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationEvent(int mediationeventId)
        {
            var t = GetMediationEvent(mediationeventId);
            DeleteMediationEvent(t);
        }
        public void DeleteMediationEvent(MediationEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationEvent>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationEvent> GetMediationEvents()
        {
            IEnumerable<MediationEvent> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationEvent>();
                t = rep.Get();
            }
            return t;
        }
        public MediationEvent GetMediationEvent(int mediationeventId)
        {
            MediationEvent t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationEvent>();
                t = rep.GetById(mediationeventId);
            }
            return t;
        }
        public void UpdateMediationEvent(MediationEvent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationEvent>();
                rep.Update(t);
            }
        }
    }
}