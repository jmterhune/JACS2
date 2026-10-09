using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.ThreatReport.Components
{
    /// <summary>
    /// Incident notification recipients are the members of the "Incident Reporter" role on the jud12
    /// (external) DNN site, whose database this module already reads through the Jud12 connection.
    /// The intranet site has its own users and roles, so membership is read from and changed in the
    /// jud12 site's Users, Roles and UserRoles tables rather than the intranet's.
    /// </summary>
    class IncidentNotificationController
    {
        private const string CONN_JUD12 = "Jud12";
        
        private readonly IHostSettings _hostSettings;

        public IncidentNotificationController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
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

        // ---- the intranet site's own role of the same name -------------------------------------------

        /// <summary>Everyone who should be notified: members of <paramref name="jud12Role"/> on the jud12 site plus
        /// members of <paramref name="localRole"/> on this (intranet) site, with duplicate addresses removed.</summary>
        public List<string> GetNotificationEmails(int portalId, string localRole, string jud12Role)
        {
            var emails = new List<string>(GetSubscriberEmails(jud12Role));
            foreach (UserInfo user in RoleController.Instance.GetUsersByRole(portalId, localRole))
            {
                if (!string.IsNullOrWhiteSpace(user.Email)) emails.Add(user.Email.Trim());
            }
            return emails.Select(e => e.Trim())
                         .Where(e => e.Length > 0)
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .ToList();
        }
    }
}
