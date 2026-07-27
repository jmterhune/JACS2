/*
' Copyright (c) 2026  Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.ExitSurvey.Components;

namespace tjc.Modules.ExitSurvey.Admin
{
    public partial class Results : ExitSurveyModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsAdmin)
                {
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
                    return;
                }
                if (!IsPostBack)
                    BindResults();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindResults()
        {
            var ctl = new ExitSurveyController();
            var responses = ctl.GetResponses(ModuleId).ToList();
            pnlEmpty.Visible = responses.Count == 0;
            rptResults.DataSource = responses;
            rptResults.DataBind();
        }

        protected void rptResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var response = (ExitSurveyResponse)e.Item.DataItem;

            ((Literal)e.Item.FindControl("ltSubmitted")).Text = response.CreatedOnDate.ToString("MM/dd/yyyy h:mm tt");
            ((Literal)e.Item.FindControl("ltName")).Text = Server.HtmlEncode(
                !string.IsNullOrWhiteSpace(response.OptName) ? response.OptName :
                (!string.IsNullOrWhiteSpace(response.CreatedByName) ? response.CreatedByName : "(anonymous)"));
            ((Literal)e.Item.FindControl("ltAccepted")).Text = YesNo(response.Q3AcceptedPosition);
            ((Literal)e.Item.FindControl("ltReturn")).Text = YesNo(response.Q9WouldReturn);
            ((Literal)e.Item.FindControl("ltShare")).Text = response.DoNotShareWithSupervisor ? "No" : "Yes";

            var lnkView = (HyperLink)e.Item.FindControl("lnkView");
            lnkView.NavigateUrl = EditUrl("rid", response.ResponseID.ToString(), "detail");

            // Print icon opens the detail in print mode (auto-fires the browser print dialog).
            var lnkPrint = (HyperLink)e.Item.FindControl("lnkPrint");
            lnkPrint.NavigateUrl = EditUrl("rid", response.ResponseID.ToString(), "detail", "print=1");
        }

        private static string YesNo(bool? value)
        {
            if (value == true) return "Yes";
            if (value == false) return "No";
            return "";
        }
    }
}
