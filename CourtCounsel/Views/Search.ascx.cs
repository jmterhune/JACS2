/*
' Copyright (c) 2026 Joe Terhune
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
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class Search : CourtCounselModuleBase, IActionable
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSearch.NavigateUrl = SearchUrl;
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                BindAttorneyDropDown();
            }
        }

        private void BindAttorneyDropDown()
        {
            var ctrl = new AttorneyController();
            var attorneys = ctrl.GetAttorneys().ToList();

            var active = attorneys.Where(a => a.IsActive == true).OrderBy(a => a.AttorneyName).ToList();
            var inactive = attorneys.Where(a => a.IsActive != true).OrderBy(a => a.AttorneyName).ToList();

            swAttorney.Items.Clear();
            swAttorney.Items.Add(new ListItem("-- Select Attorney --", ""));

            // Active group marker
            swAttorney.Items.Add(new ListItem("--- Active ---", "<"));
            foreach (var att in active)
            {
                swAttorney.Items.Add(new ListItem(att.AttorneyName, att.AttorneyName));
            }

            // Inactive group marker
            if (inactive.Any())
            {
                swAttorney.Items.Add(new ListItem("--- Inactive ---", ">"));
                foreach (var att in inactive)
                {
                    swAttorney.Items.Add(new ListItem(att.AttorneyName, att.AttorneyName));
                }
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            var searchType = hdSearchType.Value;

            switch (searchType)
            {
                case "1": // Case Name
                    var searchTerm = swSearchTerm.Text.Trim();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        Response.Redirect(EditUrl("pn", searchTerm, "CaseList"));
                    }
                    break;

                case "2": // Case Number
                    var caseNum = swSearchTerm.Text.Trim();
                    if (!string.IsNullOrEmpty(caseNum))
                    {
                        Response.Redirect(EditUrl("cn", caseNum, "CaseHistory"));
                    }
                    break;

                case "3": // Attorney
                    var attorneyName = swAttorney.SelectedValue;
                    if (!string.IsNullOrEmpty(attorneyName))
                    {
                        // Build combined status code matching VB sproc convention:
                        // A=Active, I=Inactive/Pending, C=Complete
                        // Combined: AI, AC, IC, AIC (same as "all")
                        string sf = "";
                        if (chkActive.Checked) sf += "A";
                        if (chkPending.Checked) sf += "I";
                        if (chkClosed.Checked) sf += "C";

                        if (string.IsNullOrEmpty(sf)) sf = "all";

                        var url = EditUrl("att", attorneyName, "AttorneyCaseList","sf=" + Server.UrlEncode(sf));
                        Response.Redirect(url);
                    }
                    break;
            }
        }

        protected string GetStatus(HistoryInfo item)
        {
            switch (item.Status)
            {
                case HistoryInfo.CurrentStatus.Active:
                    return "Active";
                case HistoryInfo.CurrentStatus.Inactive:
                    return "Pending";
                case HistoryInfo.CurrentStatus.Complete:
                    return "Completed";
                default:
                    return string.Empty;
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                return new ModuleActionCollection();
            }
        }
    }
}
