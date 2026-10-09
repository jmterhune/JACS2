using System.Collections.ObjectModel;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors UpdateCaseName.ascx.cs — bulk-rename a case's party name across
// every log row for that case number. No permission gate in the original
// (only the nav link is hidden for non-admins), so none here either.
public class UpdateCaseNameViewModel : ViewModelBase
{
    private readonly HistoryRepository _repo = new();

    private string _caseNumberInput = "";
    public string CaseNumberInput { get => _caseNumberInput; set => SetField(ref _caseNumberInput, value); }

    private bool _hasResults;
    public bool HasResults { get => _hasResults; set => SetField(ref _hasResults, value); }

    public ObservableCollection<HistoryInfo> Results { get; } = new();

    private string _newCaseName = "";
    public string NewCaseName { get => _newCaseName; set => SetField(ref _newCaseName, value); }

    private string _message = "";
    public string Message { get => _message; set => SetField(ref _message, value); }

    public RelayCommand FindCommand { get; }
    public RelayCommand UpdateCommand { get; }

    public UpdateCaseNameViewModel()
    {
        FindCommand = new RelayCommand(Find);
        UpdateCommand = new RelayCommand(Update);
    }

    private void Find()
    {
        var caseNumber = CaseNumberInput.Trim();
        if (string.IsNullOrEmpty(caseNumber)) return;

        var rows = _repo.GetHistoryByCaseNumber(caseNumber).ToList();
        Results.Clear();
        if (rows.Count > 0)
        {
            foreach (var r in rows) Results.Add(r);
            HasResults = true;
            Message = "";
        }
        else
        {
            HasResults = false;
            Message = $"No records found for case number: {caseNumber}";
        }
    }

    private void Update()
    {
        var caseNumber = CaseNumberInput.Trim();
        var newName = NewCaseName.Trim();
        if (string.IsNullOrEmpty(caseNumber) || string.IsNullOrEmpty(newName)) return;

        _repo.UpdateCaseName(caseNumber, newName);

        Results.Clear();
        foreach (var r in _repo.GetHistoryByCaseNumber(caseNumber)) Results.Add(r);
        HasResults = Results.Count > 0;
        Message = "Case name updated successfully.";
        NewCaseName = "";
    }
}
