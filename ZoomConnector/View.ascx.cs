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

using DotNetNuke.Abstractions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI.WebControls;

namespace tjc.Modules.ZoomConnector
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ZoomConnectorModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : ZoomConnectorModuleBase
    {

        #region Members
        private readonly INavigationManager _navigationManager;

        private string[] ManateeList
        {
            get
            {
                string _list = "";
                if (Settings.Contains("ManateeIPs"))
                    _list = Settings["ManateeIPs"].ToString();
                return _list.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private string[] SarasotaList
        {
            get
            {
                string _list = "";
                if (Settings.Contains("SarasotaIPs"))
                    _list = Settings["SarasotaIPs"].ToString();
                return _list.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private string[] DeSotoList
        {
            get
            {
                string _list = "";
                if (Settings.Contains("DeSotoIPs"))
                    _list = Settings["DeSotoIPs"].ToString();
                return _list.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #endregion

        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void AddListItems(string[] lis)
        {
            drpLocation.Items.Clear();
            foreach (string s in lis)
            {
                var li = s.Split(':');
                if (li.Length == 2)
                    drpLocation.Items.Add(new ListItem(s.Split(':')[1], s.Split(':')[0]));
            }
            drpLocation.Items.Insert(0, new ListItem("< Select PolyCom >", ""));
        }

        private void ConnectPolycom(string connector,string polycom, string polyPassword, string meetingId, string meetingPassword)
        {
            string readOut = "";
            int count = 1;
            Components.TelnetClient client = new Components.TelnetClient(polycom, 24);
            while (client.IsConnected == false)
            {
                System.Threading.Thread.Sleep(1000);
                count += 1;
                if (count >= 5) { break; }
            }
            count = 0;
            if (!client.IsConnected)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "Could Not Connect to Polycom", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }
            while (readOut.ToLower().Contains("password") == false)
            {
                System.Threading.Thread.Sleep(1000);
                readOut = client.Read();
                count += 1;
                if (count >= 5) { break; }

            }
            count = 0;

            if (readOut.ToLower().Contains("password"))
            {
                client.WriteLine(polyPassword);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "Could Not Send Password", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }
            readOut = "";

            while (readOut.ToLower().Contains("hi") == false)
            {
                System.Threading.Thread.Sleep(1000);
                readOut = client.Read();
                count += 1;
                if (count >= 5) { break; }
            }
            count = 0;
            readOut = "";
            client.WriteLine("getcallstate");
            while (readOut.ToLower().Contains("connected") == false)
            {
                System.Threading.Thread.Sleep(500);
                readOut = client.Read();
                count += 1;
                if (count >= 5) { break; }
            }
            if (readOut.Contains("connected"))
            {
                ltMessages.Text = "<div class='alert alert-danger'><em class='fa fa-warning'></em> The Zoom Connector is already in a call.  Please disconnect the current call before attempting again.</div>";
                return;
            }
            count = 0;
            readOut = "";
            string connectorString = string.Format("Dial auto 384 {0}##{1}#{2}", connector, meetingId, meetingPassword);
            client.WriteLine(connectorString);

            while (readOut.Length == 0)
            {
                System.Threading.Thread.Sleep(1000);
                readOut = client.Read();
                count += 1;
                if (count >= 5) { break; }
            }
            count = 0;
            readOut = "";
            client.WriteLine("getcallstate");
            while (readOut.ToLower().Contains("connected") == false)
            {
                System.Threading.Thread.Sleep(500);
                readOut = client.Read();
                count += 1;
                if (count >= 5) { break; }
            }
            if (readOut.Contains("connected"))
            {
                Response.Redirect(_navigationManager.NavigateURL() + "?s=1", true);
            }
            else
            {
                Response.Redirect(_navigationManager.NavigateURL() + "?s=0", true);
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                var qs = Request.QueryString["s"];
                if (qs != null)
                {
                    if (Success)
                    {
                        ltMessages.Text = "<div class='alert alert-success'><em class='fa fa-thumbs-up'></em> Connected!</div>";
                    }
                    else
                    {
                        ltMessages.Text = "<div class='alert alert-danger'><em class='fa fa-warning'></em> Connection Status Undetermined. Please check if the connection was successful. If not, please wait for 30 seconds and then reattempt. </div>";
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void drpCounty_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpLocation.Enabled = drpCounty.SelectedValue.Length > 0;
            if (drpCounty.SelectedValue == "m")
            {
                if (ManateeList.Length > 0)
                {
                    AddListItems(ManateeList);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "No PolyCom units specified for Manatee", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;
                }
            }
            else if (drpCounty.SelectedValue == "s")
            {
                if (SarasotaList.Length > 0)
                {
                    AddListItems(SarasotaList);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "No PolyCom units specified for Sarasota", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;
                }

            }
            else if (drpCounty.SelectedValue == "d")
            {
                if (DeSotoList.Length > 0)
                {
                    AddListItems(DeSotoList);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "No PolyCom units specified for DeSoto", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;
                }
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            string county = drpCounty.SelectedItem.Text.Trim().ToLower();
            string connector = "";
            if (county.Length > 0)
            {
                switch (county)
                {
                    case "manatee":
                        connector= ManateeConnectorIP;
                        break;
                    case "sarasota":
                        connector = SarasotaConnectorIP;
                        break;
                    case "desoto":
                        connector = DeSotoConnectorIP;
                        break;
                    default:
                        break;
                }
               
                string polycom = drpLocation.SelectedValue.Split('|')[0];
                string polyPassword = drpLocation.SelectedValue.Split('|')[1];
                if (txtMeetingInfo.Text.Trim().Length == 0)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "Please Paste in the Meeting Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;

                }
                string[] meetingInfo = txtMeetingInfo.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string meetingId = "";
                string meetingPassword = "";
                foreach (var s in meetingInfo)
                {
                    if (s.ToLower().StartsWith("meeting"))
                        meetingId = s.Split(':')[1].Trim().Replace(" ", "");
                    if (s.ToLower().StartsWith("password"))
                        meetingPassword = s.Split(':')[1].Trim().Replace(" ", "");
                    if (s.ToLower().StartsWith("passcode"))
                        meetingPassword = s.Split(':')[1].Trim().Replace(" ", "");

                    if (meetingId != "" & meetingPassword != "")
                        break;
                }

                ConnectPolycom(connector,polycom, polyPassword, meetingId, meetingPassword);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this.Page, "Please Select a County", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }

        }
    }
}