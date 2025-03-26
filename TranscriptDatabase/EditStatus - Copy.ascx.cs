/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DocumentFormat.OpenXml.EMMA;
using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
using Literal = System.Web.UI.WebControls.Literal;

namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditStatus : TranscriptDatabaseModuleBase
    {
        private readonly INavigationManager _navigationManager;


        #region Methods
        public EditStatus()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private string AttorneyList()
        {
            var ctl = new AttorneyController();
            IEnumerable<AttorneyViewModel> attorneys = ctl.GetDesignationAttorneys(DesignationId);

            return string.Join(";", attorneys.Select(x => x.ListName));
        }
        private void UpdateDueDate(Designation designation)
        {
            var ctl = new CalendarController();
            Components.Calendar calendarEvent = ctl.GetCalendarByDesignation(DesignationId);

            if (calendarEvent != null)
            {
                calendarEvent.StartTime = designation.DueDate.Value;
                calendarEvent.EndTime = designation.DueDate.Value;
                ctl.UpdateCalendar(calendarEvent);
            }
            else
                CreateCalendarDueDate(designation);
        }
        private Components.Calendar CreateCalendarDueDate(EventTypes eventTypeId, bool requestOutstanding, Designation designation)
        {
            var ctl = new CalendarController();
            Components.Calendar calendar = new Components.Calendar();
            {
                var withBlock = calendar;
                withBlock.CreatedByUserID = UserId;
                withBlock.CreatedDate = DateTime.Now;
                withBlock.LastModifiedDate = DateTime.Now;
                withBlock.LastModifiedByUserID = UserId;
                withBlock.DesignationID = DesignationId;
                withBlock.StartTime = designation.DueDate.Value;
                withBlock.EndTime = designation.DueDate.Value;
                withBlock.EventTypeID = (int)eventTypeId;
                withBlock.Subject = designation.CalendarName;
                withBlock.RequestOutstanding = requestOutstanding;
            }
            ctl.CreateCalendar(calendar);
            return calendar;
        }
        private Components.Calendar CreateCalendarDueDate(Designation designation)
        {
            return CreateCalendarDueDate(EventTypes.dueDate, false, designation);
        }
        private void PopulateForm()
        {
            var aCtl = new AttachmentController();
            var vCtl = new EventController();
            var eCtl = new ExtensionRequestController();
            var cCtl = new CalendarController();
            hdDesignationId.Value = DesignationId.ToString();
            var ctl = new Components.DesignationController();
            Designation designation = ctl.GetDesignation(DesignationId);
            if (designation != null)
            {
                if (designation.ServiceDate.HasValue)
                    txtServiceDate.Text = designation.ServiceDate.Value.ToShortDateString();
                if (designation.DueDate.HasValue)
                {
                    txtDueDate.Text = designation.DueDate.Value.ToShortDateString();
                    txtCurrentDueDate.Text = designation.DueDate.Value.ToShortDateString();
                    txtRequestedDueDate.Text = designation.DueDate.Value.ToShortDateString();
                    txtDueDateUpdate.Text = designation.DueDate.Value.ToShortDateString();
                }

                if (designation.ReceiptDate.HasValue)
                    txtReceiptDate.Text = designation.ReceiptDate.Value.ToShortDateString();
                if (designation.TranscriptFiled.HasValue)
                {
                    txtTranscriptFiledDate.Text = designation.TranscriptFiled.Value.ToShortDateString();
                    txtTranscriptFiledUpdate.Text = designation.TranscriptFiled.Value.ToShortDateString();
                }
                txtDefendantName.Text = designation.DisplayName;
                txtTribunalCase.Text = designation.LowerTribunalCaseNumber;
                txtAppellateCase.Text = designation.AppellateCaseNumber;
                txtCounty.Text = designation.County;
                txtAttorneys.Text = AttorneyList();
                txtComments.Text = designation.Comment;
                var CreatedByUserID = UserController.Instance.GetUserById(PortalId, designation.CreatedByUserID);
                if (CreatedByUserID != null)
                    txtCreatedBy.Text = CreatedByUserID.DisplayName;
                chkAcknowledgementFiled.Checked = designation.AcknowledgmentFiled;
                chkCourtAppointed.Checked = designation.CourtAppointedCounsel;
                chkIndigent.Checked = designation.DeclaredIndigent;
                chkPublicDefender.Checked = designation.PublicDefenderAppointed;
                txtSubmittedDate.Text = DateTime.Now.ToShortDateString();
                Components.Calendar calendar = cCtl.GetCalendarByDesignation(DesignationId);
                if (calendar != null)
                {
                    hdRequestOutstanding.Value = calendar.RequestOutstanding.ToString();
                    hdCalendarEventTypeId.Value = (calendar.EventTypeID + 1).ToString();
                }
                hdThirdExtension.Value = ((int)EventTypes.thirdExtension).ToString();
                BindAttachments(aCtl);
                BindExtensionRequests(eCtl);
                BindEvents(vCtl);
                BindDropDowns();
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to retrieve the Designation information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
        private void ClearEventForm()
        {
            hdEventId.Value = string.Empty;
            txtHearingDate.Text = string.Empty;
            judgeSearch.Text = string.Empty;
            selectedJudgeId.Value = string.Empty;
            drpHearingType.SelectedIndex = -1;
            reporterSearch.Text = string.Empty;
            selectedCourtReporterId.Value = string.Empty;
            txtEstimagedPages.Text = string.Empty;
            txtDaysUntilCompletion.Text = string.Empty;
            scopistSearch.Text = string.Empty;
            selectedScopistId.Value = string.Empty;
            txtScopeSent.Text = string.Empty;
            txtScopePagesIn.Text = string.Empty;
            txtScopeReturned.Text = string.Empty;
            txtScopePagesOut.Text = string.Empty;
            transcriptionistSearch.Text = string.Empty;
            selectedTranscriptionistId.Value = string.Empty;
            txtTransSent.Text = string.Empty;
            txtTransPagesIn.Text = string.Empty;
            txtTransReturned.Text = string.Empty;
            txtTransPagesOut.Text = string.Empty;
            editorSearch.Text = string.Empty;
            selectedEditorId.Value = string.Empty;
            txtEditSent.Text = string.Empty;
            txtEditPagesIn.Text = string.Empty;
            txtEditReturned.Text = string.Empty;
            txtEditPagesOut.Text = string.Empty;
            prooferSearch.Text = string.Empty;
            selectedProoferId.Value = string.Empty;
            txtProofSent.Text = string.Empty;
            txtProofPagesIn.Text = string.Empty;
            txtProofReturned.Text = string.Empty;
            txtProofPagesOut.Text = string.Empty;
            txtCompletedPages.Text = string.Empty;
        }
        private void ClearUploadForm()
        {
            uplFile.Enabled = true;
            hdFileId.Value = string.Empty;
            hdAttachmentId.Value = string.Empty;
            txtUploadeTitle.Text = string.Empty;
        }
        private void ClearFileSelectionForm()
        {
            txtReason.Text = string.Empty;
            hdSelectedFormType.Value = "0";
            txtRequestedDays.Text = string.Empty;
            txtRequestedDueDate.Text = string.Empty;
            hdRequestOutstanding.Value = string.Empty;
        }
        private void BindDropDowns()
        {
            var ctl = new HearingTypeController();
            drpHearingType.DataValueField = "HearingTypeName";
            drpHearingType.DataTextField = "HearingTypeName";
            drpHearingType.DataSource = ctl.GetHearingTypes().OrderBy(x => x.HearingTypeName);
            drpHearingType.DataBind();

        }
        private void BindAttachments(AttachmentController ctl)
        {
            rptAttachments.DataSource = ctl.GetAttachmentsByDesignation(DesignationId);
            rptAttachments.DataBind();
        }
        private void BindEvents(EventController ctl)
        {
            IEnumerable<EventListItem> events = ctl.GetEventListItemsByDesignation(DesignationId);
            if (events.Count() > 0)
            {
                txtEstimatedPages.Text = events.Sum(x => x.Pages).ToString();
                txtDays.Text = events.Max(x => x.DaysUntilComplete).ToString();
                DateTime currentdate = Null.NullDate;
                int trialHearingDays = 0;
                foreach (Event evt in events)
                {
                    if (evt.HearingDate.Value != currentdate)
                        trialHearingDays++;
                    currentdate = evt.HearingDate.Value;
                }
                txtHearingDates.Text = trialHearingDays.ToString();
            }
            rptEvent.DataSource = events;
            rptEvent.DataBind();
        }
        private void BindExtensionRequests(ExtensionRequestController ctl)
        {
            rptExtensions.DataSource = ctl.GetExtensionRequestsByDesignation(DesignationId);
            rptExtensions.DataBind();
        }
        private void GetExtensionForm(DateTime extensionDate)
        {
            string formCreationUrl = string.Empty;
            if (hdSelectedFormType.Value == "0")
            {
                formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&date={3}", TemplateSourceDirectory, DesignationId, hdSelectedFormType.Value, Server.UrlEncode(extensionDate.ToShortDateString()));
            }
            else
            {
                formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&reason={3}&date={4}", TemplateSourceDirectory, DesignationId, hdSelectedFormType.Value, txtReason.Text, Server.UrlEncode(extensionDate.ToShortDateString()));
            }
            ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "OpenFormExtension", string.Format("ShowForm('{0}')", formCreationUrl), true);
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    if (ErrorMessage != string.Empty)
                        ltPageMessage.Text = string.Format(MessageFormat, ErrorMessage, "alert alert-danger", "fas fa-circle-exclamation");
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    lnkEdit.NavigateUrl = EditUrl("did", DesignationId.ToString());
                    PopulateForm();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            var ctl = new DesignationController();
            ctl.DeleteDesignation(DesignationId);
            Response.Redirect(_navigationManager.NavigateURL());
        }
        protected void rptAttachments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Int32.TryParse(e.CommandArgument.ToString(), out int attachmentId);
            var ctl = new AttachmentController();
            Attachment attachment = ctl.GetAttachment(attachmentId);
            switch (e.CommandName.ToLower())
            {
                case "edit":
                    txtUploadeTitle.Text = attachment.FileDescription;
                    hdAttachmentId.Value = attachmentId.ToString();
                    hdFileId.Value = attachment.FileID.ToString();
                    uplFile.Enabled = false;
                    ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "ToggleAttachmentForm", "ToggleUploadForm(true)", true);
                    break;
                case "delete":
                    ctl.DeleteAttachment(attachmentId);
                    BindAttachments(ctl);
                    break;
                default:
                    break;
            }

        }
        protected void rptAttachments_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdEdit = e.Item.FindControl("cmdEdit") as LinkButton;
                LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
                scriptMan.RegisterPostBackControl(cmdDelete);
            }
        }
        protected void rptExtensions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctl = new DesignationController();
            var cCtl = new CalendarController();
            var eCtl = new ExtensionRequestController();

            Designation designation = ctl.GetDesignation(DesignationId);
            Components.Calendar calendarEvent = cCtl.GetCalendarByDesignation(DesignationId);
            Int32.TryParse(e.CommandArgument.ToString(), out int extensionId);
            ExtensionRequest extension = eCtl.GetExtensionRequest(extensionId);

            switch (e.CommandName.ToLower())
            {
                case "approve":
                    {
                        TextBox txtNewDate = (TextBox)e.Item.FindControl("txtNewDate");
                        DateTime.TryParse(txtNewDate.Text, out DateTime grantedDate);
                        if (grantedDate == Null.NullDate || grantedDate == null)
                        {
                            ltExtensionMessage.Text = string.Format(MessageFormat, "<strong>Warning!</strong>The date entered is not it the correct format.", "alert alert-warning", "fas fa-triangle-exclamation");
                            return;
                        }
                        extension.GrantedDate = grantedDate;
                        designation.DueDate = grantedDate;
                        if (calendarEvent != null)
                        {
                            calendarEvent.StartTime = grantedDate;
                            calendarEvent.EndTime = grantedDate;
                            calendarEvent.RequestOutstanding = false;
                        }
                        else
                            CreateCalendarDueDate(extension.EventType, false, designation);
                        txtDueDate.Text = grantedDate.ToShortDateString();
                        extension.Approved = true;

                        eCtl.UpdateExtensionRequest(extension);
                        cCtl.UpdateCalendar(calendarEvent);
                        ctl.UpdateDesignation(designation);
                        BindExtensionRequests(eCtl);
                        Notifications.SendCourtReporterExtensionNotification(designation.Events, grantedDate, designation.DisplayName, UserInfo, PortalId);
                        break;
                    }

                case "delete":
                    {
                        eCtl.DeleteExtensionRequest(extensionId);
                        //revert the event type back one stage example: second extension to first extension
                        calendarEvent.EventTypeID = extension.EventType == EventTypes.dueDate ? (int)EventTypes.dueDate : extension.EventTypeID - 1;
                        calendarEvent.RequestOutstanding = false;
                        if (txtTranscriptFiledDate.Text != "")
                            calendarEvent.EventType = EventTypes.transcriptFiled;
                        cCtl.UpdateCalendar(calendarEvent);
                        BindExtensionRequests(eCtl);
                        break;
                    }
            }

        }
        protected void rptExtensions_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdApprove = e.Item.FindControl("cmdApprove") as LinkButton;
                LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
                scriptMan.RegisterPostBackControl(cmdApprove);
                scriptMan.RegisterPostBackControl(cmdDelete);
            }
        }
        protected void rptEvent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                LinkButton cmdEdit = e.Item.FindControl("cmdEdit") as LinkButton;
                LinkButton cmdComplete = e.Item.FindControl("cmdComplete") as LinkButton;
                LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
                if (e.Item.DataItem is Event evt)
                {
                    if (evt.Completed.HasValue)
                    {
                        cmdEdit.Visible = false;
                        cmdDelete.Visible = false;
                        cmdComplete.Text = "Unmark Complete";
                        cmdComplete.CssClass = "btn btn-tertiary uncomplete-event";
                    }
                    else
                    {
                        cmdComplete.CssClass = "btn btn-tertiary complete-event";
                    }
                    if (UserInfo.IsInRole(AdminRole) || UserInfo.IsInRole(CourtReporterRole))
                        cmdComplete.Visible = true;
                    else
                        cmdComplete.Visible = false;
                    if (UserInfo.IsInRole(AdminRole))
                        cmdDelete.Visible = true;
                    else
                        cmdDelete.Visible = false;
                }
            }
        }
        protected void rptEvent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctl = new EventController();
            int eventId = Int32.Parse(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "complete")
            {
                Event evt = ctl.GetEvent(eventId);

                int sequence = e.Item.ItemIndex;
                if (evt.CompletedPages > 0)
                {
                    if (evt.Completed.HasValue)
                    {
                        evt.Completed = null;
                        evt.CompletedByUserID = -1;
                    }
                    else
                    {
                        evt.Completed = DateTime.Now;
                        evt.CompletedByUserID = UserId;

                        ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "showAlertEvent", "ShowAlert('Upload Notification','Please upload the Signed PDF to the T-Drive');", true);
                    }
                    ctl.UpdateEvent(evt);
                    try
                    {
                        txtDefendantName.Text = e.CommandArgument.ToString();
                        Notifications.NotifiyRecordingManager(txtDefendantName.Text, EditUrl("did", DesignationId.ToString(), "status"), sequence, AdminRole, PortalId, txtCounty.Text);
                    }
                    catch (Exception exc)
                    {
                        Exceptions.LogException(exc);
                        Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "showAlertEventError", "ShowAlert('Unable to Complete','Completed pages must be entered before the event can be marked complete');", true);
                }
                BindEvents(ctl);
            }
            if (e.CommandName.ToLower() == "edit")
            {
                EventListItem evtlistItem = ctl.GetEventListItem(eventId);
                if (evtlistItem.HearingDate.HasValue)
                    txtHearingDate.Text = evtlistItem.HearingDate.Value.ToShortDateString();
                hdSequence.Value = (e.Item.ItemIndex).ToString();
                hdEventId.Value = evtlistItem.EventID.ToString();
                selectedJudgeId.Value = evtlistItem.PresidingJudgeID.ToString();
                judgeSearch.Text = evtlistItem.PresidingJudgeName;
                drpHearingType.SelectedValue = evtlistItem.HearingType;
                selectedCourtReporterId.Value = evtlistItem.CourtReporterID.ToString();
                reporterSearch.Text = evtlistItem.CourtReporterName;
                txtEstimagedPages.Text = evtlistItem.Pages.ToString();
                txtDaysUntilCompletion.Text = evtlistItem.DaysUntilComplete.ToString();
                selectedScopistId.Value = evtlistItem.ScopistID.ToString();
                scopistSearch.Text = evtlistItem.ScopistName;
                if (evtlistItem.ScopSent.HasValue)
                    txtScopeSent.Text = evtlistItem.ScopSent.Value.ToShortDateString();
                txtScopePagesIn.Text = evtlistItem.ScopPagesIn.ToString();
                if (evtlistItem.ScopReturned.HasValue)
                    txtScopeReturned.Text = evtlistItem.ScopReturned.Value.ToShortDateString();
                txtScopePagesOut.Text = evtlistItem.ScopPagesOut.ToString();
                selectedTranscriptionistId.Value = evtlistItem.TranscriptionistID.ToString();
                transcriptionistSearch.Text = evtlistItem.TranscriptionistName;
                if (evtlistItem.TransSent.HasValue)
                    txtTransSent.Text = evtlistItem.TransSent.Value.ToShortDateString();
                txtTransPagesIn.Text = evtlistItem.TransPagesIn.ToString();
                if (evtlistItem.TransReturned.HasValue)
                    txtTransReturned.Text = evtlistItem.TransReturned.Value.ToShortDateString();
                txtTransPagesOut.Text = evtlistItem.TransPagesOut.ToString();
                selectedEditorId.Value = evtlistItem.EditorID.ToString();
                editorSearch.Text = evtlistItem.EditorName;
                if (evtlistItem.EditSent.HasValue)
                    txtEditSent.Text = evtlistItem.EditSent.Value.ToShortDateString();
                txtEditPagesIn.Text = evtlistItem.EditPagesIn.ToString();
                if (evtlistItem.EditReturned.HasValue)
                    txtEditReturned.Text = evtlistItem.EditReturned.Value.ToShortDateString();
                txtEditPagesOut.Text = evtlistItem.EditPagesOut.ToString();
                selectedProoferId.Value = evtlistItem.ProoferID.ToString();
                prooferSearch.Text = evtlistItem.ProoferName;
                if (evtlistItem.ProofSent.HasValue)
                    txtProofSent.Text = evtlistItem.ProofSent.Value.ToShortDateString();
                txtProofPagesIn.Text = evtlistItem.ProofPagesIn.ToString();
                if (evtlistItem.ProofReturned.HasValue)
                    txtProofReturned.Text = evtlistItem.ProofReturned.Value.ToShortDateString();
                txtProofPagesOut.Text = evtlistItem.ProofPagesOut.ToString();
                txtCompletedPages.Text = evtlistItem.CompletedPages.ToString();
                ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "ToggleEventEditForm", "ToggleEditForm(true)", true);
            }
            if (e.CommandName.ToLower() == "delete")
            {
                ctl.DeleteEvent(eventId);
                BindEvents(ctl);
            }
        }
        protected void rptEvent_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
            LinkButton cmdEdit = e.Item.FindControl("cmdEdit") as LinkButton;
            LinkButton cmdComplete = e.Item.FindControl("cmdComplete") as LinkButton;
            LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
            scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            scriptMan.RegisterPostBackControl(cmdComplete);
            scriptMan.RegisterPostBackControl(cmdDelete);
        }
        protected void valUpload_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (hdFileId.Value != "")
                args.IsValid = true;
            args.IsValid = false;
        }
        protected void cmdSaveFile_Click(object sender, EventArgs e)
        {
            var ctl = new AttachmentController();
            try
            {
                if (!string.IsNullOrEmpty(hdFileId.Value))
                {
                    Int32.TryParse(hdAttachmentId.Value, out int attachmentId);
                    if (attachmentId > 0)
                    {
                        Attachment attachment = ctl.GetAttachment(attachmentId);
                        if (attachment != null)
                        {
                            attachment.FileDescription = txtUploadeTitle.Text;
                            attachment.LastModifiedByUserID = UserId;
                            attachment.LastModifiedDate = DateTime.Now;
                            ctl.UpdateAttachment(attachment);
                        }
                    }
                    else
                    {
                        Attachment attachment = new Attachment
                        {
                            CreatedDate = DateTime.Now,
                            CreatedByUserID = UserId,
                            LastModifiedByUserID = UserId,
                            LastModifiedDate = DateTime.Now,
                            FileDescription = txtUploadeTitle.Text,
                            FileID = Int32.Parse(hdFileId.Value),
                            DesignationID = DesignationId
                        };

                        ctl.CreateAttachment(attachment);
                    }
                    Response.Redirect(EditUrl("did", DesignationId.ToString(), "status"), true);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "No File was selected or an error occurred uploading the file.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;
                }
                ClearUploadForm();
            }
            catch (Exception exc)
            {
                ClearUploadForm();
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasRequestedDays = Int32.TryParse(txtRequestedDays.Text, out int requestedDays);
                bool hasRequestedDate = DateTime.TryParse(txtRequestedDueDate.Text, out DateTime requestedDate);
                bool hasDueDate = DateTime.TryParse(txtDueDate.Text, out DateTime dueDate);
                bool hasEventTypeId = Int32.TryParse(hdCalendarEventTypeId.Value, out int eventTypeId);
                bool hasSubmittedDate = DateTime.TryParse(txtSubmittedDate.Text, out DateTime submittedDate);
                DocumentTypes documentType = (DocumentTypes)Int32.Parse(hdSelectedFormType.Value);
                string formCreationUrl = string.Empty;
                if (documentType == DocumentTypes.ExtensionRequest)
                {
                    if (hasDueDate)
                    {
                        var eCtl = new ExtensionRequestController();
                        var ctl = new CalendarController();
                        ExtensionRequest extensionRequest = new ExtensionRequest
                        {
                            Approved = false,
                            DesignationID = this.DesignationId,
                            EventTypeID = eventTypeId,
                            SubmittedDate = hasSubmittedDate ? submittedDate : DateTime.Today,
                            RequestedDate = requestedDate,
                            CreatedByUserID = UserId,
                            CreatedDate = DateTime.Now,
                            LastModifiedByUserID = UserId,
                            LastModifiedDate = DateTime.Now,
                        };
                        eCtl.CreateExtensionRequest(extensionRequest);
                        Components.Calendar calendar = ctl.GetCalendarByDesignation(this.DesignationId);
                        if (calendar != null)
                        {
                            calendar.EventTypeID = eventTypeId;
                            calendar.RequestOutstanding = true;
                            ctl.UpdateCalendar(calendar);
                        }
                        else
                        {
                            ltPageMessage.Text = string.Format(MessageFormat, "Could not retrieve calendar information for designation. Update the due date to recreate the calendar event.", "alert alert-warning", "fas fa-triangle-exclamation");
                        }
                        string subject = string.Format("Extension Request for Designation: {0}", this.DesignationId);
                        Notifications.NotifiyRecordingManager(subject, "A new extension request has been submitted", PortalId, AdminRole, txtCounty.Text);
                        BindExtensionRequests(eCtl);
                        GetExtensionForm(requestedDate);
                    }
                    else
                    {
                        ltPageMessage.Text = string.Format(MessageFormat, "The Due Date was not entered for the Current Designation. Please close the window and add the due date.", "alert alert-danger", "fas fa-circle-exclamation");
                    }
                }
                else
                {
                    string reason = txtReason.Text;
                    if (documentType == DocumentTypes.PrivatePaying)
                    {
                        reason = "";
                    }
                    formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&reason={3}&date={4}", TemplateSourceDirectory, DesignationId, hdSelectedFormType.Value, reason, Server.UrlEncode(requestedDate.ToShortDateString()));
                }
                ClearFileSelectionForm();
                ScriptManager.RegisterStartupScript(pnlStatus, pnlStatus.GetType(), "OpenForm", string.Format("ShowForm('{0}')", formCreationUrl), true);
            }
            catch (Exception exc)
            {
                ClearFileSelectionForm();
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void cmdUpdateTranscriptFiled_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasDate = DateTime.TryParse(txtTranscriptFiledUpdate.Text, out DateTime filedDate);
                if (hasDate)
                {
                    var ctl = new DesignationController();
                    Designation designation = ctl.GetDesignation(DesignationId);
                    if (filedDate != designation.TranscriptFiled.Value)
                    {
                        designation.TranscriptFiled = filedDate;
                        ctl.UpdateDesignation(designation);
                        UpdateDueDate(designation);
                        txtTranscriptFiledDate.Text = filedDate.ToShortDateString();
                    }
                    var cCtl = new CalendarController();
                    Components.Calendar calendarEvent = cCtl.GetCalendarByDesignation(DesignationId);
                    if (calendarEvent != null)
                    {
                        if (hasDate)
                            calendarEvent.EventTypeID = (int)EventTypes.transcriptFiled;
                        else
                        {
                            var eCtl = new ExtensionRequestController();
                            calendarEvent.EventTypeID = (int)EventTypes.dueDate;
                            IEnumerable<ExtensionRequest> extensions = eCtl.GetExtensionRequestsByDesignation(DesignationId);
                            if (extensions.Count() > 0)
                            {
                                ExtensionRequest extension = extensions.OrderByDescending(et => et.EventTypeID).FirstOrDefault();
                                calendarEvent.EventTypeID = extension.EventTypeID;
                                if (!extension.Approved)
                                    calendarEvent.RequestOutstanding = true;
                            }
                        }
                        cCtl.UpdateCalendar(calendarEvent);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void cmdUpdateDueDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (DateTime.TryParse(txtDueDateUpdate.Text, out DateTime dueDate))
                {
                    var ctl = new DesignationController();
                    Designation designation = ctl.GetDesignation(DesignationId);
                    if (dueDate != designation.DueDate.Value)
                    {
                        designation.DueDate = dueDate;
                        ctl.UpdateDesignation(designation);
                        UpdateDueDate(designation);
                        txtDueDate.Text = dueDate.ToShortDateString();
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void cmdSaveComment_Click(object sender, EventArgs e)
        {
            var ctl = new DesignationController();
            Designation designation = ctl.GetDesignation(DesignationId);
            if (designation != null)
            {
                designation.Comment = txtComments.Text;
                ctl.UpdateDesignation(designation);
            }
        }
        protected void cmdSaveEvent_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasEvent = Int32.TryParse(hdEventId.Value, out int eventId);
                bool hasScopist = Int32.TryParse(selectedScopistId.Value, out int scopistId);
                bool hasPresidingJudge = Int32.TryParse(selectedJudgeId.Value, out int judgeId);
                bool hasCourtReporter = Int32.TryParse(selectedCourtReporterId.Value, out int courtReporterId);
                bool hasTranscriptionist = Int32.TryParse(selectedTranscriptionistId.Value, out int transcriptionistId);
                bool hasEditor = Int32.TryParse(selectedEditorId.Value, out int editorId);
                bool hasProofer = Int32.TryParse(selectedProoferId.Value, out int proofterId);
                bool hasHearingDate = DateTime.TryParse(txtHearingDate.Text, out DateTime hearingDate);
                bool hasScopSentDate = DateTime.TryParse(txtScopeSent.Text, out DateTime scopSentDate);
                bool hasScopReturnDate = DateTime.TryParse(txtScopeReturned.Text, out DateTime scopReturnDate);
                bool hasTransSentDate = DateTime.TryParse(txtTransSent.Text, out DateTime transSentDate);
                bool hasTransReturnDate = DateTime.TryParse(txtTransReturned.Text, out DateTime transReturnDate);
                bool hasEditSentDate = DateTime.TryParse(txtEditSent.Text, out DateTime editSentDate);
                bool hasEditReturnDate = DateTime.TryParse(txtEditReturned.Text, out DateTime editReturnDate);
                bool hasProofSentDate = DateTime.TryParse(txtProofSent.Text, out DateTime proofSentDate);
                bool hasProofReturnDate = DateTime.TryParse(txtProofReturned.Text, out DateTime proofReturnDate);
                var ctl = new EventController();
                int oldCourtReporterId = 0;
                Int32.TryParse(hdSequence.Value, out int eventSequence);
                if (eventSequence <= 0)
                    eventSequence = 0;
                Event evt = new Event();
                if (hasEvent)
                    evt = ctl.GetEvent(eventId);
                evt.HearingDate = hearingDate;
                evt.HearingType = drpHearingType.SelectedValue;
                if (hasPresidingJudge)
                    evt.PresidingJudgeID = judgeId;
                if (hasCourtReporter)
                {
                    oldCourtReporterId = evt.CourtReporterID;
                    evt.CourtReporterID = courtReporterId;
                }
                if (!string.IsNullOrEmpty(txtEstimagedPages.Text))
                    evt.Pages = Int32.Parse(txtEstimagedPages.Text);
                if (!string.IsNullOrEmpty(txtEstimagedPages.Text))
                    evt.Pages = Int32.Parse(txtEstimagedPages.Text);
                if (!string.IsNullOrEmpty(txtDaysUntilCompletion.Text))
                    evt.DaysUntilComplete = Int32.Parse(txtDaysUntilCompletion.Text);
                if (hasScopist)
                    evt.ScopistID = scopistId;
                if (hasTranscriptionist)
                    evt.TranscriptionistID = transcriptionistId;
                if (hasEditor)
                    evt.EditorID = editorId;
                if (hasProofer)
                    evt.ProoferID = proofterId;
                if (hasScopSentDate)
                    evt.ScopSent = scopSentDate;
                if (hasScopReturnDate)
                    evt.ScopReturned = scopReturnDate;
                if (hasTransSentDate)
                    evt.TransSent = transSentDate;
                if (hasTransReturnDate)
                    evt.TransReturned = transReturnDate;
                if (hasEditSentDate)
                    evt.EditSent = editSentDate;
                if (hasEditReturnDate)
                    evt.EditReturned = editReturnDate;
                if (hasProofSentDate)
                    evt.ProofSent = proofSentDate;
                if (hasProofReturnDate)
                    evt.ProofReturned = proofReturnDate;
                if (!string.IsNullOrEmpty(txtScopePagesIn.Text))
                    evt.ScopPagesIn = Int32.Parse(txtScopePagesIn.Text);
                if (!string.IsNullOrEmpty(txtScopePagesOut.Text))
                    evt.ScopPagesOut = Int32.Parse(txtScopePagesOut.Text);
                if (!string.IsNullOrEmpty(txtTransPagesIn.Text))
                    evt.TransPagesIn = Int32.Parse(txtTransPagesIn.Text);
                if (!string.IsNullOrEmpty(txtTransPagesOut.Text))
                    evt.TransPagesOut = Int32.Parse(txtTransPagesOut.Text);
                if (!string.IsNullOrEmpty(txtEditPagesIn.Text))
                    evt.EditPagesIn = Int32.Parse(txtEditPagesIn.Text);
                if (!string.IsNullOrEmpty(txtEditPagesOut.Text))
                    evt.EditPagesOut = Int32.Parse(txtEditPagesOut.Text);
                if (!string.IsNullOrEmpty(txtProofPagesIn.Text))
                    evt.ProofPagesIn = Int32.Parse(txtProofPagesIn.Text);
                if (!string.IsNullOrEmpty(txtProofPagesOut.Text))
                    evt.ProofPagesOut = Int32.Parse(txtProofPagesOut.Text);
                if (!string.IsNullOrEmpty(txtCompletedPages.Text))
                    evt.CompletedPages = Int32.Parse(txtCompletedPages.Text);
                evt.LastModifiedByUserID = UserId;
                evt.LastModifiedDate = DateTime.Now;
                if (hasEvent)
                {
                    if (hasCourtReporter)
                    {
                        if (oldCourtReporterId != courtReporterId)
                        {
                            if (courtReporterId <= 0)
                            {
                                try
                                {
                                    Notifications.SendCourtReporterResetNotification(courtReporterId, txtDefendantName.Text, eventSequence, AdminRole, PortalId, UserInfo, txtCounty.Text);
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    if (oldCourtReporterId <= 0)
                                    {
                                        Notifications.SendCourtReporterNotification(courtReporterId, txtDefendantName.Text, EditUrl("designationId", DesignationId.ToString(), "status"), eventSequence, PortalId, UserInfo, txtCounty.Text);
                                    }
                                    else
                                    {
                                        Notifications.SendCourtReporterNotification(courtReporterId, txtDefendantName.Text, EditUrl("designationId", DesignationId.ToString(), "status"), eventSequence, PortalId, UserInfo, txtCounty.Text);
                                        Notifications.SendCourtReporterTransferrNotification(oldCourtReporterId, courtReporterId, txtDefendantName.Text, EditUrl("designationId", DesignationId.ToString(), "status"), eventSequence, PortalId, txtCounty.Text);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    ctl.UpdateEvent(evt);
                }
                else
                {
                    evt.DesignationID = DesignationId;
                    evt.CreatedDate = DateTime.Now;
                    evt.CreatedByUserID = UserId;
                    ctl.CreateEvent(evt);
                    Notifications.NotifiyRecordingManager(txtDefendantName.Text, EditUrl("designationId", DesignationId.ToString(), "status"), eventSequence, AdminRole, PortalId, txtCounty.Text);
                }
                ClearEventForm();
                BindEvents(ctl);
            }
            catch (Exception exc)
            {
                ClearEventForm();
                Exceptions.ProcessModuleLoadException(this, exc);
                Response.Redirect(EditUrl("did", DesignationId.ToString(), "status", "error=" + Server.UrlEncode(exc.Message)));
            }
        }
        protected void pnlStatus_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        #endregion
    }
}