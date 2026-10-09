using System.Collections.ObjectModel;
using System.Windows;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

public enum SearchType
{
    CaseName = 1,
    CaseNumber = 2,
    Attorney = 3
}

// Mirrors Search.ascx.cs — the module's landing page / three search modes.
public class SearchViewModel : ViewModelBase
{
    private readonly AttorneyRepository _attorneyRepo = new();

    private SearchType _searchType = SearchType.CaseName;
    public SearchType SearchType
    {
        get => _searchType;
        set
        {
            if (SetField(ref _searchType, value))
            {
                OnPropertyChanged(nameof(IsAttorneySearch));
                OnPropertyChanged(nameof(IsTermSearch));
            }
        }
    }

    public bool IsAttorneySearch => SearchType == SearchType.Attorney;
    public bool IsTermSearch => SearchType != SearchType.Attorney;

    private string _searchTerm = "";
    public string SearchTerm { get => _searchTerm; set => SetField(ref _searchTerm, value); }

    public ObservableCollection<AttorneyInfo> Attorneys { get; } = new();

    private AttorneyInfo? _selectedAttorney;
    public AttorneyInfo? SelectedAttorney { get => _selectedAttorney; set => SetField(ref _selectedAttorney, value); }

    private bool _includeActive = true;
    public bool IncludeActive { get => _includeActive; set => SetField(ref _includeActive, value); }

    private bool _includePending;
    public bool IncludePending { get => _includePending; set => SetField(ref _includePending, value); }

    private bool _includeClosed;
    public bool IncludeClosed { get => _includeClosed; set => SetField(ref _includeClosed, value); }

    public RelayCommand SearchCommand { get; }

    public SearchViewModel()
    {
        SearchCommand = new RelayCommand(Search);

        foreach (var a in _attorneyRepo.GetAll().OrderByDescending(a => a.IsActive == true).ThenBy(a => a.AttorneyName))
            Attorneys.Add(a);
    }

    private void Search()
    {
        switch (SearchType)
        {
            case SearchType.CaseName:
                if (string.IsNullOrWhiteSpace(SearchTerm)) return;
                NavigationService.NavigateTo(new CaseListViewModel(partyName: SearchTerm));
                break;

            case SearchType.CaseNumber:
                if (string.IsNullOrWhiteSpace(SearchTerm)) return;
                NavigationService.NavigateTo(new CaseListViewModel(caseNumber: SearchTerm));
                break;

            case SearchType.Attorney:
                if (SelectedAttorney == null)
                {
                    MessageBox.Show("Select an attorney.", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var sf = (IncludeActive ? "A" : "") + (IncludePending ? "I" : "") + (IncludeClosed ? "C" : "");
                if (string.IsNullOrEmpty(sf)) sf = "all";
                NavigationService.NavigateTo(new AttorneyCaseListViewModel(SelectedAttorney.AttorneyName, sf));
                break;
        }
    }
}
