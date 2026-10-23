using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.JCID.Components
{
    class TransactionController
    {
        // Credit-card payments are recorded in the jud12 (external/public) site's database. Read them here
        // through the Jud12 named connection string, configured in the site web.config.
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public TransactionController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Transaction>().Get();
            }
        }

        public IEnumerable<Transaction> GetTransactionsByDateRange(DateTime start, DateTime end)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Transaction>().Find("Where PaymentDate Between @0 And @1", start, end);
            }
        }
    }
}
