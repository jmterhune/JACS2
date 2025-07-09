let courtTypeControllerInstance = null;

class CourtTypeController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.courtTypeId = -1;
        this.searchTerm = "";
        this.courtTypeTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtTypeControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtTypeAPI/DeleteCourtType/`;

        const listUrl = `${this.service.baseUrl}CourtTypeAPI/GetCourtTypes/${this.recordCount}`;
        const detailModalElement = document.getElementById('CourtTypeDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CourtTypeEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.courtTypeTable = $('#tblCourtType').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                data(data) {
                    data.searchText = data.search.value;
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblCourtType_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Court Types", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Court Types", "The following error occurred attempting to retrieve court type information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="courtType-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Court Type" data-toggle="tooltip" data-id="${data}" class="courtType-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Court Type" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.courtTypeTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtTypeId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Court Type?',
                    text: 'Are you sure you wish to delete this Court Type?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtTypeControllerInstance.DeleteCourtType(courtTypeId);
                    }
                });
            });
        });

        $(document).on('click', '.courtType-detail', function (e) {
            e.preventDefault();
            var courtTypeId = $(this).data("id");
            courtTypeControllerInstance.ViewCourtType(courtTypeId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtTypeEditModal'));
        $(document).on('click', '.courtType-edit, #editCourtTypeBtn', function (e) {
            e.preventDefault();
            var courtTypeId = $(this).data("id") || $("#hdCourtTypeId").val();
            courtTypeControllerInstance.courtTypeId = courtTypeId;
            if (courtTypeId) {
                courtTypeControllerInstance.ViewCourtType(courtTypeId, true);
                $("#CourtTypeEditModalLabel").html(`Edit Court Type`);
            } else {
                courtTypeControllerInstance.ClearEditForm();
                $("#CourtTypeEditModalLabel").html("Create New Court Type");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtTypeControllerInstance.ClearEditForm();
            $("#CourtTypeEditModalLabel").html("Create New Court Type");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var courtTypeId = $("#hdCourtTypeId").val();
            Swal.fire({
                title: 'Delete Court Type?',
                text: 'Are you sure you wish to delete this Court Type?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtTypeControllerInstance.DeleteCourtType(courtTypeId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $courtTypeDescription = $("#edit_courtTypeDescription");
            const $courtTypeDescriptionError = $courtTypeDescription.next(".invalid-feedback");
            if ($courtTypeDescription.val().trim() === "") {
                $courtTypeDescriptionError.show();
                $courtTypeDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $courtTypeDescriptionError.hide();
                $courtTypeDescription.removeClass("is-invalid");
            }

            if (isValid) {
                courtTypeControllerInstance.SaveCourtType();
            }
        });

        $("#edit_courtTypeDescription").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.courtTypeTable) {
            this.courtTypeTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtType(courtTypeId) {
        $.ajax({
            url: this.deleteUrl + courtTypeId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (courtTypeControllerInstance.courtTypeTable) {
                    courtTypeControllerInstance.courtTypeTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTypeEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('CourtTypeDetailModal'));
                if (detailModal) {
                    detailModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Court Type deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Court Type", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CourtTypeDetailModal') {
            courtTypeControllerInstance.ClearDetailForm();
        } else if (modalId === 'CourtTypeEditModal') {
            courtTypeControllerInstance.ClearEditForm();
            courtTypeControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#courtTypeDescription").html("");
        $("#hdCourtTypeId").val("");
    }

    ClearEditForm() {
        $("#edit_courtTypeDescription").val("");
        $("#edit_hdCourtTypeId").val("");
    }

    ClearEditValidations() {
        $("#edit_courtTypeDescription").removeClass("is-invalid");
        $("#edit_courtTypeDescription").next(".invalid-feedback").hide();
    }

    ViewCourtType(courtTypeId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtTypeAPI/GetCourtType/${courtTypeId}`;
        const progressId = isEditMode ? "#edit_progress-courtType" : "#progress-courtType";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtTypeDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (courtTypeId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCourtTypeId").val(response.data.id);
                            $("#edit_courtTypeDescription").val(response.data.description);
                            $("#CourtTypeEditModalLabel").html(`Edit Court Type: ${response.data.description}`);
                        } else {
                            $("#courtTypeDescription").html(response.data.description);
                            $("#hdCourtTypeId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve court type details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch court type details');
                    ShowNotification("Error", "Failed to retrieve court type details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCourtType() {
        if ($("#edit_hdCourtTypeId").val() === "") {
            this.CreateCourtType();
        } else {
            this.UpdateCourtType();
        }
        if (courtTypeControllerInstance.courtTypeTable) {
            courtTypeControllerInstance.ClearEditForm();
            courtTypeControllerInstance.courtTypeTable.draw();
        }
    }

    CreateCourtType() {
        try {
            $("#edit_progress-courtType").show();
            const courtTypeData = {
                description: $("#edit_courtTypeDescription").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTypeAPI/CreateCourtType`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtTypeData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-courtType").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court Type created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTypeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-courtType").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtType").hide();
                    ShowNotification("Error Creating Court Type", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtType").hide();
            ShowNotification("Error Creating Court Type", e.statusText, 'error');
        }
    }

    UpdateCourtType() {
        try {
            $("#edit_progress-courtType").show();
            const courtTypeData = {
                id: $("#edit_hdCourtTypeId").val(),
                description: $("#edit_courtTypeDescription").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTypeAPI/UpdateCourtType`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtTypeData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-courtType").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court Type updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTypeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-courtType").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtType").hide();
                    ShowNotification("Error Updating Court Type", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtType").hide();
            ShowNotification("Error Updating Court Type", e.statusText, 'error');
        }
    }
}