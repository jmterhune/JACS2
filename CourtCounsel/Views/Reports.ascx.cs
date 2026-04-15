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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class Reports : CourtCounselModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                BindDropDowns();
            }
        }

        private void BindDropDowns()
        {
            // Counties
            var countyCtrl = new CountyController();
            var counties = countyCtrl.GetCounties().OrderBy(c => c.County).ToList();
            drpCounty.Items.Clear();
            drpCounty.Items.Add(new ListItem("-- All --", ""));
            foreach (var c in counties)
            {
                drpCounty.Items.Add(new ListItem(c.County, c.County));
            }

            // Requestors
            var reqCtrl = new RequestorController();
            var requestors = reqCtrl.GetRequestors().OrderBy(r => r.RequestorName).ToList();
            drpRequestor.Items.Clear();
            drpRequestor.Items.Add(new ListItem("-- All --", ""));
            foreach (var r in requestors)
            {
                drpRequestor.Items.Add(new ListItem(r.RequestorName, r.RequestorName));
            }

            // Attorneys
            var attCtrl = new AttorneyController();
            var attorneys = attCtrl.GetActiveAttorneys().OrderBy(a => a.AttorneyName).ToList();
            cblAttorneys.Items.Clear();
            foreach (var a in attorneys)
            {
                cblAttorneys.Items.Add(new ListItem(a.AttorneyName, a.AttorneyName));
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(txtStartDate.Text))
                startDate = DateTime.Parse(txtStartDate.Text);
            if (!string.IsNullOrEmpty(txtEndDate.Text))
                endDate = DateTime.Parse(txtEndDate.Text);

            var statusFilter = rblStatus.SelectedValue;
            var extendedStatus = drpStatus.SelectedValue;
            var county = drpCounty.SelectedValue;
            var requestor = drpRequestor.SelectedValue;

            // Get selected attorneys
            var selectedAttorneys = new List<string>();
            foreach (ListItem item in cblAttorneys.Items)
            {
                if (item.Selected)
                    selectedAttorneys.Add(item.Value);
            }

            var ctrl = new HistoryController();
            var attorney = selectedAttorneys.Count == 1 ? selectedAttorneys.First() : "";
            var results = ctrl.GetFilteredHistory(startDate, endDate, statusFilter, extendedStatus, attorney, county, requestor);

            // If multiple attorneys selected, filter in memory
            if (selectedAttorneys.Count > 1)
            {
                results = results.Where(h => selectedAttorneys.Contains(h.Responsible));
            }
            else if (selectedAttorneys.Count == 0)
            {
                // No filter - show all
            }

            var resultList = results.ToList();
            var showDetail = chkShowDetail.Checked;

            ltResults.Text = BuildReportHtml(resultList, showDetail);
        }

        private string BuildReportHtml(List<HistoryInfo> data, bool showDetail)
        {
            if (!data.Any())
            {
                return "<div class='alert alert-info mt-3'>No records found matching the selected criteria.</div>";
            }

            var grouped = data.GroupBy(h => h.CaseType ?? "Unassigned").OrderBy(g => g.Key);
            var sb = new StringBuilder();
            int grandTotal = 0;

            sb.Append("<table class='caseReport table table-bordered'>");
            sb.Append("<tr class='caseHeader'><td>Case Type</td><td class='caseCount'>Count</td></tr>");

            foreach (var group in grouped)
            {
                var count = group.Count();
                grandTotal += count;

                sb.AppendFormat("<tr><td>{0}</td><td class='caseCount'>{1}</td></tr>",
                    System.Web.HttpUtility.HtmlEncode(group.Key), count);

                if (showDetail)
                {
                    sb.Append("<tr><td colspan='2'>");
                    sb.Append("<table class='caseDetail'>");
                    sb.Append("<tr class='caseHeader'><td>Case Number</td><td>Party Name</td><td>Date Received</td><td>Responsible</td><td>Status</td></tr>");

                    foreach (var item in group.OrderBy(h => h.CaseNumber))
                    {
                        sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2:d}</td><td>{3}</td><td>{4}</td></tr>",
                            System.Web.HttpUtility.HtmlEncode(item.CaseNumber),
                            System.Web.HttpUtility.HtmlEncode(item.PartyName),
                            item.DateReceived,
                            System.Web.HttpUtility.HtmlEncode(item.Responsible),
                            System.Web.HttpUtility.HtmlEncode(item.StatusName));
                    }

                    sb.Append("</table>");
                    sb.Append("</td></tr>");
                }
            }

            sb.AppendFormat("<tr class='totals'><td class='gradTotal'>Grand Total</td><td class='gradTotal'>{0}</td></tr>", grandTotal);
            sb.Append("</table>");

            sb.AppendFormat("<p class='note'>Report generated {0:g}. Total records: {1}</p>", DateTime.Now, grandTotal);

            return sb.ToString();
        }

        protected void cmdReset_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            rblStatus.SelectedIndex = 0;
            drpStatus.SelectedIndex = 0;
            drpCounty.SelectedIndex = 0;
            drpRequestor.SelectedIndex = 0;
            chkShowDetail.Checked = false;

            foreach (ListItem item in cblAttorneys.Items)
            {
                item.Selected = false;
            }

            ltResults.Text = "";
        }
    }
}
