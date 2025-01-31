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
using System.Collections.Generic;
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
    public partial class LogItemList : RecordDestructionModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
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
        #region Methods
        private string GetDepartmentFilterHtml()
        {
            string filterHtml = "";
            filterHtml = "<label class='me-2'>Filter by Department<select id='drpfilter' class='form-control form-control-sm d-inline-block w-auto ms-2' aria-controls='tblLogItem'><option>All</option>";
            var ctl = new GroupController();

            IEnumerable<Group> departments = ctl.GetGroups().OrderBy(x=>x.GroupName);
            foreach (Group department in departments)
            {
                filterHtml += "<option>" + department.GroupName + "</option>";
            }
            filterHtml += "</select></label>";
            return filterHtml;
        }
        public LogItemList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private void BindList()
        {
            var ctl = new LogController();
            rptLogItems.DataSource = ctl.GetLogListItems();
            rptLogItems.DataBind();
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DepartmentFilterHtml = GetDepartmentFilterHtml();
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
        #endregion
    }
}