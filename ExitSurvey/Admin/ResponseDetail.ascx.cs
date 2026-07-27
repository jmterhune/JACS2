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
using System.Web;
using System.Web.UI;
using tjc.Modules.ExitSurvey.Components;

namespace tjc.Modules.ExitSurvey.Admin
{
    public partial class ResponseDetail : ExitSurveyModuleBase
    {
        private int ResponseId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["rid"], out id) ? id : -1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsAdmin)
                {
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
                    return;
                }
                lnkBack.NavigateUrl = ResultsUrl;
                if (!IsPostBack)
                {
                    BindDetail();
                    // When reached via the results print icon, open the browser print
                    // dialog automatically so the admin can save/print a filing copy.
                    if (pnlDetail.Visible && Request.QueryString["print"] == "1")
                        ScriptManager.RegisterStartupScript(this, GetType(), "esprint",
                            "window.addEventListener('load', function(){ setTimeout(function(){ window.print(); }, 300); });", true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindDetail()
        {
            var ctl = new ExitSurveyController();
            var response = ctl.GetResponse(ResponseId);
            if (response == null || response.ModuleID != ModuleId)
            {
                pnlDetail.Visible = false;
                return;
            }

            ltSubmitted.Text = response.CreatedOnDate.ToString("MM/dd/yyyy h:mm tt");
            ltSubmittedBy.Text = Enc(!string.IsNullOrWhiteSpace(response.CreatedByName) ? response.CreatedByName : "(anonymous)");
            ltShare.Text = response.DoNotShareWithSupervisor
                ? "No - employee requested this survey NOT be shared with their supervisor"
                : "Yes";

            rptGeneral.DataSource = ctl.GetRatings(response.ResponseID, SurveyDefinition.SectionGeneral);
            rptGeneral.DataBind();
            ltQ1Other.Text = Enc(response.Q1OtherSpecify);
            pnlQ1Other.Visible = !string.IsNullOrWhiteSpace(response.Q1OtherSpecify);

            rptAdvantages.DataSource = ctl.GetReasons(response.ResponseID, ExitSurveyReason.SectionAdvantages);
            rptAdvantages.DataBind();
            ltQ2Other.Text = Enc(response.Q2AdvantagesOther);
            pnlQ2Other.Visible = !string.IsNullOrWhiteSpace(response.Q2AdvantagesOther);
            ltQ3.Text = YesNo(response.Q3AcceptedPosition);

            ltQ4.Text = Enc(response.Q4EmployerType);
            if (!string.IsNullOrWhiteSpace(response.Q4EmployerTypeOther))
                ltQ4.Text += (string.IsNullOrEmpty(ltQ4.Text) ? "" : " &ndash; ") + Enc(response.Q4EmployerTypeOther);

            rptReasons.DataSource = ctl.GetReasons(response.ResponseID, ExitSurveyReason.SectionLeave);
            rptReasons.DataBind();
            ltQ5Other.Text = Enc(response.Q5ReasonsOther);
            pnlQ5Other.Visible = !string.IsNullOrWhiteSpace(response.Q5ReasonsOther);

            rptSupervision.DataSource = ctl.GetRatings(response.ResponseID, SurveyDefinition.SectionSupervision);
            rptSupervision.DataBind();

            ltQ7.Text = Multiline(response.Q7Training);
            ltQ8.Text = Multiline(response.Q8Comments);
            ltQ9.Text = YesNo(response.Q9WouldReturn);

            ltName.Text = Enc(response.OptName);
            ltPositionTitle.Text = Enc(response.OptPositionTitle);
            ltSupervisorName.Text = Enc(response.OptSupervisorName);
            ltSupervisorTitle.Text = Enc(response.OptSupervisorTitle);
            ltHired.Text = Enc(response.OptHiredMonthYear);
            ltSeparated.Text = Enc(response.OptSeparatedMonthYear);
            ltDateCompleted.Text = Enc(response.OptDateCompleted);
        }

        private static string Enc(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlEncode(value);
        }

        private static string Multiline(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlEncode(value).Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        private static string YesNo(bool? value)
        {
            if (value == true) return "Yes";
            if (value == false) return "No";
            return "";
        }
    }
}
