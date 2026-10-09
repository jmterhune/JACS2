using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class ActionTakenController
    {
        private readonly IHostSettings _hostSettings;

        public ActionTakenController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateAction(ActionTakenInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteAction(int actionId)
        {
            var t = GetAction(actionId);
            if (t != null) DeleteAction(t);
        }

        public void DeleteAction(ActionTakenInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ActionTakenInfo> GetActions()
        {
            IEnumerable<ActionTakenInfo> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                t = rep.Get();
            }
            return t;
        }

        public ActionTakenInfo GetAction(int actionId)
        {
            ActionTakenInfo t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                t = rep.GetById(actionId);
            }
            return t;
        }

        public void UpdateAction(ActionTakenInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                rep.Update(t);
            }
        }
    }
}
