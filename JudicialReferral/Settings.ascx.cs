/*
' Copyright (c) 2022  Joe Terhune
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
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.JudicialReferral
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
    /// Because the control inherits from JudicialReferralSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : JudicialReferralModuleSettingsBase
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
                    foreach (DotNetNuke.Security.Roles.RoleInfo r in listroles.OrderBy(jud => jud.RoleName)
)
                    {
                        drpJudgeRole.Items.Add(new ListItem(r.RoleName));
                        drpJaRole.Items.Add(new ListItem(r.RoleName));
                        drpCourtCounsel.Items.Add(new ListItem(r.RoleName));
                    }

                    if (Settings.Contains("JudgeRole"))
                        drpJudgeRole.SelectedValue = Settings["JudgeRole"].ToString();
                    if (Settings.Contains("JaRole"))
                        drpJaRole.SelectedValue = Settings["JaRole"].ToString();
                    if (Settings.Contains("CounselRole"))
                        drpCourtCounsel.SelectedValue = Settings["CounselRole"].ToString();
                    if (Settings.Contains("CourtCounselEmail"))
                        txtCounselEmail.Text = Settings["CourtCounselEmail"].ToString();
                    if (Settings.Contains("CourtCounselUrl"))
                        txtCourtCounselRefferalUrl.Text = Settings["CourtCounselUrl"].ToString();
                    if (Settings.Contains("FolderName"))
                        txtFolderName.Text = Settings["FolderName"].ToString();
                    if (Settings.Contains("ccModuleId"))
                        txtModuleId.Text = Settings["ccModuleId"].ToString();

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
                var modules = new ModuleController();

                modules.UpdateModuleSetting(ModuleId, "JudgeRole", drpJudgeRole.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, "JaRole", drpJaRole.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, "CounselRole", drpCourtCounsel.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, "FolderName", txtFolderName.Text);
                modules.UpdateModuleSetting(ModuleId, "CourtCounselEmail", txtCounselEmail.Text);
                modules.UpdateModuleSetting(ModuleId, "CourtCounselUrl", txtCourtCounselRefferalUrl.Text);
                modules.UpdateModuleSetting(ModuleId, "ccModuleId", txtModuleId.Text);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}