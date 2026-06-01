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
    public partial class PhaseList : CourtCounselModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public PhaseList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new PhaseController();
            rptPhase.DataSource = ctl.GetPhases();
            rptPhase.DataBind();
        }
        private void ClearForm()
        {
            hdPhaseId.Value = string.Empty;
            txtPhaseName.Text = string.Empty;
            txtGroupName.Text = string.Empty;
            txtGroupIndex.Text = string.Empty;
            chkPending.Checked = false;
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
                chkPending.InputAttributes.Add("class", "form-check-input");
                chkPending.LabelAttributes.Add("class", "form-check-label");

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
            var ctl = new PhaseController();
            Components.Phase phase = new Components.Phase();
            bool isNew = true;
            if (hdPhaseId.Value != "")
            {
                isNew = false;
                phase = ctl.GetPhase(Convert.ToInt32(hdPhaseId.Value));
            }
            phase.PhaseName = txtPhaseName.Text;
            phase.GroupName = txtGroupName.Text;
            if(txtGroupIndex.Text!="")
            phase.GroupIndex=int.Parse(txtGroupIndex.Text);
            phase.Active = chkActive.Checked;
            phase.IsPending=chkPending.Checked;
            phase.ModifiedDate = DateTime.Now;
            phase.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                phase.CreatedBy = UserInfo.Username;
                phase.CreatedDate = DateTime.Now;
                ctl.CreatePhase(phase);
            }
            else
            {
                ctl.UpdatePhase(phase);
            }
            ClearForm();
            BindList();
        }
        protected void pnlPhases_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptPhase_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int phaseId = Convert.ToInt32(e.CommandArgument);
            var ctl = new PhaseController();
            if (e.CommandName == "delete")
            {

                ctl.DeletePhase(phaseId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.Phase phase = ctl.GetPhase(phaseId);
                hdPhaseId.Value = phaseId.ToString();
                txtPhaseName.Text = phase.PhaseName;
                txtGroupIndex.Text=phase.GroupIndex.ToString();
                txtGroupName.Text = phase.GroupName;
                chkPending.Checked=phase.IsPending;
                chkActive.Checked = phase.Active;
                ScriptManager.RegisterStartupScript(rptPhase, rptPhase.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptPhase_ItemCreated(object sender, RepeaterItemEventArgs e)
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