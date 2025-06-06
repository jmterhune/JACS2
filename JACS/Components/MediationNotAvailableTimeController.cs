using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationNotAvailableTimeController
    {
        public void CreateMediationNotAvailableTime(MediationNotAvailableTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationNotAvailableTime>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationNotAvailableTime(int mediationnotavailabletimeId)
        {
            var t = GetMediationNotAvailableTime(mediationnotavailabletimeId);
            DeleteMediationNotAvailableTime(t);
        }
        public void DeleteMediationNotAvailableTime(MediationNotAvailableTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationNotAvailableTime>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationNotAvailableTime> GetMediationNotAvailableTimes()
        {
            IEnumerable<MediationNotAvailableTime> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationNotAvailableTime>();
                t = rep.Get();
            }
            return t;
        }
        public MediationNotAvailableTime GetMediationNotAvailableTime(int mediationnotavailabletimeId)
        {
            MediationNotAvailableTime t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationNotAvailableTime>();
                t = rep.GetById(mediationnotavailabletimeId);
            }
            return t;
        }
        public void UpdateMediationNotAvailableTime(MediationNotAvailableTime t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationNotAvailableTime>();
                rep.Update(t);
            }
        }
    }
}