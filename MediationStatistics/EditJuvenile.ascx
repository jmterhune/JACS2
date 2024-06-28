<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditJuvenile.ascx.cs" Inherits="tjc.Modules.MediationStatistics.EditJuvenile" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltHeading" runat="server"><h4>{0}:&nbsp{1}</h4></asp:Literal>
<div id="juvenile-form">
    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Case Toolbar">
        <div class="btn-group" role="group" aria-label="Basic example">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary" runat="server"><i class="fas fa-search"></i> Case Search</asp:HyperLink>
            <asp:HyperLink ID="lnkNew" CssClass="btn btn-dark" runat="server"><i class="fas fa-plus"></i> New Case</asp:HyperLink>
            <asp:LinkButton ID="cmdDelete" CssClass="btn btn-secondary confirm-delete-case" runat="server"><i class="fas fa-trash"></i> Delete Case</asp:LinkButton>
        </div>
    </div>
    <div class="row">
        <div class="col-6">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtCaseYear" Text="Case Number" />
                <div class="input-group">
                    <asp:TextBox TabIndex="1" ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox TabIndex="1" ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase case-code-field" placeholder="CC" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox TabIndex="1" ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="6" CssClass="form-control upperCase" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox TabIndex="1" ID="txtSuffix" title="Suffix" runat="server" MaxLength="4" CssClass="form-control upperCase case-code-field" ClientIDMode="Static"></asp:TextBox>
                    <div class="input-group-append">
                        <small class="input-group-text form-control rounded-end" title="Year - Case Type - Case Sequence - Suffix">(Format: YYYY-CC-000000-NC)</small>
                    </div>
                </div>
            </div>
            <fieldset class="outline-fieldset mb-0">
                <legend>Case Information</legend>
                <div class="form-group">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                            <asp:TextBox runat="server" ID="txtLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                            <asp:TextBox runat="server" ID="txtFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>

                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <hr />
    <asp:UpdatePanel ID="pnlSession" runat="server" RenderMode="Block" OnUnload="pnlSession_Unload">
        <ContentTemplate>
            <asp:UpdateProgress ID="upProgressEvent" runat="server">
                <ProgressTemplate>
                    <div class="modal-progress">
                        <div class="center-progress">
                            <img alt="" src="/images/loading.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:Literal ID="ltMessage" runat="server" />
            <div class="btn-toolbar mb-3" role="toolbar" aria-label="Session Toolbar">
                <div class="btn-group" role="group" aria-label="Basic example">
                    <asp:LinkButton ID="cmdNewSession" CssClass="btn btn-dark" OnClick="cmdNewSession_Click" runat="server"><i class="fas fa-plus"></i> New Session</asp:LinkButton>
                    <asp:LinkButton ID="cmdPreviousSession" CssClass="btn btn-primary" OnClick="cmdPreviousSession_Click" runat="server"><i class="fas fa-arrow-alt-circle-left"></i> Previous</asp:LinkButton>
                    <div class="input-group-text" id="sessionInfo">
                        <asp:Literal ID="ltSessionInfo" runat="server">Session 1 of 1</asp:Literal>
                    </div>
                    <asp:LinkButton ID="cmdNextSession" CssClass="btn btn-primary" OnClick="cmdNextSession_Click" runat="server">Next <i class="fas fa-arrow-alt-circle-right"></i></asp:LinkButton>
                    <asp:LinkButton ID="cmdDeleteSession" CssClass="btn btn-secondary confirm-delete-session" OnClick="cmdDeleteSession_Click" runat="server"><i class="fas fa-trash"></i> Delete Session</asp:LinkButton>
                </div>
            </div>
            <div class="row">
                <div class="col-auto">

                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtOrderReferral" Text="Order of Referral" />
                        <asp:TextBox runat="server" ID="txtOrderReferral" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                    </div>

                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtMediationDate" Text="Mediation Date / Resolved" />
                        <asp:TextBox runat="server" ID="txtMediationDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                    </div>
                    <div class="pt-4">
                        <div class="form-check form-switch d-inline-block me-2 mt-1">
                            <asp:CheckBox ID="chkTelephoneSession" runat="server" Text="Virtual Session" />
                        </div>
                        <div class="form-check form-switch d-inline-block me-2 mt-1">
                            <asp:CheckBox ID="chkInmate" runat="server" Text="Inmate" />
                        </div>
                        <div class="form-check form-switch d-inline-block mt-1">
                            <asp:CheckBox ID="chkInterpreterRequested" runat="server" Text="Interpreter Requested" />
                        </div>
                    </div>
                </div>

                <fieldset class="outline-fieldset" id="fsSecondaryIssues" runat="server">
                    <legend>Case Types</legend>
                    <asp:CheckBoxList ID="clsSecondaryIssues" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList">
                    </asp:CheckBoxList>
                </fieldset>
            </div>
            <hr />
            <div class="btn-toolbar mb-3" role="toolbar" aria-label="Session Events Toolbar">
                <div class="btn-group" role="group" aria-label="Session Events Actions">
                    <asp:LinkButton ID="cmdSaveSession" runat="server"
                        OnClick="cmdSave_Click" CssClass="btn btn-primary"><i class="fas fa-save"></i> Save Session</asp:LinkButton>
                    <asp:LinkButton ID="cmdAddEvent" CssClass="btn btn-dark" runat="server" OnClick="cmdAddEvent_Click"><i class="fas fa-plus"></i> New Event</asp:LinkButton>
                    <div class="input-group-text" id="eventInfo">
                        Session Events
                    </div>
                    <asp:HiddenField ID="hdSessionId" runat="server" />
                </div>
            </div>
            <asp:ListView ID="lstEvents" runat="server" InsertItemPosition="None" OnItemCreated="lstEvents_ItemCreated" OnItemDataBound="lstEvents_ItemDataBound" OnItemInserting="lstEvents_ItemInserting" OnItemDeleting="lstEvents_ItemDeleting" OnItemEditing="lstEvents_ItemEditing" OnItemInserted="lstEvents_ItemInserted" OnItemCanceling="lstEvents_ItemCanceling" OnItemCommand="lstEvents_ItemCommand" OnItemUpdating="lstEvents_ItemUpdating">
                <ItemSeparatorTemplate>
                    <hr />
                </ItemSeparatorTemplate>
                <LayoutTemplate>
                    <div class="event">
                        <div id="itemPlaceholder" runat="server" />
                    </div>
                </LayoutTemplate>
                <EmptyDataTemplate>
                    <div class="event">
                        <div class="alert alert-info">
                            <i class="fas fa-info-circle"></i>&nbsp;No Event has been added for this session. To add a new Event click the New Event button in the Session Toolbar.
                        </div>
                    </div>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <div class="template">
                        <fieldset disabled>
                            <legend>Event Number <%#Container.DataItemIndex + 1%>
                                <asp:Label ID="lblHoursRemaining" runat="server" CssClass="ms-5 fw-bold badge badge-danger">Hours Remaining: <%#Eval("TimeRemaining","{0:n}")%></asp:Label></legend>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch mt-4">
                                            <input class="form-check-input" type="checkbox" id="chkMeetingHeld" <%#Convert.ToBoolean(Eval("MediationHeld"))?"checked":""%>>
                                            <label class="form-check-label" for="chkMeetingHeld">Mediation Held</label>
                                        </div>
                                        <%#Convert.ToBoolean(Eval("MediationHeld").ToString())?"":"<div class='row'><div class='col-12'><div class='form-group'><label for='txtReasonNotHeld' class='form-label'>Reason Not Held:</label> <span id='txtReasonNotHeld'>" + Eval("ReasonNotHeld", "{0:d}") + "</span></div></div></div>"%>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <label for="txtEventDate" class="form-label">Event Date</label>
                                        <input id="txtEventDate" class="form-control" type="text" value='<%#Eval("EventDate", "{0:d}")%>' />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <label for="txtAgreementType">Agreement Type</label>
                                        <input class="form-control" type="text" id="txtAgreementType" value='<%#GetAgreementType(Eval("AgreementType").ToString())%>'>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <label for="txtMediatoryType">Mediator Type</label>
                                        <input id="txtMediatorType" class="form-control" type="text" value='<%#Eval("MediatorType")%>' />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <label for="txtMediator">Mediator Name</label>
                                        <input id="txtMediator" class="form-control" type="text" value='<%#Eval("MediatorName")%>' />
                                    </div>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkAdjournedTimeRemaining" <%#Convert.ToBoolean(Eval("AdjournedTimeRemaining"))?"checked":""%>>
                                            <label class="form-check-label me-3" for="chkAdjournedTimeRemaining">Adjourned with Time Remaining</label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <p class="mb-0 mt-3">
                            <asp:LinkButton ID="lnkUpdate" CssClass="btn btn-primary" CommandName="edit" runat="server"><i class="fas fa-pencil"></i> Edit Event</asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" CssClass="btn btn-secondary confirm-delete-event" CommandName="delete" runat="server"><i class="fas fa-trash"></i> Delete</asp:LinkButton>
                        </p>
                    </div>
                </ItemTemplate>
                <InsertItemTemplate>
                    <fieldset>
                        <legend>New Event </legend>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkMeetingHeld" runat="server" Checked='<%#Bind("MediationHeld")%>'
                                            onclick="hideReason(this)" Text="Mediation Held" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" ID="lblReason" AssociatedControlID="drpReason" CssClass="reason" Text="Reason Not Held" />
                                    <asp:DropDownList ID="drpReason" runat="server" CssClass="form-control reason selectMe" SelectedValue='<%#Bind("ReasonNotHeld")%>'>
                                        <asp:ListItem Value="" Text="< Select Reason>" />
                                        <asp:ListItem Text="Settled Prior" Value="Settled Prior" />
                                        <asp:ListItem Text="Session Rescheduled" Value="Session Rescheduled" />
                                        <asp:ListItem Text="Failure to Appear" Value="Failure to Appear" />
                                        <asp:ListItem Text="Party Declined to Participate" Value="Party Declined to Participate" />
                                        <asp:ListItem Text="Cancelled" Value="Cancelled" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEventDate" Text="Event Date" />
                                    <asp:TextBox runat="server" ID="txtEventDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" Text='<%#Bind("EventDate","{0:d}")%>' />
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="rblAgreementType" Text="Agreement" />
                                    <asp:RadioButtonList ID="rblAgreementType" CssClass="form-control radio-button-list" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
                                        SelectedValue='<%#Bind("AgreementType")%>'>
                                        <asp:ListItem Text="None" Value="N" Selected="True" />
                                        <asp:ListItem Text="Full" Value="F" />
                                        <asp:ListItem Text="Partial/Temporary" Value="C" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpMediatorType" Text="Mediator Type" />
                                    <asp:DropDownList ID="drpMediatorType" runat="server" ToolTip="Mediator Type" CssClass="form-control" ClientIDMode="Static" SelectedValue='<%#Bind("MediatorType")%>'>
                                        <asp:ListItem Text="< Select Mediator >" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Contracted" Value="Contracted" />
                                        <asp:ListItem Text="Staff" Value="Staff" />
                                        <asp:ListItem Text="Volunteer" Value="Volunteer" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-auto">
                                <asp:HiddenField ID="hdMediatorId" runat="server" ClientIDMode="Static" />
                                <asp:Label runat="server" AssociatedControlID="txtMediator" Text="Mediator Name" />
                                <asp:TextBox runat="server" ID="txtMediator" MaxLength="100" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("Mediator")%>' />
                            </div>
                            <div class="col-auto pt-4 mt-1">
                                <button class="btn btn-primary mediator-search" title="Search for Mediator" data-mediator="1">
                                    <i class="fas fa-search"></i>
                                </button>
                                <button class="btn btn-secondary ms-2" title="Clear Mediator" onclick="ValidateMediatorRemoval(event)">
                                    <i class="fas fa-times"></i>
                                </button>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkAdjournedTimeRemaining" runat="server" Text="Adjourned with time remaining?" Checked='<%#Bind("AdjournedTimeRemaining")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtHours" Text="Hours" />
                                    <asp:TextBox runat="server" ID="txtHours" step="0.01" TextMode="Number" MaxLength="15" ClientIDMode="Static" Text='<%#Bind("TimeRemaining")%>' CssClass="form-control" />
                                </div>
                            </div>
                        </div>
                        <p class="mt-3">
                            <asp:LinkButton ID="lnkInsert" CssClass="btn btn-primary me-3" CommandName="Insert" runat="server"><i class="fas fa-save"></i> Save Event</asp:LinkButton>
                            <asp:LinkButton ID="lnkCancel" CssClass="btn btn-secondary" CommandName="cancel" runat="server"><i class="fas fa-redo"></i> Cancel</asp:LinkButton>
                        </p>
                    </fieldset>
                </InsertItemTemplate>
                <EditItemTemplate>
                    <fieldset>
                        <legend>Event Number <%#Container.DataItemIndex + 1%></legend>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkMeetingHeld" runat="server" Checked='<%#Bind("MediationHeld")%>'
                                            onclick="hideReason(this)" Text="Mediation Held" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" ID="lblReason" AssociatedControlID="drpReason" CssClass="reason" Text="Reason Not Held" />
                                    <asp:DropDownList ID="drpReason" runat="server" CssClass="form-control reason selectMe" SelectedValue='<%#Bind("ReasonNotHeld")%>'>
                                        <asp:ListItem Value="" Text="< Select Reason>" />
                                        <asp:ListItem Text="Settled Prior" Value="Settled Prior" />
                                        <asp:ListItem Text="Session Rescheduled" Value="Session Rescheduled" />
                                        <asp:ListItem Text="Failure to Appear" Value="Failure to Appear" />
                                        <asp:ListItem Text="Party Declined to Participate" Value="Party Declined to Participate" />
                                        <asp:ListItem Text="Cancelled" Value="Cancelled" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEventDate" Text="Event Date" />
                                    <asp:TextBox runat="server" ID="txtEventDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" Text='<%#Bind("EventDate","{0:d}")%>' />
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="rblAgreementType" Text="Agreement" />
                                    <asp:RadioButtonList ID="rblAgreementType" CssClass="form-control radio-buttons" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
                                        SelectedValue='<%#Bind("AgreementType")%>'>
                                        <asp:ListItem Text="None" Value="N" Selected="True" />
                                        <asp:ListItem Text="Full" Value="F" />
                                        <asp:ListItem Text="Partial/Temporary" Value="C" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpMediatorType" Text="Mediator Type" />
                                    <asp:DropDownList ID="drpMediatorType" runat="server" ToolTip="Mediator Type" CssClass="form-control" ClientIDMode="Static" SelectedValue='<%#Bind("MediatorType")%>'>
                                        <asp:ListItem Text="< Select Mediator >" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Contracted" Value="Contracted" />
                                        <asp:ListItem Text="Staff" Value="Staff" />
                                        <asp:ListItem Text="Volunteer" Value="Volunteer" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-auto">
                                <asp:HiddenField ID="hdMediatorId" runat="server" ClientIDMode="Static" />
                                <asp:Label runat="server" AssociatedControlID="txtMediator" Text="Mediator Name" />
                                <asp:TextBox runat="server" ID="txtMediator" MaxLength="100" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("Mediator")%>' />
                            </div>
                            <div class="col-auto pt-4 mt-1">
                                <button class="btn btn-primary mediator-search" title="Search for Mediator" data-mediator="1">
                                    <i class="fas fa-search"></i>
                                </button>
                                <button class="btn btn-secondary ms-2" title="Clear Mediator" onclick="ValidateMediatorRemoval(event)">
                                    <i class="fas fa-times"></i>
                                </button>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkAdjournedTimeRemaining" runat="server" Text="Adjourned with time remaining?" Checked='<%#Bind("AdjournedTimeRemaining")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtHours" Text="Hours" />
                                    <asp:TextBox runat="server" ID="txtHours" step="0.01" TextMode="Number" MaxLength="15" ClientIDMode="Static" Text='<%#Bind("TimeRemaining")%>' CssClass="form-control" />
                                </div>
                            </div>
                        </div>
                        <p class="mt-3">
                            <asp:LinkButton ID="lnkUpdate" CssClass="btn btn-primary me-3" CommandName="update" CommandArgument='<%#Eval("eventId")%>'
                                runat="server"><i class="fas fa-save"></i> Update Event</asp:LinkButton>
                            <asp:LinkButton ID="lnkCancel" CssClass="btn btn-secondary" CommandName="cancel"
                                runat="server"><i class="fas fa-redo"></i> Cancel</asp:LinkButton>
                        </p>
                    </fieldset>
                </EditItemTemplate>
            </asp:ListView>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtComments" Text="Session Comments" />
                <asp:TextBox runat="server" ID="txtComments" ClientIDMode="Static" TextMode="MultiLine" Rows="3" CssClass="form-control" />
            </div>
                        <div class="modal fade" id="mediatorModal" tabindex="-1" role="dialog" aria-labelledby="mediatorModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="mediatorModalLabel">Mediator Search</h4>
                            <button type="button" class="close" onclick="CloseMediatorModal(event)" aria-hidden="true">&times;</button>
                        </div>
                        <div class="modal-body form-group">
                            <div class="row mb-3">
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtMediatorFirstName" Text="First Name" />
                                    <asp:TextBox runat="server" ID="txtMediatorFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtMediatorLastName" Text="Last Name" />
                                    <asp:TextBox runat="server" ID="txtMediatorLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>

                                <div class="col-auto pt-4">
                                    <button type="button" class="btn btn-primary" id="cmdMediatorSearch">Search</button>
                                </div>
                            </div>
                            <button class="btn btn-success btn-sm float-end mediator-add pull-down"><i class="fa fa-plus" aria-hidden="true"></i>Add Mediator</button>
                            <table id="tblMediators" class="table table-striped w-100">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Email</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" onclick="CloseMediatorModal(event)">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="EditMediatorModal" tabindex="-1" role="dialog" aria-labelledby="EditMediatorModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EditMediatorModalLabel">Add Mediator</h4>
                            <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="row g-3">
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstNameMed" Text="First Name" />
                                    <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstNameMed" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtLastNameMed" Text="Last Name" />
                                    <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastNameMed" />
                                </div>
                            </div>
                            <div class="row g-3">
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtEmailMed" Text="Email" />
                                    <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="250" ID="txtEmailMed" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtPhoneMed" Text="Phone" />
                                    <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="15" ID="txtPhoneMed" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" onclick="AddMediator(event)">Save</button>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>


        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdSaveSession" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdAddEvent" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdNewSession" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdPreviousSession" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdNextSession" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdDeleteSession" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

</div>
<p>
    <asp:LinkButton ID="cmdSave" runat="server"
        OnClick="cmdSave_Click" CssClass="btn btn-primary btn-lg"><i class="fas fa-save"></i> Save</asp:LinkButton>
    <asp:HyperLink ID="lnkCancel" CssClass="btn btn-secondary btn-lg" runat="server"><i class="fas fa-redo"></i> Reset</asp:HyperLink>
</p>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var lastNameMed = null;
    var firstNameMed = null;
    var pageSizeMed = 10;
    var rowOffsetMed = 0;
    var recordCountMed = 0;
    var sortDirectionMed = "asc";
    var sortColumnIndexMed = 1;
    var mediatorTable = null;
    var service = {
        path: "tjc.Modules/Mediation",
        framework: $.ServicesFramework(moduleId)
    };

    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            $("#txtLastName").focus();
            $("#txtSuffix").on("blur", function () {
                $("#txtLastName").focus();
            });
            $("#drpCountyLetter").on("change blur", function () {
                $("#txtLastName_p2").focus();
            });

            PageInit();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                PageInit();
            });
        });

    }(jQuery, window.Sys));
    function PageInit() {
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var medAction = "GetMediatorListItems";
        var medRestUrl = `${service.baseUrl}MediatorListItem/${medAction}/${recordCountMed}`;
        mediatorTable = $('#tblMediators').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: medRestUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.firstName = firstNameMed;
                    data.lastName = lastNameMed;
                },
            },
            columns: [{
                data: "mediatorid", render: function (data, type, row, meta) {
                    return `<a title="Select Mediator" data-id="${row.mediatorid}" data-mediatorname="${row.mediatorname}" data-first="${row.firstname}" data-last="${row.lastname}" onclick="SetMediator(event,this)" href="#"><i class="fas fa-user-tie"></i></a>`;
                }, className: "command-item", orderable: false
            },
            { data: "firstname" },
            { data: "lastname" },
            { data: "email" },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[sortColumnIndexMed, sortDirectionMed]],
            serverSide: true,
            process: true,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            pageLength: pageSizeMed,
        });
        $.fn.dataTable.ext.errMode = () => function (settings, helpPage, message) {
            alert("The Following Error Occurred Loading Attorney List:" + message);
        };
        $(document).on('show.bs.modal', '.modal', function (event) {
            var zIndex = 1040 + (10 * $('.modal:visible').length);
            $(this).css('z-index', zIndex);
            setTimeout(function () {
                $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
            }, 0);
        });
        $("#cmdMediatorSearch").on("click", function (e) {
            e.preventDefault();
            lastName = $("#txtMediatorLastName").val();
            firstName = $("#txtMediatorFirstName").val();
            mediatorTable.draw();
        });
        $(".mediator-search").on("click", function (e) {
            e.preventDefault();
            var attyModal = document.querySelector('#mediatorModal');
            var modal = bootstrap.Modal.getInstance(attyModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('mediatorModal'));
            }
            modal.show();
        });
        $(".mediator-add").on("click", function (e) {
            e.preventDefault();
            var medAddModal = document.querySelector('#EditMediatorModal');
            var modal = bootstrap.Modal.getInstance(medAddModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('EditMediatorModal'));
            }
            modal.show();
        });

        $(".radio-button-list input").addClass("form-check-input");
        $(".radio-button-list label").addClass("form-check-label");
        $(".datepicker").datepicker();
        $(".confirm-delete-event").on("click", function (e) {
            var item = $(this);
            e.preventDefault();
            $.dnnConfirm({
                text: 'Are you sure you wish to delete this Event?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Event?',
                callbackTrue: function () {
                    location.href = item[0].href;
                }
            });
        });
        $(".confirm-delete-session").dnnConfirm({
            text: 'Are you sure you wish to delete this Session?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Session?',
        });
        $(".confirm-delete-case").dnnConfirm({
            text: 'Are you sure you wish to delete this Case?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Case?',
        });
        $(".mediator-search").on("click", function (e) {
            e.preventDefault();
            var medModal = document.querySelector('#mediatorModal');
            var modal = bootstrap.Modal.getInstance(medModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('mediatorModal'));
            }
            modal.show();
        });
    }
    function hideReason(checkbox) {
        if (checkbox.checked == 1) {
            $(".reason").hide();
            $("select.reason").val('');
        } else {
            $(".reason").show();
        }
    }
    //* Mediator Functions*/
    function SetMediator(e, item) {
        e.preventDefault();
        ClearMediator();
        var mediatorId = item.dataset.id;
        var mediatorName = item.dataset.mediatorname;
        if (mediatorId && mediatorId != "undefined")
            $("#hdMediatorId").val(mediatorId);
        if (mediatorName != "null" && mediatorName != "undefined")
            $("#txtMediator").val(mediatorName);
        var medModal = document.querySelector('#mediatorModal');
        var modal = bootstrap.Modal.getInstance(medModal);
        modal.hide();
    }
    function AddMediator(e) {
        e.preventDefault();
        service.baseUrl = service.framework.getServiceRoot(service.path) + "MediatorListItem/";
        var action = "add-mediator";
        var firstName = $("#txtFirstNameMed").val();
        var lastName = $("#txtLastNameMed").val();
        var email = $("#txtEmailMed").val();
        var phone = $("#txtPhoneMed").val();
        var mediator = { firstname: firstName, lastname: lastName, emai: email, phone: phone };
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: service.baseUrl + action,
                beforeSend: service.framework.setModuleHeaders,
                data: (mediator),
                success: function (result) {
                    ClearAddMediatorForm();
                    var medAddModal = document.querySelector('#EditMediatorModal');
                    var modal = bootstrap.Modal.getInstance(medAddModal);
                    if (!modal) {
                        modal = new bootstrap.Modal(document.getElementById('EditMediatorModal'));
                    }
                    modal.hide();
                    mediatorTable.ajax.reload();
                    $("#txtMediatorLastName").val(lastName);
                    $("#txtMediatorFirstName").val(firstName);
                    lastNameMed = lastName;
                    firstNameMed = firstName;
                    mediatorTable.draw();
                },
                error: function (xhr, status, error) {
                    // alert(xhr.responseText);
                    alert("Unable to add mediator.\n\nMake sure you are logged in and try again.");
                }
            });
        } catch (e) {
            alert("Unable to add mediator.\n\nMake sure you are logged in and try again.");
        }
        return false;
    }
    function ClearMediator() {
        $("#hdMediatorId").val("");
        $("#txtMediator").val("");
    }
    function ClearAddMediatorForm() {
        $("#txtFirstNameMed").val("");
        $("#txtLastNameMed").val("");
        $("#txtEmailMed").val("");
        $("#txtPhoneMed").val("");
    }
    function ValidateMediatorRemoval(e) {
        e.preventDefault();
        $.dnnConfirm({
            text: 'Are you sure you wish to remove this Mediator?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Remove Mediator?',
            callbackTrue: function () {
                ClearMediator();
            }
        });
    }
    function CloseMediatorModal(e) {
        e.preventDefault();
        let modal = bootstrap.Modal.getInstance(document.getElementById("mediatorModal"));
        modal.hide();
    }
</script>
