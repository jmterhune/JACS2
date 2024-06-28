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
using System.Collections.Generic;
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
    public partial class GroupList : MediationStatisticsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods 
        public GroupList()
            {
                _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            }
        private void BindList()
        {
           
            var ctl = new GroupController();
            rptGroup.DataSource = ctl.GetGroups();
            rptGroup.DataBind();
        }
        private void ClearForm()
        {
            hdGroupId.Value = string.Empty;
            txtGroup.Text = string.Empty;
            chkCourtOrdered.Checked = false;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkCourtOrdered.InputAttributes.Add("class", "form-check-input");
                chkCourtOrdered.LabelAttributes.Add("class", "form-check-label");

                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());

                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
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
            var ctl = new GroupController();
            Group group = new Group();
            bool isNew = true;
            if (hdGroupId.Value != "")
            {
                isNew = false;
                group = ctl.GetGroup(Convert.ToInt32(hdGroupId.Value));
            }
            group.Description = txtGroup.Text;
            group.CourtOrdered = chkCourtOrdered.Checked;
            group.LastModifiedDate = DateTime.Now;
            group.LastModifiedById = UserId;
            if (isNew)
            {
                group.CreatedById = UserId;
                group.CreatedDate = DateTime.Now;
                ctl.CreateGroup(group);
            }
            else
            {
                ctl.UpdateGroup(group);
            }
            ClearForm();
            BindList();
        }
        protected void pnlGroups_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptGroup_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int groupId = Convert.ToInt32(e.CommandArgument);
            var ctl = new GroupController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteGroup(groupId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Group group = ctl.GetGroup(groupId);
                hdGroupId.Value = groupId.ToString();
                txtGroup.Text = group.Description;
                if (group.CourtOrdered.HasValue)
                    chkCourtOrdered.Checked = group.CourtOrdered.Value;
                ScriptManager.RegisterStartupScript(rptGroup, rptGroup.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptGroup_ItemCreated(object sender, RepeaterItemEventArgs e)
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