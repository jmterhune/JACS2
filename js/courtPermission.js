let courtPermissionControllerInstance = null;

class CourtPermissionController {
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
        this.courtPermissionId = -1;
        this.searchTerm = "";
        this.courtPermissionTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtPermissionControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtPermissionAPI/DeleteCourtPermission/`;

        const listUrl = `${this.service.baseUrl}CourtPermissionAPI/GetCourtPermissions/${this.recordCount}`;
        const detailModalElement = document.getElementById('CourtPermissionDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CourtPermissionEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateDropdowns();

        this.courtPermissionTable = $('#tblCourtPermission').DataTable({
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
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="cp-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Court Permission" data-toggle="tooltip" data-id="${data}" class="cp-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "user_display_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "judge_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "active",
                    render: function (data) {
                        return data ? '<i class="fas fa-check-square"></i>' : '<i class="far fa-square"></i>';
                    },
                    className: "text-center"
                },
                {
                    data: "permission",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Court Permission" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.courtPermissionTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtPermissionId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Court Permission?',
                    text: 'Are you sure you wish to delete this Court Permission?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtPermissionControllerInstance.DeleteCourtPermission(courtPermissionId);
                    }
                });
            });
        });

        $(document).on('click', '.cp-detail', function (e) {
            e.preventDefault();
            var courtPermissionId = $(this).data("id");
            courtPermissionControllerInstance.ViewCourtPermission(courtPermissionId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtPermissionEditModal'));
        $(document).on('click', '.cp-edit, #editCourtPermissionBtn', function (e) {
            e.preventDefault();
            var courtPermissionId = $(this).data("id") || $("#hdCourtPermissionId").val();
            courtPermissionControllerInstance.courtPermissionId = courtPermissionId;
            if (courtPermissionId) {
                courtPermissionControllerInstance.ViewCourtPermission(courtPermissionId, true);
                $("#CourtPermissionEditModalLabel").html(`Edit Court Permission`);
            } else {
                courtPermissionControllerInstance.ClearEditForm();
                $("#CourtPermissionEditModalLabel").html("Create New Court Permission");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtPermissionControllerInstance.ClearEditForm();
            $("#CourtPermissionEditModalLabel").html("Create New Court Permission");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var courtPermissionId = $("#hdCourtPermissionId").val();
            Swal.fire({
                title: 'Delete Court Permission?',
                text: 'Are you sure you wish to delete this Court Permission?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtPermissionControllerInstance.DeleteCourtPermission(courtPermissionId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $cpUser = $("#edit_cpUser");
            const $cpUserError = $cpUser.next(".invalid-feedback");
            if ($cpUser.val().trim() === "") {
                $cpUserError.show();
                $cpUser.addClass("is-invalid");
                isValid = false;
            } else {
                $cpUserError.hide();
                $cpUser.removeClass("is-invalid");
            }

            const $cpJudge = $("#edit_cpJudge");
            const $cpJudgeError = $cpJudge.next(".invalid-feedback");
            if ($cpJudge.val().trim() === "") {
                $cpJudgeError.show();
                $cpJudge.addClass("is-invalid");
                isValid = false;
            } else {
                $cpJudgeError.hide();
                $cpJudge.removeClass("is-invalid");
            }

            const $cpActive = $("input[name='cpActive']:checked");
            const $cpActiveError = $(".form-check").parent().find(".invalid-feedback");
            if ($cpActive.length === 0) {
                $cpActiveError.show();
                isValid = false;
            } else {
                $cpActiveError.hide();
            }

            if (isValid) {
                courtPermissionControllerInstance.SaveCourtPermission();
            }
        });

        $("#edit_cpUser, #edit_cpJudge").on("change", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("input[name='cpActive']").on("change", function () {
            const $this = $(this);
            if ($this.is(":checked")) {
                $this.closest(".form-group").find(".invalid-feedback").hide();
            }
        });
    }

    populateDropdowns() {
        $.ajax({
            url: `${this.service.baseUrl}CourtPermissionAPI/GetUsersForDropdown`,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (users) {
                const $userDropdown = $("#edit_cpUser");
                $userDropdown.empty().append('<option value="">Select User</option>');
                users.forEach(user => {
                    $userDropdown.append(`<option value="${user.Key}">${user.Value}</option>`);
                });
            },
            error: function () {
                ShowNotification("Error", "Failed to load users.", 'error');
            }
        });

        $.ajax({
            url: `${this.service.baseUrl}CourtPermissionAPI/GetJudgesForDropdown`,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (judges) {
                const $judgeDropdown = $("#edit_cpJudge");
                $judgeDropdown.empty().append('<option value="">Select Judge</option>');
                judges.forEach(judge => {
                    $judgeDropdown.append(`<option value="${judge.Key}">${judge.Value}</option>`);
                });
            },
            error: function () {
                ShowNotification("Error", "Failed to load judges.", 'error');
            }
        });
    }

    ClearState() {
        if (this.courtPermissionTable) {
            this.courtPermissionTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtPermission(courtPermissionId) {
        $.ajax({
            url: this.deleteUrl + courtPermissionId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (courtPermissionControllerInstance.courtPermissionTable) {
                    courtPermissionControllerInstance.courtPermissionTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtPermissionEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('CourtPermissionDetailModal'));
                if (detailModal) {
                    detailModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Court Permission deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Court Permission", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CourtPermissionDetailModal') {
            courtPermissionControllerInstance.ClearDetailForm();
        } else if (modalId === 'CourtPermissionEditModal') {
            courtPermissionControllerInstance.ClearEditForm();
            courtPermissionControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#cpUserDisplayName").html("");
        $("#cpJudgeName").html("");
        $("#cpActive").html("");
        $("#cpPermission").html("");
        $("#hdCourtPermissionId").val("");
    }

    ClearEditForm() {
        $("#edit_cpUser").val("");
        $("#edit_cpJudge").val("");
        $("#edit_cpEditable").val("true");
        $("input[name='cpActive']").prop("checked", false);
        $("#edit_hdCourtPermissionId").val("");
    }

    ClearEditValidations() {
        $("#edit_cpUser, #edit_cpJudge").removeClass("is-invalid");
        $("#edit_cpUser, #edit_cpJudge").next(".invalid-feedback").hide();
        $(".form-check").parent().find(".invalid-feedback").hide();
    }

    ViewCourtPermission(courtPermissionId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtPermissionAPI/GetCourtPermission/${courtPermissionId}`;
        const progressId = isEditMode ? "#edit_progress-courtpermission" : "#progress-courtpermission";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtPermissionDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (courtPermissionId) {
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
                        const permission = response.data;
                        if (isEditMode) {
                            $("#edit_hdCourtPermissionId").val(permission.id);
                            $("#edit_cpUser").val(permission.user_id);
                            $("#edit_cpJudge").val(permission.judge_id);
                            $("#edit_cpEditable").val(permission.editable.toString());
                            $(`input[name='cpActive'][value='${permission.active}']`).prop("checked", true);
                            $("#CourtPermissionEditModalLabel").html(`Edit Court Permission`);
                        } else {
                            $.ajax({
                                url: `${courtPermissionControllerInstance.service.baseUrl}CourtPermissionAPI/GetUsersForDropdown`,
                                type: 'GET',
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader('ModuleId', moduleId);
                                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                                },
                                success: function (users) {
                                    const user = users.find(u => u.Key === permission.user_id);
                                    $("#cpUserDisplayName").html(user ? user.Value : "Unknown");
                                }
                            });
                            $.ajax({
                                url: `${courtPermissionControllerInstance.service.baseUrl}CourtPermissionAPI/GetJudgesForDropdown`,
                                type: 'GET',
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader('ModuleId', moduleId);
                                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                                },
                                success: function (judges) {
                                    const judge = judges.find(j => j.Key === permission.judge_id);
                                    $("#cpJudgeName").html(judge ? judge.Value : "Unknown");
                                }
                            });
                            $("#cpActive").html(permission.active ? '<i class="fas fa-check-square"></i>' : '<i class="far fa-square"></i>');
                            $("#cpPermission").html(permission.editable ? "View and Edit" : "View");
                            $("#hdCourtPermissionId").val(permission.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve court permission details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch court permission details');
                    ShowNotification("Error", "Failed to retrieve court permission details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCourtPermission() {
        if ($("#edit_hdCourtPermissionId").val() === "") {
            this.CreateCourtPermission();
        } else {
            this.UpdateCourtPermission();
        }
        if (courtPermissionControllerInstance.courtPermissionTable) {
            courtPermissionControllerInstance.ClearEditForm();
            courtPermissionControllerInstance.courtPermissionTable.draw();
        }
    }

    CreateCourtPermission() {
        try {
            $("#edit_progress-courtpermission").show();
            const permissionData = {
                user_id: $("#edit_cpUser").val(),
                judge_id: $("#edit_cpJudge").val(),
                active: $("input[name='cpActive']:checked").val() === "true",
                editable: $("#edit_cpEditable").val() === "true"
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtPermissionAPI/CreateCourtPermission`,
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
                        $("#edit_progress-courtpermission").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court Permission created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtPermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-courtpermission").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtpermission").hide();
                    ShowNotification("Error Creating Court Permission", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtpermission").hide();
            ShowNotification("Error Creating Court Permission", e.statusText, 'error');
        }
    }

    UpdateCourtPermission() {
        try {
            $("#edit_progress-courtpermission").show();
            const permissionData = {
                id: $("#edit_hdCourtPermissionId").val(),
                user_id: $("#edit_cpUser").val(),
                judge_id: $("#edit_cpJudge").val(),
                active: $("input[name='cpActive']:checked").val() === "true",
                editable: $("#edit_cpEditable").val() === "true"
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtPermissionAPI/UpdateCourtPermission`,
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
                        $("#edit_progress-courtpermission").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court Permission updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtPermissionEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-courtpermission").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtpermission").hide();
                    ShowNotification("Error Updating Court Permission", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtpermission").hide();
            ShowNotification("Error Updating Court Permission", e.statusText, 'error');
        }
    }
}