using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class ReportController
    {

        public IEnumerable<FeesOwed> GetFeesOwed()
        {
            IEnumerable<FeesOwed> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FeesOwed>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<SessionCount> GetSessionCounts()
        {
            IEnumerable<SessionCount> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SessionCount>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<SessionCount> GetSessionCounts(DateTime startDate, DateTime endDate)
        {
            IEnumerable<SessionCount> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SessionCount>();
                t = rep.Find("Where ReferralDate Between @0 And @1", startDate, endDate).OrderBy(x=>x.Region).ThenBy(x=>x.CaseTypeGroup);
            }
            return t;
        }
        public IEnumerable<FeesOwed> GetFeesOwed(DateTime startDate,DateTime endDate)
        {
            IEnumerable<FeesOwed> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FeesOwed>();
                t = rep.Find("Where MediationDate Between @0 And @1",startDate,endDate);
            }
            return t;
        }
        
        public IEnumerable<StatisticalReport> GetStatReport(DateTime startDate, DateTime endDate)
        {
            IEnumerable<StatisticalReport> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<StatisticalReport>(System.Data.CommandType.StoredProcedure, "tjc_med_statistical_report", startDate,endDate);
            }
            return t;
        }
        public IEnumerable<FeeReportCollectedOwed> GetFeeReportCollectedOwed(DateTime startDate, DateTime endDate)
        {
            IEnumerable<FeeReportCollectedOwed> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<FeeReportCollectedOwed>(System.Data.CommandType.StoredProcedure, "tjc_med_fee_report_collected_owed", startDate, endDate);
            }
            return t;
        }


    }
}
