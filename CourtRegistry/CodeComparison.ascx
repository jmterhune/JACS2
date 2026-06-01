<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CodeComparison.ascx.cs" Inherits="tjc.Modules.CourtRegistry.CodeComparison" %>
<div class="jacCodeComparison">
    <h2>Compare JAC Codes by Year</h2>
    <table role="presentation" class="compareCriteria">
        <tbody>
            <tr>
                <td>
                    <asp:Label ID="lblAttorney" runat="server" AssociatedControlID="drpAttorney">Attorney: </asp:Label>
                    <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control" />
                </td>
                <td>
                    <asp:Label ID="lblYear" runat="server" AssociatedControlID="drpYear">Year: </asp:Label>
                    <asp:DropDownList ID="drpYear" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" />
                </td>
                <td>
                    <asp:Label ID="lblYear2" runat="server" AssociatedControlID="drpYear2">Year to Compare: </asp:Label>
                    <asp:DropDownList ID="drpYear2" runat="server" Enabled="false" CssClass="form-control" />
                </td>
                <td>
                    <asp:Button ID="cmdCompare" Text="Compare" runat="server" CssClass="btn btn-primary" OnClick="cmdCompare_Click" />
                </td>
            </tr>
        </tbody>
    </table>
    <table class="compareResults">
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
