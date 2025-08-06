let courtExtendControllerInstance = null;
class CourtExtendController {
    constructor(params = {}) {
        this.moduleId = config.moduleId;
        this.userId = config.userId;
        this.isAdmin = config.isAdmin;
        this.adminRole = config.adminRole;
        this.service = config.service;
        this.courtId = config.courtId;
        this.cancelUrl = config.cancelUrl;
        courtExtendControllerInstance = this;
    }
    int() {
        // Initialize datepicker
        $('#<%= txtStartDate.ClientID %>').datepicker({
            autoclose: true,
            format: 'mm/dd/yyyy'
        });

        // Handle cancel button
        $('#btnCancel').click(function (e) {
            e.preventDefault();
            window.location.href = config.cancelUrl;
        });

        // Form submission
        $('#extendForm').submit(function (e) {
            e.preventDefault();
            submitExtendForm();
        });

    }

    submitExtendForm() {
        var startTemplate = $('#<%= ddlStartTemplate.ClientID %>').val();
        var weeks = $('#<%= txtWeeks.ClientID %>').val();
        var startDate = $('#<%= txtStartDate.ClientID %>').val();
        var courtId = $('#<%= hfCourtId.ClientID %>').val();

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

        $('#<%= btnExtend.ClientID %>').prop('disabled', true).find('i').removeClass('fas fa-save').addClass('fas fa-spinner fa-spin');
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
                        window.location.href = config.cancelUrl;
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
                $('#<%= btnExtend.ClientID %>').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas fa-save');
            }
        });
        return false; // Prevent default form submission
    }
}