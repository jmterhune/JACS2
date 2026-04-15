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
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class AttorneyCaseList : CourtCounselModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSearch.NavigateUrl = SearchUrl;
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                var attorneyName = AttorneyName;
                var statusFilter = StatusFilter;

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

        protected string FormatDate(DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToShortDateString();
            return string.Empty;
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
    }
}
