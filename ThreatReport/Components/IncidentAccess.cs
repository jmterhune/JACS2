using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections;

namespace tjc.Modules.ThreatReport.Components
{
    enum IncidentScope
    {
        /// <summary>Not signed in.</summary>
        None,
        /// <summary>May see only the incidents they submitted.</summary>
        Own,
        /// <summary>May see every incident.</summary>
        All
    }

    /// <summary>
    /// Members of the viewer role (setting ViewerRole, default Incident Viewer) see every incident; any other
    /// signed-in user sees only the incidents they submitted, matched on the creator's email. Anyone signed in can
    /// submit: who can reach the submit page is set with the DNN page and module permissions.
    /// Administrators and host users see everything.
    /// </summary>
    static class IncidentAccess
    {
        public const string DefaultViewerRole = "Incident Viewer";

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

        public static bool IsAdministrator(UserInfo user, int portalId)
        {
            if (user == null || user.UserID <= 0) return false;
            if (user.IsSuperUser) return true;
            PortalInfo portal = PortalController.Instance.GetPortal(portalId);
            return portal != null && user.IsInRole(portal.AdministratorRoleName);
        }

        public static IncidentScope GetScope(UserInfo user, int portalId, Hashtable settings)
        {
            if (user == null || user.UserID <= 0) return IncidentScope.None;
            if (IsAdministrator(user, portalId) || user.IsInRole(ViewerRole(settings))) return IncidentScope.All;
            return IncidentScope.Own;
        }

        /// <summary>Any signed-in user who is allowed to see something: all incidents, or just their own.</summary>
        public static bool CanView(UserInfo user, int portalId, Hashtable settings)
        {
            return GetScope(user, portalId, settings) != IncidentScope.None;
        }

        /// <summary>Any signed-in user. DNN permissions on the submit page decide who can actually get to it.</summary>
        public static bool CanSubmit(UserInfo user, int portalId, Hashtable settings)
        {
            return user != null && user.UserID > 0;
        }

        public static bool Owns(UserInfo user, Incident incident)
        {
            if (user == null || incident == null) return false;
            string mine = (user.Email ?? "").Trim();
            string theirs = (incident.CreatedByEmail ?? "").Trim();
            return mine.Length > 0 && string.Equals(mine, theirs, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CanViewIncident(IncidentScope scope, UserInfo user, Incident incident)
        {
            if (incident == null) return false;
            return scope == IncidentScope.All || (scope == IncidentScope.Own && Owns(user, incident));
        }
    }
}
