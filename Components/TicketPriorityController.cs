using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TicketPriorityController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTicketPriority(TicketPriority t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketPriority>();
                rep.Insert(t);
            }
        }
        public void DeleteTicketPriority(int ticketpriorityId)
        {
            var t = GetTicketPriority(ticketpriorityId);
            DeleteTicketPriority(t);
        }
        public void DeleteTicketPriority(TicketPriority t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketPriority>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TicketPriority> GetTicketPrioritys()
        {
            IEnumerable<TicketPriority> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketPriority>();
                t = rep.Get();
            }
            return t;
        }
        public TicketPriority GetTicketPriority(int ticketpriorityId)
        {
            TicketPriority t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketPriority>();
                t = rep.GetById(ticketpriorityId);
            }
            return t;
        }
        public void UpdateTicketPriority(TicketPriority t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TicketPriority>();
                rep.Update(t);
            }
        }
    }
}