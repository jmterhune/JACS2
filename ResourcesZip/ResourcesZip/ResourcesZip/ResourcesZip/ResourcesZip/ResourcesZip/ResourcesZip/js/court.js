// court.js
let courtControllerInstance = null;
class CourtController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin === "True" || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.viewUrl = params.viewUrl || '/';
        this.editUrl = params.editUrl || '/';
        this.revisionUrl = params.revisionUrl || '/';
        this.calendarUrl = params.calendarUrl || '/';
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.courtId = -1;
        this.searchTerm = "";
        this.courtTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtAPI/DeleteCourt/`;
        if (this.isAdmin) {
            this.userId = 0; // Admins can view all timeslots, so set userId to 0
        }
        const listUrl = `${this.service.baseUrl}CourtAPI/GetCourts/${this.recordCount}`;
        const editUrl = this.editUrl;
        const calendarUrl = this.calendarUrl;
        const revisionUrl = this.revisionUrl;
        this.courtTable = $('#tblCourt').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                data: function (data) {
                    data.userId = courtControllerInstance.userId;
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: function (error) {
                    $("#tblCourt_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Courts", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Courts", "The following error occurred attempting to retrieve court information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<a href="${calendarUrl}/cid/${data}" title="View Calendar" data-toggle="tooltip" class="court-detail btn-command"><i class="fa-solid fa-calendar-days"></i></a>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<a href="${editUrl}/cid/${data}" title="Edit Court" data-toggle="tooltip" class="court-edit btn-command"><i class="fas fa-pencil"></i></a>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "description",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "judge_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "county_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (row.has_revisions === true) {
                            return `<a href="${revisionUrl}/cid/${data}" title="Revisions" data-toggle="tooltip" class="revisions btn-command"><i class="fa-solid fa-clock-rotate-left"></i></a>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },

                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === true) {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Court" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[this.sortColumnIndex, this.sortDirection]],
            serverSide: true,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: this.pageSize,
            displayStart: this.currentPage * this.pageSize,
        });
        $(".dt-length").prepend($("#lnkAdd"));
        this.courtTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Court?',
                    html: 'Are you sure you wish to delete this Court? <p class="mt-2">All associated templates, timeslots, and events will be removed and the judge assigned to this court will be updated.</p>',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtControllerInstance.DeleteCourt(courtId);
                    }
                });
            });
        });
        $('[data-toggle="tooltip"]').tooltip();
    }

    DeleteCourt(courtId) {
        $.ajax({
            url: this.deleteUrl + courtId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (result) {
                if (courtControllerInstance.courtTable) {
                    courtControllerInstance.courtTable.draw();
                }
                window.location.href = courtControllerInstance.viewUrl;
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Court deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Court", error.statusText, 'error');
            }
        });
    }

    ClearDataTableState() {
        if (this.courtTable) {
            this.courtTable.state.clear();
            window.location.reload();
        }
    }
    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }

}