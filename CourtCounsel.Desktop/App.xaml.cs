using System.Windows;
using System.Windows.Threading;

namespace CourtCounsel.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Subscribe BEFORE base.OnStartup: that call is what creates the
        // StartupUri window (and MainViewModel with it), so anything
        // thrown during startup would escape a handler wired afterwards.
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        base.OnStartup(e);
    }

    // Any unhandled exception (most commonly a database connection failure —
    // wrong connection string, server unreachable, table missing) shows a
    // message instead of crashing the app, so the user can get to Settings
    // and fix the connection string rather than losing all their work.
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.Message,
            "Court Counsel — Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}
