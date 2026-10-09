using System.Collections.ObjectModel;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors AttorneyCaseList.ascx.cs.
public class AttorneyCaseListViewModel : ViewModelBase
{
    private readonly HistoryRepository _repo = new();

    public string HeadingText { get; }
    public bool ShowCompletedColumn { get; }
    public ObservableCollection<HistoryInfo> Results { get; } = new();

    public RelayCommand OpenCaseCommand { get; }

    public AttorneyCaseListViewModel(string attorneyName, string statusFilter)
    {
        OpenCaseCommand = new RelayCommand(OpenCase);

        HeadingText = $"Cases for {attorneyName}";
        ShowCompletedColumn = statusFilter.Contains('C', StringComparison.OrdinalIgnoreCase)
            || string.Equals(statusFilter, "all", StringComparison.OrdinalIgnoreCase);

        foreach (var r in _repo.SearchByAttorney(attorneyName, statusFilter)) Results.Add(r);
    }

    private void OpenCase(object? parameter)
    {
        if (parameter is not HistoryInfo row) return;
        NavigationService.NavigateTo(new CaseHistoryViewModel(caseNumber: row.CaseNumber));
    }
}
