using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.CourtReporting.Components
{
   internal class ProceedingController
    {
        private readonly IHostSettings _hostSettings;

        public ProceedingController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public Proceeding CreateProceeding(ProceedingInfo pi)
        {
            Proceeding p = new Proceeding { MediaTypeID = pi.MediaTypeID, Price = pi.Price, ProceedingDate = pi.ProceedingDate, ProceedingTime = pi.ProceedingTime, ProceedingType = pi.ProceedingType, RequestID = pi.RequestID };
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
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
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Proceeding>();
                rep.Delete(p);
            }
        }
        public Proceeding GetProceeding(int proceedingId)
        {
            Proceeding p;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.GetById(proceedingId);
            }
            return p;
        }
        public IEnumerable<Proceeding> GetProceedings()
        {
            IEnumerable<Proceeding> p;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.Get();
            }
            return p;
        }
        public IEnumerable<Proceeding> GetProceedingsByRequest(int requestId)
        {
            IEnumerable<Proceeding> p;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Proceeding>();
                p = rep.Find("Where RequestID=@0", requestId);
            }
            return p;
        }
    }
}