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
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class DataSheet : CourtCounselModuleBase
    {
        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] == null ? 1 : (int)ViewState["CurrentPage"]; }
            set { ViewState["CurrentPage"] = value; }
        }

        private int PageSize
        {
            get
            {
                if (ViewState["PageSize"] != null) return (int)ViewState["PageSize"];
                int size;
                return int.TryParse(drpPageSize.SelectedValue, out size) ? size : 50;
            }
            set { ViewState["PageSize"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                BindAttorneyCheckBoxList();
                BindRequestedByDropDown();
                BindSheet();
            }
        }

        private void BindRequestedByDropDown()
        {
            drpRequestedBy.Items.Clear();
            drpRequestedBy.Items.Add(new ListItem("-- All --", ""));
            var ctrl = new HistoryController();
            foreach (var name in ctrl.GetDistinctRequestedBy())
            {
                drpRequestedBy.Items.Add(new ListItem(name, name));
            }
        }

        private DateTime? GetDateReceivedFrom()
        {
            DateTime d;
            if (DateTime.TryParse(txtDateReceivedFrom.Text, out d))
                return d;
            return null;
        }

        private string GetSelectedRequestedBy()
        {
            return string.IsNullOrWhiteSpace(drpRequestedBy.SelectedValue) ? null : drpRequestedBy.SelectedValue;
        }

        private bool GetExcludeCompleted()
        {
            return string.Equals(drpCompletedFilter.SelectedValue, "exclude", StringComparison.OrdinalIgnoreCase);
        }

        private void BindAttorneyCheckBoxList()
        {
            var ctrl = new AttorneyController();
            var attorneys = ctrl.GetAttorneys().ToList();
            var active = attorneys.Where(a => a.IsActive == true).OrderBy(a => a.AttorneyName).ToList();
            var inactive = attorneys.Where(a => a.IsActive != true).OrderBy(a => a.AttorneyName).ToList();

            cblAttorneys.Items.Clear();
            foreach (var att in active)
            {
                cblAttorneys.Items.Add(new ListItem(att.AttorneyName, att.AttorneyName));
            }

            cblAttorneysInactive.Items.Clear();
            foreach (var att in inactive)
            {
                var item = new ListItem(att.AttorneyName, att.AttorneyName);
                item.Attributes.Add("disabled", "disabled");
                cblAttorneysInactive.Items.Add(item);
            }
            pnlInactiveAttorneys.Visible = inactive.Count > 0;
        }

        private List<string> GetSelectedAttorneys()
        {
            var selected = new List<string>();
            foreach (ListItem item in cblAttorneys.Items)
                if (item.Selected) selected.Add(item.Value);
            foreach (ListItem item in cblAttorneysInactive.Items)
                if (item.Selected) selected.Add(item.Value);
            return selected;
        }

        private void BindSheet()
        {
            var ctrl = new HistoryController();
            var selected = GetSelectedAttorneys();
            int pageSize = PageSize;

            var page = ctrl.GetHistoryPage(
                CurrentPage,
                pageSize,
                selected.Any() ? selected : null,
                GetDateReceivedFrom(),
                GetSelectedRequestedBy(),
                GetExcludeCompleted());

            // Server may have clamped the requested page if it was past the end
            CurrentPage = page.PageNumber;

            rptSheet.DataSource = page.Items;
            rptSheet.DataBind();

            long totalPages = page.TotalPages;
            if (totalPages < 1) totalPages = 1;

            lblPageInfo.Text = string.Format("&nbsp;Page {0} of {1}&nbsp;", page.PageNumber, totalPages);
            lblTotal.Text = string.Format("{0:N0} record{1}", page.TotalItems, page.TotalItems == 1 ? "" : "s");

            cmdFirst.Enabled = page.PageNumber > 1;
            cmdPrev.Enabled = page.PageNumber > 1;
            cmdNext.Enabled = page.PageNumber < totalPages;
            cmdLast.Enabled = page.PageNumber < totalPages;
        }

        protected void cmdFilter_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindSheet();
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblAttorneys.Items)
                item.Selected = false;
            foreach (ListItem item in cblAttorneysInactive.Items)
                item.Selected = false;

            txtDateReceivedFrom.Text = string.Empty;
            drpRequestedBy.SelectedValue = "";
            drpCompletedFilter.SelectedValue = "include";

            CurrentPage = 1;
            BindSheet();
        }

        protected void cmdFirst_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindSheet();
        }

        protected void cmdPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1) CurrentPage -= 1;
            BindSheet();
        }

        protected void cmdNext_Click(object sender, EventArgs e)
        {
            CurrentPage += 1;
            BindSheet();
        }

        protected void cmdLast_Click(object sender, EventArgs e)
        {
            // Bind with a very large page number; BindSheet's controller call will clamp to the last page.
            CurrentPage = int.MaxValue;
            BindSheet();
        }

        protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int size;
            if (int.TryParse(drpPageSize.SelectedValue, out size))
                PageSize = size;
            CurrentPage = 1;
            BindSheet();
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            var ctrl = new HistoryController();
            var selected = GetSelectedAttorneys();
            var rows = ctrl.GetHistoryForExport(
                selected.Any() ? selected : null,
                GetDateReceivedFrom(),
                GetSelectedRequestedBy(),
                GetExcludeCompleted()).ToList();

            var sb = new StringBuilder();
            sb.Append("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
            sb.Append("<head><meta charset=\"utf-8\" />");
            sb.Append("<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>");
            sb.Append("<x:Name>Data Sheet</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>");
            sb.Append("</x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->");
            sb.Append("</head><body>");
            sb.Append("<table border=\"1\" style=\"border-collapse:collapse;font-family:Calibri,Arial,sans-serif;font-size:11pt;\">");
            sb.Append("<thead><tr style=\"background-color:#343a40;color:#ffffff;font-weight:bold;\">");
            sb.Append("<th>Case Name</th><th>Case Type</th><th>Case Number</th><th>Date Received</th>");
            sb.Append("<th>Motion Filed</th><th>Requested By</th><th>Responsible</th><th>Action</th>");
            sb.Append("<th>Completed</th><th>Status</th>");
            sb.Append("</tr></thead><tbody>");

            foreach (var r in rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.PartyName)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.CaseType)).Append("</td>");
                sb.Append("<td style=\"mso-number-format:'\\@';\">").Append(HttpUtility.HtmlEncode(r.CaseNumber)).Append("</td>");
                sb.Append("<td style=\"mso-number-format:'m/d/yyyy';\">").Append(FormatDate(r.DateReceived)).Append("</td>");
                sb.Append("<td style=\"mso-number-format:'m/d/yyyy';\">").Append(FormatDate(r.MotionFiled)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.RequestedBy)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.Responsible)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.Action)).Append("</td>");
                sb.Append("<td style=\"mso-number-format:'m/d/yyyy';\">").Append(FormatDate(r.DateCompleted)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(r.StatusName)).Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table></body></html>");

            string fileName = "CourtCounsel_DataSheet_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.Write("\uFEFF"); // UTF-8 BOM so Excel recognizes encoding
            Response.Write(sb.ToString());
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private static string FormatDate(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("M/d/yyyy") : string.Empty;
        }

        private static string FormatDate(DateTime d)
        {
            return d.ToString("M/d/yyyy");
        }
    }
}
