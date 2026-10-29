<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageCategories.ascx.cs" Inherits="tjc.Modules.CourtCalendar.ManageCategories" %>

<div class="cc-admin">
    <asp:Literal ID="ltMessage" runat="server" EnableViewState="false" />

    <asp:Panel ID="pnlMain" runat="server">
        <div class="cc-toolbar mb-3">
            <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-link"><i class="fas fa-arrow-left" aria-hidden="true"></i>&nbsp;Back</asp:HyperLink>
            <asp:Button ID="btnAddNew" runat="server" CssClass="btn btn-primary" Text="Add category" OnClick="btnAddNew_Click" />
        </div>
        <h2 class="cc-h2">Categories &ndash; <asp:Literal ID="ltDivision" runat="server" /></h2>
        <p class="text-muted">Assign a color to each category (except Default, which shows as light grey). Add your own categories as needed.</p>

        <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="cc-edit-form mb-3">
            <asp:HiddenField ID="hfCategoryId" runat="server" />
            <div class="row">
                <div class="col-md-5 mb-2">
                    <label class="form-label" for="<%= txtName.ClientID %>">Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-4 mb-2">
                    <label class="form-label" for="<%= txtColor.ClientID %>">Color (hex, e.g. #cfe8ff)</label>
                    <asp:TextBox ID="txtColor" runat="server" CssClass="form-control" placeholder="#rrggbb" />
                    <small class="form-text text-muted" id="defaultNote" runat="server" visible="false">The Default category has no color.</small>
                </div>
                <div class="col-md-3 mb-2 d-flex align-items-end">
                    <div class="form-check">
                        <asp:CheckBox ID="chkActive" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label" for="<%= chkActive.ClientID %>">Active</label>
                    </div>
                </div>
            </div>
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" />
        </asp:Panel>

        <table class="table cc-category-table">
            <thead><tr><th>Category</th><th>Color</th><th>Type</th><th></th></tr></thead>
            <tbody>
                <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Name") %></td>
                            <td>
                                <%# ColorSwatch((string)Eval("Color"), (bool)Eval("IsDefault")) %>
                            </td>
                            <td><%# (bool)Eval("IsSystem") ? "Built-in" : "Custom" %></td>
                            <td class="text-end">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="edit" CommandArgument='<%# Eval("CategoryId") %>'><i class="fas fa-pen" aria-hidden="true"></i> Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="delete" CommandArgument='<%# Eval("CategoryId") %>' Visible='<%# !(bool)Eval("IsSystem") %>' OnClientClick="return confirm('Delete this category?');"><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>
</div>
