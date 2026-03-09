let dashboardControllerInstance = null;

class DashboardController {
    constructor(options) {
        this.moduleId = options.moduleId || -1;
        this.service = options.service || null;
        this.eventEditUrl = options.eventEditUrl;
        this.timeslotEditUrl = options.timeslotEditUrl;
        this.userId = options.userId || -1;
        this.isJudge = options.isJudge == "True";
        dashboardControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const baseUrl = this.service.baseUrl;

        this.initEventsTable(baseUrl);
        this.initTimeslotTable(baseUrl);
        this.initCaseSearchResultsTable();  // New: prepare search results table

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
                data: () => ({
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
                    render: (data, type, row) => row.editable
                        ? `<a href="${this.eventEditUrl}/eid/${data}" class="btn-command revisions"><i title="Select to Edit Record" class="fas fa-pencil"></i></a>`
                        : '<i class="fas fa-ban text-danger" title="You do not have edit permissions for this record"></i>',
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                { data: "case_num", orderable: false, searchable: false },
                { data: "motion_name", orderable: false, searchable: false },
                { data: "timeslot_desc", orderable: false, searchable: false },
                { data: "duration", orderable: false, searchable: false },
                { data: "court_name", orderable: false, searchable: false },
                { data: "status_name", orderable: false, searchable: false },
                { data: "attorney_name", orderable: false, searchable: false },
                { data: "opp_attorney_name", orderable: false, searchable: false },
                { data: "plaintiff", orderable: false, searchable: false },
                { data: "defendant", orderable: false, searchable: false },
                { data: "courtroom_name", orderable: false, searchable: false },
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
                data: () => ({
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
                    render: (data, type, row) => row.editable
                        ? `<a href="${this.timeslotEditUrl}/sid/${data}" class="btn-command edit-timeslot"><i class="fas fa-pencil" title="Select to Edit Record"></i></a>`
                        : '<i class="fas fa-ban text-danger" title="You do not have edit permissions for this record"></i>',
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                { data: "court_name", orderable: false, searchable: false },
                { data: "formatted_start", orderable: false, searchable: false },
                { data: "duration", orderable: false, searchable: false },
                { data: "available", orderable: false, searchable: false, render: data => data ? 'Yes' : 'No' },
                { data: "quantity", orderable: false, searchable: false },
            ],
            info: false,
            responsive: true,
            paging: false,
            ordering: false,
            searching: false,
        });
    }

    initCaseSearchResultsTable() {
        this.resultsTable = $('#tblCaseSearchResults').DataTable({
            paging: true,
            pageLength: 10,
            lengthMenu: [5, 10, 15, 25],
            searching: true,
            ordering: true,
            info: true,
            autoWidth: false,
            responsive: true,
            dom: '<"top"f>rt<"bottom"lip><"clear">',
            language: {
                emptyTable: "No matching cases found.",
                zeroRecords: "No cases match your search.",
                info: "Showing _START_ to _END_ of _TOTAL_ cases",
                infoEmpty: "No cases to show",
                infoFiltered: "(filtered from _MAX_ total)",
                search: "Filter results:"
            },
            columns: [
                { data: "case_num", title: "Case Number" },
                { data: "motion_name", title: "Motion" },
                { data: "timeslot_desc",title:"Timeslot" },
                { data: "court_name", title: "Court" },
                { data: "status_name", title: "Status" },
                { data: "plaintiff", title: "Plaintiff" },
                { data: "defendant", title: "Defendant" },
                {
                    data: "id",
                    title: "Actions",
                    orderable: false,
                    searchable: false,
                    className: "text-end",
                    render: function (data) {
                        return `
                            <button class="btn btn-sm btn-primary edit-event me-2" data-id="${data}">
                                <i class="fas fa-edit me-1"></i>Edit
                            </button>
                            <button class="btn btn-sm btn-danger cancel-event" data-id="${data}">
                                <i class="fas fa-trash me-1"></i>Cancel
                            </button>
                        `;
                    }
                }
            ]
        });

        // Delegated event handlers (safe after redraws)
        $('#tblCaseSearchResults tbody').on('click', '.edit-event', (e) => {
            e.preventDefault();
            const id = $(e.currentTarget).data('id');
            window.location.href = this.eventEditUrl + '/eid/' + id;
        });

        $('#tblCaseSearchResults tbody').on('click', '.cancel-event', (e) => {
            e.preventDefault();
            const id = $(e.currentTarget).data('id');
            this.cancelEntry(id);
        });
    }

    bindEvents() {
        $("#search-button").on("click", (e) => {
            e.preventDefault();
            this.searchCaseNumber();
        });
    }

    searchCaseNumber() {
        const parts = [
            $("#case_num_part1").val().trim().toUpperCase(),
            $("#case_num_part2").val().trim(),
            $("#case_num_part3").val().trim().toUpperCase(),
            $("#case_num_part4").val().trim().padStart(6, '0'),
            $("#case_num_part5").val().trim().toUpperCase(),
            $("#case_num_part6").val().trim().toUpperCase()
        ];

        const patternParts = parts.filter(p => p !== "");
        if (patternParts.length < 2) {
            Swal.fire({
                icon: 'warning',
                title: 'Incomplete Search',
                text: 'Please fill at least the year and case type (or sequence number).'
            });
            return;
        }

        const searchPattern = patternParts.join('-');

        const $btn = $("#search-button");
        $btn.prop("disabled", true)
            .find(".btn-text").addClass("d-none")
            .siblings(".spinner-border").removeClass("d-none");

        const searchUrl = `${this.service.baseUrl}EventAPI/GetEventsByCaseNumber`;

        $.ajax({
            url: searchUrl,
            method: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                casePattern: searchPattern,
                userId: this.userId,
                isJudge: this.isJudge
            }),
            beforeSend: xhr => this.setAjaxHeaders(xhr),

            success: (response) => {
                // ────────────────────────────────────────────────
                // Handle different possible response structures
                // ────────────────────────────────────────────────
                let results = [];

                if (response && response.success === false) {
                    // API explicitly said no success
                    Swal.fire({
                        icon: 'info',
                        title: 'No Results',
                        text: response.message || 'No matching cases were found.'
                    });
                    return;
                }

                if (Array.isArray(response)) {
                    results = response;
                } else if (response?.data) {
                    results = Array.isArray(response.data) ? response.data : [response.data];
                } else if (response?.results) {
                    results = Array.isArray(response.results) ? response.results : [];
                }

                if (results.length === 0) {
                    Swal.fire({
                        icon: 'info',
                        title: 'No Results Found',
                        text: `No cases match the case number: "${searchPattern}"`,
                        confirmButtonText: 'OK'
                    });
                    return;
                }

                // We have results → show them
                this.showMultipleResults(results);
            },

            error: (xhr, status, error) => {
                let errorMsg = 'An unexpected error occurred.';

                if (xhr.status === 401 || xhr.status === 403) {
                    errorMsg = 'You are not authorized to perform this search. Please log in again.';
                } else if (xhr.responseJSON?.message) {
                    errorMsg = xhr.responseJSON.message;
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Search Failed',
                    html: errorMsg
                });
            },

            complete: () => {
                $btn.prop("disabled", false)
                    .find(".btn-text").removeClass("d-none")
                    .siblings(".spinner-border").addClass("d-none");
            }
        });
    }

    showMultipleResults(data) {
        const $modal = $("#caseSearchModal");
        const $title = $("#CaseSearchModalLabel");

        // Clear previous data and load new rows
        this.resultsTable.clear();
        this.resultsTable.rows.add(data);
        this.resultsTable.draw();

        // Update modal title with count
        $title.text(`Search Results ${data.length ? `(${data.length})` : ''}`);

        // Show the modal
        const modal = new bootstrap.Modal($modal[0]);
        modal.show();
    }

    cancelEntry(eventId) {
        const cancelUrl = `${this.service.baseUrl}EventAPI/CancelEvent`;

        Swal.fire({
            title: 'Are you sure?',
            text: "This hearing will be cancelled and cannot be undone without admin intervention.",
            icon: 'warning',
            input: 'textarea',
            inputLabel: 'Cancellation Reason (required)',
            inputPlaceholder: 'Enter reason here...',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, Cancel Hearing',
            preConfirm: (value) => {
                if (!value?.trim()) {
                    Swal.showValidationMessage('Cancellation reason is required');
                    return false;
                }
                return value.trim();
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: cancelUrl,
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ eventId: eventId, reason: result.value }),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: () => {
                        Swal.fire('Cancelled', 'The hearing has been cancelled.', 'success')
                            .then(() => {
                                // Optional: refresh dashboard or re-run search
                                // this.searchCaseNumber();
                                location.reload();
                            });
                    },
                    error: () => {
                        Swal.fire('Error', 'Failed to cancel the hearing. Please try again.', 'error');
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