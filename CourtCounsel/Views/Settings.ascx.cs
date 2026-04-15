/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class Settings : CourtCounselModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    BindRoles();

                    if (Settings.Contains("AdminRole"))
                    {
                        var adminRole = Settings["AdminRole"].ToString();
                        if (drpAdminRole.Items.FindByValue(adminRole) != null)
                            drpAdminRole.SelectedValue = adminRole;
                    }

                    if (Settings.Contains("template"))
                    {
                        txtTemplate.Text = Settings["template"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            try
            {
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "AdminRole", drpAdminRole.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "template", txtTemplate.Text);
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindRoles()
        {
            var roleCtrl = new RoleController();
            var roles = roleCtrl.GetRoles(PortalId).Cast<RoleInfo>().OrderBy(r => r.RoleName).ToList();

            drpAdminRole.Items.Clear();
            drpAdminRole.Items.Add(new ListItem("-- Select Role --", ""));
            foreach (var role in roles)
            {
                drpAdminRole.Items.Add(new ListItem(role.RoleName, role.RoleName));
            }
        }
    }
}
