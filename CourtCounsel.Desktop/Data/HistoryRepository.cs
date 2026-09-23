using Dapper;
using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.Data;

// Mirrors CourtCounsel/Components/Controllers/HistoryController.cs exactly —
// same queries, same status-filter branching, same "loose text match, no
// FKs" data model. See the module's functional spec for the full rationale
// behind each query.
public class HistoryRepository
{
    private const string Columns =
        "LogId, DateReceived, CaseNumber, PartyName, CaseType, DateDue, RequestedBy, Responsible, " +
        "County, Description, Phase, Action, FollowUp, DateCompleted, TimeSpent, Comments, StatusName, " +
        "MotionFiled, LastModifiedDate";

    public HistoryInfo? GetHistory(int logId)
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.QueryFirstOrDefault<HistoryInfo>(
            $"SELECT {Columns} FROM dbo.tjc_cc_history WHERE LogId = @logId", new { logId });
    }

    public IEnumerable<HistoryInfo> GetHistoryByCaseNumber(string caseNumber, string caseName = "")
    {
        using var conn = SqlConnectionFactory.Create();
        var sql = $"SELECT {Columns} FROM dbo.tjc_cc_history WHERE CaseNumber = @caseNumber";
        if (!string.IsNullOrEmpty(caseName)) sql += " AND PartyName = @caseName";
        return conn.Query<HistoryInfo>(sql, new { caseNumber, caseName }).ToList();
    }

    public IEnumerable<HistoryInfo> SearchByCaseName(string partyName)
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<HistoryInfo>(
            $"SELECT DISTINCT {Columns} FROM dbo.tjc_cc_history WHERE PartyName LIKE @term ORDER BY PartyName",
            new { term = $"%{partyName}%" }).ToList();
    }

    public IEnumerable<HistoryInfo> SearchByCaseNumber(string partial)
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<HistoryInfo>(
            $"SELECT DISTINCT {Columns} FROM dbo.tjc_cc_history WHERE CaseNumber LIKE @term ORDER BY CaseNumber",
            new { term = $"%{partial}%" }).ToList();
    }

    public IEnumerable<HistoryInfo> SearchByAttorney(string attorney, string statusFilter)
    {
        var normalized = string.IsNullOrEmpty(statusFilter)
            ? "ALL"
            : new string(statusFilter.ToUpperInvariant().OrderBy(c => c).ToArray());

        string where;
        switch (normalized)
        {
            case "A":
                where = "Responsible LIKE @attorney AND DateCompleted IS NULL AND DateReceived <= GETDATE()";
                break;
            case "I":
                where = "Responsible LIKE @attorney AND DateCompleted IS NULL AND DateReceived > GETDATE()";
                break;
            case "C":
                where = "Responsible LIKE @attorney AND DateCompleted IS NOT NULL";
                break;
            case "AI":
                where = "Responsible LIKE @attorney AND DateCompleted IS NULL";
                break;
            case "AC":
                where = "Responsible LIKE @attorney AND (DateReceived <= GETDATE() OR DateCompleted IS NOT NULL)";
                break;
            case "CI":
                where = "Responsible LIKE @attorney AND (DateReceived > GETDATE() OR DateCompleted IS NOT NULL)";
                break;
            default: // AIC / ALL / anything else — everything for this attorney
                where = "Responsible LIKE @attorney";
                break;
        }

        using var conn = SqlConnectionFactory.Create();
        var sql = $"SELECT {Columns} FROM dbo.tjc_cc_history WHERE {where} ORDER BY DateReceived, PartyName";
        return conn.Query<HistoryInfo>(sql, new { attorney = $"%{attorney}%" }).ToList();
    }

    public IEnumerable<HistoryInfo> GetOverdueHistory(DateTime cutoffDate)
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<HistoryInfo>(
            $"SELECT {Columns} FROM dbo.tjc_cc_history WHERE DateDue < @cutoffDate AND DateCompleted IS NULL",
            new { cutoffDate }).ToList();
    }

    public IEnumerable<string> GetPartyNamesByCaseNumber(string caseNumber)
    {
        return GetHistoryByCaseNumber(caseNumber)
            .Select(h => h.PartyName)
            .Distinct()
            .ToList();
    }

    public void CreateHistory(HistoryInfo item)
    {
        item.LastModifiedDate = DateTime.Now;
        using var conn = SqlConnectionFactory.Create();
        item.LogId = conn.ExecuteScalar<int>($@"
            INSERT INTO dbo.tjc_cc_history
                (DateReceived, CaseNumber, PartyName, CaseType, DateDue, RequestedBy, Responsible, County,
                 Description, Phase, Action, FollowUp, DateCompleted, TimeSpent, Comments, StatusName,
                 MotionFiled, LastModifiedDate)
            OUTPUT INSERTED.LogId
            VALUES
                (@DateReceived, @CaseNumber, @PartyName, @CaseType, @DateDue, @RequestedBy, @Responsible, @County,
                 @Description, @Phase, @Action, @FollowUp, @DateCompleted, @TimeSpent, @Comments, @StatusName,
                 @MotionFiled, @LastModifiedDate)", item);
    }

    public void UpdateHistory(HistoryInfo item)
    {
        item.LastModifiedDate = DateTime.Now;
        using var conn = SqlConnectionFactory.Create();
        conn.Execute(@"
            UPDATE dbo.tjc_cc_history SET
                DateReceived = @DateReceived, CaseNumber = @CaseNumber, PartyName = @PartyName,
                CaseType = @CaseType, DateDue = @DateDue, RequestedBy = @RequestedBy, Responsible = @Responsible,
                County = @County, Description = @Description, Phase = @Phase, Action = @Action,
                FollowUp = @FollowUp, DateCompleted = @DateCompleted, TimeSpent = @TimeSpent,
                Comments = @Comments, StatusName = @StatusName, MotionFiled = @MotionFiled,
                LastModifiedDate = @LastModifiedDate
            WHERE LogId = @LogId", item);
    }

    public void DeleteHistory(int logId)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_history WHERE LogId = @logId", new { logId });
    }

    public void UpdateCaseName(string caseNumber, string newName)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute(
            "UPDATE dbo.tjc_cc_history SET PartyName = @newName, LastModifiedDate = GETDATE() WHERE CaseNumber = @caseNumber",
            new { newName, caseNumber });
    }

    public IEnumerable<HistoryInfo> GetFilteredHistory(
        DateTime? startDate, DateTime? endDate, string statusFilter, string extendedStatus,
        string attorney, string county, string requestor)
    {
        var where = new List<string>();
        var args = new DynamicParameters();

        var dateColumn = string.Equals(statusFilter, "Completed", StringComparison.OrdinalIgnoreCase)
            ? "DateCompleted" : "DateReceived";
        if (startDate.HasValue) { where.Add($"{dateColumn} >= @startDate"); args.Add("startDate", startDate.Value); }
        if (endDate.HasValue) { where.Add($"{dateColumn} <= @endDate"); args.Add("endDate", endDate.Value); }

        switch (statusFilter)
        {
            case "Active":
                where.Add("DateCompleted IS NULL AND DateReceived <= GETDATE()");
                break;
            case "Inactive":
                where.Add("DateReceived > GETDATE()");
                break;
            case "NotCompleted":
                where.Add("DateCompleted IS NULL");
                break;
            case "Completed":
                where.Add("DateCompleted IS NOT NULL");
                break;
            // "" / anything else -> All, no condition
        }

        if (!string.IsNullOrEmpty(extendedStatus)) { where.Add("StatusName = @extendedStatus"); args.Add("extendedStatus", extendedStatus); }
        if (!string.IsNullOrEmpty(attorney)) { where.Add("Responsible = @attorney"); args.Add("attorney", attorney); }
        if (!string.IsNullOrEmpty(county)) { where.Add("County = @county"); args.Add("county", county); }
        if (!string.IsNullOrEmpty(requestor)) { where.Add("RequestedBy = @requestor"); args.Add("requestor", requestor); }

        var sql = $"SELECT {Columns} FROM dbo.tjc_cc_history";
        if (where.Count > 0) sql += " WHERE " + string.Join(" AND ", where);
        sql += " ORDER BY CaseType, DateReceived";

        using var conn = SqlConnectionFactory.Create();
        return conn.Query<HistoryInfo>(sql, args).ToList();
    }

    private static (string whereSql, DynamicParameters args) BuildDataSheetWhere(
        IReadOnlyList<string> attorneys, DateTime? dateReceivedFrom, string requestedBy, bool excludeCompleted)
    {
        var where = new List<string>();
        var args = new DynamicParameters();

        if (attorneys.Count > 0)
        {
            var names = new List<string>();
            for (int i = 0; i < attorneys.Count; i++)
            {
                var p = $"attorney{i}";
                names.Add($"@{p}");
                args.Add(p, attorneys[i]);
            }
            where.Add($"Responsible IN ({string.Join(",", names)})");
        }
        if (dateReceivedFrom.HasValue)
        {
            where.Add("DateReceived >= @dateReceivedFrom");
            args.Add("dateReceivedFrom", dateReceivedFrom.Value);
        }
        if (!string.IsNullOrEmpty(requestedBy))
        {
            where.Add("RequestedBy = @requestedBy");
            args.Add("requestedBy", requestedBy);
        }
        if (excludeCompleted) where.Add("DateCompleted IS NULL");

        var sql = where.Count > 0 ? " WHERE " + string.Join(" AND ", where) : "";
        return (sql, args);
    }

    public IEnumerable<string> GetDistinctRequestedBy()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<string>(
            "SELECT DISTINCT RequestedBy FROM dbo.tjc_cc_history WHERE RequestedBy IS NOT NULL AND LTRIM(RTRIM(RequestedBy)) <> '' ORDER BY RequestedBy")
            .ToList();
    }

    public IEnumerable<HistoryInfo> GetHistoryForExport(
        IReadOnlyList<string> attorneys, DateTime? dateReceivedFrom, string requestedBy, bool excludeCompleted)
    {
        var (whereSql, args) = BuildDataSheetWhere(attorneys, dateReceivedFrom, requestedBy, excludeCompleted);
        var sql = $"SELECT {Columns} FROM dbo.tjc_cc_history{whereSql} ORDER BY DateReceived DESC, LogId DESC";
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<HistoryInfo>(sql, args).ToList();
    }

    public PagedResult<HistoryInfo> GetHistoryPage(
        int pageNumber, int pageSize,
        IReadOnlyList<string> attorneys, DateTime? dateReceivedFrom, string requestedBy, bool excludeCompleted)
    {
        var (whereSql, args) = BuildDataSheetWhere(attorneys, dateReceivedFrom, requestedBy, excludeCompleted);
        using var conn = SqlConnectionFactory.Create();

        var total = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM dbo.tjc_cc_history{whereSql}", args);

        var totalPages = pageSize <= 0 ? 0 : (long)Math.Ceiling(total / (double)pageSize);
        if (totalPages > 0 && pageNumber > totalPages) pageNumber = (int)totalPages;
        if (pageNumber < 1) pageNumber = 1;

        args.Add("offset", (pageNumber - 1) * pageSize);
        args.Add("pageSize", pageSize);
        var sql = $@"SELECT {Columns} FROM dbo.tjc_cc_history{whereSql}
                     ORDER BY DateReceived DESC, LogId DESC
                     OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        var items = conn.Query<HistoryInfo>(sql, args).ToList();

        return new PagedResult<HistoryInfo>
        {
            Items = items,
            TotalItems = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
