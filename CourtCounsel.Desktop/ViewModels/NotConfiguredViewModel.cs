namespace CourtCounsel.Desktop.ViewModels;

// Shown to a non-domain-admin user who opens the app before a domain admin
// has provisioned settings.json — there's nothing useful for them to do
// here (Settings is hidden from them by design), so just say so instead of
// leaving them on a blank Search screen that will error the moment it
// tries to query the database.
public class NotConfiguredViewModel : ViewModelBase
{
}
