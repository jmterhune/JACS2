// Updated courtTemplate.js (added ct-copy handler and updated CopyCourtTemplate to GET)
let courtTemplateControllerInstance = null;

class CourtTemplateController {
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
        this.templateId = params.templateId || -1;
        this.courtId = params.courtId || -1;
        this.templateTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        this.templateConfigUrl = params.templateConfigUrl || '/';
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
            $(editModalElement).on('keydown', (e) => {
                if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                    e.preventDefault();
                    $("#edit_cmdSave").trigger('click');
                }
            });
        }

        this.populateCourtDropdown();

        this.templateTable = $('#tblCourtTemplate').DataTable({
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
                        ShowNotification("Error Retrieving Templates", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Templates", "The following error occurred attempting to retrieve template information. Error: " + error.statusText, 'error');
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
                        return `<button type="button" title="Edit Template" data-toggle="tooltip" data-id="${data}" class="ct-edit btn-command"><i class="fas fa-pencil"></i></button>`;
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
                        return `<button type="button" title="Configure Template" data-toggle="tooltip" data-id="${data}" data-court-id="${row.court_id}" class="ct-config btn-command">Configure <i class="fas fa-cog"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        return `<button type="button" title="Copy Template" data-toggle="tooltip" data-id="${data}" data-court-id="${row.court_id}" class="ct-copy btn-command">Clone <i class="fas fa-copy"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Template" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.templateTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const templateId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Template?',
                    text: 'Are you sure you wish to delete this Template?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtTemplateControllerInstance.DeleteCourtTemplate(templateId);
                    }
                });
            });
            $(".ct-config").on("click", function (e) {
                e.preventDefault();
                const templateId = $(this).data("id");
                const courtId = $(this).data("court-id");
                courtTemplateControllerInstance.NavigateToConfig(templateId, courtId);
            });
            $(".ct-copy").on("click", function (e) {
                e.preventDefault();
                const templateId = $(this).data("id");
                Swal.fire({
                    title: 'Copy Template?',
                    text: 'Are you sure you wish to copy this Template?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtTemplateControllerInstance.CopyCourtTemplate(templateId);
                    }
                });
            });
        });

        $(document).on('click', '.ct-detail', function (e) {
            e.preventDefault();
            var templateId = $(this).data("id");
            courtTemplateControllerInstance.ViewCourtTemplate(templateId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtTemplateEditModal'));
        $(document).on('click', '.ct-edit, #editCourtTemplateBtn', function (e) {
            e.preventDefault();
            var templateId = $(this).data("id") || $("#hdCourtTemplateId").val();
            courtTemplateControllerInstance.templateId = templateId;
            if (templateId) {
                courtTemplateControllerInstance.ViewCourtTemplate(templateId, true);
                $("#CourtTemplateEditModalLabel").html(`Edit Template`);
            } else {
                courtTemplateControllerInstance.ClearEditForm();
                $("#CourtTemplateEditModalLabel").html("Create New Template");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtTemplateControllerInstance.ClearEditForm();
            $("#CourtTemplateEditModalLabel").html("Create New Template");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var templateId = $("#hdCourtTemplateId").val();
            Swal.fire({
                title: 'Delete Template?',
                text: 'Are you sure you wish to delete this Template?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtTemplateControllerInstance.DeleteCourtTemplate(templateId);
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

    NavigateToConfig(templateId, courtId) {
        window.location.href = `${this.templateConfigUrl}/tid/${templateId}/cid/${courtId}`;
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
        if (this.templateTable) {
            this.templateTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtTemplate(templateId) {
        $.ajax({
            url: this.deleteUrl + templateId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (courtTemplateControllerInstance.templateTable) {
                        courtTemplateControllerInstance.templateTable.draw();
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
                        text: response.message || 'Template deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Template", error.statusText, 'error');
            }
        });
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

    ViewCourtTemplate(templateId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtTemplateAPI/GetCourtTemplate/${templateId}`;
        const progressId = isEditMode ? "#edit_progress-courttemplate" : "#progress-courttemplate";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtTemplateDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (templateId) {
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
                            $("#CourtTemplateEditModalLabel").html(`Edit Template: ${response.data.name} `);
                        } else {
                            $("#ctName").html(response.data.name);
                            $("#ctCourt").html(response.data.court_description || '');
                            $("#hdCourtTemplateId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve template details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Template Details", error.statusText || "Failed to retrieve template details. Please try again later.", 'error');
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
        if (courtTemplateControllerInstance.templateTable) {
            courtTemplateControllerInstance.ClearEditForm();
            courtTemplateControllerInstance.templateTable.draw();
        }
    }

    CreateCourtTemplate() {
        try {
            $("#edit_progress-courttemplate").show();
            const templateData = {
                name: $("#edit_ctName").val().trim(),
                court_id: $("#edit_ctCourt").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTemplateAPI/CreateCourtTemplate`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(templateData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courttemplate").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Template created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtTemplateControllerInstance.templateTable) {
                            courtTemplateControllerInstance.templateTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating template.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courttemplate").hide();
                    ShowNotification("Error Creating Template", error.statusText || "Failed to create template.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courttemplate").hide();
            ShowNotification("Error Creating Template", e.message, 'error');
        }
    }

    UpdateCourtTemplate() {
        try {
            $("#edit_progress-courttemplate").show();
            const templateData = {
                id: parseInt($("#edit_hdCourtTemplateId").val()),
                name: $("#edit_ctName").val().trim(),
                court_id: $("#edit_ctCourt").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtTemplateAPI/UpdateCourtTemplate`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(templateData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courttemplate").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Template updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtTemplateEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtTemplateControllerInstance.templateTable) {
                            courtTemplateControllerInstance.templateTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating template.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courttemplate").hide();
                    ShowNotification("Error Updating Template", error.statusText || "Failed to update template.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courttemplate").hide();
            ShowNotification("Error Updating Template", e.message, 'error');
        }
    }
    CopyCourtTemplate(templateId) {
        try {
            $.ajax({
                url: `${this.service.baseUrl}TemplateAPI/CloneTemplate/${templateId}`,
                type: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Template Copied successfully.'
                        });
                        if (courtTemplateControllerInstance.templateTable) {
                            courtTemplateControllerInstance.templateTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while copying template.", 'error');
                    }
                },
                error: function (error) {
                    ShowNotification("Error Copying Template", error.statusText || "Failed to copy template.", 'error');
                }
            });
        } catch (e) {
            ShowNotification("Error Copying Template", e.message, 'error');
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}