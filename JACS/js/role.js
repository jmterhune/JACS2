let roleControllerInstance = null;

class RoleController {
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
        this.roleId = -1;
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
                    $("#tblRole_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Roles', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Roles', 'The following error occurred attempting to retrieve role information. Error: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="role-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Role" data-toggle="tooltip" data-id="${data}" class="role-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Role" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
            if ($roleName.val() === "") {
                $roleNameError.show();
                $roleName.addClass("is-invalid");
                isValid = false;
            } else {
                $roleNameError.hide();
                $roleName.removeClass("is-invalid");
            }

            const $guardName = $("#edit_roleGuardName");
            const $guardNameError = $guardName.next(".invalid-feedback");
            if ($guardName.val() === "") {
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
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
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
                    text: 'Role deleted successfully.'
                });
            },
            error: function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error Deleting Role',
                    text: error.statusText
                });
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
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdRoleId").val(response.data.id);
                            $("#edit_roleName").val(response.data.name);
                            $("#edit_roleGuardName").val(response.data.guard_name);
                            $("#RoleEditModalLabel").html(`Edit Role: ${response.data.name}`);
                        } else {
                            $("#roleName").html(response.data.name);
                            $("#roleGuardName").html(response.data.guard_name);
                            $("#hdRoleId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', 'Failed to retrieve role details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch role details');
                    ShowNotification('Error', 'Failed to retrieve role details. Please try again later.', 'error');
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
                name: $("#edit_roleName").val(),
                guard_name: $("#edit_roleGuardName").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}RoleAPI/CreateRole`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(roleData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-role").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Role created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-role").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-role").hide();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Creating Role',
                        text: error.statusText
                    });
                }
            });
        } catch (e) {
            $("#edit_progress-role").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Creating Role',
                text: e.statusText
            });
        }
    }

    UpdateRole() {
        try {
            $("#edit_progress-role").show();
            const roleData = {
                id: $("#edit_hdRoleId").val(),
                name: $("#edit_roleName").val(),
                guard_name: $("#edit_roleGuardName").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}RoleAPI/UpdateRole`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(roleData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-role").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Role updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('RoleEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-role").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-role").hide();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Updating Role',
                        text: error.statusText
                    });
                }
            });
        } catch (e) {
            $("#edit_progress-role").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Updating Role',
                text: e.statusText
            });
        }
    }
}