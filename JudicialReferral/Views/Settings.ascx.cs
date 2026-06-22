/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class Settings : JudicialReferralSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    BindRoleDropDowns();

                    if (Settings.Contains("JudgeRole"))
                        SelectRole(drpJudgeRole, Settings["JudgeRole"].ToString());
                    if (Settings.Contains("JaRole"))
                        SelectRole(drpJaRole, Settings["JaRole"].ToString());
                    if (Settings.Contains("CounselRole"))
                        SelectRole(drpCounselRole, Settings["CounselRole"].ToString());
                    if (Settings.Contains("CounselAdminRole"))
                        SelectRole(drpCounselAdminRole, Settings["CounselAdminRole"].ToString());
                    if (Settings.Contains("CourtCounselEmail"))
                        txtCounselEmail.Text = Settings["CourtCounselEmail"].ToString();
                    if (Settings.Contains("FolderName"))
                        txtFolderName.Text = Settings["FolderName"].ToString();
                }
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "JudgeRole", drpJudgeRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "JaRole", drpJaRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "CounselRole", drpCounselRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "CounselAdminRole", drpCounselAdminRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "CourtCounselEmail", txtCounselEmail.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "FolderName", txtFolderName.Text);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindRoleDropDowns()
        {
            var roleCtrl = new RoleController();
            var roles = roleCtrl.GetRoles(PortalId)
                                .Cast<RoleInfo>()
                                .OrderBy(r => r.RoleName)
                                .ToList();

            foreach (var ddl in new[] { drpJudgeRole, drpJaRole, drpCounselRole, drpCounselAdminRole })
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("-- Select Role --", ""));
                foreach (var role in roles)
                {
                    ddl.Items.Add(new ListItem(role.RoleName, role.RoleName));
                }
            }
        }

        private static void SelectRole(DropDownList ddl, string roleName)
        {
            var item = ddl.Items.FindByValue(roleName);
            if (item != null)
                ddl.SelectedValue = roleName;
        }
    }
}
