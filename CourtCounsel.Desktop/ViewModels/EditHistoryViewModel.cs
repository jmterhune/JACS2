using System.Collections.ObjectModel;
using System.Windows;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

public class CountyLetterOption
{
    public string Letter { get; init; } = "";
    public string Name { get; init; } = "";
    public override string ToString() => string.IsNullOrEmpty(Letter) ? "" : $"{Letter} - {Name}";
}

// Mirrors EditHistory.ascx.cs — the "Data Entry" screen: create/edit a
// single tjc_cc_history row, including the composite case-number widget
// and the future-action clone-forward feature.
public class EditHistoryViewModel : ViewModelBase
{
    private readonly HistoryRepository _historyRepo = new();

    // Whether this instance was opened to edit an existing row (?lid=)
    // rather than to add a new one. Cancel's target depends only on this,
    // exactly matching the original web behavior — even a "new row for an
    // existing case" flow (opened via caseNumberForNew) returns to Search,
    // not back to the case's history, on Cancel.
    private readonly bool _arrivedViaEdit;

    public static readonly IReadOnlyList<CountyLetterOption> CountyLetters = new List<CountyLetterOption>
    {
        new() { Letter = "", Name = "" },
        new() { Letter = "D", Name = "DeSoto" },
        new() { Letter = "M", Name = "Manatee" },
        new() { Letter = "S", Name = "Sarasota" },
        new() { Letter = "V", Name = "Venice" },
    };

    public IReadOnlyList<StatusNameOption> StatusNameChoices => StatusNameOptions.All;

    public ObservableCollection<CaseTypeInfo> CaseTypes { get; } = new();
    public ObservableCollection<RequestorInfo> Requestors { get; } = new();
    public ObservableCollection<AttorneyInfo> Attorneys { get; } = new();
    public ObservableCollection<CountyInfo> Counties { get; } = new();
    public ObservableCollection<ActionTakenInfo> Actions { get; } = new();
    public ObservableCollection<TimeSpentInfo> TimeSpents { get; } = new();

    public ObservableCollection<string> ValidationErrors { get; } = new();

    private int _logId;
    public int LogId
    {
        get => _logId;
        private set
        {
            if (SetField(ref _logId, value))
            {
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(ShowFutureActionPanel));
            }
        }
    }

    public bool CanDelete => LogId > 0;
    public bool ShowFutureActionPanel => LogId > 0;

    private DateTime? _dateReceived;
    public DateTime? DateReceived { get => _dateReceived; set => SetField(ref _dateReceived, value); }

    private string _countyLetter = "";
    public string CountyLetter
    {
        get => _countyLetter;
        set { if (SetField(ref _countyLetter, value)) RefreshCaseNumberPreview(); }
    }

    private string _caseYear = "";
    public string CaseYear
    {
        get => _caseYear;
        set { if (SetField(ref _caseYear, value)) RefreshCaseNumberPreview(); }
    }

    private string _caseTypeCode = "";
    public string CaseTypeCode
    {
        get => _caseTypeCode;
        set
        {
            var upper = (value ?? "").ToUpperInvariant();
            if (SetField(ref _caseTypeCode, upper))
            {
                OnPropertyChanged(nameof(SuffixMode));
                OnPropertyChanged(nameof(ShowDefendantSuffix));
                RefreshCaseNumberPreview();
            }
        }
    }

    private string _caseSequence = "";
    public string CaseSequence
    {
        get => _caseSequence;
        set { if (SetField(ref _caseSequence, value)) RefreshCaseNumberPreview(); }
    }

    private string _defendantSuffix = "";
    public string DefendantSuffix
    {
        get => _defendantSuffix;
        set { if (SetField(ref _defendantSuffix, value)) RefreshCaseNumberPreview(); }
    }

    public SuffixMode SuffixMode => CaseNumberHelper.GetSuffixMode(CountyLetter, CaseTypeCode);
    public bool ShowDefendantSuffix => SuffixMode != SuffixMode.None;

    private string _caseNumberPreview = "";
    public string CaseNumberPreview { get => _caseNumberPreview; private set => SetField(ref _caseNumberPreview, value); }

    private void RefreshCaseNumberPreview()
    {
        CaseNumberPreview = CaseNumberHelper.Build(CountyLetter, CaseYear, CaseTypeCode, CaseSequence, DefendantSuffix);
        OnPropertyChanged(nameof(SuffixMode));
        OnPropertyChanged(nameof(ShowDefendantSuffix));
    }

    private string _caseName = "";
    public string CaseName { get => _caseName; set => SetField(ref _caseName, value); }

    private CaseTypeInfo? _selectedCaseType;
    public CaseTypeInfo? SelectedCaseType { get => _selectedCaseType; set => SetField(ref _selectedCaseType, value); }

    private RequestorInfo? _selectedRequestor;
    public RequestorInfo? SelectedRequestor { get => _selectedRequestor; set => SetField(ref _selectedRequestor, value); }

    private AttorneyInfo? _selectedAttorney;
    public AttorneyInfo? SelectedAttorney { get => _selectedAttorney; set => SetField(ref _selectedAttorney, value); }

    private DateTime? _motionFiled;
    public DateTime? MotionFiled { get => _motionFiled; set => SetField(ref _motionFiled, value); }

    private CountyInfo? _selectedCounty;
    public CountyInfo? SelectedCounty { get => _selectedCounty; set => SetField(ref _selectedCounty, value); }

    private ActionTakenInfo? _selectedAction;
    public ActionTakenInfo? SelectedAction { get => _selectedAction; set => SetField(ref _selectedAction, value); }

    private DateTime? _dateCompleted;
    public DateTime? DateCompleted { get => _dateCompleted; set => SetField(ref _dateCompleted, value); }

    private TimeSpentInfo? _selectedTimeSpent;
    public TimeSpentInfo? SelectedTimeSpent { get => _selectedTimeSpent; set => SetField(ref _selectedTimeSpent, value); }

    private string? _selectedStatusName;
    public string? SelectedStatusName { get => _selectedStatusName; set => SetField(ref _selectedStatusName, value); }

    private string _comments = "";
    public string Comments { get => _comments; set => SetField(ref _comments, value); }

    private DateTime? _futureAction;
    public DateTime? FutureAction { get => _futureAction; set => SetField(ref _futureAction, value); }

    private string _saveMessage = "";
    public string SaveMessage { get => _saveMessage; set => SetField(ref _saveMessage, value); }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public EditHistoryViewModel(int? logId = null, string? caseNumberForNew = null)
    {
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => CanDelete);

        BindLists();

        _arrivedViaEdit = logId.HasValue;

        if (logId.HasValue)
        {
            LoadRecord(logId.Value);
        }
        else if (!string.IsNullOrEmpty(caseNumberForNew))
        {
            var parts = CaseNumberHelper.Parse(caseNumberForNew);
            CountyLetter = parts.CountyLetter;
            CaseYear = parts.CaseYear;
            CaseTypeCode = parts.CaseTypeCode;
            CaseSequence = parts.Sequence;
            DefendantSuffix = parts.Suffix;

            var existing = _historyRepo.GetHistoryByCaseNumber(caseNumberForNew).FirstOrDefault();
            if (existing != null) CaseName = existing.PartyName;
        }
    }

    private void BindLists()
    {
        foreach (var c in new CaseTypeRepository().GetAll()) CaseTypes.Add(c);
        foreach (var r in new RequestorRepository().GetAll().OrderByDescending(r => r.IsActive == true).ThenBy(r => r.RequestorName))
            Requestors.Add(r);
        foreach (var a in new AttorneyRepository().GetAll().OrderByDescending(a => a.IsActive == true).ThenBy(a => a.AttorneyName))
            Attorneys.Add(a);
        foreach (var c in new CountyRepository().GetAll()) Counties.Add(c);
        foreach (var a in new ActionTakenRepository().GetAll()) Actions.Add(a);
        foreach (var t in new TimeSpentRepository().GetAll().OrderByDescending(t => t.IsActive).ThenBy(t => t.TimeSpanId))
            TimeSpents.Add(t);
    }

    private void LoadRecord(int logId)
    {
        var item = _historyRepo.GetHistory(logId);
        if (item == null) return;

        LogId = item.LogId;
        DateReceived = item.DateReceived;

        var parts = CaseNumberHelper.Parse(item.CaseNumber);
        CountyLetter = parts.CountyLetter;
        CaseYear = parts.CaseYear;
        CaseTypeCode = parts.CaseTypeCode;
        CaseSequence = parts.Sequence;
        DefendantSuffix = parts.Suffix;

        CaseName = item.PartyName;
        SelectedCaseType = CaseTypes.FirstOrDefault(c => c.CaseType == item.CaseType);
        SelectedRequestor = Requestors.FirstOrDefault(r => r.RequestorName == item.RequestedBy);
        SelectedAttorney = Attorneys.FirstOrDefault(a => a.AttorneyName == item.Responsible);
        MotionFiled = item.MotionFiled;
        SelectedCounty = Counties.FirstOrDefault(c => c.County == item.County);
        SelectedAction = Actions.FirstOrDefault(a => a.Action == item.Action);
        DateCompleted = item.DateCompleted;
        SelectedTimeSpent = TimeSpents.FirstOrDefault(t => t.TimeSpan == item.TimeSpent);
        SelectedStatusName = item.StatusName;
        Comments = item.Comments ?? "";
    }

    private bool Validate()
    {
        ValidationErrors.Clear();
        if (DateReceived == null) ValidationErrors.Add("Action Date is required.");
        if (string.IsNullOrEmpty(CountyLetter)) ValidationErrors.Add("County is required.");
        if (string.IsNullOrWhiteSpace(CaseYear)) ValidationErrors.Add("Case Year is required.");
        if (string.IsNullOrWhiteSpace(CaseTypeCode)) ValidationErrors.Add("Case Type is required.");
        if (string.IsNullOrWhiteSpace(CaseSequence)) ValidationErrors.Add("Case Sequence is required.");
        if (string.IsNullOrWhiteSpace(CaseName)) ValidationErrors.Add("Case Name is required.");
        if (SelectedCaseType == null) ValidationErrors.Add("Case Type is required.");
        if (SelectedRequestor == null) ValidationErrors.Add("Requested By is required.");
        if (SelectedAttorney == null) ValidationErrors.Add("Responsible / Attorney is required.");
        if (MotionFiled == null) ValidationErrors.Add("Date Motion Filed Required");
        if (SelectedCounty == null) ValidationErrors.Add("Please Select County");
        return ValidationErrors.Count == 0;
    }

    private void Save()
    {
        if (!Validate()) return;

        var caseNumber = CaseNumberHelper.Build(CountyLetter, CaseYear, CaseTypeCode, CaseSequence, DefendantSuffix);

        var item = new HistoryInfo
        {
            LogId = LogId,
            DateReceived = DateReceived!.Value,
            CaseNumber = caseNumber,
            PartyName = CaseName,
            CaseType = SelectedCaseType!.CaseType,
            RequestedBy = SelectedRequestor!.RequestorName,
            Responsible = SelectedAttorney!.AttorneyName,
            County = SelectedCounty!.County,
            Action = SelectedAction?.Action,
            DateCompleted = DateCompleted,
            TimeSpent = SelectedTimeSpent?.TimeSpan,
            Comments = Comments,
            StatusName = SelectedStatusName,
            MotionFiled = MotionFiled
        };

        if (LogId > 0)
        {
            _historyRepo.UpdateHistory(item);
            SaveMessage = "Update saved successfully.";
        }
        else
        {
            _historyRepo.CreateHistory(item);
            LogId = item.LogId;
            SaveMessage = "Record created successfully.";
        }

        if (FutureAction.HasValue)
        {
            var clone = new HistoryInfo
            {
                CaseNumber = item.CaseNumber,
                PartyName = item.PartyName,
                CaseType = item.CaseType,
                RequestedBy = item.RequestedBy,
                Responsible = item.Responsible,
                County = item.County,
                MotionFiled = item.MotionFiled,
                StatusName = "Inactive",
                DateReceived = FutureAction.Value
            };
            _historyRepo.CreateHistory(clone);
            FutureAction = null;
        }
    }

    private void Cancel()
    {
        if (_arrivedViaEdit)
        {
            var caseNumber = CaseNumberHelper.Build(CountyLetter, CaseYear, CaseTypeCode, CaseSequence, DefendantSuffix);
            NavigationService.NavigateTo(new CaseHistoryViewModel(caseNumber: caseNumber));
        }
        else
        {
            NavigationService.NavigateTo(new SearchViewModel());
        }
    }

    private void Delete()
    {
        if (!CanDelete) return;
        var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        var row = _historyRepo.GetHistory(LogId);
        var caseNumber = row?.CaseNumber ?? CaseNumberHelper.Build(CountyLetter, CaseYear, CaseTypeCode, CaseSequence, DefendantSuffix);
        _historyRepo.DeleteHistory(LogId);
        NavigationService.NavigateTo(new CaseHistoryViewModel(caseNumber: caseNumber));
    }
}
