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
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class AttorneyCaseList : CourtCounselModuleBase
    {
        // True only when the status filter includes Completed (C / AC / IC / AIC / all).
        private bool _showCompletedColumn;

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSearch.NavigateUrl = SearchUrl;
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                var attorneyName = AttorneyName;
                var statusFilter = StatusFilter ?? string.Empty;

                // Match legacy behavior: hide the Completed column unless the filter requests completed records.
                _showCompletedColumn = statusFilter.IndexOf("C", StringComparison.OrdinalIgnoreCase) >= 0
                                       || statusFilter.Equals("all", StringComparison.OrdinalIgnoreCase);
                thCompleted.Visible = _showCompletedColumn;

                lblAttorneyName.Text = "Cases for " + Server.HtmlEncode(attorneyName);

                if (!string.IsNullOrEmpty(attorneyName))
                {
                    var ctrl = new HistoryController();
                    var results = ctrl.SearchByAttorney(attorneyName, statusFilter).ToList();
                    rptAttorneyCaseList.DataSource = results;
                    rptAttorneyCaseList.DataBind();
                }
            }
        }

        protected void rptAttorneyCaseList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var td = e.Item.FindControl("tdCompleted") as HtmlTableCell;
            if (td != null) td.Visible = _showCompletedColumn;
        }

        protected string FormatDate(DateTime? date)
        {
            if (date.HasValue) return date.Value.ToShortDateString();
            return string.Empty;
        }

        /// <summary>
        /// Truncate a long party name to 30 chars + ellipsis, matching the legacy AttorneyCaseList.vb FormatLongName helper.
        /// </summary>
        protected string FormatLongName(string longName)
        {
            if (string.IsNullOrEmpty(longName)) return string.Empty;
            return longName.Length > 30 ? longName.Substring(0, 30) + " ..." : longName;
        }

        /// <summary>
        /// Mirrors legacy GetStatus(status, statusName):
        ///   Active + non-empty StatusName -> show the saved StatusName text
        ///   Everything else                -> show the bucket name (Active / Inactive / Complete)
        /// </summary>
        protected string GetStatus(HistoryInfo item)
        {
            if (item == null) return string.Empty;

            if (item.Status == HistoryInfo.CurrentStatus.Active && !string.IsNullOrEmpty(item.StatusName))
                return item.StatusName;

            switch (item.Status)
            {
                case HistoryInfo.CurrentStatus.Active: return "Active";
                case HistoryInfo.CurrentStatus.Inactive: return "Inactive";
                case HistoryInfo.CurrentStatus.Complete: return "Complete";
                default: return string.Empty;
            }
        }
    }
}
