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

using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using System;
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
    public partial class CaseTypeList : MediationStatisticsModuleBase
    {

        #region Methods
        private void BindList()
        {
            var ctl = new CaseTypeController();
            rptCaseType.DataSource = ctl.GetCaseTypes();
            rptCaseType.DataBind();
        }
        private void ClearForm()
        {
            hdCaseTypeId.Value = string.Empty;
            txtCaseType.Text = string.Empty;
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
            CaseType caseType = new CaseType();
            bool isNew = true;
            if (hdCaseTypeId.Value != "")
            {
                isNew = false;
                caseType = ctl.GetCaseType(Convert.ToInt32(hdCaseTypeId.Value));
            }
            caseType.Description = txtCaseType.Text;
            caseType.Active = chkActive.Checked;
            caseType.LastModifiedDate = DateTime.Now;
            caseType.LastModifiedById = UserId;
            if (isNew)
            {
                caseType.CreatedById = UserId;
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
                CaseType caseType = ctl.GetCaseType(caseTypeId);
                hdCaseTypeId.Value = caseTypeId.ToString();
                txtCaseType.Text = caseType.Description;
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