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
    public partial class RecordTypeList : RecordDestructionModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public RecordTypeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private void BindList()
        {
            var ctl = new RecordTypeController();
            rptRecordTypes.DataSource = ctl.GetRecordTypes().OrderBy(x=>x.Description);
            rptRecordTypes.DataBind();
        }
        private void ClearForm()
        {
            hdRecordTypeId.Value = string.Empty;
            txtRecordType.Text = string.Empty;
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
            var ctl = new RecordTypeController();
            RecordType recordType = new RecordType();
            bool isNew = true;
            if (hdRecordTypeId.Value != "")
            {
                isNew = false;
                recordType = ctl.GetRecordType(Convert.ToInt32(hdRecordTypeId.Value));
            }
            recordType.Description = txtRecordType.Text;
            recordType.LastModifiedDate = DateTime.Now;
            recordType.LastModifiedByID = UserId;
            if (isNew)
            {
                recordType.CreatedByID = UserId;
                recordType.CreatedDate = DateTime.Now;
                ctl.CreateRecordType(recordType);
            }
            else
            {
                ctl.UpdateRecordType(recordType);
            }
            ClearForm();
            BindList();
        }
        protected void pnlRecordTypes_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptRecordTypes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int recordTypeId = Convert.ToInt32(e.CommandArgument);
            var ctl = new RecordTypeController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteRecordType(recordTypeId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                RecordType recordType = ctl.GetRecordType(recordTypeId);
                hdRecordTypeId.Value = recordTypeId.ToString();
                txtRecordType.Text = recordType.Description;
                ScriptManager.RegisterStartupScript(rptRecordTypes, rptRecordTypes.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptRecordTypes_ItemCreated(object sender, RepeaterItemEventArgs e)
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