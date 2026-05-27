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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.FamilySelfHelp.Components;

namespace tjc.Modules.FamilySelfHelp
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from FamilySelfHelpModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditClient : FamilySelfHelpModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public EditClient()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    modSecurty = new ModuleSecurity(this.ModuleConfiguration);
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    lnkDataEntry.NavigateUrl = EditUrl("log");
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    if(IsAdmin)
                    {
                        lnkMerge.Visible = true;
                        lnkReports.Visible = true;
                        cmdDelete.Visible = true;
                    }
                    if(modSecurty.HasReportPermission)
                        lnkReports.Visible = true;
                    if(modSecurty.HasMergePermission)
                        lnkMerge.Visible = true;
                    var tc = new ClientController();
                    if (ClientId > 0)
                    {
                        Components.Client client = tc.GetClient(ClientId);
                        if (client != null)
                        {
                            txtEmail.Text = client.Email;
                            txtFirstName.Text = client.FirstName;
                            txtLastName.Text = client.LastName;
                            txtMiddleInitial.Text = client.MiddleInitial;
                            txtPhone.Text = client.Phone;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                                "Swal.fire({ title: 'Warning', html: '" + System.Web.HttpUtility.JavaScriptStringEncode("Unable to Load Client Information. Please contact HelpDesk for support.") + "', icon: 'warning', confirmButtonText: 'OK' });", true);
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            var t = new Client();
            var tc = new ClientController();
            if (ClientId > 0)
            {
                t=tc.GetClient(ClientId);
                t.Email = txtEmail.Text;
                t.FirstName = txtFirstName.Text;
                t.LastName = txtLastName.Text;
                t.MiddleInitial = txtMiddleInitial.Text;
                t.Phone = txtPhone.Text;
                t.LastModifiedById=UserId;
                t.LastModifiedDate = DateTime.Now;
                tc.UpdateClient(t);
            }
            else {
                t.Email = txtEmail.Text;
                t.FirstName = txtFirstName.Text;
                t.LastName = txtLastName.Text;
                t.MiddleInitial = txtMiddleInitial.Text;
                t.Phone = txtPhone.Text;
                t.LastModifiedById=UserId;
                t.LastModifiedDate = DateTime.Now;
                t.CreatedDate = DateTime.Now;
                t.CreatedById = UserId;
                tc.CreateClient(t);
            }
            Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + t.ClientId.ToString()));

        }
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            var t = new Client();
            var tc = new ClientController();
            var lc=new Components.LogController();
            if (ClientId > 0)
            {
                lc.GetLogsByClient(ClientId);
                tc.DeleteClient(ClientId);
            }
            Response.Redirect(_navigationManager.NavigateURL());
        }
    }
}