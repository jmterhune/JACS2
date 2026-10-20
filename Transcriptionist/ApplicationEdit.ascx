<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationEdit.ascx.cs" Inherits="tjc.Modules.Transcriptionist.ApplicationEdit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />
<dnn:dnnjsinclude runat="server" filepath="~/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:dnnjsinclude runat="server" filepath="~/Resources/Libraries/noty/noty.min.js" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Libraries/noty/noty.min.css" />

<asp:UpdatePanel ID="pnlForm" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <div class="mb-3">
            <span>Status: </span>
            <asp:Label ID="lblStatusBadge" runat="server" CssClass="badge bg-secondary" />
        </div>
        <div class="alert alert-warning" id="pnlRejectionReason" runat="server" visible="false">
            <strong>Rejection Reason:</strong> <asp:Label ID="lblRejectionReason" runat="server" />
        </div>

        <!-- Personal Information -->
        <h3>Personal Information</h3>
        <div class="row mb-2">
            <div class="col-md-4">
                <label>First Name <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtFirstName" MaxLength="50" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-4">
                <label>Middle Name / Initial</label>
                <asp:TextBox ID="txtMiddleName" MaxLength="50" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-4">
                <label>Last Name <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtLastName" MaxLength="50" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-2">
            <div class="col-md-6">
                <label>Home Address <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtHomeAddress" TextMode="MultiLine" Rows="3" MaxLength="250" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label>County <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtCounty" MaxLength="50" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-2">
            <div class="col-md-4">
                <label>Email Address <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtEmailAddress" MaxLength="100" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmailAddress" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="label label-danger" ErrorMessage="Invalid Email" Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label>Residence Phone <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtResidencePhone" MaxLength="50" runat="server" CssClass="form-control phone-mask" />
            </div>
            <div class="col-md-4">
                <label>Cell Phone <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtCellPhone" MaxLength="50" runat="server" CssClass="form-control phone-mask" />
            </div>
        </div>
        <div class="row mb-2">
            <div class="col-md-4">
                <label>Employer</label>
                <asp:TextBox ID="txtEmployer" MaxLength="100" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-4">
                <label>Office Phone</label>
                <asp:TextBox ID="txtOfficePhone" MaxLength="50" runat="server" CssClass="form-control phone-mask" />
            </div>
            <div class="col-md-4">
                <label>Employer Address</label>
                <asp:TextBox ID="txtEmployerAddress" MaxLength="250" TextMode="MultiLine" Rows="3" runat="server" CssClass="form-control" />
            </div>
        </div>

        <!-- Qualifications -->
        <h3>Qualifications</h3>
        <div class="mb-3">
            <label>Transcription Training (School, Program, etc.) <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtTranscriptionTraining" TextMode="MultiLine" MaxLength="4000" runat="server" CssClass="form-control character-limit" data-max="4000" Rows="3" />
            <small class="form-text text-muted counter">4000 characters left</small>
        </div>
        <div class="mb-3">
            <label>Certifications (if any)</label>
            <asp:TextBox ID="txtCertifications" TextMode="MultiLine" MaxLength="4000" runat="server" CssClass="form-control character-limit" data-max="4000" Rows="3" />
            <small class="form-text text-muted counter">4000 characters left</small>
        </div>
        <div class="row mb-2">
            <div class="col-md-6">
                <label>Experience (Years)</label>
                <asp:TextBox ID="txtYearsExperience" MaxLength="3" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-6">
                <label>Experience (Months)</label>
                <asp:TextBox ID="txtMonthsExperience" MaxLength="2" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
        </div>
        <asp:CustomValidator ID="valExperience" runat="server" OnServerValidate="valExperience_ServerValidate" CssClass="label label-danger" ErrorMessage="Please enter experience in years or months." Display="Dynamic" />
        <div class="mb-3">
            <label>Software Experience <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtSoftwareExperience" TextMode="MultiLine" MaxLength="2000" runat="server" CssClass="form-control character-limit" data-max="2000" Rows="3" />
            <small class="form-text text-muted counter">2000 characters left</small>
        </div>
        <div class="mb-3">
            <label>Turn Around Time <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtTurnAroundTime" TextMode="MultiLine" MaxLength="2000" runat="server" CssClass="form-control character-limit" data-max="2000" Rows="3" />
            <small class="form-text text-muted counter">2000 characters left</small>
        </div>
        <div class="mb-3">
            <label>Rule Familiarity <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtRuleFamiliarity" TextMode="MultiLine" MaxLength="2000" runat="server" CssClass="form-control character-limit" data-max="2000" Rows="3" />
            <small class="form-text text-muted counter">2000 characters left</small>
        </div>
        <div class="form-check mb-3">
            <asp:CheckBox ID="chkJacsVendor" runat="server" Text="JACS Vendor" />
        </div>

        <!-- Experience -->
        <h3>Experience</h3>
        <button type="button" class="btn btn-secondary mb-3" onclick="OpenExperienceModal(0, '', '', '', '')">
            <i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Experience
        </button>

        <ul class="list-group mb-3" id="experienceList"></ul>
        <div class="text-muted" id="noExperienceMsg" style="display:none;">No experience records added yet.</div>

        <div class="modal fade" id="ExperienceModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="ExperienceModalLabel">Add Experience</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="experienceModalError" class="alert alert-danger d-none"></div>
                        <input type="hidden" id="hdExperienceId" value="0" />
                        <div class="mb-3">
                            <label>Court Name <span class="text-danger">*</span></label>
                            <input type="text" id="txtCourtName" maxlength="50" class="form-control" />
                        </div>
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label>Start Date</label>
                                <input type="text" id="txtStartDate" class="form-control datepicker" />
                            </div>
                            <div class="col-md-6">
                                <label>End Date</label>
                                <input type="text" id="txtEndDate" class="form-control datepicker" />
                            </div>
                        </div>
                        <div class="mb-3">
                            <label>Types of Proceedings</label>
                            <textarea id="txtProceedingTypes" rows="2" maxlength="4000" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Cancel</button>
                        <button type="button" id="cmdSaveExperience" class="btn btn-primary">Save</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Documents -->
        <h3>Documents</h3>
        <asp:Repeater ID="rptAttachments" runat="server">
            <HeaderTemplate><ul class="list-group mb-2"></HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item"><%# GetFileLink(Container.DataItem) %> (uploaded <%# Eval("UploadDate", "{0:MM/dd/yyyy}") %>)</li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoAttachments" runat="server" Text="No files uploaded." Visible="false" CssClass="d-block text-muted mb-2" />
        <div class="text-muted small">Documents are uploaded and removed by the applicant on the public site.</div>

        <!-- Actions -->
        <div class="mt-4">
            <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CausesValidation="false" CssClass="btn btn-success" OnClick="btnApprove_Click" OnClientClick="confirmApproveApplication(); return false;" Visible="false" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" CausesValidation="false" CssClass="btn btn-warning" OnClick="btnReject_Click" OnClientClick="confirmRejectApplication(); return false;" Visible="false" />
            <asp:Button ID="btnDelete" runat="server" Text="Delete Application" CausesValidation="false" CssClass="btn btn-danger" OnClick="btnDelete_Click" OnClientClick="confirmDeleteApplication(); return false;" />
            <asp:HyperLink runat="server" ID="lnkList" CssClass="btn btn-outline-secondary">Cancel</asp:HyperLink>
            <asp:HiddenField ID="hdRejectionReason" runat="server" ClientIDMode="Static" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    var apiUrl = '<%= ResolveUrl("Handlers/ExperienceApi.ashx") %>';
    var moduleId = <%= ModuleId %>;
    var tabId = <%= TabId %>;
    var transcriptionistId = <%= TranscriptionistID %>;

    (function ($) {
        $('.phone-mask').mask('(000) 000-0000');
        $('.datepicker').datepicker();
        $('.datepicker').mask('00/00/0000');
        $('.character-limit').on('keyup', function () {
            var len = $(this).val().length;
            var max = parseInt($(this).data('max'), 10) || 4000;
            $(this).nextAll('.counter').eq(0).text((max - len) + ' characters left');
        });

        $(document).ready(function () {
            $(".page-top-info h1").html("Edit Transcriptionist Application");
            refreshExperienceList();
        });

        // Experience: Save (guarded against double submission)
        $("#cmdSaveExperience").on('click', function () {
            var $btn = $(this);
            if ($btn.data('saving')) return;

            var courtName = $('#txtCourtName').val().trim();
            var startDate = $('#txtStartDate').val().trim();
            var endDate = $('#txtEndDate').val().trim();
            var proceedingTypes = $('#txtProceedingTypes').val().trim();
            var experienceId = $('#hdExperienceId').val();

            if (!courtName || !startDate || !endDate) {
                $('#experienceModalError').removeClass('d-none').text('Court Name, Start Date, and End Date are required.');
                return;
            }
            $('#experienceModalError').addClass('d-none');

            $btn.data('saving', true).prop('disabled', true).text('Saving...');

            $.ajax({
                url: apiUrl,
                type: 'POST',
                dataType: 'json',
                data: {
                    action: 'saveExperience', mid: moduleId, tabid: tabId, tid: transcriptionistId,
                    experienceId: experienceId, courtName: courtName,
                    startDate: startDate, endDate: endDate, proceedingTypes: proceedingTypes
                }
            }).done(function (resp) {
                if (resp && resp.success) {
                    bootstrap.Modal.getOrCreateInstance(document.getElementById('ExperienceModal')).hide();
                    refreshExperienceList();
                } else {
                    $('#experienceModalError').removeClass('d-none').text((resp && resp.error) || 'Save failed.');
                }
            }).fail(function () {
                $('#experienceModalError').removeClass('d-none').text('Save failed. Please try again.');
            }).always(function () {
                $btn.data('saving', false).prop('disabled', false).text('Save');
            });
        });
    })(jQuery);

    // Populates and opens the Experience modal for both Add (id=0, blank fields) and
    // Edit (id + the row's current values, read from the cached list).
    function OpenExperienceModal(id, courtName, startDate, endDate, proceedingTypes) {
        document.getElementById('hdExperienceId').value = id;
        document.getElementById('txtCourtName').value = courtName;
        document.getElementById('txtStartDate').value = startDate;
        document.getElementById('txtEndDate').value = endDate;
        document.getElementById('txtProceedingTypes').value = proceedingTypes;
        document.getElementById('ExperienceModalLabel').textContent = id > 0 ? 'Edit Experience' : 'Add Experience';
        jQuery('#experienceModalError').addClass('d-none');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('ExperienceModal')).show();
    }

    // ────────────────────────────────────────────────
    // Experience list (AJAX)
    // ────────────────────────────────────────────────
    function refreshExperienceList() {
        jQuery.ajax({
            url: apiUrl,
            type: 'GET',
            dataType: 'json',
            data: { action: 'listExperience', mid: moduleId, tabid: tabId, tid: transcriptionistId }
        }).done(function (resp) {
            if (resp && resp.success) renderExperienceList(resp.items || []);
        });
    }

    function renderExperienceList(items) {
        var $ = jQuery;
        var $list = $('#experienceList').empty();
        $('#noExperienceMsg').toggle(items.length === 0);

        items.forEach(function (item) {
            var $li = $('<li class="list-group-item d-flex justify-content-between align-items-center"></li>');
            var $span = $('<span></span>');
            $('<strong></strong>').text(item.courtName).appendTo($span);
            $span.append(document.createTextNode(' (' + item.startDate + ' - ' + item.endDate + ') ' + item.proceedingTypes));

            var $actions = $('<span class="text-nowrap"></span>');
            $('<button type="button" class="btn btn-sm btn-secondary me-1"><i class="fa fa-pencil" aria-hidden="true"></i></button>')
                .on('click', function () {
                    OpenExperienceModal(item.experienceId, item.courtName, item.startDate, item.endDate, item.proceedingTypes);
                })
                .appendTo($actions);
            $('<button type="button" class="btn btn-sm btn-danger"><i class="fa fa-trash" aria-hidden="true"></i></button>')
                .on('click', function () { deleteExperience(item.experienceId); })
                .appendTo($actions);

            $li.append($span).append($actions);
            $list.append($li);
        });
    }

    function deleteExperience(id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "This experience record will be permanently deleted.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (!result.isConfirmed) return;
            jQuery.ajax({
                url: apiUrl,
                type: 'POST',
                dataType: 'json',
                data: { action: 'deleteExperience', mid: moduleId, tabid: tabId, tid: transcriptionistId, experienceId: id }
            }).done(function (resp) {
                if (resp && resp.success) {
                    refreshExperienceList();
                    ShowNotification('Deleted', 'Experience record deleted.', 'success');
                } else {
                    ShowNotification('Error', (resp && resp.error) || 'Delete failed.', 'error');
                }
            }).fail(function () {
                ShowNotification('Error', 'Delete failed. Please try again.', 'error');
            });
        });
    }

    // Delete Application button: gate the server postback behind a Swal confirm
    // instead of the native confirm() the browser would otherwise block on.
    function confirmDeleteApplication() {
        Swal.fire({
            title: 'Are you sure?',
            text: "This will permanently delete the application and its experience records. The uploaded files stay on the public site.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                __doPostBack('<%= btnDelete.UniqueID %>', '');
            }
        });
    }

    // Reject Application button: capture a required reason via Swal, stash it in a
    // hidden field, then gate the server postback behind confirmation.
    function confirmRejectApplication() {
        Swal.fire({
            title: 'Reject Application',
            text: 'Please provide a reason for rejecting this application.',
            input: 'textarea',
            inputPlaceholder: 'Reason for rejection...',
            inputAttributes: { 'aria-label': 'Rejection reason' },
            showCancelButton: true,
            confirmButtonText: 'Reject Application',
            confirmButtonColor: '#d33',
            preConfirm: (value) => {
                if (!value || !value.trim()) {
                    Swal.showValidationMessage('A rejection reason is required.');
                }
                return value;
            }
        }).then((result) => {
            if (result.isConfirmed) {
                document.getElementById('hdRejectionReason').value = result.value;
                __doPostBack('<%= btnReject.UniqueID %>', '');
            }
        });
    }

    // Approve Application button: confirm before gating the server postback.
    function confirmApproveApplication() {
        Swal.fire({
            title: 'Approve Application?',
            text: 'This will approve the application and notify the applicant by email.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#198754',
            confirmButtonText: 'Yes, approve it!'
        }).then((result) => {
            if (result.isConfirmed) {
                __doPostBack('<%= btnApprove.UniqueID %>', '');
            }
        });
    }

    // ────────────────────────────────────────────────
    // Noty toast helper (matches JacsAtty's ShowNotification convention)
    // ────────────────────────────────────────────────
    function ShowNotification(title, message, type) {
        new Noty({
            type: type.toLowerCase(),
            text: '<strong>' + title + '</strong><br>' + message,
            timeout: 5000,
            theme: 'bootstrap-v4',
            layout: 'topRight',
            progressBar: true
        }).show();
    }
</script>
