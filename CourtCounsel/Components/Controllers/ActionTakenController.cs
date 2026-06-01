using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class ActionTakenController
    {
        public void CreateAction(ActionTakenInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
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
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ActionTakenInfo> GetActions()
        {
            IEnumerable<ActionTakenInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                t = rep.Get();
            }
            return t;
        }

        public ActionTakenInfo GetAction(int actionId)
        {
            ActionTakenInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                t = rep.GetById(actionId);
            }
            return t;
        }

        public void UpdateAction(ActionTakenInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ActionTakenInfo>();
                rep.Update(t);
            }
        }
    }
}
