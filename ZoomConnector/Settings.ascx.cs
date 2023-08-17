/*
' Copyright (c) 2020  Joe Terhune
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

namespace tjc.Modules.ZoomConnector
{
    public partial class Settings : ZoomConnectorModuleSettingsBase
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

                    if (Settings.Contains("ManateeIPs"))
                        txtManatee.Text = Settings["ManateeIPs"].ToString();
                    if (Settings.Contains("SarasotaIPs"))
                        txtSarasota.Text = Settings["SarasotaIPs"].ToString();
                    if (Settings.Contains("DeSotoIPs"))
                        txtDeSoto.Text = Settings["DeSotoIPs"].ToString();

                    if (Settings.Contains("ConnectorIP"))
                        txtManateeConnectorIP.Text = Settings["ConnectorIP"].ToString();
                    if (Settings.Contains("SarasotaConnectorIP"))
                        txtSarasotaConnectorIP.Text = Settings["SarasotaConnectorIP"].ToString();
                    if (Settings.Contains("DeSotoConnectorIP"))
                        txtDeSotoConnectorIP.Text = Settings["DeSotoConnectorIP"].ToString();


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

                modules.UpdateTabModuleSetting(TabModuleId, "ManateeIPs", txtManatee.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "SarasotaIPs", txtSarasota.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "DeSotoIPs", txtDeSoto.Text.Trim());

                modules.UpdateTabModuleSetting(TabModuleId, "ConnectorIP", txtManateeConnectorIP.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "SarasotaConnectorIP", txtSarasotaConnectorIP.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "DeSotoConnectorIP", txtDeSotoConnectorIP.Text.Trim());

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}