// county.js
let countyControllerInstance = null;

class CountyController {
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
        this.countyId = -1;
        this.searchTerm = "";
        this.countyTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        countyControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CountyAPI/DeleteCounty/`;

        const listUrl = `${this.service.baseUrl}CountyAPI/GetCounties/${this.recordCount}`;
        const detailModalElement = document.getElementById('CountyDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CountyEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.countyTable = $('#tblCounty').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data(data) {
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblCounty_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Counties", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Counties", "The following error occurred attempting to retrieve county information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="county-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit County" data-toggle="tooltip" data-id="${data}" class="county-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                    data: "code",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete County" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.countyTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const countyId = $(this).data("id");
                Swal.fire({
                    title: 'Delete County?',
                    text: 'Are you sure you wish to delete this County?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        countyControllerInstance.DeleteCounty(countyId);
                    }
                });
            });
        });
        $(document).on('click', '.county-detail', function (e) {
            e.preventDefault();
            var countyId = $(this).data("id");
            countyControllerInstance.ViewCounty(countyId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CountyEditModal'));
        $(document).on('click', '.county-edit, #editCountyBtn', function (e) {
            e.preventDefault();
            var countyId = $(this).data("id") || $("#hdCountyId").val();
            countyControllerInstance.countyId = countyId;
            if (countyId) {
                countyControllerInstance.ViewCounty(countyId, true);
                $("#CountyEditModalLabel").html(`Edit County`);
            } else {
                countyControllerInstance.ClearEditForm();
                $("#CountyEditModalLabel").html("Create New County");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            countyControllerInstance.ClearEditForm();
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
                    countyControllerInstance.DeleteCounty(countyId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

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

            if (isValid) {
                countyControllerInstance.SaveCounty();
            }
        });

        $("#edit_countyName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
        $("#edit_countyCode").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.countyTable) {
            this.countyTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCounty(countyId) {
        $.ajax({
            url: this.deleteUrl + countyId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (countyControllerInstance.countyTable) {
                        countyControllerInstance.countyTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('CountyDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'County deleted successfully.'
                    });
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
            countyControllerInstance.ClearDetailForm();
        } else if (modalId === 'CountyEditModal') {
            countyControllerInstance.ClearEditForm();
            countyControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#countyName").html("");
        $("#countyCode").html("");
        $("#hdCountyId").val("");
    }

    ClearEditForm() {
        $("#edit_countyName").val("");
        $("#edit_countyCode").val("");
        $("#edit_hdCountyId").val("");
    }

    ClearEditValidations() {
        $("#edit_countyName").removeClass("is-invalid");
        $("#edit_countyName").next(".invalid-feedback").hide();
        $("#edit_countyCode").removeClass("is-invalid");
        $("#edit_countyCode").next(".invalid-feedback").hide();
    }

    ViewCounty(countyId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CountyAPI/GetCounty/${countyId}`;
        const progressId = isEditMode ? "#edit_progress-county" : "#progress-county";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CountyDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (countyId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCountyId").val(response.data.id);
                            $("#edit_countyName").val(response.data.name);
                            $("#edit_countyCode").val(response.data.code);
                            $("#CountyEditModalLabel").html(`Edit County: ${response.data.name}`);
                        } else {
                            $("#countyName").html(response.data.name);
                            $("#countyCode").html(response.data.code);
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

    SaveCounty() {
        if ($("#edit_hdCountyId").val() === "") {
            this.CreateCounty();
        } else {
            this.UpdateCounty();
        }
        if (countyControllerInstance.countyTable) {
            countyControllerInstance.ClearEditForm();
            countyControllerInstance.countyTable.draw();
        }
    }

    CreateCounty() {
        try {
            $("#edit_progress-county").show();
            const countyData = {
                name: $("#edit_countyName").val().trim(),
                code: $("#edit_countyCode").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CountyAPI/CreateCounty`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(countyData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-county").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'County created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (countyControllerInstance.countyTable) {
                            countyControllerInstance.countyTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating county.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-county").hide();
                    ShowNotification("Error Creating County", error.statusText || "Failed to create county.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-county").hide();
            ShowNotification("Error Creating County", e.message, 'error');
        }
    }

    UpdateCounty() {
        try {
            $("#edit_progress-county").show();
            const countyData = {
                id: parseInt($("#edit_hdCountyId").val()),
                name: $("#edit_countyName").val().trim(),
                code: $("#edit_countyCode").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CountyAPI/UpdateCounty`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(countyData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-county").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'County updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CountyEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (countyControllerInstance.countyTable) {
                            countyControllerInstance.countyTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating county.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-county").hide();
                    ShowNotification("Error Updating County", error.statusText || "Failed to update county.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-county").hide();
            ShowNotification("Error Updating County", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}