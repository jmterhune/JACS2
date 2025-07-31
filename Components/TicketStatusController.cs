using DotNetNuke.Data;
using System;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TicketStatusController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTicketStatus(TicketStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<TicketStatus>();
                rep.Insert(t);
            }
        }
        public void DeleteTicketStatus(int ticketstatusId)
        {
            var t = GetTicketStatus(ticketstatusId);
            DeleteTicketStatus(t);
        }
        public void DeleteTicketStatus(TicketStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketStatus>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TicketStatus> GetTicketStatuss()
        {
            IEnumerable<TicketStatus> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketStatus>();
                t = rep.Get();
            }
            return t;
        }
        public TicketStatus GetTicketStatus(int ticketstatusId)
        {
            TicketStatus t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketStatus>();
                t = rep.GetById(ticketstatusId);
            }
            return t;
        }
        public void UpdateTicketStatus(TicketStatus t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<TicketStatus>();
                rep.Update(t);
            }
        }
    }
}