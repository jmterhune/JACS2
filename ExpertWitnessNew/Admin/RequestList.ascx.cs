/*
' Copyright (c) 2023  12th Judicial Circuit
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
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class RequestList : ExpertWitnessModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public RequestList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new RequestController();
            rptRequest.DataSource = ctl.GetRequestListItems();
            rptRequest.DataBind();
        }
        private void ClearForm()
        {
            hdRequestId.Value = string.Empty;
            txtCaseNumber.Text = string.Empty;
            txtLocation.Text = string.Empty;
            txtTemplate.Text = string.Empty;
            ltMessage.Text = string.Empty;
            rptExperts.DataSource = null;
            rptExperts.DataBind();
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void pnlRequests_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptRequest_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int requestId = Convert.ToInt32(e.CommandArgument);
            var ctl = new RequestController();
            var tCtl = new TemplateController();
            var eCtl = new ExpertController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteRequest(requestId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.RequestListItem request = ctl.GetRequestListItem(requestId);
                int currentSequence = 0;
                if (request != null)
                {
                    hdRequestId.Value = requestId.ToString();
                    txtCaseNumber.Text = request.CaseNumber;
                    txtTemplate.Text = request.TemplateName;
                    txtLocation.Text = request.LocationName;
                    string templateText = "";
                    foreach (TemplateSequence templateSeq in tCtl.GetTemplateSequences(request.TemplateID))
                    {
                        if (templateSeq.Sequence != currentSequence)
                        {
                            templateText = string.Format("<strong>Requirement #{0}:</strong> Select [{1}] ", templateSeq.Sequence, templateSeq.NumberRequired);
                            foreach (Components.Type t in tCtl.GetTemplateTypeTypesBySequence(templateSeq.TemplateID,templateSeq.Sequence))
                                templateText += string.Format("{0} or ", t.TypeName);
                        }
                        ltRequirements.Text += string.Format("<li>{0}</li>", templateText.Trim().TrimEnd('r').TrimEnd('o'));
                    }
                    rptExperts.DataSource = eCtl.GetExpertRequestListItems(requestId);
                    rptExperts.DataBind();
                }
                ScriptManager.RegisterStartupScript(rptRequest, rptRequest.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptRequest_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);

                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }
        #endregion
    }
}