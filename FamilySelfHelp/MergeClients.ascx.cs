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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class MergeClients : FamilySelfHelpModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public MergeClients()
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
        public List<Client> Clients
        {
            get { return (List<Client>)Session["ClientList"]; }
            set { Session["ClientList"] = value; }
        }
        public string GetCaseNumbers(string clientId)
        {

            var ctl = new Components.LogController();
            IEnumerable<Log> logs = ctl.GetLogsByClient(long.Parse(clientId));
            string outString = "";
            foreach (Log c in logs)
                if (c.ServiceDate.HasValue)
                    outString += string.Format("<li>{0} {1}</li>", c.ServiceDate.Value.ToShortDateString(), c.FormattedServiceProvided);
            return "<ul>" + outString + "</ul>";
        }
        private void BindData()
        {
            rptClients.DataSource = Clients;
            rptClients.DataBind();
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
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    if (IsAdmin)
                    {
                        lnkMerge.Visible = true;
                        lnkReports.Visible = true;
                    }
                    if (modSecurty.HasReportPermission)
                        lnkReports.Visible = true;
                    if (modSecurty.HasMergePermission)
                        lnkMerge.Visible = true;
                    lnkDataEntry.NavigateUrl = EditUrl("log");
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                    Clients = new List<Client>();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptClients_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "merge")
            {
                long clientId = Int32.Parse(e.CommandArgument.ToString());
                var ctl = new ClientController();
                foreach (Client d in Clients)
                {
                    if (d.ClientId != clientId)
                        ctl.MergeClients(clientId, d.ClientId);
                }
                Clients = new List<Client>
                {
                    ctl.GetClient(clientId)
                };
                BindData();
            }
            if (e.CommandName == "remove")
            {
                long clientId = Int32.Parse(e.CommandArgument.ToString());
               Clients.Remove(Clients.Where(x => x.ClientId == clientId).First());
                BindData();
            }
        }
        protected void cmdClient_Click(object sender, EventArgs e)
        {
            long clientId = Int32.Parse(hdClientId.Value);
            var ctl = new ClientController();
            Client client = ctl.GetClient(clientId);
            if (!Clients.Exists(d => d.ClientId == clientId))
                Clients.Add(client);
            BindData();
        }
    }
}