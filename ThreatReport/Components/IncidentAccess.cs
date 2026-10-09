using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System.Collections;

namespace tjc.Modules.ThreatReport.Components
{
    /// <summary>
    /// Who may see the incidents and who may submit one. Viewing is limited to the Incident Viewer role;
    /// submitting is open to Incident Viewer and Judges. Administrators and host users are always allowed.
    /// The role names are module settings, so they can be changed per module.
    /// </summary>
    static class IncidentAccess
    {
        public const string DefaultViewerRole = "Incident Viewer";
        public const string DefaultJudgesRole = "Judges";

        public static string GetSetting(Hashtable settings, string key, string fallback)
        {
            if (settings != null && settings.Contains(key) && settings[key] != null)
            {
                string value = settings[key].ToString().Trim();
                if (value.Length > 0) return value;
            }
            return fallback;
        }

        public static string ViewerRole(Hashtable settings)
        {
            return GetSetting(settings, "ViewerRole", DefaultViewerRole);
        }

        public static string JudgesRole(Hashtable settings)
        {
            return GetSetting(settings, "JudgesRole", DefaultJudgesRole);
        }

        public static bool IsAdministrator(UserInfo user, int portalId)
        {
            if (user == null || user.UserID <= 0) return false;
            if (user.IsSuperUser) return true;
            PortalInfo portal = PortalController.Instance.GetPortal(portalId);
            return portal != null && user.IsInRole(portal.AdministratorRoleName);
        }

        public static bool CanView(UserInfo user, int portalId, Hashtable settings)
        {
            if (user == null || user.UserID <= 0) return false;
            return IsAdministrator(user, portalId) || user.IsInRole(ViewerRole(settings));
        }

        public static bool CanSubmit(UserInfo user, int portalId, Hashtable settings)
        {
            if (!CanView(user, portalId, settings))
            {
                return user != null && user.UserID > 0 && user.IsInRole(JudgesRole(settings));
            }
            return true;
        }
    }
}
