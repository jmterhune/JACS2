using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
    internal class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public long TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalPages
        {
            get { return PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalItems / (double)PageSize); }
        }
    }

    internal class HistoryController
    {
        public HistoryInfo GetHistory(int logId)
        {
            HistoryInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public IEnumerable<HistoryInfo> GetAllHistory()
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<HistoryInfo> GetHistoryByCaseNumber(string caseNumber, string caseName = "")
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                if (!string.IsNullOrEmpty(caseName))
                    t = rep.Find("WHERE CaseNumber = @0 AND PartyName = @1", caseNumber, caseName);
                else
                    t = rep.Find("WHERE CaseNumber = @0", caseNumber);
            }
            return t;
        }

        public IEnumerable<HistoryInfo> SearchByCaseName(string partyName)
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<HistoryInfo>(
                    System.Data.CommandType.Text,
                    "SELECT DISTINCT CaseNumber, PartyName, CaseType, Responsible, logId, DateReceived, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, DateCompleted, TimeSpent, Comments, StatusName, MotionFiled, LastModifiedDate FROM tjc_cc_history WHERE PartyName LIKE @0 ORDER BY PartyName",
                    "%" + partyName + "%");
            }
            return t;
        }

        public IEnumerable<HistoryInfo> SearchByAttorney(string attorney, string statusFilter)
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                // Match the VB stored procedure logic exactly.
                // Status codes: A=Active, I=Inactive(Pending), C=Complete
                // Combined: AI, AC, IC, AIC, all
                string sql;
                string likeAttorney = "%" + attorney + "%";

                if (string.IsNullOrEmpty(statusFilter)) statusFilter = "all";
                string normalized = new string(statusFilter.ToUpper().OrderBy(c => c).ToArray());

                switch (normalized)
                {
                    case "A":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND DateCompleted IS NULL AND DateReceived <= GETDATE() ORDER BY DateReceived, PartyName";
                        break;
                    case "I":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND DateCompleted IS NULL AND DateReceived > GETDATE() ORDER BY DateReceived, PartyName";
                        break;
                    case "C":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND DateCompleted IS NOT NULL ORDER BY DateReceived, PartyName";
                        break;
                    case "AI":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND DateCompleted IS NULL ORDER BY DateReceived, PartyName";
                        break;
                    case "AC":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND (DateReceived <= GETDATE() OR DateCompleted IS NOT NULL) ORDER BY DateReceived, PartyName";
                        break;
                    case "CI":
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 AND (DateReceived > GETDATE() OR DateCompleted IS NOT NULL) ORDER BY DateReceived, PartyName";
                        break;
                    default: // AIC, all, or any other combination
                        sql = "SELECT DISTINCT PartyName, Responsible, StatusName, DateReceived, DateCompleted, CaseType, CaseNumber, logId, DateDue, RequestedBy, County, Description, Phase, Action, FollowUp, TimeSpent, Comments, MotionFiled, LastModifiedDate " +
                              "FROM tjc_cc_history WHERE Responsible LIKE @0 ORDER BY DateReceived, PartyName";
                        break;
                }

                t = ctx.ExecuteQuery<HistoryInfo>(System.Data.CommandType.Text, sql, likeAttorney);
            }
            return t;
        }

        public IEnumerable<HistoryInfo> GetOverdueHistory(DateTime cutoffDate)
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                t = rep.Find("WHERE DateDue < @0 AND DateCompleted IS NULL", cutoffDate);
            }
            return t;
        }

        public IEnumerable<string> GetPartyNamesByCaseNumber(string caseNumber)
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<HistoryInfo>(
                    System.Data.CommandType.Text,
                    "SELECT DISTINCT PartyName, logId, DateReceived, CaseNumber, CaseType, DateDue, RequestedBy, Responsible, County, Description, Phase, Action, FollowUp, DateCompleted, TimeSpent, Comments, StatusName, MotionFiled, LastModifiedDate FROM tjc_cc_history WHERE CaseNumber = @0",
                    caseNumber);
            }
            return t.Select(h => h.PartyName).Distinct();
        }

        public void CreateHistory(HistoryInfo item)
        {
            item.LastModifiedDate = DateTime.Now;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                rep.Insert(item);
            }
        }

        public void UpdateHistory(HistoryInfo item)
        {
            item.LastModifiedDate = DateTime.Now;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryInfo>();
                rep.Update(item);
            }
        }

        public void DeleteHistory(int logId)
        {
            var item = GetHistory(logId);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<HistoryInfo>();
                    rep.Delete(item);
                }
            }
        }

        public void UpdateCaseName(string caseNumber, string newName)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE tjc_cc_history SET PartyName = @0, LastModifiedDate = GETDATE() WHERE CaseNumber = @1",
                    newName, caseNumber);
            }
        }

        public IEnumerable<HistoryInfo> GetFilteredHistory(DateTime? startDate, DateTime? endDate,
            string statusFilter, string extendedStatus, string attorney, string county, string requestor)
        {
            IEnumerable<HistoryInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var conditions = new List<string>();
                var args = new List<object>();
                int paramIndex = 0;

                if (startDate.HasValue)
                {
                    conditions.Add(string.Format("DateReceived >= @{0}", paramIndex++));
                    args.Add(startDate.Value);
                }
                if (endDate.HasValue)
                {
                    conditions.Add(string.Format("DateReceived <= @{0}", paramIndex++));
                    args.Add(endDate.Value);
                }
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    switch (statusFilter)
                    {
                        case "Active":
                            conditions.Add("DateCompleted IS NULL AND DateReceived <= GETDATE()");
                            break;
                        case "Inactive":
                            conditions.Add("DateReceived > GETDATE()");
                            break;
                        case "NotCompleted":
                            conditions.Add("DateCompleted IS NULL");
                            break;
                        case "Completed":
                            conditions.Add("DateCompleted IS NOT NULL");
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(extendedStatus))
                {
                    conditions.Add(string.Format("StatusName = @{0}", paramIndex++));
                    args.Add(extendedStatus);
                }
                if (!string.IsNullOrEmpty(attorney))
                {
                    conditions.Add(string.Format("Responsible = @{0}", paramIndex++));
                    args.Add(attorney);
                }
                if (!string.IsNullOrEmpty(county))
                {
                    conditions.Add(string.Format("County = @{0}", paramIndex++));
                    args.Add(county);
                }
                if (!string.IsNullOrEmpty(requestor))
                {
                    conditions.Add(string.Format("RequestedBy = @{0}", paramIndex++));
                    args.Add(requestor);
                }

                string sql = "SELECT * FROM tjc_cc_history";
                if (conditions.Any())
                    sql += " WHERE " + string.Join(" AND ", conditions);
                sql += " ORDER BY CaseType, DateReceived";

                t = ctx.ExecuteQuery<HistoryInfo>(System.Data.CommandType.Text, sql, args.ToArray());
            }
            return t;
        }

        /// <summary>
        /// Returns the distinct non-empty RequestedBy values for populating a filter dropdown.
        /// </summary>
        public IEnumerable<string> GetDistinctRequestedBy()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<string>(CommandType.Text,
                    "SELECT DISTINCT RequestedBy FROM tjc_cc_history " +
                    "WHERE RequestedBy IS NOT NULL AND LTRIM(RTRIM(RequestedBy)) <> '' " +
                    "ORDER BY RequestedBy").ToList();
            }
        }

        /// <summary>
        /// Build the shared WHERE clause + args used by the DataSheet page and export queries.
        /// </summary>
        private static string BuildDataSheetWhere(
            List<string> attorneys,
            DateTime? dateReceivedFrom,
            string requestedBy,
            bool excludeCompleted,
            List<object> args)
        {
            var conditions = new List<string>();

            if (attorneys != null && attorneys.Count > 0)
            {
                var placeholders = new List<string>();
                foreach (var a in attorneys)
                {
                    placeholders.Add("@" + args.Count);
                    args.Add(a);
                }
                conditions.Add("Responsible IN (" + string.Join(",", placeholders) + ")");
            }

            if (dateReceivedFrom.HasValue)
            {
                conditions.Add("DateReceived >= @" + args.Count);
                args.Add(dateReceivedFrom.Value);
            }

            if (!string.IsNullOrWhiteSpace(requestedBy))
            {
                conditions.Add("RequestedBy = @" + args.Count);
                args.Add(requestedBy);
            }

            if (excludeCompleted)
            {
                conditions.Add("DateCompleted IS NULL");
            }

            return conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : string.Empty;
        }

        public IEnumerable<HistoryInfo> GetHistoryForExport(
            List<string> attorneys = null,
            DateTime? dateReceivedFrom = null,
            string requestedBy = null,
            bool excludeCompleted = false)
        {
            var args = new List<object>();
            string where = BuildDataSheetWhere(attorneys, dateReceivedFrom, requestedBy, excludeCompleted, args);
            string sql = "SELECT * FROM tjc_cc_history" + where + " ORDER BY DateReceived DESC, logId DESC";

            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<HistoryInfo>(CommandType.Text, sql, args.ToArray()).ToList();
            }
        }

        /// <summary>
        /// Fetch a single page of history records. Uses SQL Server OFFSET/FETCH for DB-level paging
        /// (requires SQL Server 2012+). Runs one COUNT query and one paged SELECT per call.
        /// </summary>
        public PagedResult<HistoryInfo> GetHistoryPage(
            int pageNumber,
            int pageSize,
            List<string> attorneys = null,
            DateTime? dateReceivedFrom = null,
            string requestedBy = null,
            bool excludeCompleted = false)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 50;

            var args = new List<object>();
            string where = BuildDataSheetWhere(attorneys, dateReceivedFrom, requestedBy, excludeCompleted, args);

            var result = new PagedResult<HistoryInfo>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            using (IDataContext ctx = DataContext.Instance())
            {
                // Total count for pager display / last-page bounds
                string countSql = "SELECT COUNT(*) FROM tjc_cc_history" + where;
                result.TotalItems = ctx.ExecuteScalar<int>(CommandType.Text, countSql, args.ToArray());

                // If the requested page is past the end, clamp to the last page
                long totalPages = result.TotalPages;
                if (totalPages > 0 && pageNumber > totalPages)
                {
                    pageNumber = (int)totalPages;
                    result.PageNumber = pageNumber;
                }

                int offset = (pageNumber - 1) * pageSize;
                var pageArgs = new List<object>(args) { offset, pageSize };
                string dataSql = "SELECT * FROM tjc_cc_history" + where +
                                 " ORDER BY DateReceived DESC, logId DESC" +
                                 " OFFSET @" + (pageArgs.Count - 2) + " ROWS FETCH NEXT @" + (pageArgs.Count - 1) + " ROWS ONLY";

                result.Items = ctx.ExecuteQuery<HistoryInfo>(CommandType.Text, dataSql, pageArgs.ToArray()).ToList();
            }

            return result;
        }
    }
}
