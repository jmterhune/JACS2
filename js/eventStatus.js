let eventStatusControllerInstance = null;

class EventStatusController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.eventStatusId = -1;
        this.searchTerm = "";
        this.eventStatusTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        this.updateUrl = null;
        this.createUrl = null;
        this.viewUrl = null;
        eventStatusControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}EventStatusAPI/DeleteEventStatus/`;
        this.updateUrl = `${this.service.baseUrl}EventStatusAPI/UpdateEventStatus`;
        this.createUrl = `${this.service.baseUrl}EventStatusAPI/CreateEventStatus`;
        this.viewUrl = `${this.service.baseUrl}EventStatusAPI/GetEventStatus/`;

        const listUrl = `${this.service.baseUrl}EventStatusAPI/GetEventStatuses/${this.recordCount}`;
        const detailModalElement = document.getElementById('EventStatusDetailModal');
        const editModalElement = document.getElementById('EventStatusEditModal');
        const editModal = new bootstrap.Modal(document.getElementById('EventStatusEditModal'));

        if (detailModalElement) detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        if (editModalElement) editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);

        this.eventStatusTable = $('#tblEventStatus').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            paging: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                dataSrc: "data",
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: function (error) {
                    $("#tblEventStatus_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve event statuses.';
                    if (error.status === 401) errorMessage = 'Please make sure you are logged in and try again.';
                    ShowNotification('Error Retrieving Event Statuses', errorMessage, 'error');
                }
            },
            columns: [
                { data: "id", render: data => `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="es-detail btn-command"><i class="fas fa-eye"></i></button>`, className: "command-item", orderable: false },
                { data: "id", render: data => `<button type="button" title="Edit Event Status" data-toggle="tooltip" data-id="${data}" class="es-edit btn-command"><i class="fas fa-pencil"></i></button>`, className: "command-item", orderable: false },
                { data: "name", render: data => data || '' },
                { data: "id", render: (data, type, row) => isAdmin ? `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Event Status" data-id="${row.id}"><i class="fas fa-trash"></i></button>` : '', className: "command-item", orderable: false }
            ],
            language: { emptyTable: "No Records Available.", zeroRecords: "No records match the search criteria you entered." },
            order: [[2, 'asc']],
            serverSide: false,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: 25,
        });

        this.eventStatusTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const eventStatusId = $(this).data("id");
                Swal.fire({ title: 'Delete Event Status?', text: 'Are you sure?', icon: 'warning', showCancelButton: true }).then((result) => {
                    if (result.isConfirmed) eventStatusControllerInstance.deleteEventStatus(eventStatusId);
                });
            });
        });

        $(".dt-length").prepend($("#lnkAdd"));

        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        $(document).on('click', '.es-detail', function (e) {
            e.preventDefault();
            var eventStatusId = $(this).data("id");
            eventStatusControllerInstance.viewEventStatus(eventStatusId, false);
        });

        $(document).on('click', '.es-edit, #editEventStatusBtn', function (e) {
            e.preventDefault();
            var eventStatusId = $(this).data("id") || $("#hdEventStatusId").val();
            eventStatusControllerInstance.eventStatusId = eventStatusId;
            if (eventStatusId) {
                eventStatusControllerInstance.viewEventStatus(eventStatusId, true);
                $("#EventStatusEditModalLabel").html(`Edit Event Status`);
            } else {
                eventStatusControllerInstance.clearEditForm();
                $("#EventStatusEditModalLabel").html("Create New Event Status");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            eventStatusControllerInstance.clearEditForm();
            $("#EventStatusEditModalLabel").html("Create New Event Status");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var eventStatusId = $("#hdEventStatusId").val();
            Swal.fire({
                title: 'Delete Event Status?',
                text: 'Are you sure you wish to delete this Event Status?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    eventStatusControllerInstance.deleteEventStatus(eventStatusId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;
            const $esName = $("#edit_esName");
            const $esNameError = $esName.next(".invalid-feedback");
            if ($esName.val().trim() === "") {
                $esNameError.show();
                $esName.addClass("is-invalid");
                isValid = false;
            } else {
                $esNameError.hide();
                $esName.removeClass("is-invalid");
            }
            if (isValid) eventStatusControllerInstance.saveEventStatus();
        });

        $("#edit_esName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    viewEventStatus(eventStatusId, isEditMode = false) {
        const progressId = isEditMode ? "#edit_progress-eventstatus" : "#progress-eventstatus";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('EventStatusDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (eventStatusId) {
            $.ajax({
                url: this.viewUrl + eventStatusId,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdEventStatusId").val(response.data.id);
                            $("#edit_esName").val(response.data.name);
                            $("#EventStatusEditModalLabel").html(`Edit Event Status: ${response.data.name}`);
                        } else {
                            $("#esName").html(response.data.name);
                            $("#hdEventStatusId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve event status details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Event Status Details", error.statusText || "Failed to retrieve event status details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    saveEventStatus() {
        if ($("#edit_hdEventStatusId").val() === "") {
            this.createEventStatus();
        } else {
            this.updateEventStatus();
        }
    }

    createEventStatus() {
        try {
            $("#edit_progress-eventstatus").show();
            const eventStatusData = { name: $("#edit_esName").val().trim() };
            $.ajax({
                url: this.createUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(eventStatusData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-eventstatus").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Event Status created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusEditModal'));
                        if (editModal) editModal.hide();
                        if (eventStatusControllerInstance.eventStatusTable) {
                            eventStatusControllerInstance.eventStatusTable.ajax.reload();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating event status.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-eventstatus").hide();
                    ShowNotification("Error Creating Event Status", error.statusText || "Failed to create event status.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-eventstatus").hide();
            ShowNotification("Error Creating Event Status", e.message, 'error');
        }
    }

    updateEventStatus() {
        try {
            $("#edit_progress-eventstatus").show();
            const eventStatusData = {
                id: parseInt($("#edit_hdEventStatusId").val()),
                name: $("#edit_esName").val().trim()
            };
            $.ajax({
                url: this.updateUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(eventStatusData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-eventstatus").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Event Status updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusEditModal'));
                        if (editModal) editModal.hide();
                        if (eventStatusControllerInstance.eventStatusTable) {
                            eventStatusControllerInstance.eventStatusTable.ajax.reload();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating event status.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-eventstatus").hide();
                    ShowNotification("Error Updating Event Status", error.statusText || "Failed to update event status.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-eventstatus").hide();
            ShowNotification("Error Updating Event Status", e.message, 'error');
        }
    }

    deleteEventStatus(eventStatusId) {
        $.ajax({
            url: this.deleteUrl + eventStatusId,
            type: 'DELETE',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusEditModal'));
                    if (editModal) editModal.hide();
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusDetailModal'));
                    if (detailModal) detailModal.hide();
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Event Status deleted successfully.'
                    });
                    if (eventStatusControllerInstance.eventStatusTable) {
                        eventStatusControllerInstance.eventStatusTable.ajax.reload();
                    }
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Event Status", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'EventStatusDetailModal') {
            eventStatusControllerInstance.clearDetailForm();
        } else if (modalId === 'EventStatusEditModal') {
            eventStatusControllerInstance.clearEditForm();
            eventStatusControllerInstance.clearEditValidations();
        }
    }

    clearDetailForm() {
        $("#esName").html("");
        $("#hdEventStatusId").val("");
    }

    clearEditForm() {
        $("#edit_esName").val("");
        $("#edit_hdEventStatusId").val("");
    }

    clearEditValidations() {
        $("#edit_esName").removeClass("is-invalid");
        $("#edit_esName").next(".invalid-feedback").hide();
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}