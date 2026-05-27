<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewStamp.ascx.cs" Inherits="tjc.Modules.Purchasing.ViewStamp" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
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
                <td><a href='mailto:<%#Eval("EmailAddress") %>'><%#Eval("RequestedName") %></a></td>
                <td><%#Eval("ConsumerName") %></td>
                <td><%#Eval("Location") %></td>
                <td><%#Eval("Quantity") %></td>
                <td><%#Eval("DateCreated") %></td>
                <td><%#Eval("Status") %></td>
                <td class="centered">
                    <asp:LinkButton runat="server" ID="cmdComplete" CommandName="toggle" CommandArgument='<%#Eval("OrderID") %>'><%#"<i class='fa fa-"  + (Eval("CompletedDate")==null ? "square": "check-square") + "'><i/>"  %></asp:LinkButton></td>
                <td class="command-icon">
                    <asp:LinkButton runat="server" ID="cmdDelted" CssClass="text-danger confirm " CommandName="delete" CommandArgument='<%#Eval("OrderID") %>'><i class="fas fa-trash"></i></asp:LinkButton></td>
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
        $('#supply-orders .confirm').not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
            e.preventDefault();
            var href = this.href || '';
            Swal.fire({
                title: 'Delete Record?', text: 'Are you Sure you wish to delete this record?', icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                confirmButtonColor: '#d33'
            }).then(function (r) {
                if (r.isConfirmed) {
                    var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
                    if (m && typeof __doPostBack === 'function') __doPostBack(m[1], m[2]);
                }
            });
        });
        $(".datepicker").datepicker();
    });
</script>
