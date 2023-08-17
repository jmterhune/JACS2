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
using System.Net.Http.Headers;
using System.Net.Http;
using tjc.Modules.EmployeeDB.Components.Services;
using System.Collections.ObjectModel;

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
                RemoveSwnGroup(groupId.ToString());
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
            if (group.IsSwnGroup)
                SyncSwnGroup(group);
            ClearForm();
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
        private void ClearForm()
        {
            hdDepartmentId.Value = string.Empty;
            txtDescription.Text = string.Empty;
            drpType.SelectedIndex = 0;
            chkIsSWNGroup.Checked = false;
        }
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
        private void RemoveSwnGroup(string groupId)
        {
            try
            {
                TokenInformation sessionToken = SessionVariables.SwnToken;
                if (sessionToken == null)
                {
                    sessionToken = Helper.CreateSwnToken(SwnServiceIdentifier, SwnSubscriptionKey, new LoginRequest { Password = SwnPassword, Username = SwnUsername });
                    SessionVariables.SwnToken = sessionToken;
                }
                SwnClient client = new SwnClient(new HttpClient());
                var authorization = new AuthenticationHeaderValue("Bearer", sessionToken.Token);
                var result = client.DELETEGroupsIdAsync(groupId, SwnServiceIdentifier, SwnSubscriptionKey, authorization);
                result.Wait();
            }
            catch (Exception exc)
            {
                ltMessage.Text = string.Format("<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> The Following Error Occured. <strong class='d-block ms-4'>{0}</strong><span class='d-block'>Please review the {1} Group on the <a href='https://idsrv.sendwordnow.com/account/signin'>Send Word Now</a> site to ensure that it was deleted correctly.</span></div>", exc.InnerException.Message);
            }
        }
        private void SyncSwnGroup(Group group)
        {
            string process = "Sync SWN Group";
            try
            {
                LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
                string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);

                var ctl = new GroupController();
                SwgGroupDetails groupDetails = new SwgGroupDetails();
                IEnumerable<string> groupMembers = ctl.GetSwnGroupMembers(group.GroupId);
                try
                {
                    groupDetails = SwnInterface.GetSwnGroup(group.GroupId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);
                }
                catch { }
                ICollection<string> members = groupMembers.ToList();
                GroupMemberModel model = new GroupMemberModel { Contacts = members };
                if (groupDetails != null && string.IsNullOrEmpty(groupDetails.Id))
                {
                    process = String.Format("Added {0} Group to SWN", group.GroupName);
                    var addedGroup = SwnInterface.AddSwnGroup(group, model, SwnServiceIdentifier, SwnSubscriptionKey, token, true);
                    ltMessage.Text = string.Format("<div class='alert alert-success'><i class='fas fa-thumbs-up'></i> Successfully added {0} group to SWN</div>", group.GroupName);
                }
                else
                {
                    process = String.Format("Updated {0} Group", group.GroupName);
                    var updatedGroup = SwnInterface.AddSwnGroup(group, model, SwnServiceIdentifier, SwnSubscriptionKey, token, false);
                    ltMessage.Text = string.Format("<div class='alert alert-success'><i class='fas fa-thumbs-up'></i> Successfully updated {0} group in SWN</div>", group.GroupName);
                }
            }
            catch (Exception exc)
            {
                SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                var ctl = new SwnLogController();
                ctl.CreateSwnLog(swnLog);
                ltMessage.Text = string.Format("<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> The Following Error Occured. <strong class='d-block ms-4'>{0}</strong><span class='d-block'>Please review the {1} Group on the <a href='https://idsrv.sendwordnow.com/account/signin'>Send Word Now</a> site to ensure that it was updated correctly.</span></div>", exc.InnerException.Message);
            }
        }
        #endregion

    }
}