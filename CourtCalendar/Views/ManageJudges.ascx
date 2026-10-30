<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageJudges.ascx.cs" Inherits="tjc.Modules.CourtCalendar.ManageJudges" %>

<div class="cc-admin">
    <asp:Literal ID="ltMessage" runat="server" EnableViewState="false" />

    <asp:Panel ID="pnlList" runat="server">
        <div class="cc-toolbar mb-3">
            <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-link"><i class="fas fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to calendars</asp:HyperLink>
            <asp:Button ID="btnAddNew" runat="server" CssClass="btn btn-primary" Text="Add judge" OnClick="btnAddNew_Click" />
        </div>
        <h2 class="cc-h2">Judges</h2>
        <table class="table cc-judge-table">
            <thead><tr><th>Name</th><th>Judge user</th><th>Judicial assistant</th><th>Active</th><th></th></tr></thead>
            <tbody>
                <asp:Repeater ID="rptJudges" runat="server" OnItemCommand="rptJudges_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("FullName") %></td>
                            <td><%# Eval("JudgeUserId") %></td>
                            <td><%# Eval("JaName") %></td>
                            <td><%# (bool)Eval("Active") ? "Yes" : "No" %></td>
                            <td class="text-end">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="edit" CommandArgument='<%# Eval("JudgeId") %>'><i class="fas fa-pen" aria-hidden="true"></i> Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="delete" CommandArgument='<%# Eval("JudgeId") %>' OnClientClick="return confirm('Delete this judge?');"><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="cc-edit-form">
        <h2 class="cc-h2"><asp:Literal ID="ltEditTitle" runat="server" Text="Add judge" /></h2>
        <asp:HiddenField ID="hfJudgeId" runat="server" />
        <div class="mb-2">
            <label class="form-label" for="<%= txtFullName.ClientID %>">Judge name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label class="form-label" for="<%= txtJudgeUser.ClientID %>">Judge (intranet account)</label>
            <asp:TextBox ID="txtJudgeUser" runat="server" CssClass="form-control" autocomplete="off" placeholder="Type to search&hellip;" />
            <datalist id="dlJudgeUsers_<%= ModuleId %>">
                <asp:Literal ID="ltJudgeUsers" runat="server" EnableViewState="false" />
            </datalist>
            <small class="form-text text-muted">Pick a user in the configured Judge role. The judge signs in with this account.</small>
        </div>
        <div class="mb-2">
            <label class="form-label" for="<%= txtJaUser.ClientID %>">Judicial assistant (intranet account)</label>
            <asp:TextBox ID="txtJaUser" runat="server" CssClass="form-control" autocomplete="off" placeholder="Type to search&hellip;" />
            <datalist id="dlJaUsers_<%= ModuleId %>">
                <asp:Literal ID="ltJaUsers" runat="server" EnableViewState="false" />
            </datalist>
            <small class="form-text text-muted">Pick a user in the configured JA role.</small>
        </div>
        <div class="mb-2">
            <label class="form-label" for="<%= txtJaName.ClientID %>">JA display name</label>
            <asp:TextBox ID="txtJaName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label class="form-label" for="<%= txtPhone.ClientID %>">Phone</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
        </div>
        <div class="form-check mb-3">
            <asp:CheckBox ID="chkActive" runat="server" CssClass="form-check-input" Checked="true" />
            <label class="form-check-label" for="<%= chkActive.ClientID %>">Active</label>
        </div>
        <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" />
    </asp:Panel>
</div>
