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
using System.Net.NetworkInformation;
using System.Text;

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
    public partial class Reports : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Reports()
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
                    if (UserInfo.IsInRole(AdminRole))
                        li1.Visible = true;

                    PopulateDropDowns();
                }
                chkShowDetail.InputAttributes.Add("class", "form-check-input");
                chkShowDetail.LabelAttributes.Add("class", "form-check-label");
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void PopulateDropDowns()
        {
            var countyCtl = new CountyController();
            drpCounty.DataValueField = "CountyId";
            drpCounty.DataTextField = "CountyName";
            drpCounty.DataSource = countyCtl.GetCounties();
            drpCounty.DataBind();
            drpCounty.Items.Insert(0, new ListItem("All", ""));
            var ac = new MemberController();
            drpAttorney.DataValueField = "MemberId";
            drpAttorney.DataTextField = "ListName";
            IEnumerable<Member> activeMembers = ac.GetMembersByType(1, true);
            IEnumerable<Member> inActiveMembers = ac.GetMembersByType(1, false);
            drpAttorney.Items.Add(new ListItem("All", ""));
            foreach (Member member in activeMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                drpAttorney.Items.Add(li);
            }
            drpAttorney.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                li.Attributes.Add("class", "inactive");
                drpAttorney.Items.Add(li);
            }
            drpAttorney.Items.Add(new ListItem("Inactive Members", ">"));

            var pc = new PhaseController();
            IEnumerable<Phase> phases = pc.GetPhaseDropDownItems(true);
            string groupName = "";
            foreach (Phase phase in phases)
            {
                if (phase.GroupName != "Default")
                {
                    if (groupName != phase.GroupName && string.IsNullOrEmpty(groupName))
                    {
                        drpExtendedStatus.Items.Add(new ListItem(phase.GroupName, "<"));
                    }
                    if (groupName != phase.GroupName && !string.IsNullOrEmpty(groupName))
                    {
                        drpExtendedStatus.Items.Add(new ListItem(groupName, ">"));
                        drpExtendedStatus.Items.Add(new ListItem(phase.GroupName, "<"));
                    }
                    groupName = phase.GroupName;
                }
                ListItem li = new ListItem(phase.PhaseName, phase.PhaseId.ToString());
                if (phase.IsPending)
                {
                    li.Attributes.Add("data-pending", "1");
                }
                else
                {
                    li.Attributes.Add("data-pending", "0");
                }
                drpExtendedStatus.Items.Add(li);
            }
            drpExtendedStatus.Items.Add(new ListItem(groupName, ">"));
            drpRequestor.DataValueField = "MemberId";
            drpRequestor.DataTextField = "ListName";
            IEnumerable<Member> activeJudges = ac.GetMembersByType(0, true);
            IEnumerable<Member> inActiveJudges = ac.GetMembersByType(0, false);
            foreach (Member member in activeJudges)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                drpRequestor.Items.Add(li);
            }
            drpRequestor.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveJudges)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                li.Attributes.Add("class", "inactive");
                drpRequestor.Items.Add(li);
            }
            drpRequestor.Items.Add(new ListItem("Inactive Members", ">"));
            drpRequestor.Items.Insert(0, new ListItem("All", ""));

        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            DateTime.TryParse(txtEndDate.Text, out DateTime endDate);
            if (endDate != null && startDate != null)
            {
                var ctl = new LogEntryListController();
                var cCtl = new CaseTypeController();
                IEnumerable<LogEntryListItem> lstHistory = ctl.GetReportLogItems(startDate, endDate, drpStatus.SelectedValue, drpCounty.SelectedValue, drpExtendedStatus.SelectedValue, drpRequestor.SelectedValue, drpAttorney.SelectedValue);
                var sb = new StringBuilder();
                sb.Append("<table class='caseReport'><tr><th>Case Type</th><th class='caseCount'>Case Count</th></tr>");
                var caseTypes = cCtl.GetActiveCaseTypes().OrderBy(x => x.CaseTypeName);
                var numberGroups = lstHistory
                    .GroupBy(h => h.CaseTypeName)
                    .Select(g => new { CaseType = g.Key, caseCount = g.Count() })
                    .OrderBy(g => g.CaseType)
                    .ToList();

                int grandTotal = 0;

                foreach (var c in caseTypes)
                {
                    int count = numberGroups
                        .Where(n => n.CaseType == c.CaseTypeName)
                        .Select(n => n.caseCount)
                        .FirstOrDefault();

                    grandTotal += count;
                    count = Math.Max(0, count);

                    sb.Append("<tr class='caseHeader'><td>");
                    sb.Append(c.CaseTypeName);
                    sb.Append("</td><td class='caseCount'>");
                    sb.Append(count.ToString());
                    sb.Append("</td></tr>");

                    if (chkShowDetail.Checked && count > 0)
                    {
                        sb.Append("<tr><td class='containerDetail'><table class='caseDetail'><tr><th>Motion Filed</th><th>Action Date</th><th>Case Name</th><th class='caseNumber'>Case Number</th><th>Responsible</th><th>Status</th></tr>");

                        var details = lstHistory
                            .Where(h => h.CaseTypeName == c.CaseTypeName)
                            .OrderBy(h => h.DateReceived)
                            .Select(h => new
                            {
                                h.Description,
                                h.CaseNumber,
                                h.AttorneyName,
                                h.DateReceived,
                                h.MotionFiled,
                                h.PhaseName
                            });

                        foreach (var d in details)
                        {
                            sb.Append("<tr><td>");
                            sb.Append(d.MotionFiled.HasValue ? d.MotionFiled.Value.ToShortDateString() : "&nbsp;");
                            sb.Append("</td><td>");
                            if (d.DateReceived.HasValue)
                                sb.Append(d.DateReceived.Value.ToShortDateString());
                            sb.Append("</td><td>");
                            sb.Append(d.Description);
                            sb.Append("</td><td class='caseNumber'>");
                            sb.Append(d.CaseNumber);
                            sb.Append("</td><td>");
                            sb.Append(d.AttorneyName);
                            sb.Append("</td><td>");
                            sb.Append(d.PhaseName);
                            sb.Append("</td></tr>");
                        }

                        sb.Append("</table></td><td>&nbsp;</td></tr>");
                    }
                }

                sb.Append("<tr class='totals'><td>&nbsp;Grand Total:</td><td class='gradTotal'>");
                sb.Append(grandTotal);
                sb.Append("</td></tr></table>");

                ltHistory.Text = sb.ToString();
            }

        }
    }
}