<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationDetail.ascx.cs" Inherits="tjc.Modules.Transcriptionist.ApplicationDetail" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnncssinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/resources/libraries/DataTables/dataTables.bootstrap5.min.js" />
<%-- SweetAlert2 for the approve / reject confirmations --%>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />

<div id="TranscriptionistDetail" class="container-fluid">
    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="alert alert-danger">
        Unable to find the requested application. <asp:HyperLink runat="server" ID="lnkNotFoundBack" Text="Return to List" />
    </asp:Panel>

    <asp:Panel ID="pnlApplication" runat="server">
        <asp:Literal ID="ltHeading" runat="server" />

        <asp:Panel ID="pnlAlreadyReviewed" runat="server" Visible="false" CssClass="alert alert-info">
            This application is not pending, so it cannot be approved or rejected here. To change the record or its status, use Edit.
        </asp:Panel>

        <asp:Panel ID="pnlErrors" runat="server" Visible="false" CssClass="alert alert-danger" role="alert">
            <asp:Literal ID="ltErrors" runat="server" />
        </asp:Panel>

        <!-- Personal Information -->
        <h3 class="h4">Personal Information</h3>
        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label class="detail-label">First Name</label>
                <p class="detail-value"><asp:Literal ID="ltFirstName" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Middle Name</label>
                <p class="detail-value"><asp:Literal ID="ltMiddleName" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Last Name</label>
                <p class="detail-value"><asp:Literal ID="ltLastName" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12 col-lg-6">
                <label class="detail-label">Home Address</label>
                <p class="detail-value"><asp:Literal ID="ltHomeAddress" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-6">
                <label class="detail-label">County</label>
                <p class="detail-value"><asp:Literal ID="ltCounty" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label class="detail-label">Email Address</label>
                <p class="detail-value"><asp:HyperLink ID="lnkEmail" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Residence Phone</label>
                <p class="detail-value"><asp:Literal ID="ltResidencePhone" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Cell Phone</label>
                <p class="detail-value"><asp:Literal ID="ltCellPhone" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label class="detail-label">Employer</label>
                <p class="detail-value"><asp:Literal ID="ltEmployer" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Office Phone</label>
                <p class="detail-value"><asp:Literal ID="ltOfficePhone" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Employer Address</label>
                <p class="detail-value"><asp:Literal ID="ltEmployerAddress" runat="server" /></p>
            </div>
        </div>
        <hr />

        <!-- Qualifications -->
        <h3 class="h4">Qualifications</h3>
        <div class="row mb-3">
            <div class="col-12">
                <label class="detail-label">Transcription Training</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltTranscriptionTraining" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12">
                <label class="detail-label">Certifications</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltCertifications" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12 col-lg-6">
                <label class="detail-label">Experience</label>
                <p class="detail-value"><asp:Literal ID="ltMonthsExperience" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-6">
                <label class="detail-label">Turn Around Time</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltTurnAroundTime" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12 col-lg-6">
                <label class="detail-label">Software Experience</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltSoftwareExperience" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-6">
                <label class="detail-label">Rule Familiarity</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltRuleFamiliarity" runat="server" /></p>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12">
                <asp:CheckBox ID="chkJacsVendor" runat="server" Text="JACS Vendor" CssClass="form-check form-check-inline readonly-checkbox" />
            </div>
        </div>
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
        <asp:Label ID="lblNoExperience" runat="server" Text="No experience records." Visible="false" CssClass="d-block text-muted mb-3" />
        <hr />

        <!-- Documents -->
        <h3 class="h4">Documents</h3>
        <asp:Repeater ID="rptAttachments" runat="server">
            <HeaderTemplate><ul class="list-group mb-2"></HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item"><%# GetFileLink(Container.DataItem) %> <small class="text-muted">(uploaded <%# Eval("UploadDate", "{0:MM/dd/yyyy}") %>)</small></li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoAttachments" runat="server" Text="No attachments." Visible="false" CssClass="d-block text-muted mb-2" />
        <asp:Label ID="lblNoExternalUrl" runat="server" Visible="false" CssClass="d-block text-muted small mb-2"
            Text="Set the public Transcriptionist list in the module settings to turn these file names into links." />
        <hr />

        <!-- Status -->
        <h3 class="h4">Status</h3>
        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label class="detail-label">Submitted</label>
                <p class="detail-value"><asp:Literal ID="ltSubmitted" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label class="detail-label">Status</label>
                <p class="detail-value"><asp:Literal ID="ltStatus" runat="server" /></p>
            </div>
        </div>
        <asp:Panel ID="pnlRejectionReason" runat="server" Visible="false" CssClass="row mb-3">
            <div class="col-12">
                <label class="detail-label">Rejection Reason</label>
                <p class="detail-value white-space-pre"><asp:Literal ID="ltRejectionReason" runat="server" /></p>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlReview" runat="server">
            <hr />
            <div class="row mb-3">
                <div class="col-12">
                    <label for="<%=txtReviewNotes.ClientID %>">Review Notes <small class="text-muted">(included in the applicant's notification email; required to reject, and saved as the rejection reason)</small></label>
                    <asp:TextBox ID="txtReviewNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="4000" />
                </div>
            </div>
        </asp:Panel>

        <div>
            <asp:LinkButton ID="cmdApprove" runat="server" CssClass="btn btn-success" Text="Approve" OnClientClick="return Jud12ConfirmPostback(this, 'This will approve the application and email the applicant.', 'Approve Application?');" OnClick="cmdApprove_Click" />
            <asp:LinkButton ID="cmdReject" runat="server" CssClass="btn btn-danger" Text="Reject" OnClientClick="return Jud12ConfirmReject(this);" OnClick="cmdReject_Click" />
            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-primary">Edit</asp:HyperLink>
            <asp:HyperLink ID="lnkList" runat="server" CssClass="btn btn-secondary">Return to List</asp:HyperLink>
        </div>
    </asp:Panel>
</div>

<script type="text/javascript">
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

    // Rejecting needs a reason (it is saved on the record and emailed), so ask for it before confirming.
    function Jud12ConfirmReject(btn) {
        var box = document.getElementById('<%= txtReviewNotes.ClientID %>');
        if (!box || !box.value.trim()) {
            var msg = 'Enter a reason for rejecting this application in the Review Notes box.';
            if (window.Swal) { Swal.fire({ title: 'Reason required', text: msg, icon: 'info' }); }
            else { window.alert(msg); }
            if (box) box.focus();
            return false;
        }
        return Jud12ConfirmPostback(btn, 'This will reject the application and email the applicant the reason.', 'Reject Application?');
    }

    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>
