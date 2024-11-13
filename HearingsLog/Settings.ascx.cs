/*
' Copyright (c) 2024  Joe Terhune
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

namespace tjc.Modules.HearingLog
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from HearingsLogSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : HearingsLogModuleSettingsBase
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
                        drpRoles.Items.Add(new ListItem(r.RoleName));
                        drpJudgeRole.Items.Add(new ListItem(r.RoleName));
                        drpJaRole.Items.Add(new ListItem(r.RoleName));
                    }
                    drpRoles.Items.Insert(0, new ListItem("< Select Role >", ""));
                    drpJudgeRole.Items.Insert(0, new ListItem("< Select Role >", ""));
                    drpJaRole.Items.Insert(0, new ListItem("< Select Role >", ""));
                    if (Settings.Contains("ChiefJudgeRole"))
                        drpRoles.SelectedValue = Convert.ToString(Settings["ChiefJudgeRole"]);
                    if (Settings.Contains("JudgeRole"))
                        drpJudgeRole.SelectedValue = Convert.ToString(Settings["JudgeRole"]);
                    if (Settings.Contains("JaRole"))
                        drpJaRole.SelectedValue = Convert.ToString(Settings["JaRole"]);
                    if (Settings.Contains("PageUrl"))
                       txtUrl.Text = Convert.ToString(Settings["PageUrl"]);
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
                ModuleController objModules = new ModuleController();
                string ChiefJudgeRole = drpRoles.SelectedValue;
                string JudgeRole = drpJudgeRole.SelectedValue;
                string JaRole = drpJaRole.SelectedValue;
                string PageUrl=txtUrl.Text;
                if (!string.IsNullOrEmpty(ChiefJudgeRole.Trim()))
                    objModules.UpdateModuleSetting(ModuleId, "ChiefJudgeRole", ChiefJudgeRole.Trim());
                if (!string.IsNullOrEmpty(JudgeRole.Trim()))
                    objModules.UpdateModuleSetting(ModuleId, "JudgeRole", JudgeRole.Trim());
                if (!string.IsNullOrEmpty(JaRole.Trim()))
                    objModules.UpdateModuleSetting(ModuleId, "JaRole", JaRole.Trim());
                if (!string.IsNullOrEmpty(PageUrl.Trim()))
                    objModules.UpdateModuleSetting(ModuleId, "PageUrl", PageUrl.Trim());

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}