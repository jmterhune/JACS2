let courtroomControllerInstance = null;

class CourtroomController {
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
        this.courtroomId = -1;
        this.searchTerm = "";
        this.courtroomTable = null;
        this.courtroomXrefTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtroomControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtroomAPI/DeleteCourtroom/`;

        const listUrl = `${this.service.baseUrl}CourtroomAPI/GetCourtrooms/${this.recordCount}`;
        const detailModalElement = document.getElementById('CourtroomDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CourtroomEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateXrefCounties();

        $("#xref_county").on("change", () => {
            const selectedCountyId = parseInt($("#xref_county").val()) || null;
            if (selectedCountyId) localStorage.setItem('jacs_lastXrefCountyId', selectedCountyId);
            courtroomControllerInstance.populateXrefCourtrooms(selectedCountyId);
            $("#xref_clerkCourtroom").val("").trigger("change");
        });

        this.courtroomTable = $('#tblCourtroom').DataTable({
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
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblCourtroom_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Courtrooms', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Courtrooms', 'The following error occurred attempting to retrieve courtroom information. Error: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="courtroom-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Courtroom" data-toggle="tooltip" data-id="${data}" class="courtroom-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin) {
                            return `<button type="button" class="courtroom-xref btn-command" data-toggle="tooltip" aria-role="button" title="Manage Clerk References" data-id="${row.id}"><i class="fas fa-exchange-alt"></i></button>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "description",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin) {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Courtroom" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
            createdRow: function (row, data) {
                if (data && data.xref_count > 0) $(row).addClass('has-xref');
            },
        });

        $(".dt-length").prepend($("#lnkAdd"));
        this.courtroomTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtroomId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Courtroom?',
                    text: 'Are you sure you wish to delete this Courtroom?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtroomControllerInstance.DeleteCourtroom(courtroomId);
                    }
                });
            });
        });

        $(document).on('click', '.courtroom-detail', function (e) {
            e.preventDefault();
            var courtroomId = $(this).data("id");
            courtroomControllerInstance.ViewCourtroom(courtroomId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CourtroomEditModal'));
        $(document).on('click', '.courtroom-edit, #editCourtroomBtn', function (e) {
            e.preventDefault();
            var courtroomId = $(this).data("id") || $("#hdCourtroomId").val();
            courtroomControllerInstance.courtroomId = courtroomId;
            if (courtroomId) {
                courtroomControllerInstance.ViewCourtroom(courtroomId, true);
                $("#CourtroomEditModalLabel").html(`Edit Courtroom`);
            } else {
                courtroomControllerInstance.ClearEditForm();
                $("#CourtroomEditModalLabel").html("Create New Courtroom");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            courtroomControllerInstance.ClearEditForm();
            $("#CourtroomEditModalLabel").html("Create New Courtroom");
            editModal.show();
        });

        $("#edit_courtroomDescription").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var courtroomId = $("#hdCourtroomId").val();
            Swal.fire({
                title: 'Delete Courtroom?',
                text: 'Are you sure you wish to delete this Courtroom?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    courtroomControllerInstance.DeleteCourtroom(courtroomId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $courtroomDescription = $("#edit_courtroomDescription");
            const $courtroomDescriptionError = $courtroomDescription.next(".invalid-feedback");
            if ($courtroomDescription.val().trim() === "") {
                $courtroomDescriptionError.show();
                $courtroomDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $courtroomDescriptionError.hide();
                $courtroomDescription.removeClass("is-invalid");
            }

            if (isValid) {
                courtroomControllerInstance.SaveCourtroom();
            }
        });

        $(document).on('click', '.courtroom-xref', function (e) {
            e.preventDefault();
            const $row = $(this).closest('tr');
            const rowData = courtroomControllerInstance.courtroomTable.row($row).data();

            if (!rowData || !rowData.id) {
                Swal.fire({
                    title: 'Retrieve Courtroom Failed?',
                    text: 'The requested courtroom record could not be found',
                    icon: 'warning',
                    showCancelButton: true,
                });
                return;
            }

            const courtroomId = rowData.id;
            const courtroomDescription = rowData.description?.trim() || "Unknown Courtroom";

            $("#hdXrefCourtroomId").val(courtroomId);
            courtroomControllerInstance.SetXrefCourtroomHeader(courtroomDescription);
            courtroomControllerInstance.GetCourtroomXrefs(courtroomId);
            const xrefModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('CourtroomXrefModal'));
            if (xrefModal) {
                xrefModal.show();
            } else {
                $('#CourtroomXrefModal').modal('show');
            }
        });

        $("#xref_cmdSaveReference").on("click", function (e) {
            e.preventDefault();
            courtroomControllerInstance.SaveCourtroomXref();
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
                    response.data.forEach(item => {
                        $select.append(`<option value="${item.Key}">${item.Value}</option>`);
                    });
                    const lastCountyId = localStorage.getItem('jacs_lastXrefCountyId');
                    if (lastCountyId && $select.find(`option[value="${lastCountyId}"]`).length) {
                        // Fire change so the dependent clerk-courtroom dropdown loads;
                        // setting .val() alone does not trigger it.
                        $select.val(lastCountyId).trigger('change');
                    }
                } else {
                    ShowNotification("Warning", "No counties available.", 'warning');
                }
            },
            error: (error) => {
                ShowNotification("Error Loading Counties", error.statusText || "Failed to load county list.", 'error');
            }
        });
    }

    populateXrefCourtrooms(countyId = null) {
        const $clerkCourtroom = $("#xref_clerkCourtroom");
        const progressId = "#xref_progress_courtroom";

        $(progressId).show();
        $clerkCourtroom.empty().append('<option value="">Select Clerk Courtroom</option>').prop('disabled', true);

        if (!countyId || countyId <= 0) {
            $(progressId).hide();
            return;
        }

        const url = `${this.service.baseUrl}CourtroomAPI/GetCourtroomOptions/${countyId}`;
        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            timeout: 15000,
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response?.data && Array.isArray(response.data)) {
                    response.data.forEach(item => $clerkCourtroom.append(`<option value="${item.Key}">${item.Value}</option>`));
                    if (response.data.length > 0) $clerkCourtroom.prop('disabled', false);
                }
            },
            error: (error) => {
                if (error.statusText === "timeout" || error.status === 0) {
                    ShowNotification("Timeout", "The request to load Clerk Courtrooms timed out. Please try again later.", 'error');
                } else {
                    ShowNotification("Error Loading Clerk Courtrooms", error.statusText || "Failed to load list.", 'error');
                }
            },
            complete: () => $(progressId).hide()
        });
    }

    ClearState() {
        if (this.courtroomTable) {
            this.courtroomTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCourtroom(courtroomId) {
        $.ajax({
            url: this.deleteUrl + courtroomId,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (courtroomControllerInstance.courtroomTable) {
                        courtroomControllerInstance.courtroomTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Courtroom deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Courtroom", error.statusText || "Failed to delete courtroom.", 'error');
            }
        });
    }

    ClearDetailForm() {
        $("#courtroomDescription").html("");
        $("#hdCourtroomId").val("");
    }

    ClearEditForm() {
        $("#edit_courtroomDescription").val("");
        $("#edit_hdCourtroomId").val("");
    }

    ClearEditValidations() {
        $("#edit_courtroomDescription").removeClass("is-invalid");
        $("#edit_courtroomDescription").next(".invalid-feedback").hide();
    }

    ViewCourtroom(courtroomId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtroomAPI/GetCourtroom/${courtroomId}`;
        const progressId = isEditMode ? "#edit_progress-courtroom" : "#progress-courtroom";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CourtroomDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (courtroomId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCourtroomId").val(response.data.id);
                            $("#edit_courtroomDescription").val(response.data.description);
                            $("#CourtroomEditModalLabel").html(`Edit Courtroom: ${response.data.description}`);
                        } else {
                            $("#courtroomDescription").html(response.data.description);
                            $("#hdCourtroomId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", response.error || "Failed to retrieve courtroom details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification("Error Retrieving Courtroom Details", error.statusText || "Failed to retrieve courtroom details.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCourtroom() {
        if ($("#edit_hdCourtroomId").val() === "") {
            this.CreateCourtroom();
        } else {
            this.UpdateCourtroom();
        }
        if (courtroomControllerInstance.courtroomTable) {
            courtroomControllerInstance.ClearEditForm();
            courtroomControllerInstance.courtroomTable.draw();
        }
    }

    CreateCourtroom() {
        try {
            $("#edit_progress-courtroom").show();
            const courtroomData = {
                description: $("#edit_courtroomDescription").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtroomAPI/CreateCourtroom`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtroomData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courtroom").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Courtroom created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtroomControllerInstance.courtroomTable) {
                            courtroomControllerInstance.courtroomTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating courtroom.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtroom").hide();
                    ShowNotification("Error Creating Courtroom", error.statusText || "Failed to create courtroom.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtroom").hide();
            ShowNotification("Error Creating Courtroom", e.message, 'error');
        }
    }

    UpdateCourtroom() {
        try {
            $("#edit_progress-courtroom").show();
            const courtroomData = {
                id: parseInt($("#edit_hdCourtroomId").val()),
                description: $("#edit_courtroomDescription").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtroomAPI/UpdateCourtroom`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(courtroomData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress-courtroom").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Courtroom updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CourtroomEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (courtroomControllerInstance.courtroomTable) {
                            courtroomControllerInstance.courtroomTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating courtroom.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-courtroom").hide();
                    ShowNotification("Error Updating Courtroom", error.statusText || "Failed to update courtroom.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-courtroom").hide();
            ShowNotification("Error Updating Courtroom", e.message, 'error');
        }
    }

    GetCourtroomXrefs(courtroomId) {
        const xrefList = `${this.service.baseUrl}CourtroomAPI/GetCourtroomXrefs/${courtroomId}`;
        const progressId = "#xref_progress_courtroom";
        $(progressId).show();
        $("#hdXrefCourtroomId").val(courtroomId);

        if (courtroomId) {
            if (this.courtroomXrefTable) {
                this.courtroomXrefTable.destroy();
            }

            this.courtroomXrefTable = $('#tblCourtroomXref').DataTable({
                searching: false,
                paging: false,
                info: false,
                lengthChange: false,
                ordering: true,
                autoWidth: true,
                stateSave: false,
                destroy: true,
                ajax: {
                    url: xrefList,
                    type: "GET",
                    dataType: 'json',
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    error: function (error) {
                        $(progressId).hide();
                        ShowNotification('Error Retrieving Courtroom Cross-References', error.statusText || 'Failed to retrieve xrefs.', 'error');
                    }
                },
                columns: [
                    { data: "clerk_courtroom_id", render: d => d || '' },
                    { data: "clerk_courtroom_name", render: d => d || '' },
                    { data: "county_name", render: d => d || '' },
                    {
                        data: "courtroom_id",
                        render: function (data, type, row) {
                            if (courtroomControllerInstance.isAdmin) {
                                return `<button type="button" class="delete-xref btn-command" data-toggle="tooltip" data-county-id="${row.county_id}" data-courtroom-id="${row.courtroom_id}"><i class="fas fa-trash"></i></button>`;
                            }
                            return '';
                        },
                        className: "command-item",
                        orderable: false
                    }
                ],
                language: {
                    emptyTable: "No Cross References Available.",
                    zeroRecords: "No records match the search criteria you entered."
                },
                serverSide: true,
                processing: true,
                paging: false,
                initComplete: function () {
                    $(progressId).hide();
                }
            });

            this.courtroomXrefTable.on('draw', function () {
                $(".delete-xref").off("click").on("click", function (e) {
                    e.preventDefault();
                    const courtroomId = $(this).data("courtroom-id");
                    const countyId = $(this).data("county-id");
                    Swal.fire({
                        title: 'Delete Courtroom Cross Reference?',
                        text: 'Are you sure you wish to delete this cross-reference?',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'Yes',
                        cancelButtonText: 'No'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            courtroomControllerInstance.DeleteCourtroomXref(courtroomId, countyId);
                        }
                    });
                });

                courtroomControllerInstance.DisableUsedCounties();
            });
        }
        $(progressId).hide();
    }

    DeleteCourtroomXref(courtroomId, countyId) {
        $.ajax({
            url: `${this.service.baseUrl}CourtroomAPI/DeleteCourtroomXref/${courtroomId}/${countyId}`,
            type: 'DELETE',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response.status === 200) {
                    if (courtroomControllerInstance.courtroomXrefTable) {
                        courtroomControllerInstance.courtroomXrefTable.draw();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Courtroom cross-reference deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: (error) => {
                ShowNotification("Error Deleting Courtroom Xref", error.statusText, 'error');
            }
        });
    }

    SaveCourtroomXref() {
        let isValid = true;
        const $county = $("#xref_county");
        const $clerkCourtroom = $("#xref_clerkCourtroom");

        if (!$county.val()) {
            $("#xref_county_error").show();
            $county.addClass("is-invalid");
            isValid = false;
        } else {
            $("#xref_county_error").hide();
            $county.removeClass("is-invalid");
        }

        if (!$clerkCourtroom.val()) {
            $("#xref_clerkCourtroom_error").show();
            $clerkCourtroom.addClass("is-invalid");
            isValid = false;
        } else {
            $("#xref_clerkCourtroom_error").hide();
            $clerkCourtroom.removeClass("is-invalid");
        }

        if (isValid) {
            $("#xref_progress_courtroom").show();
            this.CreateCourtroomXref();
        }
    }

    CreateCourtroomXref() {
        try {
            const xrefData = {
                courtroom_id: parseInt($("#hdXrefCourtroomId").val()),
                county_id: parseInt($("#xref_county").val()),
                clerk_courtroom_id: $("#xref_clerkCourtroom").val() ? parseInt($("#xref_clerkCourtroom").val()) : 0,
                clerk_courtroom_name: $("#xref_clerkCourtroom option:selected").text().trim() || ''
            };

            $.ajax({
                url: `${this.service.baseUrl}CourtroomAPI/CreateCourtroomXref`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(xrefData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: (response) => {
                    $("#xref_progress_courtroom").hide();
                    if (response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Courtroom Xref created successfully.'
                        });
                        courtroomControllerInstance.courtroomXrefTable.draw();
                        courtroomControllerInstance.ClearXrefEditForm();
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                    }
                },
                error: (error) => {
                    $("#xref_progress_courtroom").hide();
                    ShowNotification("Error Creating Courtroom Xref", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#xref_progress_courtroom").hide();
            ShowNotification("Error Creating Courtroom Xref", e.message, 'error');
        }
    }

    ClearXrefEditForm() {
        const lastCountyId = localStorage.getItem('jacs_lastXrefCountyId') || "";
        $("#xref_county").val(lastCountyId).removeClass("is-invalid");
        $("#xref_county_error").hide();
        $("#xref_clerkCourtroom").val("").removeClass("is-invalid").prop("disabled", true);
        $("#xref_clerkCourtroom_error").hide();
        $("#hdXrefCourtroomId").val("");
        if (lastCountyId) $("#xref_county").trigger("change");
    }

    ClearXrefCourtroomHeader() {
        $("#xrefSelectedCourtroomName").text("");
        $("#xrefCourtroomHeader").hide();
    }

    SetXrefCourtroomHeader(description) {
        const $nameSpan = $("#xrefSelectedCourtroomName");
        if (description && description.trim()) {
            $nameSpan.text(description.trim());
            $("#xrefCourtroomHeader").show();
        } else {
            this.ClearXrefCourtroomHeader();
        }
    }

    DisableUsedCounties() {
        const $countySelect = $("#xref_county");
        const usedCountyIds = new Set();

        if (this.courtroomXrefTable) {
            this.courtroomXrefTable.rows().every(function () {
                const data = this.data();
                if (data && data.county_id) {
                    usedCountyIds.add(parseInt(data.county_id));
                }
            });
        }

        $countySelect.find("option").each(function () {
            const val = parseInt($(this).val());
            if (val && val > 0) {
                $(this).prop("disabled", usedCountyIds.has(val));
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CourtroomDetailModal') {
            courtroomControllerInstance.ClearXrefEditForm();
            courtroomControllerInstance.ClearXrefCourtroomHeader();
            $("#hdXrefCourtroomId").val("");
            $("#xref_progress_courtroom").hide();
            if (courtroomControllerInstance.courtroomXrefTable) {
                courtroomControllerInstance.courtroomXrefTable.clear().draw();
            }
        }
        // Add other modal cleanups as needed
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}