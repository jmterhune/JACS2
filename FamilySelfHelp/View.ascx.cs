/*
' Copyright (c) 2023  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using tjc.Modules.FamilySelfHelp.Components;
namespace tjc.Modules.FamilySelfHelp
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from FamilySelfHelpModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : FamilySelfHelpModuleBase
    {
        public bool hasDelete
        {
            get
            {
                if (ViewState["hasDelete"] != null)
                    return System.Convert.ToBoolean(ViewState["hasDelete"]);
                else
                    return false;
            }
            set
            {
                ViewState["hasDelete"] = value;
            }
        }

        public string HasInterpreter(string isInterpreter)
        {
            if (isInterpreter != "" && bool.Parse(isInterpreter) == true)
                return "Y";
            else
                return "N";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                    
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                if (!IsPostBack)
                {
                    lnkDataEntry.NavigateUrl = EditUrl();
                    lnkEditLink.NavigateUrl = EditUrl("cid",hdClientId.Value);
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            PopulateLogItems();
        }
        private void PopulateLogItems()
        {
            long ClientId = Null.NullInteger;

            if (hdClientId.Value != "")
            {
                ClientId = long.Parse(hdClientId.Value);
                pnlDetails.Visible = true;
                fsDetails.Visible = true;
                var ctl = new Components.LogController();
                var ctlC = new Components.ClientController();
                Client objClient = ctlC.GetClient(ClientId);
                lnkEditLink.NavigateUrl = EditUrl("ClientId", ClientId.ToString(), "EditClient");
                lnkNewLog.NavigateUrl = EditUrl("ClientId", ClientId.ToString(), "AddLog");
                lblName.Text = objClient.FullName;
                lblNumber.Text = objClient.ClientId.ToString();

                IEnumerable<Log> colLog = ctl.GetLogsByClient(ClientId);
                rptEvents.DataSource = colLog;
                rptEvents.DataBind();
                cmdNewClient.Visible = false;
            }
            else
            {
                pnlDetails.Visible = false;
                fsDetails.Visible = false;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "The Name entered does not exist.  Click the New Client Button to create a new record for this Client.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                cmdNewClient.Visible = true;
            }

        }

    }
}