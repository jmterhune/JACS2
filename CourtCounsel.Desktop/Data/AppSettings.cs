using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.Data;

// Local replacement for the DNN module's two module settings
// ("AdminRole" / "template" in CourtCounselModuleBase). AdminGroupName is
// the load-bearing one: it names a Windows/AD group whose members are
// treated as admins (gates only the Admin lookup-management screen, exactly
// like the web module's IsAdmin). TemplateText is carried over for parity
// but nothing in this app reads it either, same as the original.
//
// Stored machine-wide (ProgramData, not per-user AppData) and edited only
// through the Settings screen, which is itself gated to administrators
// (AuthorizationService.CanEditSettings) — see MainViewModel/SettingsViewModel.
// That combination is what makes "install with the settings already set"
// work: an admin configures this once, and the resulting settings.json can
// be dropped into %ProgramData%\CourtCounselDesktop\ on every target
// machine as part of deployment; ordinary end users never see an editable
// Settings screen to change it.
public class AppSettings
{
    [JsonIgnore]
    public string ConnectionString { get; set; } = "";

    // The only field persisted for ConnectionString — see
    // ConnectionStringProtector for what "encrypted" does and doesn't
    // protect against.
    public string ConnectionStringProtected { get; set; } = "";

    public string AdminGroupName { get; set; } = "";
    public string TemplateText { get; set; } = "";

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "CourtCounselDesktop", "settings.json");

    // Where this app stored settings before the ProgramData/encryption
    // change (per-user, plain text). Read-only fallback so a connection
    // string already configured under the old scheme keeps working instead
    // of silently reverting to "not configured" — it does NOT get written
    // back here automatically; only an admin saving from the Settings
    // screen produces a new machine-wide, encrypted file.
    private static readonly string LegacySettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CourtCounselDesktop", "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    loaded.ConnectionString = ConnectionStringProtector.Unprotect(loaded.ConnectionStringProtected);
                    return loaded;
                }
            }

            if (File.Exists(LegacySettingsPath))
            {
                var json = File.ReadAllText(LegacySettingsPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                return new AppSettings
                {
                    ConnectionString = GetString(root, "ConnectionString"),
                    AdminGroupName = GetString(root, "AdminGroupName"),
                    TemplateText = GetString(root, "TemplateText")
                };
            }
        }
        catch
        {
            // Fall through to defaults — a corrupt settings file shouldn't
            // prevent the app from starting; a domain admin can re-enter
            // values on the Settings screen.
        }
        return new AppSettings();
    }

    private static string GetString(JsonElement root, string propertyName) =>
        root.TryGetProperty(propertyName, out var value) ? value.GetString() ?? "" : "";

    public void Save()
    {
        ConnectionStringProtected = ConnectionStringProtector.Protect(ConnectionString);

        var dir = Path.GetDirectoryName(SettingsPath)!;
        Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }
}
