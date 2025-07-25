// courtTemplate.js
let courtTemplateControllerInstance = null;

class CourtTemplateController {
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
        this.courtTemplateId = -1;
        this.searchTerm = "";
        this.courtTemplateTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtTemplateControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtTemplateAPI/DeleteCourtTemplate/`;

        const listUrl = `${this.service.baseUrl}CourtTemplateAPI/GetCourtTemplates/${this.recordCount}`;
        const detailModalElement = document.getElementById('CourtTemplateDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CourtTemplateEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateCourtDropdown();

        this.courtTemplateTable = $('#tblCourtTemplate').DataTable({
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
                    $("#tblCourtTemplate_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Court Templates", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Court Templates", "The following error occurred attempting to retrieve court template information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="ct-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Court Template" data-toggle="tooltip" data-id="${data}" class="ct-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                    data: "judge_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Court Template" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.courtTemplateTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtTemplateId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Court Template?',
                    text: 'Are you sure you wish to delete this Court Template?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtTemplateControllerInstance.DeleteCourtTemplate(courtTemplateId);
                    }
                });
            });
        });

        $(document).on('click', '.ct-detail', function (e) {
            e.preventDefault();
            var courtTemplateId = $(this).data("id");
            courtTemplateControllerInstance.ViewCourtTemplate(courtTemplateId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtTemplateEditModal'));
        $(document).on('click', '.ct-edit, #editCourtTemplateBtn', function (e) {
            e.preventDefault();
            var courtTemplateId = $(this).data("id") || $("#hdCourtTemplateId").val();
            courtTemplateControllerInstance.courtTemplateId = courtTemplateId;
            if (courtTemplateId) {
                courtTemplateControllerInstance.ViewCourtTemplate(courtTemplateId, true);
                $("#CourtTemplateEditModalLabel").html(`Edit Court Template`);
            } else {
                courtTemplateControllerInstance.ClearEditForm();
                $("#CourtTemplateEditModalLabel").html("Create New Court Template");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtTemplateControllerInstance.ClearEditForm();
            $("#CourtTemplateEditModalLabel").html("Create New Court Template");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var courtTemplateId = $("#hdCourtTemplateId").val();
            Swal.fire({
                title: 'Delete Court Template?',
                text: 'Are you sure you wish to delete this Court Template?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtTemplateControllerInstance.DeleteCourtTemplate(courtTemplateId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $ctName = $("#edit_ctName");
            const $ctNameError = $ctName.next(".invalid-feedback");
            if ($ctName.val().trim() === "") {
                $ctNameError.show();
                $ctName.addClass("is-invalid");
                isValid = false;
            } else {
                $ctNameError.hide();
                $ctName.removeClass("is-invalid");
            }

            const $ctCourt = $("#edit_ctCourt");
            const $ctCourtError = $ctCourt.next(".invalid-feedback");
            if ($ctCourt.val() === "") {
                $ctCourtError.show();
                $ctCourt.addClass("is-invalid");
                isValid = false;
            } else {
                $ctCourtError.hide();
                $ctCourt.removeClass("is-invalid");
            }

            if (isValid) {
                courtTemplateControllerInstance.SaveCourtTemplate();
            }
        });

        $("#edit_ctName").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
        $("#edit_ctCourt").on("change", function () {
            const $this = $(this);
            if ($this.val() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    populateCourtDropdown() {
        $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourtDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.data) {
                    const $courtSelect = $("#edit_ctCourt");
                    $courtSelect.empty();
                    $courtSelect.append('<option value="">Select a Court</option>');
                    response.data.forEach(court => {
                        $courtSelect.append(`<option value="${court.Key}">${court.Value}</option>`);
                    });
                } else {
                    ShowNotification("Error", response.error || "Failed to load courts.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Loading Courts", error.statusText || "Failed to load courts. Please try again later.", 'error');
            }
        });
    }

    ClearState() {
        if (this.courtTemplateTable) {
            this.courtTemplateTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtTemplate(courtTemplateId) {
        $.ajax({
            url: this.deleteUrl + courtTemplateId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (courtTemplateControllerInstance.courtTemplateTable) {
                        courtTemplateControllerInstance.courtTemplateTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Court Template deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Court Template", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CourtTemplateDetailModal') {
            courtTemplateControllerInstance.ClearDetailForm();
        } else if (modalId === 'CourtTemplateEditModal') {
            courtTemplateControllerInstance.ClearEditForm();
            courtTemplateControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#ctName").html("");
        $("#ctCourt").html("");
        $("#hdCourtTemplateId").val("");
    }

    ClearEditForm() {
        $("#edit_ctName").val("");
        $("#edit_ctCourt").val("");
        $("#edit_hdCourtTemplateId").val("");
    }

    ClearEditValidations() {
        $("#edit_ctName").removeClass("is-invalid");
        $("#edit_ctName").next(".invalid-feedback").hide();
        $("#edit_ctCourt").removeClass("is-invalid");
        $("#edit_ctCourt").next(".invalid-feedback").hide();
    }

    ViewCourtTemplate(courtTemplateId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtTemplateAPI/GetCourtTemplate/${courtTemplateId}`;
        const progressId = isEditMode ? "#edit_progress-courttemplate" : "#progress-courttemplate";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtTemplateDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (courtTemplateId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCourtTemplateId").val(response.data.id);
                            $("#edit_ctName").val(response.data.name);
                            $("#edit_ctCourt").val(response.data.court_id);
                            $("#CourtTemplateEditModalLabel").html(`Edit Court Template: ${response.data.name}`);
                        } else {
                            $("#ctName").html(response.data.name);
                            $("#ctCourt").html(response.data.court_description || '');
                            $("#hdCourtTemplateId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve court template details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Court Template Details", error.statusText || "Failed to retrieve court template details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCourtTemplate() {
        if ($("#edit_hdCourtTemplateId").val() === "") {
            this.CreateCourtTemplate();
        } else {
            this.UpdateCourtTemplate();
        }
        if (courtTemplateControllerInstance.courtTemplateTable) {
            courtTemplateControllerInstance.ClearEditForm();
            courtTemplateControllerInstance.courtTemplateTable.draw();
        }
    }

    CreateCourtTemplate() {
        try {
            $("#edit_progress-courttemplate").show();
            const courtTemplateData = {
                name: $("#edit_ctName").val().trim(),
                court_id: $("#edit_ctCourt").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTemplateAPI/CreateCourtTemplate`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtTemplateData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courttemplate").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Court Template created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtTemplateControllerInstance.courtTemplateTable) {
                            courtTemplateControllerInstance.courtTemplateTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating court template.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courttemplate").hide();
                    ShowNotification("Error Creating Court Template", error.statusText || "Failed to create court template.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courttemplate").hide();
            ShowNotification("Error Creating Court Template", e.message, 'error');
        }
    }

    UpdateCourtTemplate() {
        try {
            $("#edit_progress-courttemplate").show();
            const courtTemplateData = {
                id: parseInt($("#edit_hdCourtTemplateId").val()),
                name: $("#edit_ctName").val().trim(),
                court_id: $("#edit_ctCourt").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTemplateAPI/UpdateCourtTemplate`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtTemplateData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courttemplate").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Court Template updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtTemplateControllerInstance.courtTemplateTable) {
                            courtTemplateControllerInstance.courtTemplateTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating court template.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courttemplate").hide();
                    ShowNotification("Error Updating Court Template", error.statusText || "Failed to update court template.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courttemplate").hide();
            ShowNotification("Error Updating Court Template", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}