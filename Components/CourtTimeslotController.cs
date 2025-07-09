using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtTimeslotController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateCourtTimeslot(CourtTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtTimeslot(int courttimeslotId)
        {
            var t = GetCourtTimeslot(courttimeslotId);
            DeleteCourtTimeslot(t);
        }
        public void DeleteCourtTimeslot(CourtTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtTimeslot> GetCourtTimeslots()
        {
            IEnumerable<CourtTimeslot> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                t = rep.Get();
            }
            return t;
        }
        public CourtTimeslot GetCourtTimeslot(int courttimeslotId)
        {
            CourtTimeslot t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                t = rep.GetById(courttimeslotId);
            }
            return t;
        }
        
        public void UpdateCourtTimeslot(CourtTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTimeslot>();
                rep.Update(t);
            }
        }
    }
}