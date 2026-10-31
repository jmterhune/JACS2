/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;

namespace tjc.Modules.CourtCalendar
{
    /// <summary>
    /// Module settings: which DNN role identifies judges and which identifies
    /// judicial assistants. The Manage Judges picker lists users in these roles.
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        public const string JudgeRoleSetting = "JudgeRole";
        public const string JaRoleSetting = "JaRole";
        public const string PublicCalendarUrlSetting = "PublicCalendarUrl";

        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack) return;
                BindRoles(ddlJudgeRole);
                BindRoles(ddlJaRole);
                if (Settings.Contains(JudgeRoleSetting)) Select(ddlJudgeRole, Settings[JudgeRoleSetting].ToString());
                if (Settings.Contains(JaRoleSetting)) Select(ddlJaRole, Settings[JaRoleSetting].ToString());
                if (Settings.Contains(PublicCalendarUrlSetting)) txtPublicUrl.Text = Settings[PublicCalendarUrlSetting].ToString();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            try
            {
                var modules = ModuleController.Instance;
                modules.UpdateModuleSetting(ModuleId, JudgeRoleSetting, ddlJudgeRole.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, JaRoleSetting, ddlJaRole.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, PublicCalendarUrlSetting, (txtPublicUrl.Text ?? string.Empty).Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindRoles(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("< Select Role >", string.Empty));
            IList<RoleInfo> roles = RoleController.Instance.GetRoles(PortalId);
            foreach (var role in roles)
                ddl.Items.Add(new ListItem(role.RoleName, role.RoleName));
        }

        private static void Select(DropDownList ddl, string value)
        {
            var item = ddl.Items.FindByValue(value);
            if (item != null) ddl.SelectedValue = value;
        }
    }
}
