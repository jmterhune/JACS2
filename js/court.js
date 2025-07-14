let courtControllerInstance = null;

class CourtController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.pageSize = params.pageSize || 25;
        this.viewUrl = params.viewUrl || '/';
        this.editUrl = params.editUrl || '/';
        this.revisionUrl = params.revisionUrl || '/';
        this.calendarUrl = params.calendarUrl || '/';
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.courtId = -1;
        this.searchTerm = "";
        this.courtTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtAPI/DeleteCourt/`;

        const listUrl = `${this.service.baseUrl}CourtAPI/GetCourts/${this.recordCount}`;
        const editUrl = this.editUrl;
        const calendarUrl = this.calendarUrl;
        const revisionUrl = this.revisionUrl;
        this.courtTable = $('#tblCourt').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                data: function (data) {
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                error: function (error) {
                    $("#tblCourt_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Courts", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Courts", "The following error occurred attempting to retrieve court information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<a href="${calendarUrl}/cid/${data}" title="View Calendar" data-toggle="tooltip" class="court-detail btn-command"><i class="fa-solid fa-calendar-days"></i></a>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<a href="${editUrl}/cid/${data}" title="Edit Court" data-toggle="tooltip" class="court-edit btn-command"><i class="fas fa-pencil"></i></a>`;
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
                    data: "judge_name",
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
                        if (row.has_revisions === true) {
                            return `<a href="${revisionUrl}/cid/${data}" title="Revisions" data-toggle="tooltip" class="revisions btn-command"><i class="fa-solid fa-clock-rotate-left"></i></a>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },

                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Court" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.courtTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const courtId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Court?',
                    text: 'Are you sure you wish to delete this Court?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        courtControllerInstance.DeleteCourt(courtId);
                    }
                });
            });
        });
    }

    initEdit() {
        const courtId = getValueFromUrl('courtId');
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtAPI/DeleteCourt/`;
        this.populateCountyDropdown();
        this.populateAttorneyDropdown('#edit_defAttorney', 'Default Prosecuting Attorney');
        this.populateAttorneyDropdown('#edit_oppAttorney', 'Default Opposing Attorney');
        this.populateMotionDropdown('#edit_availableMotions', 'Select entries');
        this.populateMotionDropdown('#edit_restrictedMotions', 'Select entries');
        this.populateEventTypeDropdown('#edit_availableHearingTypes', 'Select entries');

        if (courtId) {
            this.ViewCourt(courtId, true);
        }

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $courtDescription = $("#edit_courtDescription");
            const $courtDescriptionError = $courtDescription.next(".invalid-feedback");
            if ($courtDescription.val().trim() === "") {
                $courtDescriptionError.show();
                $courtDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $courtDescriptionError.hide();
                $courtDescription.removeClass("is-invalid");
            }
            const $courtCounty = $("#edit_courtCounty");
            const $courtCountyError = $courtCounty.next(".invalid-feedback");
            if ($courtCounty.val().trim() === "") {
                $courtCountyError.show();
                $courtCounty.addClass("is-invalid");
                isValid = false;
            } else {
                $courtCountyError.hide();
                $courtCounty.removeClass("is-invalid");
            }
            const $calendarWeeks = $("#edit_calendarWeeks");
            const $calendarWeeksError = $calendarWeeks.next(".invalid-feedback");
            if ($calendarWeeks.val().trim() === "" || parseInt($calendarWeeks.val()) < 0) {
                $calendarWeeksError.show();
                $calendarWeeks.addClass("is-invalid");
                isValid = false;
            } else {
                $calendarWeeksError.hide();
                $calendarWeeks.removeClass("is-invalid");
            }
            if (isValid) {
                courtControllerInstance.SaveCourt();
            }
        });

        $("#edit_courtDescription, #edit_calendarWeeks").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "" && ($this.attr('id') !== 'edit_calendarWeeks' || parseInt($this.val()) >= 0)) {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#switch_emailConfirmations").on("change", function () {
            $("#edit_emailConfirmations").val(this.checked ? "1" : "0");
        });
        $("#switch_allowWebScheduling").on("change", function () {
            $("#edit_allowWebScheduling").val(this.checked ? "1" : "0");
            courtControllerInstance.updateLagTimeDisplay();
        });
        $("#switch_publicAvailableTimeslots").on("change", function () {
            $("#edit_publicAvailableTimeslots").val(this.checked ? "1" : "0");
            courtControllerInstance.updateLagTimeDisplay();
        });
        $("#switch_showDocketInternet").on("change", function () {
            $("#edit_showDocketInternet").val(this.checked ? "1" : "0");
            courtControllerInstance.updatePublicDocketDaysDisplay();
        });
        $("#switch_plaintiffRequired").on("change", function () {
            $("#edit_plaintiffRequired").val(this.checked ? "1" : "0");
        });
        $("#switch_defendantRequired").on("change", function () {
            $("#edit_defendantRequired").val(this.checked ? "1" : "0");
        });
        $("#switch_plaintiffAttorneyRequired").on("change", function () {
            $("#edit_plaintiffAttorneyRequired").val(this.checked ? "1" : "0");
        });
        $("#switch_defendantAttorneyRequired").on("change", function () {
            $("#edit_defendantAttorneyRequired").val(this.checked ? "1" : "0");
        });

        $(".case_num_format_multiple").on("input change", function () {
            courtControllerInstance.updateCaseNumFormatPreview();
        });
        $("[name='case_format_type']").on("change", function () {
            courtControllerInstance.updateCaseNumFormatPreview();
        });
    }
    updateCaseNumFormatPreview() {
        const selectedRadio = $("[name='case_format_type']:checked");
        if (!selectedRadio.length) return;

        const formatType = selectedRadio.val();
        const $row = selectedRadio.closest(".case-format-row");
        const $inputs = $row.find(".case_num_format_multiple");
        const values = $inputs
            .map(function () {
                return $(this).val() || $(this).attr("placeholder");
            })
            .get()
            .filter(val => val); // Remove empty values
        const caseNumFormat = values.join("-");
        $("#edit_courtCaseNumFormat").val(caseNumFormat);
    }
    // Function to populate the county dropdown
    populateCountyDropdown() {
        $.ajax({
            url: `${this.service.baseUrl}CountyAPI/GetCounties`,
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (response) {
                if (response.data) {
                    const $countySelect = $("#edit_courtCounty");
                    $countySelect.empty();
                    $countySelect.append('<option value="">Select a County</option>');
                    response.data.forEach(county => {
                        $countySelect.append(`<option value="${county.id}">${county.name}</option>`);
                    });
                }
            },
            error: function (error) {
                if (error.status !== 401) {
                    console.error('Failed to fetch counties for dropdown');
                    ShowNotification("Error", "Failed to load counties. Please try again later.", 'error');
                }
            }
        });
    }
    // Function to populate the attorney dropdown with Select2
    populateAttorneyDropdown(selector, placeholder) {
        $(selector).select2({
            ajax: {
                url: `${this.service.baseUrl}AttorneyAPI/GetAttorneyDropDownItems/`,
                dataType: 'json',
                delay: 500,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page
                    };
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.id,
                                text: item.name
                            }))
                        };
                    } else {
                        return { results: null }
                    }
                },
                error: function () {
                    console.error('Failed to fetch attorney dropdown entries');
                    ShowNotification("Error", "Failed to retrieve attorney records. Make sure you are logged in and try again.", 'error');
                },
                cache: true
            },
            placeholder: placeholder,
            minimumInputLength: 2,
            allowClear: true,
            theme: "bootstrap-5",
        });
    }
    // Function to populate the motion dropdown with Select2
    populateMotionDropdown(selector, placeholder) {
        $(selector).select2({
            ajax: {
                url: `${this.service.baseUrl}MotionAPI/GetMotionDropDownItems/`,
                dataType: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.id,
                                text: item.description
                            }))
                        };
                    } else {
                        return { results: null }
                    }
                },
                error: function () {
                    console.error('Failed to fetch motion dropdown entries');
                    ShowNotification("Error", "Failed to retrieve motion records. Make sure you are logged in and try again.", 'error');
                },
                cache: true
            },
            placeholder: placeholder,
            multiple: true,
            allowClear: true,
            theme: "bootstrap-5",
        });
    }
    // Function to populate the hearing event type dropdown
    populateEventTypeDropdown(selector, placeholder) {
        $(selector).select2({
            ajax: {
                url: `${this.service.baseUrl}EventTypeAPI/GetEventTypeDropDownItems/`,
                dataType: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.id,
                                text: item.name
                            }))
                        };
                    } else {
                        return { results: null }
                    }
                },
                error: function () {
                    console.error('Failed to fetch hearing event type dropdown entries');
                    ShowNotification("Error", "Failed to retrieve hearing event type records. Make sure you are logged in and try again.", 'error');
                },
                cache: true
            },
            placeholder: placeholder,
            multiple: true,
            allowClear: true,
            theme: "bootstrap-5",
        });
    }
    // Function to preselect options in Select2 dropdowns
    preselectSelect2Options(selector, items) {
        if (!items || !Array.isArray(items)) return;

        const $select = $(selector);
        $select.empty(); // Clear existing options

        // Add options to Select2
        items.forEach(item => {
            const option = new Option(item.text, item.value, true, true);
            $select.append(option);
        });

        // Trigger change to update Select2 UI
        $select.trigger('change');
    }
    // Function to preselect a single option in Select2 dropdowns
    preselectSelect2Option(selector, item) {
        if (!item) return;

        const $select = $(selector);
        $select.empty(); // Clear existing options

        // Add options to Select2
        const option = new Option(item.text, item.value, true, true);
        $select.append(option);
        // Trigger change to update Select2 UI
        $select.trigger('change');
    }
    // Function to handle the display logic for $divLagTime
    updateLagTimeDisplay() {
        const $divLagTime = $("#div_lagTime");
        const $switch_allowWebScheduling = $("#switch_allowWebScheduling");
        const $switch_publicAvailableTimeslots = $("#switch_publicAvailableTimeslots");

        // Check if either switch is checked
        if ($switch_allowWebScheduling.is(':checked') || $switch_publicAvailableTimeslots.is(':checked')) {
            $divLagTime.removeClass("d-none").show();
        } else {
            $divLagTime.addClass("d-none").hide();
            $("#edit_lagTime").val("");
        }
    }
    // Function to handle the display logic for $divPublicDocketDays
    updatePublicDocketDaysDisplay() {
        const $divPublicDocketDays = $("#div_publicDocketDays");
        const $switch_showDocketInternet = $("#switch_showDocketInternet");
        // Check if the switch is checked
        if ($switch_showDocketInternet.is(':checked')) {
            $divPublicDocketDays.removeClass("d-none").show();
        } else {
            $divPublicDocketDays.addClass("d-none").hide();
            $("#edit_publicDocketDays").val("");
        }
    }
    // Function to view court details
    ViewCourt(courtId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CourtAPI/GetCourt/${courtId}`;
        const progressId = "#edit_progress-court";
        $(progressId).show();
        if (courtId) {
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
                            // Set court details
                            $("#edit_hdCourtId").val(response.data.id);
                            $("#edit_courtDescription").val(response.data.description);
                            $("#edit_courtCaseNumFormat").val(response.data.case_num_format);
                            const formatType = response.data.case_format_type || 3;
                            $(`#radio_${formatType}`).prop("checked", true);
                            const $row = $(`#radio_${formatType}`).closest(".case-format-row");
                            const $inputs = $row.find(".case_num_format_multiple");
                            if (response.data.case_num_format) {
                                const formatParts = response.data.case_num_format.split("-");
                                $inputs.each(function (index) {
                                    if (formatParts[index]) {
                                        $(this).val(formatParts[index]);
                                    }
                                });
                            }
                            $("#edit_courtCounty").val(response.data.county_id);
                            $("#edit_courtPlaintiff").val(response.data.plaintiff);
                            $("#edit_courtDefendant").val(response.data.defendant);
                            $("#edit_emailConfirmations").val(response.data.email_confirmations ? "1" : "0");
                            $("#switch_emailConfirmations").prop("checked", response.data.email_confirmations);
                            $("#edit_calendarWeeks").val(response.data.calendar_weeks);
                            $(`#edit_autoExtension${response.data.auto_extension ? 'Auto' : 'Manual'}`).prop("checked", true);
                            $("#edit_customHeader").val(response.data.custom_header);
                            $('#editor_webPolicy').summernote('code', response.data.web_policy);
                            $('#editor_customEmailBody').summernote('code', response.data.custom_email_body);
                            $('#editor_timeslotHeader').summernote('code', response.data.timeslot_header);
                            $("#edit_allowWebScheduling").val(response.data.scheduling ? "1" : "0");
                            $("#switch_allowWebScheduling").prop("checked", response.data.scheduling);

                            // Set public timeslot switch and toggle div_lagTime
                            $("#edit_publicAvailableTimeslots").val(response.data.public_timeslot ? "1" : "0");
                            $("#switch_publicAvailableTimeslots").prop("checked", response.data.public_timeslot);
                            $("#div_lagTime").toggle(response.data.public_timeslot); // Show/hide based on switch state
                            if (!response.data.public_timeslot) {
                                $("#edit_lagTime").val(""); // Clear input if hidden
                            }

                            // Set public docket switch and toggle div_publicDocketDays
                            $("#edit_showDocketInternet").val(response.data.public_docket ? "1" : "0");
                            $("#switch_showDocketInternet").prop("checked", response.data.public_docket);
                            $("#div_publicDocketDays").toggle(response.data.public_docket); // Show/hide based on switch state
                            if (!response.data.public_docket) {
                                $("#edit_publicDocketDays").val(""); // Clear input if hidden
                            }

                            $("#edit_maxAvailableSlots").val(response.data.max_lagtime || '');
                            $("#edit_publicDocketDays").val(response.data.public_docket_days || '');
                            $("#edit_plaintiffRequired").val(response.data.plaintiff_required ? "1" : "0");
                            $("#switch_plaintiffRequired").prop("checked", response.data.plaintiff_required);
                            $("#edit_defendantRequired").val(response.data.defendant_required ? "1" : "0");
                            $("#switch_defendantRequired").prop("checked", response.data.defendant_required);
                            $("#edit_plaintiffAttorneyRequired").val(response.data.plaintiff_attorney_required ? "1" : "0");
                            $("#switch_plaintiffAttorneyRequired").prop("checked", response.data.plaintiff_attorney_required);
                            $("#edit_defendantAttorneyRequired").val(response.data.defendant_attorney_required ? "1" : "0");
                            $("#switch_defendantAttorneyRequired").prop("checked", response.data.defendant_attorney_required);
                            if (!response.data.judge_name) {
                                $('#switch_allowWebScheduling').prop('disabled', true);
                            }
                            // Preselect Select2 options
                            courtControllerInstance.preselectSelect2Options('#edit_availableMotions', response.data.available_motion_items);
                            courtControllerInstance.preselectSelect2Options('#edit_restrictedMotions', response.data.restricted_motion_items);
                            courtControllerInstance.preselectSelect2Options('#edit_availableHearingTypes', response.data.available_hearing_type_items);
                            courtControllerInstance.preselectSelect2Option('#edit_defAttorney', response.data.def_attorney_item);
                            courtControllerInstance.preselectSelect2Option('#edit_oppAttorney', response.data.opp_attorney_item);
                            courtControllerInstance.updateLagTimeDisplay(); // Update lag time display based on switches
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve court details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Court Details", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Courts Details", "The following error occurred attempting to retrieve court information. Error: " + error.statusText, 'error');
                    }
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }
    // Function to save the court details
    SaveCourt() {
        if ($("#edit_hdCourtId").val() === "") {
            this.CreateCourt();
        } else {
            this.UpdateCourt();
        }
        if (courtControllerInstance.courtTable) {
            courtControllerInstance.ClearEditForm();
            courtControllerInstance.courtTable.draw();
        }
        window.location.href = this.viewUrl;
    }
    // Function to create a new court
    CreateCourt() {
        try {
            $("#edit_progress-court").show();
            const selectedRadio = $("[name='case_format_type']:checked");
            const $row = selectedRadio.closest(".case-format-row");
            const $inputs = $row.find(".case_num_format_multiple");
            const caseNumFormat = $inputs
                .map(function () {
                    return $(this).val() || $(this).attr("placeholder");
                })
                .get()
                .filter(val => val)
                .join("-");
            const courtData = {
                description: $("#edit_courtDescription").val(),
                case_num_format: caseNumFormat,
                case_format_type: parseInt(selectedRadio.val()) || 3,
                county_id: $("#edit_courtCounty").val() || 0,
                plaintiff: $("#edit_courtPlaintiff").val(),
                defendant: $("#edit_courtDefendant").val(),
                def_attorney_id: $("#edit_defAttorney").val() || null,
                opp_attorney_id: $("#edit_oppAttorney").val() || null,
                email_confirmations: $("#edit_emailConfirmations").val() === "1",
                calendar_weeks: parseInt($("#edit_calendarWeeks").val()) || 0,
                auto_extension: $("#edit_autoExtensionAuto").is(":checked"),
                custom_email_body: $('#editor_customEmailBody').summernote('code'),
                timeslot_header: $('#editor_timeslotHeader').summernote('code'),
                custom_header: $("#edit_customHeader").val(),
                web_policy: $('#editor_webPolicy').summernote('code'),
                scheduling: $("#edit_allowWebScheduling").val() === "1",
                public_timeslot: $("#edit_publicAvailableTimeslots").val() === "1",
                public_docket: $("#edit_showDocketInternet").val() === "1",
                max_lagtime: parseInt($("#edit_maxAvailableSlots").val()) || null,
                public_docket_days: parseInt($("#edit_publicDocketDays").val()) || null,
                plaintiff_required: $("#edit_plaintiffRequired").val() === "1",
                defendant_required: $("#edit_defendantRequired").val() === "1",
                plaintiff_attorney_required: $("#edit_plaintiffAttorneyRequired").val() === "1",
                defendant_attorney_required: $("#edit_defendantAttorneyRequired").val() === "1",
                available_motions: $("#edit_availableMotions").val() || [],
                restricted_motions: $("#edit_restrictedMotions").val() || [],
                available_hearing_types: $("#edit_availableHearingTypes").val() || []
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtAPI/CreateCourt`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-court").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court created successfully.'
                        });
                    } else {
                        $("#edit_progress-court").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-court").hide();
                    ShowNotification("Error Creating Court", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-court").hide();
            ShowNotification("Error Creating Court", e.message, 'error');
        }
    }
    // Function to update an existing court
    UpdateCourt() {
        try {
            $("#edit_progress-court").show();
            const selectedRadio = $("[name='case_format_type']:checked");
            const $row = selectedRadio.closest(".case-format-row");
            const $inputs = $row.find(".case_num_format_multiple");
            const caseNumFormat = $inputs
                .map(function () {
                    return $(this).val() || $(this).attr("placeholder");
                })
                .get()
                .filter(val => val)
                .join("-");
            const courtData = {
                id: $("#edit_hdCourtId").val(),
                description: $("#edit_courtDescription").val(),
                case_num_format: caseNumFormat,
                case_format_type: parseInt(selectedRadio.val()) || 3,
                county_id: $("#edit_courtCounty").val() || 0,
                plaintiff: $("#edit_courtPlaintiff").val(),
                defendant: $("#edit_courtDefendant").val(),
                def_attorney_id: $("#edit_defAttorney").val() || null,
                opp_attorney_id: $("#edit_oppAttorney").val() || null,
                email_confirmations: $("#edit_emailConfirmations").val() === "1",
                calendar_weeks: parseInt($("#edit_calendarWeeks").val()) || 0,
                auto_extension: $("#edit_autoExtensionAuto").is(":checked"),
                custom_email_body: $('#editor_customEmailBody').summernote('code'),
                timeslot_header: $('#editor_timeslotHeader').summernote('code'),
                custom_header: $("#edit_customHeader").val(),
                web_policy: $('#editor_webPolicy').summernote('code'),
                scheduling: $("#edit_allowWebScheduling").val() === "1",
                public_timeslot: $("#edit_publicAvailableTimeslots").val() === "1",
                public_docket: $("#edit_showDocketInternet").val() === "1",
                max_lagtime: parseInt($("#edit_maxAvailableSlots").val()) || null,
                public_docket_days: parseInt($("#edit_publicDocketDays").val()) || null,
                plaintiff_required: $("#edit_plaintiffRequired").val() === "1",
                defendant_required: $("#edit_defendantRequired").val() === "1",
                plaintiff_attorney_required: $("#edit_plaintiffAttorneyRequired").val() === "1",
                defendant_attorney_required: $("#edit_defendantAttorneyRequired").val() === "1",
                available_motions: $("#edit_availableMotions").val() || [],
                restricted_motions: $("#edit_restrictedMotions").val() || [],
                available_hearing_types: $("#edit_availableHearingTypes").val() || []
            };
            $.ajax({
                url: `${this.service.baseUrl}CourtAPI/UpdateCourt`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-court").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court updated successfully.'
                        });
                    } else {
                        $("#edit_progress-court").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-court").hide();
                    ShowNotification("Error Updating Court", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-court").hide();
            ShowNotification("Error Updating Court", e.message, 'error');
        }
    }
    // Function to delete a court
    DeleteCourt(courtId) {
        $.ajax({
            url: this.deleteUrl + courtId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (courtControllerInstance.courtTable) {
                    courtControllerInstance.courtTable.draw();
                }
                window.location.href = this.viewUrl;
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Court deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Court", error.statusText, 'error');
            }
        });
    }
    // Function to clear the edit form fields
    ClearEditForm() {
        $("#edit_courtDescription").val("");
        $("#edit_courtCaseNumFormat").val("");
        $("#edit_courtCounty").val("");
        $("#edit_courtPlaintiff").val("");
        $("#edit_courtDefendant").val("");
        $("#edit_defAttorney").val("").trigger('change');
        $("#edit_oppAttorney").val("").trigger('change');
        $("#edit_emailConfirmations").val("0");
        $("#switch_emailConfirmations").prop("checked", false);
        $("#edit_calendarWeeks").val("0");
        $("#edit_autoExtensionAuto").prop("checked", true);
        $("#edit_autoExtensionManual").prop("checked", false);
        $("#edit_customEmailBody").val("");
        $("#editor_customEmailBody").summernote('code', '');
        $("#edit_timeslotHeader").val("");
        $("#editor_timeslotHeader").summernote('code', '');
        $("#edit_customHeader").val("");
        $("#edit_webPolicy").val("");
        $("#editor_webPolicy").summernote('code', '');
        $("#edit_hdCourtId").val("");
        $("#edit_allowWebScheduling").val("0");
        $("#switch_allowWebScheduling").prop("checked", false);
        $("#edit_publicAvailableTimeslots").val("0");
        $("#switch_publicAvailableTimeslots").prop("checked", false);
        $("#edit_showDocketInternet").val("0");
        $("#switch_showDocketInternet").prop("checked", false);
        $("#edit_maxAvailableSlots").val("");
        $("#edit_plaintiffRequired").val("0");
        $("#switch_plaintiffRequired").prop("checked", false);
        $("#edit_defendantRequired").val("0");
        $("#switch_defendantRequired").prop("checked", false);
        $("#edit_plaintiffAttorneyRequired").val("0");
        $("#switch_plaintiffAttorneyRequired").prop("checked", false);
        $("#edit_defendantAttorneyRequired").val("0");
        $("#switch_defendantAttorneyRequired").prop("checked", false);
        $("#edit_availableMotions").val([]).trigger('change');
        $("#edit_restrictedMotions").val([]).trigger('change');
        $("#edit_availableHearingTypes").val([]).trigger('change');
        $("[name='case_format_type']").prop("checked", false);
        $("#third_radio").prop("checked", true);
        $(".case_num_format_multiple").val("");
        $("#edit_publicDocketDays").val("");
    }
    // Function to clear validation errors in the edit form
    ClearEditValidations() {
        $("#edit_courtDescription").removeClass("is-invalid");
        $("#edit_courtDescription").next(".invalid-feedback").hide();
        $("#edit_courtCounty").removeClass("is-invalid");
        $("#edit_courtCounty").next(".invalid-feedback").hide();
        $("#edit_calendarWeeks").removeClass("is-invalid");
        $("#edit_calendarWeeks").next(".invalid-feedback").hide();
    }
    // Function to clear the state of the DataTable and reload the page
    ClearDataTableState() {
        if (this.courtTable) {
            this.courtTable.state.clear();
            window.location.reload();
        }
    }
}