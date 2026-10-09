using System.Runtime.InteropServices;
using System.Security.Principal;
using CourtCounsel.Desktop.Data;

namespace CourtCounsel.Desktop.Services;

// Two separate, unrelated authorization questions live here:
//
//   IsAdmin         — the app-level admin role, a Windows/AD group named in
//                     Settings. Gates only the Admin (lookup-management)
//                     screen, exactly like the DNN module's IsAdmin.
//   CanEditSettings — gates the Settings screen itself. Can't use IsAdmin
//                     for this (Settings is where that group name is
//                     configured, so it would be circular), so it asks a
//                     fixed question instead: is this user an administrator?
public static class AuthorizationService
{
    public static bool IsAdmin
    {
        get
        {
            var groupName = AppSettingsService.Current.AdminGroupName;
            if (string.IsNullOrWhiteSpace(groupName)) return true;

            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(groupName);
            }
            catch
            {
                return false;
            }
        }
    }

    // True for a member of the local Administrators group or of Domain
    // Admins, whether or not the app is running elevated.
    //
    // The "whether or not elevated" part is why this isn't a one-liner. UAC
    // strips Administrators/Domain Admins out of the process token for a
    // non-elevated run, and .NET also drops those deny-only SIDs from
    // WindowsIdentity.Groups — so every straightforward check reports false
    // for an admin who simply double-clicked the app. Asking the OS for the
    // *linked* (elevated) token and inspecting that instead gives the real
    // answer, so admins don't have to right-click → Run as administrator
    // just to open Settings. A standard user has no linked token, which is
    // the correct "false".
    public static bool CanEditSettings
    {
        get
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                return HasAdminMembership(identity) || LinkedTokenHasAdminMembership(identity);
            }
            catch
            {
                return false;
            }
        }
    }

    private static bool HasAdminMembership(WindowsIdentity identity)
    {
        var principal = new WindowsPrincipal(identity);
        if (principal.IsInRole(WindowsBuiltInRole.Administrator)) return true;

        // 512 is the well-known RID for "Domain Admins" in every AD domain,
        // so matching on the SID suffix avoids hard-coding a domain name.
        return identity.Groups?.Any(g => g.Value.EndsWith("-512", StringComparison.Ordinal)) ?? false;
    }

    private const uint TokenLinkedToken = 19;

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool GetTokenInformation(
        IntPtr tokenHandle, uint tokenInformationClass, IntPtr tokenInformation,
        uint tokenInformationLength, out uint returnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    private static bool LinkedTokenHasAdminMembership(WindowsIdentity identity)
    {
        var buffer = Marshal.AllocHGlobal(IntPtr.Size);
        var linkedToken = IntPtr.Zero;
        try
        {
            if (!GetTokenInformation(identity.AccessToken.DangerousGetHandle(), TokenLinkedToken,
                    buffer, (uint)IntPtr.Size, out _))
                return false;

            linkedToken = Marshal.ReadIntPtr(buffer);
            if (linkedToken == IntPtr.Zero) return false;

            using var elevated = new WindowsIdentity(linkedToken);
            return HasAdminMembership(elevated);
        }
        catch
        {
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
            if (linkedToken != IntPtr.Zero) CloseHandle(linkedToken);
        }
    }
}
