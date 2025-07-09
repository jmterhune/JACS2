let userControllerInstance = null;

class UserController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.userRole = params.userRole || 'JACSUser';
        this.jaRole = params.jaRole || 'Judicial Assistant';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.userIdField = -1;
        this.searchTerm = "";
        this.userTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        userControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}UserAPI/DeleteUser/`;

        const listUrl = `${this.service.baseUrl}UserAPI/GetUsers/${this.recordCount}`;
        const detailModalElement = document.getElementById('UserDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('UserEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateUsers();

        this.userTable = $('#tblUser').DataTable({
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
                    $("#tblUser_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Users', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Users', 'The following error occurred attempting to retrieve user information. Error: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="user-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit User" data-toggle="tooltip" data-id="${data}" class="user-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete User" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.userTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const userId = $(this).data("id");
                Swal.fire({
                    title: 'Delete User?',
                    text: 'Are you sure you wish to delete this User?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        userControllerInstance.DeleteUser(userId);
                    }
                });
            });
        });

        $(document).on('click', '.user-detail', function (e) {
            e.preventDefault();
            var userId = $(this).data("id");
            userControllerInstance.ViewUser(userId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('UserEditModal'));
        $(document).on('click', '.user-edit, #editUserBtn', function (e) {
            e.preventDefault();
            var userId = $(this).data("id") || $("#hdUserId").val();
            userControllerInstance.userIdField = userId;
            if (userId) {
                userControllerInstance.ViewUser(userId, true);
                $("#UserEditModalLabel").html(`Edit User`);
                $("#edit_userName").hide();
                $("#edit_userNameText").show();
                $("#userHelp").hide();
            } else {
                userControllerInstance.ClearEditForm();
                $("#UserEditModalLabel").html("Create New User");
                $("#edit_userName").show();
                $("#edit_userNameText").hide();
                $("#userHelp").show();
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            userControllerInstance.ClearEditForm();
            $("#UserEditModalLabel").html("Create New User");
            $("#edit_userName").show();
            $("#edit_userNameText").hide();
            $("#userHelp").show();
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var userId = $("#hdUserId").val();
            Swal.fire({
                title: 'Delete User?',
                text: 'Are you sure you wish to delete this User?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    userControllerInstance.DeleteUser(userId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $userName = $("#edit_userNameText").is(":visible") ? $("#edit_userNameText") : $("#edit_userName");
            const $userNameError = $userName.nextAll(".invalid-feedback").first();
            if ($userName.val().trim() === "") {
                $userNameError.show();
                $userName.addClass("is-invalid");
                isValid = false;
            } else {
                $userNameError.hide();
                $userName.removeClass("is-invalid");
            }

            const $userEmail = $("#edit_userEmail");
            const $userEmailError = $userEmail.next(".invalid-feedback");
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if ($userEmail.val().trim() === "" || !emailRegex.test($userEmail.val().trim())) {
                $userEmailError.show();
                $userEmail.addClass("is-invalid");
                isValid = false;
            } else {
                $userEmailError.hide();
                $userEmail.removeClass("is-invalid");
            }

            if (isValid) {
                userControllerInstance.SaveUser();
            }
        });

        $("#edit_userName").on("change", function () {
            const $this = $(this);
            const selectedOption = $this.find('option:selected');
            const email = selectedOption.data('email') || '';
            $("#edit_userEmail").val(email);
            if ($this.val().trim() !== "") {
                $this.nextAll(".invalid-feedback").first().hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_userNameText").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.nextAll(".invalid-feedback").first().hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_userEmail").on("input", function () {
            const $this = $(this);
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if ($this.val().trim() !== "" && emailRegex.test($this.val().trim())) {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    populateUsers() {
        $.ajax({
            url: `${this.service.baseUrl}UserAPI/GetUsersByRole/${this.jaRole},${this.userRole}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (response) {
                const $userSelect = $("#edit_userName");
                $userSelect.empty();
                $userSelect.append('<option value="">Select User</option>');
                if (response.data) {
                    response.data.forEach(user => {
                        $userSelect.append(`<option value="${user.id}" data-email="${user.email}">${user.name}</option>`);
                    });
                }
            },
            error: function () {
                console.error('Failed to fetch users');
            }
        });
    }

    ClearState() {
        if (this.userTable) {
            this.userTable.state.clear();
            window.location.reload();
        }
    }

    DeleteUser(userId) {
        $.ajax({
            url: this.deleteUrl + userId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (userControllerInstance.userTable) {
                    userControllerInstance.userTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('UserEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('UserDetailModal'));
                if (editModal) {
                    editModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'User deleted successfully.'
                });
            },
            error: function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error Deleting User',
                    text: error.statusText
                });
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'UserDetailModal') {
            userControllerInstance.ClearDetailForm();
        } else if (modalId === 'UserEditModal') {
            userControllerInstance.ClearEditForm();
            userControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#userName").html("");
        $("#userEmail").html("");
        $("#hdUserId").val("");
    }

    ClearEditForm() {
        $("#edit_userName").val("");
        $("#edit_userNameText").val("");
        $("#edit_userEmail").val("");
        $("#edit_hdUserId").val("");
    }

    ClearEditValidations() {
        $("#edit_userName").removeClass("is-invalid");
        $("#edit_userNameText").removeClass("is-invalid");
        $("#edit_userName").nextAll(".invalid-feedback").first().hide();
        $("#edit_userEmail").removeClass("is-invalid");
        $("#edit_userEmail").next(".invalid-feedback").hide();
    }

    ViewUser(userId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}UserAPI/GetUser/${userId}`;
        const progressId = isEditMode ? "#edit_progress-user" : "#progress-user";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('UserDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (userId) {
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
                            $("#edit_hdUserId").val(response.data.id);
                            $("#edit_userNameText").val(response.data.name);
                            $("#edit_userEmail").val(response.data.email);
                            $("#UserEditModalLabel").html(`Edit User: ${response.data.name}`);
                        } else {
                            $("#userName").html(response.data.name);
                            $("#userEmail").html(response.data.email);
                            $("#hdUserId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve user details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch user details');
                    ShowNotification("Error", "Failed to retrieve user details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveUser() {
        const isEditMode = $("#edit_userNameText").is(":visible");
        if (!isEditMode) {
            this.CreateUser();
        } else {
            this.UpdateUser();
        }
        if (userControllerInstance.userTable) {
            userControllerInstance.ClearEditForm();
            userControllerInstance.userTable.draw();
        }
    }

    CreateUser() {
        try {
            $("#edit_progress-user").show();
            const userData = {
                id: $("#edit_userName").val(),
                name: $("#edit_userName option:selected").text(),
                email: $("#edit_userEmail").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}UserAPI/CreateUser`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(userData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-user").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'User created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('UserEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-user").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-user").hide();
                    ShowNotification("Error Creating User", error.responseJSON, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-user").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Creating User',
                text: error.statusText
            });
        }
    }

    UpdateUser() {
        try {
            $("#edit_progress-user").show();
            const userData = {
                id: $("#edit_hdUserId").val(),
                name: $("#edit_userNameText").val(),
                email: $("#edit_userEmail").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}UserAPI/UpdateUser`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(userData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-user").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'User updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('UserEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-user").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-user").hide();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Updating User',
                        text: error.statusText
                    });
                }
            });
        } catch (e) {
            $("#edit_progress-user").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Updating User',
                text: e.statusText
            });
        }
    }
}