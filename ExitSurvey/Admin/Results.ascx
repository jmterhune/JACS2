<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Results.ascx.cs" Inherits="tjc.Modules.ExitSurvey.Admin.Results" %>

<div class="container-fluid exit-survey">

    <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>

    <div class="row mb-3">
        <div class="col">
            <h4>Exit Survey Results</h4>
        </div>
    </div>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
        <p>No exit surveys have been submitted yet.</p>
    </asp:Panel>

    <asp:Repeater ID="rptResults" runat="server" OnItemDataBound="rptResults_ItemDataBound">
        <HeaderTemplate>
            <table class="table table-striped exit-survey-results">
                <thead>
                    <tr>
                        <th>Submitted</th>
                        <th>Name</th>
                        <th>Accepted Another Position</th>
                        <th>Would Return</th>
                        <th>Share With Supervisor</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><asp:Literal ID="ltSubmitted" runat="server" /></td>
                <td><asp:Literal ID="ltName" runat="server" /></td>
                <td><asp:Literal ID="ltAccepted" runat="server" /></td>
                <td><asp:Literal ID="ltReturn" runat="server" /></td>
                <td><asp:Literal ID="ltShare" runat="server" /></td>
                <td>
                    <asp:HyperLink ID="lnkView" runat="server" CssClass="btn btn-sm btn-primary" Text="View" />
                    <asp:HyperLink ID="lnkPrint" runat="server" CssClass="btn btn-sm btn-secondary" ToolTip="Print / Save as PDF"><i class="fas fa-print"></i></asp:HyperLink>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

</div>
