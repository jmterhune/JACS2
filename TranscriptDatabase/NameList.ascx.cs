/*
' Copyright (c) 2025  Joe Terhune
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;

namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class NameList : TranscriptDatabaseModuleBase
    {
        private readonly INavigationManager _navigationManager;

        #region Methods
        public NameList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new EmployeeController();
            rptName.DataSource = ctl.GetEmployees();
            rptName.DataBind();
        }
        private void ClearForm()
        {
            hdNameId.Value = string.Empty;
            txtName.Text = string.Empty;
            drpNameType.SelectedIndex = 0;
        }
#endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    var employeeTypes=Enumerations.GetValues<EmployeeTypes>();
                    foreach (EmployeeTypes employeeType in employeeTypes)
                    {
                        drpNameType.Items.Add(new ListItem(Enumerations.GetEnumDescription(employeeType),employeeType.ToString()));
                    }
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptNames_ItemCreated(object sender, RepeaterItemEventArgs e)
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

        protected void rptNames_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int nameId = Convert.ToInt32(e.CommandArgument);
            var ctl = new EmployeeController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteEmployee(nameId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Employee name = ctl.GetEmployee(nameId);
                hdNameId.Value = nameId.ToString();
                txtName.Text = name.EmployeeName;
                drpNameType.SelectedValue = name.EmployeeTypeID.ToString();
                ScriptManager.RegisterStartupScript(rptName, rptName.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void pnlNames_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new EmployeeController();
            Employee name = new Employee();
            bool isNew = true;
            if (hdNameId.Value != "")
            {
                isNew = false;
                name = ctl.GetEmployee(Convert.ToInt32(hdNameId.Value));
            }
            name.EmployeeName = txtName.Text;
            if(drpNameType.SelectedIndex>0) 
            name.EmployeeTypeID = Int32.Parse(drpNameType.SelectedValue);
            name.LastModifiedDate = DateTime.Now;
            name.LastModifiedByUser = UserId;
            if (isNew)
            {
                name.CreatedByUser = UserId;
                name.CreatedDate = DateTime.Now;
                ctl.CreateEmployee(name);
            }
            else
            {
                ctl.UpdateEmployee(name);
            }
            ClearForm();
            BindList();
        }
    }
}