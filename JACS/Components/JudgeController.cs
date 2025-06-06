using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class JudgeController
    {
        public void CreateJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Judge>();
                rep.Insert(t);
            }
        }
        public void DeleteJudge(int judgeId)
        {
            var t = GetJudge(judgeId);
            DeleteJudge(t);
        }
        public void DeleteJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Judge>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Judge> GetJudges()
        {
            IEnumerable<Judge> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.Get();
            }
            return t;
        }
        public Judge GetJudge(int judgeId)
        {
            Judge t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.GetById(judgeId);
            }
            return t;
        }
        public void UpdateJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Judge>();
                rep.Update(t);
            }
        }
    }
}