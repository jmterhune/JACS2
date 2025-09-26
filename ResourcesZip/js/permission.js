// permission.js
let permissionControllerInstance = null;
class PermissionController {
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
        this.permissionId = -1;
        this.searchTerm = "";
        this.permissionTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        permissionControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}PermissionAPI/DeletePermission/`;

        const listUrl = `${this.service.baseUrl}PermissionAPI/GetPermissions/${this.recordCount}`;
        const detailModalElement = document.getElementById('PermissionDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('PermissionEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.permissionTable = $('#tblPermission').DataTable({
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
                    $("#tblPermission_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve permissions.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification('Error Retrieving Permissions', errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="permission-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Permission" data-toggle="tooltip" data-id="${data}" class="permission-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                    data: "guard_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Permission" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.permissionTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const permissionId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Permission?',
                    text: 'Are you sure you wish to delete this Permission?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        permissionControllerInstance.DeletePermission(permissionId);
                    }
                });
            });
        });

        $(document).on('click', '.permission-detail', function (e) {
            e.preventDefault();
            var permissionId = $(this).data("id");
            permissionControllerInstance.ViewPermission(permissionId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('PermissionEditModal'));
        $(document).on('click', '.permission-edit, #editPermissionBtn', function (e) {
            e.preventDefault();
            var permissionId = $(this).data("id") || $("#hdPermissionId").val();
            permissionControllerInstance.permissionId = permissionId;
            if (permissionId) {
                permissionControllerInstance.ViewPermission(permissionId, true);
                $("#PermissionEditModalLabel").html(`Edit Permission`);
            } else {
                permissionControllerInstance.ClearEditForm();
                $("#PermissionEditModalLabel").html("Create New Permission");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            permissionControllerInstance.ClearEditForm();
            $("#PermissionEditModalLabel").html("Create New Permission");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var permissionId = $("#hdPermissionId").val();
            Swal.fire({
                title: 'Delete Permission?',
                text: 'Are you sure you wish to delete this Permission?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    permissionControllerInstance.DeletePermission(permissionId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $permissionName = $("#edit_permissionName");
            const $permissionNameError = $permissionName.next(".invalid-feedback");
            if ($permissionName.val().trim() === "") {
                $permissionNameError.show();
                $permissionName.addClass("is-invalid");
                isValid = false;
            } else {
                $permissionNameError.hide();
                $permissionName.removeClass("is-invalid");
            }

            if (isValid) {
                permissionControllerInstance.SavePermission();
            }
        });

        $("#edit_permissionName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.permissionTable) {
            this.permissionTable.state.clear();
            window.location.reload();
        }
    }

    DeletePermission(permissionId) {
        $.ajax({
            url: this.deleteUrl + permissionId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (permissionControllerInstance.permissionTable) {
                        permissionControllerInstance.permissionTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('PermissionEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('PermissionDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Permission deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Permission", error.statusText || "Failed to delete permission.", 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'PermissionDetailModal') {
            permissionControllerInstance.ClearDetailForm();
        } else if (modalId === 'PermissionEditModal') {
            permissionControllerInstance.ClearEditForm();
            permissionControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#permissionName").html("");
        $("#permissionGuardName").html("");
        $("#hdPermissionId").val("");
    }

    ClearEditForm() {
        $("#edit_permissionName").val("");
        $("#edit_permissionGuardName").val("");
        $("#edit_hdPermissionId").val("");
    }

    ClearEditValidations() {
        $("#edit_permissionName").removeClass("is-invalid");
        $("#edit_permissionName").next(".invalid-feedback").hide();
        $("#edit_permissionGuardName").removeClass("is-invalid");
        $("#edit_permissionGuardName").next(".invalid-feedback").hide();
    }

    ViewPermission(permissionId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}PermissionAPI/GetPermission/${permissionId}`;
        const progressId = isEditMode ? "#edit_progress-permission" : "#progress-permission";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('PermissionDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (permissionId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdPermissionId").val(response.data.id);
                            $("#edit_permissionName").val(response.data.name);
                            $("#edit_permissionGuardName").val(response.data.guard_name || '');
                            $("#PermissionEditModalLabel").html(`Edit Permission: ${response.data.name}`);
                        } else {
                            $("#permissionName").html(response.data.name);
                            $("#permissionGuardName").html(response.data.guard_name || '');
                            $("#hdPermissionId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve permission details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Permission Details", error.statusText || "Failed to retrieve permission details.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SavePermission() {
        if ($("#edit_hdPermissionId").val() === "") {
            this.CreatePermission();
        } else {
            this.UpdatePermission();
        }
        if (permissionControllerInstance.permissionTable) {
            permissionControllerInstance.ClearEditForm();
            permissionControllerInstance.permissionTable.draw();
        }
    }

    CreatePermission() {
        try {
            $("#edit_progress-permission").show();
            const permissionData = {
                name: $("#edit_permissionName").val().trim(),
                guard_name: $("#edit_permissionGuardName").val().trim() || null
            };
            $.ajax({
                url: `${this.service.baseUrl}PermissionAPI/CreatePermission`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(permissionData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-permission").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Permission created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('PermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (permissionControllerInstance.permissionTable) {
                            permissionControllerInstance.permissionTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating permission.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-permission").hide();
                    ShowNotification("Error Creating Permission", error.statusText || "Failed to create permission.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-permission").hide();
            ShowNotification("Error Creating Permission", e.message, 'error');
        }
    }

    UpdatePermission() {
        try {
            $("#edit_progress-permission").show();
            const permissionData = {
                id: parseInt($("#edit_hdPermissionId").val()),
                name: $("#edit_permissionName").val().trim(),
                guard_name: $("#edit_permissionGuardName").val().trim() || null
            };
            $.ajax({
                url: `${this.service.baseUrl}PermissionAPI/UpdatePermission`, // Fixed incorrect URL
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(permissionData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-permission").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Permission updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('PermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (permissionControllerInstance.permissionTable) {
                            permissionControllerInstance.permissionTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating permission.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-permission").hide();
                    ShowNotification("Error Updating Permission", error.statusText || "Failed to update permission.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-permission").hide();
            ShowNotification("Error Updating Permission", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}