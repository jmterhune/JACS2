using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationTimeController
    {
        public void CreateMediationTime(MediationTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationTime>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationTime(int mediationtimeId)
        {
            var t = GetMediationTime(mediationtimeId);
            DeleteMediationTime(t);
        }
        public void DeleteMediationTime(MediationTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationTime>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationTime> GetMediationTimes()
        {
            IEnumerable<MediationTime> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationTime>();
                t = rep.Get();
            }
            return t;
        }
        public MediationTime GetMediationTime(int mediationtimeId)
        {
            MediationTime t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationTime>();
                t = rep.GetById(mediationtimeId);
            }
            return t;
        }
        public void UpdateMediationTime(MediationTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationTime>();
                rep.Update(t);
            }
        }
    }
}