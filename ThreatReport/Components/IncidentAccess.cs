using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    /// signed-in user sees only the incidents they submitted, matched on the creator's email. Viewers and the
    /// submitter roles (setting SubmitterRoles, a comma-separated list, default Judges and Judicial Assistant)
    /// can submit. Administrators and host users can do everything.
    /// </summary>
    static class IncidentAccess
    {
        public const string DefaultViewerRole = "Incident Viewer";
        public const string DefaultSubmitterRoles = "Judges,Judicial Assistant";

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

        public static List<string> SubmitterRoles(Hashtable settings)
        {
            // JudgesRole is the name this setting had before it became a list.
            string value = GetSetting(settings, "SubmitterRoles", GetSetting(settings, "JudgesRole", DefaultSubmitterRoles));
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim())
                        .Where(r => r.Length > 0)
                        .ToList();
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

        public static bool CanSubmit(UserInfo user, int portalId, Hashtable settings)
        {
            if (user == null || user.UserID <= 0) return false;
            if (GetScope(user, portalId, settings) == IncidentScope.All) return true;
            return SubmitterRoles(settings).Any(r => user.IsInRole(r));
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
