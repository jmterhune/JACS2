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
        this.motionXrefTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        motionControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}MotionAPI/DeleteMotion/`;

        const listUrl = `${this.service.baseUrl}MotionAPI/GetMotions/${this.recordCount}`;
        const detailModalElement = document.getElementById('MotionDetailModal');
        if (detailModalElement) detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        const editModalElement = document.getElementById('MotionEditModal');
        if (editModalElement) editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        const xrefModalElement = document.getElementById('MotionXrefModal');
        if (xrefModalElement) xrefModalElement.addEventListener('hidden.bs.modal', this.onModalClose);

        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateXrefCounties();

        $("#xref_county").on("change", () => {
            const selectedCountyId = parseInt($("#xref_county").val()) || null;
            motionControllerInstance.populateXrefMotions(selectedCountyId);
            $("#xref_clerkMotion").val("").trigger("change");
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
                        if (isAdmin) return `<button type="button" class="motion-xref btn-command" data-id="${row.id}" title="Manage Clerk References"><i class="fas fa-exchange-alt"></i></button>`;
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },
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

        $(document).on('click', '.motion-xref', function (e) {
            e.preventDefault();
            const $row = $(this).closest('tr');
            const rowData = motionControllerInstance.motionTable.row($row).data();
            const motionId = rowData.id;
            const motionName = rowData.description?.trim() || "Unknown Motion";

            $("#hdXrefMotionId").val(motionId);
            motionControllerInstance.SetXrefMotionHeader(motionName);
            motionControllerInstance.GetMotionXrefs(motionId);
            const xrefModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('MotionXrefModal'));
            xrefModal.show();
        });

        $("#xref_cmdSaveReference").on("click", function (e) {
            e.preventDefault();
            motionControllerInstance.SaveMotionXref();
        });
    }

    populateXrefCounties() {
        const url = `${this.service.baseUrl}CountyAPI/GetCountyDropDownItems`;
        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                const $select = $("#xref_county");
                $select.empty().append('<option value="">Select County</option>');
                if (response?.data && Array.isArray(response.data)) {
                    response.data.forEach(item => $select.append(`<option value="${item.Key}">${item.Value}</option>`));
                }
            },
            error: () => ShowNotification("Error Loading Counties", "Failed to load county list.", 'error')
        });
    }

    populateXrefMotions(countyId = null) {
        const $clerkMotion = $("#xref_clerkMotion");
        const progressId = "#xref_progress_motion";

        $(progressId).show();
        $clerkMotion.empty().append('<option value="">Select Clerk Motion</option>').prop('disabled', true);

        if (!countyId || countyId <= 0) {
            $(progressId).hide();
            return;
        }

        const url = `${this.service.baseUrl}MotionAPI/GetMotionOptions/${countyId}`;
        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            timeout: 15000,
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response?.data && Array.isArray(response.data)) {
                    response.data.forEach(item => $clerkMotion.append(`<option value="${item.Key}">${item.Value}</option>`));
                    if (response.data.length > 0) $clerkMotion.prop('disabled', false);
                }
            },
            error: (error) => {
                if (error.statusText === "timeout" || error.status === 0) {
                    ShowNotification("Timeout", "The request to load Clerk Motions timed out. Please try again later.", 'error');
                } else {
                    ShowNotification("Error Loading Clerk Motions", "Failed to load list.", 'error');
                }
            },
            complete: () => $(progressId).hide()
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

    GetMotionXrefs(motionId) {
        const xrefList = `${this.service.baseUrl}MotionAPI/GetMotionXrefs/${motionId}`;
        const progressId = "#xref_progress_motion";
        $(progressId).show();

        if (this.motionXrefTable) this.motionXrefTable.destroy();

        this.motionXrefTable = $('#tblMotionXref').DataTable({
            searching: false, paging: false, info: false, lengthChange: false, ordering: true,
            autoWidth: true, stateSave: false, destroy: true,
            ajax: {
                url: xrefList,
                type: "GET",
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: () => $(progressId).hide()
            },
            columns: [
                { data: "clerk_motion_id", render: d => d || '' },
                { data: "clerk_motion_name", render: d => d || '' },
                { data: "county_name", render: d => d || '' },
                {
                    data: "motion_id",
                    render: function (data, type, row) {
                        if (motionControllerInstance.isAdmin) {
                            return `<button type="button" class="delete-xref btn-command" data-county-id="${row.county_id}" data-motion-id="${row.motion_id}"><i class="fas fa-trash"></i></button>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                }
            ],
            language: { emptyTable: "No Cross References Available." },
            initComplete: function () {
                $(progressId).hide();   // <-- fixed: hide only after table finishes loading
            }
        });

        this.motionXrefTable.on('draw', function () {
            $(".delete-xref").off("click").on("click", function (e) {
                e.preventDefault();
                const motionId = $(this).data("motion-id");
                const countyId = $(this).data("county-id");
                Swal.fire({ title: 'Delete Cross Reference?', text: 'Are you sure?', icon: 'warning', showCancelButton: true }).then((result) => {
                    if (result.isConfirmed) motionControllerInstance.DeleteMotionXref(motionId, countyId);
                });
            });
            motionControllerInstance.DisableUsedCounties();
        });
    }

    DeleteMotionXref(motionId, countyId) {
        $.ajax({
            url: `${this.service.baseUrl}MotionAPI/DeleteMotionXref/${motionId}/${countyId}`,
            type: 'DELETE',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response.status === 200) {
                    motionControllerInstance.GetMotionXrefs($("#hdXrefMotionId").val()); // FULL RELOAD
                    Swal.fire({ icon: 'success', title: 'Success', text: response.message });
                }
            },
            error: (error) => ShowNotification("Error Deleting Xref", error.statusText, 'error')
        });
    }

    SaveMotionXref() {
        let isValid = true;
        const $county = $("#xref_county");
        const $clerkMotion = $("#xref_clerkMotion");

        if (!$county.val()) { $("#xref_county_error").show(); $county.addClass("is-invalid"); isValid = false; }
        else { $("#xref_county_error").hide(); $county.removeClass("is-invalid"); }

        if (!$clerkMotion.val()) { $("#xref_clerkMotion_error").show(); $clerkMotion.addClass("is-invalid"); isValid = false; }
        else { $("#xref_clerkMotion_error").hide(); $clerkMotion.removeClass("is-invalid"); }

        if (isValid) {
            $("#xref_progress_motion").show();
            this.CreateMotionXref();
        }
    }

    CreateMotionXref() {
        const xrefData = {
            motion_id: parseInt($("#hdXrefMotionId").val()),
            county_id: parseInt($("#xref_county").val()),
            clerk_motion_id: $("#xref_clerkMotion").val() ? parseInt($("#xref_clerkMotion").val()) : 0,
            clerk_motion_name: $("#xref_clerkMotion option:selected").text().trim() || ''
        };

        $.ajax({
            url: `${this.service.baseUrl}MotionAPI/CreateMotionXref`,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(xrefData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                $("#xref_progress_motion").hide();
                if (response.status === 200) {
                    Swal.fire({ icon: 'success', title: 'Success', text: response.message });
                    motionControllerInstance.GetMotionXrefs($("#hdXrefMotionId").val()); // FULL RELOAD
                    motionControllerInstance.ClearXrefEditForm();
                }
            },
            error: (error) => {
                $("#xref_progress_motion").hide();
                ShowNotification("Error Creating Xref", error.statusText, 'error');
            }
        });
    }

    ClearXrefEditForm() {
        $("#xref_county").val("").removeClass("is-invalid").trigger("change");
        $("#xref_county_error").hide();
        $("#xref_clerkMotion").val("").removeClass("is-invalid").prop("disabled", true);
        $("#xref_clerkMotion_error").hide();
    }

    SetXrefMotionHeader(description) {
        $("#xrefSelectedMotionName").text(description.trim());
    }

    DisableUsedCounties() {
        const $countySelect = $("#xref_county");
        const usedCountyIds = new Set();
        if (this.motionXrefTable) {
            this.motionXrefTable.rows().every(function () {
                const data = this.data();
                if (data && data.county_id) usedCountyIds.add(parseInt(data.county_id));
            });
        }
        $countySelect.find("option").each(function () {
            const val = parseInt($(this).val());
            if (val && val > 0) $(this).prop("disabled", usedCountyIds.has(val));
        });
    }

    onModalClose(event) {
        if (event.target.id === 'MotionXrefModal') {
            motionControllerInstance.ClearXrefEditForm();
            if (motionControllerInstance.motionXrefTable) {
                motionControllerInstance.motionXrefTable.clear().draw();
            }
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}