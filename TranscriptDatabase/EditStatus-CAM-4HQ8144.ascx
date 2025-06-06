<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditStatus.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.EditStatus" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CalendartUrl%>">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormListUrl%>">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=HearingListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <asp:UpdatePanel ID="pnlStatus" runat="server" RenderMode="Block" UpdateMode="Always" OnUnload="pnlStatus_Unload">
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
                <div id="designationStatus" class="tab-pane active">
                    <div class="btn-group mb-2" role="group" aria-label="Basic example">
                        <button id="cmdAddEvent" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#AddEventModal"><i class="fas fa-circle-plus me-1"></i>Add Event</button>
                        <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-primary"><i class="fas fa-pencil me-1"></i> Edit Designation</asp:HyperLink>
                        <button id="cmdUploadFile" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#UploadModal"><i class="fas fa-upload me-1"></i>Upload File</button>
                        <button id="cmdAcknowledgements" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#FileSelectionModal"><i class="fas fa-envelope-circle-check me-1"></i>Acknowledgements</button>
                        <button id="cmdExtension" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#FileSelectionModal"><i class="fas fa-calendar-plus me-1"></i>Extensions</button>
                        <button id="cmdDueDate" type="button" class="btn btn-primary"><i class="fas fa-hourglass-end me-1"></i>Edit Due Date</button>
                        <button id="cmdFiled" type="button" class="btn btn-primary"><i class="fas fa-folder-open me-1"></i>Transcript Filed</button>
                    </div>
                    <asp:Literal ID="ltPageMessage" runat="server" />
                    <div id="updateDueDate" class="alert alert-warning form-group date-panel" style="display: none">
                        <label for="txtDueDate">Enter Due Date<em>*</em></label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtDueDateUpdate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="duedate" CssClass="label label-danger"
                            ErrorMessage="Due Date Is Required" ControlToValidate="txtDueDateUpdate" runat="server" />
                        <asp:CustomValidator ID="valDueDateIsDate" runat="server" Display="Dynamic" SetFocusOnError="true" ValidationGroup="duedate" CssClass="label label-danger"
                            ErrorMessage="Invalid Date" ControlToValidate="txtDueDateUpdate" ClientValidationFunction="ValidateDateType">
                        </asp:CustomValidator>
                        <hr />
                        <div class="d-flex justify-content-between">
                            <asp:Button ID="cmdUpdateDueDate" CssClass="btn btn-primary hide-panels" ValidationGroup="duedate" Text="Update" runat="server" OnClick="cmdUpdateDueDate_Click" />
                            <button id="cmdCancelUpdate" type="button" class="btn btn-secondary hide-panels">Cancel</button>
                        </div>

                    </div>
                    <div id="updateTrascriptFiled" class="alert alert-warning form-group date-panel" style="display: none">
                        <label for="txtTranscriptFiledUpdate">Enter File Date<em>*</em></label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtTranscriptFiledUpdate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="transcriptfiled" CssClass="label label-danger"
                            ErrorMessage="File Date Is Required" ControlToValidate="txtTranscriptFiledUpdate" runat="server" />
                        <asp:CustomValidator ID="valTranscriptFiledIsDate" runat="server" Display="Dynamic" SetFocusOnError="true" ValidationGroup="transcriptfiled" CssClass="label label-danger"
                            ErrorMessage="Invalid Date" ControlToValidate="txtTranscriptFiledUpdate" ClientValidationFunction="ValidateDateType">
                        </asp:CustomValidator>
                        <hr />
                        <div class="d-flex justify-content-between">

                            <asp:Button ID="cmdUpdateTranscriptFiled" CssClass="btn btn-primary hide-panels" ValidationGroup="transcriptfiled" Text="Update" runat="server" OnClick="cmdUpdateTranscriptFiled_Click" />
                            <button id="cmdCancelTranscriptFiled" type="button" class="btn btn-secondary hide-panels">Cancel</button>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="mb-1 row">
                                <label for="txtDefendantName" class="col-sm-4 col-form-label text-end">Defendant Name:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDefendantName" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtTribunalCase" class="col-sm-4 col-form-label text-end">Tribunal Case #:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtTribunalCase" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtAppellateCase" class="col-sm-4 col-form-label text-end">Appellate Case #:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAppellateCase" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtCounty" class="col-sm-4 col-form-label text-end">County:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtCounty" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="mb-1 row">
                                <label for="txtServiceDate" class="col-sm-4 col-form-label text-end">Service Date:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtServiceDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtReceiptDate" class="col-sm-4 col-form-label text-end">Receipt Date:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtReceiptDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtDueDate" class="col-sm-4 col-form-label text-end">Due Date:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDueDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtTranscriptFiledDate" class="col-sm-4 col-form-label text-end">Transcript Filed:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtTranscriptFiledDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="mb-1 row">
                                <label for="txtHearingDates" class="col-sm-4 col-form-label text-end">Hearing Dates:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtHearingDates" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtDays" class="col-sm-4 col-form-label text-end">Days:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDays" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtEstimatedPages" class="col-sm-4 col-form-label text-end">Estimated Pages:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtEstimatedPages" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="mb-1 row">
                                <label for="txtCreatedBy" class="col-sm-4 col-form-label text-end">Created By:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtCreatedBy" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-1 row">
                            <label for="txtAttorneys" class="col-auto col-form-label">Attorney(s):</label>
                            <div class="col-auto">
                                <asp:TextBox ID="txtAttorneys" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="row radio-button-list mb-3">
                        <div class="col-auto">
                            <asp:CheckBox ID="chkAcknowledgementFiled" ClientIDMode="Static" AutoPostBack="true" OnCheckedChanged="chkAcknowledgementFiled_CheckedChanged" Text="Acknowledgment Filed/No Acknowledgment Needed" runat="server" />
                        </div>
                        <div class="col-auto">
                            <asp:CheckBox ID="chkPublicDefender" Enabled="false" ClientIDMode="Static" runat="server" Text="Public Defender/Special Appointed Public Defender" />
                        </div>
                        <div class="col-auto">
                            <asp:CheckBox ID="chkIndigent" Enabled="false" ClientIDMode="Static" Text="Declared Indigent" runat="server" />
                        </div>
                        <div class="col-auto">
                            <asp:CheckBox ID="chkCourtAppointed" Enabled="false" ClientIDMode="Static" Text="Court Appointed Attorney" runat="server" />
                        </div>
                    </div>
                    <div class="accordion" id="accordionSection">
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="extensionHeading">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#extensionSection" aria-expanded="false" aria-controls="extensionSection">
                                    Extension Requests
                                </button>
                            </h2>
                            <div id="extensionSection" class="accordion-collapse collapse" aria-labelledby="extensionHeading" data-bs-parent="#accordionSection">
                                <div class="accordion-body">
                                    <asp:Literal ID="ltExtensionMessage" runat="server" />
                                    <asp:Repeater runat="server" ID="rptExtensions" OnItemCommand="rptExtensions_ItemCommand" OnItemCreated="rptExtensions_ItemCreated">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <thead>
                                                    <tr>
                                                        <th>Extension Type</th>
                                                        <th>Submitted</th>
                                                        <th>Requested</th>
                                                        <th>Granted</th>
                                                        <th>&nbsp;</th>
                                                        <th>&nbsp;</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%#Eval("EventTypeName") %></td>
                                                <td><%#Eval("SubmittedDateFormatted") %></td>
                                                <td><%#Eval("RequestedDateFormatted") %></td>
                                                <td>
                                                    <asp:TextBox ID="txtNewDate" runat="server" MaxLength="10" Width="80" Text='<%# Eval("GrantedDateFormatted")%>'
                                                        Visible='<%#!bool.Parse(Eval("Approved").ToString())%>' />
                                                    <%# bool.Parse(Eval("Approved").ToString()) ? Eval("GrantedDateFormatted") : ""%></td>
                                                <td class="command-item">
                                                    <asp:LinkButton ID="cmdApprove" runat="server" Visible='<%#!bool.Parse(Eval("Approved").ToString())%>' ToolTip="Approve Extension" CommandName="approve" CommandArgument='<%# Eval("ExtensionID")%>'><i class="fas fa-circle-check"></i> </asp:LinkButton>
                                                </td>
                                                <td class="command-item">
                                                    <asp:LinkButton ID="cmdDelete" CssClass="delete-extension" runat="server" Visible='<%#!bool.Parse(Eval("Approved").ToString())%>' ToolTip="Delete Extension" CommandName="delete" CommandArgument='<%# Eval("ExtensionID")%>'><i class="fa fa-trash"></i> </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody></table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="filesHeading">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#fileSection" aria-expanded="false" aria-controls="fileSection">
                                    Associated Files
                                </button>
                            </h2>
                            <div id="fileSection" class="accordion-collapse collapse" aria-labelledby="filesHeading" data-bs-parent="#accordionSection">
                                <div class="accordion-body">
                                    <asp:Repeater runat="server" ID="rptAttachments" OnItemCommand="rptAttachments_ItemCommand" OnItemCreated="rptAttachments_ItemCreated">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <thead style="display: none">
                                                    <tr>
                                                        <th>&nbsp;</th>
                                                        <th>File Link</th>
                                                        <th>&nbsp;</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="command-item">
                                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CommandArgument='<%# Eval("AttachmentID")%>'><i class="fa fa-pencil"></i> </asp:LinkButton>
                                                </td>
                                                <td>
                                                    <asp:HyperLink ID="lnkFile" runat="server" Target="_blank" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "RelativePath")%>'><%# DataBinder.Eval(Container.DataItem, "FileDescription")%></asp:HyperLink></td>
                                                <td class="command-item">
                                                    <asp:LinkButton ID="cmdDelete" CssClass="delete-file" runat="server" ToolTip="Delete File" CommandName="delete" CommandArgument='<%# Eval("AttachmentID")%>'><i class="fa fa-trash"></i> </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody></table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="commentsHeading">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#commentSection" aria-expanded="false" aria-controls="commentSection">
                                    Comments
                                </button>
                            </h2>
                            <div id="commentSection" class="accordion-collapse collapse" aria-labelledby="commentsHeading" data-bs-parent="#accordionSection">
                                <div class="accordion-body">
                                    <div class="form-group row mb-2">
                                        <div class="col-12">
                                            <asp:TextBox runat="server" ID="txtComments" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                                        </div>
                                    </div>
                                    <asp:Button ID="cmdSaveComment" CssClass="btn btn-default" Text="Save Comments" runat="server" OnClick="cmdSaveComment_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div id="events" class="mb-3">
                        <asp:Repeater runat="server" ID="rptEvent" OnItemDataBound="rptEvent_ItemDataBound" OnItemCommand="rptEvent_ItemCommand" OnItemCreated="rptEvent_ItemCreated">
                            <ItemTemplate>
                                <section class="status p-3 border rounded mt-2">
                                    <h4 class="text-danger">Event&nbsp;<%# Container.ItemIndex + 1%></h4>
                                    <table class="table">
                                        <thead>
                                            <tr>
                                                <th>Hearing Date
                                                </th>
                                                <th>Presiding Judge
                                                </th>
                                                <th>Hearing Type
                                                </th>
                                                <th>Court Reporter
                                                </th>
                                                <th>Estimated Page #
                                                </th>
                                                <th>Days Until Completion
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "HearingDateFormatted")%>
                                                </td>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "PresidingJudgeName")%>
                                                </td>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "HearingType")%>
                                                </td>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "CourtReporterName")%>
                                                </td>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "Pages")%>
                                                </td>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "DaysUntilComplete")%>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th>&nbsp;
                                                </th>
                                                <th>Name
                                                </th>
                                                <th>Begin
                                                </th>
                                                <th class="text-center">Pages
                                                </th>
                                                <th class="text-center">Completed
                                                </th>
                                                <th class="text-center">Pages
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th class="col-auto text-end">To Scopist
                                                </th>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "ScopistName")%>
                                                </td>
                                                <td>
                                                    <%#  Eval("ScopSentFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("ScopPagesIn")%>
                                                </td>
                                                <td>
                                                    <%# Eval("ScopReturnedFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("ScopPagesOut")%>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">To Transcriptionist
                                                </th>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "TranscriptionistName")%>
                                                </td>
                                                <td>
                                                    <%# Eval("transSentFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("transPagesIn")%>
                                                </td>
                                                <td>
                                                    <%# Eval("transReturnedFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("transPagesOut")%>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Editing
                                                </th>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "EditorName")%>
                                                </td>
                                                <td>
                                                    <%# Eval("editSentFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("editPagesIn")%>
                                                </td>
                                                <td>
                                                    <%# Eval("editReturnedFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("editPagesOut")%>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Proofing
                                                </th>
                                                <td>
                                                    <%#DataBinder.Eval(Container.DataItem, "ProoferName")%>
                                                </td>
                                                <td>
                                                    <%# Eval("proofSentFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("proofPagesIn")%>
                                                </td>
                                                <td>
                                                    <%# Eval("proofReturnedFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("proofPagesOut")%>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Corrections, Indexing, Binding
                                                </th>
                                                <td>
                                                    <%# Eval("CompletedByName")%>
                                                </td>
                                                <td>&nbsp;
                                                </td>
                                                <td>&nbsp;
                                                </td>
                                                <td>
                                                    <%# Eval("completedFormatted")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("CompletedPages")%>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <asp:Literal ID="ltEventMessage" runat="server" />
                                    <asp:LinkButton CssClass="btn btn-primary" ID="cmdEdit" runat="server" CommandArgument='<%#Eval("EventID") %>'
                                        CommandName="edit"><i class="fas fa-pencil"></i> Edit Event</asp:LinkButton>
                                    <asp:LinkButton CssClass="btn btn-tertiary" ID="cmdComplete" Visible="true" runat="server" CommandArgument='<%#Eval("EventID") %>'
                                        CommandName="complete"><i class="fas fa-check"></i> Mark Complete</asp:LinkButton>
                                    <asp:LinkButton CssClass="btn btn-danger delete-event" ID="cmdDelete" Visible="true" runat="server" CommandArgument='<%#Eval("EventID") %>'
                                        CommandName="delete"><i class="fas fa-trash"></i> Delete Event</asp:LinkButton>

                                </section>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:HiddenField ID="hdDesignationId" runat="server" ClientIDMode="Static" />
                    </div>
                    <div class="modal fade" id="AddEventModal" tabindex="-1" role="dialog" aria-labelledby="AddEventModalLabel">
                        <div class="modal-dialog modal-dialog-centered modal-xl">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="AddEventModalLabel">Add / Edit Event</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row form-group">
                                        <div class="col-md-3">
                                            <label for="txtHearingDate">Hearing Date</label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtHearingDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                                        </div>
                                        <div class="col">
                                            <label for="selectedJudgeId">Presiding Judge</label>
                                            <div class="drpjudge">
                                                <asp:TextBox runat="server" ClientIDMode="Static" ID="judgeSearch" CssClass="form-control" placeholder="Type to search..." />
                                                <asp:HiddenField ID="selectedJudgeId" runat="server" ClientIDMode="Static" />
                                                <div id="drpJudge" class="list-group position-absolute w-100 combo-list"></div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <label for="drpHearingType">Hearing Type</label>
                                            <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpHearingType" CssClass="form-control">
                                                <asp:ListItem Text="< Select Hearing Type >" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <label for="selectedCourtReporterId">Court Reporter</label>
                                            <div class="drpcourtreporter">
                                                <asp:TextBox runat="server" ClientIDMode="Static" ID="reporterSearch" CssClass="form-control" placeholder="Type to search..." />
                                                <asp:HiddenField ID="selectedCourtReporterId" runat="server" ClientIDMode="Static" />
                                                <div id="drpCourtReporter" class="list-group position-absolute w-100 combo-list" style="display: none;"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-3">
                                            <label for="txtEstimagedPages">Estimated Pages</label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtEstimagedPages" ClientIDMode="Static" TextMode="Number" min="0" step="1" runat="server" CssClass="form-control" MaxLength="7"></asp:TextBox>
                                        </div>
                                        <div class="col-3">
                                            <label for="txtDaysUntilCompletion">Days Until Completion</label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtDaysUntilCompletion" ClientIDMode="Static" TextMode="Number" min="0" step="1" runat="server" CssClass="form-control" MaxLength="7"></asp:TextBox>
                                        </div>
                                    </div>
                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th>&nbsp;
                                                </th>
                                                <th class="col-auto text-center">Name
                                                </th>
                                                <th class="text-center">Begin
                                                </th>
                                                <th class="text-center">Pages
                                                </th>
                                                <th class="text-center">Completed
                                                </th>
                                                <th class="text-center">Pages
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th class="text-end">To Scopist
                                                </th>
                                                <td>
                                                    <div class="drpscopist">
                                                        <asp:TextBox runat="server" ClientIDMode="Static" ID="scopistSearch" CssClass="form-control" placeholder="Type to search..." />
                                                        <asp:HiddenField ID="selectedScopistId" runat="server" ClientIDMode="Static" />
                                                        <div id="drpScopist" class="list-group position-absolute" style="display: none;"></div>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtScopeSent" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtScopePagesIn" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtScopeReturned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtScopePagesOut" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">To Transcriptionist
                                                </th>
                                                <td>
                                                    <div class="drptranscriptionist">
                                                        <asp:TextBox runat="server" ClientIDMode="Static" ID="transcriptionistSearch" CssClass="form-control" placeholder="Type to search..." />
                                                        <asp:HiddenField ID="selectedTranscriptionistId" runat="server" ClientIDMode="Static" />
                                                        <div id="drpTranscriptionist" class="list-group position-absolute" style="display: none;"></div>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtTransSent" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtTransPagesIn" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtTransReturned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtTransPagesOut" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Editing
                                                </th>
                                                <td>
                                                    <div class="drpeditor">
                                                        <asp:TextBox runat="server" ClientIDMode="Static" ID="editorSearch" CssClass="form-control" placeholder="Type to search..." />
                                                        <asp:HiddenField ID="selectedEditorId" runat="server" ClientIDMode="Static" />
                                                        <div id="drpEditor" class="list-group position-absolute" style="display: none;"></div>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtEditSent" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtEditPagesIn" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtEditReturned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtEditPagesOut" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Proofing
                                                </th>
                                                <td>
                                                    <div class="drpproofer">
                                                        <asp:TextBox runat="server" ClientIDMode="Static" ID="prooferSearch" CssClass="form-control" placeholder="Type to search..." />
                                                        <asp:HiddenField ID="selectedProoferId" runat="server" ClientIDMode="Static" />
                                                        <div id="drpProofer" class="list-group position-absolute" style="display: none;"></div>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtProofSent" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtProofPagesIn" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtProofReturned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="20"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtProofPagesOut" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-end">Corrections, Indexing, Binding
                                                </th>
                                                <td>&nbsp;
                                                </td>
                                                <td>&nbsp;
                                                </td>
                                                <td>&nbsp;
                                                </td>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <asp:TextBox AutoCompleteType="Disabled" ID="txtCompletedPages" ToolTip="Completed Pages" ClientIDMode="Static" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button ID="cmdSaveEvent" OnClientClick="ToggleEventForm(false)" CssClass="btn btn-primary" Text="Save" runat="server" OnClick="cmdSaveEvent_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                                <asp:HiddenField ID="hdEventId" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="hdSequence" runat="server" ClientIDMode="Static" />
                            </div>
                        </div>
                    </div>
                    <div class="modal fade" id="UploadModal" tabindex="-1" role="dialog" aria-labelledby="UploadModalLabel">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="UploadModalLabel">Upload Files</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <label for="txtUploadeTitle">File Description<em>*</em></label>
                                        <asp:TextBox AutoCompleteType="Disabled" ID="txtUploadeTitle" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="upload" CssClass="label label-danger"
                                            ErrorMessage="Last Name Is Required" ControlToValidate="txtUploadeTitle" runat="server" />
                                    </div>
                                    <div>
                                        <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplFile" Text="Upload File<em>*</em>" />
                                        <div style="position: relative;">
                                            <div id="upload-overlay" class="overlay" style="display: none;">
                                                <div class="spinner"></div>
                                            </div>
                                            <asp:FileUpload ID="uplFile" runat="server" ToolTip="Select File to Upload" AllowMultiple="false" ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx" />
                                            <span id="fileUploadWarning" style="display: none" class="label label-danger upload-warning">Please Choose File to Upload</span>
                                            <asp:CustomValidator ID="valUpload" Display="Dynamic" ValidationGroup="upload" runat="server" CssClass="label label-danger" ClientValidationFunction="validateUpload"
                                                ErrorMessage="Please select a file" OnServerValidate="valUpload_ServerValidate"></asp:CustomValidator>
                                            <span id="uploadInfo"></span>
                                            <asp:HiddenField ID="hdAttachmentId" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="hdFileId" ClientIDMode="Static" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="return ToggleUploadForm(false);" CssClass="btn btn-primary" ID="cmdSaveFile" ValidationGroup="upload" runat="server" Text="Save" OnClick="cmdSaveFile_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal fade" id="FileSelectionModal" tabindex="-1" role="dialog" aria-labelledby="FileSelectionModalLabel">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="FileSelectionModalLabel">Create Acknowledgement</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <label for="txtReason">Reason</label>
                                        <asp:TextBox ID="txtReason" ClientIDMode="Static" runat="server" MaxLength="300" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-group">
                                                <label for="drpFormType">Form</label>
                                                <select id="drpFormType" class="form-control">
                                                </select>
                                                <asp:HiddenField ID="hdSelectedFormType" ClientIDMode="Static" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-6">
                                            <div class="form-group">
                                                <label for="txtSubmittedDate">Submitted Date</label>
                                                <asp:TextBox ID="txtSubmittedDate" ClientIDMode="Static" runat="server" MaxLength="15" CssClass="form-control date-picker"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <label for="txtRequestedDays">Requested Days</label>
                                            <asp:TextBox ID="txtRequestedDays" ClientIDMode="Static" runat="server" TextMode="Number" step="5" min="0" MaxLength="15" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-6">
                                            <div class="form-group">
                                                <div class="form-group">
                                                    <label for="txtCurrentDueDate">Current Due Date</label>
                                                    <asp:TextBox ID="txtCurrentDueDate" ReadOnly="true" ClientIDMode="Static" runat="server" MaxLength="15" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <div class="form-group">
                                                <label for="txtRequestedDueDate">Requested Due Date</label>
                                                <asp:TextBox ID="txtRequestedDueDate" ClientIDMode="Static" ReadOnly="true" runat="server" MaxLength="15" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdRequestOutstanding" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="hdCalendarEventTypeId" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="hdThirdExtension" runat="server" ClientIDMode="Static" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button ID="cmdRefreshExtensions" OnClick="cmdRefreshExtensions_Click" CausesValidation="false" CssClass="hidden" ClientIDMode="Static" Text="Refresh" runat="server" />
                                    <button id="cmdSave" type="button" class="btn btn-primary" data-bs-dismiss="modal">Submit</button>
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var extensionHash = {
        'pdf': 1,
        'doc': 1,
        'docx': 1,
        'xls': 1,
        'xlsx': 1,
    };
    const moduleId = <%=ModuleId%>;
    const portalId = <%=PortalId%>;
    const tabId = <%=TabId%>;
    const designationId = <%=DesignationId%>;
    const isAdmin = "<%=IsAdmin%>";
    const userId = <%=UserId%>;
    const adminRole = "<%=AdminRole%>";
    const uploadHandler = "<%=UploadHandler%>";
    const templateSourceDirectory = "<%=TemplateSourceDirectory%>";
    const domainUrl = window.location.origin;
    var courtReporterOptions = [];
    var scopistOptions = [];
    var transcriptionistOptions = [];
    var staffOptions = [];
    var judgeOptions = [];
    var acknowledgementTypes = [{ id: 0, name: "Acknowledgment Fee or Deposit Waived" }, { id: 1, name: "Acknowledgment Private Paying" }];
    var extensionDocTypes = [{ id: 2, name: "Extension Request" }];
    var extensionAddUrl = null;
    var serviceEmployee = {
        path: "TranscriptEmployee",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceReporter = {
        path: "TranscriptReporter",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceEvent = {
        path: "TranscriptEvent",
        framework: $.ServicesFramework(moduleId)
    };
    (function ($, Sys) {
        $(document).ready(function () {
            $(".date-picker").on("blur", function (e) {
                var date = $(this).val();
                $(this).val(date.replace(/\.|-/g, "/"));
            });
            PageInit();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                PageInit();
            });

        });
    }(jQuery, window.Sys));
    function PageInit() {
        $(document).on('show.bs.modal', '.modal', function (event) {
            var zIndex = 50 + (10 * $('.modal:visible').length);
            $(this).css('z-index', zIndex);
            setTimeout(function () {
                $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
            }, 0);
        });
        serviceEvent.baseUrl = serviceEvent.framework.getServiceRoot(serviceEvent.path);
        serviceEmployee.baseUrl = serviceEmployee.framework.getServiceRoot(serviceEmployee.path);
        serviceReporter.baseUrl = serviceReporter.framework.getServiceRoot(serviceReporter.path);
        extensionAddUrl = `${serviceEvent.baseUrl}Event/CreateExtension/`;
        judgeOptions = fetchEmployeeOptions(0);
        courtReporterOptions = fetchCourtReporterOptions("<%=CourtReporterRole%>");
        scopistOptions = fetchEmployeeOptions(2);
        transcriptionistOptions = fetchEmployeeOptions(3);
        staffOptions = fetchEmployeeOptions(4);

        $(document).on('click', function (event) {
            if (!$(event.target).closest('.drpjudge').length) {
                $('#drpJudge').hide();
            }
            if (!$(event.target).closest('.drpcourtreporter').length) {
                $('#drpCourtReporter').hide();
            }
            if (!$(event.target).closest('.drpscopist').length) {
                $('#drpScopist').hide();
            }
            if (!$(event.target).closest('.drptranscriptionist').length) {
                $('#drpTranscriptionist').hide();
            }
            if (!$(event.target).closest('.drpeditor').length) {
                $('#drpEditor').hide();
            }
            if (!$(event.target).closest('.drpproofer').length) {
                $('#drpProofer').hide();
            }
            if (!$(event.target).closest('.close').length) {
                $('.alert-dismissible').fadeOut();
            }
        });
        var uploadModal = document.getElementById('UploadModal')
        uploadModal.addEventListener('hidden.bs.modal', function (event) {
            ClearUploadForm();
        });
        var fileSelectionModal = document.getElementById('FileSelectionModal')
        fileSelectionModal.addEventListener('hidden.bs.modal', function (event) {
            ClearFileSelectionForm();
        });
        var addEventModal = document.getElementById('AddEventModal')
        addEventModal.addEventListener('hidden.bs.modal', function (event) {
            ClearEventForm();
        });
        $(".delete-extension").dnnConfirm({
            text: 'Are you sure you wish to Delete the selected Extension?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Extension?'
        });
        $(".delete-file").dnnConfirm({
            text: 'Are you sure you wish to Delete the selected Attachment?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Attachment?'
        });
        $(".delete-event").dnnConfirm({
            text: 'Are you sure you wish to Delete the selected Event?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Event?'
        });
        $(".complete-event").dnnConfirm({
            text: 'Are you sure you wish to Complete the selected Event?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Complete Event?'
        });
        $(".uncomplete-event").dnnConfirm({
            text: 'Are you sure you wish to Unmark Completion of the selected Event?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Unmark Complete?'
        });
        $("#drpFormType").on("change", function (e) {
            $("#hdSelectedFormType").val($(this).val());
        });
        $("#cmdExtension").on("click", function (e) {
            SetupFileSelectionForm(0);
        });
        $("#cmdAcknowledgements").on("click", function (e) {
            SetupFileSelectionForm(1);
        });
        $("#txtRequestedDays").on("input", function (e) {
            var days = Number($(this).val());
            var requestedValue = $("#txtCurrentDueDate").val();
            var requestedDate = new Date(requestedValue);
            requestedDate.setDate(requestedDate.getDate() + days);
            var datestring = requestedDate.toLocaleDateString();
            $("#txtRequestedDueDate").val(datestring);
        });
        $("#cmdDueDate").on("click", function (e) {
            e.preventDefault();
            $("#updateDueDate").fadeIn();
            $("#updateTrascriptFiled").fadeOut();
        });
        $("#cmdFiled").on("click", function (e) {
            e.preventDefault();
            $("#updateTrascriptFiled").fadeIn();
            $("#updateDueDate").fadeOut();
        });
        $(".hide-panels").on("click", function (e) {
            $("#updateTrascriptFiled").fadeOut();
            $("#updateDueDate").fadeOut();
        });
        $('#editorSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = staffOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpEditor').empty();
            if (filteredOptions.length > 0) {
                $('#drpEditor').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#editorSearch').val(option.name);
                        $('#selectedEditorId').val(option.id)
                        $('#drpEditor .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpEditor').hide();
                        $('#editorSearch').focus();
                    });
                    $('#drpEditor').append(div);
                });
            } else {
                $('#drpEditor').hide();
            }
        });
        $('#editorSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = staffOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Editor Not Found", name + " is not in the list of availalbe Editors. Please add the Name using the Names Tab and Try again.");
                $('#editorSearch').val("");
                $('#selectedEditorId').val("");
            } else {
                let foundOption = staffOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedEditorId').val() != foundOption[0].id)
                        $('#selectedEditorId').val(foundOption[0].id);
                } else {
                    ShowAlert("Editor Selection Invalid", "The Name entered is either not in the list of Editors or Matches more than one option. Please Select an Editor from the dropdown list.");
                }
                $('#drpEditor').hide();
            }
        });
        $('#transcriptionistSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = transcriptionistOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpTranscriptionist').empty();
            if (filteredOptions.length > 0) {
                $('#drpTranscriptionist').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#transcriptionistSearch').val(option.name);
                        $('#selectedTranscriptionistId').val(option.id);
                        $('#drpTranscriptionist .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpTranscriptionist').hide();
                        $('#transcriptionistSearch').focus();
                    });
                    $('#drpTranscriptionist').append(div);
                });
            } else {
                $('#drpTranscriptionist').hide();
            }
        });
        $('#transcriptionistSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = transcriptionistOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Trascriptionist Not Found", name + " is not in the list of availalbe Trascriptionists. Please add the Name using the Names Tab and Try again.");
                $('#selectedTranscriptionistId').val("");
                $('#transcriptionistSearch').val("");
            } else {
                let foundOption = transcriptionistOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedTranscriptionistId').val() != foundOption[0].id)
                        $('#selectedTranscriptionistId').val(foundOption[0].id);
                } else {
                    ShowAlert("Transcriptionist Selection Invalid", "The Name entered is either not in the list of Transcriptionists or Matches more than one option. Please Select a Transcriptionist from the dropdown list.");
                }
                $('#drpTranscriptionist').hide();
            }
        });
        $('#prooferSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = staffOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpProofer').empty();
            if (filteredOptions.length > 0) {
                $('#drpProofer').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#prooferSearch').val(option.name);
                        $('#selectedProoferId').val(option.id);
                        $('#drpProofer .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpProofer').hide();
                        $('#prooferSearch').focus();
                    });
                    $('#drpProofer').append(div);
                });
            } else {
                $('#drpProofer').hide();
            }
        });
        $('#prooferSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = staffOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Proofer Not Found", name + " is not in the list of availalbe Proofers. Please add the Name using the Names Tab and Try again.");
                $('#prooferSearch').val("");
                $('#selectedProoferId').val("");
            } else {
                let foundOption = staffOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedProoferId').val() != foundOption[0].id)
                        $('#selectedProoferId').val(foundOption[0].id);
                } else {
                    ShowAlert("Proofer Selection Invalid", "The Name entered is either not in the list of Proofers or Matches more than one option. Please Select a Proofer from the dropdown list.");
                }
                $('#drpProofer').hide();
            }
        });
        $('#scopistSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = scopistOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpScopist').empty();
            if (filteredOptions.length > 0) {
                $('#drpScopist').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#scopistSearch').val(option.name);
                        $('#selectedScopistId').val(option.id);
                        $('#drpScopist .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpScopist').hide();
                        $('#scopistSearch').focus();
                    });
                    $('#drpScopist').append(div);
                });
            } else {
                $('#drpScopist').hide();
            }
        });
        $('#scopistSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = scopistOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Scopist Not Found", name + " is not in the list of availalbe Scropists. Please add the Name using the Names Tab and Try again.");
                $('#scopistSearch').val("");
                $('#selectedScopistId').val("");
            } else {
                let foundOption = scopistOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedScopistId').val() != foundOption[0].id)
                        $('#selectedScopistId').val(foundOption[0].id);
                } else {
                    ShowAlert("Scopist Selection Invalid", "The Name entered is either not in the list of Scopists or Matches more than one option. Please Select a Scopist from the dropdown list.");
                }
                $('#drpScopist').hide();
            }
        });
        $('#reporterSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = courtReporterOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpCourtReporter').empty();
            if (filteredOptions.length > 0) {
                $('#drpCourtReporter').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#reporterSearch').val(option.name);
                        $('#selectedCourtReporterId').val(option.id);
                        $('#drpCourtReporter .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpCourtReporter').hide();
                        $("#reporterSearch").focus();
                    });
                    $('#drpCourtReporter').append(div);
                });
            } else {
                $('#drpCourtReporter').hide();
            }
        });
        $('#reporterSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = courtReporterOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Court Reporter Not Found", name + " is not in the list of availalbe Court Reporters. Please contact the Help Desk to have the user added to the court reporter role.");
                $('#selectedCourtReporterId').val("");
                $('#reporterSearch').val("");
            } else {
                let foundOption = courtReporterOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedCourtReporterId').val() != foundOption[0].id)
                        $('#selectedCourtReporterId').val(foundOption[0].id);
                } else {
                    ShowAlert("Court Reporter Selection Invalid", "The Name entered is either not in the list of Court Reporters or Matches more than one option. Please Select a Court Reporter from the dropdown list.");
                }
                $('#drpCourtReporter').hide();
            }
        });
        $('#judgeSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = judgeOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#drpJudge').empty();
            if (filteredOptions.length > 0) {
                $('#drpJudge').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#judgeSearch').val(option.name);
                        $('#selectedJudgeId').val(option.id);
                        $('#drpJudge .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#drpJudge').hide();
                        $("#judgeSearch").focus();
                    });
                    $('#drpJudge').append(div);
                });
            } else {
                $('#drpJudge').hide();
            }
        });
        $('#judgeSearch').on("blur", function () {
            let input = $(this).val().toLowerCase();
            let name = $(this).val();
            let filteredOptions = judgeOptions.filter(option => option.name.toLowerCase().includes(input));
            if (filteredOptions.length === 0) {
                ShowAlert("Judge Not Found", name + " is not in the list of availalbe Judges. Please contact the Help Desk to have the user added.");
                $('#selectedJudgeId').val("");
                $('#judgeSearch').val("");
            } else {
                let foundOption = judgeOptions.filter(option => option.name.toLowerCase() === input);
                if (foundOption.length == 1) {
                    if ($('#selectedJudgeId').val() != foundOption[0].id)
                        $('#selectedJudgeId').val(foundOption[0].id);
                } else {
                    ShowAlert("Judge Selection Invalid", "The Name entered is either not in the list of Judges or Matches more than one option. Please Select a judge from the dropdown list.");
                }
                $('#drpJudge').hide();
            }
        });
        $('#cmdSave').on("click", function (e) {
            e.preventDefault();
            ToggleFileForm(false);
            var documentType = Number($("#drpFormType").val());
            var eventTypeId = Number($("#hdCalendarEventTypeId").val());
            var submittedDate = $("#txtSubmittedDate").val();
            var requestedDate = $("#txtRequestedDueDate").val();
            var countyName = $("#txtCounty").val();
            var reason = $("#txtReason").val();
            var formCreationUrl = `${domainUrl}/${templateSourceDirectory}/Handlers/WordDocHandler.ashx?did=${designationId}&type=${documentType}&reason=${reason}&date=${requestedDate}`;
            var createdByUserId = userId;
            if (documentType == 2) {
                var newExtension = {
                    designationid: designationId, eventtypeid: eventTypeId, requesteddate: requestedDate, submitteddate: submittedDate,
                    countyname: countyName, createdbyuserid: userId, portalid: portalId, adminrole: adminRole
                };
                AddExtension(newExtension, formCreationUrl);
            } else {
                window.location.replace(formCreationUrl);
            }
        });

        function searchListForValue(dropdownId, searchText) {
            // Get the dropdown element
            let $dropdown = $('#' + dropdownId);

            // Find the option with matching text (case-insensitive)
            let $option = $dropdown.find('option').filter(function () {
                return $(this).text().toLowerCase() === searchText.toLowerCase();
            });

            // Return the value if found, otherwise return null
            return $option.length ? $option.val() : null;
        }
        $("#UploadModal").on("click", "#uplFile", function (e) {
            $("#upload-overlay").show();
        });
        $("#UploadModal").on("change", "#uplFile", function (e) {
            check_extension($(this).val());
        });
    }
    function AddExtension(extension, url) {
        var extensionMessage = "";
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: extensionAddUrl,
                beforeSend: serviceEvent.framework.setModuleHeaders,
                data: (extension),
                success: function (result) {
                    var extensionId = result.ExtensionId;
                    if (extensionId) {
                        $("#cmdRefreshExtensions").click();
                        window.location.replace(url);
                    } else {
                        extensionMessage += "Unable to add extension. Please refresh the page and retry \n\n\n";
                        ShowAlert("Error Attempting to Add Extension", extensionMessage);
                    }
                },
                error: function (xhr, status, error) {
                    ShowAlert("Error Attempting to Add Extension", "Unable to add extension.\n\nMake sure you are logged in and try again. \n\nError:" + error);
                }
            });
        } catch (error) {
            extensionMessage += error + " \n\n\n";
            ShowAlert("Error Attempting to Add Extension", extensionMessage);
        }
    }
    function ValidateDateType(sender, args) {
        args.IsValid = false;
        var controlId = sender.controltovalidate;
        var dateValue = $("#" + controlId).val();
        dateValue = dateValue.replace(/\.|-/g, "/");
        const currentYear = new Date().getFullYear().toString();
        const indexOfSlash = dateValue.lastIndexOf('/');
        if (indexOfSlash == dateValue.length - 1) {
            dateValue += currentYear;
        } else {
            if (dateValue.length <= 5) {
                dateValue += "/" + currentYear;
            }
        }
        var isDate = isValidDate(dateValue);
        if (isDate) {
            $("#" + controlId).val(dateValue);
            args.IsValid = true;
        }
    }
    function isValidDate(dateString) {
        const regex = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
        if (!regex.test(dateString)) return false;

        const [month, day, year] = dateString.split('/').map(Number);
        const date = new Date(`${year}-${month}-${day}`);

        return (
            date.getFullYear() === year &&
            date.getMonth() === month - 1 &&
            date.getDate() === day
        );
    }
    function SetupFileSelectionForm(type) {
        $("#txtRequestedDays").val("");
        $("#txtRequestedDueDate").val("");
        var select = $("#drpFormType");
        select.empty();
        if (type == 0) {
            $("#FileSelectionModalLabel").text("Create Extension Request");
            $.each(extensionDocTypes, function (i, option) {
                select.append($('<option>', {
                    value: option.id,
                    text: option.name
                }));
            });
        }
        if (type == 1) {
            $("#FileSelectionModalLabel").text("Create Acknowledgement");
            $.each(acknowledgementTypes, function (i, option) {
                select.append($('<option>', {
                    value: option.id,
                    text: option.name
                }));
            });
        }
        $("#hdSelectedFormType").val($("#drpFormType").val());
    }
    function ShowForm(url) {
        $('.create-form').attr('src', url);

        // window.open(url, '_blank').focus();
    }
    function GetSelectedJudge() {
        let selectedElement = $('#drpJudge .list-group-item.active');
        return {
            id: Number($('#selectedJudgeId').val()),
            name: $('#judgeSearch').val()
        };
    }
    function GetSelectedCourtReporter() {
        let selectedElement = $('#drpCourtReporter .list-group-item.active');
        return {
            id: Number($('#selectedCourtReporterId').val()),
            name: $('#judgeSearch').val()
        };
    }
    function GetSelectedScopist() {
        let selectedElement = $('#drpScopist .list-group-item.active');
        return {
            id: Number($('#selectedScopistId').val()),
            name: $('#scopistSearch').val()
        };
    }
    function GetSelectedEditor() {
        let selectedElement = $('#drpEditor .list-group-item.active');
        return {
            id: Number($('#selectedEditorId').val()),
            name: $('#editorSearch').val()
        };
    }
    function GetSelectedProofer() {
        let selectedElement = $('#drpProofer .list-group-item.active');
        return {
            id: Number($('#selectedProoferId').val()),
            name: $('#prooferSearch').val()
        };
    }
    function GetSelectedTranscriptionist() {
        let selectedElement = $('#drpTranscriptionist .list-group-item.active');
        return {
            id: Number($('#selectedTranscriptionistId').val()),
            name: $('#transcriptionistSearch').val()
        };
    }

    function fetchEmployeeOptions(employeeType) {
        $.ajax({
            url: `${serviceEmployee.baseUrl}Employee/GetEmployeeDropDown/${employeeType}`,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                switch (employeeType) {
                    case 0:
                        judgeOptions = response.data;
                        break;
                    case 2:
                        scopistOptions = response.data;
                        break;
                    case 3:
                        transcriptionistOptions = response.data;
                        break;
                    default:
                        staffOptions = response.data;
                }
            },
            error: function () {
                console.error('Failed to fetch options');
            }
        });
    }
    function fetchCourtReporterOptions(roleName) {
        $.ajax({
            url: `${serviceReporter.baseUrl}Employee/GetCourtReporterDropDown/${roleName}`,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                courtReporterOptions = response.data;
            },
            error: function () {
                console.error('Failed to fetch options');
            }
        });
    }
    function ClearEventForm() {
        $("#txtHearingDate").val("");
        $("#judgeSearch").val("");
        $("#selectedJudgeId").val("");
        $("#drpHearingType").val("0");
        $("#reporterSearch").val("");
        $("#selectedCourtReporterId").val("");
        $("#txtEstimagedPages").val("");
        $("#txtDaysUntilCompletion").val("");
        $("#scopistSearch").val("");
        $("#selectedScopistId").val("");
        $("#txtScopeSent").val("");
        $("#txtScopePagesIn").val("");
        $("#txtScopeReturned").val("");
        $("#txtScopePagesOut").val("");
        $("#transcriptionistSearch").val("");
        $("#selectedTranscriptionistId").val("");
        $("#txtTransSent").val("");
        $("#txtTransPagesIn").val("");
        $("#txtTransReturned").val("");
        $("#txtTransPagesOut").val("");
        $("#editorSearch").val("");
        $("#selectedEditorId").val("");
        $("#txtEditSent").val("");
        $("#txtEditPagesIn").val("");
        $("#txtEditReturned").val("");
        $("#txtEditPagesOut").val("");
        $("#prooferSearch").val("");
        $("#selectedProoferId").val("");
        $("#txtProofSent").val("");
        $("#txtProofPagesIn").val("");
        $("#txtProofReturned").val("");
        $("#txtProofPagesOut").val("");
        $("#txtCompletedPages").val("");
        $("#hdEventId").val("");
    }
    function ClearUploadForm() {
        $("#uplFile").prop('disabled', false);
        $('#txtUploadeTitle').val("");
        $('#hdAttachmentId').val("");
        $('#hdFileId').val("");
        return false;
    }
    function ClearFileSelectionForm() {
        $('#txtReason').val("");
        $('#drpFormType').val("");
        $('#txtRequestedDays').val("");
        $('#txtRequestedDueDate').val("");
        $('.create-form').attr('src', '');
    }
    function ToggleEventForm(toggleValue) {
        if (toggleValue) {
            $('#AddEventModal').modal('show');
        } else {
            $('#AddEventModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ToggleUploadForm(toggleValue) {
        if (toggleValue) {
            $('#UploadModal').modal('show');
        } else {
            $('#UploadModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#AddEventModal').modal('show');
        } else {
            $('#AddEventModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ToggleFileForm(toggleValue) {
        if (toggleValue) {
            $('#FileSelectionModal').modal('show');
        } else {
            $('#FileSelectionModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function validateUpload(source, e) {
        $("#hdFileId").val().length > 0 ? e.IsValid = true : e.IsValid = false;
    }
    function WriteAttachmentMessage(filename) {
        if (filename == "") {
            $("#uploadInfo").html("<span class='text-danger'>Unable to upload file. Please make sure the file is in an allowed format.</span>");
        } else {
            $("#fileAttachmentWarning").fadeOut();
            $("#uploadInfo").html("<div class='alert alert-warning mt-3'><i class='fas fa-file-arrow-up'></i> File Uploaded! Click Save to add to Database.</div>");
        }
        var upload = $("#uplFile");
        var html = upload.parent().html();
        upload.parent().html(html.replace(filename, "File Selected"));
    }
    function HandleUpload() {
        $("#upload-overlay").show();
        var description = $("#txtUploadeTitle").val();
        var upload = $("#uplFile");
        if (upload.is(':enabled')) {
            var fileUpload = $("#uplFile").get(0);
            var file = fileUpload.files[0];
            if (file.length == 0) {
                $("#uploadInfo").html("<span class='text-danger'>Please Choose a File!</span>");
                return false;
            }
            var filename = file.name;
            var data = new FormData();
            data.append("file", file);
            data.append("mid", moduleId);
            data.append("did", designationId);
            data.append("des", description);
            var options = {};
            options.url = uploadHandler;
            options.type = "POST";
            options.data = data;
            options.contentType = false;
            options.processData = false;
            options.success = function (fileId) {
                $("#upload-overlay").hide();
                $("#hdFileId").val(fileId);
                WriteAttachmentMessage(filename);
            };
            options.error = function (err) {
                ShowAlert("Upload Error", err.statusText);
                setTimeout(function () {
                    $("#upload-overlay").hide();
                    $("#uploadInfo").html('');
                }, 1000);
            };
            $.ajax(options);
        }
    }
    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        if (extensionHash[ext]) {
            $("#uploadInfo").html("");
            HandleUpload();
            return true;
        } else {
            $("#uploadInfo").html("<span class='text-danger'>Invalid File Type, please choose an allowed file type!</span>");
            return false;
        }
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }

</script>
