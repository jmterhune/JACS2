using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotController
    {
        public void CreateTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslot(int timeslotId)
        {
            var t = GetTimeslot(timeslotId);
            DeleteTimeslot(t);
        }
        public void DeleteTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Timeslot> GetTimeslots()
        {
            IEnumerable<Timeslot> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Timeslot>();
                t = rep.Get();
            }
            return t;
        }
        public Timeslot GetTimeslot(int timeslotId)
        {
            Timeslot t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Timeslot>();
                t = rep.GetById(timeslotId);
            }
            return t;
        }
        public void UpdateTimeslot(Timeslot t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Timeslot>();
                rep.Update(t);
            }
        }
    }
}