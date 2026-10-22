namespace CourtCounsel.Desktop.Models;

public class AttorneyInfo
{
    public int AttorneyId { get; set; }
    public string AttorneyName { get; set; } = "";
    public bool? IsActive { get; set; }
}

public class RequestorInfo
{
    public int RequestorId { get; set; }
    public string RequestorName { get; set; } = "";
    public bool? IsActive { get; set; }
}

public class CaseTypeInfo
{
    public int CaseTypeId { get; set; }
    public string CaseType { get; set; } = "";
}

public class CountyInfo
{
    public int CountyId { get; set; }
    public string County { get; set; } = "";
}

public class PhaseInfo
{
    public int PhaseId { get; set; }
    public string Phase { get; set; } = "";
}

public class ActionTakenInfo
{
    public int ActionId { get; set; }
    public string Action { get; set; } = "";
}

public class TimeSpentInfo
{
    public int TimeSpanId { get; set; }
    public string TimeSpan { get; set; } = "";
    public bool IsActive { get; set; }
}
