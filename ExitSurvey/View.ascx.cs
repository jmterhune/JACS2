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
            rptGeneral.DataSource = SurveyDefinition.GeneralConditions;
            rptGeneral.DataBind();

            rptSupervision.DataSource = SurveyDefinition.SupervisionItems;
            rptSupervision.DataBind();

            foreach (SurveyItem t in SurveyDefinition.EmployerTypes)
                rblEmployerType.Items.Add(new ListItem(t.Label, t.Key));

            foreach (SurveyItem r in SurveyDefinition.LeaveReasons)
            {
                cblAdvantages.Items.Add(new ListItem(r.Label, r.Key));
                cblReasons.Items.Add(new ListItem(r.Label, r.Key));
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
