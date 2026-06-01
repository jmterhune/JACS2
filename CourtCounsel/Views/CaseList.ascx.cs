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

                if (!string.IsNullOrEmpty(_partyName))
                {
                    var ctrl = new HistoryController();
                    var results = ctrl.SearchByCaseName(_partyName).ToList();
                    rptCaseList.DataSource = results;
                    rptCaseList.DataBind();
                }
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
