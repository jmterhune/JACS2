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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using System;
using System.Web.UI.WebControls;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.EmployeeDB.Components;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.Globals;

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
    public partial class RaceView : EmployeeDBModuleBase
    {
        #region Members
        public string DrpSortHtml;
        private readonly INavigationManager _navigationManager;
        private bool ShowActive { get { if (ViewState["ShowActive"] != null) { return Convert.ToBoolean(ViewState["ShowActive"]); } return true; } set { ViewState["ShowActive"] = value; } }
        #endregion

        #region Events

        #endregion

        #region Methods
        public RaceView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateEmployeeList()
        {
            var ctl = new EmployeeController();
            rptEmployees.DataSource = ctl.GetEmployeeListItems(ShowActive);
            rptEmployees.DataBind();
        }
        private void PopulateSectionDropdown()
        {
            DrpSortHtml = "<label class='mr-2'>Filter by Department <select id='drpfilter' class='form-control input-sm' aria-controls='employees'><option value='-1'>All</option>";
            var ctl = new GroupController();

            IEnumerable<Group> departments = ctl.GetGroups().Where(x => x.GroupType == Convert.ToInt32(Group.GroupTypeName.Internal));
            foreach (Group department in departments)
            {
                DrpSortHtml += "<option value='" + department.GroupId.ToString() + "'>" + department.GroupName + "</option>";
            }
            DrpSortHtml += "</select></label>";
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateSectionDropdown();
                    PopulateEmployeeList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkInactiveEmployees_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowActive)
            {
                ShowActive = false;
                lblInactiveEmployees.Text = "Toggle On for Active Employees";
            }
            else
            {
                ShowActive = true;
                lblInactiveEmployees.Text = "Toggle Off for Inactive Employees";
            }
            PopulateEmployeeList();

        }
    }
}