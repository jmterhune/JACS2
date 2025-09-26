let courtTruncateControllerInstance = null;
class CourtTruncateController {
    constructor(config) {
        this.moduleId = config.moduleId;
        this.userId = config.userId;
        this.isAdmin = config.isAdmin;
        this.adminRole = config.adminRole;
        this.service = config.service;
        this.courtId = config.courtId;
        this.cancelUrl = config.cancelUrl;
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
        $('#truncateForm').submit((e) => {
            e.preventDefault();
            this.submitTruncateForm();
        });
    }

    submitTruncateForm() {
        var truncateDate = $('#txtTruncateDate').val();
        var filter = $('#ddlFilter').val();
        var courtId = $('#hdCourtId').val();

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

                $.ajax({
                    url: `${this.service.baseUrl}CourtAPI/TruncateCalendar`,
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        courtId: courtId,
                        date: truncateDate,
                        filter: filter
                    }),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: (response) => {
                        if (response.status === 200) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: response.message,
                                confirmButtonText: 'OK'
                            }).then(() => {
                                window.location.href = this.cancelUrl;
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: response.message
                            });
                        }
                    },
                    error: (xhr) => {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: xhr.responseJSON?.message || 'An error occurred while truncating the calendar.'
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