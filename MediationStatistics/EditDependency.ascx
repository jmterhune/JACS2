<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDependency.ascx.cs" Inherits="tjc.Modules.MediationStatistics.EditDependency" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltHeading" runat="server"><h4>{0}:&nbsp{1}</h4></asp:Literal>
<div id="dependency-form">
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
        </div>
    </div>
    <fieldset class="outline-fieldset">
        <legend>In the Interest of</legend>
        <div class="form-group">
            <div class="row">
                <div class="col-4">
                    <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                    <asp:TextBox runat="server" ID="txtLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                </div>
                <div class="col-4">
                    <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                    <asp:TextBox runat="server" ID="txtFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                </div>
               
            </div>
        </div>
    </fieldset>
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
                <div class="col-3">
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
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtReferralSource" Text="Program Referral Source" />
                        <div class="combo-container">
                            <asp:TextBox runat="server" ID="txtReferralSource" MaxLength="50" ClientIDMode="Static" CssClass="combo form-control" autocomplete="off" />
                            <datalist id="dlReferralSource" class="form-control combo-list">
                                <asp:Literal ID="ltReferralSourceOptions" runat="server" />
                            </datalist>
                        </div>
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtOrderReferralDate" Text="Order of Referral" />
                        <asp:TextBox runat="server" ID="txtOrderReferralDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpActionStage" Text="Stage of Action" />
                        <asp:DropDownList ID="drpActionStage" runat="server" DataValueField="StageOfActionId" AppendDataBoundItems="true" DataTextField="Description" ToolTip="Stage of Action" CssClass="form-control" ClientIDMode="Static">
                            <asp:ListItem Text="< Select Stage of Action >" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtChildrenInvolved" Text="Children Involved" />
                        <asp:TextBox runat="server" ID="txtChildrenInvolved" MaxLength="2" TextMode="Number" min="0" max="99" ClientIDMode="Static" CssClass="form-control" />
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtParentsInvolved" Text="Parents Involved" />
                        <asp:TextBox runat="server" ID="txtParentsInvolved" MaxLength="2" TextMode="Number" min="0" max="99" ClientIDMode="Static" CssClass="form-control" />
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-auto">
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="chkInterpreterRequested" runat="server" Text="Interpreter Requested" />
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="chkTelephoneSession" runat="server" Text="Telephonic Session" />
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="chkInmate" runat="server" Text="Inmate" />
                    </div>
                </div>
            </div>
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
                            <%#GetAppearanceItems(Eval("EventID").ToString())%>
                            <fieldset class="outline-fieldset">
                                <legend class="small">Agreement Signed</legend>
                                <div class="row">
                                    <div class="col-3">
                                        <div class="form-check form-switch mt-3">
                                            <input class="form-check-input" type="checkbox" id="chkSignedMediation" <%#Convert.ToBoolean(Eval("Signed1"))?"checked":""%>>
                                            <label class="form-check-label" for="chkSignedMediation">At Mediation?</label>
                                        </div>
                                    </div>
                                    <div class="col-1">
                                        <asp:TextBox runat="server" ID="txtSignedMediationCount" ToolTip="Signed At Mediation Count" Enabled="false" CssClass="form-control" Text='<%#Eval("SignedCount1")%>' />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-3">
                                        <div class="form-check form-switch mt-3">
                                            <input class="form-check-input" type="checkbox" id="chkSignedAfterMediation" <%#Convert.ToBoolean(Eval("Signed2"))?"checked":""%>>
                                            <label class="form-check-label" for="chkSignedAfterMediation">After Mediation Before Trial?</label>
                                        </div>
                                    </div>
                                    <div class="col-1">
                                        <asp:TextBox runat="server" ID="txtSignedAfterMediationCount" ToolTip="Signed After Mediation or Before Trial Count" Enabled="false" CssClass="form-control" Text='<%#Eval("SignedCount2")%>' />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-3">
                                        <div class="form-check form-switch mt-3">
                                            <input class="form-check-input" type="checkbox" id="chkSignedTrial" <%#Convert.ToBoolean(Eval("Signed3"))?"checked":""%>>
                                            <label class="form-check-label" for="chkSignedTrial">At Trial?</label>
                                        </div>
                                    </div>
                                    <div class="col-1">
                                        <asp:TextBox runat="server" ID="txtSignedTrialCount" ToolTip="Signed At Trial Count" Enabled="false" CssClass="form-control" Text='<%#Eval("SignedCount3")%>' />
                                    </div>
                                </div>
                            </fieldset>
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
                                    <asp:RadioButtonList ID="rblAgreementType" CssClass="form-control radio-button-list" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
                                        SelectedValue='<%#Bind("AgreementType")%>'>
                                        <asp:ListItem Text="None" Value="N" Selected="True" />
                                        <asp:ListItem Text="Full" Value="F" />
                                        <asp:ListItem Text="Partial/Temporary" Value="C" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                        <fieldset class="outline-fieldset">
                            <legend class="small">Appearance Record</legend>
                            <asp:CheckBoxList ID="cblAppearanceRecord" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList" DataTextField="Description" DataValueField="AppearanceId">
                            </asp:CheckBoxList>
                        </fieldset>
                        <fieldset class="outline-fieldset">
                            <legend class="small">Agreement Signed</legend>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-3">
                                        <asp:CheckBox ID="chkSignedMediation" runat="server" Text="At Mediation?" Checked='<%#Bind("Signed1")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedMediationCount" ToolTip="Signed At Mediation Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount1")%>' />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-3">
                                        <asp:CheckBox ID="chkSignedAfterMediation" runat="server" Text="After Mediation Before Trial?" Checked='<%#Bind("Signed2")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedAfterMediationCount" ToolTip="Signed After Mediation or Before Trial Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount2")%>' />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-3">
                                        <asp:CheckBox ID="chkSignedTrial" runat="server" Text="At Trial?" Checked='<%#Bind("Signed3")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedTrialCount" ToolTip="Signed At Trial Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount3")%>' />
                                </div>
                            </div>
                        </fieldset>
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
                        <fieldset class="outline-fieldset">
                            <legend class="small">Appearance Record</legend>
                            <asp:CheckBoxList ID="cblAppearanceRecord" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch" RepeatLayout="UnorderedList" DataTextField="Description" DataValueField="AppearanceId">
                            </asp:CheckBoxList>
                        </fieldset>
                        <fieldset class="outline-fieldset">
                            <legend class="small">Agreement Signed</legend>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkSignedMediation" runat="server" Text="At Mediation?" Checked='<%#Bind("Signed1")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedMediationCount" ToolTip="Signed At Mediation Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount1")%>' />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkSignedAfterMediation" runat="server" Text="After Mediation Before Trial?" Checked='<%#Bind("Signed2")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedAfterMediationCount" ToolTip="Signed After Mediation or Before Trial Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount2")%>' />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-3">
                                    <div class="form-check form-switch mt-4">
                                        <asp:CheckBox ID="chkSignedTrial" runat="server" Text="At Trial?" Checked='<%#Bind("Signed3")%>' />
                                    </div>
                                </div>
                                <div class="col-1">
                                    <asp:TextBox runat="server" ID="txtSignedTrialCount" ToolTip="Signed At Trial Count" TextMode="Number" min="0" max="9" MaxLength="15" ClientIDMode="Static" CssClass="form-control" Text='<%#Bind("SignedCount3")%>' />
                                </div>
                            </div>
                        </fieldset>
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

<script type="text/javascript">

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
        const dataList = document.getElementById('dlReferralSource');
        const input = document.getElementById('txtReferralSource');

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
    }

    function hideReason(checkbox) {
        if (checkbox.checked == 1) {
            $(".reason").hide();
            $("select.reason").val('');
        } else {
            $(".reason").show();
        }
    }
</script>
