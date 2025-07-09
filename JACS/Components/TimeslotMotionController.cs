using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotMotionController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslotMotion(int timeslotmotionId)
        {
            var t = GetTimeslotMotion(timeslotmotionId);
            DeleteTimeslotMotion(t);
        }
        public void DeleteTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TimeslotMotion> GetTimeslotMotions()
        {
            IEnumerable<TimeslotMotion> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                t = rep.Get();
            }
            return t;
        }
        public TimeslotMotion GetTimeslotMotion(int timeslotmotionId)
        {
            TimeslotMotion t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                t = rep.GetById(timeslotmotionId);
            }
            return t;
        }
        public void UpdateTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Update(t);
            }
        }
    }
}