using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class OfficeController
    {
        public void CreateOffice(Office t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Office>();
                rep.Insert(t);
            }
        }
        public void DeleteOffice(int officeId)
        {
            var t = GetOffice(officeId);
            DeleteOffice(t);
        }
        public void DeleteOffice(Office t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Office>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Office> GetOffices()
        {
            IEnumerable<Office> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Office>();
                t = rep.Get();
            }
            return t;
        }
        public Office GetOffice(int officeId)
        {
            Office t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Office>();
                t = rep.GetById(officeId);
            }
            return t;
        }
        public void UpdateOffice(Office t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Office>();
                rep.Update(t);
            }
        }
    }
}