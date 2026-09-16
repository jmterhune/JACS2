let countyControllerInstance = null;

class CountyController {
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
        this.countyId = -1;
        this.searchTerm = "";
        this.countyTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        this.updateUrl = null;
        this.createUrl = null;
        this.viewUrl = null;
        this.testAuthUrl = null;
        countyControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CountyAPI/DeleteCounty/`;
        this.updateUrl = `${this.service.baseUrl}CountyAPI/UpdateCounty`;
        this.createUrl = `${this.service.baseUrl}CountyAPI/CreateCounty`;
        this.viewUrl = `${this.service.baseUrl}CountyAPI/GetCounty/`;
        this.testAuthUrl = `${this.service.baseUrl}CountyAPI/TestAuthToken`;
        const listUrl = `${this.service.baseUrl}CountyAPI/GetCounties/${this.recordCount}`;
        const detailModalElement = document.getElementById('CountyDetailModal');
        const editModalElement = document.getElementById('CountyEditModal');
        const editModal = new bootstrap.Modal(document.getElementById('CountyEditModal'));

        if (detailModalElement) detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        if (editModalElement) editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);

        this.countyTable = $('#tblCounty').DataTable({
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
                    $("#tblCounty_processing").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve counties.';
                    if (error.status === 401) errorMessage = 'Please make sure you are logged in and try again.';
                    ShowNotification('Error Retrieving Counties', errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: data => `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="county-detail btn-command"><i class="fas fa-eye"></i></button>`,
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: data => `<button type="button" title="Edit County" data-toggle="tooltip" data-id="${data}" class="county-edit btn-command"><i class="fas fa-pencil"></i></button>`,
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "name",
                    render: data => data || ''
                },
                {
                    data: "code",
                    render: data => data || ''
                },
                {
                    data: "auth_end_point_url",
                    render: data => data || ''
                },
                {
                    data: "id",
                    render: (data, type, row) => isAdmin ? `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete County" data-id="${row.id}"><i class="fas fa-trash"></i></button>` : '',
                    className: "command-item",
                    orderable: false
                }
            ],
            language: { emptyTable: "No Records Available.", zeroRecords: "No records match the search criteria you entered." },
            order: [[2, 'asc']],
            serverSide: false,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: 25,
        });
        this.countyTable.search('').draw(false);   // ← Clears saved search term

        // The browser was autofilling the DataTables search box (with "sitehost"),
        // because an unnamed text input matches its saved-value heuristics. Give it a
        // distinct name so there is nothing remembered to match, and opt out of
        // autofill/autocorrect.
        $('#tblCounty_wrapper').find('input[type="search"], input.dt-input')
            .attr('name', 'countySearch')
            .attr('autocomplete', 'off')
            .attr('autocorrect', 'off')
            .attr('autocapitalize', 'off')
            .attr('spellcheck', 'false');
        this.countyTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const countyId = $(this).data("id");
                Swal.fire({ title: 'Delete County?', text: 'Are you sure?', icon: 'warning', showCancelButton: true }).then((result) => {
                    if (result.isConfirmed) countyControllerInstance.deleteCounty(countyId);
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

        $(document).on('click', '.county-detail', function (e) {
            e.preventDefault();
            var countyId = $(this).data("id");
            countyControllerInstance.viewCounty(countyId, false);
        });

        $(document).on('click', '.county-edit, #editCountyBtn', function (e) {
            e.preventDefault();
            var countyId = $(this).data("id") || $("#hdCountyId").val();
            countyControllerInstance.countyId = countyId;
            if (countyId) {
                countyControllerInstance.viewCounty(countyId, true);
                $("#CountyEditModalLabel").html(`Edit County`);
            } else {
                countyControllerInstance.clearEditForm();
                $("#CountyEditModalLabel").html("Create New County");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            countyControllerInstance.clearEditForm();
            $("#CountyEditModalLabel").html("Create New County");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var countyId = $("#hdCountyId").val();
            Swal.fire({
                title: 'Delete County?',
                text: 'Are you sure you wish to delete this County?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    countyControllerInstance.deleteCounty(countyId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            // Name validation
            const $countyName = $("#edit_countyName");
            const $countyNameError = $countyName.next(".invalid-feedback");
            if ($countyName.val().trim() === "") {
                $countyNameError.show();
                $countyName.addClass("is-invalid");
                isValid = false;
            } else {
                $countyNameError.hide();
                $countyName.removeClass("is-invalid");
            }

            // Code validation
            const $countyCode = $("#edit_countyCode");
            const $countyCodeError = $countyCode.next(".invalid-feedback");
            if ($countyCode.val().trim() === "") {
                $countyCodeError.show();
                $countyCode.addClass("is-invalid");
                isValid = false;
            } else {
                $countyCodeError.hide();
                $countyCode.removeClass("is-invalid");
            }

            // Auth Endpoint URL validation (optional but must be valid URL if filled)
            const $authUrl = $("#edit_countyAuthUrl");
            const $authUrlError = $authUrl.next(".invalid-feedback") || $authUrl.parent().find(".invalid-feedback");
            const authValue = $authUrl.val().trim();
            if (authValue !== "" && !countyControllerInstance.isValidUrl(authValue)) {
                $authUrlError.text("Please enter a valid URL (e.g. https://api.example.com)").show();
                $authUrl.addClass("is-invalid");
                isValid = false;
            } else {
                $authUrlError.hide();
                $authUrl.removeClass("is-invalid");
            }

            if (isValid) {
                countyControllerInstance.saveCounty();
            }
        });

        $("#edit_cmdTestAuth").on("click", function (e) {
            e.preventDefault();
            countyControllerInstance.testAuthToken();
        });

        // The test posts the credentials typed on the form, so it only becomes
        // available once both are present.
        $("#edit_countyUserName, #edit_countyPassword").on("input", function () {
            countyControllerInstance.toggleTestAuthEnabled();
        });

        $("#edit_countyAuthUrl").on("input", function () {
            const $this = $(this);
            const value = $this.val().trim();
            const $error = $this.next(".invalid-feedback") || $this.parent().find(".invalid-feedback");
            if (value === "" || countyControllerInstance.isValidUrl(value)) {
                $error.hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#edit_countyName, #edit_countyCode").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    viewCounty(countyId, isEditMode = false) {
        const progressId = isEditMode ? "#edit_progress_county" : "#progress_county";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CountyDetailModal'));
            if (!modal._element.classList.contains('show')) modal.show();
        }

        if (countyId) {
            $.ajax({
                url: this.viewUrl + countyId,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCountyId").val(response.data.id);
                            $("#edit_countyName").val(response.data.name);
                            $("#edit_countyCode").val(response.data.code);
                            $("#edit_countyAuthUrl").val(response.data.auth_end_point_url || '');
                            $("#edit_countyUserName").val(response.data.user_name || '');
                            $("#edit_countyPassword").val('');
                            // Token + expiration are read-only here: they are issued by
                            // the auth endpoint, never edited on this form.
                            $("#edit_countyToken").val(response.data.token || '');
                            // Keep the raw value next to the formatted display so Save
                            // posts something the server can parse.
                            $("#edit_countyExpiration")
                                .val(countyControllerInstance.formatDateTime(response.data.expiration_date))
                                .data("raw", response.data.expiration_date || null);
                            // Testing uses the entered credentials, so the button stays
                            // disabled until a user name and password are typed.
                            countyControllerInstance.toggleTestAuthEnabled();
                            $("#CountyEditModalLabel").html(`Edit County: ${response.data.name}`);
                        } else {
                            $("#countyName").html(response.data.name);
                            $("#countyCode").html(response.data.code);
                            $("#countyAuthUrl").html(response.data.auth_end_point_url || '');
                            $("#countyUserName").html(response.data.user_name || '');
                            $("#countyPassword").html(response.data.password || '');
                            $("#countyToken").html(response.data.token || '');
                            $("#countyExpiration").html(countyControllerInstance.formatDateTime(response.data.expiration_date));
                            $("#hdCountyId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve county details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving County Details", error.statusText || "Failed to retrieve county details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    saveCounty() {
        if ($("#edit_hdCountyId").val() === "") {
            this.createCounty();
        } else {
            this.updateCounty();
        }
        if (countyControllerInstance.countyTable) {
            countyControllerInstance.clearEditForm();
        }
    }

    createCounty() {
        try {
            $("#edit_progress_county").show();
            const countyData = {
                name: $("#edit_countyName").val().trim(),
                code: $("#edit_countyCode").val().trim(),
                auth_end_point_url: $("#edit_countyAuthUrl").val().trim(),
                user_name: $("#edit_countyUserName").val().trim(),
                password: $("#edit_countyPassword").val().trim() || null,
                // Populated by "Test Auth Endpoint" and saved with the record.
                token: $("#edit_countyToken").val().trim() || null,
                expiration_date: $("#edit_countyExpiration").data("raw") || null
            };
            $.ajax({
                url: this.createUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(countyData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_county").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'County created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                        if (editModal) editModal.hide();
                        if (countyControllerInstance.countyTable) countyControllerInstance.countyTable.ajax.reload();
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating county.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_county").hide();
                    ShowNotification("Error Creating County", error.statusText || "Failed to create county.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_county").hide();
            ShowNotification("Error Creating County", e.message, 'error');
        }
    }

    updateCounty() {
        try {
            $("#edit_progress_county").show();
            const countyData = {
                id: parseInt($("#edit_hdCountyId").val()),
                name: $("#edit_countyName").val().trim(),
                code: $("#edit_countyCode").val().trim(),
                auth_end_point_url: $("#edit_countyAuthUrl").val().trim(),
                user_name: $("#edit_countyUserName").val().trim(),
                password: $("#edit_countyPassword").val().trim() || null,
                // Populated by "Test Auth Endpoint" and saved with the record. When it is
                // empty the server keeps whatever token is already stored.
                token: $("#edit_countyToken").val().trim() || null,
                expiration_date: $("#edit_countyExpiration").data("raw") || null
            };
            $.ajax({
                url: this.updateUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(countyData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_county").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'County updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                        if (editModal) editModal.hide();
                        if (countyControllerInstance.countyTable) countyControllerInstance.countyTable.ajax.reload();
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating county.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_county").hide();
                    ShowNotification("Error Updating County", error.statusText || "Failed to update county.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_county").hide();
            ShowNotification("Error Updating County", e.message, 'error');
        }
    }

    deleteCounty(countyId) {
        $.ajax({
            url: this.deleteUrl + countyId,
            type: 'DELETE',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                    if (editModal) editModal.hide();
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('CountyDetailModal'));
                    if (detailModal) detailModal.hide();
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'County deleted successfully.'
                    });
                    if (countyControllerInstance.countyTable) {
                        countyControllerInstance.countyTable.ajax.reload();
                    }
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting County", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CountyDetailModal') {
            countyControllerInstance.clearDetailForm();
        } else if (modalId === 'CountyEditModal') {
            countyControllerInstance.clearEditForm();
            countyControllerInstance.clearEditValidations();
        }
    }

    clearDetailForm() {
        $("#countyName").html("");
        $("#countyCode").html("");
        $("#countyAuthUrl").html("");
        $("#countyUserName").html("");
        $("#countyPassword").html("");
        $("#countyToken").html("");
        $("#countyExpiration").html("");
        $("#hdCountyId").val("");
    }

    clearEditForm() {
        $("#edit_hdCountyId").val("");
        $("#edit_countyName").val("");
        $("#edit_countyCode").val("");
        $("#edit_countyAuthUrl").val("");
        $("#edit_countyUserName").val("");
        $("#edit_countyPassword").val("");
        $("#edit_countyToken").val("");
        $("#edit_countyExpiration").val("").removeData("raw");
        $("#edit_cmdTestAuth").prop("disabled", true);
    }

    clearEditValidations() {
        $("#edit_countyName").removeClass("is-invalid");
        $("#edit_countyName").next(".invalid-feedback").hide();
        $("#edit_countyCode").removeClass("is-invalid");
        $("#edit_countyCode").next(".invalid-feedback").hide();
    }

    toggleTestAuthEnabled() {
        const hasUserName = $("#edit_countyUserName").val().trim() !== "";
        const hasPassword = $("#edit_countyPassword").val().trim() !== "";
        $("#edit_cmdTestAuth").prop("disabled", !(hasUserName && hasPassword));
    }

    testAuthToken() {
        const authUrl = $("#edit_countyAuthUrl").val().trim();
        const userName = $("#edit_countyUserName").val().trim();
        const password = $("#edit_countyPassword").val().trim();

        if (!userName || !password) {
            Swal.fire({
                icon: 'warning',
                title: 'Credentials Required',
                text: 'Enter a user name and password before testing the auth endpoint.'
            });
            return;
        }

        const countyId = $("#edit_hdCountyId").val();
        $("#edit_progress_county").show();
        $("#edit_cmdTestAuth").prop("disabled", true);

        $.ajax({
            url: this.testAuthUrl,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify({
                id: countyId ? parseInt(countyId) : 0,
                auth_end_point_url: authUrl,
                user_name: userName,
                password: password
            }),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                $("#edit_progress_county").hide();
                countyControllerInstance.toggleTestAuthEnabled();
                if (response && response.status === 200) {
                    $("#edit_countyToken").val(response.token || '');
                    $("#edit_countyExpiration")
                        .val(countyControllerInstance.formatDateTime(response.expiration_date))
                        .data("raw", response.expiration_date || null);
                    Swal.fire({
                        icon: 'success',
                        title: 'Auth Succeeded',
                        text: response.message || 'Token retrieved. Save the county to store it.'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Auth Failed',
                        text: response.message || 'The auth endpoint did not return a token.'
                    });
                }
            },
            error: function (error) {
                $("#edit_progress_county").hide();
                countyControllerInstance.toggleTestAuthEnabled();
                const msg = (error.responseJSON && error.responseJSON.message)
                    ? error.responseJSON.message
                    : (error.statusText || "Failed to reach the auth endpoint.");
                Swal.fire({ icon: 'error', title: 'Auth Failed', text: msg });
            }
        });
    }

    formatDateTime(value) {
        // The expiration is stored and sent as UTC; formatEasternDateTime (jacs.js)
        // converts it into EST/EDT so the admin reads it in court time.
        return formatEasternDateTime(value);
    }

    isValidUrl(url) {
        if (!url || typeof url !== 'string') return false;
        try {
            new URL(url.trim());
            return true;
        } catch (_) {
            return false;
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}