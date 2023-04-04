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
    public partial class LogEdit : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public LogEdit()
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

                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    if (UserInfo.IsInRole(AdminRole))
                    {
                        li1.Visible = true;
                        chkReassign.Visible = true;
                    }
                    PopulateDropDowns();
                    if (AssignmentId > 0)
                    {
                        PopulateForm(AssignmentId);
                        BindEvents();
                        BindFiles();
                    }
                    else
                    {
                        pnlUpdateEvent.Visible = false;
                        pnlUpdateFiles.Visible = false;
                    }
                    ShowMessages();

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            ltEventMessage.Text = "";
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
            int logRecordStatus = -1;
            int assignmentRecordStatus = -1;
            if (AssignmentId > 0)
            {
                Assignment assignment = aCtl.GetAssignment(AssignmentId);
                LogEntry logEntry = lCtl.GetLogEntry(assignment.LogId);
                long existingJudge = assignment.CurrentJudiciaryId;
                DateTime originalReceivedDate = assignment.DateReceived.Value;
                if (hdCaseInfoChanged.Value == "1")
                {
                    logEntry.Description = txtCaseName.Text;
                    logEntry.CaseNumber = GetCaseNumber();
                    logEntry.CountyId = currentCountyId;
                    logEntry.ModifiedBy = UserInfo.Username;
                    logEntry.ModifiedDate = DateTime.Now;
                    lCtl.UpdateLogEntry(logEntry);
                    hdCaseInfoChanged.Value = "";
                    logRecordStatus = (int)RecordStatus.updated;
                }

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

                assignment.Comments = txtComments.Text;
                assignment.PreventReassignment = chkReassign.Checked;
                assignment.ModifiedBy = UserInfo.Username;
                assignment.ModifiedDate = DateTime.Now;

                if (existingJudge != currentJudge)
                {
                    var jaCtl = new JudicialAssignmentController();
                    JudicialAssignment judicialAssignment = new JudicialAssignment
                    {
                        AssignmentId = assignment.AssignmentId,
                        JudgeId = currentAttorney,
                        DateAssigned = DateTime.Now,
                        Reason = txtReason.Text,
                        CreatedBy = UserInfo.Username,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserInfo.Username,
                        ModifiedDate = DateTime.Now
                    };
                    jaCtl.CreateJudicialAssignment(judicialAssignment);
                }
                if (assignment.DateReceived > DateTime.Now && originalReceivedDate <= DateTime.Now)
                {
                    Assignment newAssignment = new Assignment
                    {
                        ModifiedBy = assignment.ModifiedBy,
                        LogId = assignment.LogId,
                        DateReceived = assignment.DateReceived,
                        ActionId = assignment.ActionId,
                        CaseTypeId = assignment.CaseTypeId,
                        Comments = assignment.Comments,
                        CreatedBy = assignment.ModifiedBy,
                        CreatedDate = assignment.CreatedDate,
                        CurrentAttorneyId = assignment.CurrentAttorneyId,
                        CurrentJudiciaryId = assignment.CurrentJudiciaryId,
                        ModifiedDate = assignment.ModifiedDate,
                        MotionFiled = assignment.MotionFiled,
                        PhaseId = assignment.PhaseId,
                        PreventReassignment = assignment.PreventReassignment,
                        StatusType = StatusTypes.pending,
                        TimeSpanId = assignment.TimeSpanId,
                    };
                    aCtl.CreateAssignment(newAssignment); //Do we want to copy Files and Events?
                    CreateEvent(logEntry, newAssignment);
                    Response.Redirect(EditUrl("aid", newAssignment.AssignmentId.ToString(), "logedit", "as=" + (int)RecordStatus.future, "ls=" + logRecordStatus), true);
                }
                else
                {
                    if (assignment.DateCompleted.HasValue)
                        assignment.StatusType = StatusTypes.closed;
                    aCtl.UpdateAssignment(assignment);
                    Response.Redirect(EditUrl("aid", assignment.AssignmentId.ToString(), "logedit", "as=" + (int)RecordStatus.updated, "ls=" + logRecordStatus), true);
                }
            }
            else
            {
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
                        logRecordStatus = (int)RecordStatus.created;
                    }
                }
                else
                {
                    lCtl.CreateLogEntry(logEntry);
                    logRecordStatus = (int)RecordStatus.created;
                }
                hdLogId.Value = logEntry.LogId.ToString();
                Assignment assignment = new Assignment
                {
                    LogId = logEntry.LogId,
                    Comments = txtComments.Text,
                    PreventReassignment = chkReassign.Checked,
                    ModifiedBy = UserInfo.Username,
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
                assignmentRecordStatus = (int)RecordStatus.created;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "<i class='fa fa-thumbs-up'></i> New Assignment Created SuccessFully!", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                var jaCtl = new JudicialAssignmentController();
                JudicialAssignment judicialAssignment = new JudicialAssignment { AssignmentId = assignment.AssignmentId, JudgeId = assignment.CurrentJudiciaryId, DateAssigned = DateTime.Now, Reason = txtReason.Text, CreatedBy = UserInfo.Username, CreatedDate = DateTime.Now, ModifiedBy = UserInfo.Username, ModifiedDate = DateTime.Now };
                jaCtl.CreateJudicialAssignment(judicialAssignment);
                if (assignment.DateReceived > DateTime.Now)
                {
                    CreateEvent(logEntry, assignment);
                    assignmentRecordStatus = (int)RecordStatus.future;
                }
                Response.Redirect(EditUrl("aid", assignment.AssignmentId.ToString(), "logedit", "as=" + assignmentRecordStatus, "ls=" + logRecordStatus), true);
            }
        }
        protected void valCaseNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
        }

        protected void cmdSubmitEvent_Click(object sender, EventArgs e)
        {
            var ctl = new EventController();
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            Int32.TryParse(txtReminderDays.Text, out int reminderDays);
            Event @event = new Event { ExternalId = hdExternalId.Value, AssignmentId = AssignmentId, Subject = txtSubject.Text, Body = txtBody.Text, StartDate = startDate, EndDate = startDate.AddDays(1), IsAllDay = true, IsReminderOn = true, ReminderMinutesBeforeStart = reminderDays * 1440, UserName = UserInfo.Email, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, CreatedBy = UserInfo.Username, ModifiedBy = UserInfo.Username, };
            if (!string.IsNullOrEmpty(hdExternalId.Value))
            {
                if (!ctl.UpdateEvent(@event, UserInfo.Email, PortalId))
                {
                    ltEventMessage.Text = string.Format("<div class='alert alert-danger fade-alert'><i class='fa fa-warning'></i> Could not update selected event.</div>");

                }
                ltEventMessage.Text = string.Format("<div class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> Selected Event Updated!</div>");

            }
            else
            {
                ctl.CreateEvent(@event, UserInfo.Email, PortalId);

            }

            BindEvents();
        }
        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var ctl = new EventController();
                long.TryParse(e.CommandArgument.ToString(), out long eventId);
                if (eventId > 0)
                {
                    ctl.DeleteEvent(eventId, UserInfo.Email, PortalId);
                    BindEvents();
                    ltEventMessage.Text = string.Format("<div class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> Selected Event Deleted!</div>");

                }
                else
                {
                    ltEventMessage.Text = string.Format("<div class='alert alert-danger fade-alert'><i class='fa fa-warning'></i> Could not delete selected event.</div>");
                }
            }
            if (e.CommandName == "edit")
            {
                var ctl = new EventController();
                long.TryParse(e.CommandArgument.ToString(), out long eventId);
                if (eventId > 0)
                {
                    Event @event = ctl.GetEvent(eventId);
                    txtStartDate.Text = @event.StartDate.ToString("yyyy-MM-dd");
                    txtReminderDays.Text = @event.ReminderDays.ToString();
                    txtSubject.Text = @event.Subject;
                    txtBody.Text = @event.Body;
                    hdExternalId.Value = @event.ExternalId;
                }
                ScriptManager.RegisterStartupScript(rptEvents, rptEvents.GetType(), "ShowEvent", "ToggleEventModal()", true);
            }
        }

        protected void rptEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) ||
            (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                LinkButton cmdEditEvent = (LinkButton)e.Item.FindControl("cmdEditEvent");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                Event @event = (Event)e.Item.DataItem;
                if (@event.CreatedBy != UserInfo.Username)
                {
                    cmdEditEvent.Visible = false;
                    cmdDelete.Visible = false;
                }
            }
        }
        protected void pnlUpdateEvent_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }

        protected void rptEvents_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);

                LinkButton cmdEditEvent = (LinkButton)e.Item.FindControl("cmdEditEvent");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEditEvent);
            }
        }
        protected void cmdSubmitFile_Click(object sender, EventArgs e)
        {
            if (uplFiles.HasFiles)
            {
                foreach (var postedFile in uplFiles.PostedFiles)
                {
                    var ctl = new FileController();
                    File file = new File
                    {
                        AssignmentId = AssignmentId,
                        CreatedBy = UserInfo.Username,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserInfo.Username,
                        ModifiedDate = DateTime.Now,
                        DriveId = DocumentDriveId,
                        ParentId = OrdersDriveId,
                        FileName = postedFile.FileName
                    };
                    var existingFile = ctl.GetFilesByFileName(AssignmentId, postedFile.FileName);
                    if (existingFile != null)
                        file = existingFile;
                    file.FileStream = postedFile.InputStream;
                    ctl.CreateFile(file, GetCaseNumber(), PortalId);

                }
                Response.Redirect(EditUrl("aid", AssignmentId.ToString(), "logedit", "as=" + (int)RecordStatus.fileUpload), true);

            }
            BindFiles();
        }
        protected void rptFiles_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);

                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
            }
        }

        protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var ctl = new FileController();
                long.TryParse(e.CommandArgument.ToString(), out long fileId);
                if (fileId > 0)
                {
                    ctl.DeleteFile(fileId,  PortalId);
                    BindFiles();
                    ltFileMessage.Text = string.Format("<div class='alert alert-success fade-alert'><i class='fa fa-thumbs-up'></i> Selected File Deleted!</div>");

                }
                else
                {
                    ltFileMessage.Text = string.Format("<div class='alert alert-danger fade-alert'><i class='fa fa-warning'></i> Could not delete selected File.</div>");
                }
            }
        }

        protected void rptFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) ||
            (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                File file = (File)e.Item.DataItem;
                if (file.CreatedBy != UserInfo.Username)
                {
                    cmdDelete.Visible = false;
                }
            }
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
            txtReminderDays.Text = "10";
            var countyCtl = new tjc.Modules.Globals.CountyController();
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
        private void PopulateForm(int assignmentId)
        {
            drpCountyLetter.Attributes.Add("disabled", "disabled");
            txtCaseYear.Attributes.Add("disabled", "disabled");
            txtCaseType.Attributes.Add("disabled", "disabled");
            txtCaseSequence.Attributes.Add("disabled", "disabled");
            txtCaseName.Attributes.Add("disabled", "disabled");
            drpCounty.Attributes.Add("disabled", "disabled");
            var ctl = new AssignmentController();
            Assignment assignment = ctl.GetAssignment(assignmentId);
            var ctlLog = new LogEntryController();
            LogEntry logEntry = ctlLog.GetLogEntry(assignment.LogId);
            string[] caseNumber = logEntry.CaseNumber.Split('-');
            drpCountyLetter.SelectedValue = caseNumber[0];
            txtCaseYear.Text = caseNumber[1];
            txtCaseType.Text = caseNumber[2];
            txtCaseSequence.Text = caseNumber[3];
            for (int i = 4; i < caseNumber.Length; i++)
            {
                txtCaseSequence.Text += string.Format("-{0}", caseNumber[i]);
            }

            drpCaseType.SelectedValue = assignment.CaseTypeId.ToString();
            txtCaseName.Text = logEntry.Description;
            drpCounty.SelectedValue = logEntry.CountyId.ToString();
            if (assignment.DateReceived.HasValue)
                txtAssignedDate.Text = assignment.DateReceived.Value.ToString("yyyy-MM-dd");
            drpActionTaken.SelectedValue = assignment.ActionId.ToString();
            drpRequestedBy.SelectedValue = assignment.CurrentJudiciaryId.ToString();
            drpResponsible.SelectedValue = assignment.CurrentAttorneyId.ToString();
            if (assignment.MotionFiled.HasValue)
                txtMotionFiled.Text = assignment.MotionFiled.Value.ToString("yyyy-MM-dd");
            drpTimeSpent.SelectedValue = assignment.TimeSpanId.ToString();
            if (assignment.DateCompleted.HasValue)
                txtDateCompleted.Text = assignment.DateCompleted.Value.ToString("yyyy-MM-dd");
            drpStatus.SelectedValue = assignment.PhaseId.ToString();
            txtComments.Text = assignment.Comments;
        }

        protected void BindData()
        {

        }

        protected void BindEvents()
        {
            var ctl = new EventController();
            rptEvents.DataSource = ctl.GetEventsByAssignment(AssignmentId);
            rptEvents.DataBind();
        }
        protected void BindFiles()
        {
            var ctl = new FileController();
            rptFiles.DataSource = ctl.GetFilesByAssignment(AssignmentId);
            rptFiles.DataBind();
        }


        #endregion

    }
}