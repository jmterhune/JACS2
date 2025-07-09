/*
' Copyright (c) 2025  Joe Terhune
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
using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;

namespace tjc.Modules.jacs
{
    public partial class Settings : JACSModuleSettingsBase
    {
        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    DotNetNuke.Security.Roles.RoleController ctl = new DotNetNuke.Security.Roles.RoleController();
                    var listroles = ctl.GetRoles(PortalId);
                    foreach (DotNetNuke.Security.Roles.RoleInfo r in listroles)
                    {
                        drpJugeRole.Items.Add(new ListItem(r.RoleName));
                        drpAdminRole.Items.Add(new ListItem(r.RoleName));
                        drpJaRole.Items.Add(new ListItem(r.RoleName));
                        drpJacsUserRole.Items.Add(new ListItem(r.RoleName));
                    }
                    if (Settings.Contains("JudgeRole"))
                        drpJugeRole.SelectedValue = Convert.ToString(Settings["JudgeRole"]);
                    if (Settings.Contains("JacsUserRole"))
                        drpJacsUserRole.SelectedValue = Convert.ToString(Settings["JacsUserRole"]);
                    if (Settings.Contains("JaRole"))
                        drpJaRole.SelectedValue = Convert.ToString(Settings["JaRole"]);
                    if (Settings.Contains("AdminRole"))
                        drpAdminRole.SelectedValue = Convert.ToString(Settings["AdminRole"]);
                    if (Settings.Contains("QuickRefUrl"))
                    {
                        txtQuickRefUrl.Text = Settings["QuickRefUrl"].ToString();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                string JudgeRole = drpJugeRole.SelectedValue;
                string AdminRole = drpAdminRole.SelectedValue;
                string JaRole = drpJaRole.SelectedValue;
                string JacsUserRole = drpJacsUserRole.SelectedValue;
                var modules = new ModuleController();
                if (!string.IsNullOrEmpty(AdminRole.Trim()))
                    modules.UpdateModuleSetting(ModuleId, "AdminRole", AdminRole.Trim());
                if (!string.IsNullOrEmpty(JudgeRole.Trim()))
                    modules.UpdateModuleSetting(ModuleId, "JudgeRole", JudgeRole.Trim());
                if (!string.IsNullOrEmpty(JaRole.Trim()))
                    modules.UpdateModuleSetting(ModuleId, "JaRole", JaRole.Trim());
                if (!string.IsNullOrEmpty(JacsUserRole.Trim()))
                    modules.UpdateModuleSetting(ModuleId, "JacsUserRole", JacsUserRole.Trim());
                modules.UpdateModuleSetting(ModuleId, "QuickRefUrl", txtQuickRefUrl.Text);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}