using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.ProcessServer
{
    /// <summary>
    /// Module settings for the internal Process Server admin module: the roles that gate the
    /// management and approval screens (chosen from the site's roles), the notification from-address,
    /// and the public applicant page linked in the approve/reject emails. The enrollment window and
    /// payment-gateway settings stay on the public module.
    /// </summary>
    public partial class Settings : ProcessServerModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindRoles();
                    SelectRole(drpAdminRole, Setting("AdminRole"));
                    SelectRole(drpChiefJudgeRole, Setting("ChiefJudgeRole"));
                    SelectRole(drpCourtCounselRole, Setting("CourtCounselRole"));
                    txtNotificationEmail.Text = Setting("NotificationEmail");
                    if (Settings.Contains(ProcessServerModuleBase.ExternalUrlSetting))
                        txtExternalUrl.Text = Settings[ProcessServerModuleBase.ExternalUrlSetting].ToString();
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
                SaveRole("AdminRole", drpAdminRole);
                SaveRole("ChiefJudgeRole", drpChiefJudgeRole);
                SaveRole("CourtCounselRole", drpCourtCounselRole);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "NotificationEmail", txtNotificationEmail.Text.Trim());

                string url = txtExternalUrl.Text.Trim();
                if (url.Length == 0)
                    ModuleController.Instance.DeleteModuleSetting(ModuleId, ProcessServerModuleBase.ExternalUrlSetting);
                else if (ProcessServerModuleBase.ValidExternalUrl(url) != null)
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, ProcessServerModuleBase.ExternalUrlSetting, url);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valExternalUrl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ProcessServerModuleBase.ValidExternalUrl(args.Value) != null;
        }

        private string Setting(string key)
        {
            return Settings.Contains(key) ? Settings[key].ToString() : "";
        }

        private void BindRoles()
        {
            var roles = RoleController.Instance.GetRoles(PortalId).Cast<RoleInfo>().OrderBy(r => r.RoleName).ToList();
            foreach (var drp in new[] { drpAdminRole, drpChiefJudgeRole, drpCourtCounselRole })
            {
                drp.Items.Clear();
                drp.Items.Add(new ListItem("-- Select Role --", ""));
                foreach (var role in roles)
                    drp.Items.Add(new ListItem(role.RoleName, role.RoleName));
            }
        }

        // Selects a stored role, adding it as an item first if it is no longer in the site's role list,
        // so an older setting never throws and stays visible until the admin picks another.
        private static void SelectRole(DropDownList drp, string value)
        {
            value = (value ?? "").Trim();
            if (value.Length == 0) return;
            ListItem item = drp.Items.FindByValue(value) ?? new ListItem(value, value);
            if (!drp.Items.Contains(item)) drp.Items.Add(item);
            drp.ClearSelection();
            item.Selected = true;
        }

        // An empty selection removes the setting so the module base falls back to its default role,
        // rather than storing a blank role that would gate everyone out.
        private void SaveRole(string key, DropDownList drp)
        {
            string value = drp.SelectedValue;
            if (string.IsNullOrEmpty(value))
                ModuleController.Instance.DeleteModuleSetting(ModuleId, key);
            else
                ModuleController.Instance.UpdateModuleSetting(ModuleId, key, value);
        }
    }
}
