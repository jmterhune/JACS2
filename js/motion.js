let motionControllerInstance = null;

class MotionController {
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
        this.motionId = -1;
        this.searchTerm = "";
        this.motionTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        motionControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}MotionAPI/DeleteMotion/`;

        const listUrl = `${this.service.baseUrl}MotionAPI/GetMotions/${this.recordCount}`;
        const detailModalElement = document.getElementById('MotionDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('MotionEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.motionTable = $('#tblMotion').DataTable({
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
                    $("#tblMotion_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve motions.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification('Error Retrieving Motions', errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="motion-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Motion" data-toggle="tooltip" data-id="${data}" class="motion-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                    data: "lag",
                    render: function (data) {
                        return data != null ? data : '';
                    }
                },
                {
                    data: "lead",
                    render: function (data) {
                        return data != null ? data : '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Motion" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.motionTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const motionId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Motion?',
                    text: 'Are you sure you wish to delete this Motion?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        motionControllerInstance.DeleteMotion(motionId);
                    }
                });
            });
        });

        $(document).on('click', '.motion-detail', function (e) {
            e.preventDefault();
            var motionId = $(this).data("id");
            motionControllerInstance.ViewMotion(motionId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('MotionEditModal'));
        $(document).on('click', '.motion-edit, #editMotionBtn', function (e) {
            e.preventDefault();
            var motionId = $(this).data("id") || $("#hdMotionId").val();
            motionControllerInstance.motionId = motionId;
            if (motionId) {
                motionControllerInstance.ViewMotion(motionId, true);
                $("#MotionEditModalLabel").html(`Edit Motion`);
            } else {
                motionControllerInstance.ClearEditForm();
                $("#MotionEditModalLabel").html("Create New Motion");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            motionControllerInstance.ClearEditForm();
            $("#MotionEditModalLabel").html("Create New Motion");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var motionId = $("#hdMotionId").val();
            Swal.fire({
                title: 'Delete Motion?',
                text: 'Are you sure you wish to delete this Motion?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    motionControllerInstance.DeleteMotion(motionId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $motionDescription = $("#edit_motionDescription");
            const $motionDescriptionError = $motionDescription.next(".invalid-feedback");
            if ($motionDescription.val().trim() === "") {
                $motionDescriptionError.show();
                $motionDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $motionDescriptionError.hide();
                $motionDescription.removeClass("is-invalid");
            }

            if (isValid) {
                motionControllerInstance.SaveMotion();
            }
        });

        $("#edit_motionDescription").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.motionTable) {
            this.motionTable.state.clear();
            window.location.reload();
        }
    }

    DeleteMotion(motionId) {
        $.ajax({
            url: this.deleteUrl + motionId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (motionControllerInstance.motionTable) {
                        motionControllerInstance.motionTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('MotionEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('MotionDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Motion deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Motion", error.statusText || "Failed to delete motion.", 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'MotionDetailModal') {
            motionControllerInstance.ClearDetailForm();
        } else if (modalId === 'MotionEditModal') {
            motionControllerInstance.ClearEditForm();
            motionControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#motionDescription").html("");
        $("#motionLag").html("");
        $("#motionLead").html("");
        $("#hdMotionId").val("");
    }

    ClearEditForm() {
        $("#edit_motionDescription").val("");
        $("#edit_motionLag").val("");
        $("#edit_motionLead").val("");
        $("#edit_hdMotionId").val("");
    }

    ClearEditValidations() {
        $("#edit_motionDescription").removeClass("is-invalid");
        $("#edit_motionDescription").next(".invalid-feedback").hide();
    }

    ViewMotion(motionId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}MotionAPI/GetMotion/${motionId}`;
        const progressId = isEditMode ? "#edit_progress-motion" : "#progress-motion";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('MotionDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (motionId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdMotionId").val(response.data.id);
                            $("#edit_motionDescription").val(response.data.description);
                            $("#edit_motionLag").val(response.data.lag || '');
                            $("#edit_motionLead").val(response.data.lead || '');
                            $("#MotionEditModalLabel").html(`Edit Motion: ${response.data.description}`);
                        } else {
                            $("#motionDescription").html(response.data.description);
                            $("#motionLag").html(response.data.lag || '');
                            $("#motionLead").html(response.data.lead || '');
                            $("#hdMotionId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', response.error || 'Failed to retrieve motion details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification('Error Retrieving Motion Details', error.statusText || 'Failed to retrieve motion details.', 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveMotion() {
        if ($("#edit_hdMotionId").val() === "") {
            this.CreateMotion();
        } else {
            this.UpdateMotion();
        }
        if (motionControllerInstance.motionTable) {
            motionControllerInstance.ClearEditForm();
            motionControllerInstance.motionTable.draw();
        }
    }

    CreateMotion() {
        try {
            $("#edit_progress-motion").show();
            const motionData = {
                description: $("#edit_motionDescription").val().trim(),
                lag: $("#edit_motionLag").val() ? parseInt($("#edit_motionLag").val()) : null,
                lead: $("#edit_motionLead").val() ? parseInt($("#edit_motionLead").val()) : null
            };
            $.ajax({
                url: `${this.service.baseUrl}MotionAPI/CreateMotion`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(motionData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-motion").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Motion created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('MotionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (motionControllerInstance.motionTable) {
                            motionControllerInstance.motionTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating motion.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-motion").hide();
                    ShowNotification("Error Creating Motion", error.statusText || "Failed to create motion.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-motion").hide();
            ShowNotification("Error Creating Motion", e.message, 'error');
        }
    }

    UpdateMotion() {
        try {
            $("#edit_progress-motion").show();
            const motionData = {
                id: parseInt($("#edit_hdMotionId").val()),
                description: $("#edit_motionDescription").val().trim(),
                lag: $("#edit_motionLag").val() ? parseInt($("#edit_motionLag").val()) : null,
                lead: $("#edit_motionLead").val() ? parseInt($("#edit_motionLead").val()) : null
            };
            $.ajax({
                url: `${this.service.baseUrl}MotionAPI/UpdateMotion`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(motionData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-motion").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Motion updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('MotionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (motionControllerInstance.motionTable) {
                            motionControllerInstance.motionTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating motion.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-motion").hide();
                    ShowNotification("Error Updating Motion", error.statusText || "Failed to update motion.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-motion").hide();
            ShowNotification("Error Updating Motion", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}