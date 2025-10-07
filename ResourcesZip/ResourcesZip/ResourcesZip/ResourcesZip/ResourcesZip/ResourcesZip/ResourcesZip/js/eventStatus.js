let eventStatusControllerInstance = null;

class EventStatusController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
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
        eventStatusControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}EventStatusAPI/DeleteEventStatus/`;

        const listUrl = `${this.service.baseUrl}EventStatusAPI/GetEventStatuses/${this.recordCount}`;
        const detailModalElement = document.getElementById('EventStatusDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('EventStatusEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.eventStatusTable = $('#tblEventStatus').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data(data) {
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblEventStatus_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Event Statuses", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Event Statuses", "The following error occurred attempting to retrieve event status information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="es-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Event Status" data-toggle="tooltip" data-id="${data}" class="es-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Event Status" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.eventStatusTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const eventStatusId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Event Status?',
                    text: 'Are you sure you wish to delete this Event Status?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        eventStatusControllerInstance.DeleteEventStatus(eventStatusId);
                    }
                });
            });
        });

        $(document).on('click', '.es-detail', function (e) {
            e.preventDefault();
            var eventStatusId = $(this).data("id");
            eventStatusControllerInstance.ViewEventStatus(eventStatusId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('EventStatusEditModal'));
        $(document).on('click', '.es-edit, #editEventStatusBtn', function (e) {
            e.preventDefault();
            var eventStatusId = $(this).data("id") || $("#hdEventStatusId").val();
            eventStatusControllerInstance.eventStatusId = eventStatusId;
            if (eventStatusId) {
                eventStatusControllerInstance.ViewEventStatus(eventStatusId, true);
                $("#EventStatusEditModalLabel").html(`Edit Event Status`);
            } else {
                eventStatusControllerInstance.ClearEditForm();
                $("#EventStatusEditModalLabel").html("Create New Event Status");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            eventStatusControllerInstance.ClearEditForm();
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
                    eventStatusControllerInstance.DeleteEventStatus(eventStatusId);
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

            if (isValid) {
                eventStatusControllerInstance.SaveEventStatus();
            }
        });

        $("#edit_esName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.eventStatusTable) {
            this.eventStatusTable.state.clear();
            window.location.reload();
        }
    }

    DeleteEventStatus(eventStatusId) {
        $.ajax({
            url: this.deleteUrl + eventStatusId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (eventStatusControllerInstance.eventStatusTable) {
                        eventStatusControllerInstance.eventStatusTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('EventStatusDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Event Status deleted successfully.'
                    });
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
            eventStatusControllerInstance.ClearDetailForm();
        } else if (modalId === 'EventStatusEditModal') {
            eventStatusControllerInstance.ClearEditForm();
            eventStatusControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#esName").html("");
        $("#hdEventStatusId").val("");
    }

    ClearEditForm() {
        $("#edit_esName").val("");
        $("#edit_hdEventStatusId").val("");
    }

    ClearEditValidations() {
        $("#edit_esName").removeClass("is-invalid");
        $("#edit_esName").next(".invalid-feedback").hide();
    }

    ViewEventStatus(eventStatusId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}EventStatusAPI/GetEventStatus/${eventStatusId}`;
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
                url: getUrl,
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

    SaveEventStatus() {
        if ($("#edit_hdEventStatusId").val() === "") {
            this.CreateEventStatus();
        } else {
            this.UpdateEventStatus();
        }
        if (eventStatusControllerInstance.eventStatusTable) {
            eventStatusControllerInstance.ClearEditForm();
            eventStatusControllerInstance.eventStatusTable.draw();
        }
    }

    CreateEventStatus() {
        try {
            $("#edit_progress-eventstatus").show();
            const eventStatusData = {
                name: $("#edit_esName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}EventStatusAPI/CreateEventStatus`,
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
                        if (editModal) {
                            editModal.hide();
                        }
                        if (eventStatusControllerInstance.eventStatusTable) {
                            eventStatusControllerInstance.eventStatusTable.draw();
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

    UpdateEventStatus() {
        try {
            $("#edit_progress-eventstatus").show();
            const eventStatusData = {
                id: parseInt($("#edit_hdEventStatusId").val()),
                name: $("#edit_esName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}EventStatusAPI/UpdateEventStatus`,
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
                        if (editModal) {
                            editModal.hide();
                        }
                        if (eventStatusControllerInstance.eventStatusTable) {
                            eventStatusControllerInstance.eventStatusTable.draw();
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

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}