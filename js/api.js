let apiConfigControllerInstance = null;

class ApiConfigController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.editUrl = params.editUrl || '';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 3;
        this.currentPage = params.currentPage || 0;
        this.apiId = -1;
        this.searchTerm = "";
        this.apiTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        this.updateUrl = null;
        this.createUrl = null;
        this.viewUrl = null;
        this.countyListUrl = null;
        apiConfigControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}EndpointAPI/DeleteApiEndpoint/`;
        this.updateUrl = `${this.service.baseUrl}EndpointAPI/UpdateApiEndpoint/`;
        this.createUrl = `${this.service.baseUrl}EndpointAPI/CreateApiEndpoint`;
        this.viewUrl = `${this.service.baseUrl}EndpointAPI/GetApiEndpoint/`;
        this.countyListUrl = `${this.service.baseUrl}CountyAPI/GetCountyDropDownItems`;
        const listUrl = `${this.service.baseUrl}EndpointAPI/GetApiEndpoints/${this.recordCount}`;
        const editModalElement = document.getElementById('ApiEditModal');
        const editModal = new bootstrap.Modal(document.getElementById('ApiEditModal'));

        this.loadCountyDropdown();
        this.loadTypeDropdown();
        this.apiTable = $('#tblApi').DataTable({
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
                    $("#edit_progress_api").hide();
                    let errorMessage = error.statusText || 'Failed to retrieve API Endpoint Records.';
                    if (error.status === 401) {
                        errorMessage = 'Please make sure you are logged in and try again.';
                    }
                    ShowNotification("Error Retrieving API Endpoint Records", errorMessage, 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit API Endpoint" data-toggle="tooltip" data-id="${data}" class="api-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "end_point_url",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "type_desc",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "county_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete API Endpoint" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[1, 'asc']],
            serverSide: false,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: 25,
        });

        $.fn.dataTable.ext.errMode = 'none';

        this.apiTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const apiId = $(this).data("id");
                Swal.fire({
                    title: 'Delete API Endpoint?',
                    text: 'Are you sure you wish to delete this Endpoint?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        apiConfigControllerInstance.deleteApiEndpoint(apiId);
                    }
                });
            });
        });

        $(".dt-length").prepend($("#lnkAdd"));

        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        $(document).on('dt-error', function (e, settings, technical, message) {
            ShowNotification("Error", "An error occurred: " + message, 'error');
            return false;
        });

        $(document).on('click', '.api-edit', function (e) {
            e.preventDefault();
            var apiId = $(this).data("id") || $("#edit_hdApiId").val();
            apiConfigControllerInstance.apiId = apiId;
            if (apiId) {
                apiConfigControllerInstance.viewApiEndpoint(apiId);
            } else {
                apiConfigControllerInstance.clearEditForm();
                $("#ApiEditModalLabel").html("Create New API Endpoint");
            }
            editModal.show();
        });

        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            apiConfigControllerInstance.clearEditForm();
            $("#ApiEditModalLabel").html("Create New API Endpoint");
            editModal.show();
        });

        $("#edit_cmdDelete").on("click", function (e) {
            e.preventDefault();
            var apiId = $("#edit_hdApiId").val();
            Swal.fire({
                title: 'Delete API Endpoint?',
                text: 'Are you sure you wish to delete this API Endpoint?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    apiConfigControllerInstance.deleteApiEndpoint(apiId);  // ← fixed
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $endpointUrl = $("#edit_end_point");
            const $endpointUrlError = $endpointUrl.next(".invalid-feedback");
            const urlValue = $endpointUrl.val().trim();
            if (urlValue === "") {
                $endpointUrlError.text("Endpoint URL is required").show();
                $endpointUrl.addClass("is-invalid");
                isValid = false;
            } else if (!apiConfigControllerInstance.isValidUrl(urlValue)) {
                $endpointUrlError.text("Please enter a valid URL (e.g. https://api.example.com)").show();
                $endpointUrl.addClass("is-invalid");
                isValid = false;
            } else {
                $endpointUrlError.hide();
                $endpointUrl.removeClass("is-invalid");
            }

            const $endpointType = $("#edit_type");
            const $endpointTypeError = $endpointType.next(".invalid-feedback");
            if ($endpointType.val().trim() === "") {
                $endpointTypeError.show();
                $endpointType.addClass("is-invalid");
                isValid = false;
            } else {
                $endpointTypeError.hide();
                $endpointType.removeClass("is-invalid");
            }

            const $endpointCounty = $("#edit_county");
            const $endpointCountyError = $endpointCounty.next(".invalid-feedback");
            if ($endpointCounty.val().trim() === "") {
                $endpointCountyError.show();
                $endpointCounty.addClass("is-invalid");
                isValid = false;
            } else {
                $endpointCountyError.hide();
                $endpointCounty.removeClass("is-invalid");
            }

            if (isValid) {
                apiConfigControllerInstance.saveApiEndpoint();
            }
        });
    }

    clearEditForm() {
        $("#edit_hdApiId").val("");
        $("#edit_county").val("");
        $("#edit_end_point").val("");
        $("#edit_type").val("");
        $('#ApiEditModalLabel').text('Add API Endpoint');
        $('#edit_cmdDelete').hide();
        $('.is-invalid').removeClass('is-invalid');
    }

    clearEditValidations() {
        $("#edit_county").removeClass("is-invalid");
        $("#edit_county").next(".invalid-feedback").hide();
        $("#edit_end_point").removeClass("is-invalid");
        $("#edit_end_point").next(".invalid-feedback").hide();
        $("#edit_type").removeClass("is-invalid");
        $("#edit_type").next(".invalid-feedback").hide();
    }

    viewApiEndpoint(apiId) {
        const getUrl = this.viewUrl + apiId;
        const progressId = "#edit_progress_api";
        $(progressId).show();
        if (apiId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        $("#edit_end_point").val(response.data.end_point_url);
                        $("#edit_county").val(response.data.county_id);
                        $("#edit_type").val(response.data.type);
                        $("#edit_hdApiId").val(response.data.id);
                        $('#ApiEditModalLabel').text('Edit API Endpoint');
                        $('#edit_cmdDelete').show();
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve API Endpoint Record. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving API Endpoint Record", error.statusText || "Failed to retrieve API Endpoint Record. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    saveApiEndpoint() {
        if ($("#edit_hdApiId").val() === "") {
            this.createApiEndpoint();
        } else {
            this.updateApiEndpoint();
        }
    }

    createApiEndpoint() {
        try {
            $("#edit_progress_api").show();
            const apiData = {
                county_id: parseInt($("#edit_county").val()),
                end_point_url: $("#edit_end_point").val().trim(),
                type: parseInt($("#edit_type").val()),
            };
            $.ajax({
                url: this.createUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(apiData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_api").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'API Endpoint created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('ApiEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (apiConfigControllerInstance.apiTable) {
                            apiConfigControllerInstance.apiTable.ajax.reload();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating API Endpoint.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_api").hide();
                    ShowNotification("Error Creating API Endpoint", error.statusText || "Failed to create API Endpoint.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_api").hide();
            ShowNotification("Error Creating API Endpoint", e.message, 'error');
        }
    }

    updateApiEndpoint() {
        try {
            $("#edit_progress_api").show();
            const apiData = {
                id: parseInt($("#edit_hdApiId").val()),
                county_id: parseInt($("#edit_county").val()),
                end_point_url: $("#edit_end_point").val().trim(),
                type: parseInt($("#edit_type").val()),
            };
            $.ajax({
                url: this.updateUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(apiData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_api").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'API Endpoint updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('ApiEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (apiConfigControllerInstance.apiTable) {
                            apiConfigControllerInstance.apiTable.ajax.reload();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating API Endpoint.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_api").hide();
                    ShowNotification("Error Updating API Endpoint", error.statusText || "Failed to update API Endpoint.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_api").hide();
            ShowNotification("Error Updating API Endpoint", e.message, 'error');
        }
    }

    deleteApiEndpoint(apiId) {
        if (!apiId) return;
        $.ajax({
            url: this.deleteUrl + apiId,
            type: 'DELETE',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (apiConfigControllerInstance.apiTable) {
                        apiConfigControllerInstance.apiTable.ajax.reload();   // ← guaranteed redraw
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('ApiEditModal'));
                    if (editModal) editModal.hide();

                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'API Endpoint deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting API Endpoint", error.statusText || "Failed to delete API Endpoint.", 'error');
            }
        });
    }

    loadCountyDropdown() {
        const url = this.countyListUrl;
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response && response.data) {
                    const select = $('#edit_county');
                    select.empty();
                    select.append('<option value="">-- Select County --</option>');

                    response.data.forEach(item => {
                        select.append(
                            `<option value="${item.Key}">${item.Value}</option>`
                        );
                    });
                }
            },
            error: (xhr) => {
                ShowNotification("Error", "Failed to load counties.", 'error');
            }
        });
    }

    loadTypeDropdown() {
        // Hard-coded from your ApiEndpointType enum (update when enum changes)
        const types = [
            { value: 1, text: "Get Case Information" },
            { value: 2, text: "Add Hearing" },
            { value: 3, text: "Reschedule Hearing" },
            { value: 4, text: "Update Hearing" },
            { value: 5, text: "Cancel Hearing" },
            { value: 6, text: "Get Hearing Information" },
            { value: 7, text: "Get Clerk Judges" },
            { value: 8, text: "Get Clerk Courtrooms" }
            // Add more as you extend the enum
        ];

        const select = $('#edit_type');
        select.empty();
        select.append('<option value="">Select Action</option>');

        types.forEach(t => {
            select.append(
                `<option value="${t.value}">${t.text}</option>`
            );
        });
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
    onModalClose(event) {
        apiConfigControllerInstance.clearEditForm();
        apiConfigControllerInstance.clearEditValidations();
    }
    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}