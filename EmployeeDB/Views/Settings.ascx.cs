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
            try
            {
                if (!IsPostBack)
                {
                    BindRoles();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    BindRoles();

                    if (TabModuleSettings.Contains("Employee_ReportUrl"))
                        txtReportUrl.Text = TabModuleSettings["Employee_ReportUrl"].ToString();

                    if (TabModuleSettings.Contains("HrAdminRole"))
                    {
                        var roleName = TabModuleSettings["HrAdminRole"].ToString();
                        var item = drpHrAdminRole.Items.FindByValue(roleName);
                        if (item != null)
                            drpHrAdminRole.SelectedValue = roleName;
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
