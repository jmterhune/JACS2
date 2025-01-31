using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.Purchasing.Components
{
    internal class FormOrderController
    {
        #region Form Orders

        public void CreateFormOrder(FormOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                rep.Insert(t);
            }
        }

        public void DeleteFormOrder(int orderId)
        {
            var t = GetFormOrder(orderId);
            DeleteFormOrder(t);
        }
        public void DeleteFormOrder(int moduleId, int orderId)
        {
            var t = GetFormOrder(orderId);
            DeleteAllFormOrderItems(moduleId, orderId);
            DeleteFormOrder(t);
        }
        public void DeleteFormOrder(FormOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                rep.Delete(t);
            }
        }

        public IEnumerable<FormOrder> GetFormOrders()
        {
            IEnumerable<FormOrder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<FormOrder> GetFormOrders(DateTime startDate, DateTime endDate)
        {
            IEnumerable<FormOrder> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                t = rep.Find("Where DateRequested Between @0 And @1", startDate, endDate);
            }
            return t;
        }

        public FormOrder GetFormOrder(int orderId)
        {
            FormOrder t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                t = rep.GetById(orderId);
            }
            return t;
        }

        public void UpdateFormOrder(FormOrder t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrder>();
                rep.Update(t);
            }
        }
        #endregion

        #region Form Order Items
        public void CreateFormOrderItem(FormOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                rep.Insert(t);
            }
        }

        public void DeleteFormOrderItem(int formId)
        {
            var t = GetFormOrderItem(formId);
            DeleteFormOrderItem(t);
        }
        public void DeleteAllFormOrderItems(int moduleId, int orderId)
        {
            var ctl = new AttachmentController();
            ctl.DeleteFormAttachmentByOrderId(orderId);
            var f = GetFormOrderItemsByOrder(orderId);
            foreach (FormOrderItem fi in f)
            {
                DeleteFormOrderItem(fi);
            }
        }
        public void DeleteFormOrderItem(FormOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                rep.Delete(t);
            }
        }

        public IEnumerable<FormOrderItem> GetFormOrderItems()
        {
            IEnumerable<FormOrderItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<FormOrderItem> GetFormOrderItemsByOrder(int orderId)
        {
            IEnumerable<FormOrderItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }

        public FormOrderItem GetFormOrderItem(int orderId)
        {
            FormOrderItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                t = rep.GetById(orderId);
            }
            return t;
        }

        public void UpdateFormOrderItem(FormOrderItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderItem>();
                rep.Update(t);
            }
        }
        #endregion
    }
}
