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

        public void DeleteSupplyOrder(int orderId)
        {
            var t = GetSupplyOrder(orderId);
            DeleteSupplyOrder(t);
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

        public void UpdateSupplyOrder(SupplyOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrder>();
                rep.Update(t);
            }
        }

        
    }
}
