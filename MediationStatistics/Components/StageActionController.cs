using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class StageActionController
    {
        public void CreateStageAction(StageAction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StageAction>();
                rep.Insert(t);
            }
        }

        public void DeleteStageAction(int stageActionId)
        {
            var t = GetStageAction(stageActionId);
            DeleteStageAction(t);
        }

        public void DeleteStageAction(StageAction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StageAction>();
                rep.Delete(t);
            }
        }

        public IEnumerable<StageAction> GetStageActions()
        {
            IEnumerable<StageAction> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StageAction>();
                t = rep.Get();
            }
            return t;
        }

        public StageAction GetStageAction(int stageActionId)
        {
            StageAction t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StageAction>();
                t = rep.GetById(stageActionId);
            }
            return t;
        }

        public void UpdateStageAction(StageAction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StageAction>();
                rep.Update(t);
            }
        }

    }
}
