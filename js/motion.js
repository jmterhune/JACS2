let motionControllerInstance = null;

class MotionController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.motionId = -1;
        this.searchTerm = "";
        this.motionTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        motionControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}MotionAPI/DeleteMotion/`;

        const listUrl = `${this.service.baseUrl}MotionAPI/GetMotions/${this.recordCount}`;
        const editModalElement = document.getElementById('MotionEditModal');

        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.motionTable = $('#tblMotion').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data(data) { data.searchText = data.search?.value || ''; delete data.columns; },
                error: function (error) {
                    $("#tblMotion_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Motions', 'Please make sure you are logged in and try again.', 'error');
                    } else {
                        ShowNotification('Error Retrieving Motions', 'The following error occurred: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                { data: "id", render: data => `<button type="button" title="View Details" data-id="${data}" class="motion-detail btn-command"><i class="fas fa-eye"></i></button>`, className: "command-item", orderable: false },
                { data: "id", render: data => `<button type="button" title="Edit Motion" data-id="${data}" class="motion-edit btn-command"><i class="fas fa-pencil"></i></button>`, className: "command-item", orderable: false },
                { data: "description", render: data => data || '' },
                { data: "lag", render: data => data || '' },
                { data: "lead", render: data => data || '' },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin) return `<button type="button" class="delete btn-command" data-id="${row.id}" title="Delete Motion"><i class="fas fa-trash"></i></button>`;
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                }
            ],
            language: { emptyTable: "No Records Available.", zeroRecords: "No records match the search criteria." },
            order: [[this.sortColumnIndex, this.sortDirection]],
            serverSide: true,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: this.pageSize,
            displayStart: this.currentPage * this.pageSize,
        });

        $(".dt-length").prepend($("#lnkAdd"));

        this.motionTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const motionId = $(this).data("id");
                Swal.fire({ title: 'Delete Motion?', text: 'Are you sure?', icon: 'warning', showCancelButton: true }).then((result) => {
                    if (result.isConfirmed) motionControllerInstance.DeleteMotion(motionId);
                });
            });
        });

        $(document).on('click', '.motion-detail', function (e) {
            e.preventDefault();
            motionControllerInstance.ViewMotion($(this).data("id"), false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('MotionEditModal'));
        $(document).on('click', '.motion-edit, #editMotionBtn', function (e) {
            e.preventDefault();
            const motionId = $(this).data("id") || $("#hdMotionId").val();
            if (motionId) {
                motionControllerInstance.ViewMotion(motionId, true);
                $("#MotionEditModalLabel").html(`Edit Motion`);
            } else {
                motionControllerInstance.ClearEditForm();
                $("#MotionEditModalLabel").html("Create New Motion");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            motionControllerInstance.ClearEditForm();
            $("#MotionEditModalLabel").html("Create New Motion");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            motionControllerInstance.DeleteMotion($("#hdMotionId").val());
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;
            const $desc = $("#edit_motionDescription");
            if ($desc.val().trim() === "") {
                $desc.addClass("is-invalid");
                isValid = false;
            } else $desc.removeClass("is-invalid");
            if (isValid) motionControllerInstance.SaveMotion();
        });
    }

    DeleteMotion(motionId) {
        $.ajax({
            url: this.deleteUrl + motionId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    motionControllerInstance.motionTable.draw();
                    Swal.fire({ icon: 'success', title: 'Success', text: response.message || 'Motion deleted successfully.' });
                }
            },
            error: (error) => ShowNotification("Error Deleting Motion", error.statusText, 'error')
        });
    }

    ClearEditForm() {
        $("#edit_motionDescription").val("");
        $("#edit_motionLag").val("");
        $("#edit_motionLead").val("");
        $("#edit_hdMotionId").val("");
    }

    ViewMotion(motionId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}MotionAPI/GetMotion/${motionId}`;
        const progressId = isEditMode ? "#edit_progress-motion" : "#progress-motion";
        $(progressId).show();
        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('MotionDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }
        $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.data) {
                    if (isEditMode) {
                        $("#edit_hdMotionId").val(response.data.id);
                        $("#edit_motionDescription").val(response.data.description);
                        $("#edit_motionLag").val(response.data.lag);
                        $("#edit_motionLead").val(response.data.lead);
                    } else {
                        $("#motionDescription").html(response.data.description);
                        $("#motionLag").html(response.data.lag);
                        $("#motionLead").html(response.data.lead);
                        $("#hdMotionId").val(response.data.id);
                    }
                    $(progressId).hide();
                }
            },
            error: (error) => {
                ShowNotification("Error Retrieving Motion", error.statusText, 'error');
                $(progressId).hide();
            }
        });
    }

    SaveMotion() {
        if ($("#edit_hdMotionId").val() === "") this.CreateMotion(); else this.UpdateMotion();
        motionControllerInstance.motionTable.draw();
    }

    CreateMotion() {
        $("#edit_progress-motion").show();
        const motionData = {
            description: $("#edit_motionDescription").val().trim(),
            lag: parseInt($("#edit_motionLag").val()) || 0,
            lead: parseInt($("#edit_motionLead").val()) || 0
        };
        $.ajax({
            url: `${this.service.baseUrl}MotionAPI/CreateMotion`,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(motionData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                $("#edit_progress-motion").hide();
                if (response.status === 200) {
                    Swal.fire({ icon: 'success', title: 'Success', text: response.message });
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('MotionEditModal'));
                    if (editModal) editModal.hide();
                }
            },
            error: (error) => {
                $("#edit_progress-motion").hide();
                ShowNotification("Error Creating Motion", error.statusText, 'error');
            }
        });
    }

    UpdateMotion() {
        $("#edit_progress-motion").show();
        const motionData = {
            id: parseInt($("#edit_hdMotionId").val()),
            description: $("#edit_motionDescription").val().trim(),
            lag: parseInt($("#edit_motionLag").val()) || 0,
            lead: parseInt($("#edit_motionLead").val()) || 0
        };
        $.ajax({
            url: `${this.service.baseUrl}MotionAPI/UpdateMotion`,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(motionData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                $("#edit_progress-motion").hide();
                if (response.status === 200) {
                    Swal.fire({ icon: 'success', title: 'Success', text: response.message });
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('MotionEditModal'));
                    if (editModal) editModal.hide();
                }
            },
            error: (error) => {
                $("#edit_progress-motion").hide();
                ShowNotification("Error Updating Motion", error.statusText, 'error');
            }
        });
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}
