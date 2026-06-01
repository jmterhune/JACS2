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

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class UpdateCaseName : CourtCounselModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                pnlResults.Visible = false;
                pnlMessage.Visible = false;
            }
        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
            pnlMessage.Visible = false;
            var caseNumber = txtCaseNumber.Text.Trim();

            if (string.IsNullOrEmpty(caseNumber))
                return;

            var ctrl = new HistoryController();
            var results = ctrl.GetHistoryByCaseNumber(caseNumber).ToList();

            if (results.Any())
            {
                rptResults.DataSource = results;
                rptResults.DataBind();
                pnlResults.Visible = true;
            }
            else
            {
                pnlResults.Visible = false;
                pnlMessage.CssClass = "alert alert-warning";
                ltMessage.Text = "No records found for case number: " + Server.HtmlEncode(caseNumber);
                pnlMessage.Visible = true;
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            var caseNumber = txtCaseNumber.Text.Trim();
            var newName = txtNewCaseName.Text.Trim();

            if (string.IsNullOrEmpty(caseNumber) || string.IsNullOrEmpty(newName))
                return;

            var ctrl = new HistoryController();
            ctrl.UpdateCaseName(caseNumber, newName);

            // Rebind to show updated names
            var results = ctrl.GetHistoryByCaseNumber(caseNumber).ToList();
            rptResults.DataSource = results;
            rptResults.DataBind();

            pnlMessage.CssClass = "alert alert-success";
            ltMessage.Text = "Case name updated successfully to: " + Server.HtmlEncode(newName);
            pnlMessage.Visible = true;
            txtNewCaseName.Text = "";
        }
    }
}
