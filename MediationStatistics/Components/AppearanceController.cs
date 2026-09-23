using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class AppearanceController
    {
        private readonly IHostSettings _hostSettings;

        public AppearanceController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        private IDataContext GetContext()
        {
            return DataContext.Instance(_hostSettings);
        }

        public void CreateAppearance(Appearance t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Appearance>();
                rep.Insert(t);
            }
        }

        public void DeleteAppearance(int appearanceId)
        {
            var t = GetAppearance(appearanceId);
            DeleteAppearance(t);
        }

        public void DeleteAppearance(Appearance t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Appearance>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Appearance> GetAppearances()
        {
            IEnumerable<Appearance> t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Appearance>();
                t = rep.Get();
            }
            return t;
        }

        public Appearance GetAppearance(int appearanceId)
        {
            Appearance t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Appearance>();
                t = rep.GetById(appearanceId);
            }
            return t;
        }

        public void UpdateAppearance(Appearance t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Appearance>();
                rep.Update(t);
            }
        }
        public IEnumerable<Appearance> GetEventAppearances(int eventId)
        {
            IEnumerable<Appearance> t;
            using (IDataContext ctx = GetContext())
            {

                t = ctx.ExecuteQuery<Appearance>(System.Data.CommandType.StoredProcedure, "tjc_med_get_event_appearances",eventId);
            }
            return t;
        }

    }
}
