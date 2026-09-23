using System.Collections.ObjectModel;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors CaseList.ascx.cs — search-results grid for a case-name or
// case-number substring search.
public class CaseListViewModel : ViewModelBase
{
    private readonly HistoryRepository _repo = new();

    public string HeadingText { get; }
    public ObservableCollection<HistoryInfo> Results { get; } = new();

    public RelayCommand OpenCaseCommand { get; }

    public CaseListViewModel(string? caseNumber = null, string? partyName = null)
    {
        OpenCaseCommand = new RelayCommand(OpenCase);

        IEnumerable<HistoryInfo> rows;
        if (!string.IsNullOrEmpty(caseNumber))
        {
            HeadingText = "Case Number Search Results";
            rows = _repo.SearchByCaseNumber(caseNumber);
        }
        else
        {
            HeadingText = "Case Name Search Results";
            rows = _repo.SearchByCaseName(partyName ?? "");
        }

        foreach (var r in rows) Results.Add(r);
    }

    private void OpenCase(object? parameter)
    {
        if (parameter is not HistoryInfo row) return;
        NavigationService.NavigateTo(new CaseHistoryViewModel(caseNumber: row.CaseNumber));
    }
}
