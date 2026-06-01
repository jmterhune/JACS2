using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.CourtReporting.Components
{
    internal class TransactionController
    {
        public void CreateTransaction(Transaction a)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Insert(a);
            }
        }
        public void DeleteTransaction(int transactionId)
        {
            var a = GetTransaction(transactionId);

            DeleteTransaction(a);
        }
        public void DeleteTransaction(Transaction a)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Delete(a);
            }
        }
        public Transaction GetTransaction(int transactionId)
        {
            Transaction a;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                a = rep.GetById(transactionId);
            }
            return a;
        }
        public IEnumerable<Transaction> GetTransactions()
        {
            IEnumerable<Transaction> a;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                a = rep.Get();
            }
            return a;
        }
        public IEnumerable<Transaction> GetTransactionsByDateRange(DateTime start, DateTime end)
        {
            IEnumerable<Transaction> a;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                a = rep.Find("Where PaymentDate Between @0 And @1", start, end);

            }
            return a;
        }
        internal void UpdateTransaction(Transaction a)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Update(a);
            }
        }
    }

}