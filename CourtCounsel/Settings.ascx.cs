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

namespace tjc.Modules.CourtCounsel
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
    /// Because the control inherits from CourtCounselSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : CourtCounselModuleSettingsBase
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
                    if (Settings.Contains("AdminRole"))
                        txtAdminRole.Text = Settings["AdminRole"].ToString();
                    if (Settings.Contains("DefaultReminderPeriod"))
                        txtDefaultReminderPeriod.Text = Settings["DefaultReminderPeriod"].ToString();
                    if (Settings.Contains("SharePointSiteURL"))
                        txtSharePointSiteURL.Text = Settings["SharePointSiteURL"].ToString();
                    //SharePoint Config
                    if (Settings.Contains("Id"))
                        txtId.Text = Settings["Id"].ToString();
                   
                    if (Settings.Contains("DocumentLibraryURL"))
                        txtDocumentLibraryURL.Text = Settings["DocumentLibraryURL"].ToString();
                    if (Settings.Contains("DocumentDriveId"))
                        txtDocumentDriveId.Text = Settings["DocumentDriveId"].ToString();
                    if (Settings.Contains("OrderDriveId"))
                        txtOrderDriveId.Text = Settings["OrderDriveId"].ToString();
                    if (Settings.Contains("SharePointSiteURL"))
                        txtSharePointSiteURL.Text = Settings["SharePointSiteURL"].ToString();
                    if (Settings.Contains("OrderPath"))
                        txtOrderPath.Text = Settings["OrderPath"].ToString();
                    if (Settings.Contains("GraphConfig"))
                        txtGraphConfig.Text = Settings["GraphConfig"].ToString();
                    if (Settings.Contains("DocumentLibraryName"))
                        txtDocumentLibraryName.Text = Settings["DocumentLibraryName"].ToString();
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
                modules.UpdateModuleSetting(ModuleId, "AdminRole", txtAdminRole.Text);
                modules.UpdateModuleSetting(ModuleId, "DefaultReminderPeriod", txtDefaultReminderPeriod.Text);
                modules.UpdateModuleSetting(ModuleId, "SharePointSiteURL", txtSharePointSiteURL.Text);

                modules.UpdateModuleSetting(ModuleId, "Id", txtId.Text);
                modules.UpdateModuleSetting(ModuleId, "DocumentLibraryURL", txtDocumentLibraryURL.Text);
                modules.UpdateModuleSetting(ModuleId, "DocumentDriveId", txtDocumentDriveId.Text);
                modules.UpdateModuleSetting(ModuleId, "DocumentLibraryName", txtDocumentLibraryName.Text);
                modules.UpdateModuleSetting(ModuleId, "OrderDriveId", txtOrderDriveId.Text);
                modules.UpdateModuleSetting(ModuleId, "SharePointSiteURL", txtSharePointSiteURL.Text);
                modules.UpdateModuleSetting(ModuleId, "OrderPath", txtOrderPath.Text);
                modules.UpdateModuleSetting(ModuleId, "GraphConfig", txtGraphConfig.Text);

                //tab module settings
                //modules.UpdateTabModuleSetting(TabModuleId, "Setting1",  txtSetting1.Text);
                //modules.UpdateTabModuleSetting(TabModuleId, "Setting2",  txtSetting2.Text);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}