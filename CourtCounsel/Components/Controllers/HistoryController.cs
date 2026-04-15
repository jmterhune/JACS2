using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Components.Controllers
{
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
                var rep = ctx.GetRepository<HistoryInfo>();
                string where = "WHERE Responsible = @0";
                var args = new List<object> { attorney };

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    var statuses = statusFilter.Split(',');
                    var conditions = new List<string>();

                    foreach (var s in statuses)
                    {
                        switch (s.Trim().ToUpper())
                        {
                            case "A": // Active
                                conditions.Add("(DateCompleted IS NULL AND DateReceived <= GETDATE())");
                                break;
                            case "I": // Inactive
                                conditions.Add("(DateReceived > GETDATE())");
                                break;
                            case "C": // Complete
                                conditions.Add("(DateCompleted IS NOT NULL)");
                                break;
                        }
                    }

                    if (conditions.Any())
                        where += " AND (" + string.Join(" OR ", conditions) + ")";
                }

                t = rep.Find(where, args.ToArray());
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
    }
}
