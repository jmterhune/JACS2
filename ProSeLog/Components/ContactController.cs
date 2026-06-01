using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.ProSeLog.Components
{
    internal class ContactController
    {
        public void CreateContact(Contact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Contact>();
                rep.Insert(t);
            }
        }

        public void DeleteContact(int contactId)
        {
            var t = GetContact(contactId);
            DeleteContact(t);
        }

        public void DeleteContact(Contact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Contact>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Contact> GetContacts()
        {
            IEnumerable<Contact> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Contact>();
                t = rep.Get();
            }
            return t;
        }

        public Contact GetContact(int contactId)
        {
            Contact t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Contact>();
                t = rep.GetById(contactId);
            }
            return t;
        }

        public void UpdateContact(Contact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Contact>();
                rep.Update(t);
            }
        }

    }
}
