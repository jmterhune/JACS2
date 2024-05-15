using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.Purchasing.Components
{
    internal class StampOrderController
    {
        public void CreateStampOrder(StampOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                rep.Insert(t);
            }
        }

        public void DeleteStampOrder(int orderId)
        {
            var t = GetStampOrder(orderId);
            DeleteStampOrder(t);
        }

        public void DeleteStampOrder(StampOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                rep.Delete(t);
            }
        }

        public IEnumerable<StampOrder> GetStampOrders()
        {
            IEnumerable<StampOrder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                t = rep.Get();
            }
            return t;
        }

        public StampOrder GetStampOrder(int orderId)
        {
            StampOrder t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                t = rep.GetById(orderId);
            }
            return t;
        }

        public void UpdateStampOrder(StampOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                rep.Update(t);
            }
        }

        public IEnumerable<StampOrder> GetOrders(DateTime startDate, DateTime endDate)
        {
            IEnumerable<StampOrder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrder>();
                t = rep.Find("Where DateCreated Between @0 And @1", startDate,endDate);
            }
            return t;
        }
    }
}
