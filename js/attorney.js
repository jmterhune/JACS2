let attorneyControllerInstance = null;

class AttorneyController {
    constructor(params = {}) {
        // Default values with parameter overrides
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.editUrl = params.editUrl || '';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 3;
        this.currentPage = params.currentPage || 0;
        this.attorneyId = -1;
        this.searchTerm = "";
        this.attorneyTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        attorneyControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}AttorneyAPI/DeleteAttorney/`;

        // Initialize DataTable for attorney list view
        const listUrl = `${this.service.baseUrl}AttorneyAPI/GetAttorneys/${this.recordCount}`;
        const detailModalElement = document.getElementById('AttorneyDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('AttorneyEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.attorneyTable = $('#tblAttorney').DataTable({
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
                    $("#tblAttorney_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Attorneys", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Attorneys", "The following error occurred attempting to retrieve attorney information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="atty-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Attorney" data-toggle="tooltip" data-id="${data}" class="atty-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "enabled",
                    render: function (data) {
                        return data ? '<i title="Active" data-toggle="tooltip" class="text-success far fa-circle-check"></i>' : '<i title="Disabled" data-toggle="tooltip" class="text-danger fas fa-ban"></i>';
                    },
                    className: "command-item"
                },
                {
                    data: "name",
                    render: function (data) {
                        return `<span class="text-capitalize">${data.toLowerCase()}</span>`;
                    }
                },
                {
                    data: "bar_num",
                    render: function (data) {
                        return `<a target="_blank" data-toggle="tooltip" href="https://www.floridabar.org/directories/find-mbr/?barNum=${data}" title="View Florida Bar Record (Opens in new Window)" class="bar_num-link">${data}</a>`;
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Attorney" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        $.fn.dataTable.ext.errMode = 'none';

        $(".dt-length").prepend($("#lnkAdd"));
        this.attorneyTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const attorneyId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Attorney?',
                    text: 'Are you sure you wish to delete this Attorney?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        attorneyControllerInstance.DeleteAttorney(attorneyId);
                    }
                });
            });
        });
        $(document).on('dt-error', function (e, settings, technical, message) {
            ShowNotification("Error", "An error occurred: " + message, 'error');
            return false;
        });

        $(document).on('click', '.atty-detail', function (e) {
            e.preventDefault();
            var attorneyId = $(this).data("id");
            attorneyControllerInstance.ViewAttorney(attorneyId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('AttorneyEditModal'));
        $(document).on('click', '.atty-edit, #editAttorneyBtn', function (e) {
            e.preventDefault();
            var attorneyId = $(this).data("id") || $("#hdAttorneyId").val();
            attorneyControllerInstance.attorneyId = attorneyId;
            if (attorneyId) {
                attorneyControllerInstance.ViewAttorney(attorneyId, true);
                $("#AttorneyEditModalLabel").html(`Edit Attorney`);
            } else {
                attorneyControllerInstance.ClearEditForm();
                $("#AttorneyEditModalLabel").html("Create New Attorney");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            attorneyControllerInstance.ClearEditForm();
            $("#AttorneyEditModalLabel").html("Create New Attorney");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var attorneyId = $("#hdAttorneyId").val();
            Swal.fire({
                title: 'Delete Attorney?',
                text: 'Are you sure you wish to delete this Attorney?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    attorneyControllerInstance.DeleteAttorney(attorneyId);
                }
            });
        });

        $("input[name='edit_enabled']").on("change", function () {
            $("#edit_radio-error").hide();
            $("input[name='edit_enabled']").removeClass("is-invalid");
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            // Validate Account Status (radio buttons)
            const isRadioSelected = $("input[name='edit_enabled']:checked").length > 0;
            const $radioError = $("#edit_radio-error");
            const $radioInputs = $("input[name='edit_enabled']");
            if (!isRadioSelected) {
                $radioError.show();
                $radioInputs.addClass("is-invalid");
                isValid = false;
            } else {
                $radioError.hide();
                $radioInputs.removeClass("is-invalid");
            }

            // Validate Name
            const $attyName = $("#edit_attyName");
            const $attyNameError = $attyName.next(".invalid-feedback");
            if ($attyName.val().trim() === "") {
                $attyNameError.show();
                $attyName.addClass("is-invalid");
                isValid = false;
            } else {
                $attyNameError.hide();
                $attyName.removeClass("is-invalid");
            }

            // Validate User ID
            const $attyUserId = $("#edit_attyUserId");
            const $attyUserIdError = $attyUserId.next(".invalid-feedback");
            if ($attyUserId.val().trim() === "" || isNaN($attyUserId.val()) || parseInt($attyUserId.val()) < 0) {
                $attyUserIdError.show();
                $attyUserId.addClass("is-invalid");
                isValid = false;
            } else {
                $attyUserIdError.hide();
                $attyUserId.removeClass("is-invalid");
            }

            // Validate Bar Number
            const $attyBar = $("#edit_attyBar");
            const $attyBarError = $attyBar.next(".invalid-feedback");
            if ($attyBar.val().trim() === "") {
                $attyBarError.show();
                $attyBar.addClass("is-invalid");
                isValid = false;
            } else {
                $attyBarError.hide();
                $attyBar.removeClass("is-invalid");
            }

            // Validate Email Addresses
            const $hdEmails = $("#edit_hdEmails");
            const $emailError = $("#edit_email-error");
            const $emailList = $("#edit_email-list");
            const hasEmails = $hdEmails.val().trim() !== "" || $emailList.children().length > 0;
            if (!hasEmails) {
                $emailError.show();
                $emailList.addClass("is-invalid");
                isValid = false;
            } else {
                $emailError.hide();
                $emailList.removeClass("is-invalid");
            }
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            const emails = $hdEmails.val().split(",").filter(e => e.trim() !== "");
            const allEmailsValid = emails.every(email => emailRegex.test(email));
            if (!allEmailsValid) {
                $emailError.text("All email addresses must be valid.");
                $emailError.show();
                $emailList.addClass("is-invalid");
                isValid = false;
            }

            if (isValid) {
                attorneyControllerInstance.SaveAttorney();
            }
        });

        $("#edit_attyName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_attyUserId").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "" && !isNaN($this.val()) && parseInt($this.val()) >= 0) {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_attyBar").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
        $("#edit_user_lookup").on("click", function (e) {
            e.preventDefault();
            const barNumber = $("#edit_attyBar").val();
            if (barNumber.trim() !== "") {
                // Trigger GetSiteUser only in create mode
                if ($("#edit_hdAttorneyId").val() === "") {
                    attorneyControllerInstance.GetSiteUser(barNumber);
                }
            }
        });

        $("#edit_new-email").on("click", function () {
            $("#edit_email-list").append('<li><input type="text" class="form-control me-3 d-inline-block" value=""><a href="#" data-toggle="tooltip" class="delete-email" title="Delete Email Address" role="button" aria-disabled="true" aria-label="Delete Email Address"><i class="fas fa-trash"></i></a></li>');
            setTimeout(() => {
                if ($("#edit_hdEmails").val().trim() !== "" || $("#edit_email-list").children().length > 0) {
                    $("#edit_email-error").hide();
                    $("#edit_email-list").removeClass("is-invalid");
                }
            }, 100);
        });

        $(document).on('click', '.delete-email', function (e) {
            e.preventDefault();
            Swal.fire({
                title: 'Delete Email Address?',
                text: 'Are you sure you want to delete this email address?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    $(this).closest('li').remove();
                }
            });
        });
    }

    ClearState() {
        if (this.attorneyTable) {
            this.attorneyTable.state.clear();
            window.location.reload();
        }
    }

    DeleteAttorney(attorneyId) {
        $.ajax({
            url: this.deleteUrl + attorneyId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (attorneyControllerInstance.attorneyTable) {
                    attorneyControllerInstance.attorneyTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('AttorneyEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('AttorneyDetailModal'));
                if (detailModal) {
                    detailModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Attorney deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Attorney", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'AttorneyDetailModal') {
            attorneyControllerInstance.ClearDetailForm();
        } else if (modalId === 'AttorneyEditModal') {
            attorneyControllerInstance.ClearEditForm();
            attorneyControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#attyName").html("");
        $("#attyUserId").html("");
        $("#attyBar").html("");
        $("#attyPhone").html("");
        $("#attySchedule").html("");
        $("#attyEnabled").html("");
        $("#attyNotes").html("");
        $("#hdAttorneyId").val("");
        $("#attyEmails").html("");
    }

    ClearEditForm() {
        $("#edit_attyName").val("");
        $("#edit_attyUserId").val("");
        $("#edit_attyBar").val("");
        $("#edit_attyPhone").val("");
        $("#edit_attyNotes").val("");
        $("#edit_hdAttorneyId").val("");
        $("#edit_hdScheduling").val("");
        $("#edit_email-list").html("");
        $("#edit_hdEmails").val("");
        $('input[name="edit_enabled"]').prop('checked', false);
    }

    ClearEditValidations() {
        // Clear account status validation
        $("input[name='edit_enabled']").removeClass("is-invalid");
        $("#edit_radio-error").hide();

        // Clear name validation
        $("#edit_attyName").removeClass("is-invalid");
        $("#edit_attyName").next(".invalid-feedback").hide();

        // Clear user ID validation
        $("#edit_attyUserId").removeClass("is-invalid");
        $("#edit_attyUserId").next(".invalid-feedback").hide();

        // Clear bar number validation
        $("#edit_attyBar").removeClass("is-invalid");
        $("#edit_attyBar").next(".invalid-feedback").hide();

        // Clear email validation
        $("#edit_email-list").removeClass("is-invalid");
        $("#edit_email-error").hide();
        $("#edit_email-error").text("At least one email address is required.");
    }

    GetSiteUser(barNumber) {
        if (!barNumber) return;
        const getUrl = `${this.service.baseUrl}AttorneyAPI/GetSiteUser/${encodeURIComponent(barNumber)}`;
        $("#edit_progress-attorney").show();

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
                    $("#edit_attyName").val(`${response.data.lastname}, ${response.data.firstname}`);
                    $("#edit_attyUserId").val(response.data.userid);
                    $("#edit_email-list").html(`<li><input type="text" class="form-control me-3 d-inline-block" value="${response.data.email}"><a href="#" role="button" data-toggle="tooltip" class="delete-email" aria-disabled="true" aria-label="Delete Email Address" title="Delete Email Address"><i class="fas fa-trash"></i></a></li>`);
                    $("#edit_attyName").removeClass("is-invalid");
                    $("#edit_attyName").next(".invalid-feedback").hide();
                    $("#edit_attyUserId").removeClass("is-invalid");
                    $("#edit_attyUserId").next(".invalid-feedback").hide();
                    $("#edit_email-list").removeClass("is-invalid");
                    $("#edit_email-error").hide();
                } else {
                    ShowNotification("No User Found", "No user found for the provided Bar Number.", 'error');
                }
                $("#edit_progress-attorney").hide();
            },
            error: function () {
                ShowNotification("Error", "Failed to retrieve user details. Please try again later.", 'error');
                $("#edit_progress-attorney").hide();
            }
        });
    }

    ViewAttorney(attorneyId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}AttorneyAPI/GetAttorney/${attorneyId}`;
        const progressId = isEditMode ? "#edit_progress-attorney" : "#progress-attorney";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('AttorneyDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (attorneyId) {
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
                            $("#edit_attyName").val(response.data.name);
                            $("#edit_attyUserId").val(response.data.UserId);
                            $("#edit_attyBar").val(response.data.bar_num);
                            $("#edit_attyPhone").val(response.data.phone);
                            $("#edit_attyNotes").val(response.data.notes);
                            $("#edit_hdAttorneyId").val(response.data.id);
                            $("#edit_hdScheduling").val(response.data.scheduling);
                            if (response.data.enabled) {
                                $("#edit_radio_enabled").prop('checked', true);
                                $("#edit_radio_disabled").prop('checked', false);
                            } else {
                                $("#edit_radio_disabled").prop('checked', true);
                                $("#edit_radio_enabled").prop('checked', false);
                            }
                            if (response.data.email_list?.length > 0) {
                                $("#edit_email-list").html(response.data.email_list.map(email => `<li><input type="text" class="form-control me-3 d-inline-block" value="${email}"><a href="#" role="button" data-toggle="tooltip" class="delete-email" aria-disabled="true" aria-label="Delete Email Address" title="Delete Email Address"><i class="fas fa-trash"></i></a></li>`).join(''));
                            }
                            $("#AttorneyEditModalLabel").html(`Edit Attorney: ${response.data.name}`);
                        } else {
                            $("#attyName").html(response.data.name);
                            $("#attyUserId").html(response.data.UserId);
                            $("#attyBar").html(response.data.bar_num);
                            $("#attyPhone").html(response.data.phone);
                            $("#attySchedule").html(response.data.scheduling ? "Yes" : "No");
                            $("#attyEnabled").html(response.data.enabled ? "Active" : "Disabled");
                            $("#attyNotes").html(response.data.notes);
                            $("#hdAttorneyId").val(response.data.id);
                            if (response.data.emails?.length > 0) {
                                $("#attyEmails").html(response.data.emails.map(email => `<span class="d-inline-flex">${email}</span>`).join(' '));
                            }
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve attorney details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch attorney details');
                    ShowNotification("Error", "Failed to retrieve attorney details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveAttorney() {
        if ($("#edit_hdAttorneyId").val() === "") {
            this.CreateAttorney();
        } else {
            this.UpdateAttorney();
        }
        if (attorneyControllerInstance.attorneyTable) {
            attorneyControllerInstance.ClearEditForm();
            attorneyControllerInstance.attorneyTable.draw();
        }
    }

    CreateAttorney() {
        try {
            $("#edit_progress-attorney").show();
            const attorneyData = {
                UserId: parseInt($("#edit_attyUserId").val()),
                name: $("#edit_attyName").val(),
                bar_num: $("#edit_attyBar").val(),
                phone: $("#edit_attyPhone").val(),
                scheduling: $("#edit_hdScheduling").val(),
                enabled: $('input[name="edit_enabled"]:checked').val() === "1",
                notes: $("#edit_attyNotes").val(),
                emails: $("#edit_email-list li input[type='text']").map(function () { return $(this).val(); }).get()
            };
            $.ajax({
                url: `${this.service.baseUrl}AttorneyAPI/CreateAttorney`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(attorneyData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-attorney").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Attorney created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('AttorneyEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-attorney").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-attorney").hide();
                    ShowNotification("Error Creating Attorney", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-attorney").hide();
            ShowNotification("Error Creating Attorney", e.statusText, 'error');
        }
    }

    UpdateAttorney() {
        try {
            $("#edit_progress-attorney").show();
            const attorneyData = {
                id: $("#edit_hdAttorneyId").val(),
                UserId: parseInt($("#edit_attyUserId").val()),
                name: $("#edit_attyName").val(),
                bar_num: $("#edit_attyBar").val(),
                phone: $("#edit_attyPhone").val(),
                scheduling: $("#edit_hdScheduling").val(),
                enabled: $('input[name="edit_enabled"]:checked').val() === "1",
                notes: $("#edit_attyNotes").val(),
                emails: $("#edit_email-list li input[type='text']").map(function () { return $(this).val(); }).get()
            };
            $.ajax({
                url: `${this.service.baseUrl}AttorneyAPI/UpdateAttorney`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(attorneyData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-attorney").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Attorney updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('AttorneyEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-attorney").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-attorney").hide();
                    ShowNotification("Error Updating Attorney", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-attorney").hide();
            ShowNotification("Error Updating Attorney", e.statusText, 'error');
        }
    }
}