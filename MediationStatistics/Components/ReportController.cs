using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
                t = rep.Find("Where ReferralDate Between @0 And @1", startDate, endDate).OrderBy(x => x.Region).ThenBy(x => x.CaseTypeGroup);
            }
            return t;
        }
        public IEnumerable<FeesOwed> GetFeesOwed(DateTime startDate, DateTime endDate)
        {
            IEnumerable<FeesOwed> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FeesOwed>();
                t = rep.Find("Where MediationDate Between @0 And @1", startDate, endDate);
            }
            return t;
        }

        public IEnumerable<StatisticalReport> GetStatReport(DateTime startDate, DateTime endDate)
        {
            // Need to set command timeout. No other way in DAL2 to do that so going old school.
            List<StatisticalReport> stats = new List<StatisticalReport>();
            string connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("tjc_med_statistical_report", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandTimeout = 200
                };
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int index = 0;
                        StatisticalReport report = new StatisticalReport
                        {
                            questionaire = reader.GetString(index++),
                            question = reader.GetString(index++),
                            sarasota = reader.GetInt32(index++),
                            manatee = reader.GetInt32(index++),
                            desoto = reader.GetInt32(index++),
                            southCounty = reader.GetInt32(index++),
                            northCounty = reader.GetInt32(index++),
                            sPercent = reader.GetDouble(index++),
                            mPercent = reader.GetDouble(index++),
                            dPercent = reader.GetDouble(index++)
                        };
                        stats.Add(report);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return stats;
        }
        public IEnumerable<StatMediatorCounts> GetMediatorTypeReport(DateTime startDate, DateTime endDate)
        {
            IEnumerable<StatMediatorCounts> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<StatMediatorCounts>(System.Data.CommandType.StoredProcedure, "tjc_med_mediator_type_stats", startDate, endDate);
            }
            return t;
        }
        public IEnumerable<StatMediatorCounts> GetMediatorReport(DateTime startDate, DateTime endDate)
        {

            IEnumerable<StatMediatorCounts> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<StatMediatorCounts>(System.Data.CommandType.StoredProcedure, "tjc_med_mediator_stats", startDate, endDate);
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
