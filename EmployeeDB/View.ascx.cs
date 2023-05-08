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
using System;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.EmployeeDB.Components;
using System.Collections.Generic;
using System.Linq;

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
    public partial class View : EmployeeDBModuleBase
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
        public string DepartmentFilterHtml
        {
            get
            {
                if (ViewState["DepartmentFilterHtml"] != null) { return ViewState["DepartmentFilterHtml"].ToString(); }
                return "";
            }
            set { ViewState["DepartmentFilterHtml"] = value; }
        }
        #endregion

        #region Events

        #endregion

        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateEmployeeList()
        {
            var ctl = new EmployeeController();
            rptEmployees.DataSource = ctl.GetEmployeeListItems(ShowActive);
            rptEmployees.DataBind();
        }
        private string GetDepartmentFilterHtml()
        {
            string filterHtml="";
            filterHtml = "<label class='me-2'>Filter by Department <select id='drpfilter' class='form-control input-sm' aria-controls='employees'><option value='-1'>All</option>";
            var ctl = new GroupController();

            IEnumerable<Group> departments = ctl.GetGroups().Where(x => x.GroupType == Convert.ToInt32(Group.GroupTypes.Internal));
            foreach (Group department in departments)
            {
                filterHtml += "<option value='" + department.GroupId.ToString() + "'>" + department.GroupName + "</option>";
            }
            filterHtml += "</select></label>";
            return filterHtml;
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateEmployeeList(); 
                    DepartmentFilterHtml=GetDepartmentFilterHtml();
                }
                chkInactiveEmployees.InputAttributes.Add("class", "form-check-input");
                chkInactiveEmployees.LabelAttributes.Add("class", "form-check-label");


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
                chkInactiveEmployees.Text = "Inactive Employees";
            }
            else
            {
                ShowActive = true;
                chkInactiveEmployees.Text = "Active Employees";
            }
            PopulateEmployeeList();

        }
    }
}