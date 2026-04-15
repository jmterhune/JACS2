using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class RequestorController
    {
        public void CreateRequestor(RequestorInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteRequestor(int requestorId)
        {
            var t = GetRequestor(requestorId);
            if (t != null) DeleteRequestor(t);
        }

        public void DeleteRequestor(RequestorInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<RequestorInfo> GetRequestors()
        {
            IEnumerable<RequestorInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<RequestorInfo> GetActiveRequestors()
        {
            IEnumerable<RequestorInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                t = rep.Find("WHERE IsActive = 1");
            }
            return t;
        }

        public RequestorInfo GetRequestor(int requestorId)
        {
            RequestorInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                t = rep.GetById(requestorId);
            }
            return t;
        }

        public void UpdateRequestor(RequestorInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestorInfo>();
                rep.Update(t);
            }
        }
    }
}
