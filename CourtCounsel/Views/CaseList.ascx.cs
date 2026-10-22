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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class CaseList : CourtCounselModuleBase
    {
        private string _partyName;

        // True only when the search was narrowed by Action Type, so the matching action is worth showing.
        private bool _showActionColumn;

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSearch.NavigateUrl = SearchUrl;
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                _partyName = PartyName;
                var caseNumberPartial = CaseNumber;
                var ctrl = new HistoryController(_hostSettings);

                if (!string.IsNullOrEmpty(caseNumberPartial))
                {
                    // Substring case-number search routed here from Search.ascx
                    litResultsHeading.Text = "Case Number Search Results";
                    var results = ctrl.SearchByCaseNumber(caseNumberPartial).ToList();
                    rptCaseList.DataSource = results;
                    rptCaseList.DataBind();
                }
                else if (!string.IsNullOrEmpty(_partyName) || ActionIds.Count > 0)
                {
                    litResultsHeading.Text = "Case Name Search Results";

                    var actionNames = new System.Collections.Generic.List<string>();
                    if (ActionIds.Count > 0)
                    {
                        actionNames = new ActionTakenController(_hostSettings).GetActions()
                            .Where(a => ActionIds.Contains(a.ActionId))
                            .Select(a => a.Action)
                            .OrderBy(a => a)
                            .ToList();
                        litResultsHeading.Text += " <small class=\"text-muted\">Action: " +
                            HttpUtility.HtmlEncode(string.Join(", ", actionNames)) + "</small>";
                        thAction.Visible = true;
                    }

                    _showActionColumn = actionNames.Count > 0;
                    var results = ctrl.SearchByCaseName(_partyName, actionNames).ToList();
                    rptCaseList.DataSource = results;
                    rptCaseList.DataBind();
                }
            }
        }

        protected void rptCaseList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var td = e.Item.FindControl("tdAction") as HtmlTableCell;
            if (td != null) td.Visible = _showActionColumn;
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
