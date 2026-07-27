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
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.ExitSurvey.Components;

namespace tjc.Modules.ExitSurvey
{
    public partial class View : ExitSurveyModuleBase
    {
        // Greets the signed-in employee by name; falls back to "Employee" for anonymous.
        protected string UserDisplayName
        {
            get { return UserId > 0 && !string.IsNullOrWhiteSpace(UserInfo.DisplayName) ? UserInfo.DisplayName : "Employee"; }
        }

        // Bind the two rating matrices on every request (including postback) so their
        // rows exist before postback data is applied - otherwise the repeaters come
        // back empty on submit and ratings would neither validate nor save.
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            rptGeneral.DataSource = SurveyDefinition.GeneralConditions;
            rptGeneral.DataBind();

            rptSupervision.DataSource = SurveyDefinition.SupervisionItems;
            rptSupervision.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (IsAdmin)
                    {
                        lnkResults.Visible = true;
                        lnkResults.NavigateUrl = ResultsUrl;
                    }
                    BindForm();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindForm()
        {
            foreach (SurveyItem t in SurveyDefinition.EmployerTypes)
                rblEmployerType.Items.Add(new ListItem(t.Label, t.Key));

            // Q2 leads with an opt-out for employees not leaving for another job.
            cblAdvantages.Items.Add(new ListItem("Not Accepting employment elsewhere", "not_accepting"));
            foreach (SurveyItem r in SurveyDefinition.LeaveReasons)
            {
                cblAdvantages.Items.Add(new ListItem(r.Label, r.Key));
                cblReasons.Items.Add(new ListItem(r.Label, r.Key));
            }

            txtDateCompleted.Text = DateTime.Now.ToString("MM/dd/yyyy");

            AutoFillPersonalInfo();
        }

        // Pre-fills the Personal Info section from the EmployeeDB record that matches
        // the signed-in user. Fields stay editable so the employee can correct them.
        private void AutoFillPersonalInfo()
        {
            if (UserId <= 0 || UserInfo == null) return;

            var empCtl = new EmployeeLookupController();
            EmployeeLookup emp = empCtl.GetForUser(UserId, UserInfo.FirstName, UserInfo.LastName);
            if (emp == null) return;

            txtName.Text = emp.FullName;
            txtPositionTitle.Text = emp.Title ?? "";
            if (emp.HireDate.HasValue) txtHired.Text = emp.HireDate.Value.ToString("MM/yyyy");
            if (emp.TerminationDate.HasValue) txtSeparated.Text = emp.TerminationDate.Value.ToString("MM/yyyy");

            if (emp.SupervisorId.HasValue && emp.SupervisorId.Value > 0)
            {
                EmployeeLookup sup = empCtl.GetById(emp.SupervisorId.Value);
                if (sup != null)
                {
                    txtSupervisorName.Text = sup.FullName;
                    txtSupervisorTitle.Text = sup.Title ?? "";
                }
            }
        }

        // Fills the label and the 5-point rating list for a single matrix row.
        protected void rptRating_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            SurveyItem item = (SurveyItem)e.Item.DataItem;
            Literal ltLabel = (Literal)e.Item.FindControl("ltLabel");
            HiddenField hfKey = (HiddenField)e.Item.FindControl("hfKey");
            RadioButtonList rblRating = (RadioButtonList)e.Item.FindControl("rblRating");

            ltLabel.Text = Server.HtmlEncode(item.Label);
            hfKey.Value = item.Key;
            foreach (SurveyItem scale in SurveyDefinition.RatingScale)
                rblRating.Items.Add(new ListItem(scale.Label, scale.Key));

            // Only Q1 (general conditions) has an "Other (specify)" row; mark it so
            // the client can reveal the Q1 specify text box once it is rated.
            if (item.Key == "other")
                rblRating.CssClass += " q1-other-trigger";
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Client-side validation normally blocks submission; this is the
                // no-JS backstop and reports through the same SweetAlert dialog.
                List<string> errors = ValidateForm();
                if (errors.Count > 0)
                {
                    string items = "";
                    foreach (var er in errors) items += "<li>" + HttpUtility.HtmlEncode(er) + "</li>";
                    string html = "<p>Please complete the following before submitting:</p><ul style='text-align:left;'>" + items + "</ul>";
                    string script = "if(window.Swal){Swal.fire({title:'Some items still need your attention',icon:'error',html:" +
                        HttpUtility.JavaScriptStringEncode(html, true) + ",confirmButtonText:'OK'});}";
                    ScriptManager.RegisterStartupScript(this, GetType(), "esvalfail", script, true);
                    return;
                }

                var ctl = new ExitSurveyController();

                var response = new ExitSurveyResponse
                {
                    ModuleID = ModuleId,
                    DoNotShareWithSupervisor = chkDoNotShare.Checked,
                    Q1OtherSpecify = NullIfEmpty(txtQ1Other.Text),
                    Q2AdvantagesOther = NullIfEmpty(txtQ2Other.Text),
                    Q3AcceptedPosition = ParseYesNo(rblAccepted.SelectedValue),
                    Q4EmployerType = rblEmployerType.SelectedItem != null ? rblEmployerType.SelectedItem.Text : null,
                    Q4EmployerTypeOther = NullIfEmpty(txtQ4Other.Text),
                    Q5ReasonsOther = NullIfEmpty(txtQ5Other.Text),
                    Q7Training = NullIfEmpty(txtQ7.Text),
                    Q8Comments = NullIfEmpty(txtQ8.Text),
                    Q9WouldReturn = ParseYesNo(rblWouldReturn.SelectedValue),
                    OptName = NullIfEmpty(txtName.Text),
                    OptPositionTitle = NullIfEmpty(txtPositionTitle.Text),
                    OptSupervisorName = NullIfEmpty(txtSupervisorName.Text),
                    OptSupervisorTitle = NullIfEmpty(txtSupervisorTitle.Text),
                    OptHiredMonthYear = NullIfEmpty(txtHired.Text),
                    OptSeparatedMonthYear = NullIfEmpty(txtSeparated.Text),
                    OptDateCompleted = NullIfEmpty(txtDateCompleted.Text),
                    CreatedByUserId = UserId > 0 ? (int?)UserId : null,
                    CreatedByName = UserId > 0 ? UserInfo.DisplayName : null,
                    CreatedOnDate = DateTime.Now
                };

                int responseId = ctl.CreateResponse(response);

                SaveRatings(ctl, responseId, SurveyDefinition.SectionGeneral, SurveyDefinition.GeneralConditions, rptGeneral);
                SaveRatings(ctl, responseId, SurveyDefinition.SectionSupervision, SurveyDefinition.SupervisionItems, rptSupervision);

                SaveSelections(ctl, responseId, ExitSurveyReason.SectionAdvantages, cblAdvantages);
                SaveSelections(ctl, responseId, ExitSurveyReason.SectionLeave, cblReasons);

                pnlForm.Visible = false;
                plhMessage.Controls.Add(Skin.GetModuleMessageControl(
                    "Thank you!",
                    "Your exit survey has been submitted. Thank you for telling us about your experience working for the State Courts System. Best of luck in your future endeavors!",
                    ModuleMessage.ModuleMessageType.GreenSuccess));
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        // Server-side "everything is required" check. Q2 and Q4 apply only when the
        // employee accepted another position (Q3 = Yes); the Q1 "Other (specify)" row
        // is the one optional matrix item.
        private List<string> ValidateForm()
        {
            var errors = new List<string>();
            bool acceptedPosition = rblAccepted.SelectedValue == "1";

            if (!AllRowsRated(rptGeneral, SurveyDefinition.GeneralConditions, "other"))
                errors.Add("Question 1 - rate every work area.");

            if (!AnyChecked(cblAdvantages))
                errors.Add("Question 2 - select at least one option (or \"Not Accepting employment elsewhere\").");

            if (string.IsNullOrEmpty(rblAccepted.SelectedValue))
                errors.Add("Question 3 - indicate whether you accepted another position.");

            // Question 4 only applies when Q3 = Yes.
            if (acceptedPosition && rblEmployerType.SelectedIndex < 0)
                errors.Add("Question 4 - select the type of employer.");

            if (!AnyChecked(cblReasons))
                errors.Add("Question 5 - select at least one reason you left.");

            if (!AllRowsRated(rptSupervision, SurveyDefinition.SupervisionItems, null))
                errors.Add("Question 6 - rate every supervision item.");

            if (string.IsNullOrWhiteSpace(txtQ7.Text))
                errors.Add("Question 7 - describe your training/resources.");

            if (string.IsNullOrWhiteSpace(txtQ8.Text))
                errors.Add("Question 8 - add your comments.");

            if (string.IsNullOrEmpty(rblWouldReturn.SelectedValue))
                errors.Add("Question 9 - indicate whether you would return.");

            if (string.IsNullOrWhiteSpace(txtName.Text)) errors.Add("Personal Info - Name.");
            if (string.IsNullOrWhiteSpace(txtPositionTitle.Text)) errors.Add("Personal Info - Most recent position title.");
            if (string.IsNullOrWhiteSpace(txtSupervisorName.Text)) errors.Add("Personal Info - Name of Supervisor.");
            if (string.IsNullOrWhiteSpace(txtSupervisorTitle.Text)) errors.Add("Personal Info - Supervisor's Title.");
            if (string.IsNullOrWhiteSpace(txtHired.Text)) errors.Add("Personal Info - Month/Year you were hired.");
            if (string.IsNullOrWhiteSpace(txtSeparated.Text)) errors.Add("Personal Info - Month/Year you separated.");
            if (string.IsNullOrWhiteSpace(txtDateCompleted.Text)) errors.Add("Personal Info - Date Exit Survey Completed.");

            return errors;
        }

        // True when every matrix row (except an optional key) has a rating selected.
        private bool AllRowsRated(Repeater repeater, List<SurveyItem> items, string optionalKey)
        {
            for (int i = 0; i < items.Count && i < repeater.Items.Count; i++)
            {
                if (optionalKey != null && items[i].Key == optionalKey) continue;
                var rbl = (RadioButtonList)repeater.Items[i].FindControl("rblRating");
                if (rbl == null || string.IsNullOrEmpty(rbl.SelectedValue)) return false;
            }
            return true;
        }

        private static bool AnyChecked(CheckBoxList list)
        {
            foreach (ListItem li in list.Items)
                if (li.Selected) return true;
            return false;
        }

        // Walks a matrix repeater in list order and persists each selected rating.
        private void SaveRatings(ExitSurveyController ctl, int responseId, string section, List<SurveyItem> items, Repeater repeater)
        {
            for (int i = 0; i < items.Count && i < repeater.Items.Count; i++)
            {
                RadioButtonList rblRating = (RadioButtonList)repeater.Items[i].FindControl("rblRating");
                int rating = 0;
                int.TryParse(rblRating.SelectedValue, out rating);
                ctl.CreateRating(new ExitSurveyRating
                {
                    ResponseID = responseId,
                    Section = section,
                    ItemKey = items[i].Key,
                    ItemLabel = items[i].Label,
                    Rating = rating
                });
            }
        }

        // Persists the checked items of a multi-select question as reason rows.
        private void SaveSelections(ExitSurveyController ctl, int responseId, string section, CheckBoxList list)
        {
            foreach (ListItem li in list.Items)
            {
                if (li.Selected)
                    ctl.CreateReason(new ExitSurveyReason
                    {
                        ResponseID = responseId,
                        Section = section,
                        ReasonKey = li.Value,
                        ReasonLabel = li.Text
                    });
            }
        }

        private static string NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool? ParseYesNo(string value)
        {
            if (value == "1") return true;
            if (value == "0") return false;
            return null;
        }
    }
}
