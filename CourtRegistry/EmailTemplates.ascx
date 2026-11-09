<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTemplates.ascx.cs" Inherits="tjc.Modules.CourtRegistry.EmailTemplates" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=WithdrawalListUrl%>">Withdrawals</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#email-templates" data-bs-toggle="tab">Email Templates</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="email-templates" class="tab-pane active">

            <asp:Literal ID="ltMessage" runat="server" />

            <p class="text-muted">
                These are the emails sent to the attorney during the removal process. Editing one here
                changes what is sent from then on. If a template is switched off or left empty, the
                module sends its original built-in wording instead, so an attorney is never sent a
                blank email.
            </p>

            <asp:Panel ID="pnlList" runat="server">
                <asp:Repeater ID="rptTemplates" runat="server" OnItemCommand="rptTemplates_ItemCommand">
                    <HeaderTemplate>
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th>Email</th>
                                    <th>Subject</th>
                                    <th>Status</th>
                                    <th>Last changed</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <strong><%# Eval("TemplateName") %></strong>
                                <div class="small text-muted"><%# Eval("Description") %></div>
                            </td>
                            <td><%# Eval("Subject") %></td>
                            <td><%# EnabledBadge(Eval("IsEnabled")) %></td>
                            <td><%# ModifiedText(Eval("DateModified")) %></td>
                            <td class="text-end">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-primary" CausesValidation="false"
                                    CommandName="edit" CommandArgument='<%# Eval("TemplateId") %>' Text="Edit" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                    <div class="alert alert-warning">
                        <strong>No templates are set up yet.</strong>
                        Run the email-templates.sql script against the registry database to create them.
                        Until then the emails still send, using their built-in wording.
                    </div>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                <asp:HiddenField ID="hdTemplateId" runat="server" />

                <div class="card mb-3 cr-template-edit">
                    <div class="card-header">
                        <h4 class="mb-0"><asp:Literal ID="ltEditTitle" runat="server" /></h4>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-8">
                                <asp:Label runat="server" AssociatedControlID="txtSubject" Text="Subject <em>*</em>" />
                                <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" MaxLength="500" />
                                <asp:RequiredFieldValidator ID="valSubject" runat="server" Display="Dynamic"
                                    ControlToValidate="txtSubject" ValidationGroup="template"
                                    ErrorMessage="Subject is required" CssClass="label label-danger" />
                            </div>
                            <div class="col-md-4">
                                <asp:Label runat="server" Text="Status" />
                                <div class="form-check">
                                    <asp:CheckBox ID="chkEnabled" runat="server" CssClass="form-check-input" />
                                    <asp:Label runat="server" AssociatedControlID="chkEnabled" CssClass="form-check-label"
                                        Text="Send this template. When unticked, the built-in wording is sent instead." />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Description (shown in the list above)" />
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" MaxLength="500" />
                            </div>
                        </div>

                        <div class="alert alert-info">
                            <strong>Placeholders you can use in the subject and body.</strong>
                            They are replaced when the email is sent. One that has no value is replaced with nothing.
                            <asp:Literal ID="ltTokens" runat="server" />
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <asp:Label runat="server" Text="Body" />
                                <dnn:texteditor id="txtBody" runat="server" height="420" width="100%" />
                            </div>
                        </div>
                    </div>
                    <div class="card-footer">
                        <asp:Button ID="cmdSave" runat="server" Text="Save Template" CssClass="btn btn-primary"
                            ValidationGroup="template" OnClick="cmdSave_Click" />
                        <asp:Button ID="cmdCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
                            CausesValidation="false" OnClick="cmdCancel_Click" />
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
