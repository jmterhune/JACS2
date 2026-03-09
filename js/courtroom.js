let courtroomControllerInstance = null;

class CourtroomController {
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
        this.courtroomId = -1;
        this.searchTerm = "";
        this.courtroomTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtroomControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtroomAPI/DeleteCourtroom/`;

        const listUrl = `${this.service.baseUrl}CourtroomAPI/GetCourtrooms/${this.recordCount}`;
        const detailModalElement = document.getElementById('CourtroomDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CourtroomEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.courtroomTable = $('#tblCourtroom').DataTable({
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
                    $("#tblCourtroom_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve courtrooms.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification("Error Retrieving Courtrooms", errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="courtroom-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Courtroom" data-toggle="tooltip" data-id="${data}" class="courtroom-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                        if (isAdmin) {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Courtroom" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.courtroomTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtroomId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Courtroom?',
                    text: 'Are you sure you wish to delete this Courtroom?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtroomControllerInstance.DeleteCourtroom(courtroomId);
                    }
                });
            });
        });

        $(document).on('click', '.courtroom-detail', function (e) {
            e.preventDefault();
            var courtroomId = $(this).data("id");
            courtroomControllerInstance.ViewCourtroom(courtroomId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtroomEditModal'));
        $(document).on('click', '.courtroom-edit, #editCourtroomBtn', function (e) {
            e.preventDefault();
            var courtroomId = $(this).data("id") || $("#hdCourtroomId").val();
            courtroomControllerInstance.courtroomId = courtroomId;
            if (courtroomId) {
                courtroomControllerInstance.ViewCourtroom(courtroomId, true);
                $("#CourtroomEditModalLabel").html(`Edit Courtroom`);
            } else {
                courtroomControllerInstance.ClearEditForm();
                $("#CourtroomEditModalLabel").html("Create New Courtroom");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtroomControllerInstance.ClearEditForm();
            $("#CourtroomEditModalLabel").html("Create New Courtroom");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var courtroomId = $("#hdCourtroomId").val();
            Swal.fire({
                title: 'Delete Courtroom?',
                text: 'Are you sure you wish to delete this Courtroom?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtroomControllerInstance.DeleteCourtroom(courtroomId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $courtroomDescription = $("#edit_courtroomDescription");
            const $courtroomDescriptionError = $courtroomDescription.next(".invalid-feedback");
            if ($courtroomDescription.val().trim() === "") {
                $courtroomDescriptionError.show();
                $courtroomDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $courtroomDescriptionError.hide();
                $courtroomDescription.removeClass("is-invalid");
            }

            if (isValid) {
                courtroomControllerInstance.SaveCourtroom();
            }
        });

        $("#edit_courtroomDescription").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.courtroomTable) {
            this.courtroomTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtroom(courtroomId) {
        $.ajax({
            url: this.deleteUrl + courtroomId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (courtroomControllerInstance.courtroomTable) {
                        courtroomControllerInstance.courtroomTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Courtroom deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Courtroom", error.statusText || "Failed to delete courtroom.", 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CourtroomDetailModal') {
            courtroomControllerInstance.ClearDetailForm();
        } else if (modalId === 'CourtroomEditModal') {
            courtroomControllerInstance.ClearEditForm();
            courtroomControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#courtroomDescription").html("");
        $("#hdCourtroomId").val("");
    }

    ClearEditForm() {
        $("#edit_courtroomDescription").val("");
        $("#edit_hdCourtroomId").val("");
    }

    ClearEditValidations() {
        $("#edit_courtroomDescription").removeClass("is-invalid");
        $("#edit_courtroomDescription").next(".invalid-feedback").hide();
    }

    ViewCourtroom(courtroomId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtroomAPI/GetCourtroom/${courtroomId}`;
        const progressId = isEditMode ? "#edit_progress-courtroom" : "#progress-courtroom";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtroomDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (courtroomId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCourtroomId").val(response.data.id);
                            $("#edit_courtroomDescription").val(response.data.description);
                            $("#CourtroomEditModalLabel").html(`Edit Courtroom: ${response.data.description}`);
                        } else {
                            $("#courtroomDescription").html(response.data.description);
                            $("#hdCourtroomId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve courtroom details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Courtroom Details", error.statusText || "Failed to retrieve courtroom details.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCourtroom() {
        if ($("#edit_hdCourtroomId").val() === "") {
            this.CreateCourtroom();
        } else {
            this.UpdateCourtroom();
        }
        if (courtroomControllerInstance.courtroomTable) {
            courtroomControllerInstance.ClearEditForm();
            courtroomControllerInstance.courtroomTable.draw();
        }
    }

    CreateCourtroom() {
        try {
            $("#edit_progress-courtroom").show();
            const courtroomData = {
                description: $("#edit_courtroomDescription").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtroomAPI/CreateCourtroom`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtroomData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courtroom").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Courtroom created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtroomControllerInstance.courtroomTable) {
                            courtroomControllerInstance.courtroomTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating courtroom.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtroom").hide();
                    ShowNotification("Error Creating Courtroom", error.statusText || "Failed to create courtroom.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtroom").hide();
            ShowNotification("Error Creating Courtroom", e.message, 'error');
        }
    }

    UpdateCourtroom() {
        try {
            $("#edit_progress-courtroom").show();
            const courtroomData = {
                id: parseInt($("#edit_hdCourtroomId").val()),
                description: $("#edit_courtroomDescription").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtroomAPI/UpdateCourtroom`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtroomData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courtroom").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Courtroom updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtroomControllerInstance.courtroomTable) {
                            courtroomControllerInstance.courtroomTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating courtroom.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtroom").hide();
                    ShowNotification("Error Updating Courtroom", error.statusText || "Failed to update courtroom.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtroom").hide();
            ShowNotification("Error Updating Courtroom", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}