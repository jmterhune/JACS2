<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.CourtCalendar.View" %>

<div class="cc-admin">
    <asp:Panel ID="pnlAdminTools" runat="server" CssClass="cc-admin-tools" Visible="false">
        <asp:HyperLink ID="lnkManageJudges" runat="server" CssClass="btn btn-secondary"><i class="fas fa-user-tie" aria-hidden="true"></i>&nbsp;Manage Judges</asp:HyperLink>
        <asp:HyperLink ID="lnkManageDivisions" runat="server" CssClass="btn btn-secondary"><i class="fas fa-sitemap" aria-hidden="true"></i>&nbsp;Manage Divisions</asp:HyperLink>
    </asp:Panel>

    <asp:Literal ID="ltContent" runat="server" EnableViewState="false" />
</div>
