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

using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
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
                    modSecurty = new ModuleSecurity(this.ModuleConfiguration);
                    lnkDataEntry.NavigateUrl = EditUrl("log");
                    lnkEditLink.NavigateUrl = EditUrl("cid", hdClientId.Value, "client");
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                    lnkNewLog.NavigateUrl=EditUrl("cid", hdClientId.Value, "log");
                    if (ClientId > 0)
                    {
                        hdClientId.Value = ClientId.ToString();

                        PopulateLogItems();
                    }
                    if (IsAdmin)
                    {
                        lnkMerge.Visible = true;
                        lnkReports.Visible = true;
                    }
                    if (modSecurty.HasReportPermission)
                        lnkReports.Visible = true;
                    if (modSecurty.HasMergePermission)
                        lnkMerge.Visible = true;
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
                lnkEditLink.NavigateUrl = EditUrl("cid", ClientId.ToString(), "client");
                lnkNewLog.NavigateUrl = EditUrl("cid", ClientId.ToString(), "log");
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
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "Swal.fire({ title: 'Warning', html: '" + System.Web.HttpUtility.JavaScriptStringEncode("The Name entered does not exist.  Click the New Client Button to create a new record for this Client.") + "', icon: 'warning', confirmButtonText: 'OK' });", true);
                cmdNewClient.Visible = true;
            }
        }

        protected void rptEvents_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            var ctl = new Components.LogController();
            int logId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "delete")
            {
                ctl.DeleteLog(logId);
                IEnumerable<Log> colLog = ctl.GetLogsByClient(ClientId);
                rptEvents.DataSource = colLog;
                rptEvents.DataBind();
            }
        }
    }
}