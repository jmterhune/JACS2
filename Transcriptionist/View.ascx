<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.Transcriptionist.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnncssinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.js" />

<div class="container-fluid dnn-admin-dashboard">
    <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
        <h2 class="mb-0">Transcriptionist Admin Dashboard</h2>
        <asp:HyperLink ID="lnkExternal" runat="server" Visible="false" Target="_blank" rel="noopener noreferrer" CssClass="btn btn-outline-primary">
            <i class="fa fa-external-link me-2" aria-hidden="true"></i>Public Application Page
        </asp:HyperLink>
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
                    <p class="card-text  display-4"><asp:Literal ID="litPending" runat="server" /></p>
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
                    <h5 class="card-title">Rejected</h5>
                    <p class="card-text text-light display-4"><asp:Literal ID="litRejected" runat="server" /></p>
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
                                <th>County</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Server.HtmlEncode(Convert.ToString(Eval("FirstName"))) %> <%# Server.HtmlEncode(Convert.ToString(Eval("LastName"))) %></td>
                        <td><%# Eval("CreatedDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Server.HtmlEncode(Convert.ToString(Eval("County"))) %></td>
                        <td>
                            <a href="<%# GetEditUrl(Container.DataItem) %>" class="btn btn-sm btn-primary">Edit</a>
                            <a href="<%# GetViewUrl(Container.DataItem) %>" class="btn btn-sm btn-info">View</a>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:HyperLink runat="server" ID="lnkList" CssClass="btn btn-outline-primary mt-3">
                View All Applications →
            </asp:HyperLink>
        </div>
    </div>
</div>
<script>
    $(document).ready(function () {
        if ($('#pendingApplicationsTable tbody tr').length && !$.fn.dataTable.isDataTable('#pendingApplicationsTable')) {
            $('#pendingApplicationsTable').DataTable({
                order: [[1, 'desc']],
                columnDefs: [
                    { targets: 3, orderable: false, searchable: false }
                ],
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                oLanguage: { sSearch: "Filter by Text" }
            });
        }
    });
</script>
