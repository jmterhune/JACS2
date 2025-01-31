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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.MediationStatistics.Components;

namespace tjc.Modules.MediationStatistics
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from MediationStatisticsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class MediatorList : MediationStatisticsModuleBase
    {
        private readonly INavigationManager _navigationManager;

        #region Methods
        public MediatorList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new MediatorController();
            rptMediator.DataSource = ctl.GetMediators();
            rptMediator.DataBind();
        }
        private void ClearForm()
        {
            hdMediatorId.Value = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
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
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new MediatorController();
            Mediator mediator = new Mediator();
            bool isNew = true;
            if (hdMediatorId.Value != "")
            {
                isNew = false;
                mediator = ctl.GetMediator(Convert.ToInt32(hdMediatorId.Value));
            }
            mediator.FirstName = txtFirstName.Text;
            mediator.LastName = txtLastName.Text;
            mediator.Email = txtEmail.Text;
            mediator.Phone = txtPhone.Text;
            mediator.LastModifiedDate = DateTime.Now;
            mediator.LastModifiedById = UserId;
            if (isNew)
            {
                mediator.CreatedById = UserId;
                mediator.CreatedDate = DateTime.Now;
                ctl.CreateMediator(mediator);
            }
            else
            {
                ctl.UpdateMediator(mediator);
            }
            ClearForm();
            BindList();
        }
        protected void pnlMediators_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptMediator_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int mediatorId = Convert.ToInt32(e.CommandArgument);
            var ctl = new MediatorController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteMediator(mediatorId);
                BindList();
            }
           
            if (e.CommandName == "edit")
            {
                Mediator mediator = ctl.GetMediator(mediatorId);
                hdMediatorId.Value = mediatorId.ToString();
                txtFirstName.Text = mediator.FirstName;
                txtLastName.Text = mediator.LastName;
                txtPhone.Text = mediator.Phone;
                txtEmail.Text = mediator.Email;
                ScriptManager.RegisterStartupScript(rptMediator, rptMediator.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptMediator_ItemCreated(object sender, RepeaterItemEventArgs e)
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