<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.ascx.cs" Inherits="tjc.Modules.Arbitrators.Views.Dashboard" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<div class="container-fluid arbitrators-dashboard">
    <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
        <h2 class="mb-0">Arbitrator Admin Dashboard</h2>
        <div>
            <% if (CanEditRecords) { %>
            <a href="<%= Server.HtmlEncode(AddArbitratorUrl) %>" class="btn btn-success text-white">
                <i class="fas fa-plus me-2" aria-hidden="true"></i>Add Arbitrator
            </a>
            <% } %>
            <asp:HyperLink ID="lnkExternal" runat="server" Visible="false" Target="_blank" rel="noopener noreferrer" CssClass="btn btn-outline-primary">
                <i class="fas fa-external-link-alt me-2" aria-hidden="true"></i>Public Arbitrators List
            </asp:HyperLink>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-md-3">
            <div class="card text-white bg-primary mb-3">
                <div class="card-body p-3">
                    <h5 class="card-title">Total Applications (This Year)</h5>
                    <p class="card-text text-light display-4"><asp:Literal ID="litTotalApplications" runat="server" /></p>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card bg-warning mb-3">
                <div class="card-body p-3">
                    <h5 class="card-title">Pending Review</h5>
                    <p class="card-text display-4"><asp:Literal ID="litPending" runat="server" /></p>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card text-white bg-success mb-3">
                <div class="card-body p-3">
                    <h5 class="card-title">Approved</h5>
                    <p class="card-text text-light display-4"><asp:Literal ID="litApproved" runat="server" /></p>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card text-white bg-danger mb-3">
                <div class="card-body p-3">
                    <h5 class="card-title">Denied</h5>
                    <p class="card-text text-light display-4"><asp:Literal ID="litDenied" runat="server" /></p>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-12">
            <h3>Recent Pending Applications</h3>
            <asp:Repeater ID="rptPendingApplications" runat="server">
                <HeaderTemplate>
                    <table id="pendingApplicationsTable" class="table table-striped table-hover">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Submitted</th>
                                <th>Preferred Areas</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Server.HtmlEncode(Convert.ToString(Eval("LastFirstName"))) %></td>
                        <td data-order='<%# Eval("CreatedOnDate", "{0:yyyyMMddHHmmss}") %>'><%# Eval("CreatedOnDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Server.HtmlEncode(Convert.ToString(Eval("PreferredAreas"))) %></td>
                        <td>
                            <a href="<%# DetailUrl((int)Eval("ArbitratorApplicationID")) %>" class="btn btn-sm btn-info">Review</a>
                            <% if (CanEditRecords) { %>
                            <a href="<%# EditArbitratorUrl((int)Eval("ArbitratorApplicationID")) %>" class="btn btn-sm btn-primary">Edit</a>
                            <% } %>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="litNoPending" runat="server" Visible="false"><p class="text-muted">No applications are waiting for review.</p></asp:Literal>

            <asp:HyperLink runat="server" ID="lnkList" CssClass="btn btn-outline-primary mt-3">
                View All Applications &rarr;
            </asp:HyperLink>
        </div>
    </div>
</div>
<script type="text/javascript">
    jQuery(document).ready(function ($) {
        if ($('#pendingApplicationsTable tbody tr').length && !$.fn.dataTable.isDataTable('#pendingApplicationsTable')) {
            $('#pendingApplicationsTable').DataTable({
                order: [[1, 'desc']],
                columnDefs: [{ targets: 3, orderable: false, searchable: false }],
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                oLanguage: { sSearch: "Filter by Text" }
            });
        }
    });
</script>
