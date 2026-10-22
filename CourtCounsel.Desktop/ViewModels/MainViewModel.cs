using System.Windows;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

public class MainViewModel : ViewModelBase
{
    private object _currentViewModel = null!;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetField(ref _currentViewModel, value);
    }

    public bool IsAdmin => AuthorizationService.IsAdmin;
    public bool CanEditSettings => AuthorizationService.CanEditSettings;

    public RelayCommand GoSearchCommand { get; }
    public RelayCommand GoDataEntryCommand { get; }
    public RelayCommand GoReportsCommand { get; }
    public RelayCommand GoDataSheetCommand { get; }
    public RelayCommand GoAdminCommand { get; }
    public RelayCommand GoSettingsCommand { get; }
    public RelayCommand GoUpdateCaseNameCommand { get; }

    public MainViewModel()
    {
        NavigationService.NavigationRequested += vm => CurrentViewModel = vm;

        GoSearchCommand = new RelayCommand(() => CurrentViewModel = TryCreate(() => new SearchViewModel()));
        GoDataEntryCommand = new RelayCommand(() => CurrentViewModel = TryCreate(() => new EditHistoryViewModel()));
        GoReportsCommand = new RelayCommand(() => CurrentViewModel = TryCreate(() => new ReportsViewModel()));
        GoDataSheetCommand = new RelayCommand(() => CurrentViewModel = TryCreate(() => new DataSheetViewModel()));
        GoAdminCommand = new RelayCommand(() => CurrentViewModel = TryCreate(() => new AdminViewModel()), () => IsAdmin);
        GoSettingsCommand = new RelayCommand(() => CurrentViewModel = new SettingsViewModel(), () => CanEditSettings);
        GoUpdateCaseNameCommand = new RelayCommand(() => CurrentViewModel = new UpdateCaseNameViewModel());

        CurrentViewModel = TryCreate(PickStartupViewModel);
    }

    // Most screens query the database as they're constructed, so a bad
    // connection string or missing table would otherwise throw straight out
    // of a ViewModel constructor — during startup that happens before the
    // window exists and kills the app outright. Report the failure and land
    // somewhere usable instead: Settings for a domain admin (so the
    // connection string can actually be corrected), otherwise the
    // "not configured" screen.
    private object TryCreate(Func<object> factory)
    {
        try
        {
            return factory();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Court Counsel — Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return CanEditSettings ? new SettingsViewModel() : new NotConfiguredViewModel();
        }
    }

    // No connection string yet: an admin lands on Settings to configure it;
    // anyone else (the normal case — settings should already be provisioned
    // before an ordinary user ever runs this) gets a plain "not configured"
    // message instead of Settings, which they can't use anyway and wouldn't
    // be able to save even if they saw it.
    private object PickStartupViewModel()
    {
        if (!string.IsNullOrWhiteSpace(AppSettingsService.Current.ConnectionString))
            return new SearchViewModel();

        return CanEditSettings ? new SettingsViewModel() : new NotConfiguredViewModel();
    }
}
