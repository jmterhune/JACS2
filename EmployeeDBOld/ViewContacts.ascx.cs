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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components;
using tjc.Modules.EmployeeDB.Components.Services;

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
    public partial class ViewContacts : EmployeeDBModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private bool ShowActive
        {
            get
            {
                if (ViewState["ShowActive"] != null) { return Convert.ToBoolean(ViewState["ShowActive"]); }
                return true;
            }
            set { ViewState["ShowActive"] = value; }
        }
        #endregion

        #region Methods
        public ViewContacts()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateContactList()
        {
            var ctl = new EmployeeController();
            rptContacts.DataSource = ctl.GetEmployeeListItems(ShowActive, false);
            rptContacts.DataBind();
        }
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
                    PopulateContactList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        
        protected void rptContacts_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int contactId = Convert.ToInt32(e.CommandArgument);
            var ctl = new EmployeeController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteEmployee(contactId);
                PopulateContactList();
                LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
                string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
                try
                {
                    SwnInterface.DeleteSwnContactById(contactId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);
                }
                catch (Exception exc)
                {
                    string process = string.Format("Delete Contact with Id:{0}", contactId.ToString());
                    SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                    var logCtl = new SwnLogController();
                    logCtl.CreateSwnLog(swnLog);
                }
            }
        }

        protected void pnlContacts_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptContacts_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
            }
        }
        #endregion
    }
}