using Dapper;
using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.Data;

public class AttorneyRepository
{
    public IEnumerable<AttorneyInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<AttorneyInfo>(
            "SELECT AttorneyId, AttorneyName, IsActive FROM dbo.tjc_cc_attorney ORDER BY AttorneyName").ToList();
    }

    public IEnumerable<AttorneyInfo> GetActive()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<AttorneyInfo>(
            "SELECT AttorneyId, AttorneyName, IsActive FROM dbo.tjc_cc_attorney WHERE IsActive = 1 ORDER BY AttorneyName").ToList();
    }

    public void Insert(AttorneyInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.AttorneyId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_attorney (AttorneyName, IsActive) OUTPUT INSERTED.AttorneyId VALUES (@AttorneyName, @IsActive)",
            item);
    }

    public void Update(AttorneyInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute(
            "UPDATE dbo.tjc_cc_attorney SET AttorneyName = @AttorneyName, IsActive = @IsActive WHERE AttorneyId = @AttorneyId",
            item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_attorney WHERE AttorneyId = @id", new { id });
    }
}

public class RequestorRepository
{
    public IEnumerable<RequestorInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<RequestorInfo>(
            "SELECT RequestorId, RequestorName, IsActive FROM dbo.tjc_cc_requestor ORDER BY RequestorName").ToList();
    }

    public IEnumerable<RequestorInfo> GetActive()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<RequestorInfo>(
            "SELECT RequestorId, RequestorName, IsActive FROM dbo.tjc_cc_requestor WHERE IsActive = 1 ORDER BY RequestorName").ToList();
    }

    public void Insert(RequestorInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.RequestorId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_requestor (RequestorName, IsActive) OUTPUT INSERTED.RequestorId VALUES (@RequestorName, @IsActive)",
            item);
    }

    public void Update(RequestorInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute(
            "UPDATE dbo.tjc_cc_requestor SET RequestorName = @RequestorName, IsActive = @IsActive WHERE RequestorId = @RequestorId",
            item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_requestor WHERE RequestorId = @id", new { id });
    }
}

public class CaseTypeRepository
{
    public IEnumerable<CaseTypeInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<CaseTypeInfo>(
            "SELECT CaseTypeId, CaseType FROM dbo.tjc_cc_case_type ORDER BY CaseType").ToList();
    }

    public void Insert(CaseTypeInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.CaseTypeId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_case_type (CaseType) OUTPUT INSERTED.CaseTypeId VALUES (@CaseType)", item);
    }

    public void Update(CaseTypeInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("UPDATE dbo.tjc_cc_case_type SET CaseType = @CaseType WHERE CaseTypeId = @CaseTypeId", item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_case_type WHERE CaseTypeId = @id", new { id });
    }
}

public class CountyRepository
{
    public IEnumerable<CountyInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<CountyInfo>(
            "SELECT CountyId, County FROM dbo.tjc_cc_county ORDER BY County").ToList();
    }

    public void Insert(CountyInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.CountyId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_county (County) OUTPUT INSERTED.CountyId VALUES (@County)", item);
    }

    public void Update(CountyInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("UPDATE dbo.tjc_cc_county SET County = @County WHERE CountyId = @CountyId", item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_county WHERE CountyId = @id", new { id });
    }
}

public class PhaseRepository
{
    public IEnumerable<PhaseInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<PhaseInfo>(
            "SELECT PhaseId, Phase FROM dbo.tjc_cc_phase ORDER BY Phase").ToList();
    }

    public void Insert(PhaseInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.PhaseId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_phase (Phase) OUTPUT INSERTED.PhaseId VALUES (@Phase)", item);
    }

    public void Update(PhaseInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("UPDATE dbo.tjc_cc_phase SET Phase = @Phase WHERE PhaseId = @PhaseId", item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_phase WHERE PhaseId = @id", new { id });
    }
}

public class ActionTakenRepository
{
    public IEnumerable<ActionTakenInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<ActionTakenInfo>(
            "SELECT ActionId, Action FROM dbo.tjc_cc_action_taken ORDER BY Action").ToList();
    }

    public void Insert(ActionTakenInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.ActionId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_action_taken (Action) OUTPUT INSERTED.ActionId VALUES (@Action)", item);
    }

    public void Update(ActionTakenInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("UPDATE dbo.tjc_cc_action_taken SET Action = @Action WHERE ActionId = @ActionId", item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_action_taken WHERE ActionId = @id", new { id });
    }
}

public class TimeSpentRepository
{
    // Ordered by TimeSpanId (not alphabetically) to match the original
    // drpTimeSpan binding in EditHistory.ascx.cs.
    public IEnumerable<TimeSpentInfo> GetAll()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<TimeSpentInfo>(
            "SELECT TimeSpanId, TimeSpan, IsActive FROM dbo.tjc_cc_time_spent ORDER BY TimeSpanId").ToList();
    }

    public IEnumerable<TimeSpentInfo> GetActive()
    {
        using var conn = SqlConnectionFactory.Create();
        return conn.Query<TimeSpentInfo>(
            "SELECT TimeSpanId, TimeSpan, IsActive FROM dbo.tjc_cc_time_spent WHERE IsActive = 1 ORDER BY TimeSpanId").ToList();
    }

    public void Insert(TimeSpentInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        item.TimeSpanId = conn.ExecuteScalar<int>(
            "INSERT INTO dbo.tjc_cc_time_spent (TimeSpan, IsActive) OUTPUT INSERTED.TimeSpanId VALUES (@TimeSpan, @IsActive)",
            item);
    }

    public void Update(TimeSpentInfo item)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute(
            "UPDATE dbo.tjc_cc_time_spent SET TimeSpan = @TimeSpan, IsActive = @IsActive WHERE TimeSpanId = @TimeSpanId",
            item);
    }

    public void Delete(int id)
    {
        using var conn = SqlConnectionFactory.Create();
        conn.Execute("DELETE FROM dbo.tjc_cc_time_spent WHERE TimeSpanId = @id", new { id });
    }
}
