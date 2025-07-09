let permissionControllerInstance = null;

class PermissionController {
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
                    $("#tblPermission_processing").hide();
                    if (error.status === 401) {
                        ShowAlert("Error Retrieving Permissions", "Please make sure you are logged in and try again. Error: " + error.statusText);
                    } else {
                        ShowAlert("Error Retrieving Permissions", "The following error occurred attempting to retrieve permission information. Error: " + error.statusText);
                    }
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
                $.dnnConfirm({
                    text: 'Are you sure you wish to delete this Permission?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Delete Permission?',
                    callbackTrue: function () {
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
            $.dnnConfirm({
                text: 'Are you sure you wish to delete this Permission?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Permission?',
                callbackTrue: function () {
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
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
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
            },
            error: function (error) {
                ShowAlert("Error Deleting Permission", error.statusText);
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
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdPermissionId").val(response.data.id);
                            $("#edit_permissionName").val(response.data.name);
                            $("#edit_permissionGuardName").val(response.data.guard_name);
                            $("#PermissionEditModalLabel").html(`Edit Permission: ${response.data.name}`);
                        } else {
                            $("#permissionName").html(response.data.name);
                            $("#permissionGuardName").html(response.data.guard_name);
                            $("#hdPermissionId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowAlert("Error", "Failed to retrieve permission details. Please try again later.");
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch permission details');
                    ShowAlert("Error", "Failed to retrieve permission details. Please try again later.");
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
                name: $("#edit_permissionName").val(),
                guard_name: $("#edit_permissionGuardName").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}PermissionAPI/CreatePermission`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(permissionData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-permission").hide();
                        ShowAlert("Success", "Permission created successfully.");
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('PermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-permission").hide();
                        ShowAlert("Error", "Unexpected Error: Status=" + result);
                    }
                },
                error: function (error) {
                    $("#edit_progress-permission").hide();
                    ShowAlert("Error Creating Permission", error.statusText);
                }
            });
        } catch (e) {
            $("#edit_progress-permission").hide();
            ShowAlert("Error Creating Permission", e.statusText);
        }
    }

    UpdatePermission() {
        try {
            $("#edit_progress-permission").show();
            const permissionData = {
                id: $("#edit_hdPermissionId").val(),
                name: $("#edit_permissionName").val(),
                guard_name: $("#edit_permissionGuardName").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}PermissionAPI/CreatePermission`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(permissionData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-permission").hide();
                        ShowAlert("Success", "Permission updated successfully.");
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('PermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-permission").hide();
                        ShowAlert("Error", "Unexpected Error: Status=" + result);
                    }
                },
                error: function (error) {
                    $("#edit_progress-permission").hide();
                    ShowAlert("Error Updating Permission", error.statusText);
                }
            });
        } catch (e) {
            $("#edit_progress-permission").hide();
            ShowAlert("Error Updating Permission", e.statusText);
        }
    }
}