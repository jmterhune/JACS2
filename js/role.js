let roleControllerInstance = null;

class RoleController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 3;
        this.currentPage = params.currentPage || 0;
        this.userUrl = params.userUrl || '';
        this.searchTerm = "";
        this.roleTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        roleControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}RoleAPI/DeleteRole/`;

        const listUrl = `${this.service.baseUrl}RoleAPI/GetRoles/${this.recordCount}`;
        const detailModalElement = document.getElementById('RoleDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('RoleEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.roleTable = $('#tblRole').DataTable({
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
                    $("#tblRole_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve roles.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification('Error Retrieving Roles', errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="role-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    searchable: false,
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Role" data-toggle="tooltip" data-id="${data}" class="role-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    searchable: false,
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<a title="View Users in Role" href="${roleControllerInstance.userUrl}/rid/${data}" class="role-users btn-command"><i class="fas fa-users"></i></a>`;
                    },
                    className: "command-item",
                    searchable: false,
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
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Role" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    searchable: false,
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
        this.roleTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const roleId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Role?',
                    text: 'Are you sure you wish to delete this Role?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        roleControllerInstance.DeleteRole(roleId);
                    }
                });
            });
        });

        $(document).on('click', '.role-detail', function (e) {
            e.preventDefault();
            var roleId = $(this).data("id");
            roleControllerInstance.ViewRole(roleId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('RoleEditModal'));
        $(document).on('click', '.role-edit, #editRoleBtn', function (e) {
            e.preventDefault();
            var roleId = $(this).data("id") || $("#hdRoleId").val();
            roleControllerInstance.roleId = roleId;
            if (roleId) {
                roleControllerInstance.ViewRole(roleId, true);
                $("#RoleEditModalLabel").html(`Edit Role`);
            } else {
                roleControllerInstance.ClearEditForm();
                $("#RoleEditModalLabel").html("Create New Role");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            roleControllerInstance.ClearEditForm();
            $("#RoleEditModalLabel").html("Create New Role");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var roleId = $("#hdRoleId").val();
            Swal.fire({
                title: 'Delete Role?',
                text: 'Are you sure you wish to delete this Role?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    roleControllerInstance.DeleteRole(roleId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $roleName = $("#edit_roleName");
            const $roleNameError = $roleName.next(".invalid-feedback");
            if ($roleName.val().trim() === "") {
                $roleNameError.show();
                $roleName.addClass("is-invalid");
                isValid = false;
            } else {
                $roleNameError.hide();
                $roleName.removeClass("is-invalid");
            }

            const $guardName = $("#edit_roleGuardName");
            const $guardNameError = $guardName.next(".invalid-feedback");
            if ($guardName.val().trim() === "") {
                $guardNameError.show();
                $guardName.addClass("is-invalid");
                isValid = false;
            } else {
                $guardNameError.hide();
                $guardName.removeClass("is-invalid");
            }

            if (isValid) {
                roleControllerInstance.SaveRole();
            }
        });

        $("#edit_roleName, #edit_roleGuardName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.roleTable) {
            this.roleTable.state.clear();
            window.location.reload();
        }
    }

    DeleteRole(roleId) {
        $.ajax({
            url: this.deleteUrl + roleId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (roleControllerInstance.roleTable) {
                        roleControllerInstance.roleTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('RoleDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Role deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Role", error.statusText || "Failed to delete role.", 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'RoleDetailModal') {
            roleControllerInstance.ClearDetailForm();
        } else if (modalId === 'RoleEditModal') {
            roleControllerInstance.ClearEditForm();
            roleControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#roleName").html("");
        $("#roleGuardName").html("");
        $("#hdRoleId").val("");
    }

    ClearEditForm() {
        $("#edit_roleName").val("");
        $("#edit_roleGuardName").val("");
        $("#edit_hdRoleId").val("");
    }

    ClearEditValidations() {
        $("#edit_roleName").removeClass("is-invalid");
        $("#edit_roleName").next(".invalid-feedback").hide();
        $("#edit_roleGuardName").removeClass("is-invalid");
        $("#edit_roleGuardName").next(".invalid-feedback").hide();
    }

    ViewRole(roleId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}RoleAPI/GetRole/${roleId}`;
        const progressId = isEditMode ? "#edit_progress-role" : "#progress-role";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('RoleDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (roleId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdRoleId").val(response.data.id);
                            $("#edit_roleName").val(response.data.name);
                            $("#edit_roleGuardName").val(response.data.guard_name || '');
                            $("#RoleEditModalLabel").html(`Edit Role: ${response.data.name}`);
                        } else {
                            $("#roleName").html(response.data.name);
                            $("#roleGuardName").html(response.data.guard_name || '');
                            $("#hdRoleId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', response.error || 'Failed to retrieve role details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification('Error Retrieving Role Details', error.statusText || 'Failed to retrieve role details.', 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveRole() {
        if ($("#edit_hdRoleId").val() === "") {
            this.CreateRole();
        } else {
            this.UpdateRole();
        }
        if (roleControllerInstance.roleTable) {
            roleControllerInstance.ClearEditForm();
            roleControllerInstance.roleTable.draw();
        }
    }

    CreateRole() {
        try {
            $("#edit_progress-role").show();
            const roleData = {
                name: $("#edit_roleName").val().trim(),
                guard_name: $("#edit_roleGuardName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}RoleAPI/CreateRole`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(roleData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-role").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Role created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (roleControllerInstance.roleTable) {
                            roleControllerInstance.roleTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating role.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-role").hide();
                    ShowNotification("Error Creating Role", error.statusText || "Failed to create role.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-role").hide();
            ShowNotification("Error Creating Role", e.message, 'error');
        }
    }

    UpdateRole() {
        try {
            $("#edit_progress-role").show();
            const roleData = {
                id: parseInt($("#edit_hdRoleId").val()),
                name: $("#edit_roleName").val().trim(),
                guard_name: $("#edit_roleGuardName").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}RoleAPI/UpdateRole`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(roleData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-role").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Role updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (roleControllerInstance.roleTable) {
                            roleControllerInstance.roleTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating role.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-role").hide();
                    ShowNotification("Error Updating Role", error.statusText || "Failed to update role.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-role").hide();
            ShowNotification("Error Updating Role", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}