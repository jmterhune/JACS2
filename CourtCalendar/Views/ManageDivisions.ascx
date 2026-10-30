<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageDivisions.ascx.cs" Inherits="tjc.Modules.CourtCalendar.ManageDivisions" %>

<div class="cc-admin">
    <asp:Literal ID="ltMessage" runat="server" EnableViewState="false" />

    <asp:Panel ID="pnlList" runat="server">
        <div class="cc-toolbar mb-3">
            <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-link"><i class="fas fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to calendars</asp:HyperLink>
            <asp:Button ID="btnAddNew" runat="server" CssClass="btn btn-primary" Text="Add division" OnClick="btnAddNew_Click" />
        </div>
        <h2 class="cc-h2">Divisions</h2>
        <table class="table cc-division-admin-table">
            <thead><tr><th>Name</th><th>Judge</th><th>Layout</th><th>Slug</th><th>Active</th><th></th></tr></thead>
            <tbody>
                <asp:Repeater ID="rptDivisions" runat="server" OnItemCommand="rptDivisions_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Name") %></td>
                            <td><%# Eval("JudgeName") %></td>
                            <td><%# LayoutName((int)Eval("CalendarTypeId")) %></td>
                            <td><code><%# Eval("Slug") %></code></td>
                            <td><%# (bool)Eval("Active") ? "Yes" : "No" %></td>
                            <td class="text-end">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="edit" CommandArgument='<%# Eval("DivisionId") %>'><i class="fas fa-pen" aria-hidden="true"></i> Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="categories" CommandArgument='<%# Eval("DivisionId") %>'><i class="fas fa-palette" aria-hidden="true"></i></asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="delete" CommandArgument='<%# Eval("DivisionId") %>' OnClientClick="return confirm('Delete this division? Its calendars and entries are not removed automatically.');"><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="cc-edit-form">
        <h2 class="cc-h2"><asp:Literal ID="ltEditTitle" runat="server" Text="Add division" /></h2>
        <asp:HiddenField ID="hfDivisionId" runat="server" />
        <div class="row">
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= txtName.ClientID %>">Division name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= ddlCounty.ClientID %>">County</label>
                <asp:DropDownList ID="ddlCounty" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- select --</asp:ListItem>
                    <asp:ListItem Value="Sarasota">Sarasota</asp:ListItem>
                    <asp:ListItem Value="Manatee">Manatee</asp:ListItem>
                    <asp:ListItem Value="DeSoto">DeSoto</asp:ListItem>
                    <asp:ListItem Value="South County">South County</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= ddlJudge.ClientID %>">Assigned judge</label>
                <asp:DropDownList ID="ddlJudge" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= ddlType.ClientID %>">Calendar layout</label>
                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
                    <asp:ListItem Value="1">Weekday headers (Mon&ndash;Fri across the top)</asp:ListItem>
                    <asp:ListItem Value="2">Row headers (week labels in the first column)</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= txtCourtroom.ClientID %>">Courtroom</label>
                <asp:TextBox ID="txtCourtroom" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= txtSlug.ClientID %>">URL slug</label>
                <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control" />
                <small class="form-text text-muted">Leave blank to generate from the name. Used in the public link (?div=slug).</small>
            </div>
            <div class="col-md-4 mb-2">
                <label class="form-label" for="<%= txtZoomId.ClientID %>">Zoom meeting ID</label>
                <asp:TextBox ID="txtZoomId" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-4 mb-2">
                <label class="form-label" for="<%= txtZoomPwd.ClientID %>">Zoom passcode</label>
                <asp:TextBox ID="txtZoomPwd" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-4 mb-2">
                <label class="form-label" for="<%= txtJaPhone.ClientID %>">JA phone</label>
                <asp:TextBox ID="txtJaPhone" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-2">
                <label class="form-label" for="<%= txtEmail.ClientID %>">Division email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3 mb-2">
                <label class="form-label" for="<%= txtSortOrder.ClientID %>">Sort order</label>
                <asp:TextBox ID="txtSortOrder" runat="server" CssClass="form-control" Text="0" />
            </div>
            <div class="col-md-3 mb-2 d-flex align-items-end">
                <div class="form-check">
                    <asp:CheckBox ID="chkActive" runat="server" CssClass="form-check-input" Checked="true" />
                    <label class="form-check-label" for="<%= chkActive.ClientID %>">Active</label>
                </div>
            </div>
        </div>
        <div class="mt-2">
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" />
        </div>
    </asp:Panel>
</div>
