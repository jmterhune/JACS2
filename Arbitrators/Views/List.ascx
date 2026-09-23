<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="tjc.Modules.Arbitrators.Views.List" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<div class="container-fluid arbitrators-list">
    <h3><i class="fas fa-gavel"></i>&nbsp;Arbitrator Applications</h3>

    <div class="bg-dark text-white border-dark rounded p-2 mb-2">
        <div class="row">
            <div class="col-auto me-2 pt-2"><strong>Filter By:</strong></div>
            <div class="col-auto">
                <select id="drpStatusFilter" class="form-control">
                    <option value="">Every Status</option>
                    <option value="Pending" selected="selected">Pending</option>
                    <option value="Approved">Approved</option>
                    <option value="Denied">Denied</option>
                    <option value="Removed">Removed</option>
                </select>
            </div>
        </div>
    </div>

    <asp:Repeater ID="rptApplications" runat="server">
        <HeaderTemplate>
            <table id="tblApplications" class="table table-striped table-bordered table-hover">
                <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Submitted</th>
                        <th>Reviewed</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="command-item">
                    <a href='<%# DetailUrl((int)Eval("ArbitratorApplicationID")) %>' title="Review Application"><i class="fas fa-search"></i></a>
                </td>
                <td><%# Eval("ArbitratorApplicationID") %></td>
                <td><a href='<%# DetailUrl((int)Eval("ArbitratorApplicationID")) %>'><%# Server.HtmlEncode(Convert.ToString(Eval("FullName"))) %></a></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Email"))) %></td>
                <td data-order='<%# Eval("CreatedOnDate", "{0:yyyyMMddHHmmss}") %>'><%# Eval("CreatedOnDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("ReviewedOnDate", "{0:MM/dd/yyyy}") %></td>
                <td data-status="<%# Eval("Status") %>"><span class="badge <%# StatusBadgeClass(Eval("Status")) %>"><%# Eval("Status") %></span></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    (function ($) {
        $(document).ready(function () {
            var table = $('#tblApplications').DataTable({
                order: [[1, 'desc']],
                columnDefs: [{ targets: 0, orderable: false }]
            });

            $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
                var wanted = $('#drpStatusFilter').val();
                if (!wanted) return true;
                var row = table.row(dataIndex).node();
                return $(row).find('td[data-status]').data('status') === wanted;
            });

            $('#drpStatusFilter').on('change', function () { table.draw(); });
            table.draw();
        });
    }(jQuery));
</script>
