using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class PartyController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateParty(Party t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Party>();
                rep.Insert(t);
            }
        }
        public void DeleteParty(int partyId)
        {
            var t = GetParty(partyId);
            DeleteParty(t);
        }
        public void DeleteParty(Party t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Party>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Party> GetPartys()
        {
            IEnumerable<Party> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Party>();
                t = rep.Get();
            }
            return t;
        }
        public Party GetParty(int partyId)
        {
            Party t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Party>();
                t = rep.GetById(partyId);
            }
            return t;
        }
        public void UpdateParty(Party t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Party>();
                rep.Update(t);
            }
        }
    }
}