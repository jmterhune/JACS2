using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Excel;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

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
                return rep.Find("Where template_id = @0", templateId);
            }
        }
        public IEnumerable<CourtTemplateOrder> GetCourtTemplateOrdersByCourtId(long courtId, bool auto)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                string query = "WHERE court_id = @0 AND auto = @1";
                object[] parameters;
                if (!auto)
                {
                    query += " AND date >= @2";
                    parameters = new object[] { courtId, 0, Common.GetMondayOfCurrentWeek(DateTime.Now).AddDays(7) };
                }
                else
                {
                    parameters = new object[] { courtId, 1 };
                }
                var results = rep.Find(query, parameters);
                if (auto)
                {
                    return results.OrderBy(to => to.order);
                }
                else
                {
                    return results.OrderBy(to => to.date);
                }
            }
        }
        public IEnumerable<CourtTemplateOrder> GetManualTemplateOrders(long courtId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplateOrder>();
                return rep.Find("Where court_id = @0 AND auto = 0", courtId).OrderBy(to => to.date);
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
        public bool AutoExtendCalendar(long courtId, long startTemplateId, DateTime startDate, int weeks)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(CONN_JACS))
                {
                    // Validate court exists
                    var court = ctx.ExecuteSingleOrDefault<Dictionary<string, object>>(
                        CommandType.Text,
                        "SELECT id FROM courts WHERE id = @0",
                        courtId);

                    if (court == null)
                    {
                        return false;
                    }

                    // Get auto template orders
                    var lastTemplateTimeslot = ctx.ExecuteQuery<Timeslot>(
                        CommandType.Text,
                        @"SELECT TOP (1) t.* FROM timeslots AS t
                            INNER JOIN court_timeslots AS ct ON t.id = ct.timeslot_id
                            WHERE ct.court_id = @0 AND t.template_id IS NOT NULL
                            ORDER BY t.start DESC",courtId).FirstOrDefault();
                    var ctlHolidays = new HolidayController();
                    var holidays = ctlHolidays.GetHolidays();
                    // Find starting order
                    var orderedTemplates = ctx.ExecuteQuery<CourtTemplateOrder>(
                        CommandType.Text,
                        @"SELECT *
                        FROM court_template_order
                        WHERE court_id = @0 AND auto = 1
                        ORDER BY [order] ASC", courtId);

                    long startOrder = startTemplateId;

                    DateTime startWeek;

                    if (lastTemplateTimeslot != null)
                    {
                        DateTime lastDate = lastTemplateTimeslot.start;
                        if (lastDate == startDate.Date)
                        {
                            startWeek = lastTemplateTimeslot.start.AddDays(7).StartOfWeek();
                        }
                        else
                        {
                            startWeek = startDate.StartOfWeek();
                        }
                    }
                    else
                    {
                        startWeek = Common.StartOfWeek(DateTime.Now);
                    }

                    for (int x = 0; x < weeks; x++)
                    {
                        CourtTemplate currentTemplate;
                        IEnumerable<TemplateTimeslot> timeslots;

                        if (x == 0)
                        {
                            var orderItem = orderedTemplates.FirstOrDefault(o => o.order == startOrder);
                            currentTemplate = orderItem?.template;
                            timeslots = currentTemplate.template_timeslots.Count() > 0 ? currentTemplate.template_timeslots: Enumerable.Empty<TemplateTimeslot>();
                        }
                        else
                        {
                            var orderItem = orderedTemplates.FirstOrDefault(o => o.order == startOrder);
                            if (orderItem != null)
                            {
                                currentTemplate = orderItem.template;
                                timeslots = currentTemplate.template_timeslots;
                            }
                            else
                            {
                                startOrder = 1;
                                orderItem = orderedTemplates.FirstOrDefault(o => o.order == startOrder);
                                currentTemplate = orderItem?.template;
                                timeslots = currentTemplate.template_timeslots.Count()>0 ? currentTemplate.template_timeslots: Enumerable.Empty<TemplateTimeslot>();
                            }
                        }

                        startOrder++;

                        for (int y = 0; y < 5; y++)
                        {
                            string day = startWeek.ToString("yyyy-MM-dd");

                            foreach (var timeslot in timeslots.Where(ts => ts.day == y + 1))
                            {
                                string timeStart = timeslot.start.ToString("HH:mm:ss");
                                string timeEnd = timeslot.end.ToString("HH:mm:ss");

                                DateTime start = DateTime.ParseExact($"{day} {timeStart}", "yyyy-MM-dd HH:mm:ss", null);
                                DateTime end = DateTime.ParseExact($"{day} {timeEnd}", "yyyy-MM-dd HH:mm:ss", null);

                                var newTimeslot = new Timeslot
                                {
                                    start = start,
                                    end = end,
                                    description = timeslot.description,
                                    allDay = timeslot.allDay,
                                    duration = timeslot.duration,
                                    quantity = timeslot.quantity,
                                    blocked = !holidays.Any(h => h.date.ToString("yyyy-MM-dd") == day) ? timeslot.blocked : true,
                                    block_reason = string.IsNullOrEmpty(timeslot.block_reason) ? null : timeslot.block_reason,
                                    public_block = timeslot.public_block,
                                    category_id = timeslot.category_id,
                                    template_id = currentTemplate?.id
                                };
                                var ctlTimeslot = new TimeslotController();
                                ctlTimeslot.CreateTimeslot(newTimeslot);

                                var courtTimeslot = new CourtTimeslot
                                {
                                    court_id = courtId,
                                    timeslot_id = newTimeslot.id
                                };
                                var ctlCourtTimeslot = new CourtTimeslotController();
                                ctlCourtTimeslot.CreateCourtTimeslot(courtTimeslot);
                            }

                            startWeek = startWeek.AddDays(1);
                        }

                        startWeek = startWeek.AddDays(7);
                        startWeek = Common.StartOfWeek(startWeek);
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}