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
using DotNetNuke.Services.Exceptions;
using System;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.EmployeeDB.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class DepartmentView : EmployeeDBModuleBase
    {
        #region Members
        public string DrpSortHtml;
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }

                    PopulateDepartmentList();
                    chkIsSWNGroup.InputAttributes.Add("class", "form-check-input");
                    chkIsSWNGroup.LabelAttributes.Add("class", "form-check-label");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptDepartments_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int groupId = Convert.ToInt32(e.CommandArgument);
            var ctl = new GroupController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteGroup(groupId);
                PopulateDepartmentList();
            }
            if (e.CommandName == "edit")
            {
                Group group = ctl.GetGroup(groupId);

                hdDepartmentId.Value = groupId.ToString();
                txtDescription.Text = group.GroupName;
                drpType.SelectedValue = group.GroupType.ToString();
                chkIsSWNGroup.Checked = group.IsSwnGroup;
                ScriptManager.RegisterStartupScript(rptDepartments, rptDepartments.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Group group = new Group();
            bool isNew = true;
            if (hdDepartmentId.Value != "")
            {
                isNew = false;
                group = ctl.GetGroup(Convert.ToInt32(hdDepartmentId.Value));
            }
            group.GroupName = txtDescription.Text;
            group.GroupType = Convert.ToInt32(drpType.SelectedValue);
            group.IsSwnGroup = chkIsSWNGroup.Checked;
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
            hdDepartmentId.Value = "";
            PopulateDepartmentList();
        }
        protected void pnlDepartments_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptDepartments_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
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

        #region Methods
        public DepartmentView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateDepartmentList()
        {
            var ctl = new GroupController();
            rptDepartments.DataSource = ctl.GetGroups();
            rptDepartments.DataBind();
        }
        #endregion

    }
}