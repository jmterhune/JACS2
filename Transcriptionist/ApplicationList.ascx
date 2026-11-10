<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationList.ascx.cs" Inherits="tjc.Modules.Transcriptionist.ApplicationList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnncssinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.js" />
<div class="container-fluid dnn-admin-list">
    <div class="d-flex justify-content-end mt-3">
        <asp:HyperLink ID="lnkAdd" runat="server" CssClass="btn btn-primary">
            <i class="fa fa-plus me-2" aria-hidden="true"></i>Add Applicant
        </asp:HyperLink>
    </div>
    <!-- Search Filters -->
    <div class="row mt-2 mb-3">
        <div class="col-md-3">
            <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" Placeholder="Search by Name" />
        </div>
        <div class="col-md-3">
            <asp:TextBox ID="txtSearchCounty" runat="server" CssClass="form-control" Placeholder="Search by County" />
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlSearchStatus" runat="server" CssClass="form-control">
                <asp:ListItem Text="All Statuses" Value="" />
                <asp:ListItem Text="Pending" Value="Pending" />
                <asp:ListItem Text="Approved" Value="Approved" />
                <asp:ListItem Text="Rejected" Value="Rejected" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
        </div>
    </div>

    <div class="row">
        <div class="col-12">
            <asp:GridView ID="gvAllApplications" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-hover">
                <Columns>
                    <asp:TemplateField HeaderText="&nbsp;" HeaderStyle-CssClass="command-col" ItemStyle-CssClass="command-item">
                        <ItemTemplate>
                            <a href="<%# GetViewUrl(Eval("TranscriptionistID").ToString()) %>" title="Review Application" aria-label="Review Application"><i class="fas fa-search"></i></a>
                            <a href="<%# GetEditUrl(Eval("TranscriptionistID").ToString()) %>" title="Edit Record" aria-label="Edit Record"><i class="fas fa-edit"></i></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TranscriptionistID" HeaderText="ID" HeaderStyle-CssClass="id-col" ItemStyle-CssClass="id-col" />
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <%# FormatName(Eval("LastName"), Eval("FirstName")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmailAddress" HeaderText="Email" />
                    <asp:BoundField DataField="CreatedDate" HeaderText="Submitted" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:BoundField DataField="County" HeaderText="County" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge <%# GetStatusBadgeClass(Eval("Status").ToString()) %>'><%# Eval("Status") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:HyperLink runat="server" ID="lnkDashboard" CssClass="btn btn-outline-secondary mt-3">
                Back to Dashboard
            </asp:HyperLink>
            <asp:HyperLink ID="lnkExternal" runat="server" Visible="false" Target="_blank" rel="noopener noreferrer" CssClass="btn btn-outline-primary mt-3">
                <i class="fa fa-external-link me-2" aria-hidden="true"></i>Public Transcriptionist List
            </asp:HyperLink>
        </div>
    </div>
</div>
<script>
    $(document).ready(function () {
        $(".page-top-info h1").html("All Transcriptionist Applications");

        var $table = $('#gvAllApplications');
        if ($table.length && !$.fn.dataTable.isDataTable('#gvAllApplications')) {
            // ASP.NET's GridView renders its header <tr> (of <th>s) as the first row of
            // <tbody> — there's no real <thead>. DataTables' auto-detection of the header
            // depends on a genuine <thead>, and without one it can miscount columns while
            // scanning <th>/<td> across the whole table (the "Incorrect column count"
            // warning / "Cannot read properties of undefined (reading 'mData')" crash).
            // Promote that first row into an actual <thead> so DataTables sees the
            // standard structure it expects.
            if ($table.find('thead').length === 0) {
                var $headerRow = $table.find('tbody tr').first();
                if ($headerRow.length && $headerRow.find('th').length) {
                    $table.prepend('<thead></thead>');
                    $table.find('thead').append($headerRow);
                }
            }

            $table.DataTable({
                order: [[4, 'desc']],
                columnDefs: [
                    { targets: 0, orderable: false, searchable: false }
                ],
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                oLanguage: { sSearch: "Filter by Text" }
            });
        }
    });
</script>
