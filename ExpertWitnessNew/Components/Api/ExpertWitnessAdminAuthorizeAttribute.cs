/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Net.Http;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>
    /// Authorizes the admin list endpoints for a page/module editor (DNN Edit
    /// permission) OR a member of the role configured in the module's "AdminRole"
    /// setting -- matching <see cref="ExpertWitnessModuleBase.IsAdmin"/>, which the UI
    /// uses to decide who sees the admin tabs.
    ///
    /// Runs as a Web API authorization filter. The module is resolved from the
    /// ModuleId / TabId headers that ew-core.js sends: DnnApiController.ActiveModule /
    /// HttpRequestMessage.FindModuleInfo() can resolve a DIFFERENT module here, whose
    /// ModuleSettings then lack "AdminRole" -- which denied legitimate role members
    /// while admins still passed HasModuleAccess on any module. The user is resolved
    /// from the authenticated identity name (GetCurrentUserInfo()/UserInfo can be
    /// anonymous in a filter).
    /// </summary>
    // Implements IOverrideDefaultAuthLevel so DNN does NOT also apply its default Web API
    // authorization on top of this filter. Without it, a member of the AdminRole is still
    // denied ("Authorization has been denied for this request") even when IsAuthorized returns
    // true, because DNN's default level requires the module edit/host access a plain role
    // member lacks. (DNN's own DnnModuleAuthorize/DnnAuthorize implement this marker.)
    public class ExpertWitnessAdminAuthorizeAttribute : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        public override bool IsAuthorized(AuthFilterContext context)
        {
            var request = context.ActionContext.Request;

            int moduleId = GetHeaderInt(request, "ModuleId");
            int tabId = GetHeaderInt(request, "TabId");
            var module = moduleId > 0 ? ModuleController.Instance.GetModule(moduleId, tabId, false) : null;
            if (module == null)
            {
                var apiController = context.ActionContext.ControllerContext.Controller as DnnApiController;
                module = (apiController != null ? apiController.ActiveModule : null) ?? request.FindModuleInfo();
            }
            if (module == null)
                return false;

            // Page/module editors, portal admins, and host.
            if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", module))
                return true;

            // The role configured in this module's "AdminRole" setting.
            var adminRole = Convert.ToString(module.ModuleSettings["AdminRole"]);
            if (string.IsNullOrWhiteSpace(adminRole))
                return false;

            // Resolve the authenticated user (with roles) by username from the module's portal.
            var principal = context.ActionContext.RequestContext.Principal;
            var identity = principal != null ? principal.Identity : null;
            if (identity == null || !identity.IsAuthenticated || string.IsNullOrEmpty(identity.Name))
                return false;

            var user = UserController.GetUserByName(module.PortalID, identity.Name);
            return user != null && user.UserID > 0 && user.IsInRole(adminRole);
        }

        private static int GetHeaderInt(HttpRequestMessage request, string name)
        {
            IEnumerable<string> values;
            if (request != null && request.Headers.TryGetValues(name, out values))
            {
                foreach (var v in values)
                {
                    int n;
                    if (int.TryParse(v, out n))
                        return n;
                }
            }
            return -1;
        }
    }
}
