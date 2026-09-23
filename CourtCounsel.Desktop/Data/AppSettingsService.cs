namespace CourtCounsel.Desktop.Data;

// Process-wide holder for the loaded settings, so every repository/service
// can read Current without threading a settings object through every
// constructor. Reassign Current after saving new values from the Settings
// screen.
public static class AppSettingsService
{
    public static AppSettings Current { get; private set; } = AppSettings.Load();

    public static void Save(AppSettings settings)
    {
        settings.Save();
        Current = settings;
    }
}
