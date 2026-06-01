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

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class Settings : EmployeeDBModuleSettingsBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Don't bind roles here — DNN's lifecycle calls LoadSettings either
            // before or after Page_Load depending on version, and binding twice
            // wipes out the SelectedValue set by LoadSettings. All settings
            // initialization happens in LoadSettings instead.
        }

        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    BindRoles();

                    // Read from the merged Settings hash (covers both ModuleSettings
                    // and TabModuleSettings). UpdateSettings writes via
                    // UpdateModuleSetting, so reading TabModuleSettings alone
                    // never round-trips the value.
                    if (Settings.Contains("Employee_ReportUrl"))
                        txtReportUrl.Text = Settings["Employee_ReportUrl"].ToString();

                    if (Settings.Contains("HrAdminRole"))
                    {
                        var roleName = Settings["HrAdminRole"].ToString();
                        var item = drpHrAdminRole.Items.FindByValue(roleName);
                        if (item != null)
                            drpHrAdminRole.SelectedValue = roleName;
                    }

                    if (Settings.Contains("Swn_TestUsername"))
                        txtSwnTestUsername.Text = Settings["Swn_TestUsername"].ToString();
                    if (Settings.Contains("Swn_TestPassword"))
                        txtSwnTestPassword.Attributes["value"] = Settings["Swn_TestPassword"].ToString();
                    if (Settings.Contains("Swn_LiveUsername"))
                        txtSwnLiveUsername.Text = Settings["Swn_LiveUsername"].ToString();
                    if (Settings.Contains("Swn_LivePassword"))
                        txtSwnLivePassword.Attributes["value"] = Settings["Swn_LivePassword"].ToString();
                    if (Settings.Contains("Swn_UseLive"))
                    {
                        bool useLive;
                        if (bool.TryParse(Settings["Swn_UseLive"].ToString(), out useLive))
                            chkSwnUseLive.Checked = useLive;
                    }

                    if (Settings.Contains("Notify_FromEmail"))
                        txtNotifyFrom.Text = Settings["Notify_FromEmail"].ToString();
                    if (Settings.Contains("Notify_ToEmail"))
                        txtNotifyTo.Text = Settings["Notify_ToEmail"].ToString();
                    if (Settings.Contains("Notify_OnSave"))
                    {
                        bool notify;
                        if (bool.TryParse(Settings["Notify_OnSave"].ToString(), out notify))
                            chkNotifyOnSave.Checked = notify;
                    }
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Employee_ReportUrl", txtReportUrl.Text.Trim());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "HrAdminRole", drpHrAdminRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Swn_TestUsername", txtSwnTestUsername.Text.Trim());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Swn_TestPassword", txtSwnTestPassword.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Swn_LiveUsername", txtSwnLiveUsername.Text.Trim());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Swn_LivePassword", txtSwnLivePassword.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Swn_UseLive", chkSwnUseLive.Checked.ToString());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Notify_FromEmail", txtNotifyFrom.Text.Trim());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Notify_ToEmail", txtNotifyTo.Text.Trim());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "Notify_OnSave", chkNotifyOnSave.Checked.ToString());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindRoles()
        {
            var roleCtrl = new RoleController();
            var roles = roleCtrl.GetRoles(PortalId)
                                .Cast<RoleInfo>()
                                .OrderBy(r => r.RoleName)
                                .ToList();

            drpHrAdminRole.Items.Clear();
            drpHrAdminRole.Items.Add(new ListItem("-- Select Role --", ""));
            foreach (var role in roles)
            {
                drpHrAdminRole.Items.Add(new ListItem(role.RoleName, role.RoleName));
            }
        }
    }
}
