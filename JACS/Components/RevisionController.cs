using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class RevisionController
    {
        public void CreateRevision(Revision t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Revision>();
                rep.Insert(t);
            }
        }
        public void DeleteRevision(int revisionId)
        {
            var t = GetRevision(revisionId);
            DeleteRevision(t);
        }
        public void DeleteRevision(Revision t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Revision>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Revision> GetRevisions()
        {
            IEnumerable<Revision> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Revision>();
                t = rep.Get();
            }
            return t;
        }
        public Revision GetRevision(int revisionId)
        {
            Revision t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Revision>();
                t = rep.GetById(revisionId);
            }
            return t;
        }
        public void UpdateRevision(Revision t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Revision>();
                rep.Update(t);
            }
        }
    }
}