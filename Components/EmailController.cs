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

        /// <summary>
        /// Returns every non-blank email address in the emails table. Used by
        /// the bar-member import to pre-build an in-memory dedup set so the
        /// per-row "is this address already on file?" check is O(1) instead of
        /// one SELECT per CSV row.
        /// </summary>
        public IEnumerable<string> GetAllEmailAddresses()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<string>(
                    System.Data.CommandType.Text,
                    "SELECT email FROM emails WHERE email IS NOT NULL AND LTRIM(RTRIM(email)) <> ''");
            }
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