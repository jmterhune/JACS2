using System.Collections.ObjectModel;
using System.Windows;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors CaseHistory.ascx.cs — all log rows for one case number.
public class CaseHistoryViewModel : ViewModelBase
{
    private readonly HistoryRepository _repo = new();

    private string _caseNumber;
    public string CaseNumber { get => _caseNumber; private set => SetField(ref _caseNumber, value); }

    public ObservableCollection<string> PartyNames { get; } = new();
    public ObservableCollection<HistoryInfo> HistoryRows { get; } = new();

    public RelayCommand AddNewCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public CaseHistoryViewModel(string? caseNumber = null, int? logId = null)
    {
        if (string.IsNullOrEmpty(caseNumber) && logId.HasValue)
        {
            var row = _repo.GetHistory(logId.Value);
            caseNumber = row?.CaseNumber ?? "";
        }
        _caseNumber = caseNumber ?? "";

        AddNewCommand = new RelayCommand(() =>
            NavigationService.NavigateTo(new EditHistoryViewModel(caseNumberForNew: CaseNumber)));
        EditCommand = new RelayCommand(Edit);
        DeleteCommand = new RelayCommand(Delete);

        BindData();
    }

    private void BindData()
    {
        PartyNames.Clear();
        foreach (var n in _repo.GetPartyNamesByCaseNumber(CaseNumber)) PartyNames.Add(n);

        HistoryRows.Clear();
        var rows = _repo.GetHistoryByCaseNumber(CaseNumber)
            .OrderBy(h => h.DateCompleted.HasValue) // open (null) first
            .ThenByDescending(h => h.DateCompleted)
            .ThenByDescending(h => h.DateReceived);
        foreach (var r in rows) HistoryRows.Add(r);
    }

    private void Edit(object? parameter)
    {
        if (parameter is not HistoryInfo row) return;
        NavigationService.NavigateTo(new EditHistoryViewModel(logId: row.LogId));
    }

    private void Delete(object? parameter)
    {
        if (parameter is not HistoryInfo row) return;
        var result = MessageBox.Show($"Delete this log entry from {row.DateReceived:d}?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        _repo.DeleteHistory(row.LogId);
        BindData();
    }
}
