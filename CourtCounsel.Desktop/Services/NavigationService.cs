namespace CourtCounsel.Desktop.Services;

// Minimal pub/sub so any ViewModel can trigger a screen change (mirroring
// the web module's Response.Redirect(EditUrl(...)) calls) without every
// ViewModel needing a direct reference to MainViewModel.
public static class NavigationService
{
    public static event Action<object>? NavigationRequested;

    public static void NavigateTo(object viewModel) => NavigationRequested?.Invoke(viewModel);
}
