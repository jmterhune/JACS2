using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class MediatorController
    {
        private readonly IHostSettings _hostSettings;

        public MediatorController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        private IDataContext GetContext()
        {
            return DataContext.Instance(_hostSettings);
        }

        public void CreateMediator(Mediator t)
        {
            using (IDataContext ctx = GetContext())
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
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Mediator>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Mediator> GetMediators()
        {
            IEnumerable<Mediator> t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Mediator>();
                t = rep.Get();
            }
            return t;
        }

        public Mediator GetMediator(int mediatorId)
        {
            Mediator t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Mediator>();
                t = rep.GetById(mediatorId);
            }
            return t;
        }

        public void UpdateMediator(Mediator t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Mediator>();
                rep.Update(t);
            }
        }

    }
}
