using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class AccountController
    {
        public void CreateAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Insert(t);
            }
        }
        public void DeleteAccount(int accountId)
        {
            var t = GetAccount(accountId);
            DeleteAccount(t);
        }
        public void DeleteAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Account> GetAccounts()
        {
            IEnumerable<Account> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                t = rep.Get();
            }
            return t;
        }
        public Account GetAccount(int accountId)
        {
            Account t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                t = rep.GetById(accountId);
            }
            return t;
        }
        public void UpdateAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Update(t);
            }
        }

        internal Account GetAccountByProceeding(int proceedingId)
        {
            Account t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                t = rep.Find("Where ProceedingID=@0",proceedingId).FirstOrDefault();
            }
            return t;
        }
    }
}
