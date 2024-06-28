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
    public partial class AppearanceList : MediationStatisticsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public AppearanceList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new AppearanceController();
            rptAppearance.DataSource = ctl.GetAppearances();
            rptAppearance.DataBind();
        }
        private void ClearForm()
        {
            hdAppearanceId.Value = string.Empty;
            txtAppearance.Text = string.Empty;
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
            var ctl = new AppearanceController();
            Appearance appearance = new Appearance();
            bool isNew = true;
            if (hdAppearanceId.Value != "")
            {
                isNew = false;
                appearance = ctl.GetAppearance(Convert.ToInt32(hdAppearanceId.Value));
            }
            appearance.Description = txtAppearance.Text;
            appearance.Active = chkActive.Checked;
            appearance.LastModifiedDate = DateTime.Now;
            appearance.LastModifiedById = UserId;
            if (isNew)
            {
                appearance.CreatedById = UserId;
                appearance.CreatedDate = DateTime.Now;
                ctl.CreateAppearance(appearance);
            }
            else
            {
                ctl.UpdateAppearance(appearance);
            }
            ClearForm();
            BindList();
        }
        protected void pnlAppearances_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptAppearance_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int appearanceId = Convert.ToInt32(e.CommandArgument);
            var ctl = new AppearanceController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteAppearance(appearanceId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Appearance appearance = ctl.GetAppearance(appearanceId);
                hdAppearanceId.Value = appearanceId.ToString();
                txtAppearance.Text = appearance.Description;
                chkActive.Checked = appearance.Active;
                ScriptManager.RegisterStartupScript(rptAppearance, rptAppearance.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptAppearance_ItemCreated(object sender, RepeaterItemEventArgs e)
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