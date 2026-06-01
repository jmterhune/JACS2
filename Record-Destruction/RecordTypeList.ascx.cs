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
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.RecordDestruction.Components;

namespace tjc.Modules.RecordDestruction 
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
    public partial class DestructionMethodList : RecordDestructionModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public DestructionMethodList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private void BindList()
        {
            var ctl = new DestructionMethodController();
            rptDestructionMethods.DataSource = ctl.GetDestructionMethods().OrderBy(x => x.Description); ;
            rptDestructionMethods.DataBind();
        }
        private void ClearForm()
        {
            hdDestructionMethodId.Value = string.Empty;
            txtDestructionMethod.Text = string.Empty;
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
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new DestructionMethodController();
            DestructionMethod destructionMethod = new DestructionMethod();
            bool isNew = true;
            if (hdDestructionMethodId.Value != "")
            {
                isNew = false;
                destructionMethod = ctl.GetDestructionMethod(Convert.ToInt32(hdDestructionMethodId.Value));
            }
            destructionMethod.Description = txtDestructionMethod.Text;
            destructionMethod.LastModifiedDate = DateTime.Now;
            destructionMethod.LastModifiedByID = UserId;
            if (isNew)
            {
                destructionMethod.CreatedByID = UserId;
                destructionMethod.CreatedDate = DateTime.Now;
                ctl.CreateDestructionMethod(destructionMethod);
            }
            else
            {
                ctl.UpdateDestructionMethod(destructionMethod);
            }
            ClearForm();
            BindList();
        }
        protected void pnlDestructionMethods_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptDestructionMethods_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int destructionMethodId = Convert.ToInt32(e.CommandArgument);
            var ctl = new DestructionMethodController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteDestructionMethod(destructionMethodId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                DestructionMethod destructionMethod = ctl.GetDestructionMethod(destructionMethodId);
                hdDestructionMethodId.Value = destructionMethodId.ToString();
                txtDestructionMethod.Text = destructionMethod.Description;
                ScriptManager.RegisterStartupScript(rptDestructionMethods, rptDestructionMethods.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptDestructionMethods_ItemCreated(object sender, RepeaterItemEventArgs e)
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