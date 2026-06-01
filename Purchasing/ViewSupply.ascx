<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSupply.ascx.cs" Inherits="tjc.Modules.Purchasing.ViewSupply" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div id="stamp-container" class="purchasing">
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
                        <th>ID</th>
                        <th>Requested By</th>
                        <th>Location</th>
                        <th>Date Requested</th>
                        <th class="centered">Completed?</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:HyperLink ID="lnkDetails" runat="server" ToolTip="Click to view details of the Order"><%#Eval("OrderID") %></asp:HyperLink></td>
                <td><a href='mailto:<%#Eval("EmailAddress") %>'><%#Eval("RequestedName") %></a></td>
                <td><%#Eval("Location") %></td>
                <td><%#Eval("DateRequested") %></td>
                <td class="centered">
                    <asp:LinkButton runat="server" ID="cmdComplete" CssClass="item-link" CommandName="toggle" CommandArgument='<%#Eval("OrderID") %>'><%#"<i class='fa fa-"  + (Eval("CompletedDate")==null ? "square": "check-square") + "'><i/>"  %></asp:LinkButton></td>
                <td class="command-icon">
                    <asp:LinkButton runat="server" ID="cmdDelted" CssClass="confirm item-link" CommandName="delete" CommandArgument='<%#Eval("OrderID") %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                </td>
            </tr>
            <tr class="border-0">
                <td colspan="7" class="border-0">
                    <div class="bg-light bg-gradient rounded p-2 ms-5 border border-dark">
                        <asp:Repeater ID="rptOrderItems" runat="server" DataSource='<%# Eval("SupplyOrderItems") %>'>
                            <HeaderTemplate>
                                <table id="supply-order-lines" class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>Item #</th>
                                            <th>Store</th>
                                            <th>Description</th>
                                            <th>Qty</th>
                                            <th>Units of Measure</th>
                                            <th>End User</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("ItemNumber") %></td>
                                    <td><%#Eval("Store") %></td>
                                    <td><%#Eval("LinkedDescription") %></td>
                                    <td><%#Eval("Quantity") %></td>
                                    <td><%#Eval("UnitOfMeasure") %></td>
                                    <td><%#Eval("Recipient") %></td>
                                    <td><%#Eval("ToolTipComment") %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    jQuery(function ($) {
        $('#supply-orders .confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        $(".datepicker").datepicker();
        $('[data-toggle="tooltip"]').tooltip();

    });
</script>
