let judgeControllerInstance = null;
class JudgeController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.judgeRole = params.judgeRole || 'Judge';
        this.portalId = params.portalId || -1;
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.judgeId = -1;
        this.searchTerm = "";
        this.judgeTable = null;
        this.judgeXrefTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        judgeControllerInstance = this;
    }

    init() {
        $("#edit_progress_judge").show();
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}JudgeAPI/DeleteJudge/`;

        const listUrl = `${this.service.baseUrl}JudgeAPI/GetJudges/${this.recordCount}`;
        const detailModalElement = document.getElementById('JudgeDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        const xrefModalElement = document.getElementById('JudgeXrefModal');
        if (xrefModalElement) {
            xrefModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        const editModalElement = document.getElementById('JudgeEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateCourts();
        this.populateJudgeUsers();
        this.populateXrefCounties();

        $("#xref_county").on("change", function () {
            const selectedCountyId = parseInt($(this).val()) || null;
            if (selectedCountyId) localStorage.setItem('jacs_lastXrefCountyId', selectedCountyId);
            judgeControllerInstance.populateXrefJudges(selectedCountyId);
            $("#xref_clerkJudge").val("").trigger("change");
        });
        this.judgeTable = $('#tblJudge').DataTable({
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
                    $("#tblJudge_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Judges', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Judges', 'The following error occurred attempting to retrieve judge information. Error: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-bs-toggle="tooltip" data-id="${data}" class="judge-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Judge" data-bs-toggle="tooltip" data-id="${data}" class="judge-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin) {
                            return `<button type="button" class="judge-xref btn-command" data-bs-toggle="tooltip" aria-role="button" title="Manage Clerk References" data-id="${row.id}"><i class="fas fa-exchange-alt"></i></button>`;
                        }
                        return '';
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
                    data: "phone",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "court_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "title",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin) {
                            return `<button type="button" class="delete btn-command" data-bs-toggle="tooltip" aria-role="button" title="Delete Judge" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.judgeTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const judgeId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Judge?',
                    text: 'Are you sure you wish to delete this Judge?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        judgeControllerInstance.DeleteJudge(judgeId);
                    }
                });
            });
        });

        $(document).on('click', '.judge-detail', function (e) {
            e.preventDefault();
            var judgeId = $(this).data("id");
            judgeControllerInstance.ViewJudge(judgeId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('JudgeEditModal'));
        $(document).on('click', '.judge-edit, #editJudgeBtn', function (e) {
            e.preventDefault();
            var judgeId = $(this).data("id") || $("#hdJudgeId").val();
            judgeControllerInstance.judgeId = judgeId;
            if (judgeId) {
                judgeControllerInstance.ViewJudge(judgeId, true);
                $("#JudgeEditModalLabel").html(`Edit Judge`);
                $("#edit_judgeName").hide();
                $("#edit_judgeNameText").show();
            } else {
                judgeControllerInstance.ClearEditForm();
                judgeControllerInstance.populateCourts(false);
                $("#JudgeEditModalLabel").html("Create New Judge");
                $("#edit_judgeName").show();
                $("#edit_judgeNameText").hide();
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.populateCourts(false);
            $("#JudgeEditModalLabel").html("Create New Judge");
            $("#edit_judgeName").show();
            $("#edit_judgeNameText").hide();
            editModal.show();
        });

        $("#edit_judgeName, #edit_judgeNameText").on("change", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var judgeId = $("#hdJudgeId").val();
            Swal.fire({
                title: 'Delete Judge?',
                text: 'Are you sure you wish to delete this Judge?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    judgeControllerInstance.DeleteJudge(judgeId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $judgeName = $("#edit_judgeName").is(":visible") ? $("#edit_judgeName") : $("#edit_judgeNameText");
            const $judgeNameError = $judgeName.next(".invalid-feedback");
            if ($judgeName.val().trim() === "") {
                $judgeNameError.show();
                $judgeName.addClass("is-invalid");
                isValid = false;
            } else {
                $judgeNameError.hide();
                $judgeName.removeClass("is-invalid");
            }

            if (isValid) {
                judgeControllerInstance.SaveJudge();
            }
        });

        $(document).on('click', '.judge-xref', function (e) {
            e.preventDefault();

            // Get the clicked row and extract judgeId + judge name directly from the DataTable
            const $row = $(this).closest('tr');
            const rowData = judgeControllerInstance.judgeTable.row($row).data();

            if (!rowData || !rowData.id) {
                Swal.fire({
                    title: 'Retrieve Judge Failed?',
                    text: 'The requested judge record could not be found',
                    icon: 'warning',
                    showCancelButton: true,
                });
                return;
            }

            const judgeId = rowData.id;
            const judgeName = rowData.name?.trim() || "Unknown Judge";
            $("#hdXrefJudgeId").val(judgeId);
            judgeControllerInstance.SetXrefJudgeHeader(judgeName);
            judgeControllerInstance.GetJudgeXrefs(judgeId);
            const xrefModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('JudgeXrefModal'));
            if (xrefModal) {
                xrefModal.show();
            } else {
                $('#JudgeXrefModal').modal('show');
            }
        });

        $("#xref_cmdSaveReference").on("click", function (e) {
            e.preventDefault();
            judgeControllerInstance.SaveJudgeXref();
        });
    }

    populateJudgeUsers() {
        $.ajax({
            url: `${this.service.baseUrl}JudgeAPI/GetJudgeDropDownItems/${this.judgeRole}/0`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                const $judgeSelect = $("#edit_judgeName");
                $judgeSelect.empty();
                $judgeSelect.append('<option value="">Select Judge</option>');
                if (response.data) {
                    response.data.forEach(user => {
                        $judgeSelect.append(`<option value="${user.Key}">${user.Value}</option>`);
                    });
                } else {
                    ShowNotification("Error", response.error || "Failed to load judge users.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Loading Judge Users", error.statusText || "Failed to load judge users. Please try again later.", 'error');
            }
        });
    }

    populateCourts(isEditMode = false, judgeCourtId = null, judgeCourtName = null) {
        $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourtsUnassigned`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                const $courtSelect = $("#edit_judgeCourt");
                $courtSelect.empty();
                $courtSelect.append('<option value="">Select Court</option>');

                let courts = response.data || [];

                if (isEditMode && judgeCourtId && judgeCourtName) {
                    courts = [{ id: judgeCourtId, description: judgeCourtName }, ...courts];
                }
                courts.forEach(court => {
                    if (court.id === judgeCourtId) {
                        $courtSelect.append(`<option value="${court.id}" selected>${court.description}</option>`);
                    } else {
                        $courtSelect.append(`<option value="${court.id}">${court.description}</option>`);
                    }
                });

                $("#edit_progress_judge").hide();
            },
            error: function (error) {
                ShowNotification("Error Loading Courts", error.statusText || "Failed to load unassigned courts. Please try again later.", 'error');
                $("#edit_progress_judge").hide();
            }
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
                        // Restore the selection only — deliberately no 'change'.
                        // Firing it here loaded the clerk judge list during page load,
                        // an auth + GetClerkJudges round trip to the clerk before any
                        // judge has been picked. The list loads when the
                        // cross-reference modal opens instead.
                        $select.val(lastCountyId);
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

    populateXrefJudges(countyId = null) {
        const $clerkJudge = $("#xref_clerkJudge");
        const progressId = "#xref_progress_judge";

        $(progressId).show();
        $clerkJudge.empty().append('<option value="">Select Clerk Judge</option>').prop('disabled', true);

        if (!countyId || countyId <= 0) {
            $(progressId).hide();
            return;
        }

        const url = `${this.service.baseUrl}JudgeAPI/GetJudgeOptions/${countyId}`;
        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            timeout: 15000,
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                const items = Array.isArray(response?.data) ? response.data : [];

                items.forEach(item => $clerkJudge.append(`<option value="${item.Key}">${item.Value}</option>`));

                if (items.length > 0) {
                    $clerkJudge.prop('disabled', false);
                    this.SelectClosestClerkJudge();
                    return;
                }

                // Nothing to choose from — say why rather than leaving an empty list.
                if (response?.error) {
                    ShowNotification("Clerk Judges", response.error, 'warning');
                } else {
                    ShowNotification("Clerk Judges", "No results were returned from the clerk.", 'info');
                }
            },
            error: (error) => {
                if (error.statusText === "timeout" || error.status === 0) {
                    ShowNotification("Timeout", "The request to load Clerk Judges timed out. Please try again later.", 'error');
                } else {
                    ShowNotification("Error Loading Clerk Judges", error.statusText || "Failed to load clerk judge list.", 'error');
                }
            },
            complete: () => $(progressId).hide()
        });
    }

    ClearState() {
        if (this.judgeTable) {
            this.judgeTable.state.clear();
            window.location.reload();
        }
    }

    DeleteJudge(judgeId) {
        $.ajax({
            url: this.deleteUrl + judgeId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (judgeControllerInstance.judgeTable) {
                        judgeControllerInstance.judgeTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('JudgeDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Judge deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Judge", error.statusText, 'error');
            }
        });
    }

    ClearDetailForm() {
        $("#judgeName").html("");
        $("#judgePhone").html("");
        $("#judgeCourt").html("");
        $("#judgeTitle").html("");
        $("#hdJudgeId").val("");
    }

    ClearEditForm() {
        $("#edit_judgeName").val("");
        $("#edit_judgeNameText").val("");
        $("#edit_judgePhone").val("");
        $("#edit_judgeCourt").val("");
        $("#edit_judgeTitle").val("");
        $("#edit_hdJudgeId").val("");
    }

    ClearEditValidations() {
        $("#edit_judgeName").removeClass("is-invalid");
        $("#edit_judgeNameText").removeClass("is-invalid");
        $("#edit_judgeName").next(".invalid-feedback").hide();
        $("#edit_judgePhone").removeClass("is-invalid");
        $("#edit_judgePhone").next(".invalid-feedback").hide();
        $("#edit_judgeCourt").removeClass("is-invalid");
        $("#edit_judgeCourt").next(".invalid-feedback").hide();
        $("#edit_judgeTitle").removeClass("is-invalid");
        $("#edit_judgeTitle").next(".invalid-feedback").hide();
    }

    ViewJudge(judgeId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}JudgeAPI/GetJudge/${judgeId}`;
        const progressId = isEditMode ? "#edit_progress_judge" : "#progress-judge";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('JudgeDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (judgeId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdJudgeId").val(response.data.id);
                            $("#edit_judgeNameText").val(response.data.name);
                            $("#edit_judgePhone").val(response.data.phone);
                            $("#edit_judgeCourt").val(response.data.court_id || "");
                            $("#edit_judgeTitle").val(response.data.title);
                            $("#JudgeEditModalLabel").html(`Edit Judge: ${response.data.name}`);
                            judgeControllerInstance.populateCourts(true, response.data.court_id, response.data.court_name);
                        } else {
                            $("#judgeName").html(response.data.name);
                            $("#judgePhone").html(response.data.phone);
                            $("#judgeCourt").html(response.data.court_name);
                            $("#judgeTitle").html(response.data.title);
                            $("#hdJudgeId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', response.error || 'Failed to retrieve judge details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification('Error Retrieving Judge Details', error.statusText || 'Failed to retrieve judge details. Please try again later.', 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveJudge() {
        const $judgeName = $("#edit_judgeName").is(":visible") ? $("#edit_judgeName") : $("#edit_judgeNameText");
        if ($judgeName.val().trim() === "") {
            $judgeName.addClass("is-invalid");
            $judgeName.next(".invalid-feedback").show();
            return;
        }

        if ($("#edit_hdJudgeId").val() === "") {
            this.CreateJudge();
        } else {
            this.UpdateJudge();
        }
        if (judgeControllerInstance.judgeTable) {
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.judgeTable.draw();
        }
    }

    CreateJudge() {
        try {
            $("#edit_progress_judge").show();
            const judgeData = {
                id: $("#edit_judgeName").val(),
                name: $("#edit_judgeName option:selected").text().trim(),
                phone: $("#edit_judgePhone").val().trim(),
                court_id: $("#edit_judgeCourt").val() || null,
                title: $("#edit_judgeTitle").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}JudgeAPI/CreateJudge`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(judgeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_judge").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Judge created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (judgeControllerInstance.judgeTable) {
                            judgeControllerInstance.judgeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating judge.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_judge").hide();
                    ShowNotification("Error Creating Judge", error.statusText || "Failed to create judge.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_judge").hide();
            ShowNotification("Error Creating Judge", e.message, 'error');
        }
    }

    UpdateJudge() {
        try {
            $("#edit_progress_judge").show();
            const judgeData = {
                id: parseInt($("#edit_hdJudgeId").val()),
                name: $("#edit_judgeNameText").val().trim(),
                phone: $("#edit_judgePhone").val().trim(),
                court_id: $("#edit_judgeCourt").val() || null,
                title: $("#edit_judgeTitle").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}JudgeAPI/UpdateJudge`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(judgeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_judge").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Judge updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (judgeControllerInstance.judgeTable) {
                            judgeControllerInstance.judgeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating judge.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_judge").hide();
                    ShowNotification("Error Updating Judge", error.statusText || "Failed to update judge.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_judge").hide();
            ShowNotification("Error Updating Judge", e.message, 'error');
        }
    }

    GetJudgeXrefs(judgeId) {
        const xrefList = `${this.service.baseUrl}JudgeAPI/GetJudgeXrefs/${judgeId}`;
        const progressId = "#xref_progress_judge";
        $(progressId).show();
        $("#hdXrefJudgeId").val(judgeId);

        // Armed here and consumed by the first draw below, so opening Manage Clerk
        // References lands on the county this judge is already mapped in. Only the
        // first draw: later ones follow a save or delete, and must not pull the
        // select out from under whatever the user has since chosen.
        this.preselectXrefCountyPending = true;

        if (judgeId) {
            // Destroy previous instance if exists (prevents DataTable re-init errors)
            if (this.judgeXrefTable) {
                this.judgeXrefTable.destroy();
            }
            this.judgeXrefTable = $('#tblJudgeXref').DataTable({
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
                        ShowNotification('Error Retrieving Judge Cross-References', error.statusText || 'Failed to retrieve xrefs.', 'error');
                    }
                },
                columns: [
                    {
                        data: "clerk_judge_id",
                        render: function (data) { return data || ''; }
                    },
                    {
                        data: "clerk_judge_name",
                        render: function (data) { return data || ''; }
                    },
                    {
                        data: "county_name",
                        render: function (data) { return data || ''; }
                    },
                    {
                        data: "judge_id",
                        render: function (data, type, row) {
                            return `<button type="button" class="delete-xref btn-command" data-bs-toggle="tooltip" data-county-id="${row.county_id}" data-judge-id="${row.judge_id}"><i class="fas fa-trash"></i></button>`;
                        },
                        className: "command-item",
                        orderable: false
                    },
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

            // Bind row action events AFTER every draw
            this.judgeXrefTable.on('draw', function () {
                $(".delete-xref").off("click").on("click", function (e) {
                    e.preventDefault();
                    const judgeId = $(this).data("judge-id");
                    const countyId = $(this).data("county-id");
                    Swal.fire({
                        title: 'Delete Judge Cross Reference?',
                        text: 'Are you sure you wish to delete this cross-reference?',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'Yes',
                        cancelButtonText: 'No'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            judgeControllerInstance.DeleteJudgeXref(judgeId, countyId);
                        }
                    });
                });
                judgeControllerInstance.DisableUsedCounties();
                judgeControllerInstance.PreselectXrefCounty();
            });
        }
        $(progressId).hide();
    }

    DeleteJudgeXref(judgeId, countyId) {
        $.ajax({
            url: `${this.service.baseUrl}JudgeAPI/DeleteJudgeXref/${judgeId}/${countyId}`,
            type: 'DELETE',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (judgeControllerInstance.judgeXrefTable) {
                        judgeControllerInstance.judgeXrefTable.draw();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Judge cross-reference deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Judge Xref", error.statusText, 'error');
            }
        });
    }

    SaveJudgeXref() {
        let isValid = true;
        const $county = $("#xref_county");
        const $clerkJudge = $("#xref_clerkJudge");

        if (!$county.val()) {
            $("#xref_county_error").show();
            $county.addClass("is-invalid");
            isValid = false;
        } else {
            $("#xref_county_error").hide();
            $county.removeClass("is-invalid");
        }

        if (!$clerkJudge.val()) {
            $("#xref_clerkJudge_error").show();
            $clerkJudge.addClass("is-invalid");
            isValid = false;
        } else {
            $("#xref_clerkJudge_error").hide();
            $clerkJudge.removeClass("is-invalid");
        }

        if (isValid) {
            $("#xref_progress_judge").show();
            this.CreateJudgeXref();
        }
    }

    CreateJudgeXref() {
        try {
            const xrefData = {
                judge_id: parseInt($("#hdXrefJudgeId").val()),
                county_id: parseInt($("#xref_county").val()),
                clerk_judge_id: $("#xref_clerkJudge").val() ? parseInt($("#xref_clerkJudge").val()) : 0,
                clerk_judge_name: $("#xref_clerkJudge option:selected").text().trim() || ''
            };

            $.ajax({
                url: `${this.service.baseUrl}JudgeAPI/CreateJudgeXref`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(xrefData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#xref_progress_judge").hide();
                    if (response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Judge Xref created successfully.'
                        });
                        judgeControllerInstance.judgeXrefTable.draw();
                        judgeControllerInstance.ClearXrefEditForm();
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                    }
                },
                error: function (error) {
                    $("#xref_progress_judge").hide();
                    ShowNotification("Error Creating Judge Xref", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#xref_progress_judge").hide();
            ShowNotification("Error Creating Judge Xref", e.message, 'error');
        }
    }

    ClearXrefEditForm() {
        const lastCountyId = localStorage.getItem('jacs_lastXrefCountyId') || "";
        $("#xref_county").val(lastCountyId).removeClass("is-invalid");
        $("#xref_county_error").hide();
        $("#xref_clerkJudge").val("").removeClass("is-invalid").prop("disabled", true);
        $("#xref_clerkJudge_error").hide();
        $("#hdXrefJudgeId").val("");

        // Reload the clerk list only while the cross-reference modal is actually
        // open — that is, after a save, with the user still working in it. This
        // method also runs from onModalClose for the other judge modals, where
        // firing 'change' sent an auth + GetClerkJudges request to the clerk merely
        // for closing an unrelated dialog. Called directly rather than by trigger so
        // jacs_lastXrefCountyId is not rewritten.
        if (lastCountyId && $("#JudgeXrefModal").hasClass("show")) {
            this.populateXrefJudges(parseInt(lastCountyId) || 0);
        }
    }

    ClearXrefJudgeHeader() {
        $("#xrefSelectedJudgeName").text("");
        $("#xrefJudgeHeader").hide();
    }

    SetXrefJudgeHeader(judgeName) {
        const $nameSpan = $("#xrefSelectedJudgeName");
        if (judgeName && judgeName.trim()) {
            $nameSpan.text(judgeName.trim());
            $("#xrefJudgeHeader").show();  // in case it was hidden
        } else {
            this.ClearXrefJudgeHeader();
        }
    }

    /// Reduces a judge name to the tokens worth matching on.
    ///
    /// Either system may write a name either way round — "Moss, Dann" or
    /// "Dann Moss" — so anything before the first comma moves to the end and the
    /// result is compared as a set, which makes ordering irrelevant. Honorifics,
    /// suffixes and single-letter middle initials come and go between the two
    /// systems, so they are dropped rather than left to spoil a match.
    /// Apostrophes are removed without splitting the name, so O'Brien and OBrien
    /// agree; hyphens do split it, so Smith-Jones matches either half.
    static JudgeNameTokens(name) {
        if (!name) return [];

        let s = String(name).toUpperCase();
        const comma = s.indexOf(',');
        if (comma > -1) s = `${s.slice(comma + 1)} ${s.slice(0, comma)}`;

        const ignored = new Set(['JUDGE', 'HON', 'HONORABLE', 'THE', 'JR', 'SR',
                                 'II', 'III', 'IV', 'MR', 'MRS', 'MS', 'DR']);

        return s.replace(/['’.]/g, '')
                .replace(/[^A-Z\s]/g, ' ')
                .split(/\s+/)
                .filter(t => t.length > 1 && !ignored.has(t));
    }

    /// Preselects the clerk judge whose name best matches the judge being edited,
    /// so changing the county lands on the obvious candidate instead of leaving
    /// the user to search the clerk's whole list.
    ///
    /// Nothing is selected unless one candidate wins outright: at least two name
    /// parts must match, and a tie selects nothing. A single shared surname is
    /// deliberately not enough — two different judges named Smith would otherwise
    /// produce a confident-looking guess. A wrong cross-reference saved without
    /// the user noticing is worse than an empty field, and the list is still
    /// there to pick from either way.
    SelectClosestClerkJudge() {
        const $clerkJudge = $("#xref_clerkJudge");
        const target = JudgeController.JudgeNameTokens($("#xrefSelectedJudgeName").text());
        if (target.length === 0) return;

        const targetSet = new Set(target);
        let bestValue = null;
        let bestScore = 0;
        let tied = false;

        $clerkJudge.find("option").each(function () {
            const value = $(this).val();
            if (!value) return;   // the "Select Clerk Judge" placeholder

            const tokens = JudgeController.JudgeNameTokens($(this).text());
            if (tokens.length === 0) return;

            const shared = tokens.filter(t => targetSet.has(t)).length;
            if (shared === 0) return;

            // An exact set match outranks every partial one, so a judge carrying
            // an extra middle name cannot beat the one who matches outright.
            const exact = shared === target.length && shared === tokens.length;
            const score = shared + (exact ? 100 : 0);

            if (score > bestScore) {
                bestScore = score;
                bestValue = value;
                tied = false;
            } else if (score === bestScore) {
                tied = true;
            }
        });

        if (bestValue !== null && !tied && bestScore >= 2) {
            $clerkJudge.val(bestValue);
        }
    }

    /// Points the county select at the county this judge is already mapped in.
    /// Without this the select keeps jacs_lastXrefCountyId, which is shared with
    /// the courtroom screen and is not per-judge, so opening a Sarasota judge
    /// showed whichever county was last used anywhere in the module.
    ///
    /// Runs once per modal open, armed by GetJudgeXrefs.
    PreselectXrefCounty() {
        if (!this.preselectXrefCountyPending) return;
        this.preselectXrefCountyPending = false;

        if (!this.judgeXrefTable) return;

        const $county = $("#xref_county");
        const rows = this.judgeXrefTable.rows().data();

        // Mapped in several counties? No single right answer, so take the first
        // row and let the user change it.
        let countyId = (rows && rows.length > 0 && rows[0] && rows[0].county_id)
            ? parseInt(rows[0].county_id)
            : 0;

        if (countyId && $county.find(`option[value="${countyId}"]`).length) {
            $county.val(String(countyId));
        } else {
            // Not mapped yet, so this is an add: keep whatever the select already
            // shows. The last-used county is a reasonable starting point.
            countyId = parseInt($county.val()) || 0;
        }

        // Load the dependent dropdown here — opening this modal is the first point
        // at which the clerk list is actually needed, and page load no longer
        // fetches it. Called directly rather than by firing 'change', because that
        // handler also rewrites jacs_lastXrefCountyId and this is the form filling
        // itself in, not the user picking a county. A zero id is handled inside:
        // the dropdown is cleared and left disabled, with no request sent.
        this.populateXrefJudges(countyId);
    }

    DisableUsedCounties() {
        const $countySelect = $("#xref_county");
        const usedCountyIds = new Set();

        // Collect all county_ids currently in the xref table
        if (this.judgeXrefTable) {
            this.judgeXrefTable.rows().every(function () {
                const data = this.data();
                if (data && data.county_id) {
                    usedCountyIds.add(parseInt(data.county_id));
                }
            });
        }

        // Enable/disable options
        $countySelect.find("option").each(function () {
            const val = parseInt($(this).val());
            if (val && val > 0) {
                if (usedCountyIds.has(val)) {
                    $(this).prop("disabled", true);
                } else {
                    $(this).prop("disabled", false);
                }
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'JudgeDetailModal') {
            judgeControllerInstance.ClearDetailForm();
        }
        else if (modalId === 'JudgeXrefModal') {
            $("#xref_progress_judge").hide();
            judgeControllerInstance.ClearXrefEditForm();
            judgeControllerInstance.ClearXrefJudgeHeader();
            if (judgeControllerInstance.judgeXrefTable) {
                judgeControllerInstance.judgeXrefTable.clear().draw();
            }
        }
        else if (modalId === 'JudgeEditModal') {
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.ClearEditValidations();
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}