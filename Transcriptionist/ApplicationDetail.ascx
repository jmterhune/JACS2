<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationDetail.ascx.cs" Inherits="tjc.Modules.Transcriptionist.ApplicationDetail" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnncssinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.js" />
<div class="container-fluid dnn-admin-view">
    <!-- Personal Information -->
    <h3 class="h4">Personal Information</h3>
    <div class="row mb-2">
        <div class="col-md-4"><strong>First Name:</strong> <asp:Label ID="lblFirstName" runat="server" /></div>
        <div class="col-md-4"><strong>Middle Name:</strong> <asp:Label ID="lblMiddleName" runat="server" /></div>
        <div class="col-md-4"><strong>Last Name:</strong> <asp:Label ID="lblLastName" runat="server" /></div>
    </div>
    <div class="row mb-2">
        <div class="col-md-6"><strong>Home Address:</strong> <asp:Label ID="lblHomeAddress" runat="server" /></div>
        <div class="col-md-6"><strong>County:</strong> <asp:Label ID="lblCounty" runat="server" /></div>
    </div>
    <div class="row mb-2">
        <div class="col-md-4"><strong>Email Address:</strong> <asp:Label ID="lblEmailAddress" runat="server" /></div>
        <div class="col-md-4"><strong>Residence Phone:</strong> <asp:Label ID="lblResidencePhone" runat="server" /></div>
        <div class="col-md-4"><strong>Cell Phone:</strong> <asp:Label ID="lblCellPhone" runat="server" /></div>
    </div>
    <div class="row mb-2">
        <div class="col-md-4"><strong>Employer:</strong> <asp:Label ID="lblEmployer" runat="server" /></div>
        <div class="col-md-4"><strong>Office Phone:</strong> <asp:Label ID="lblOfficePhone" runat="server" /></div>
        <div class="col-md-4"><strong>Employer Address:</strong> <asp:Label ID="lblEmployerAddress" runat="server" /></div>
    </div>
    <hr />
    <!-- Qualifications -->
    <h3 class="h4">Qualifications</h3>
    <div class="mb-3"><strong>Transcription Training:</strong> <asp:Label ID="lblTranscriptionTraining" runat="server" /></div>
    <div class="mb-3"><strong>Certifications:</strong> <asp:Label ID="lblCertifications" runat="server" /></div>
    <div class="mb-3"><strong>Experience:</strong> <asp:Label ID="lblMonthsExperience" runat="server" /></div>
    <div class="mb-3"><strong>Software Experience:</strong> <asp:Label ID="lblSoftwareExperience" runat="server" /></div>
    <div class="mb-3"><strong>Turn Around Time:</strong> <asp:Label ID="lblTurnAroundTime" runat="server" /></div>
    <div class="mb-3"><strong>Rule Familiarity:</strong> <asp:Label ID="lblRuleFamiliarity" runat="server" /></div>
    <div class="mb-3"><strong>JACS Vendor:</strong> <asp:Label ID="lblJacsVendor" runat="server" /></div>
    <hr />
    <!-- Experience -->
    <h3 class="h4">Experience</h3>
    <asp:Repeater ID="rptExperiences" runat="server">
        <HeaderTemplate>
            <table id="experienceHistoryTable" class="table table-striped">
                <thead>
                    <tr>
                        <th>Court Name</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th>Proceeding Types</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("CourtName"))) %></td>
                <td><%# Eval("StartDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("EndDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("ProceedingTypes"))) %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <hr />
    <!-- Documents -->
    <h3 class="h4">Documents</h3>
    <asp:Repeater ID="rptAttachments" runat="server">
        <ItemTemplate>
            <div><%# GetFileLink(Container.DataItem) %> (Uploaded: <%# Eval("UploadDate", "{0:MM/dd/yyyy}") %>)</div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoAttachments" runat="server" Text="No attachments." Visible="false" CssClass="text-muted" />
    <asp:Label ID="lblNoExternalUrl" runat="server" Visible="false" CssClass="d-block text-muted small mt-1"
        Text="Set the public application page in the module settings to turn these file names into links." />
    <hr />
    <!-- Status -->
    <h3 class="h4">Status</h3>
    <div><strong>Created:</strong> <asp:Label ID="lblCreatedDate" runat="server" /></div>
    <div><strong>Status:</strong> <asp:Label ID="lblStatus" runat="server" /></div>
    <div id="pnlRejectionReason" runat="server" visible="false"><strong>Rejection Reason:</strong> <asp:Label ID="lblRejectionReason" runat="server" /></div>

    <!-- Actions -->
    <div class="mt-4">
        <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-primary">Edit</asp:HyperLink>
        <asp:HyperLink ID="lnkList" runat="server" CssClass="btn btn-outline-secondary">Back to List</asp:HyperLink>
    </div>
</div>
<script>
    $(document).ready(function () {
        $(".page-top-info h1").html("View Transcriptionist Application");

        if ($('#experienceHistoryTable tbody tr').length && !$.fn.dataTable.isDataTable('#experienceHistoryTable')) {
            $('#experienceHistoryTable').DataTable({
                order: [[1, 'desc']],
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                oLanguage: { sSearch: "Filter by Text" }
            });
        }
    });
</script>
