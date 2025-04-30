using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class StatsController
    {
        public IEnumerable<StatRecord> ExcludedSum(DateTime startDate,DateTime endDate,int jurisdictionId)
        {
            IEnumerable<StatRecord> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT a.CDCount,a.TotalMinutes FROM tjc_dcr_proceeding p INNER JOIN tjc_dcr_audio a ON p.ProceedingID=a.ProceedingID " +
                    "WHERE (NOT (LOWER(p.Involvement) IN ('state attorney','public defender','court appointed counsel')) AND (DateotCA BETWEEN @0 AND @1) AND JurisdictionID = @2)";
                t = ctx.ExecuteQuery<StatRecord>(System.Data.CommandType.Text, sql, startDate,endDate,jurisdictionId);
            }
            return t;
        }
        public IEnumerable<StatRecord> StateAttorneySum(DateTime startDate, DateTime endDate, int jurisdictionId)
        {
            IEnumerable<StatRecord> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT a.CDCount,a.TotalMinutes FROM tjc_dcr_proceeding p INNER JOIN tjc_dcr_audio a ON p.ProceedingID=a.ProceedingID " +
                    "WHERE (LOWER(p.Involvement) = 'state attorney' AND (DateotCA BETWEEN @0 AND @1) AND JurisdictionID = @2)";
                t = ctx.ExecuteQuery<StatRecord>(System.Data.CommandType.Text, sql, startDate, endDate, jurisdictionId);
            }
            return t;
        }
        public IEnumerable<StatRecord> PublicDefenderSum(DateTime startDate, DateTime endDate, int jurisdictionId)
        {
            IEnumerable<StatRecord> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT a.CDCount,a.TotalMinutes FROM tjc_dcr_proceeding p INNER JOIN tjc_dcr_audio a ON p.ProceedingID=a.ProceedingID " +
                    "WHERE (LOWER(p.Involvement) = 'public defender' AND (DateotCA BETWEEN @0 AND @1) AND JurisdictionID = @2)";
                t = ctx.ExecuteQuery<StatRecord>(System.Data.CommandType.Text, sql, startDate, endDate, jurisdictionId);
            }
            return t;
        }
        public IEnumerable<StatRecord> CourtAttorneySum(DateTime startDate, DateTime endDate, int jurisdictionId)
        {
            IEnumerable<StatRecord> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT a.CDCount,a.TotalMinutes FROM tjc_dcr_proceeding p INNER JOIN tjc_dcr_audio a ON p.ProceedingID=a.ProceedingID " +
                    "WHERE (LOWER(p.Involvement) = 'court appointed counsel' AND (DateotCA BETWEEN @0 AND @1) AND JurisdictionID = @2)";
                t = ctx.ExecuteQuery<StatRecord>(System.Data.CommandType.Text, sql, startDate, endDate, jurisdictionId);
            }
            return t;
        }
        public IEnumerable<StatRecord> TotalSum(DateTime startDate, DateTime endDate, int jurisdictionId)
        {
            IEnumerable<StatRecord> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT a.CDCount,a.TotalMinutes FROM tjc_dcr_proceeding p INNER JOIN tjc_dcr_audio a ON p.ProceedingID=a.ProceedingID " +
                    "WHERE ((DateotCA BETWEEN @0 AND @1) AND JurisdictionID = @2)";
                t = ctx.ExecuteQuery<StatRecord>(System.Data.CommandType.Text, sql, startDate, endDate, jurisdictionId);
            }
            return t;

        }
    }
}
