using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class CourtTemplateOrderController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateCourtTemplateOrder(CourtTemplateOrder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Insert(t);
            }
        }
        public void DeleteCourtTemplateOrder(int courttemplateorderId)
        {
            var t = GetCourtTemplateOrder(courttemplateorderId);
            DeleteCourtTemplateOrder(t);
        }
        public void DeleteCourtTemplateOrder(CourtTemplateOrder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Delete(t);
            }
        }
        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrders()
        {
            IEnumerable<CourtTemplateOrder> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                t = rep.Get();
            }
            return t;
        }
        public CourtTemplateOrder GetCourtTemplateOrder(int courttemplateorderId)
        {
            CourtTemplateOrder t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                t = rep.GetById(courttemplateorderId);
            }
            return t;
        }
        public void UpdateCourtTemplateOrder(CourtTemplateOrder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Update(t);
            }
        }
    }
}