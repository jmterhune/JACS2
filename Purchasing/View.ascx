<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.Purchasing.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div id="stamp-container">
    <div class="row">
        <div class="col-4">
            <div class="input-group">
                <asp:Label runat="server" CssClass="input-group-text" AssociatedControlID="txtStartDate" Text="Start Date" />
                <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtStartDate" />
                <asp:Label runat="server" CssClass="input-group-text" AssociatedControlID="txtEndDate" Text="End Date" />
                <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtEndDate" />
            </div>
        </div>
        <div class="col-auto">
            <div class="form-check form-switch">
                <asp:CheckBox ID="chkShowCompleted" runat="server" AutoPostBack="true" OnCheckedChanged="chkShowCompleted_CheckedChanged" Text="Show Completed" />
            </div>
        </div>
    </div>
    <p>
        <asp:LinkButton ID="cmdSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="cmdSearch_Click" />
        <asp:HyperLink ID="lnkReset" runat="server" CssClass="btn btn-secondary" Text="Reset" />
        <asp:HyperLink ID="lnkForm" runat="server" CssClass="btn btn-tertiary" Text="View Form" />
    </p>
    <hr />
    <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand" OnItemDataBound="rptOrders_ItemDataBound">
        <HeaderTemplate>
            <table id="supply-orders" class="table table-striped">
                <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>ID</th>
                        <th>Requested By</th>
                        <th>Consumer</th>
                        <th>Location</th>
                        <th>Quantity</th>
                        <th>Date Created</th>
                        <th>Status</th>
                        <th class="centered">Completed?</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="command-icon">
                    <asp:HyperLink ID="lnkDetails" runat="server" ToolTip="Click to view details of the Order"><i class="fa fa-search"></i></asp:HyperLink>
                </td>
                <td><%#Eval("OrderID") %></td>
                <td><%#Eval("RequestedName") %></td>
                <td><%#Eval("ConsumerName") %></td>
                <td><%#Eval("Location") %></td>
                <td><%#Eval("Quantity") %></td>
                <td><%#Eval("DateCreated") %></td>
                <td><%#Eval("Status") %></td>
                <td class="centered">
                    <asp:LinkButton runat="server" ID="cmdComplete" CommandName="toggle" CommandArgument='<%#Eval("OrderID") %>'><%#"<i class='fa fa-"  + (Eval("CompletedDate")==null ? "check-square": "square") + "'><i/>"  %></asp:LinkButton></td>
                <td class="command-icon">
                    <asp:LinkButton runat="server" ID="cmdDelted" CssClass="confirm " CommandName="delete" CommandArgument='<%#Eval("OrderID") %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    jQuery(function ($) {
        $('#supply-orders .confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        $(".datepicker").datepicker();
    });
</script>
