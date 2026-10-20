using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.ThreatReport.Components
{
    /// <summary>
    /// Incident notification recipients are the members of a role on the jud12 (external) DNN site, whose
    /// database this module already reads through the Jud12 connection. The intranet site has its own users and
    /// roles, so the members are read from the jud12 site's Users, Roles and UserRoles tables.
    /// </summary>
    class IncidentNotificationController
    {
        private const string CONN_JUD12 = "Jud12";
        
        private readonly IHostSettings _hostSettings;

        public IncidentNotificationController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>Names of the roles that exist on the jud12 site, for the settings dropdown.</summary>
        public List<string> GetRoleNames()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<string>(CommandType.Text,
                    "SELECT RoleName FROM Roles ORDER BY RoleName").ToList();
            }
        }

        /// <summary>Distinct email addresses of the active members of the role on the jud12 site.</summary>
        public List<string> GetSubscriberEmails(string roleName)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<string>(CommandType.Text,
                    "SELECT DISTINCT u.Email FROM Users u " +
                    "JOIN UserRoles ur ON ur.UserID = u.UserID " +
                    "JOIN Roles r ON r.RoleID = ur.RoleID " +
                    "WHERE r.RoleName = @0 AND u.IsDeleted = 0 AND ur.Status = 1 " +
                    "AND (ur.ExpiryDate IS NULL OR ur.ExpiryDate > GETDATE()) " +
                    "AND u.Email IS NOT NULL AND u.Email <> ''", roleName)
                    .ToList();
            }
        }

    }
}
