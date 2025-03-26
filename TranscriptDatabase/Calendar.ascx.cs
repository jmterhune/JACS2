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
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Calendar : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public Calendar()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateUserList()
        {
            var courtReporters = RoleController.Instance.GetUsersByRole(PortalId, CourtReporterRole);
            lstUsers.DataSource = courtReporters.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            lstUsers.DataTextField = "DisplayName";
            lstUsers.DataValueField = "UserID";
            lstUsers.DataBind();
        }
        private void BindCalendar(DateTime currentDate)
        {
            CurrentDate = currentDate;
            var tc = new CalendarController();
            List<int> selectedUsers = GetSelectedUsers();
            rptCalendar.DataSource = tc.GetCalendarEvents(currentDate, drpCounty.SelectedValue, selectedUsers, EditUrl("status"));
            rptCalendar.DataBind();
            Literal ltHeader = (Literal)rptCalendar.Controls[0].Controls[0].FindControl("ltHeader");
            ltHeader.Text = string.Format("<div class='d-inline-block calendar-title'>{0} {1}</div>", currentDate.ToString("MMMM"), currentDate.Year);
        }
        private List<int> GetSelectedUsers()
        {
            List<int> selectedUsers = new List<int>();
            foreach (ListItem item in lstUsers.Items)
            {
                if (item.Selected) { selectedUsers.Add(Int32.Parse(item.Value)); }
            }
            return selectedUsers;
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    PopulateUserList();
                    txtCurrentDate.Text = CurrentDate.ToShortDateString();
                    BindCalendar(CurrentDate);
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdPreviousYear_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddYears(-1));
            txtCurrentDate.Text = CurrentDate.AddYears(-1).ToShortDateString();

        }
        protected void cmdPreviousMonth_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddMonths(-1));
            txtCurrentDate.Text = CurrentDate.AddMonths(-1).ToShortDateString();
        }
        protected void cmdNextMonth_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddMonths(1));
            txtCurrentDate.Text = CurrentDate.AddMonths(1).ToShortDateString();
        }
        protected void cmdNextYear_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddYears(1));
            txtCurrentDate.Text = CurrentDate.AddYears(1).ToShortDateString();
        }
        protected void pnlUpdate_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }
        protected void rptCalendar_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
            LinkButton cmdPreviousYear = (LinkButton)e.Item.FindControl("cmdPreviousYear");
            LinkButton cmdPreviousMonth = (LinkButton)e.Item.FindControl("cmdPreviousMonth");
            LinkButton cmdNextMonth = (LinkButton)e.Item.FindControl("cmdNextMonth");
            LinkButton cmdNextYear = (LinkButton)e.Item.FindControl("cmdNextYear");

            if (cmdPreviousYear != null)
                scriptMan.RegisterAsyncPostBackControl(cmdPreviousYear);
            if (cmdPreviousMonth != null)
                scriptMan.RegisterAsyncPostBackControl(cmdPreviousMonth);
            if (cmdNextMonth != null)
                scriptMan.RegisterAsyncPostBackControl(cmdNextMonth);
            if (cmdNextYear != null)
                scriptMan.RegisterAsyncPostBackControl(cmdNextYear);
        }
        protected void drpCounty_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate);
        }

        protected void txtCurrentDate_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtCurrentDate.Text, out DateTime currentDate))
                CurrentDate = currentDate;
            BindCalendar(CurrentDate);

        }
        protected void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate);
        }
        #endregion


    }
}