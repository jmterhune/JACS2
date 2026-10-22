<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CodeComparison.ascx.cs" Inherits="tjc.Modules.CourtRegistry.CodeComparison" %>
<div class="jacCodeComparison">
    <h2>Compare JAC Codes by Year</h2>
    <div class="compareCriteria d-flex flex-wrap align-items-end gap-3 mb-3">
        <div>
            <asp:Label ID="lblAttorney" runat="server" AssociatedControlID="drpAttorney" CssClass="form-label d-block">Attorney</asp:Label>
            <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-select compare-attorney" />
        </div>
        <div>
            <asp:Label ID="lblYear" runat="server" AssociatedControlID="drpYear" CssClass="form-label d-block">Year</asp:Label>
            <asp:DropDownList ID="drpYear" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" />
        </div>
        <div>
            <asp:Label ID="lblYear2" runat="server" AssociatedControlID="drpYear2" CssClass="form-label d-block">Year to Compare</asp:Label>
            <asp:DropDownList ID="drpYear2" runat="server" Enabled="false" CssClass="form-select" />
        </div>
        <div>
            <asp:Button ID="cmdCompare" Text="Compare" runat="server" CssClass="btn btn-primary" OnClick="cmdCompare_Click" />
        </div>
    </div>
    <table class="compareResults table table-striped table-bordered mt-3">
        <thead>
            <asp:Literal ID="ltCompareTableHeader" runat="server" />
        </thead>
        <tbody>
            <asp:Literal ID="ltCompareTable" runat="server" />
        </tbody>
    </table>
    <p>
        <asp:HyperLink runat="server" ID="lnkCancel" CssClass="btn btn-default" Text="Return to List" />
    </p>
</div>
