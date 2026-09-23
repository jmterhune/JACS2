using System.Collections.ObjectModel;
using Microsoft.Win32;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors DataSheet.ascx.cs — server-paged grid + filtered Excel export.
public class DataSheetViewModel : ViewModelBase
{
    private readonly HistoryRepository _repo = new();

    public ObservableCollection<AttorneyCheckItem> AttorneyChecks { get; } = new();

    private DateTime? _dateReceivedFrom;
    public DateTime? DateReceivedFrom { get => _dateReceivedFrom; set => SetField(ref _dateReceivedFrom, value); }

    public ObservableCollection<string> RequestedByOptions { get; } = new();
    private string _selectedRequestedBy = "";
    public string SelectedRequestedBy { get => _selectedRequestedBy; set => SetField(ref _selectedRequestedBy, value); }

    public static readonly TextValueOption[] CompletedFilterOptions =
    {
        new("Include Completed", "include"),
        new("Does Not Include Completed", "exclude"),
    };

    private string _completedFilter = "include";
    public string CompletedFilter { get => _completedFilter; set => SetField(ref _completedFilter, value); }

    public static readonly int[] PageSizeOptions = { 25, 50, 100, 250 };

    private int _pageSize = 50;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetField(ref _pageSize, value))
            {
                CurrentPage = 1;
                Reload();
            }
        }
    }

    private int _currentPage = 1;
    public int CurrentPage { get => _currentPage; private set => SetField(ref _currentPage, value); }

    private long _totalPages;
    public long TotalPages { get => _totalPages; private set => SetField(ref _totalPages, value); }

    private long _totalRecords;
    public long TotalRecords { get => _totalRecords; private set => SetField(ref _totalRecords, value); }

    public string PageInfo => $"Page {CurrentPage} of {Math.Max(TotalPages, 1)}";
    public string TotalInfo => $"{TotalRecords} record(s)";

    public ObservableCollection<HistoryInfo> Rows { get; } = new();

    public RelayCommand FilterCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand FirstPageCommand { get; }
    public RelayCommand PrevPageCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand LastPageCommand { get; }

    public DataSheetViewModel()
    {
        FilterCommand = new RelayCommand(() => { CurrentPage = 1; Reload(); });
        ClearCommand = new RelayCommand(Clear);
        ExportCommand = new RelayCommand(Export);
        FirstPageCommand = new RelayCommand(() => { CurrentPage = 1; Reload(); }, () => CurrentPage > 1);
        PrevPageCommand = new RelayCommand(() => { CurrentPage = Math.Max(1, CurrentPage - 1); Reload(); }, () => CurrentPage > 1);
        NextPageCommand = new RelayCommand(() => { CurrentPage++; Reload(); }, () => CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(() => { CurrentPage = int.MaxValue; Reload(); }, () => CurrentPage < TotalPages);

        foreach (var a in new AttorneyRepository().GetAll().OrderByDescending(a => a.IsActive == true).ThenBy(a => a.AttorneyName))
            AttorneyChecks.Add(new AttorneyCheckItem(a));
        foreach (var r in _repo.GetDistinctRequestedBy()) RequestedByOptions.Add(r);

        Reload();
    }

    private List<string> SelectedAttorneys() => AttorneyChecks.Where(a => a.IsChecked).Select(a => a.Attorney.AttorneyName).ToList();

    private void Reload()
    {
        var page = _repo.GetHistoryPage(
            CurrentPage, PageSize,
            SelectedAttorneys(), DateReceivedFrom, SelectedRequestedBy, CompletedFilter == "exclude");

        Rows.Clear();
        foreach (var r in page.Items) Rows.Add(r);

        CurrentPage = page.PageNumber;
        TotalPages = page.TotalPages;
        TotalRecords = page.TotalItems;
        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(TotalInfo));
    }

    private void Clear()
    {
        foreach (var a in AttorneyChecks) a.IsChecked = false;
        DateReceivedFrom = null;
        SelectedRequestedBy = "";
        CompletedFilter = "include";
        CurrentPage = 1;
        Reload();
    }

    private void Export()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Excel Workbook (*.xls)|*.xls",
            FileName = $"CourtCounsel_DataSheet_{DateTime.Now:yyyyMMdd_HHmmss}.xls"
        };
        if (dialog.ShowDialog() != true) return;

        var rows = _repo.GetHistoryForExport(SelectedAttorneys(), DateReceivedFrom, SelectedRequestedBy, CompletedFilter == "exclude");
        ExcelExportService.ExportDataSheet(rows, dialog.FileName);
    }
}
