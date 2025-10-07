let userDefinedFieldControllerInstance = null;

class UserDefinedFieldController {
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
        this.userDefinedFieldId = -1;
        this.searchTerm = "";
        this.userDefinedFieldTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        userDefinedFieldControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}UserDefinedFieldAPI/DeleteUserDefinedField/`;

        const listUrl = `${this.service.baseUrl}UserDefinedFieldAPI/GetUserDefinedFields/${this.recordCount}`;
        const detailModalElement = document.getElementById('UserDefinedFieldDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('UserDefinedFieldEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.userDefinedFieldTable = $('#tblUserDefinedField').DataTable({
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
                    data.courtId = $("#edit_hdCourtId").val();
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblUserDefinedField_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve user defined fields.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification("Error Retrieving User Defined Fields", errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="udf-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit User Defined Field" data-toggle="tooltip" data-id="${data}" class="udf-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "field_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "field_type",
                    render: function (data) {
                        return data == "yes_no" ? "YES/NO" : data.toUpperCase();
                    }
                },
                {
                    data: "alignment",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "default_value",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "required",
                    render: function (data) {
                        return data == 1 ? 'Yes' : 'No';
                    }
                },
                {
                    data: "yes_answer_required",
                    render: function (data) {
                        return data == 1 ? 'Yes' : 'No';
                    }
                },
                {
                    data: "display_on_docket",
                    render: function (data) {
                        return data == 1 ? 'Yes' : 'No';
                    }
                },
                {
                    data: "display_on_schedule",
                    render: function (data) {
                        return data == 1 ? 'Yes' : 'No';
                    }
                },
                {
                    data: "use_in_attorany_scheduling",
                    render: function (data) {
                        return data == 1 ? 'Yes' : 'No';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete User Defined Field" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.userDefinedFieldTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const userDefinedFieldId = $(this).data("id");
                Swal.fire({
                    title: 'Delete User Defined Field?',
                    text: 'Are you sure you wish to delete this User Defined Field?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        userDefinedFieldControllerInstance.DeleteUserDefinedField(userDefinedFieldId);
                    }
                });
            });
        });

        $(document).on('click', '.udf-detail', function (e) {
            e.preventDefault();
            var userDefinedFieldId = $(this).data("id");
            userDefinedFieldControllerInstance.ViewUserDefinedField(userDefinedFieldId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('UserDefinedFieldEditModal'));
        $(document).on('click', '.udf-edit, #editUserDefinedFieldBtn', function (e) {
            e.preventDefault();
            var userDefinedFieldId = $(this).data("id") || $("#hdUserDefinedFieldId").val();
            userDefinedFieldControllerInstance.userDefinedFieldId = userDefinedFieldId;
            if (userDefinedFieldId) {
                userDefinedFieldControllerInstance.ViewUserDefinedField(userDefinedFieldId, true);
                $("#UserDefinedFieldEditModalLabel").html(`Edit User Defined Field`);
            } else {
                userDefinedFieldControllerInstance.ClearEditForm();
                $("#UserDefinedFieldEditModalLabel").html("Create New User Defined Field");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            userDefinedFieldControllerInstance.ClearEditForm();
            $("#UserDefinedFieldEditModalLabel").html("Create New User Defined Field");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var userDefinedFieldId = $("#hdUserDefinedFieldId").val();
            Swal.fire({
                title: 'Delete User Defined Field?',
                text: 'Are you sure you wish to delete this User Defined Field?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    userDefinedFieldControllerInstance.DeleteUserDefinedField(userDefinedFieldId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $fieldName = $("#edit_fieldName");
            const $fieldNameError = $fieldName.next(".invalid-feedback");
            if ($fieldName.val().trim() === "") {
                $fieldNameError.show();
                $fieldName.addClass("is-invalid");
                isValid = false;
            } else {
                $fieldNameError.hide();
                $fieldName.removeClass("is-invalid");
            }

            if (isValid) {
                userDefinedFieldControllerInstance.SaveUserDefinedField();
            }
        });

        $("#edit_fieldName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_fieldType").on("change", function () {
            const val = $(this).val();
            if (val === "yes_no") {
                $("#edit_defaultValue").prop('disabled', true);
            } else {
                $("#edit_defaultValue").prop('disabled', false);
            }
            if (val === "yes_no") {
                $("#edit_yesAnswerRequired").prop('disabled', false);
                $("#edit_required").prop('disabled', true);
            } else {
                $("#edit_yesAnswerRequired").prop('disabled', true);
                $("#edit_required").prop('disabled', false);
            }
        });
    }

    ClearState() {
        if (this.userDefinedFieldTable) {
            this.userDefinedFieldTable.state.clear();
            window.location.reload();
        }
    }

    DeleteUserDefinedField(userDefinedFieldId) {
        $.ajax({
            url: this.deleteUrl + userDefinedFieldId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (userDefinedFieldControllerInstance.userDefinedFieldTable) {
                        userDefinedFieldControllerInstance.userDefinedFieldTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('UserDefinedFieldEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('UserDefinedFieldDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'User Defined Field deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting User Defined Field", error.statusText || "Failed to delete user defined field.", 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'UserDefinedFieldDetailModal') {
            userDefinedFieldControllerInstance.ClearDetailForm();
        } else if (modalId === 'UserDefinedFieldEditModal') {
            userDefinedFieldControllerInstance.ClearEditForm();
            userDefinedFieldControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#fieldName").html("");
        $("#fieldType").html("");
        $("#alignment").html("");
        $("#defaultValue").html("");
        $("#required").html("");
        $("#yesAnswerRequired").html("");
        $("#displayOnDocket").html("");
        $("#displayOnSchedule").html("");
        $("#useInAttorneyScheduling").html("");
        $("#hdUserDefinedFieldId").val("");
    }

    ClearEditForm() {
        $("#edit_fieldName").val("");
        $("#edit_fieldType").val("");
        $("#edit_alignment").val("");
        $("#edit_defaultValue").val("");
        $("#edit_required").prop("checked", false);
        $("#edit_yesAnswerRequired").prop("checked", false);
        $("#edit_displayOnDocket").prop("checked", false);
        $("#edit_displayOnSchedule").prop("checked", false);
        $("#edit_useInAttorneyScheduling").prop("checked", false);
        $("#edit_hdUserDefinedFieldId").val("");
    }

    ClearEditValidations() {
        $("#edit_fieldName").removeClass("is-invalid");
        $("#edit_fieldName").next(".invalid-feedback").hide();
    }

    ViewUserDefinedField(userDefinedFieldId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}UserDefinedFieldAPI/GetUserDefinedField/${userDefinedFieldId}`;
        const progressId = isEditMode ? "#edit_progress-userDefinedField" : "#progress-userDefinedField";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('UserDefinedFieldDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (userDefinedFieldId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdUserDefinedFieldId").val(response.data.id);
                            $("#edit_hdCourtId").val(response.data.court_id);
                            $("#edit_fieldName").val(response.data.field_name);
                            $("#edit_fieldType").val(response.data.field_type);
                            $("#edit_alignment").val(response.data.alignment);
                            $("#edit_defaultValue").val(response.data.default_value);
                            $("#edit_required").prop("checked", response.data.required == 1);
                            $("#edit_yesAnswerRequired").prop("checked", response.data.yes_answer_required == 1);
                            $("#edit_displayOnDocket").prop("checked", response.data.display_on_docket == 1);
                            $("#edit_displayOnSchedule").prop("checked", response.data.display_on_schedule == 1);
                            $("#edit_useInAttorneyScheduling").prop("checked", response.data.use_in_attorany_scheduling == 1);
                            $("#UserDefinedFieldEditModalLabel").html(`Edit User Defined Field: ${response.data.field_name}`);
                        } else {
                            $("#fieldName").html(response.data.field_name);
                            $("#fieldType").html(response.data.field_type);
                            $("#alignment").html(response.data.alignment);
                            $("#defaultValue").html(response.data.default_value);
                            $("#required").html(response.data.required == 1 ? "Yes" : "No");
                            $("#yesAnswerRequired").html(response.data.yes_answer_required == 1 ? "Yes" : "No");
                            $("#displayOnDocket").html(response.data.display_on_docket == 1 ? "Yes" : "No");
                            $("#displayOnSchedule").html(response.data.display_on_schedule == 1 ? "Yes" : "No");
                            $("#useInAttorneyScheduling").html(response.data.use_in_attorany_scheduling == 1 ? "Yes" : "No");
                            $("#hdUserDefinedFieldId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve user defined field details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving User Defined Field Details", error.statusText || "Failed to retrieve user defined field details.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveUserDefinedField() {
        if ($("#edit_hdUserDefinedFieldId").val() === "") {
            this.CreateUserDefinedField();
        } else {
            this.UpdateUserDefinedField();
        }
        if (userDefinedFieldControllerInstance.userDefinedFieldTable) {
            userDefinedFieldControllerInstance.ClearEditForm();
            userDefinedFieldControllerInstance.userDefinedFieldTable.draw();
        }
    }

    CreateUserDefinedField() {
        try {
            $("#edit_progress-userDefinedField").show();
            const userDefinedFieldData = {
                court_id: parseInt($("#edit_hdCourtId").val()) || 0,
                field_name: $("#edit_fieldName").val().trim(),
                field_type: $("#edit_fieldType").val(),
                alignment: $("#edit_alignment").val(),
                default_value: $("#edit_defaultValue").val().trim(),
                required: $("#edit_required").prop("checked") ? 1 : 0,
                yes_answer_required: $("#edit_yesAnswerRequired").prop("checked") ? 1 : 0,
                display_on_docket: $("#edit_displayOnDocket").prop("checked") ? 1 : 0,
                display_on_schedule: $("#edit_displayOnSchedule").prop("checked") ? 1 : 0,
                use_in_attorany_scheduling: $("#edit_useInAttorneyScheduling").prop("checked") ? 1 : 0
            };
            $.ajax({
                url: `${this.service.baseUrl}UserDefinedFieldAPI/CreateUserDefinedField`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(userDefinedFieldData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-userDefinedField").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'User Defined Field created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('UserDefinedFieldEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (userDefinedFieldControllerInstance.userDefinedFieldTable) {
                            userDefinedFieldControllerInstance.userDefinedFieldTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating user defined field.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-userDefinedField").hide();
                    ShowNotification("Error Creating User Defined Field", error.statusText || "Failed to create user defined field.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-userDefinedField").hide();
            ShowNotification("Error Creating User Defined Field", e.message, 'error');
        }
    }

    UpdateUserDefinedField() {
        try {
            $("#edit_progress-userDefinedField").show();
            const userDefinedFieldData = {
                id: parseInt($("#edit_hdUserDefinedFieldId").val()),
                court_id: parseInt($("#edit_hdCourtId").val()) || 0,
                field_name: $("#edit_fieldName").val().trim(),
                field_type: $("#edit_fieldType").val(),
                alignment: $("#edit_alignment").val(),
                default_value: $("#edit_defaultValue").val().trim(),
                required: $("#edit_required").prop("checked") ? 1 : 0,
                yes_answer_required: $("#edit_yesAnswerRequired").prop("checked") ? 1 : 0,
                display_on_docket: $("#edit_displayOnDocket").prop("checked") ? 1 : 0,
                display_on_schedule: $("#edit_displayOnSchedule").prop("checked") ? 1 : 0,
                use_in_attorany_scheduling: $("#edit_useInAttorneyScheduling").prop("checked") ? 1 : 0
            };
            $.ajax({
                url: `${this.service.baseUrl}UserDefinedFieldAPI/UpdateUserDefinedField`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(userDefinedFieldData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-userDefinedField").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'User Defined Field updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('UserDefinedFieldEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (userDefinedFieldControllerInstance.userDefinedFieldTable) {
                            userDefinedFieldControllerInstance.userDefinedFieldTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating user defined field.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-userDefinedField").hide();
                    ShowNotification("Error Updating User Defined Field", error.statusText || "Failed to update user defined field.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-userDefinedField").hide();
            ShowNotification("Error Updating User Defined Field", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}