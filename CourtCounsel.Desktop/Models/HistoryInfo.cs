namespace CourtCounsel.Desktop.Models;

public enum HistoryStatus
{
    Active = 0,
    Inactive = 1,
    Complete = 2
}

// Maps to tjc_cc_history. Column names/behavior mirror the DNN module's
// HistoryInfo model exactly (see CourtCounsel/Components/Models), including
// the loose (non-FK) text matching against the lookup tables and the
// computed, never-persisted Status.
public class HistoryInfo
{
    public int LogId { get; set; }
    public DateTime DateReceived { get; set; }
    public string CaseNumber { get; set; } = "";
    public string PartyName { get; set; } = "";
    public string CaseType { get; set; } = "";
    public DateTime? DateDue { get; set; }
    public string RequestedBy { get; set; } = "";
    public string Responsible { get; set; } = "";
    public string County { get; set; } = "";
    public string? Description { get; set; }
    public string? Phase { get; set; }
    public string? Action { get; set; }
    public string? FollowUp { get; set; }
    public DateTime? DateCompleted { get; set; }
    public string? TimeSpent { get; set; }
    public string? Comments { get; set; }
    public string? StatusName { get; set; }
    public DateTime? MotionFiled { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public HistoryStatus Status =>
        DateCompleted.HasValue ? HistoryStatus.Complete
        : DateReceived > DateTime.Now ? HistoryStatus.Inactive
        : HistoryStatus.Active;

    public int StatusSort => (int)Status;

    // Active status with a non-empty StatusName shows the StatusName text
    // instead of the word "Active" (list screens only; matches GetStatus()
    // in CaseList/AttorneyCaseList/DataSheet code-behind).
    public string DisplayStatus =>
        Status == HistoryStatus.Active && !string.IsNullOrWhiteSpace(StatusName)
            ? StatusName!
            : Status.ToString();
}
