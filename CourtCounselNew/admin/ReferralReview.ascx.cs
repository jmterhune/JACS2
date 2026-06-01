/*
' Copyright (c) 2022  Joe Terhune
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
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.CourtCounsel.Components;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.UI;
using System.Linq;
using static DotNetNuke.Modules.NavigationProvider.NavigationProvider;
using DotNetNuke.Web.UI.WebControls.Extensions;
using tjc.Modules.JudicialReferral.Components;

namespace tjc.Modules.CourtCounsel
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtCounselModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ReferralReview : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ReferralReview()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    chkReassign.InputAttributes.Add("class", "form-check-input");
                    chkReassign.LabelAttributes.Add("class", "form-check-label");

                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    if (UserInfo.IsInRole(AdminRole))
                    {
                        li1.Visible = true;
                        chkReassign.Visible = true;
                    }
                    PopulateDropDowns();
                    PopulateForm();
                    ShowMessages();

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void ShowMessages()
        {
            switch (LogRecordStatus)
            {
                case RecordStatus.created:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> New Case Record Created Successfully!";
                    break;
                case RecordStatus.updated:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> Case Record Updated Successfully!";
                    break;
                default:
                    break;
            }
            switch (AssignmentRecordStatus)
            {
                case RecordStatus.created:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> New Assignment Created Successfully!";

                    break;
                case RecordStatus.updated:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> Assignment Updated Successfully!";

                    break;
                case RecordStatus.future:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> New Pending Assignment Created Successfully!";
                    break;
                case RecordStatus.fileUpload:
                    ltMessage.Text += "<p class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> File(s) Uploaded Successfully!";
                    break;
                default:
                    break;
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new JudicialReferralController();
                tjc.Modules.JudicialReferral.Components.JudicialReferral objReferral = ctl.GetReferral(ReferralID);
                if (objReferral != null)
                {
                    if (UserId <= 0)
                        Response.Redirect(_navigationManager.NavigateURL());
                    var aCtl = new AssignmentController();
                    var lCtl = new LogEntryController();
                    Int32.TryParse(drpResponsible.SelectedValue, out int currentAttorney);
                    Int32.TryParse(drpRequestedBy.SelectedValue, out int currentJudge);
                    Int32.TryParse(drpStatus.SelectedValue, out int currentPhaseId);
                    Int32.TryParse(drpActionTaken.SelectedValue, out int currentActionId);
                    Int32.TryParse(drpCaseType.SelectedValue, out int currentCaseTypeId);
                    Int32.TryParse(drpTimeSpent.SelectedValue, out int currentTimeSpanId);
                    Int32.TryParse(drpCounty.SelectedValue, out int currentCountyId);
                    bool assignedDateHasValue = DateTime.TryParse(txtAssignedDate.Text, out DateTime currentDateReceived);
                    bool completedDateHasValue = DateTime.TryParse(txtDateCompleted.Text, out DateTime currentDateCompleted);
                    bool motionFiledDateHasValue = DateTime.TryParse(txtMotionFiled.Text, out DateTime currentDateMotionFiled);
                    LogEntry logEntry = new LogEntry
                    {
                        CaseNumber = GetCaseNumber(),
                        CountyId = currentCountyId,
                        Description = txtCaseName.Text,
                        CreatedBy = UserInfo.Username,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserInfo.Username,
                        ModifiedDate = DateTime.Now,
                        IsCase = GetIsCase()

                    };
                    if (!string.IsNullOrEmpty(hdLogId.Value))
                    {
                        long.TryParse(hdLogId.Value, out long logId);
                        LogEntry existingLogEntry = lCtl.GetLogEntry(logId);
                        if (existingLogEntry != null)
                        {
                            logEntry = existingLogEntry;
                        }
                        else
                        {
                            lCtl.CreateLogEntry(logEntry);
                        }
                    }
                    else
                    {
                        lCtl.CreateLogEntry(logEntry);
                    }
                    hdLogId.Value = logEntry.LogId.ToString();
                    Assignment assignment = new Assignment
                    {
                        LogId = logEntry.LogId,
                        Comments = txtComments.Text,
                        PreventReassignment = chkReassign.Checked,
                        ModifiedBy = UserInfo.Username,
                        DefendantName = txtDefendantName.Text,
                        DefendantSuffix = txtDefendantSuffix.Text,
                        ModifiedDate = DateTime.Now,
                        CreatedBy = UserInfo.Username,
                        CreatedDate = DateTime.Now,
                        StatusType = currentDateReceived > DateTime.Now ? StatusTypes.pending : StatusTypes.active
                    };
                    if (assignedDateHasValue)
                        assignment.DateReceived = currentDateReceived;
                    if (completedDateHasValue)
                        assignment.DateCompleted = currentDateCompleted;
                    if (motionFiledDateHasValue)
                        assignment.MotionFiled = currentDateMotionFiled;
                    if (currentAttorney > 0)
                        assignment.CurrentAttorneyId = currentAttorney;
                    if (currentJudge > 0)
                        assignment.CurrentJudiciaryId = currentJudge;
                    if (currentPhaseId > 0)
                        assignment.PhaseId = Int32.Parse(drpStatus.SelectedValue);
                    if (currentActionId > 0)
                        assignment.ActionId = currentActionId;
                    if (currentCaseTypeId > 0)
                        assignment.CaseTypeId = currentCaseTypeId;
                    if (currentTimeSpanId > 0)
                        assignment.TimeSpanId = currentTimeSpanId;
                    aCtl.CreateAssignment(assignment);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "<i class='fa fa-thumbs-up'></i> New Assignment Created SuccessFully!", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                    var jaCtl = new JudicialAssignmentController();
                    JudicialAssignment judicialAssignment = new JudicialAssignment { AssignmentId = assignment.AssignmentId, JudgeId = assignment.CurrentJudiciaryId, DateAssigned = DateTime.Now, Reason = txtReason.Text, CreatedBy = UserInfo.Username, CreatedDate = DateTime.Now, ModifiedBy = UserInfo.Username, ModifiedDate = DateTime.Now };
                    jaCtl.CreateJudicialAssignment(judicialAssignment);
                    if (assignment.DateReceived > DateTime.Now)
                    {
                        CreateEvent(logEntry, assignment);
                    }
                    objReferral.CounselReceivedDate = DateTime.Now;
                    ctl.UpdateReferral(objReferral);
                    Response.Redirect(EditUrl("referrals"), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }
        protected void valCaseNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
        }


        #endregion

        #region Methods
        private bool GetIsCase()
        {
            return true;
        }
        private void CreateEvent(LogEntry logEntry, Assignment assignment)
        {
            var mCtl = new MemberController();
            Member member = mCtl.GetMember(assignment.CurrentAttorneyId);
            var eCtl = new EventController();
            var pCtl = new PhaseController();
            string phaseName = "";
            Phase phase = pCtl.GetPhase(assignment.PhaseId);
            if (phase != null)
                phaseName = phase.PhaseName;
            eCtl.CreateEvent(new Event
            {
                AssignmentId = assignment.AssignmentId,
                UserName = member.UserName,
                Subject = string.Format("{0}: {1}", logEntry.CaseNumber, logEntry.Description),
                Body = String.Format("Assignment Status: {0}", phaseName),
                EndDate = assignment.DateReceived.Value.AddDays(1),
                CreatedBy = UserInfo.Username,
                CreatedDate = DateTime.Now,
                IsAllDay = true,
                IsReminderOn = true,
                ModifiedBy = UserInfo.Username,
                ModifiedDate = DateTime.Now,
                ReminderMinutesBeforeStart = 1440 * 4, //config reminder period
                StartDate = assignment.DateReceived.Value
            }, UserInfo.Email, PortalId);
        }
        private string GetCaseNumber()
        {
            return String.Format("{0}-{1}-{2}-{3}", drpCountyLetter.SelectedValue, txtCaseYear.Text, txtCaseType.Text, txtCaseSequence.Text);
        }
        private void PopulateDropDowns()
        {
            var countyCtl = new CountyController();
            var memberCtl = new MemberController();
            var timeCtl = new TimeSpanController();
            var statusCtl = new PhaseController();
            var actionCtl = new ActionController();
            var caseTypeCtl = new CaseTypeController();
            drpCounty.DataValueField = "CountyId";
            drpCounty.DataTextField = "CountyName";
            drpCounty.DataSource = countyCtl.GetCounties();
            drpCounty.DataBind();

            drpActionTaken.DataValueField = "ActionId";
            drpActionTaken.DataTextField = "ActionName";
            drpActionTaken.DataSource = actionCtl.GetActiveActions();
            drpActionTaken.DataBind();

            drpActionTaken.Items.Insert(0, new ListItem("< Select Option >", "0"));
            drpCaseType.DataValueField = "CaseTypeId";
            drpCaseType.DataTextField = "CaseTypeName";
            drpCaseType.DataSource = caseTypeCtl.GetActiveCaseTypes();
            drpCaseType.DataBind();
            drpCaseType.Items.Insert(0, new ListItem("< Select Option >", "0"));

            drpTimeSpent.DataValueField = "TimeSpanId";
            drpTimeSpent.DataTextField = "TimeSpanName";
            drpTimeSpent.DataSource = timeCtl.GetTimeSpans(true);
            drpTimeSpent.DataBind();
            drpTimeSpent.Items.Insert(0, new ListItem("< Select Option >", "0"));

            IEnumerable<Phase> phases = statusCtl.GetPhaseDropDownItems(true);
            string groupName = "";
            foreach (Phase phase in phases)
            {
                if (phase.GroupName != "Default")
                {
                    if (groupName != phase.GroupName && string.IsNullOrEmpty(groupName))
                    {
                        drpStatus.Items.Add(new ListItem(phase.GroupName, "<"));
                    }
                    if (groupName != phase.GroupName && !string.IsNullOrEmpty(groupName))
                    {
                        drpStatus.Items.Add(new ListItem(groupName, ">"));
                        drpStatus.Items.Add(new ListItem(phase.GroupName, "<"));
                    }
                    groupName = phase.GroupName;
                }

                ListItem li = new ListItem(phase.PhaseName, phase.PhaseId.ToString());
                if (phase.IsPending)
                {
                    li.Attributes.Add("data-pending", "1");
                }
                else
                {
                    li.Attributes.Add("data-pending", "0");
                }
                drpStatus.Items.Add(li);
            }
            drpStatus.Items.Add(new ListItem(groupName, ">"));
            drpResponsible.DataValueField = "MemberId";
            drpResponsible.DataTextField = "ListName";
            IEnumerable<Member> activeMembers = memberCtl.GetMembersByType(1, true);
            IEnumerable<Member> inActiveMembers = memberCtl.GetMembersByType(1, false);
            foreach (Member member in activeMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                drpResponsible.Items.Add(li);
            }
            drpResponsible.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                li.Attributes.Add("class", "inactive");
                drpResponsible.Items.Add(li);
            }
            drpResponsible.Items.Add(new ListItem("Inactive Members", ">"));

            drpRequestedBy.DataValueField = "MemberId";
            drpRequestedBy.DataTextField = "ListName";
            IEnumerable<Member> activeJudges = memberCtl.GetMembersByType(0, true);
            IEnumerable<Member> inActiveJudges = memberCtl.GetMembersByType(0, false);
            foreach (Member member in activeJudges)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                drpRequestedBy.Items.Add(li);
            }
            drpRequestedBy.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveJudges)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                li.Attributes.Add("class", "inactive");
                drpRequestedBy.Items.Add(li);
            }
            drpRequestedBy.Items.Add(new ListItem("Inactive Members", ">"));

        }
        private void PopulateForm()
        {
            var ctl = new JudicialReferralController();
            if (ReferralID > 0)
            {
                tjc.Modules.JudicialReferral.Components.JudicialReferral objReferral = ctl.GetReferral(ReferralID);
                var lCtl = new LogEntryController();
                LogEntry logEntry = lCtl.GetLogEntryByCaseNumber(objReferral.CaseNumber);
                if (logEntry != null)
                {
                    txtCaseName.Text = logEntry.Description;
                    drpCounty.SelectedValue = logEntry.CountyId.ToString();
                    hdLogId.Value = logEntry.LogId.ToString();
                }
                if (objReferral != null)
                {
                    rptFiles.DataSource = objReferral.Attachments;
                    rptFiles.DataBind();
                    if (rptFiles.Items.Count < 1)
                    {
                        rptFiles.Visible = false;
                        ltAttachments.Text = "No Attachment";
                    }
                    txtReferralCaseNumber.Text = objReferral.CaseNumber;
                    txtReferralCaseParties.Text = objReferral.CaseParties;
                    txtReferralMotionDate.Text = objReferral.MotionDate.Value.ToShortDateString();
                    txtReferralMotionTitle.Text = objReferral.MotionTitle;
                    txtReferralCaseType.Text = objReferral.CaseType;
                    txtReferralJudge.Text = objReferral.JudgeName;
                    var caseNumberArray = objReferral.CaseNumber.Split('-');
                    for (int i = 0; i < caseNumberArray.Length; i++)
                    {
                        if (i == 0)
                        {
                            drpCountyLetter.SelectedValue = caseNumberArray[i];
                            switch (caseNumberArray[i])
                            {
                                case "D":
                                    drpCounty.SelectedValue = "DeSoto";
                                    break;
                                case "M":
                                    drpCounty.SelectedValue = "Manatee";
                                    break;
                                default:
                                    drpCounty.SelectedValue = "Sarasota";
                                    break;
                            }
                        }
                        if (i == 1)
                            txtCaseYear.Text = caseNumberArray[i];
                        if (i == 2)
                            txtCaseType.Text += caseNumberArray[i];
                        if (i == 3)
                            txtCaseSequence.Text += caseNumberArray[i];
                        if (i == 4)
                            txtDefendantSuffix.Text += caseNumberArray[i];
                    }
                    txtCaseName.Text = objReferral.CaseParties.ToString();
                    if (objReferral.MotionDate.HasValue)
                        txtMotionFiled.Text = objReferral.MotionDate.Value.ToShortDateString();
                    switch (objReferral.CaseNumber.Substring(0,1))
                    {
                        case "M":
                            drpCounty.SelectedValue = "3";
                            break;
                        case "D":
                            drpCounty.SelectedValue = "1";
                            break;
                        default:
                            drpCounty.SelectedValue = "2";
                            break;
                    }
                    if (objReferral.JudgeID >0)
                    {
                        var jCtl = new MemberController();
                        var member=jCtl.GetMemberByUserId(objReferral.JudgeID);
                        if (member != null) { 
                        drpRequestedBy.SelectedValue=member.MemberId.ToString();
                        }
                    }
                    if (objReferral.JudgeResponseDate.HasValue) { 
                        txtAssignedDate.Text=objReferral.JudgeResponseDate.Value.ToShortDateString();
                    }
                    string motionText = "";
                    if (objReferral.MotionCorrect)
                        motionText += "<li><strong>3.800(b)</strong> Motion to Correct Sentencing Error</li>";
                    if (objReferral.MotionVacate)
                        motionText += "<li><strong>3.850</strong> Motion to Vacate, Set Aside, or Correct Sentence</li>";
                    if (motionText.Length > 0)
                        motionText = string.Format("<ul class='list'>{0}</ul>", motionText);
                    if (!string.IsNullOrEmpty(objReferral.DirectedMotions))
                    {
                        motionText += "<div class='alert alert-default'>The directed motions below shall be handled directly by the presiding judge unless the complexity of the issue warrants further assistance by Court Counsel</div>";
                        motionText += "<h5 class='d-inline'>Directed Motions:</h5>" + objReferral.DirectedMotions.Replace("|", "; ");
                    }
                    if (objReferral.MotionOther)
                        motionText += "<div class='mt-2'><h5 class='d-inline'>Other Motions:</h5>All other motions: Court Counsel will assist with all other motions, as referred by the presiding judge</div>";
                    if (!string.IsNullOrEmpty(objReferral.JudgeMotions))
                    {
                        motionText += "<h5>Judicial Instructions</h5><ul class='list'>";
                        foreach (string item in objReferral.JudgeMotions.Split('|'))
                        {
                            motionText += string.Format("<li>{0}</li>", item);
                        }
                        motionText += "</ul>";
                    }
                    ltMotions.Text = motionText;
                }

                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to find requested record", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
        }
        #endregion

    }
}