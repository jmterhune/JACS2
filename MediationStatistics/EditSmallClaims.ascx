<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditSmallClaims.ascx.cs" Inherits="tjc.Modules.MediationStatistics.EditSmallClaims" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltHeading" runat="server"><h4>{0}:&nbsp{1}</h4></asp:Literal>
<div id="county-form">
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
                    <asp:TextBox ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase case-code-field" placeholder="CC" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="6" CssClass="form-control upperCase" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtSuffix" title="Suffix" runat="server" MaxLength="4" CssClass="form-control upperCase case-code-field"  ClientIDMode="Static"></asp:TextBox>
                    <div class="input-group-append">
                        <small class="input-group-text form-control rounded-end" title="Year - Case Type - Case Sequence - Suffix">(Format: YYYY-CC-000000-NC)</small>
                    </div>
                </div>
            </div>
            <fieldset class="outline-fieldset mb-0">
                <legend>Plaintiff</legend>
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
                        <div class="col-12">
                            <asp:Label runat="server" AssociatedControlID="txtBusinessName" Text="Business Name" />
                            <asp:TextBox runat="server" ID="txtBusinessName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
        <div class="col-6">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="drpCountyLetter" ToolTip="Citizen's Dispute Settlement Program Case Number" Text="CDSP Number" />
                <div class="input-group">
                    <asp:DropDownList runat="server" CssClass="form-control" ID="drpCDSPType" ClientIDMode="Static">
                        <asp:ListItem Text="< Select Type >" Value="" />
                        <asp:ListItem Text="CDSP" title="Citizen's Dispute Settlement Program" />
                        <asp:ListItem Text="CDSPF" title="Citizen's Dispute Settlement Program Family" />
                    </asp:DropDownList>
                    <asp:TextBox ID="txtCDSPYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtCDSPNumber" title="Case Type" runat="server" MaxLength="3" CssClass="form-control upperCase" placeholder="000" ClientIDMode="Static"></asp:TextBox>
                    <asp:DropDownList ID="drpCountyLetter" runat="server" ToolTip="County" CssClass="form-control location-field" ClientIDMode="Static">
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
                <legend>Defendant</legend>
                <div class="form-group">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                            <asp:TextBox runat="server" ID="txtLastName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                            <asp:TextBox runat="server" ID="txtFirstName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                        </div>
                        <div class="col-12">
                            <asp:Label runat="server" AssociatedControlID="txtBusinessName" Text="Business Name" />
                            <asp:TextBox runat="server" ID="txtBusinessName_p2" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
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
                <asp:HiddenField ID="hdPetitionerAttorneyId" runat="server" ClientIDMode="Static" />
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch mt-4">
                                <asp:CheckBox ID="chkProSePetitioner" runat="server" Text="Pro Se" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPetitionerFirstName" Text="First Name" />
                            <asp:TextBox runat="server" ID="txtPetitionerFirstName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />

                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPetitionerLastName" Text="Last Name" />
                            <asp:TextBox runat="server" ID="txtPetitionerLastName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPetitionerPhone" Text="Phone" />
                            <asp:TextBox runat="server" ID="txtPetitionerPhone" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtPetitionerExtension" Text="Extension" />
                            <asp:TextBox runat="server" ID="txtPetitionerExtension" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
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
                <asp:HiddenField ID="hdRespondentAttorneyId" runat="server" ClientIDMode="Static" />
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <div class="form-check form-switch mt-4">
                                <asp:CheckBox ID="chkProSeRespondent" runat="server" Text="Pro Se" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtRespondentFirstName" Text="First Name" />
                            <asp:TextBox runat="server" ID="txtRespondentFirstName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtRespondentLastName" Text="Last Name" />
                            <asp:TextBox runat="server" ID="txtRespondentLastName" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtRespondentPhone" Text="Phone" />
                            <asp:TextBox runat="server" ID="txtRespondentPhone" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtRespondentExtension" Text="Extension" />
                            <asp:TextBox runat="server" ID="txtRespondentExtension" ClientIDMode="Static" Enabled="false" CssClass="form-control" />
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
                                <asp:ListItem Text="< Select Case Type >" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtOrderReferral" Text="Order of Referral" />
                            <asp:TextBox runat="server" ID="txtOrderReferral" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="drpMediator" Text="Mediator" />
                            <asp:DropDownList ID="drpMediator" runat="server" ToolTip="Mediation" CssClass="form-control" ClientIDMode="Static">
                                <asp:ListItem Text="< Select Mediator >" Value=""></asp:ListItem>
                                <asp:ListItem Text="Contracted" Value="Contracted" />
                                <asp:ListItem Text="Staff" Value="Staff" />
                                <asp:ListItem Text="Volunteer" Value="Volunteer" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtMediationDate" Text="Mediation Date / Resolved" />
                            <asp:TextBox runat="server" ID="txtMediationDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="rblPtcOrdered" Text="Mediation Forum" />
                            <asp:RadioButtonList ID="rblPtcOrdered" CssClass="form-control radio-buttons" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                <asp:ListItem Text="None" Value="N" />
                                <asp:ListItem Text="PTC" Value="PTC" />
                                <asp:ListItem Text="Court Ordered" Value="Court Ordered" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="col-auto pt-4">
                        <div class="form-group pt-2">
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="chkTelephoneSession" runat="server" Text="Telephonic Session" />
                            </div>
                        </div>
                    </div>
                    <div class="col-auto pt-4">
                        <div class="form-group pt-2">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkArbitrationReferral" runat="server" Text="Arbitration Referral" />
                            </div>
                        </div>
                    </div>
                     <div class="col-auto pt-4">
                        <div class="form-group pt-2">
                            <div class="form-check form-switch ">
                                <asp:CheckBox ID="chkInterpreterRequested" runat="server" Text="Interpreter Requested" />
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
            <fieldset class="outline-fieldset" id="fsSecondaryIssues" runat="server">
                <legend>Secondary Issues</legend>
                <asp:CheckBoxList ID="clsSecondaryIssues" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList">
                </asp:CheckBoxList>
            </fieldset>
            <hr />
            <div class="btn-group mb-3" role="group" aria-label="Session Events Actions">
                <asp:LinkButton ID="cmdSaveSession" runat="server"
                    OnClick="cmdSave_Click" CssClass="btn btn-primary"><i class="fas fa-save"></i> Save</asp:LinkButton>

                <asp:LinkButton ID="cmdAddEvent" CssClass="btn btn-dark" runat="server" OnClick="cmdAddEvent_Click"><i class="fas fa-plus"></i> New Event</asp:LinkButton>
                <div class="input-group-text" id="eventInfo">
                    Session Events
                </div>
                <asp:HiddenField ID="hdSessionId" runat="server" />
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
                            </div>
                            <div class="row">
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkSubmittedToParties" <%#Convert.ToBoolean(Eval("AgreementSubmittedParties"))?"checked":""%>>
                                            <label class="form-check-label" for="chkSubmittedToParties">Submitted to Parties</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkAgreementSigned" <%#Convert.ToBoolean(Eval("AgreementSigned"))?"checked":""%>>
                                            <label class="form-check-label" for="chkAgreementSigned">Parties Signed Agreement</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkPreparedAttorney" <%#Convert.ToBoolean(Eval("AgreementPreparedAttorney"))?"checked":""%>>
                                            <label class="form-check-label" for="chkPreparedAttorney">Prepared by Attorney</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-group">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="chkAdjournedTimeRemaining" <%#Convert.ToBoolean(Eval("AdjournedTimeRemaining"))?"checked":""%>>
                                            <label class="form-check-label me-3" for="chkPreparedAttorney">Adjourned with Time Remaining</label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <fieldset class="outline-fieldset">
                                <legend class="small">Appearance Record</legend>
                                <%#GetAppearanceItems(Eval("EventID").ToString())%>
                            </fieldset>
                        </fieldset>
                        <p class="mb-0">
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
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkSubmittedToParties" Text="Submitted to Parties" runat="server" Checked='<%#Bind("AgreementSubmittedParties")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkAgreementSigned" Text="Parties Signed Agreement" runat="server" Checked='<%#Bind("AgreementSigned")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkPreparedAttorney" Text="Prepared by Attorney" runat="server" Checked='<%#Bind("AgreementPreparedAttorney")%>' />
                                    </div>
                                </div>
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
                        <fieldset class="outline-fieldset">
                            <legend class="small">Appearance Record</legend>
                            <asp:CheckBoxList ID="cblAppearanceRecord" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList" DataTextField="Description" DataValueField="AppearanceId">
                            </asp:CheckBoxList>
                        </fieldset>
                        <p>
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
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkSubmittedToParties" Text="Submitted to Parties" runat="server" Checked='<%#Bind("AgreementSubmittedParties")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkAgreementSigned" Text="Parties Signed Agreement" runat="server" Checked='<%#Bind("AgreementSigned")%>' />
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="form-group">
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkPreparedAttorney" Text="Prepared by Attorney" runat="server" Checked='<%#Bind("AgreementPreparedAttorney")%>' />
                                    </div>
                                </div>
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
                        <fieldset class="outline-fieldset">
                            <legend class="small">Appearance Record</legend>
                            <asp:CheckBoxList ID="cblAppearanceRecord" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList" DataTextField="Description" DataValueField="AppearanceId">
                            </asp:CheckBoxList>
                        </fieldset>
                        <p>
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
            <div class="modal fade" id="attorneyModal" tabindex="-1" role="dialog" aria-labelledby="attorneyModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="attorneyModalLabel">Attorney Search</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        </div>
                        <div class="modal-body form-group">
                            <div class="row mb-3">
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyFirstName" Text="First Name" />
                                    <asp:TextBox runat="server" ID="txtAttorneyFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyLastName" Text="Last Name" />
                                    <asp:TextBox runat="server" ID="txtAttorneyLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtAttorneyFirm" Text="Firm" />
                                    <asp:TextBox runat="server" ID="txtAttorneyFirm" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                </div>
                                <div class="col-auto pt-4">
                                    <button type="button" class="btn btn-primary" id="cmdSearch">Search</button>
                                </div>
                            </div>
                            <table id="tblAttorneys" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Firm</th>
                                        <th>Phone</th>
                                        <th>Extension</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
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
    var lastName = null;
    var firstName = null;
    var firm = null;
    var pageSize = 10;
    var rowOffset = 0;
    var recordCount = 0;
    var sortDirection = "asc";
    var sortColumnIndex = 1;
    var attorneyRole = 0;
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                PageInit();
            });
        });

    }(jQuery, window.Sys));
    function PageInit() {
        var restUrl = `/DesktopModules/tjc.Modules/Mediation/api/AttorneyListItem/GetAttorneyListItems/${recordCount}`;
        var attorneyTable = $('#tblAttorneys').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: restUrl,
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
                    return `<a title="Select Attorney" data-id="${row.attorneyid}" data-first="${row.firstname}" data-last="${row.lastname}" data-phone="${row.phone}" data-extension="${row.extension}" onclick="SetAttorney(event,this)" href="#"><i class="fas fa-user-tie"></i></a>`;
                }, className: "command-item", orderable: false
            },
            { data: "firstname" },
            { data: "lastname" },
            { data: "firm" },
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
        $.fn.dataTable.ext.errMode = () => function (settings, helpPage, message) {
            alert("The Following Error Occurred Loading Attorney List:" + message);
        };
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            lastName = $("#txtAttorneyLastName").val();
            firstName = $("#txtAttorneyFirstName").val();
            firm = $("#txtAttorneyFirm").val();
            attorneyTable.draw();
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
    }

    function hideReason(checkbox) {
        if (checkbox.checked == 1) {
            $(".reason").hide();
            $("select.reason").val('');
        } else {
            $(".reason").show();
        }
    }
    function ClearAttorney(attyRole) {
        if (attyRole == 1) {
            $("#hdPetitionerAttorneyId").val("");
            $("#txtPetitionerFirstName").val("");
            $("#txtPetitionerLastName").val("");
            $("#txtPetitionerPhone").val("");
            $("#txtPetitionerExtension").val("");
        } else if (attyRole == 2) {
            $("#hdRespondentAttorneyId").val("");
            $("#txtRespondentFirstName").val("");
            $("#txtRespondentLastName").val("");
            $("#txtRespondentPhone").val("");
            $("#txtRespondentExtension").val("");
        }
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
    function SetAttorney(e, item) {
        e.preventDefault();
        ClearAttorney(attorneyRole);
        var attyid = item.dataset.id;
        var first = item.dataset.first;
        var last = item.dataset.last;
        var phone = item.dataset.phone;
        var ext = item.dataset.extension;
        if (attorneyRole == 1) {
            if (attyid && attyid != "undefined")
                $("#hdPetitionerAttorneyId").val(attyid);
            if (first != "null" && first != "undefined")
                $("#txtPetitionerFirstName").val(first);
            if (last != "null" && last != "undefined")
                $("#txtPetitionerLastName").val(last);
            if (phone != "null" && phone != "undefined")
                $("#txtPetitionerPhone").val(phone);
            if (ext != "null" && ext != "undefined")
                $("#txtPetitionerExtension").val(ext);
        }
        else if (attorneyRole == 2) {
            if (attyid && attyid != "undefined")
                $("#hdRespondentAttorneyId").val(attyid);
            if (first != "null" && first != "undefined")
                $("#txtRespondentFirstName").val(first);
            if (last != "null" && last != "undefined")
                $("#txtRespondentLastName").val(last);
            if (phone != "null" && phone != "undefined")
                $("#txtRespondentPhone").val(phone);
            if (ext != "null" && ext != "undefined")
                $("#txtRespondentExtension").val(ext);
        }
        var attyModal = document.querySelector('#attorneyModal');
        var modal = bootstrap.Modal.getInstance(attyModal);
        modal.hide();
    }

</script>
