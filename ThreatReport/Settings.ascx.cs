/*
' Copyright (c) 2019  jud12
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
using System.IO;
using System.Web.UI.WebControls;

namespace tjc.Modules.ThreatReport
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
    /// Because the control inherits from ThreatReportSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : ThreatReportModuleSettingsBase
    {
        #region Base Method Implementations

        private void BindList()
        {
            DotNetNuke.Security.Roles.RoleController ctl = new DotNetNuke.Security.Roles.RoleController();
            var listroles = ctl.GetRoles(PortalId);
            foreach (DotNetNuke.Security.Roles.RoleInfo r in listroles)
            {
                drpRole.Items.Add(new ListItem(r.RoleName));
            }
            drpRole.Items.Insert(0, new ListItem("< Select Role >", ""));
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                BindList();
                if (Page.IsPostBack == false)
                {
                    //Check for existing settings and use those on this page
                    //Settings["SettingName"]

                    if (Settings.Contains("AttachmentDirectory"))
                    {
                        txtAttachmentDirectory.Text = Settings["AttachmentDirectory"].ToString();
                    }


                    if (Settings.Contains("EditTabID"))
                    {
                        txtTabID.Text = Settings["EditTabID"].ToString();
                    }
                    if (Settings.Contains("ViewTabID"))
                    {
                        txtViewTabID.Text = Settings["ViewTabID"].ToString();
                    }
                    if (Settings.Contains("ViewerRole"))
                    {
                        drpRole.SelectedValue = Settings["ViewerRole"].ToString();
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
                var modules = new ModuleController();

                //the following are two sample Module Settings, using the text boxes that are commented out in the ASCX file.
                //module settings
                //modules.UpdateModuleSetting(ModuleId, "Setting1", txtSetting1.Text);
                //modules.UpdateModuleSetting(ModuleId, "Setting2", txtSetting2.Text);

                //tab module settings
                string physicalDirectory = txtAttachmentDirectory.Text.ToString();
                if (physicalDirectory != "")
                {
                    DirectoryInfo dir = new DirectoryInfo(physicalDirectory);
                    if (!dir.Exists)
                    {
                        try
                        {
                            dir.Create();
                        }
                        catch
                        {
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to create directory. Please create manually insuring that the app pool for this site has read/write priveledges.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                            return;
                        }
                    }

                }
                modules.UpdateTabModuleSetting(TabModuleId, "AttachmentDirectory", txtAttachmentDirectory.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "EditTabID", txtTabID.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "ViewTabID", txtViewTabID.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "ViewerRole", drpRole.SelectedValue);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}