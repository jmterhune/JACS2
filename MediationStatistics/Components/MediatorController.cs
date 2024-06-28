using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class MediatorController
    {
        public void CreateMediator(Mediator t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Mediator>();
                rep.Insert(t);
            }
        }

        public void DeleteMediator(int mediatorId)
        {
            var t = GetMediator(mediatorId);
            DeleteMediator(t);
        }

        public void DeleteMediator(Mediator t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Mediator>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Mediator> GetMediators()
        {
            IEnumerable<Mediator> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Mediator>();
                t = rep.Get();
            }
            return t;
        }

        public Mediator GetMediator(int mediatorId)
        {
            Mediator t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Mediator>();
                t = rep.GetById(mediatorId);
            }
            return t;
        }

        public void UpdateMediator(Mediator t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Mediator>();
                rep.Update(t);
            }
        }

    }
}
