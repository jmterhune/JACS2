using System.Windows;
using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Desktop equivalent of Settings.ascx.cs — the DNN module's Settings panel.
// AdminGroupName replaces AdminRole (a Windows/AD group instead of a DNN
// portal role); TemplateText carries over even though nothing reads it,
// same as in the web module.
public class SettingsViewModel : ViewModelBase
{
    // MainWindow hides the Settings nav button and MainViewModel won't
    // navigate here at all unless AuthorizationService.CanEditSettings, but
    // this instance could still be constructed directly (tests, a future
    // nav path) — refuse to persist changes either way rather than relying
    // solely on the UI not offering the option.
    public bool CanEditSettings => AuthorizationService.CanEditSettings;

    private string _connectionString;
    public string ConnectionString { get => _connectionString; set => SetField(ref _connectionString, value); }

    private string _adminGroupName;
    public string AdminGroupName { get => _adminGroupName; set => SetField(ref _adminGroupName, value); }

    private string _templateText;
    public string TemplateText { get => _templateText; set => SetField(ref _templateText, value); }

    private string _statusMessage = "";
    public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }

    public RelayCommand SaveCommand { get; }
    public RelayCommand TestConnectionCommand { get; }

    public SettingsViewModel()
    {
        var current = AppSettingsService.Current;
        _connectionString = current.ConnectionString;
        _adminGroupName = current.AdminGroupName;
        _templateText = current.TemplateText;

        SaveCommand = new RelayCommand(Save);
        TestConnectionCommand = new RelayCommand(TestConnection);
    }

    private void Save()
    {
        if (!CanEditSettings)
        {
            StatusMessage = "Only an administrator can change these settings.";
            return;
        }

        try
        {
            AppSettingsService.Save(new AppSettings
            {
                ConnectionString = ConnectionString,
                AdminGroupName = AdminGroupName,
                TemplateText = TemplateText
            });
            StatusMessage = "Settings saved.";
        }
        catch (UnauthorizedAccessException)
        {
            // The settings file lives under ProgramData, which IT may have
            // ACL-restricted to administrators. Membership is enough to
            // reach this screen, but writing through a restricted ACL needs
            // the elevated token.
            StatusMessage = "Windows denied access to the settings file. Re-run this app as administrator " +
                            "(right-click → Run as administrator) and save again.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Could not save settings: " + ex.Message;
        }
    }

    private void TestConnection()
    {
        try
        {
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString);
            conn.Open();
            StatusMessage = "Connection succeeded.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Connection failed: " + ex.Message;
        }
    }
}
