// eventType.js
let eventTypeControllerInstance = null;

class EventTypeController {
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
        this.eventTypeId = -1;
        this.searchTerm = "";
        this.eventTypeTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        eventTypeControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}EventTypeAPI/DeleteEventType/`;

        const listUrl = `${this.service.baseUrl}EventTypeAPI/GetEventTypes/${this.recordCount}`;
        const detailModalElement = document.getElementById('EventTypeDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('EventTypeEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.eventTypeTable = $('#tblEventType').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data(data) {
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblEventType_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Event Types", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Event Types", "The following error occurred attempting to retrieve event type information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="et-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Event Type" data-toggle="tooltip" data-id="${data}" class="et-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Event Type" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.eventTypeTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const eventTypeId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Event Type?',
                    text: 'Are you sure you wish to delete this Event Type?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        eventTypeControllerInstance.DeleteEventType(eventTypeId);
                    }
                });
            });
        });

        $(document).on('click', '.et-detail', function (e) {
            e.preventDefault();
            var eventTypeId = $(this).data("id");
            eventTypeControllerInstance.ViewEventType(eventTypeId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('EventTypeEditModal'));
        $(document).on('click', '.et-edit, #editEventTypeBtn', function (e) {
            e.preventDefault();
            var eventTypeId = $(this).data("id") || $("#hdEventTypeId").val();
            eventTypeControllerInstance.eventTypeId = eventTypeId;
            if (eventTypeId) {
                eventTypeControllerInstance.ViewEventType(eventTypeId, true);
                $("#EventTypeEditModalLabel").html(`Edit Event Type`);
            } else {
                eventTypeControllerInstance.ClearEditForm();
                $("#EventTypeEditModalLabel").html("Create New Event Type");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            eventTypeControllerInstance.ClearEditForm();
            $("#EventTypeEditModalLabel").html("Create New Event Type");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var eventTypeId = $("#hdEventTypeId").val();
            Swal.fire({
                title: 'Delete Event Type?',
                text: 'Are you sure you wish to delete this Event Type?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    eventTypeControllerInstance.DeleteEventType(eventTypeId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $etName = $("#edit_etName");
            const $etNameError = $etName.next(".invalid-feedback");
            if ($etName.val().trim() === "") {
                $etNameError.show();
                $etName.addClass("is-invalid");
                isValid = false;
            } else {
                $etNameError.hide();
                $etName.removeClass("is-invalid");
            }

            if (isValid) {
                eventTypeControllerInstance.SaveEventType();
            }
        });

        $("#edit_etName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.eventTypeTable) {
            this.eventTypeTable.state.clear();
            window.location.reload();
        }
    }

    DeleteEventType(eventTypeId) {
        $.ajax({
            url: this.deleteUrl + eventTypeId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (eventTypeControllerInstance.eventTypeTable) {
                        eventTypeControllerInstance.eventTypeTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('EventTypeEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('EventTypeDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Event Type deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Event Type", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'EventTypeDetailModal') {
            eventTypeControllerInstance.ClearDetailForm();
        } else if (modalId === 'EventTypeEditModal') {
            eventTypeControllerInstance.ClearEditForm();
            eventTypeControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#etName").html("");
        $("#hdEventTypeId").val("");
    }

    ClearEditForm() {
        $("#edit_etName").val("");
        $("#edit_hdEventTypeId").val("");
    }

    ClearEditValidations() {
        $("#edit_etName").removeClass("is-invalid");
        $("#edit_etName").next(".invalid-feedback").hide();
    }

    ViewEventType(eventTypeId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}EventTypeAPI/GetEventType/${eventTypeId}`;
        const progressId = isEditMode ? "#edit_progress-eventtype" : "#progress-eventtype";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('EventTypeDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (eventTypeId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdEventTypeId").val(response.data.id);
                            $("#edit_etName").val(response.data.name);
                            $("#EventTypeEditModalLabel").html(`Edit Event Type: ${response.data.name}`);
                        } else {
                            $("#etName").html(response.data.name);
                            $("#hdEventTypeId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve event type details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Event Type Details", error.statusText || "Failed to retrieve event type details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveEventType() {
        if ($("#edit_hdEventTypeId").val() === "") {
            this.CreateEventType();
        } else {
            this.UpdateEventType();
        }
        if (eventTypeControllerInstance.eventTypeTable) {
            eventTypeControllerInstance.ClearEditForm();
            eventTypeControllerInstance.eventTypeTable.draw();
        }
    }

    CreateEventType() {
        try {
            $("#edit_progress-eventtype").show();
            const eventTypeData = {
                name: $("#edit_etName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}EventTypeAPI/CreateEventType`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(eventTypeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-eventtype").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Event Type created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('EventTypeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (eventTypeControllerInstance.eventTypeTable) {
                            eventTypeControllerInstance.eventTypeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating event type.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-eventtype").hide();
                    ShowNotification("Error Creating Event Type", error.statusText || "Failed to create event type.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-eventtype").hide();
            ShowNotification("Error Creating Event Type", e.message, 'error');
        }
    }

    UpdateEventType() {
        try {
            $("#edit_progress-eventtype").show();
            const eventTypeData = {
                id: parseInt($("#edit_hdEventTypeId").val()),
                name: $("#edit_etName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}EventTypeAPI/UpdateEventType`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(eventTypeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-eventtype").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Event Type updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('EventTypeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (eventTypeControllerInstance.eventTypeTable) {
                            eventTypeControllerInstance.eventTypeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating event type.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-eventtype").hide();
                    ShowNotification("Error Updating Event Type", error.statusText || "Failed to update event type.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-eventtype").hide();
            ShowNotification("Error Updating Event Type", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}