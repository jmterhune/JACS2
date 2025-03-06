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

using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Policy;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
using static tjc.Modules.TranscriptDatabase.Services.AttorneyController;

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
                    txtDueDate.Text = designation.DueDate.Value.ToShortDateString();
                if (designation.ReceiptDate.HasValue)
                    txtReceiptDate.Text = designation.ReceiptDate.Value.ToShortDateString();
                if (designation.TranscriptFiled.HasValue)
                    txtTranscriptFiledDate.Text = designation.TranscriptFiled.Value.ToShortDateString();
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
                if (designation.DueDate.HasValue)
                    txtCurrentDueDate.Text = designation.DueDate.Value.ToShortDateString();
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
                rptEvent.DataSource = events;
                rptEvent.DataBind();
            }

        }
        private void BindExtensionRequests(ExtensionRequestController ctl)
        {
            rptExtensions.DataSource = ctl.GetExtensionRequestsByDesignation(DesignationId);
            rptExtensions.DataBind();
        }
        private void GetExtensionForm(DateTime extensionDate)
        {
            if (drpFormType.SelectedValue == "0")
            {
                string formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&date={3}", TemplateSourceDirectory, DesignationId, drpFormType.SelectedValue, Server.UrlEncode(extensionDate.ToShortDateString()));
                ifCreateDocument.Attributes.Add("src", formCreationUrl);
            }
            else
            {
                string formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&reason={3}&date={4}", TemplateSourceDirectory, DesignationId, drpFormType.SelectedValue, txtReason.Text, Server.UrlEncode(extensionDate.ToShortDateString()));
                ifCreateDocument.Attributes.Add("src", formCreationUrl);
            }
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
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    lnkEdit.NavigateUrl = EditUrl("did", DesignationId.ToString());
                    PopulateForm();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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
                    hdFileId.Value = attachmentId.ToString();
                    ScriptManager.RegisterStartupScript(rptEvent, rptEvent.GetType(), "ToggleForm", "ToggleUploadForm(true)", true);
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
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
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
                            ltMessage.Text = "<div class=\"alert alert-warning alert-dismissible\" role=\"alert\">\r\n\t<button aria-label=\"Close\" class=\"close\" data-dismiss=\"alert\" type=\"button\">\r\n\t\t<span aria-hidden=\"true\">&times;</span>\r\n\t</button>\r\n\t<strong><i class=\"fa fa-warning\"></i> Warning!</strong> The date entered is not it the correct format.\r\n</div>";
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
                scriptMan.RegisterAsyncPostBackControl(cmdApprove);
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
            }
        }
        protected void rptEvent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                LinkButton cmdEdit = e.Item.FindControl("cmdEdit") as LinkButton;
                LinkButton cmdComplete = e.Item.FindControl("cmdComplete") as LinkButton;
                LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
                Event evt = e.Item.DataItem as Event;
                if (evt != null)
                {
                    if (evt.Completed.HasValue)
                    {
                        cmdEdit.Visible = false;
                        cmdDelete.Visible = false;
                        cmdComplete.Text = "Unmark Complete";
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
            EventListItem evt = ctl.GetEventListItem(eventId);

            switch (e.CommandName.ToLower())
            {
                case "complete":
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
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "ShowAlert('Upload Notification','Please upload the Signed PDF to the T-Drive');", true);

                        }
                        ctl.UpdateEvent(evt);
                        BindEvents(ctl);
                        try
                        {
                            txtDefendantName.Text = e.CommandArgument.ToString();
                            Notifications.NotifiyRecordingManager(txtDefendantName.Text, EditUrl("did", DesignationId.ToString(), "status"), sequence, AdminRole, PortalId, txtCounty.Text);
                        }
                        catch (Exception exc)
                        {
                            Exceptions.LogException(exc);
                        }
                    }
                    break;
                case "edit":
                    if (evt.HearingDate.HasValue)
                        txtHearingDate.Text = evt.HearingDate.Value.ToShortDateString();
                    hdEventId.Value = evt.EventID.ToString();
                    selectedJudgeId.Value = evt.PresidingJudgeID.ToString();
                    judgeSearch.Text = evt.PresidingJudgeName;
                    drpHearingType.SelectedValue = evt.HearingType;
                    selectedCourtReporterId.Value = evt.CourtReporterID.ToString();
                    reporterSearch.Text = evt.CourtReporterName;
                    txtEstimagedPages.Text = evt.Pages.ToString();
                    txtDaysUntilCompletion.Text = evt.DaysUntilComplete.ToString();
                    selectedScopistId.Value = evt.ScopistID.ToString();
                    scopistSearch.Text = evt.ScopistName;
                    if (evt.ScopSent.HasValue)
                        txtScopeSent.Text = evt.ScopSent.Value.ToShortTimeString();
                    txtScopePagesIn.Text = evt.ScopPagesIn.ToString();
                    txtScopeReturned.Text = evt.ScopReturned.ToString();
                    txtScopePagesOut.Text = evt.ScopPagesOut.ToString();
                    selectedTranscriptionistId.Value = evt.TranscriptionistID.ToString();
                    transcriptionistSearch.Text = evt.TranscriptionistName;
                    if (evt.TransSent.HasValue)
                        txtTransSent.Text = evt.TransSent.Value.ToShortTimeString();
                    txtTransPagesIn.Text = evt.TransPagesIn.ToString();
                    txtTransReturned.Text = evt.TransReturned.ToString();
                    txtTransPagesOut.Text = evt.TransPagesOut.ToString();
                    selectedEditorId.Value = evt.EditorID.ToString();
                    editorSearch.Text = evt.EditorName;
                    if (evt.EditSent.HasValue)
                        txtEditSent.Text = evt.EditSent.Value.ToShortTimeString();
                    txtEditPagesIn.Text = evt.EditPagesIn.ToString();
                    txtEditReturned.Text = evt.EditReturned.ToString();
                    txtEditPagesOut.Text = evt.EditPagesOut.ToString();
                    selectedProoferId.Value = evt.ProoferID.ToString();
                    prooferSearch.Text = evt.ProoferName;
                    if (evt.ProofSent.HasValue)
                        txtProofSent.Text = evt.ProofSent.Value.ToShortTimeString();
                    txtProofPagesIn.Text = evt.ProofPagesIn.ToString();
                    txtProofReturned.Text = evt.ProofReturned.ToString();
                    txtProofPagesOut.Text = evt.ProofPagesOut.ToString();
                    txtCompletedPages.Text = evt.CompletedPages.ToString();
                    ScriptManager.RegisterStartupScript(rptEvent, rptEvent.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
                    break;
                case "delete":
                    ctl.DeleteEvent(eventId);
                    BindEvents(ctl);
                    break;

                default:
                    break;
            }
        }
        protected void rptEvent_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
            LinkButton cmdEdit = e.Item.FindControl("cmdEdit") as LinkButton;
            LinkButton cmdComplete = e.Item.FindControl("cmdComplete") as LinkButton;
            LinkButton cmdDelete = e.Item.FindControl("cmdDelete") as LinkButton;
            scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            scriptMan.RegisterAsyncPostBackControl(cmdComplete);
            scriptMan.RegisterAsyncPostBackControl(cmdDelete);
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

            if (!string.IsNullOrEmpty(hdAttachmentId.Value))
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
                    else
                    {
                        attachment.CreatedDate = DateTime.Now;
                        attachment.CreatedByUserID = UserId;
                        attachment.LastModifiedByUserID = UserId;
                        attachment.LastModifiedDate = DateTime.Now;
                        attachment.FileDescription = txtUploadeTitle.Text;
                        attachment.FileID = Int32.Parse(hdFileId.Value);
                        attachment.DesignationID = DesignationId;
                    }
                }
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "No File was selected or an error occurred uploading the file.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasRequestedDays = Int32.TryParse(txtRequestedDays.Text, out int requestedDays);
                bool hasRequestedDate = DateTime.TryParse(txtDueDateUpdate.Text, out DateTime requestedDate);
                bool hasDueDate = DateTime.TryParse(txtDueDate.Text, out DateTime dueDate);
                bool hasEventTypeId = Int32.TryParse(hdCalendarEventTypeId.Value, out int eventTypeId);
                bool hasSubmittedDate = DateTime.TryParse(txtSubmittedDate.Text, out DateTime submittedDate);
                DocumentTypes documentType = (DocumentTypes)Int32.Parse(drpFormType.SelectedValue);
                if (documentType == DocumentTypes.ExtensionRequest)
                {
                    if (hasDueDate == false)
                    {
                        var eCtl = new ExtensionRequestController();
                        var ctl = new CalendarController();
                        ExtensionRequest extensionRequest = new ExtensionRequest
                        {
                            Approved = false,
                            DesignationID = this.DesignationId,
                            EventTypeID = eventTypeId,
                            SubmittedDate = hasSubmittedDate ? submittedDate : DateTime.Today,
                            RequestedDate = requestedDate
                        };
                        eCtl.CreateExtensionRequest(extensionRequest);
                        Components.Calendar calendar = ctl.GetCalendar(this.DesignationId);
                        if (calendar != null)
                        {
                            calendar.EventTypeID = eventTypeId;
                            calendar.RequestOutstanding = true;
                            ctl.UpdateCalendar(calendar);
                        }
                        else
                        {
                            phMessage.Controls.Clear();
                            DotNetNuke.UI.Skins.Controls.ModuleMessage message = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("", "Could not retrieve calendar information for designation. Update the duedate to recreate the calendar event.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                            phMessage.Controls.Add(message);
                        }
                        string subject = string.Format("Extension Request for Designation: {0}", this.DesignationId);
                        Notifications.NotifiyRecordingManager(subject, "A new extension request has been submitted", PortalId, AdminRole, txtCounty.Text);
                        GetExtensionForm(requestedDate);
                    }
                    else
                    {
                        phMessage.Controls.Clear();
                        DotNetNuke.UI.Skins.Controls.ModuleMessage message = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("", "Could not retrieve informaiton from designation. Please close the window and try again.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        phMessage.Controls.Add(message);
                    }
                }
                else
                {
                    string reason = txtReason.Text;
                    if (documentType == DocumentTypes.PrivatePaying)
                    {
                        reason = "";
                    }
                    string formCreationUrl = string.Format("{0}/Handlers/WordDocHandler.ashx?did={1}&type={2}&reason={3}&date={4}", TemplateSourceDirectory, DesignationId, drpFormType.SelectedValue, reason, Server.UrlEncode(requestedDate.ToShortDateString()));
                    ifCreateDocument.Attributes.Add("src", formCreationUrl);
                }
                ScriptManager.RegisterStartupScript(rptEvent, rptEvent.GetType(), "ToggleForm", "ToggleFileForm(true)", true);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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
                Event evt = new Event();
                if (hasEvent)
                    evt = ctl.GetEvent(eventId);
                evt.HearingDate = hearingDate;
                evt.HearingType = drpHearingType.SelectedValue;
                if (hasPresidingJudge)
                    evt.PresidingJudgeID = judgeId;
                if (hasCourtReporter)
                    evt.CourtReporterID = courtReporterId;
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
                    ctl.UpdateEvent(evt);
                }
                else
                {
                    evt.DesignationID = DesignationId;
                    evt.CreatedDate = DateTime.Now;
                    evt.CreatedByUserID = UserId;
                    ctl.CreateEvent(evt);
                }
                ClearEventForm();
                BindEvents(ctl);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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