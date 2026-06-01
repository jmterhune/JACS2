/*
' Copyright (c) 2022  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DotNetNuke.Services.Scheduling;
using System.Web.UI;

namespace tjc.Modules.CourtCounsel
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtCounselModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Calendar : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Calendar()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    PopulateUserList();
                    BindCalendar(CurrentDate);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateUserList()
        {
            var memberCtl = new MemberController();

            lstUsers.DataValueField = "MemberId";
            lstUsers.DataTextField = "ListName";
            IEnumerable<Member> activeMembers = memberCtl.GetMembersByType(1, true);
            IEnumerable<Member> inActiveMembers = memberCtl.GetMembersByType(1, false);
            foreach (Member member in activeMembers)
            {

                ListItem li = new ListItem(member.ListName, member.UserName);
                if (UserInfo.Username == member.UserName)
                    li.Selected = true;
                lstUsers.Items.Add(li);
            }
            lstUsers.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveMembers)
            {
                ListItem li = new ListItem(member.ListName, member.Email.ToString());
                li.Attributes.Add("class", "inactive");
                lstUsers.Items.Add(li);
            }
            lstUsers.Items.Add(new ListItem("Inactive Members", ">"));

        }

        private void BindCalendar(DateTime currentDate)
        {
            CurrentDate = currentDate;
            var tc = new EventController();
            List<string> selectedUsers = GetSelectedUsers();
            rptCalendar.DataSource = tc.GetCalendarEvents(currentDate, selectedUsers, EditUrl("logEdit"));
            rptCalendar.DataBind();
            Literal ltHeader = (Literal)rptCalendar.Controls[0].Controls[0].FindControl("ltHeader");
            ltHeader.Text = string.Format("{0} {1}", currentDate.ToString("MMMM"), currentDate.Year);
        }

        private List<string> GetSelectedUsers()
        {
            List<string> selectedUsers = new List<string>();
            foreach (ListItem item in lstUsers.Items)
            {
                if (item.Selected) { selectedUsers.Add(item.Value); }
            }
            return selectedUsers;
        }

        protected void cmdPreviousYear_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddYears(-1));
        }

        protected void cmdPreviousMonth_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddMonths(-1));
        }

        protected void cmdNextMonth_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddMonths(1));
        }

        protected void cmdNextYear_Click(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate.AddYears(1));
        }

        protected void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCalendar(CurrentDate);
        }
    }
}