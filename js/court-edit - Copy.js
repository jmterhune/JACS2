// court-edit.js
let courtControllerInstance = null;
class CourtController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin === "True" || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.pageSize = params.pageSize || 25;
        this.viewUrl = params.viewUrl || '/';
        this.editUrl = params.editUrl || '/';
        this.revisionUrl = params.revisionUrl || '/';
        this.calendarUrl = params.calendarUrl || '/';
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.courtId = getValueFromUrl('cid');
        this.searchTerm = "";
        this.courtTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        courtControllerInstance = this;
    }

    initEdit() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CourtAPI/DeleteCourt/`;
        this.populateCountyDropdown();
        this.populateAttorneyDropdown('#edit_defAttorney', 'Default Prosecuting Attorney');
        this.populateAttorneyDropdown('#edit_oppAttorney', 'Default Opposing Attorney');
        this.populateMotionDropdown('#edit_availableMotions', 'Select entries');
        this.populateMotionDropdown('#edit_restrictedMotions', 'Select entries');
        this.populateEventTypeDropdown('#edit_availableHearingTypes', 'Select entries');
        this.populateCaseTypeDropdown();
        this.initTemplatesTable();
        $('li a[href="#tab_templates"]').parent().hide();
        if (this.courtId) {
            this.ViewCourt(true);
        } else {
            $('#switch_allowWebScheduling').prop('disabled', true);
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

        $(".case-num-format-multi").on("input change", function () {
            courtControllerInstance.updateCaseNumFormatPreview();
        });
        $("[name='case_format_type']").on("change", function () {
            courtControllerInstance.updateCaseNumFormatPreview();
        });
        $('[data-toggle="tooltip"]').tooltip();
    }

    ViewCourt(edit = false) {
        const progressId = "#edit_progress-court";
        $(progressId).show();
        $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            data: { userId: this.userId },
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (result) {
                if (result && result.data) {
                    const court = result.data;
                    $("#edit_hdCourtId").val(court.id);
                    $("#edit_courtDescription").val(court.description);
                    $("#edit_courtCounty").val(court.county_id);
                    $("#edit_courtPlaintiff").val(court.plaintiff);
                    $("#edit_courtDefendant").val(court.defendant);
                    if (court.def_attorney_id) {
                        courtControllerInstance.preselectSelect2Option('#edit_defAttorney', court.def_attorney_item);
                    }
                    if (court.opp_attorney_id) {
                        courtControllerInstance.preselectSelect2Option('#edit_oppAttorney', court.opp_attorney_item);
                    }
                    $("#switch_emailConfirmations").prop('checked', court.email_confirmations).trigger('change');
                    $("#edit_calendarWeeks").val(court.calendar_weeks);
                    if (court.auto_extension) {
                        $("#edit_autoExtensionAuto").prop('checked', true);
                        $("#auto_extension_label").text("Automatic");
                        $("#add_template_row").show();
                    } else {
                        $("#edit_autoExtensionManual").prop('checked', true);
                        $("#auto_extension_label").text("Manual");
                        $("#add_template_row").hide();

                    }
                    $('#editor_customEmailBody').summernote('code', court.custom_email_body);
                    $('#editor_timeslotHeader').summernote('code', court.timeslot_header);
                    $("#edit_customHeader").val(court.custom_header);
                    $('#editor_webPolicy').summernote('code', court.web_policy);
                    $("#switch_allowWebScheduling").prop('checked', court.scheduling).trigger('change');
                    $("#switch_publicAvailableTimeslots").prop('checked', court.public_timeslot).trigger('change');
                    $("#switch_showDocketInternet").prop('checked', court.public_docket).trigger('change');
                    $("#edit_maxAvailableSlots").val(court.max_lagtime);
                    $("#edit_publicDocketDays").val(court.public_docket_days);
                    $("#switch_plaintiffRequired").prop('checked', court.plaintiff_required).trigger('change');
                    $("#switch_defendantRequired").prop('checked', court.defendant_required).trigger('change');
                    $("#switch_plaintiffAttorneyRequired").prop('checked', court.plaintiff_attorney_required).trigger('change');
                    $("#switch_defendantAttorneyRequired").prop('checked', court.defendant_attorney_required).trigger('change');
                    courtControllerInstance.preselectSelect2Options('#edit_availableMotions', court.available_motion_items);
                    courtControllerInstance.preselectSelect2Options('#edit_restrictedMotions', court.restricted_motion_items);
                    courtControllerInstance.preselectSelect2Options('#edit_availableHearingTypes', court.available_hearing_type_items);
                    $("[name='case_format_type'][value='" + court.case_format_type + "']").prop('checked', true).trigger('change');
                    const selectedRadio = $("[name='case_format_type']:checked");
                    const $row = selectedRadio.closest(".case-format-row");
                    const values = court.case_num_format.split("-");
                    const $inputs = $row.find(".case-num-format-multi");
                    $inputs.each(function (index) {
                        $(this).val(values[index]);
                        if ($(this).is('select')) {
                            $(this).trigger('change');
                        }
                    });
                    courtControllerInstance.loadTemplates();
                    if (!court.judge_name || court.judge_name <= 0) {
                        $('#switch_allowWebScheduling').prop('disabled', true);
                    } else {
                        $('#switch_allowWebScheduling').prop('disabled', false);
                    }
                    courtControllerInstance.updateLagTimeDisplay();
                    courtControllerInstance.updatePublicDocketDaysDisplay();
                    $(progressId).hide();

                } else {
                    $(progressId).hide();
                }
            },
            error: function (error) {
                ShowNotification("Error", "Failed to load court: " + error.statusText, 'error');
            }
        });
    }

    populateCaseTypeDropdown() {
        $.ajax({
            url: `${this.service.baseUrl}CourtTypeAPI/GetCourtTypeDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.data) {
                    $('select.case-type-select').each(function () {
                        const $select = $(this);
                        $select.empty();
                        $select.append('<option value="0">-</option>');
                        response.data.forEach(item => {
                            $select.append(`<option value="${item.Value}">${item.Value}</option>`);
                        });
                    });
                }
            },
            error: function (error) {
                console.error('Failed to fetch court types for dropdown');
                ShowNotification("Error", "Failed to load court types. Please try again later.", 'error');
            }
        });
    }
    validateCaseNumFormat() {
        const selectedRadio = $("[name='case_format_type']:checked");
        if (!selectedRadio.length) return false;

        const formatType = parseInt(selectedRadio.val());
        const $row = selectedRadio.closest(".case-format-row");
        const $inputs = $row.find(".case-num-format-multi");
        const values = $inputs
            .map(function () {
                return $(this).val() || $(this).attr("placeholder");
            })
            .get()
            .filter(val => val);

        // Validation rules based on case_format_type
        switch (formatType) {
            case 1:
                return values.length === 2 && values[0].length === 4 && values[1].length === 7;
            case 2:
                return values.length === 3 && values[0].length === 4 && values[1].length === 2 && values[2].length === 7;
            case 3:
                return values.length === 5 && values[0].length === 2 && values[1].length === 4 && values[2].length === 2 && values[3].length === 6 && values[4].length === 4;
            case 4:
                return values.length === 3 && values[0].length === 4 && values[1].length === 7 && values[2].length === 4;
            case 5:
                return values.length === 1 && values[0].length <= 12;
            default:
                return false;
        }
    }

    updateCaseNumFormatPreview() {
        const selectedRadio = $("[name='case_format_type']:checked");
        if (!selectedRadio.length) return;

        const formatType = parseInt(selectedRadio.val());
        const $row = selectedRadio.closest(".case-format-row");
        const $inputs = $row.find(".case-num-format-multi");
        const values = $inputs
            .map(function () {
                return $(this).val() || $(this).attr("placeholder");
            })
            .get()
            .filter(val => val);

        const caseNumFormat = values.join("-");
        $("#edit_courtCaseNumFormat").val(caseNumFormat);

        // Validate on update
        const $caseNumFormatError = $("#edit_courtCaseNumFormat").next(".invalid-feedback");
        if (this.validateCaseNumFormat()) {
            $caseNumFormatError.hide();
            $("#edit_courtCaseNumFormat").removeClass("is-invalid");
        } else {
            $caseNumFormatError.show();
            $("#edit_courtCaseNumFormat").addClass("is-invalid");
        }
    }

    populateCountyDropdown() {
        $.ajax({
            url: `${this.service.baseUrl}CountyAPI/GetCountyDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.data) {
                    const $countySelect = $("#edit_courtCounty");
                    $countySelect.empty();
                    $countySelect.append('<option value="">Select a County</option>');
                    response.data.forEach(county => {
                        $countySelect.append(`<option value="${county.Key}">${county.Value}</option>`);
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
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.Key,
                                text: item.Value
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
                cache: false
            },
            placeholder: placeholder,
            minimumInputLength: 2,
            allowClear: true,
            theme: "bootstrap-5",
        });
    }

    populateMotionDropdown(selector, placeholder) {
        $(selector).select2({
            ajax: {
                url: `${this.service.baseUrl}MotionAPI/GetMotionDropDownItems/`,
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.Key,
                                text: item.Value
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

    populateEventTypeDropdown(selector, placeholder) {
        $(selector).select2({
            ajax: {
                url: `${this.service.baseUrl}EventTypeAPI/GetEventTypeDropDownItems/`,
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                processResults: function (response) {
                    if (response.data) {
                        return {
                            results: response.data.map(item => ({
                                id: item.Key,
                                text: item.Value
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

    preselectSelect2Options(selector, items) {
        if (!items || !Array.isArray(items)) return;

        const $select = $(selector);
        $select.empty();

        items.forEach(item => {
            const option = new Option(item.Value, item.Key, true, true);
            $select.append(option);
        });

        $select.trigger('change');
    }

    preselectSelect2Option(selector, item) {
        if (!item) return;

        const $select = $(selector);
        $select.empty();

        const option = new Option(item.Value, item.Key, true, true);
        $select.append(option);
        $select.trigger('change');
    }

    updateLagTimeDisplay() {
        const $divLagTime = $("#div_lagTime");
        const $switch_allowWebScheduling = $("#switch_allowWebScheduling");
        const $switch_publicAvailableTimeslots = $("#switch_publicAvailableTimeslots");

        if ($switch_allowWebScheduling.is(':checked') || $switch_publicAvailableTimeslots.is(':checked')) {
            $divLagTime.removeClass("d-none").show();
        } else {
            $divLagTime.addClass("d-none").hide();
        }
    }

    updatePublicDocketDaysDisplay() {
        const $divPublicDocketDays = $("#div_publicDocketDays");
        const $switch_showDocketInternet = $("#switch_showDocketInternet");

        if ($switch_showDocketInternet.is(':checked')) {
            $divPublicDocketDays.removeClass("d-none").show();
        } else {
            $divPublicDocketDays.addClass("d-none").hide();
        }
    }

    populateTemplateDropdown() {
        const $selects = $('#templates_table tbody select.template-select');
        $.ajax({
            url: `${this.service.baseUrl}CourtTemplateAPI/GetCourtTemplateDropDownItems/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            data: { userId: this.userId },
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (result) {
                $selects.each(function () {
                    const $select = $(this);
                    $select.empty().append('<option value="">-</option>');
                    if (result && result.data) {
                        result.data.forEach(function (template) {
                            $select.append(`<option value="${template.Key}">${template.Value}</option>`);
                        });
                    }
                    $select.select2({
                        placeholder: 'Select a template',
                        allowClear: true
                    });
                });
            },
            error: function (error) {
                ShowNotification("Error", "Failed to load templates: " + error.statusText, 'error');
            }
        });
    }
    addTemplateRow(templateId = '', week = '', date = '', auto = null) {
        const isAuto = auto !== null ? auto : $("#edit_autoExtensionAuto").is(":checked");
        const rowId = 'template_' + Math.random().toString(36).substring(2, 9);
        const weekInput = isAuto
            ? `<input type="number" class="form-control week-input" value="${week}" min="1" max="52" required>`
            : `<input type="text" class="form-control date-input" value="${date}" disabled>`;
        const commandButtons = isAuto
            ? `<td>
                    <a href="" class="btn btn-sm text-danger delete-template" title="Delete" data-toggle="tooltip"><i class="fas fa-trash"></i></a>
                </td>`
            : "";
        const rowHtml = `
            <tr data-row-id="${rowId}" data-template-id="${templateId}" data-week="${week}" data-date="${date}" data-auto="${isAuto}">
                <td>${weekInput}</td>
                <td>
                    <select class="form-control template-select" style="width: 100%"></select>
                    <input type="hidden" class="auto-input" value="${isAuto ? 1 : 0}">
                </td>
                ${commandButtons}
            </tr>`;
        $('#templates_table tbody').append(rowHtml);
        this.populateTemplateDropdown();
        if (templateId) {
            $('#templates_table tbody tr:last-child select.template-select').val(templateId).trigger('change');
        }
    }
    loadTemplates() {
        $.ajax({
            url: `${this.service.baseUrl}CourtTemplateAPI/GetTemplatesByCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            data: { userId: this.userId },
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (result) {
                if (result && result.data) {
                    if (result.data.length > 0) {
                        courtControllerInstance.populateTemplates();
                        $('li a[href="#tab_templates"]').parent().show();
                    }
                }
            },
            error: function (error) {
                ShowNotification("Error", "Failed to load court templates: " + error.statusText, 'error');
            }
        });
    }
    populateTemplates() {
        $.ajax({
            url: `${this.service.baseUrl}CourtTemplateOrderAPI/GetTemplateOrdersByCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (result) {
                $('#templates_table tbody').empty();
                if (result && result.data) {
                    result.data.forEach(template => {
                        courtControllerInstance.addTemplateRow(
                            template.template_id,
                            template.order,
                            template.date_string,
                            template.auto
                        );
                    });
                }
            },
            error: function (error) {
                ShowNotification("Error", "Failed to load court templates: " + error.statusText, 'error');
            }
        });
    }
    updateCaseNumFormatPreview() {
        const selectedRadio = $("[name='case_format_type']:checked");
        if (!selectedRadio.length) return;

        const formatType = parseInt(selectedRadio.val());
        const $row = selectedRadio.closest(".case-format-row");
        const $inputs = $row.find(".case-num-format-multi");
        const values = $inputs
            .map(function () {
                return $(this).val() || $(this).attr("placeholder");
            })
            .get()
            .filter(val => val); // Remove empty values
        const caseNumFormat = values.join("-");
        $("#edit_courtCaseNumFormat").val(caseNumFormat);
    }
    // Function to validate the case number format based on type
    validateCaseNumFormat() {
        const selectedRadio = $("[name='case_format_type']:checked");
        if (!selectedRadio.length) return false;

        const formatType = selectedRadio.val();
        const $row = selectedRadio.closest(".case-format-row");
        const $inputs = $row.find(".case-num-format-multi");
        const values = $inputs
            .map(function () {
                return $(this).val() || $(this).attr("placeholder");
            })
            .get()
            .filter(val => val);

        switch (formatType) {
            case '1':
                return values.length === 2;
            case '2':
                return values.length === 3;
            case '3':
                return values.length === 6;
            case '4':
                return values.length === 3;
            case '5':
                return values.length === 1;
            default:
                return false;
        }
    }

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
    }

    CreateCourt() {
        try {
            $("#edit_progress-court").show();
            const selectedRadio = $("[name='case_format_type']:checked");
            const formatType = parseInt(selectedRadio.val()) || 3;
            const $row = selectedRadio.closest(".case-format-row");
            const $inputs = $row.find(".case-num-format-multi");
            const values = $inputs
                .map(function () {
                    return $(this).val() || $(this).attr("placeholder");
                })
                .get()
                .filter(val => val);
            const caseNumFormat = values.join("-");

            if (!this.validateCaseNumFormat()) {
                $("#edit_progress-court").hide();
                ShowNotification("Error", "Invalid case number format.", 'error');
                return;
            }

            const courtData = {
                description: $("#edit_courtDescription").val(),
                case_num_format: caseNumFormat,
                case_format_type: formatType,
                county_id: parseInt($("#edit_courtCounty").val()) || 0,
                plaintiff: $("#edit_courtPlaintiff").val(),
                defendant: $("#edit_courtDefendant").val(),
                def_attorney_id: parseInt($("#edit_defAttorney").val()) || null,
                opp_attorney_id: parseInt($("#edit_oppAttorney").val()) || null,
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
                available_hearing_types: $("#edit_availableHearingTypes").val() || [],
                templates: null // No templates for new court creation
            };

            $.ajax({
                url: `${this.service.baseUrl}CourtAPI/CreateCourt`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (result) {
                    if (result.status === 200) {
                        $("#edit_progress-court").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court created successfully.',
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                window.location.href = courtControllerInstance.viewUrl;
                            }
                        });
                    } else {
                        $("#edit_progress-court").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result.message, 'error');
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

    UpdateCourt() {
        try {
            $("#edit_progress-court").show();
            const selectedRadio = $("[name='case_format_type']:checked");
            const formatType = parseInt(selectedRadio.val()) || 3;
            const $row = selectedRadio.closest(".case-format-row");
            const $inputs = $row.find(".case-num-format-multi");
            const values = $inputs
                .map(function () {
                    return $(this).val() || $(this).attr("placeholder");
                })
                .get()
                .filter(val => val);
            const caseNumFormat = values.join("-");

            if (!this.validateCaseNumFormat()) {
                $("#edit_progress-court").hide();
                ShowNotification("Error", "Invalid case number format.", 'error');
                return;
            }
            const templates = [];
            $('#templates_table tbody tr').each(function () {
                const $row = $(this);
                const templateId = $row.data('templateId');
                const week = $row.data('week');
                const date = $row.data('date');
                const auto = $row.data('auto') === 'true' || $row.data('auto') === true;
                if (templateId && (week || date)) {
                    templates.push({
                        template_id: parseInt(templateId),
                        order: week ? parseInt(week) : null,
                        court_id: parseInt($("#edit_hdCourtId").val()),
                        date: date || null,
                        auto: auto
                    });
                }
            });
            const courtData = {
                id: parseInt($("#edit_hdCourtId").val()),
                description: $("#edit_courtDescription").val(),
                case_num_format: caseNumFormat,
                case_format_type: formatType,
                county_id: parseInt($("#edit_courtCounty").val()) || 0,
                plaintiff: $("#edit_courtPlaintiff").val(),
                defendant: $("#edit_courtDefendant").val(),
                def_attorney_id: parseInt($("#edit_defAttorney").val()) || null,
                opp_attorney_id: parseInt($("#edit_oppAttorney").val()) || null,
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
                available_hearing_types: $("#edit_availableHearingTypes").val() || [],
                templates: templates
            };

            $.ajax({
                url: `${this.service.baseUrl}CourtAPI/UpdateCourt`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(courtData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (result) {
                    if (result.status === 200) {
                        $("#edit_progress-court").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court updated successfully.',
                            confirmButtonText: 'OK'
                        });
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Court updated successfully.',
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                window.location.href = courtControllerInstance.viewUrl;
                            }
                        });
                    } else {
                        $("#edit_progress-court").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result.status, 'error');
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

    ClearEditForm() {
        $("#edit_courtDescription").val("");
        $("#edit_courtCaseNumFormat").val("");
        $("[name='case_format_type']").prop("checked", false);
        $("#radio_3").prop("checked", true);
        $(".case-num-format-multi").val("");
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
        $("#edit_lagTime").val("");
        $("#edit_publicDocketDays").val("");
        $('#templates_table tbody').empty();
    }

    ClearEditValidations() {
        $("#edit_courtDescription").removeClass("is-invalid");
        $("#edit_courtDescription").next(".invalid-feedback").hide();
        $("#edit_courtCounty").removeClass("is-invalid");
        $("#edit_courtCounty").next(".invalid-feedback").hide();
        $("#edit_calendarWeeks").removeClass("is-invalid");
        $("#edit_calendarWeeks").next(".invalid-feedback").hide();
        $("#edit_courtCaseNumFormat").removeClass("is-invalid");
        $("#edit_courtCaseNumFormat").next(".invalid-feedback").hide();
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }

    initTemplatesTable() {
        $("#add_template_row").on("click", function () {
            courtControllerInstance.addTemplateRow();
        });
        $('#templates_table tbody').on('click', '.delete-template', function (e) {
            e.preventDefault();
            courtControllerInstance.deleteTemplateRow(this);
        });
        $('#templates_table tbody').on('change', '.template-select', function () {
            const $row = $(this).closest('tr');
            $row.data('templateId', $(this).val());
        });
        $('#templates_table tbody').on('input change', '.week-input', function () {
            const $row = $(this).closest('tr');
            $row.data('week', $(this).val());
        });
    }

    deleteTemplateRow(deleteLink) {
        $(deleteLink).closest('tr').remove();
    }
}