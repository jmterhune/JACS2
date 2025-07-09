using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TicketController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTicket(Ticket t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Ticket>();
                rep.Insert(t);
            }
        }
        public void DeleteTicket(int ticketId)
        {
            var t = GetTicket(ticketId);
            DeleteTicket(t);
        }
        public void DeleteTicket(Ticket t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Ticket>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Ticket> GetTickets()
        {
            IEnumerable<Ticket> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Ticket>();
                t = rep.Get();
            }
            return t;
        }
        public Ticket GetTicket(int ticketId)
        {
            Ticket t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Ticket>();
                t = rep.GetById(ticketId);
            }
            return t;
        }
        public void UpdateTicket(Ticket t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Ticket>();
                rep.Update(t);
            }
        }
    }
}