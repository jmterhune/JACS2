let courtExtendControllerInstance = null;
class CourtExtendController {
    constructor(params = {}) {
        this.moduleId = params.moduleId;
        this.userId = params.userId;
        this.isAdmin = params.isAdmin;
        this.adminRole = params.adminRole;
        this.service = params.service;
        this.courtId = params.courtId;
        this.cancelUrl = params.cancelUrl;
        courtExtendControllerInstance = this;
    }
    init() {
        // Initialize datepicker
        $('#txtStartDate').datepicker({
            autoclose: true,
            format: 'mm/dd/yyyy'
        });

        // Handle cancel button
        $('#btnCancel').click(function (e) {
            e.preventDefault();
            window.location.href = this.cancelUrl;
        });

        // Form submission
        $('#extendForm').submit(function (e) {
            e.preventDefault();
            submitExtendForm();
        });

    }

    submitExtendForm() {
        var startTemplate = $('#ddlStartTemplate').val();
        var weeks = $('#txtWeeks').val();
        var startDate = $('#txtStartDate').val();
        var courtId = $('#hfCourtId').val();

        if (!startTemplate || !weeks || !startDate) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'All fields are required.'
            });
            return false;
        }

        if (weeks <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Weeks to extend must be greater than 0.'
            });
            return false;
        }

        $('#btnExtend').prop('disabled', true).find('i').removeClass('fas fa-save').addClass('fas fa-spinner fa-spin');
        const getUrl = `${this.service.baseUrl}CourtAPI/ExtendCalendar`;
        $.ajax({
            url: getUrl,
            method: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            data: {
                courtId: courtId,
                startDate: startDate,
                weeks: weeks,
                startTemplate: startTemplate
            },
            success: function (response) {
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
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: xhr.responseJSON?.message || 'An error occurred while extending the calendar.'
                });
            },
            complete: function () {
                $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas fa-save');
            }
        });
        return false; // Prevent default form submission
    }
}