using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationCaseEventPaymentController
    {
        public void CreateMediationCaseEventPayment(MediationCaseEventPayment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCaseEventPayment>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationCaseEventPayment(int mediationcaseeventpaymentId)
        {
            var t = GetMediationCaseEventPayment(mediationcaseeventpaymentId);
            DeleteMediationCaseEventPayment(t);
        }
        public void DeleteMediationCaseEventPayment(MediationCaseEventPayment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCaseEventPayment>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationCaseEventPayment> GetMediationCaseEventPayments()
        {
            IEnumerable<MediationCaseEventPayment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCaseEventPayment>();
                t = rep.Get();
            }
            return t;
        }
        public MediationCaseEventPayment GetMediationCaseEventPayment(int mediationcaseeventpaymentId)
        {
            MediationCaseEventPayment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCaseEventPayment>();
                t = rep.GetById(mediationcaseeventpaymentId);
            }
            return t;
        }
        public void UpdateMediationCaseEventPayment(MediationCaseEventPayment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationCaseEventPayment>();
                rep.Update(t);
            }
        }
    }
}