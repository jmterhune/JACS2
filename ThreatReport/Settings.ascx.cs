using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Module settings shared by the incident list and the submit-incident modules: the role that sees every
    /// incident, the role on the jud12 site that is also notified, and the pages the two modules link to.
    /// </summary>
    public partial class Settings : ThreatReportModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                string viewer = Settings.Contains("ViewerRole") ? Settings["ViewerRole"].ToString() : "";
                string jud12 = Settings.Contains("Jud12Role") ? Settings["Jud12Role"].ToString() : "";

                if (ddlViewerRole.Items.Count == 0)
                {
                    var local = RoleController.Instance.GetRoles(PortalId).Select(r => r.RoleName).OrderBy(n => n).ToList();
                    Fill(ddlViewerRole, local, IncidentAccess.DefaultViewerRole, viewer);

                    List<string> remote = new List<string>();
                    try
                    {
                        remote = new IncidentNotificationController(DependencyProvider.GetRequiredService<IHostSettings>()).GetRoleNames();
                    }
                    catch (Exception ex)
                    {
                        // The jud12 database could not be read; the saved role is still shown and kept.
                        Exceptions.LogException(ex);
                    }
                    Fill(ddlJud12Role, remote, "Incident Reporter", jud12);
                }

                if (!Page.IsPostBack)
                {
                    if (Settings.Contains("ViewTabID")) txtViewTab.Text = Settings["ViewTabID"].ToString();
                    if (Settings.Contains("EditTabID")) txtEditTab.Text = Settings["EditTabID"].ToString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        // A blank first item means "use the default". A saved role that is no longer in the list is kept
        // as an item so saving the page does not silently change it.
        private static void Fill(DropDownList list, List<string> roles, string defaultName, string saved)
        {
            list.Items.Add(new ListItem("(default: " + defaultName + ")", ""));
            foreach (string role in roles) list.Items.Add(new ListItem(role, role));
            saved = (saved ?? "").Trim();
            if (saved.Length > 0)
            {
                ListItem item = list.Items.FindByValue(saved);
                if (item == null)
                {
                    item = new ListItem(saved + " (not found)", saved);
                    list.Items.Add(item);
                }
                list.ClearSelection();
                item.Selected = true;
            }
        }

        public override void UpdateSettings()
        {
            try
            {
                var modules = ModuleController.Instance;
                modules.UpdateTabModuleSetting(TabModuleId, "ViewerRole", ddlViewerRole.SelectedValue);
                modules.UpdateTabModuleSetting(TabModuleId, "Jud12Role", ddlJud12Role.SelectedValue);
                modules.UpdateTabModuleSetting(TabModuleId, "ViewTabID", txtViewTab.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "EditTabID", txtEditTab.Text.Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
