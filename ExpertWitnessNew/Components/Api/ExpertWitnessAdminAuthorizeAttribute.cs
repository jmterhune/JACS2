/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using System;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>
    /// Authorizes the admin list endpoints for a page/module editor (DNN Edit
    /// permission) OR a member of the role configured in the module's "AdminRole"
    /// setting. This mirrors <see cref="ExpertWitnessModuleBase.IsAdmin"/>, which the
    /// UI uses to decide who sees the admin tabs, so the tabs and the API agree on
    /// who may manage the lists. (Replaces a plain [DnnModuleAuthorize(Edit)], which
    /// only honored DNN module permissions and denied the configured AdminRole.)
    /// </summary>
    public class ExpertWitnessAdminAuthorizeAttribute : AuthorizeAttributeBase
    {
        public override bool IsAuthorized(AuthFilterContext context)
        {
            var module = context.ActionContext.Request.FindModuleInfo();
            if (module == null)
                return false;

            var user = UserController.Instance.GetCurrentUserInfo();
            if (user == null || user.UserID <= 0)
                return false;

            if (user.IsSuperUser)
                return true;

            // Page / module editors keep the access they had before this change.
            if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", module))
                return true;

            // The role chosen in the module's Settings ("AdminRole") may manage the lists.
            var adminRole = module.ModuleSettings["AdminRole"] as string;
            return !string.IsNullOrWhiteSpace(adminRole) && user.IsInRole(adminRole);
        }
    }
}
