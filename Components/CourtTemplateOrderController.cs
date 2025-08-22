using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.jacs.Components
{
    internal class CourtTemplateOrderController
    {
        private const string CONN_JACS = "jacs"; // Connection

        public void CreateCourtTemplateOrder(CourtTemplateOrder t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Insert(t);
            }
        }

        public void DeleteCourtTemplateOrder(long courttemplateorderId)
        {
            var t = GetCourtTemplateOrder(courttemplateorderId);
            DeleteCourtTemplateOrder(t);
        }
        public void DeleteCourtTemplateOrdersByCourtId(long courtId, bool auto = false)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Delete($"WHERE court_id = @0 AND auto = @1", courtId, auto ? 1 : 0);
            }
        }
        public void DeleteCourtTemplateOrder(CourtTemplateOrder t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrders()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                return rep.Get();
            }
        }
        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrdersByTemplateId(long templateId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                return rep.Find("Where template_id = @0",templateId);
            }
        }
        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrdersByCourtId(long courtId,bool auto)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                string query = "Where court_id = @0";
                if(!auto)
                {
                    query += " AND date >= @1";
                }
                return rep.Find(query, courtId, Common.GetMondayOfCurrentWeek(DateTime.Now).AddDays(7)).OrderBy(to=>to.date);
            }
        }
        public CourtTemplateOrder GetCourtTemplateOrder(long courttemplateorderId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                return rep.GetById(courttemplateorderId);
            }
        }

        public void UpdateCourtTemplateOrder(CourtTemplateOrder t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                rep.Update(t);
            }
        }

        public int GetCourtTemplateOrdersCount(long courtId, string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_order_count",
                    searchTerm,
                    courtId);
            }
        }

        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrdersPaged(long courtId, string searchTerm, int recordOffset, int pageSize, string sortColumn, string sortDirection)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtTemplateOrder>(System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_order_paged",
                    searchTerm,
                    courtId,
                    recordOffset,
                    pageSize,
                    sortColumn,
                    sortDirection);
            }
        }
    }
}