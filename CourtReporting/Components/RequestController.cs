using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtReporting.Components
{
   internal class RequestController
    {
        public Request CreateRequest(RequestInfo ri)
        {
            Request r = new Request { Address = ri.Address, Jurisdiction = ri.Jurisdiction, TranscriptionList = ri.TranscriptionList, DeliveryMethod = ri.DeliveryMethod, Location = ri.Location, CA = ri.CA, CaseName = ri.CaseName, CaseNumber = ri.CaseNumber, City = ri.City, Email = ri.Email, Fax = ri.Fax, Instructions = ri.Instructions, IsInquiry = ri.IsInquiry, Involvement = ri.Involvement, Judge = ri.Judge, LawFirm = ri.LawFirm, OrderStatus = ri.OrderStatus, PaymentRequired = ri.PaymentRequired, Phone = ri.Phone, PaymentType = ri.PaymentType, FirstName = ri.FirstName, LastName = ri.LastName, RequestedDate = ri.RequestedDate, State = ri.State, Zip = ri.Zip, UserId = ri.UserId, Guid = Guid.NewGuid(), TotalAmount = ri.TotalAmount };
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Insert(r);
            }
            return r;
        }

        public void DeleteRequest(int requestId)
        {
            var r = GetRequest(requestId);

            DeleteRequest(r);
        }

        public void DeleteRequest(Request r)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Delete(r);
            }
        }
        public Request GetRequest(int requestId)
        {
            Request r;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                r = rep.GetById(requestId);
            }
            return r;
        }
        public IEnumerable<Request> GetRequests()
        {
            IEnumerable<Request> r;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                r = rep.Get();
            }
            return r;
        }

        public Request GetRequestByOrderReference(Guid orderReference)
        {
            Request r;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                // r = rep.Get().AsQueryable().Where(x => x.Guid == orderReference).FirstOrDefault();
                r = rep.Find("Where Guid=@0", orderReference).FirstOrDefault();
            }
            return r;
        }
        public void UpdateRequest(Request r)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Update(r);
            }
        }

    }
}