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
using System.Text.RegularExpressions;
using System.Web;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class CaseList : CourtCounselModuleBase
    {
        private string _partyName;

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSearch.NavigateUrl = SearchUrl;
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                _partyName = PartyName;
                var caseNumberPartial = CaseNumber;
                var ctrl = new HistoryController();

                if (!string.IsNullOrEmpty(caseNumberPartial))
                {
                    // Substring case-number search routed here from Search.ascx
                    litResultsHeading.Text = "Case Number Search Results";
                    var results = ctrl.SearchByCaseNumber(caseNumberPartial).ToList();
                    rptCaseList.DataSource = results;
                    rptCaseList.DataBind();
                }
                else if (!string.IsNullOrEmpty(_partyName))
                {
                    litResultsHeading.Text = "Case Name Search Results";
                    var results = ctrl.SearchByCaseName(_partyName).ToList();
                    rptCaseList.DataSource = results;
                    rptCaseList.DataBind();
                }
            }
        }

        /// <summary>
        /// Same logic as AttorneyCaseList.GetStatus — Active + non-empty StatusName
        /// shows the saved StatusName text; otherwise the bucket name.
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

        protected string FormatLongName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var displayName = name.Length > 30 ? name.Substring(0, 30) + "..." : name;

            if (!string.IsNullOrEmpty(_partyName))
            {
                // Highlight the search term (case-insensitive)
                displayName = Regex.Replace(
                    displayName,
                    Regex.Escape(HttpUtility.HtmlEncode(_partyName)),
                    m => "<span class=\"highLight\">" + m.Value + "</span>",
                    RegexOptions.IgnoreCase);
            }

            return displayName;
        }
    }
}
