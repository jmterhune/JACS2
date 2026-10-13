let courtTruncateControllerInstance = null;
class CourtTruncateController {
    constructor(config) {
        this.moduleId = config.moduleId;
        this.userId = config.userId;
        this.isAdmin = config.isAdmin == "True" ? true : false || false;
        this.adminRole = config.adminRole;
        this.service = config.service;
        this.courtId = config.courtId;
        this.cancelUrl = `${config.cancelUrl}/cid/${config.courtId}`;
        courtTruncateControllerInstance = this;
    }

    init() {
        // Initialize datepicker
        $('#txtTruncateDate').datepicker({
            autoclose: true,
            format: 'mm/dd/yyyy'
        });

        // Handle cancel button
        $('#btnCancel').click((e) => {
            e.preventDefault();
            window.location.href = this.cancelUrl;
        });

        // Form submission
        $('#btnTruncate').click((e) => {
            e.preventDefault();
            this.submitTruncateForm();
        });
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
    }

    submitTruncateForm() {
        var courtId = parseInt(this.courtId);
        var truncateDate = $('#txtTruncateDate').val();
        var filter = $('#ddlFilter input[type=radio]:checked').val();

        if (!truncateDate) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Truncate date is required.'
            });
            return false;
        }

        Swal.fire({
            title: 'Are you sure?',
            text: "This will truncate the calendar from the selected date and beyond. This action cannot be undone!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, truncate it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $('#btnTruncate').prop('disabled', true).find('i').removeClass('fas fa-trash').addClass('fas fa-spinner fa-spin');
                $('#btnTruncate').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas fa-trash');
                $.ajax({
                    url: `${this.service.baseUrl}CourtAPI/TruncateTimeslots`,
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        courtId: courtId,
                        startDate: truncateDate,
                        filter: filter
                    }),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: (response) => {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: response.message,
                                confirmButtonText: 'OK'
                            }).then(() => {
                                window.location.href = this.cancelUrl;
                            });
                        } else {
                            // Build a per-event failure list (HTML) when the
                            // server returned any. Hearings that the clerk
                            // refused to cancel stay scheduled, so the user
                            // needs to know exactly which ones to retry.
                            const failures = Array.isArray(response.failures) ? response.failures : [];
                            const escape = s => $('<div>').text(s == null ? '' : s).html();
                            let html = '<p>' + escape(response.message || 'One or more hearings could not be cancelled.') + '</p>';
                            if (failures.length) {
                                html += '<div class="text-start mt-2" style="max-height:300px;overflow:auto;">';
                                html += '<table class="table table-sm table-striped mb-0"><thead><tr>'
                                    + '<th>Case</th><th>Event ID</th><th>Clerk ID</th><th>Error</th>'
                                    + '</tr></thead><tbody>';
                                failures.forEach(f => {
                                    html += '<tr>'
                                        + '<td>' + escape(f.case_num) + '</td>'
                                        + '<td>' + escape(f.event_id) + '</td>'
                                        + '<td>' + escape(f.clerk_event_id) + '</td>'
                                        + '<td>' + escape(f.error) + '</td>'
                                        + '</tr>';
                                });
                                html += '</tbody></table></div>';
                            }
                            Swal.fire({
                                icon: 'error',
                                title: 'Truncate finished with errors',
                                html: html,
                                width: failures.length ? 700 : undefined
                            });
                        }
                    },
                    error: (xhr) => {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: xhr.responseJSON || 'An error occurred while truncating the calendar.'
                        });
                    },
                    complete: () => {
                        $('#btnTruncate').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas fa-trash');
                    }
                });
            }
        });

        return false;
    }
    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}