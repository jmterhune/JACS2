let roleControllerInstance = null;

class RoleController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 3;
        this.currentPage = params.currentPage || 0;
        this.userUrl = params.userUrl || '';
        this.roleId = -1;
        this.searchTerm = "";
        this.roleTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        this.updateUrl = null;
        this.createUrl = null;
        this.viewUrl = null;
        roleControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}RoleAPI/DeleteRole/`;
        this.updateUrl = `${this.service.baseUrl}RoleAPI/UpdateRole`;
        this.createUrl = `${this.service.baseUrl}RoleAPI/CreateRole`;
        this.viewUrl = `${this.service.baseUrl}RoleAPI/GetRole/`;

        const listUrl = `${this.service.baseUrl}RoleAPI/GetRoles/${this.recordCount}`;
        const detailModalElement = document.getElementById('RoleDetailModal');
        const editModalElement = document.getElementById('RoleEditModal');
        const editModal = new bootstrap.Modal(document.getElementById('RoleEditModal'));

        if (detailModalElement) detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        if (editModalElement) editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);

        this.roleTable = $('#tblRole').DataTable({
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
                    $("#tblRole_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve roles.';
                    if (error.status === 401) errorMessage = 'Please make sure you are logged in and try again.';
                    ShowNotification('Error Retrieving Roles', errorMessage, 'error');
                }
            },
            columns: [
                { data: "id", render: data => `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="role-detail btn-command"><i class="fas fa-eye"></i></button>`, className: "command-item", orderable: false },
                { data: "id", render: data => `<button type="button" title="Edit Role" data-toggle="tooltip" data-id="${data}" class="role-edit btn-command"><i class="fas fa-pencil"></i></button>`, className: "command-item", orderable: false },
                { data: "id", render: data => `<a title="View Users in Role" href="${roleControllerInstance.userUrl}/rid/${data}" class="role-users btn-command"><i class="fas fa-users"></i></a>`, className: "command-item", orderable: false },
                { data: "name", render: data => data || '' },
                { data: "guard_name", render: data => data || '' },
                { data: "id", render: (data, type, row) => isAdmin ? `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Role" data-id="${row.id}"><i class="fas fa-trash"></i></button>` : '', className: "command-item", orderable: false }
            ],
            language: { emptyTable: "No Records Available.", zeroRecords: "No records match the search criteria you entered." },
            order: [[3, 'asc']],
            serverSide: false,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: 25,
        });

        this.roleTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const roleId = $(this).data("id");
                Swal.fire({ title: 'Delete Role?', text: 'Are you sure?', icon: 'warning', showCancelButton: true }).then((result) => {
                    if (result.isConfirmed) roleControllerInstance.deleteRole(roleId);
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

        $(document).on('click', '.role-detail', function (e) {
            e.preventDefault();
            var roleId = $(this).data("id");
            roleControllerInstance.viewRole(roleId, false);
        });

        $(document).on('click', '.role-edit, #editRoleBtn', function (e) {
            e.preventDefault();
            var roleId = $(this).data("id") || $("#hdRoleId").val();
            roleControllerInstance.roleId = roleId;
            if (roleId) {
                roleControllerInstance.viewRole(roleId, true);
                $("#RoleEditModalLabel").html(`Edit Role`);
            } else {
                roleControllerInstance.clearEditForm();
                $("#RoleEditModalLabel").html("Create New Role");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            roleControllerInstance.clearEditForm();
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
                    roleControllerInstance.deleteRole(roleId);
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

            if (isValid) roleControllerInstance.saveRole();
        });

        $("#edit_roleName, #edit_roleGuardName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'RoleDetailModal') {
            roleControllerInstance.clearDetailForm();
        } else if (modalId === 'RoleEditModal') {
            roleControllerInstance.clearEditForm();
            roleControllerInstance.clearEditValidations();
        }
    }

    clearDetailForm() {
        $("#roleName").html("");
        $("#roleGuardName").html("");
        $("#hdRoleId").val("");
    }

    clearEditForm() {
        $("#edit_roleName").val("");
        $("#edit_roleGuardName").val("");
        $("#edit_hdRoleId").val("");
    }

    clearEditValidations() {
        $("#edit_roleName").removeClass("is-invalid");
        $("#edit_roleName").next(".invalid-feedback").hide();
        $("#edit_roleGuardName").removeClass("is-invalid");
        $("#edit_roleGuardName").next(".invalid-feedback").hide();
    }

    viewRole(roleId, isEditMode = false) {
        const progressId = isEditMode ? "#edit_progress-role" : "#progress-role";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('RoleDetailModal'));
            if (!modal._element.classList.contains('show')) modal.show();
        }

        if (roleId) {
            $.ajax({
                url: this.viewUrl + roleId,
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
                        ShowNotification('Error', response.error || 'Failed to retrieve role details.', 'error');
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

    saveRole() {
        if ($("#edit_hdRoleId").val() === "") {
            this.createRole();
        } else {
            this.updateRole();
        }
    }

    createRole() {
        try {
            $("#edit_progress-role").show();
            const roleData = {
                name: $("#edit_roleName").val().trim(),
                guard_name: $("#edit_roleGuardName").val().trim()
            };
            $.ajax({
                url: this.createUrl,
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
                        if (editModal) editModal.hide();
                        if (roleControllerInstance.roleTable) {
                            roleControllerInstance.roleTable.ajax.reload();
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

    updateRole() {
        try {
            $("#edit_progress-role").show();
            const roleData = {
                id: parseInt($("#edit_hdRoleId").val()),
                name: $("#edit_roleName").val().trim(),
                guard_name: $("#edit_roleGuardName").val().trim()
            };
            $.ajax({
                url: this.updateUrl,
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
                        if (editModal) editModal.hide();
                        if (roleControllerInstance.roleTable) {
                            roleControllerInstance.roleTable.ajax.reload();
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

    deleteRole(roleId) {
        $.ajax({
            url: this.deleteUrl + roleId,
            type: 'DELETE',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                    if (editModal) editModal.hide();
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('RoleDetailModal'));
                    if (detailModal) detailModal.hide();
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Role deleted successfully.'
                    });
                    if (roleControllerInstance.roleTable) {
                        roleControllerInstance.roleTable.ajax.reload();
                    }
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Role", error.statusText || "Failed to delete role.", 'error');
            }
        });
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}