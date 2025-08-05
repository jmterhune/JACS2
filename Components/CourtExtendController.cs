// Filename: CourtExtendController.cs
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs
{
    public class CourtExtendController
    {
        public bool HasPermission(int userId, int courtId)
        {
            try
            {
                var courtController = new CourtController();
                var court = courtController.GetCourt(courtId);
                if (court == null)
                    return false;

                var permissionController = new CourtPermissionController();
                var judge = court.GetJudge();
                return permissionController.HasCourtPermission(userId, judge?.id ?? 0) ||DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo()?.IsInRole("Administrators") == true;
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return false;
            }
        }
    }
}