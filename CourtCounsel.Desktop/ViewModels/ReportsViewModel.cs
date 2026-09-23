using System.Collections.ObjectModel;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.ViewModels;

public class ReportGroupRow
{
    public string CaseTypeName { get; init; } = "";
    public int Count { get; init; }
    public List<HistoryInfo> Rows { get; init; } = new();
}

// Mirrors Reports.ascx.cs.
public class ReportsViewModel : ViewModelBase
{
    private readonly HistoryRepository _historyRepo = new();

    public static readonly TextValueOption[] ExtendedStatusOptions =
    {
        new("-- All --", ""),
        new("New", "New"),
        new("In Progress", "In Progress"),
        new("Under Review", "Under Review"),
        new("On Hold", "On Hold"),
        new("Completed", "Completed"),
        new("Dismissed", "Dismissed"),
        new("Withdrawn", "Withdrawn"),
    };

    private DateTime? _startDate;
    public DateTime? StartDate { get => _startDate; set => SetField(ref _startDate, value); }

    private DateTime? _endDate;
    public DateTime? EndDate { get => _endDate; set => SetField(ref _endDate, value); }

    private string _statusFilter = "Active";
    public string StatusFilter
    {
        get => _statusFilter;
        set { if (SetField(ref _statusFilter, value)) OnPropertyChanged(nameof(IsAllStatusSelected)); }
    }

    // RadioButton binding helper for the "All" option (empty-string value) —
    // ConverterParameter can't cleanly express an empty string in XAML.
    public bool IsAllStatusSelected
    {
        get => StatusFilter == "";
        set { if (value) StatusFilter = ""; }
    }

    private string _extendedStatus = "";
    public string ExtendedStatus { get => _extendedStatus; set => SetField(ref _extendedStatus, value); }

    public ObservableCollection<CountyInfo> Counties { get; } = new();
    private CountyInfo? _selectedCounty;
    public CountyInfo? SelectedCounty { get => _selectedCounty; set => SetField(ref _selectedCounty, value); }

    public ObservableCollection<RequestorInfo> Requestors { get; } = new();
    private RequestorInfo? _selectedRequestor;
    public RequestorInfo? SelectedRequestor { get => _selectedRequestor; set => SetField(ref _selectedRequestor, value); }

    public ObservableCollection<AttorneyCheckItem> AttorneyChecks { get; } = new();

    private bool _showDetail;
    public bool ShowDetail { get => _showDetail; set => SetField(ref _showDetail, value); }

    public ObservableCollection<ReportGroupRow> ReportGroups { get; } = new();

    private int _grandTotal;
    public int GrandTotal { get => _grandTotal; private set => SetField(ref _grandTotal, value); }

    private bool _hasResults;
    public bool HasResults { get => _hasResults; private set => SetField(ref _hasResults, value); }

    private bool _hasSearched;
    public bool HasSearched { get => _hasSearched; private set => SetField(ref _hasSearched, value); }

    public bool ShowNoResultsMessage => HasSearched && !HasResults;

    private string _generatedMessage = "";
    public string GeneratedMessage { get => _generatedMessage; private set => SetField(ref _generatedMessage, value); }

    public RelayCommand SubmitCommand { get; }
    public RelayCommand ResetCommand { get; }

    public ReportsViewModel()
    {
        SubmitCommand = new RelayCommand(Submit);
        ResetCommand = new RelayCommand(Reset);

        foreach (var c in new CountyRepository().GetAll()) Counties.Add(c);
        foreach (var r in new RequestorRepository().GetAll()) Requestors.Add(r);
        foreach (var a in new AttorneyRepository().GetAll().OrderByDescending(a => a.IsActive == true).ThenBy(a => a.AttorneyName))
            AttorneyChecks.Add(new AttorneyCheckItem(a));
    }

    private void Submit()
    {
        var checkedAttorneys = AttorneyChecks.Where(a => a.IsChecked).Select(a => a.Attorney.AttorneyName).ToList();
        var attorneyExactFilter = checkedAttorneys.Count == 1 ? checkedAttorneys[0] : "";

        var rows = _historyRepo.GetFilteredHistory(
            StartDate, EndDate, StatusFilter, ExtendedStatus,
            attorneyExactFilter,
            SelectedCounty?.County ?? "",
            SelectedRequestor?.RequestorName ?? "").ToList();

        if (checkedAttorneys.Count >= 2)
            rows = rows.Where(h => checkedAttorneys.Contains(h.Responsible)).ToList();

        ReportGroups.Clear();
        var groups = rows
            .GroupBy(h => string.IsNullOrEmpty(h.CaseType) ? "Unassigned" : h.CaseType)
            .OrderBy(g => g.Key)
            .Select(g => new ReportGroupRow
            {
                CaseTypeName = g.Key,
                Count = g.Count(),
                Rows = g.OrderBy(h => h.CaseNumber).ToList()
            });
        foreach (var g in groups) ReportGroups.Add(g);

        GrandTotal = rows.Count;
        HasResults = rows.Count > 0;
        HasSearched = true;
        GeneratedMessage = $"Report generated {DateTime.Now:g}. Total records: {rows.Count}.";
        OnPropertyChanged(nameof(ShowNoResultsMessage));
    }

    private void Reset()
    {
        StartDate = null;
        EndDate = null;
        StatusFilter = "Active";
        ExtendedStatus = "";
        SelectedCounty = null;
        SelectedRequestor = null;
        foreach (var a in AttorneyChecks) a.IsChecked = false;
        ShowDetail = false;
        ReportGroups.Clear();
        GrandTotal = 0;
        HasResults = false;
        HasSearched = false;
        GeneratedMessage = "";
        OnPropertyChanged(nameof(ShowNoResultsMessage));
    }
}
