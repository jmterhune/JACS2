using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class TimeSpentController
    {
        private readonly IHostSettings _hostSettings;

        public TimeSpentController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateTimeSpent(TimeSpentInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteTimeSpent(int timeSpanId)
        {
            var t = GetTimeSpent(timeSpanId);
            if (t != null) DeleteTimeSpent(t);
        }

        public void DeleteTimeSpent(TimeSpentInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<TimeSpentInfo> GetTimeSpents()
        {
            IEnumerable<TimeSpentInfo> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<TimeSpentInfo> GetActiveTimeSpents()
        {
            IEnumerable<TimeSpentInfo> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                t = rep.Find("WHERE IsActive = 1");
            }
            return t;
        }

        public TimeSpentInfo GetTimeSpent(int timeSpanId)
        {
            TimeSpentInfo t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                t = rep.GetById(timeSpanId);
            }
            return t;
        }

        public void UpdateTimeSpent(TimeSpentInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<TimeSpentInfo>();
                rep.Update(t);
            }
        }
    }
}
