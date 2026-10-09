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
    enum SubscriptionState
    {
        /// <summary>The notification role does not exist on the jud12 site.</summary>
        NoRole,
        /// <summary>No jud12 site account matches the intranet user.</summary>
        NoAccount,
        /// <summary>More than one jud12 site account matches, so it is unclear which one to change.</summary>
        Ambiguous,
        NotSubscribed,
        Subscribed
    }

    class SubscriptionInfo
    {
        public SubscriptionState State;
        public int UserId;
        public int RoleId;
    }

    /// <summary>
    /// Incident notification recipients are the members of the "Incident Reporter" role on the jud12
    /// (external) DNN site, whose database this module already reads through the Jud12 connection.
    /// The intranet site has its own users and roles, so membership is read from and changed in the
    /// jud12 site's Users, Roles and UserRoles tables rather than the intranet's.
    /// </summary>
    class IncidentNotificationController
    {
        private const string CONN_JUD12 = "Jud12";
        public const string DefaultRoleName = "Incident Reporter";

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

        /// <summary>
        /// Finds the jud12 site account for an intranet user (by username, then by email) and whether
        /// that account is in the role.
        /// </summary>
        public SubscriptionInfo GetSubscription(string username, string email, string roleName)
        {
            var info = new SubscriptionInfo();
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                info.RoleId = ctx.ExecuteQuery<int>(CommandType.Text,
                    "SELECT TOP 1 RoleID FROM Roles WHERE RoleName = @0 ORDER BY RoleID", roleName).FirstOrDefault();
                if (info.RoleId == 0)
                {
                    info.State = SubscriptionState.NoRole;
                    return info;
                }

                List<int> users = new List<int>();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    users = ctx.ExecuteQuery<int>(CommandType.Text,
                        "SELECT UserID FROM Users WHERE IsDeleted = 0 AND Username = @0", username).ToList();
                }
                if (users.Count == 0 && !string.IsNullOrWhiteSpace(email))
                {
                    users = ctx.ExecuteQuery<int>(CommandType.Text,
                        "SELECT UserID FROM Users WHERE IsDeleted = 0 AND Email = @0", email).ToList();
                }
                if (users.Count == 0)
                {
                    info.State = SubscriptionState.NoAccount;
                    return info;
                }
                if (users.Count > 1)
                {
                    info.State = SubscriptionState.Ambiguous;
                    return info;
                }

                info.UserId = users[0];
                int active = ctx.ExecuteQuery<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM UserRoles WHERE UserID = @0 AND RoleID = @1 AND Status = 1 " +
                    "AND (ExpiryDate IS NULL OR ExpiryDate > GETDATE())", info.UserId, info.RoleId).FirstOrDefault();
                info.State = active > 0 ? SubscriptionState.Subscribed : SubscriptionState.NotSubscribed;
                return info;
            }
        }

        public void Subscribe(SubscriptionInfo info, int changedByUserId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                // Reactivate an expired or unauthorized membership, otherwise add one.
                ctx.Execute(CommandType.Text,
                    "UPDATE UserRoles SET Status = 1, ExpiryDate = NULL, EffectiveDate = NULL, " +
                    "LastModifiedByUserID = @2, LastModifiedOnDate = GETDATE() WHERE UserID = @0 AND RoleID = @1",
                    info.UserId, info.RoleId, changedByUserId);
                ctx.Execute(CommandType.Text,
                    "INSERT INTO UserRoles (UserID, RoleID, ExpiryDate, IsTrialUsed, EffectiveDate, CreatedByUserID, " +
                    "CreatedOnDate, LastModifiedByUserID, LastModifiedOnDate, Status, IsOwner) " +
                    "SELECT @0, @1, NULL, 0, NULL, @2, GETDATE(), @2, GETDATE(), 1, 0 " +
                    "WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserID = @0 AND RoleID = @1)",
                    info.UserId, info.RoleId, changedByUserId);
            }
        }

        public void Unsubscribe(SubscriptionInfo info)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.Execute(CommandType.Text,
                    "DELETE FROM UserRoles WHERE UserID = @0 AND RoleID = @1", info.UserId, info.RoleId);
            }
        }

        // ---- the intranet site's own role of the same name -------------------------------------------

        /// <summary>Everyone who should be notified: members of the role on the jud12 site plus members of the
        /// local (intranet) role, with duplicate addresses removed.</summary>
        public List<string> GetNotificationEmails(int portalId, string roleName)
        {
            var emails = new List<string>(GetSubscriberEmails(roleName));
            foreach (UserInfo user in RoleController.Instance.GetUsersByRole(portalId, roleName))
            {
                if (!string.IsNullOrWhiteSpace(user.Email)) emails.Add(user.Email.Trim());
            }
            return emails.Select(e => e.Trim())
                         .Where(e => e.Length > 0)
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .ToList();
        }

        public bool IsInLocalRole(int portalId, int userId, string roleName)
        {
            RoleInfo role = RoleController.Instance.GetRoleByName(portalId, roleName);
            if (role == null) return false;
            UserRoleInfo userRole = RoleController.Instance.GetUserRole(portalId, userId, role.RoleID);
            return userRole != null && userRole.Status == RoleStatus.Approved
                && (userRole.ExpiryDate == Null.NullDate || userRole.ExpiryDate > DateTime.Now);
        }

        /// <summary>Adds or removes the user from the local role. Returns false when the role does not exist here.</summary>
        public bool SetLocalRole(int portalId, int userId, string roleName, bool member)
        {
            RoleInfo role = RoleController.Instance.GetRoleByName(portalId, roleName);
            if (role == null) return false;
            if (member)
            {
                if (!IsInLocalRole(portalId, userId, roleName))
                {
                    RoleController.Instance.AddUserRole(portalId, userId, role.RoleID, RoleStatus.Approved, false, Null.NullDate, Null.NullDate);
                }
            }
            else
            {
                RoleController.Instance.UpdateUserRole(portalId, userId, role.RoleID, RoleStatus.Approved, false, true);
            }
            return true;
        }
    }
}
