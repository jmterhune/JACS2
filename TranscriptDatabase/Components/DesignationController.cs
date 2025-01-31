using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class DesignationController
    {
        public void CreateDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Insert(t);
            }
        }
        public void DeleteDesignation(int designationId)
        {
            var t = GetDesignation(designationId);
            DeleteDesignation(t);
        }
        public void DeleteDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Designation> GetDesignations()
        {
            IEnumerable<Designation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t = rep.Get();
            }
            return t;
        }
        public Designation GetDesignation(int designationId)
        {
            Designation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t = rep.GetById(designationId);
            }
            return t;
        }
        public void UpdateDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Update(t);
            }
        }
    }
}
