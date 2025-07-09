using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections;

namespace CustomDNNModule
{
    public class UserHelper
    {
        /// <summary>
        /// Retrieves a UserInfo object based on a profile property value
        /// </summary>
        /// <param name="portalId">The ID of the portal</param>
        /// <param name="propertyName">The name of the profile property (case-sensitive)</param>
        /// <param name="propertyValue">The value to match</param>
        /// <returns>UserInfo object if found, null if no match or error</returns>
        public static UserInfo GetUserByProfileProperty(int portalId, string propertyName, string propertyValue)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue))
                {
                    return null;
                }

                // Ensure portalId is valid
                portalId = PortalController.GetEffectivePortalId(portalId);

                // Get users by profile property
                int totalRecords = 0;
                ArrayList users = UserController.GetUsersByProfileProperty(
                    portalId,
                    propertyName,
                    propertyValue,
                    0,  // pageIndex
                    1,  // pageSize (we only need one match)
                    ref totalRecords,
                    false,  // includeDeleted
                    false   // superUsersOnly
                );

                // Return first matching user, if any
                if (users != null && users.Count > 0)
                {
                    return users[0] as UserInfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log exception (in a production environment, use DNN logging)
                 DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return null;
            }
        }
    }
}