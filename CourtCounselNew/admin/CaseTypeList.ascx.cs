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
    public partial class CaseTypeList : CourtCounselModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public CaseTypeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new CaseTypeController();
            rptCaseType.DataSource = ctl.GetCaseTypes();
            rptCaseType.DataBind();
        }
        private void ClearForm()
        {
            hdCaseTypeId.Value = string.Empty;
            txtCaseTypeName.Text = string.Empty;
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
            var ctl = new CaseTypeController();
            Components.CaseType caseType = new Components.CaseType();
            bool isNew = true;
            if (hdCaseTypeId.Value != "")
            {
                isNew = false;
                caseType = ctl.GetCaseType(Convert.ToInt32(hdCaseTypeId.Value));
            }
            caseType.CaseTypeName = txtCaseTypeName.Text;
            caseType.Active = chkActive.Checked;
            caseType.ModifiedDate = DateTime.Now;
            caseType.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                caseType.CreatedBy = UserInfo.Username;
                caseType.CreatedDate = DateTime.Now;
                ctl.CreateCaseType(caseType);
            }
            else
            {
                ctl.UpdateCaseType(caseType);
            }
            ClearForm();
            BindList();
        }
        protected void pnlCaseTypes_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptCaseType_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int caseTypeId = Convert.ToInt32(e.CommandArgument);
            var ctl = new CaseTypeController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteCaseType(caseTypeId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.CaseType caseType = ctl.GetCaseType(caseTypeId);
                hdCaseTypeId.Value = caseTypeId.ToString();
                txtCaseTypeName.Text = caseType.CaseTypeName;
                chkActive.Checked = caseType.Active;
                ScriptManager.RegisterStartupScript(rptCaseType, rptCaseType.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptCaseType_ItemCreated(object sender, RepeaterItemEventArgs e)
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