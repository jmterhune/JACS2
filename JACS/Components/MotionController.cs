using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MotionController
    {
        public void CreateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Motion>();
                rep.Insert(t);
            }
        }
        public void DeleteMotion(int motionId)
        {
            var t = GetMotion(motionId);
            DeleteMotion(t);
        }
        public void DeleteMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Motion>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Motion> GetMotions()
        {
            IEnumerable<Motion> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.Get();
            }
            return t;
        }
        public Motion GetMotion(int motionId)
        {
            Motion t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.GetById(motionId);
            }
            return t;
        }
        public void UpdateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Motion>();
                rep.Update(t);
            }
        }
    }
}