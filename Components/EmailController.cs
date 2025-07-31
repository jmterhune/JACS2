using DotNetNuke.Data;
using System;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class EmailController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateEmail(Email t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;
                var rep = ctx.GetRepository<Email>();
                rep.Insert(t);
            }
        }
        public void DeleteEmail(int emailId)
        {
            var t = GetEmail(emailId);
            DeleteEmail(t);
        }
        public void DeletAllEmailsByAttorney(long attorney)
        {
            IEnumerable<Email> emails = GetEmails(attorney);
            foreach (Email email in emails)
            {
                DeleteEmail(email);
            }
        }
        public void DeleteEmail(Email t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Email>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Email> GetEmails()
        {
            IEnumerable<Email> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Email>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Email> GetEmails(long attorneyId)
        {
            IEnumerable<Email> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Email>();
                t = rep.Find("Where emailable_id=@0", attorneyId);
            }
            return t;
        }
        public Email GetEmail(int emailId)
        {
            Email t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Email>();
                t = rep.GetById(emailId);
            }
            return t;
        }

        public void UpdateEmail(Email t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;
                var rep = ctx.GetRepository<Email>();
                rep.Update(t);
            }
        }
    }
}