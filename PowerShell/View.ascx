<%@ Control Language="vb" AutoEventWireup="true" CodeBehind="View.ascx.vb" Inherits="tjc.Modules.PowerShell.View" %>
<style>
    #account td,#account th{border:solid 1px #333;padding:8px;}
    #account{border-collapse:collapse}
    #account th{background-color:#ccc;color:#000}
    #account .alt{background-color:#eee}
</style>
<asp:Repeater ID="rptUsers" runat="server">
    <HeaderTemplate>
        <table id="account">
            <thead>
                <tr>
                    <th>Account Name</th>
                    <th>Last Login Date</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td><%#Eval("UserName") %></td>
            <td><%#Eval("LastLoginDate") %></td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alt">
            <td><%#Eval("UserName") %></td>
            <td><%#Eval("LastLoginDate") %></td>
        </tr>

    </AlternatingItemTemplate>
    <FooterTemplate></tbody></table></FooterTemplate>
</asp:Repeater>
