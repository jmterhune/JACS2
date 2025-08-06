let dashboardControllerInstance = null;
class DashboardController {
    constructor(options) {
        this.moduleId = options.moduleId || -1;
        this.service = options.service || null;
        this.eventEditUrl = options.eventEditUrl;
        this.eventRevisionUrl = options.eventRevisionUrl;
        this.userId = options.userId || -1;
        this.isJudge = options.isJudge || false;
        dashboardControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const baseUrl = this.service.baseUrl;
        this.initEventsTable(baseUrl);
        this.initTimeslotTable(baseUrl);
        this.bindEvents();
    }
    initEventsTable(baseUrl) {
        const listUrl = `${baseUrl}EventAPI/GetDashsboardEvents`;
        this.table = $("#tblEvent").DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                data: data => ({
                    userId: this.userId,
                    isJudge: this.isJudge,
                }),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: (xhr, status, error) => {
                    ShowNotification("Error Retrieving Events", error || "Failed to load events.", 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: data => `<a href="${this.editUrl}/eid/${data}" class="btn-command revisions"><i class="fas fa-pencil"></i></a>`,
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                {
                    data: "CaseNumber", orderable: false,
                    searchable: false
                },
                {
                    data: "Motion", orderable: false,
                    searchable: false
                },
                {
                    data: "Timeslot", orderable: false,
                    searchable: false
                },
                {
                    data: "Duration", orderable: false,
                    searchable: false
                },
                {
                    data: "Court", orderable: false,
                    searchable: false
                },
                {
                    data: "Status", orderable: false,
                    searchable: false
                },
                {
                    data: "Attorney", orderable: false,
                    searchable: false
                },
                {
                    data: "OpposingAttorney", orderable: false,
                    searchable: false
                },
                {
                    data: "Plaintiff", orderable: false,
                    searchable: false
                },
                {
                    data: "Defendant", orderable: false,
                    searchable: false
                },
                {
                    data: "Category", orderable: false,
                    searchable: false
                },
            ],
            info: false,
            responsive: true,
            paging: false,
            ordering: false,
            searching: false,
            
        });
    }

    initTimeslotTable(baseUrl) {
        const listUrl = `${baseUrl}TimeslotAPI/GetDashboardTimeslots`;
        this.table = $("#tblTimeslot").DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                data: data => ({
                    userId: this.userId,
                    isJudge: this.isJudge,
                }),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: (xhr, status, error) => {
                    ShowNotification("Error Retrieving Timeslots", error || "Failed to load timeslots.", 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: data => `<a href="${this.editUrl}/sid/${data}" class="btn-command edit-timeslot"><i class="fas fa-pencil"></i></a>`,
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                {
                    data: "court_name",
                    orderable: false,
                    searchable: false
                },
                {
                    data: "FormattedStart", orderable: false,
                    searchable: false
                },
                {
                    data: "duration", orderable: false,
                    searchable: false
                },
                {
                    data: "available", orderable: false,
                    searchable: false, render: function (data) { return data ? 'Yes' : 'No' }
                },
                {
                    data: "quantity", orderable: false,
                    searchable: false
                },
            ],
            info: false,
            responsive: true,
            paging: false,
            ordering: false,
            searching: false,
        });
    }

    bindEvents() {
        $("#search-button").on("click", () => {
            this.searchCaseNumber();
            return false;
        });
    }

    searchCaseNumber() {
        const caseNum = { searchTerm: $("#case_num").val() };
        const searchUrl = `${this.service.baseUrl}EventAPI/SearchCaseNumber`;
        $.ajax({
            url: searchUrl,
            method: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(caseNum),
            success: response => {
                if (!response.data) {
                    alert("No Event found with this case number!");
                    return;
                }
                const data = response.data;
                const caseDetails = '<tbody>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Case Number:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.CaseNum + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Motion:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Motion ? data.Motion.Description : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Timeslot:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.Timeslot.Date + ' @ ' + data.Timeslot.StartTime + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Court:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.Timeslot.Court.Description + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Status:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Status ? data.Status.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Attorney:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Attorney ? data.Attorney.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Opposing Attorney:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.OppAttorney ? data.OppAttorney.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Plaintiff:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Plaintiff ? data.Plaintiff : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Defendant:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Defendant ? data.Defendant : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Category:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Timeslot.Category ? data.Timeslot.Category.Description : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Actions:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' +
                    '<a href="' + this.eventEditUrl + '/eid/' + data.Id + '" class="btn btn-sm btn-link"><i class="la la-edit"></i> Edit</a>' +
                    '<a href="javascript:void(0)" onclick="dashboardControllerInstance.cancelEntry(' + data.Id + ')" class="btn btn-sm btn-link" data-button-type="delete"><i class="la la-trash"></i> Cancel</a>' +
                    '<a href="' + this.eventRevisionUrl + '/eid/' + data.Id + '" class="btn btn-sm btn-link"><i class="la la-history"></i> Revisions</a>' +
                    '</td></tr>' +
                    '</tbody>';
                $("#caseDetailsTable").empty().append(caseDetails);
                $("#caseSearchModal").modal('show');
            },
            error: () => {
                alert("No Event found with this case number!");
            }
        });
    }

    cancelEntry(eventId) {
        const cancelUrl = `${this.service.baseUrl}EventAPI/CancelEvent`;
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            input: 'textarea',
            inputLabel: 'Cancellation Reason',
            inputPlaceholder: 'Type your message here...',
            inputAttributes: { 'aria-label': 'Type your message here' },
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, cancel it!',
            preConfirm: (value) => {
                if (!value) {
                    Swal.showValidationMessage('<i class="fa fa-info-circle"></i> Cancellation reason is required!');
                }
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: cancelUrl,
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ eventId: eventId, reason: result.value }),
                    dataType: 'json',
                    success: () => {
                        Swal.fire('Cancelled!', 'Your hearing has now been cancelled.', 'success').then(() => {
                            window.location.href = '/';
                        });
                    },
                    error: () => {
                        Swal.fire({ icon: 'error', title: 'Oops...', text: 'Something went wrong!' });
                    }
                });
            }
        });
    }
    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }

}