using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.Purchasing.Components
{
    internal class SupplyOrderController
    {
        public void CreateSupplyOrder(SupplyOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                rep.Insert(t);
            }
        }
        public void CreateSupplyOrderItem(SupplyOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderItem>();
                rep.Insert(t);
            }
        }
        public void DeleteSupplyOrder(int orderId)
        {
            var t = GetSupplyOrder(orderId);
            DeleteSupplyOrder(t);
        }
        public void DeleteSupplyOrderItem(int supplyId)
        {
            var t = GetSupplyOrderItem(supplyId);
            DeleteSupplyOrderItem(t);
        }

        public void DeleteSupplyOrderItem(SupplyOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderItem>();
                rep.Delete(t);
            }
        }
        public void DeleteSupplyOrder(SupplyOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                rep.Delete(t);
            }
        }

        public IEnumerable<SupplyOrder> GetSupplyOrders()
        {
            IEnumerable<SupplyOrder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                t = rep.Get();
            }
            return t;
        }

        public SupplyOrder GetSupplyOrder(int orderId)
        {
            SupplyOrder t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                t = rep.GetById(orderId);
            }
            return t;
        }
        public SupplyOrderItem GetSupplyOrderItem(int supplyId)
        {
            SupplyOrderItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderItem>();
                t = rep.GetById(supplyId);
            }
            return t;
        }
        public IEnumerable<SupplyOrderItem> GetSupplyOrderItemsByOrder(int orderId)
        {
            IEnumerable<SupplyOrderItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderItem>();
                t = rep.Find("Where OrderId = @0",orderId);
            }
            return t;
        }

        public void UpdateSupplyOrder(SupplyOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                rep.Update(t);
            }
        }

        public void UpdateSupplyOrderItem(SupplyOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderItem>();
                rep.Update(t);
            }
        }


    }
}
