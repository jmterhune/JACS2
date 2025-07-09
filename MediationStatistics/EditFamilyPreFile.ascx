<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFamilyPreFile.ascx.cs" Inherits="tjc.Modules.MediationStatistics.EditFamilyPreFile" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltHeading" runat="server"><h4>{0}:&nbsp{1}</h4></asp:Literal>
<div id="family-prefile-form">
    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Case Toolbar">
        <div class="btn-group" role="group" aria-label="Basic example">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary" runat="server"><i class="fas fa-search"></i> Case Search</asp:HyperLink>
            <asp:HyperLink ID="lnkNew" CssClass="btn btn-dark" runat="server"><i class="fas fa-plus"></i> New Case</asp:HyperLink>
            <asp:LinkButton ID="cmdDelete" CssClass="btn btn-secondary confirm-delete-case" runat="server" OnClick="cmdDelete_Click"><i class="fas fa-trash"></i> Delete Case</asp:LinkButton>
        </div>
    </div>
    <div class="row">
        <div class="col-6">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtCaseYear" Text="Case Number" />
                <div class="input-group">
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase case-code-field" placeholder="CC" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="6" CssClass="form-control upperCase" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtSuffix" title="Suffix" runat="server" MaxLength="4" CssClass="form-control upperCase case-code-field" ClientIDMode="Static"></asp:TextBox>
                    <div class="input-group-append">
                        <small class="input-group-text form-control rounded-end" title="Year - Case Type - Case Sequence - Suffix">(Format: YYYY-CC-000000-NC)</small>
                    </div>
                </div>
            </div>
            <fieldset class="outline-fieldset mb-0">
                <legend>Claimant</legend>
                <div class="form-group">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-12">
                            <asp:Label runat="server" AssociatedControlID="txtBusinessName" Text="Business Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtBusinessName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
        <div class="col-6">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="drpCountyLetter" ToolTip="Citizen's Dispute Settlement Program Case Number" Text="CDSP Number" />
                <div class="input-group">
                    <asp:DropDownList TabIndex="1" runat="server" CssClass="form-control" ID="drpCDSPType" ClientIDMode="Static">
                        <asp:ListItem Text="< Select Type >" Value="" />
                        <asp:ListItem Text="CDSP" title="Citizen's Dispute Settlement Program" />
                        <asp:ListItem Text="CDSPF" title="Citizen's Dispute Settlement Program Family" />
                    </asp:DropDownList>
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtCDSPYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox AutoCompleteType="Disabled" TabIndex="1" ID="txtCDSPNumber" title="Case Type" runat="server" MaxLength="3" CssClass="form-control upperCase" placeholder="000" ClientIDMode="Static"></asp:TextBox>
                    <asp:DropDownList TabIndex="1" ID="drpCountyLetter" runat="server" ToolTip="County" CssClass="form-control location-field" ClientIDMode="Static">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="D" Value="D" title="DeSoto"></asp:ListItem>
                        <asp:ListItem Text="M" Value="M" title="Manatee"></asp:ListItem>
                        <asp:ListItem Text="S" Value="S" title="Sarasota"></asp:ListItem>
                        <asp:ListItem Text="V" Value="V" title="Venice"></asp:ListItem>
                    </asp:DropDownList>
                    <div class="input-group-append">
                        <small class="input-group-text form-control rounded-end" title="Type - Year - Number - Location">(Format: CDSP-YYYY-000-C)</small>
                    </div>
                </div>
            </div>
            <fieldset class="outline-fieldset mb-0">
                <legend>Respondent</legend>
                <div class="form-group">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtLastName_p2" Text="Last Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtLastName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtFirstName_p2" Text="First Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtFirstName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-12">
                            <asp:Label runat="server" AssociatedControlID="txtBusinessName_p2" Text="Business Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtBusinessName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
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
                        <asp:Label runat="server" AssociatedControlID="txtReferralSource" Text="Program Referral Source" />
                        <div class="combo-container">
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtReferralSource" MaxLength="50" ClientIDMode="Static" CssClass="combo form-control" />
                            <datalist id="dlReferralSource" class="form-control combo-list">
                                <asp:Literal ID="ltReferralSourceOptions" runat="server" />
                            </datalist>
                        </div>
                        <div id="emailHelp" class="form-text">Delete current value to see full list of options.</div>

                    </div>
                    <div class="form-check form-switch mt-4">
                        <asp:CheckBox ID="chkTelephoneSession" runat="server" Text="Virtual Session" />
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtCaseReceived" Text="Date Case Received" />
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtCaseReceived" TextMode="Date" MaxLength="15" ClientIDMode="Static" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtMediationDate" Text="Mediation Date / Resolved" />
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtMediationDate" MaxLength="15" TextMode="Date" ClientIDMode="Static" CssClass="form-control" />
                    </div>
                </div>
                <fieldset class="outline-fieldset" id="fsSecondaryIssues" runat="server">
                    <legend>Pre-File Issues</legend>
                    <asp:CheckBoxList ID="clsSecondaryIssues" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList">
                    </asp:CheckBoxList>
                </fieldset>

            </div>
            <hr />
            <div class="btn-toolbar mb-3" role="toolbar" aria-label="Session Events Toolbar">
                <div class="btn-group" role="group" aria-label="Session Events Actions">
                    <asp:LinkButton ID="cmdSaveSession" runat="server"
                        OnClick="cmdSave_Click" CssClass="btn btn-primary"><i class="fas fa-save"></i> Save</asp:LinkButton>
                    <button onclick="return ClearEventForm()" id="cmdAddNewEvent" class="btn btn-dark" data-bs-toggle="modal" data-bs-target="#EventModal"><i class="fa fa-plus"></i>&nbsp;New Event</button>
                    <div class="input-group-text" id="eventInfo">
                        Session Events
                    </div>
                    <asp:HiddenField ID="hdSessionId" runat="server" />
                </div>
            </div>
            <asp:Repeater ID="rptEvent" runat="server" OnItemCommand="rptEvent_ItemCommand" OnItemDataBound="rptEvent_ItemDataBound" OnItemCreated="rptEvent_ItemCreated">
                <ItemTemplate>
                    <div class="template">
                        <legend>Event Number <%#Container.ItemIndex + 1%>
                            <asp:Label ID="lblHoursRemainingItem" runat="server" CssClass="ms-5 fw-bold badge badge-danger">Hours Remaining: <%#Eval("TimeRemaining","{0:n}")%></asp:Label></legend>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch mt-4">
                                        <input class="form-check-input" type="checkbox" id="chkMeetingHeldItem" <%#Convert.ToBoolean(Eval("MediationHeld"))?"checked":""%>>
                                        <label class="form-check-label" for="chkMeetingHeldItem">Mediation Held</label>
                                    </div>
                                    <%#Convert.ToBoolean(Eval("MediationHeld").ToString())?"":"<div class='row'><div class='col-12'><div class='form-group'><label for='txtReasonNotHeld' class='form-label'>Reason Not Held:</label> <span id='txtReasonNotHeld'>" + Eval("ReasonNotHeld", "{0:d}") + "</span></div></div></div>"%>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <label for="txtEventDateItem" class="form-label">Event Date</label>
                                    <input id="txtEventDateItem" class="form-control" type="text" value='<%#Eval("EventDate", "{0:d}")%>' />
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <label for="txtAgreementTypeItem">Agreement Type</label>
                                    <input class="form-control" type="text" id="txtAgreementTypeItem" value='<%#GetAgreementType(Eval("AgreementType").ToString())%>'>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <label for="txtMediatoryTypeItem">Mediator Type</label>
                                    <input id="txtMediatorTypeItem" class="form-control" type="text" value='<%#Eval("MediatorType")%>' />
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <label for="txtMediatorItem">Mediator Name</label>
                                    <input id="txtMediatorItem" class="form-control" type="text" value='<%#Eval("MediatorName")%>' />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" id="chkAdjournedTimeRemainingItem" <%#Convert.ToBoolean(Eval("AdjournedTimeRemaining"))?"checked":""%>>
                                        <label class="form-check-label me-3" for="chkAdjournedTimeRemainingItem">Adjourned with Time Remaining</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <fieldset class="outline-fieldset mb-3">
                            <legend class="small">Appearance Record</legend>
                            <%#GetAppearanceItems(Eval("EventID").ToString())%>
                        </fieldset>
                        <p class="mb-0 mt-3">
                            <asp:LinkButton ID="lnkUpdate" OnClientClick="UpdateEventHeader(event)" CssClass="btn btn-primary" CommandName="edit" CommandArgument='<%#Eval("EventId") %>' runat="server"><i class="fas fa-pencil"></i> Edit Event</asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" CssClass="btn btn-secondary confirm-delete-event" CommandName="delete" CommandArgument='<%#Eval("EventId") %>' runat="server"><i class="fas fa-trash"></i> Delete</asp:LinkButton>
                        </p>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <hr />
                </SeparatorTemplate>
            </asp:Repeater>
            <div class="form-group">
                <fieldset class="outline-fieldset pt-0 pb-0">
                    <legend class="mb-0">
                        <asp:Label runat="server" AssociatedControlID="txtComments" Text="Session Comments" /></legend>
                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtComments" ClientIDMode="Static" TextMode="MultiLine" Rows="3" CssClass="form-control border-0" />
                </fieldset>
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
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtMediatorFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtMediatorLastName" Text="Last Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtMediatorLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
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
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstNameMed" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtLastNameMed" Text="Last Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastNameMed" />
                                </div>
                            </div>
                            <div class="row g-3">
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtEmailMed" Text="Email" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="250" ID="txtEmailMed" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtPhoneMed" Text="Phone" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="15" ID="txtPhoneMed" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer justify-content-between">
                            <button type="button" class="btn btn-primary" onclick="AddMediator(event)">Save</button>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="EventModal" tabindex="-1" role="dialog" aria-labelledby="EventModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EventModalLabel">Add / Edit Event</h4>
                            <button type="button" class="close" onclick="CloseEventModal(event)" aria-hidden="true">&times;</button>
                        </div>
                        <div class="modal-body form-group">
                            <asp:HiddenField ID="hdEventId" runat="server" />
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch mt-4">
                                            <asp:CheckBox ID="chkMeetingHeld" runat="server" Checked='<%#Bind("MediationHeld")%>' ClientIDMode="Static" onclick="hideReason(this)" Text="Mediation Held" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" ID="lblReason" AssociatedControlID="drpReason" CssClass="reason" Text="Reason Not Held" />
                                        <asp:DropDownList ID="drpReason" runat="server" CssClass="form-control reason selectMe" ClientIDMode="Static">
                                            <asp:ListItem Value="" Text="< Select Reason>" />
                                            <asp:ListItem Text="Not Eligible" Value="Not Eligible" />
                                            <asp:ListItem Text="Settled Prior" Value="Settled Prior" />
                                            <asp:ListItem Text="Session Rescheduled" Value="Session Rescheduled" />
                                            <asp:ListItem Text="Failure to Appear" Value="Failure to Appear" />
                                            <asp:ListItem Text="Party Declined to Participate" Value="Party Declined to Participate" />
                                            <asp:ListItem Text="Cancelled" Value="Cancelled" />
                                            <asp:ListItem Text="No Response" Value="No Response" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtEventDate" Text="Event Date" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtEventDate" MaxLength="15" ClientIDMode="Static" TextMode="Date" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="rblAgreementType" Text="Agreement" />
                                        <asp:RadioButtonList ID="rblAgreementType" CssClass="form-control radio-button-list agreement" ClientIDMode="Static" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
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
                                        <asp:DropDownList ID="drpMediatorType" runat="server" ToolTip="Mediator Type" CssClass="form-control" ClientIDMode="Static">
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
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtMediator" MaxLength="100" ClientIDMode="Static" CssClass="form-control" />
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
                                        <div class="form-check form-switch mt-2">
                                            <asp:CheckBox ID="chkAdjournedTimeRemaining" ClientIDMode="Static" runat="server" Text="Adjourned with time remaining?" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group row">
                                        <asp:Label runat="server" AssociatedControlID="txtHours" Text="Hours" CssClass="col-auto col-form-label" />
                                        <div class="col-auto">
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtHours" step="0.01" TextMode="Number" MaxLength="15" ClientIDMode="Static" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <fieldset class="outline-fieldset">
                                <legend class="small">Appearance Record</legend>
                                <asp:CheckBoxList ID="cblAppearanceRecord" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch appearance-record" RepeatLayout="UnorderedList" DataTextField="Description" DataValueField="AppearanceId">
                                </asp:CheckBoxList>
                            </fieldset>
                            <div class="modal-footer justify-content-between">
                                <asp:LinkButton ID="cmdSaveEvent" OnClick="cmdSaveEvent_Click" CssClass="btn btn-primary" runat="server"><i class="fas fa-save"></i> Save Event</asp:LinkButton>
                                <button type="button" class="btn btn-default" onclick="CloseEventModal(event)">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdSaveSession" EventName="Click" />
            <asp:PostBackTrigger ControlID="cmdSaveEvent" />
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
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

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
        const dataList = document.getElementById('dlReferralSource');
        const input = document.getElementById('txtReferralSource');
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
        dataList.querySelectorAll('option').forEach((el, idx, arr) => {
            el.addEventListener('click', (e) => {
                input.value = el.value;
            });
        });
        input.addEventListener('focus', showList);
        input.addEventListener('blur', () => {
            setTimeout(() => {
                dataList.classList.remove('show');
            }, 300);
        });
        input.addEventListener('keyup', showList);
        function showList() {
            if (!!input.value) {
                input.setAttribute("list", "dlReferralSource");
                dataList.classList.remove('show');
            } else {
                input.removeAttribute("list");
                dataList.classList.add('show');
            }
        }
        $(".radio-button-list input").addClass("form-check-input");
        $(".radio-button-list label").addClass("form-check-label");
        //mediator search
        $('input').keypress(function (event) {
            if (event.which === 13) { // 13 is the keycode for Enter
                event.preventDefault();
            }
        });

        $('#txtMediatorFirstName').keypress(function (event) {
            if (event.which === 13) { // 13 is the keycode for Enter
                event.preventDefault();
                $('#cmdMediatorSearch').click();
            }
        });
        $('#txtMediatorLastName').keypress(function (event) {

            if (event.which === 13) { // 13 is the keycode for Enter
                event.preventDefault();
                $('#cmdMediatorSearch').click();
            }
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
        $("#cmdMediatorSearch").on("click", function (e) {
            e.preventDefault();
            lastNameMed = $("#txtMediatorLastName").val();
            firstNameMed = $("#txtMediatorFirstName").val();
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
        //confirmations
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
    }
    function hideReason(checkbox) {
        if (checkbox.checked == 1) {
            $(".reason").hide();
            $("select.reason").val('');
        } else {
            $(".reason").show();
        }
    }
    function UpdateEventHeader(e) {
        $("#EventModalLabel").html("Edit Event")
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
    function ClearEventForm() {
        $("#hdEventId").val("");
        $("#chkMeetingHeld").prop("checked", false);
        $("#drpReason").val("");
        $(".reason").show();
        $(".agreement input:radio:checked").removeAttr("checked");
        $("#drpMediatorType").val("");
        $("#hdMediatorId").val("");
        $("#txtMediator").val("");
        $("#txtEventDate").val("");
        $("#txtHours").val("");
        $("#chkAdjournedTimeRemaining").prop("checked", false);
        $(".appearance-record input[type='checkbox']").prop("checked", false);
    }
    function ToggleEventForm(toggleValue) {
        if (toggleValue) {
            $('#EventModal').modal('show');
            if ($("#chkMeetingHeld").is(":checked")) {
                $(".reason").hide();
                $("select.reason").val('');
            } else {
                $(".reason").show();
            }
        } else {
            $('#EventModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            // modal.hide();
        }
        return true;
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
    function CloseEventModal(e) {
        ClearEventForm();
        e.preventDefault();
        let modal = bootstrap.Modal.getInstance(document.getElementById("EventModal"));
        modal.hide();
    }

</script>
