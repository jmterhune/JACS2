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
using tjc.Modules.CourtCounsel.Components;
namespace tjc.Modules.CourtCounsel
{
    public partial class MemberList : CourtCounselModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public MemberList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new MemberController();
            rptMember.DataSource = ctl.GetMembers();
            rptMember.DataBind();
        }
        private void ClearForm()
        {
            hdMemberId.Value = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            drpMember.SelectedValue = string.Empty;
            txtUserName.Text = string.Empty;
            chkActive.Checked = false;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkActive.InputAttributes.Add("class", "form-check-input");
                chkActive.LabelAttributes.Add("class", "form-check-label");

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
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new MemberController();
            Components.Member member = new Components.Member();
            bool isNew = true;
            if (hdMemberId.Value != "")
            {
                isNew = false;
                member = ctl.GetMember(Convert.ToInt32(hdMemberId.Value));
            }
            member.FirstName = txtFirstName.Text;
            member.LastName = txtLastName.Text;
            member.Email = txtEmail.Text;
            member.UserName = txtUserName.Text;
            member.MemberTypeId = Convert.ToInt32(drpMember.SelectedValue);
            member.Active = chkActive.Checked;
            member.ModifiedDate = DateTime.Now;
            member.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                member.CreatedBy = UserInfo.Username;
                member.CreatedDate = DateTime.Now;
                ctl.CreateMember(member);
            }
            else
            {
                ctl.UpdateMember(member);
            }
            ClearForm();
            BindList();
        }
        protected void pnlMembers_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptMember_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int memberId = Convert.ToInt32(e.CommandArgument);
            var ctl = new MemberController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteMember(memberId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.Member member = ctl.GetMember(memberId);
                hdMemberId.Value = memberId.ToString();
                txtFirstName.Text = member.FirstName;
                txtLastName.Text = member.LastName;
                txtEmail.Text = member.Email;
                txtUserName.Text = member.UserName;
                drpMember.SelectedValue = member.MemberTypeId.ToString();
                chkActive.Checked = member.Active;
                ScriptManager.RegisterStartupScript(rptMember, rptMember.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptMember_ItemCreated(object sender, RepeaterItemEventArgs e)
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