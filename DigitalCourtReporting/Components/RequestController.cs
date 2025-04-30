using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class RequestController
    {
        public void CreateRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Insert(t);
            }
        }
        public void DeleteRequest(int requestId)
        {
            var t = GetRequest(requestId);
            DeleteRequest(t);
        }
        public void DeleteRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Request> GetRequests()
        {
            IEnumerable<Request> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.Get();
            }
            return t;
        }
        public Request GetRequest(int requestId)
        {
            Request t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.GetById(requestId);
            }
            return t;
        }
        public void UpdateRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Update(t);
            }
        }
    }
}
