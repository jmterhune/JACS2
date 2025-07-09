using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class FailedJobController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateFailedJob(FailedJob t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<FailedJob>();
                rep.Insert(t);
            }
        }
        public void DeleteFailedJob(int failedjobId)
        {
            var t = GetFailedJob(failedjobId);
            DeleteFailedJob(t);
        }
        public void DeleteFailedJob(FailedJob t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<FailedJob>();
                rep.Delete(t);
            }
        }
        public IEnumerable<FailedJob> GetFailedJobs()
        {
            IEnumerable<FailedJob> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<FailedJob>();
                t = rep.Get();
            }
            return t;
        }
        public FailedJob GetFailedJob(int failedjobId)
        {
            FailedJob t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<FailedJob>();
                t = rep.GetById(failedjobId);
            }
            return t;
        }
        public void UpdateFailedJob(FailedJob t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<FailedJob>();
                rep.Update(t);
            }
        }
    }
}
