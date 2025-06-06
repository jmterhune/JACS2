<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCountyClaims.ascx.cs" Inherits="tjc.Modules.MediationStatistics.EditCountyClaims" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltHeading" runat="server"><h4>{0}:&nbsp{1}</h4></asp:Literal>
<div id="county-form">
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
        </div>
    </div>
    <div class="row">
        <div class="col-6">
            <fieldset class="outline-fieldset mb-0">
                <legend>Plaintiff</legend>
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
            <fieldset class="outline-fieldset mb-0">
                <legend>Defendant</legend>
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
            <fieldset class="outline-fieldset">
                <legend>Plaintiff's Attorney</legend>
                <asp:HiddenField ID="hdPlaintiffAttorneyId" runat="server" ClientIDMode="Static" />
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch mt-4">
                                <asp:CheckBox ID="chkProSePlaintiff" runat="server" Text="Pro Se" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPlaintiffName" Text="Plaintiff Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtPlaintiffName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />

                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPlaintiffEmail" Text="Email" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtPlaintiffEmail" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPlaintiffPhone" Text="Phone" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtPlaintiffPhone" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPlaintiffExtension" Text="Extension" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtPlaintiffExtension" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto pt-4">
                        <button class="btn btn-primary attorney-search" title="Search for Attorney" data-attorney="1">
                            <i class="fas fa-search"></i>
                        </button>
                        <button class="btn btn-secondary ms-2" title="Clear Attorney Fields" onclick="ValidateAttorneyRemoval(1)">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </fieldset>
            <fieldset class="outline-fieldset">
                <legend>Defendant's Attorney</legend>
                <asp:HiddenField ID="hdDefendantAttorneyId" runat="server" ClientIDMode="Static" />
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch mt-4">
                                <asp:CheckBox ID="chkProSeDefendant" runat="server" Text="Pro Se" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtDefendantName" Text="Defendant Name" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtDefendantName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtDefendantEmail" Text="Email" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtDefendantEmail" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtDefendantPhone" Text="Phone" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtDefendantPhone" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtDefendantExtension" Text="Extension" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtDefendantExtension" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto pt-4">
                        <button class="btn btn-primary attorney-search" title="Search for Attorney" data-attorney="2">
                            <i class="fas fa-search"></i>
                        </button>
                        <button class="btn btn-secondary ms-2" title="Clear Attorney Fields" onclick="ValidateAttorneyRemoval(2)">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </fieldset>
            <fieldset class="outline-fieldset">
                <legend>General Information</legend>
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Case Type" />
                            <asp:DropDownList ID="drpCaseType" runat="server" ToolTip="Case Type" CssClass="form-control" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtOrderReferral" Text="Order of Referral" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtOrderReferral" MaxLength="15" TextMode="Date" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtMediationDate" Text="Mediation Date / Resolved" />
                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtMediationDate" MaxLength="15" ClientIDMode="Static" TextMode="Date" CssClass="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row mt-3">
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkFeeJudgmentEntered" runat="server" Text="Fee judgment Entered" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkFeeAgreementEntered" runat="server" Text="Fee Agreement Entered" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkInterpreterRequested" runat="server" Text="Interpreter Requested" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkDepartmentFeeWaiver" runat="server" Text="Department Fee Waiver" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkTelephoneSession" runat="server" Text="Virtual Session" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkCircuitCivilReferal" runat="server" Text="Circuit Civil Referral" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkArbitrationReferral" runat="server" Text="Arbitration Referral" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkOTSC" runat="server" ToolTip="Order to Show Cause" Text="OTSC" />
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
            <fieldset class="outline-fieldset">
                <legend>Fee Information</legend>
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="drpFeeAmount" Text="Fee Amount" />
                            <asp:DropDownList runat="server" ID="drpFeeAmount" CssClass="form-control">
                                <asp:ListItem Text="< Select Fee Amount >" Value="" />
                                <asp:ListItem Text="$60" Value="$60" />
                                <asp:ListItem Text="Indigent" Value="Indigent" />
                                <asp:ListItem Text="Eviction" Value="Eviction" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-auto">
                        <fieldset class="outline-fieldset">
                            <legend class="small">Fee's Paid By</legend>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpPlaintiffFeesPaid" Text="Plaintiff" />
                                        <asp:DropDownList ID="drpPlaintiffFeesPaid" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Fee Paid >" Value="" />
                                            <asp:ListItem Text="$0" />
                                            <asp:ListItem Text="$60" />
                                            <asp:ListItem Text="Indigent" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpDefendantFeesPaid" Text="Defendant" />
                                        <asp:DropDownList ID="drpDefendantFeesPaid" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Fee Paid >" Value="" />
                                            <asp:ListItem Text="$0" />
                                            <asp:ListItem Text="$60" />
                                            <asp:ListItem Text="Indigent" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                    <div class="col-auto">
                        <fieldset class="outline-fieldset">
                            <legend class="small">Fee's Owed By</legend>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpPlaintiffFeesOwed" Text="Plaintiff" />
                                        <asp:DropDownList ID="drpPlaintiffFeesOwed" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Fee Owed >" Value="" />
                                            <asp:ListItem Text="$0" />
                                            <asp:ListItem Text="$60" />
                                            <asp:ListItem Text="Certificate of Indigency" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-auto pt-4">
                                    <div class="form-check form-switch mt-2">
                                        <asp:CheckBox ID="chkPlaintiffFta" Text="P-FTA" ToolTip="Plaintiff Failure to Appear" runat="server" />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpDefendantFeesOwed" Text="Defendant" />
                                        <asp:DropDownList ID="drpDefendantFeesOwed" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Fee Owed >" Value="" />
                                            <asp:ListItem Text="$0" />
                                            <asp:ListItem Text="$60" />
                                            <asp:ListItem Text="Certificate of Indigency" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-auto pt-4">
                                    <div class="form-check form-switch mt-2">
                                        <asp:CheckBox ID="chkDefendantFta" ToolTip="Defendant Failure to Appear" Text="R-FTA" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
            </fieldset>
            <fieldset class="outline-fieldset" id="fsSecondaryIssues" runat="server">
                <legend>Secondary Issues</legend>
                <asp:CheckBoxList ID="clsSecondaryIssues" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList">
                </asp:CheckBoxList>
            </fieldset>
            <hr />
            <div class="btn-toolbar mb-3" role="toolbar" aria-label="Session Events Toolbar">
                <div class="btn-group" role="group" aria-label="Session Events Actions">
                    <asp:LinkButton ID="cmdSaveSession" runat="server"
                        OnClick="cmdSave_Click" CssClass="btn btn-primary session-save"><i class="fas fa-save"></i> Save</asp:LinkButton>
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
                        <fieldset disabled>
                            <legend>Event Number <%#Container.ItemIndex + 1%>
                                <asp:Label ID="lblHoursRemaining" runat="server" CssClass="ms-5 fw-bold badge badge-danger">Hours Remaining: <%#Eval("TimeRemaining","{0:n}")%></asp:Label></legend>
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
                                        <label for="txtMediatorTypeItem">Mediator Type</label>
                                        <input id="txtMediatorTypeItem" class="form-control" type="text" value='<%#Eval("MediatorType")%>' />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <label for="txtMediatorNameItem">Mediator Name</label>
                                        <input id="txtMediatorNameItem" class="form-control" type="text" value='<%#Eval("MediatorName")%>' />
                                    </div>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkSubmittedToPartiesItem" <%#Convert.ToBoolean(Eval("AgreementSubmittedParties"))?"checked":""%>>
                                            <label class="form-check-label" for="chkSubmittedToPartiesItem">Submitted to Parties</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkAgreementSignedItem" <%#Convert.ToBoolean(Eval("AgreementSigned"))?"checked":""%>>
                                            <label class="form-check-label" for="chkAgreementSignedItem">Parties Signed Agreement</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkPreparedAttorneyItem" <%#Convert.ToBoolean(Eval("AgreementPreparedAttorney"))?"checked":""%>>
                                            <label class="form-check-label" for="chkPreparedAttorneyItem">Prepared by Attorney</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkAdjournedTimeRemainingItem" <%#Convert.ToBoolean(Eval("AdjournedTimeRemaining"))?"checked":""%>>
                                            <label class="form-check-label me-3" for="chkAdjournedTimeRemainingItem">Adjourned with Time Remaining</label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <fieldset class="outline-fieldset">
                                <legend class="small">Appearance Record</legend>
                                <%#GetAppearanceItems(Eval("EventID").ToString())%>
                            </fieldset>
                        </fieldset>
                        <p class="mb-0 mt-3">
                            <asp:LinkButton ID="lnkUpdate" OnClientClick="UpdateEventHeader(event)" CssClass="btn btn-primary event-save" CommandName="edit" CommandArgument='<%#Eval("EventId") %>' runat="server"><i class="fas fa-pencil"></i> Edit Event</asp:LinkButton>
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
            <div class="modal fade" id="EventModal" tabindex="-1" role="dialog" aria-labelledby="EventModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EventModalLabel">Add / Edit Event</h4>
                            <button type="button" class="close event-close" onclick="CloseEventModal(event)">&times;</button>
                        </div>
                        <div class="modal-body form-group">
                            <asp:HiddenField ID="hdEventId" runat="server" />
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch mt-4">
                                            <asp:CheckBox ID="chkMeetingHeld" ClientIDMode="Static" runat="server" onclick="hideReason(this)" Text="Mediation Held" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" ID="lblReason" AssociatedControlID="drpReason" CssClass="reason" Text="Reason Not Held" />
                                        <asp:DropDownList ID="drpReason" ClientIDMode="Static" runat="server" CssClass="form-control reason selectMe">
                                            <asp:ListItem Value="" Text="< Select Reason>" />
                                            <asp:ListItem Text="Not Eligible" Value="Not Eligible" />
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
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtEventDate" MaxLength="15" TextMode="Date" ClientIDMode="Static" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="rblAgreementType" Text="Agreement" />
                                        <asp:RadioButtonList ClientIDMode="Static" ID="rblAgreementType" CssClass="form-control radio-button-list agreement" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
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
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" Enabled="false" ID="txtMediator" MaxLength="100" ClientIDMode="Static" CssClass="form-control" />
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
                            <div class="row mt-2">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <asp:CheckBox ID="chkSubmittedToParties" ClientIDMode="Static" Text="Submitted to Parties" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <asp:CheckBox ID="chkAgreementSigned" ClientIDMode="Static" Text="Parties Signed Agreement" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <asp:CheckBox ID="chkPreparedAttorney" ClientIDMode="Static" Text="Prepared by Attorney" runat="server" />
                                        </div>
                                    </div>
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
                                        <asp:Label runat="server" AssociatedControlID="txtHours" CssClass="col-auto col-form-label" Text="Hours" />
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
                        </div>
                        <div class="modal-footer justify-content-between">
                            <asp:LinkButton ID="cmdSaveEvent" OnClick="cmdSaveEvent_Click" CssClass="btn btn-primary" runat="server"><i class="fas fa-save"></i> Save Event</asp:LinkButton>
                            <button type="button" class="btn btn-default event-close" onclick="CloseEventModal(event)">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="attorneyModal" tabindex="-1" role="dialog" aria-labelledby="attorneyModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="attorneyModalLabel">Attorney Search</h4>
                            <button type="button" class="close" onclick="CloseAttorneyModal(event)">&times;</button>
                        </div>
                        <div class="modal-body form-group">
                            <div class="row mb-3">
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyFirstName" Text="First Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtAttorneyFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyLastName" Text="Last Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtAttorneyLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyFirm" Text="Firm" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtAttorneyFirm" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto pt-4">
                                    <button type="button" class="btn btn-primary" id="cmdSearch">Search</button>
                                </div>
                            </div>
                            <button class="btn btn-success btn-sm float-end attorney-add pull-down"><i class="fa fa-plus" aria-hidden="true"></i>Add Attorney</button>
                            <table id="tblAttorneys" class="table table-striped w-100">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Firm</th>
                                        <th>Email</th>
                                        <th>Phone</th>
                                        <th>Extension</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" onclick="CloseAttorneyModal(event)">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="EditAttorneyModal" tabindex="-1" role="dialog" aria-labelledby="EditAttorneyModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EditAttorneyModalLabel">Add Attorney</h4>
                            <button type="button" class="close" data-bs-dismiss="modal">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="row g-3">
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstNameAdd" Text="First Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstNameAdd" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtLastNameAdd" Text="Last Name" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastNameAdd" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtFirm" Text="Firm" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirm" />
                                </div>
                            </div>
                            <div class="row g-3">
                                <div class="col">
                                    <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="250" ID="txtEmail" />
                                </div>
                                <div class="col-4">
                                    <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Phone" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="15" ID="txtPhone" />
                                </div>
                                <div class="col-4">
                                    <asp:Label runat="server" AssociatedControlID="txtExtension" Text="Extension" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="10" ID="txtExtension" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress" Text="Address" />
                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress" />
                            </div>
                            <div class="row g-3">
                                <div class="col-5">
                                    <asp:Label runat="server" AssociatedControlID="txtCity" Text="City" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCity" />
                                </div>
                                <div class="col-3">
                                    <asp:Label runat="server" AssociatedControlID="drpState" Text="State" />
                                    <asp:DropDownList ID="drpState" ClientIDMode="Static" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="" Text="" />
                                        <asp:ListItem Value="AL" Text="Alabama" />
                                        <asp:ListItem Value="AK" Text="Alaska" />
                                        <asp:ListItem Value="AZ" Text="Arizona" />
                                        <asp:ListItem Value="AR" Text="Arkansas" />
                                        <asp:ListItem Value="CA" Text="California" />
                                        <asp:ListItem Value="CO" Text="Colorado" />
                                        <asp:ListItem Value="CT" Text="Connecticut" />
                                        <asp:ListItem Value="DE" Text="Delaware" />
                                        <asp:ListItem Value="DC" Text="District of Columbia" />
                                        <asp:ListItem Value="FL" Text="Florida" />
                                        <asp:ListItem Value="GA" Text="Georgia" />
                                        <asp:ListItem Value="HI" Text="Hawaii" />
                                        <asp:ListItem Value="ID" Text="Idaho" />
                                        <asp:ListItem Value="IL" Text="Illinois" />
                                        <asp:ListItem Value="IN" Text="Indiana" />
                                        <asp:ListItem Value="IA" Text="Iowa" />
                                        <asp:ListItem Value="KS" Text="Kansas" />
                                        <asp:ListItem Value="KY" Text="Kentucky" />
                                        <asp:ListItem Value="LA" Text="Louisiana" />
                                        <asp:ListItem Value="ME" Text="Maine" />
                                        <asp:ListItem Value="MD" Text="Maryland" />
                                        <asp:ListItem Value="MA" Text="Massachusetts" />
                                        <asp:ListItem Value="MI" Text="Michigan" />
                                        <asp:ListItem Value="MN" Text="Minnesota" />
                                        <asp:ListItem Value="MS" Text="Mississippi" />
                                        <asp:ListItem Value="MO" Text="Missouri" />
                                        <asp:ListItem Value="MT" Text="Montana" />
                                        <asp:ListItem Value="NE" Text="Nebraska" />
                                        <asp:ListItem Value="NV" Text="Nevada" />
                                        <asp:ListItem Value="NH" Text="New Hampshire" />
                                        <asp:ListItem Value="NJ" Text="New Jersey" />
                                        <asp:ListItem Value="NM" Text="New Mexico" />
                                        <asp:ListItem Value="NY" Text="New York" />
                                        <asp:ListItem Value="NC" Text="North Carolina" />
                                        <asp:ListItem Value="ND" Text="North Dakota" />
                                        <asp:ListItem Value="OH" Text="Ohio" />
                                        <asp:ListItem Value="OK" Text="Oklahoma" />
                                        <asp:ListItem Value="OR" Text="Oregon" />
                                        <asp:ListItem Value="PA" Text="Pennsylvania" />
                                        <asp:ListItem Value="RI" Text="Rhode Island" />
                                        <asp:ListItem Value="SC" Text="South Carolina" />
                                        <asp:ListItem Value="SD" Text="South Dakota" />
                                        <asp:ListItem Value="TN" Text="Tennessee" />
                                        <asp:ListItem Value="TX" Text="Texas" />
                                        <asp:ListItem Value="UT" Text="Utah" />
                                        <asp:ListItem Value="VT" Text="Vermont" />
                                        <asp:ListItem Value="VA" Text="Virginia" />
                                        <asp:ListItem Value="WA" Text="Washington" />
                                        <asp:ListItem Value="WV" Text="West Virginia" />
                                        <asp:ListItem Value="WI" Text="Wisconsin" />
                                        <asp:ListItem Value="WY" Text="Wyoming" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-4">
                                    <asp:Label runat="server" AssociatedControlID="txtZip" Text="Zip" />
                                    <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtZip" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer justify-content-between">
                            <button type="button" class="btn btn-primary" onclick="AddAttorney(event)">Save</button>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="mediatorModal" tabindex="-1" role="dialog" aria-labelledby="mediatorModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="mediatorModalLabel">Mediator Search</h4>
                            <button type="button" class="close" onclick="CloseMediatorModal(event)">&times;</button>
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
                            <button type="button" class="close" data-bs-dismiss="modal">&times;</button>
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
        OnClick="cmdSave_Click" CssClass="btn btn-primary btn-lg case-save"><i class="fas fa-save"></i> Save</asp:LinkButton>
    <asp:HyperLink ID="lnkCancel" CssClass="btn btn-secondary btn-lg" runat="server"><i class="fas fa-redo"></i> Reset</asp:HyperLink>
</p>
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var lastName = null;
    var firstName = null;
    var lastNameMed = null;
    var firstNameMed = null;
    var firm = null;
    var pageSize = 10;
    var rowOffset = 0;
    var recordCount = 0;
    var pageSizeMed = 10;
    var rowOffsetMed = 0;
    var recordCountMed = 0;
    var sortDirection = "asc";
    var sortColumnIndex = 1;
    var sortDirectionMed = "asc";
    var sortColumnIndexMed = 1;
    var attorneyRole = 0;
    var attorneyTable = null;
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
        var attyAction = "GetAttorneyListItems";
        var attyRestUrl = `${service.baseUrl}AttorneyListItem/${attyAction}/${recordCount}`;
        attorneyTable = $('#tblAttorneys').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: attyRestUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.firstName = firstName;
                    data.lastName = lastName;
                    data.firm = firm;
                },
            },
            columns: [{
                data: "attorneyid", render: function (data, type, row, meta) {
                    return `<a title="Select Attorney" data-id="${row.attorneyid}" data-name="${row.attorneyname} " data-first="${row.firstname}" data-last="${row.lastname}" data-phone="${row.phone}" data-email="${row.email}" data-extension="${row.extension}" onclick="SetAttorney(event,this)" href="#"><i class="fas fa-user-tie"></i></a>`;
                }, className: "command-item", orderable: false
            },
            { data: "firstname" },
            { data: "lastname" },
            { data: "firm" },
            { data: "email" },
            { data: "phone" },
            { data: "extension" },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[sortColumnIndex, sortDirection]],
            serverSide: true,
            process: true,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            pageLength: pageSize,
        });
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
        $('#txtAttorneyLastName').keypress(function (event) {
            if (event.which === 13) { // 13 is the keycode for Enter
                event.preventDefault();
                $('#cmdSearch').click();
            }
        });
        $('#txtAttorneyFirstName').keypress(function (event) {
            if (event.which === 13) { // 13 is the keycode for Enter
                event.preventDefault();
                $('#cmdSearch').click();
            }
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            lastName = $("#txtAttorneyLastName").val();
            firstName = $("#txtAttorneyFirstName").val();
            firm = $("#txtAttorneyFirm").val();
            attorneyTable.draw();
        });
        $(".radio-button-list input").addClass("form-check-input");
        $(".radio-button-list label").addClass("form-check-label");
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
        //attorney search
        $(".attorney-search").on("click", function (e) {
            e.preventDefault();
            attorneyRole = $(this).data("attorney");
            var attyModal = document.querySelector('#attorneyModal');
            var modal = bootstrap.Modal.getInstance(attyModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('attorneyModal'));
            }
            modal.show();

        });
        $(".attorney-add").on("click", function (e) {
            e.preventDefault();
            var attyAddModal = document.querySelector('#EditAttorneyModal');
            var modal = bootstrap.Modal.getInstance(attyAddModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('EditAttorneyModal'));
            }
            modal.show();

        });
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
        $("#cmdMediatorSearch").on("click", function (e) {
            e.preventDefault();
            lastNameMed = $("#txtMediatorLastName").val();
            firstNameMed = $("#txtMediatorFirstName").val();
            mediatorTable.draw();
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
        $(".mediator-add").on("click", function (e) {
            e.preventDefault();
            var medAddModal = document.querySelector('#EditMediatorModal');
            var modal = bootstrap.Modal.getInstance(medAddModal);
            if (!modal) {
                modal = new bootstrap.Modal(document.getElementById('EditMediatorModal'));
            }
            modal.show();
        });
        $("#cmdAddNewEvent").on("click", function (e) {
            $("#EventModalLabel").text("Add New Event");
        });
    }

    function UpdateEventHeader(e) {
        var test = $("#EventModalLabel").html();
        $("#EventModalLabel").html("Edit Event")
    }

    /* Attorney Functions*/
    function SetAttorney(e, item) {
        e.preventDefault();
        ClearAttorney(attorneyRole);
        var attyid = item.dataset.id;
        var name = item.dataset.name;
        var email = item.dataset.email;
        var phone = item.dataset.phone;
        var ext = item.dataset.extension;
        if (attorneyRole == 1) {
            if (attyid && attyid != "undefined")
                $("#hdPlaintiffAttorneyId").val(attyid);
            if (name != "null" && name != "undefined")
                $("#txtPlaintiffName").val(name);
            if (email != "null" && email != "undefined")
                $("#txtPlaintiffEmail").val(email);
            if (phone != "null" && phone != "undefined")
                $("#txtPlaintiffPhone").val(phone);
            if (ext != "null" && ext != "undefined")
                $("#txtPlaintiffExtension").val(ext);
        }
        else if (attorneyRole == 2) {
            if (attyid && attyid != "undefined")
                $("#hdDefendantAttorneyId").val(attyid);
            if (name != "null" && name != "undefined")
                $("#txtDefendantName").val(name);
            if (email != "null" && email != "undefined")
                $("#txtDefendantEmail").val(email);
            if (phone != "null" && phone != "undefined")
                $("#txtDefendantPhone").val(phone);
            if (ext != "null" && ext != "undefined")
                $("#txtDefendantExtension").val(ext);
        }
        var attyModal = document.querySelector('#attorneyModal');
        var modal = bootstrap.Modal.getInstance(attyModal);
        modal.hide();
    }
    function AddAttorney(e) {
        e.preventDefault();
        service.baseUrl = service.framework.getServiceRoot(service.path) + "AttorneyListItem/";
        var action = "CreateAttorney";
        var attyFirstName = $("#txtFirstNameAdd").val();
        var attyLastName = $("#txtLastNameAdd").val();
        var email = $("#txtEmail").val();
        var attyFirm = $("#txtFirm").val();
        var phone = $("#txtPhone").val();
        var extension = $("#txtExtension").val();
        var city = $("#txtCity").val();
        var address = $("#txtAddress").val();
        var state = $("#drpState").val();
        var zip = $("#txtZip").val();
        var attroney = { firstname: attyFirstName, lastname: attyLastName, email: email, firm: attyFirm, phone: phone, extension: extension, city: city, address: address, state: state, zip: zip };
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: service.baseUrl + action,
                beforeSend: service.framework.setModuleHeaders,
                data: (attroney),
                success: function (result) {
                    ClearAddAttorneyForm();
                    var attyAddModal = document.querySelector('#EditAttorneyModal');
                    var modal = bootstrap.Modal.getInstance(attyAddModal);
                    if (!modal) {
                        modal = new bootstrap.Modal(document.getElementById('EditAttorneyModal'));
                    }
                    modal.hide();
                    attorneyTable.ajax.reload();
                    $("#txtAttorneyLastName").val(attyLastName);
                    $("#txtAttorneyFirstName").val(attyFirstName);
                    lastName = attyLastName;
                    firstName = attyFirstName;
                    attorneyTable.draw();
                },
                error: function (xhr, status, error) {
                    // alert(xhr.responseText);
                    alert("Unable to add attorney.\n\nMake sure you are logged in and try again. \n\nError:" + error);
                }
            });
        } catch (e) {
            alert("Unable to add attorney.\n\nMake sure you are logged in and try again.");
        }
        return false;
    }
    function ClearAttorney(attyRole) {
        if (attyRole == 1) {
            $("#hdPlaintiffAttorneyId").val("");
            $("#txtPlaintiffName").val("");
            $("#txtPlaintiffEmail").val("");
            $("#txtPlaintiffPhone").val("");
            $("#txtPlaintiffExtension").val("");
        } else if (attyRole == 2) {
            $("#hdDefendantAttorneyId").val("");
            $("#txtDefendantName").val("");
            $("#txtDefendantEmail").val("");
            $("#txtDefendantPhone").val("");
            $("#txtDefendantExtension").val("");
        }
    }
    function ClearAddAttorneyForm() {
        $("#txtFirstNameAdd").val("");
        $("#txtLastNameAdd").val("");
        $("#txtEmail").val("");
        $("#txtFirm").val("");
        $("#txtPhone").val("");
        $("#txtExtension").val("");
        $("#txtCity").val("");
        $("#txtAddress").val("");
        $("#drpState").val("");
        $("#txtZip").val("");
    }
    function ValidateAttorneyRemoval(attyRole) {
        $.dnnConfirm({
            text: 'Are you sure you wish to remove this Attorney?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Remove Attorney?',
            callbackTrue: function () {
                ClearAttorney(attyRole);
            }
        });
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
        $("#chkSubmittedToParties").prop("checked", false);
        $("#chkAgreementSigned").prop("checked", false);
        $("#chkPreparedAttorney").prop("checked", false);
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
    function ValidateAttorneyRemoval(attyRole) {
        $.dnnConfirm({
            text: 'Are you sure you wish to remove this Attorney?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Remove Attorney?',
            callbackTrue: function () {
                ClearAttorney(attyRole);
            }
        });
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
    function CloseAttorneyModal(e) {
        e.preventDefault();
        let modal = bootstrap.Modal.getInstance(document.getElementById("attorneyModal"));
        modal.hide();
    }
    function CloseEventModal(e) {
        ClearEventForm();
        e.preventDefault();
        let modal = bootstrap.Modal.getInstance(document.getElementById("EventModal"));
        modal.hide();
    }
    /* Utility Functions*/
    function hideReason(checkbox) {
        if (checkbox.checked == 1) {
            $(".reason").hide();
            $("select.reason").val('');
        } else {
            $(".reason").show();
        }
    }
</script>
