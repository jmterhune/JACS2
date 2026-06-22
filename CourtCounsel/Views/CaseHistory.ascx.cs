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
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class CaseHistory : CourtCounselModuleBase
    {
        private string _caseNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            _caseNumber = CaseNumber;

            // If we arrived via a LogId link but no case number, look it up
            if (string.IsNullOrEmpty(_caseNumber) && LogId > 0)
            {
                var ctrl = new HistoryController();
                var item = ctrl.GetHistory(LogId);
                if (item != null)
                    _caseNumber = item.CaseNumber;
            }

            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            var ctrl = new HistoryController();

            litCaseNumber.Text = _caseNumber;

            // Bind party names
            var names = ctrl.GetPartyNamesByCaseNumber(_caseNumber);
            rptNames.DataSource = names;
            rptNames.DataBind();

            // Bind history records — sort by DateCompleted desc, with null/open records at the top.
            var history = ctrl.GetHistoryByCaseNumber(_caseNumber)
                .OrderBy(h => h.DateCompleted.HasValue)        // false (null) first
                .ThenByDescending(h => h.DateCompleted)        // newest completed next
                .ThenByDescending(h => h.DateReceived);        // tiebreaker
            rptHistory.DataSource = history;
            rptHistory.DataBind();

            // Set Add New link
            lnkAddNew.NavigateUrl = EditUrl("EditHistory") + "?cn=" + Server.UrlEncode(_caseNumber);
        }

        protected void rptHistory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var logId = Convert.ToInt32(e.CommandArgument);
                var ctrl = new HistoryController();
                ctrl.DeleteHistory(logId);
                BindData();
            }
        }

        protected string FormatDate(object dateValue)
        {
            if (dateValue != null && dateValue != DBNull.Value)
            {
                return Convert.ToDateTime(dateValue).ToString("d");
            }
            return string.Empty;
        }

        /// <summary>
        /// ISO-8601 (yyyy-MM-dd) so DataTables can sort the date column
        /// lexicographically as if it were a real date. Emit as the cell's
        /// data-order attribute; the visible text stays in short-date form.
        /// Empty string for null sorts first under ASC (keeps open records on top).
        /// </summary>
        protected string FormatDateIso(object dateValue)
        {
            if (dateValue != null && dateValue != DBNull.Value)
            {
                return Convert.ToDateTime(dateValue).ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }
    }
}
