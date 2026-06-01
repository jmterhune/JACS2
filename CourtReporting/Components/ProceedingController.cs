using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.CourtReporting.Components
{
   internal class ProceedingController
    {
        public Proceeding CreateProceeding(ProceedingInfo pi)
        {
            Proceeding p = new Proceeding { MediaTypeID = pi.MediaTypeID, Price = pi.Price, ProceedingDate = pi.ProceedingDate, ProceedingTime = pi.ProceedingTime, ProceedingType = pi.ProceedingType, RequestID = pi.RequestID };
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                rep.Insert(p);
            }
            return p;
        }

        public void DeleteProceeding(int proceedingId)
        {
            var p = GetProceeding(proceedingId);

            DeleteProceeding(p);
        }

        public void DeleteProceeding(Proceeding p)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                rep.Delete(p);
            }
        }
        public Proceeding GetProceeding(int proceedingId)
        {
            Proceeding p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.GetById(proceedingId);
            }
            return p;
        }
        public IEnumerable<Proceeding> GetProceedings()
        {
            IEnumerable<Proceeding> p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.Get();
            }
            return p;
        }
        public IEnumerable<Proceeding> GetProceedingsByRequest(int requestId)
        {
            IEnumerable<Proceeding> p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.Find("Where RequestID=@0", requestId);
            }
            return p;
        }
    }
}