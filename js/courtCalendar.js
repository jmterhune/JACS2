// Filename: Resources/js/courtcalendar.js
let courtCalendarControllerInstance = null;
let multi_timeslots = [];
let dragEvents = [];
class CourtCalendarController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True" ? true : false || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.service = params.service || null;
        this.calendar = null;
        this.rescheduleCalendar = null;
        this.courtId = params.courtId;
        this.courtEditUrl = params.courtEditUrl || '/court-edit';
        this.userDefinedFieldUrl = params.userDefinedFieldUrl || '/user-fields';
        this.truncateCalendarUrl = params.truncateCalendarUrl || '/truncate-calendar';
        this.calendarUrl = params.calendarUrl || '/court-calendar';
        this.courtData = null;
        this.caseTypes = null;
        this.currentEvent = null;
        this.currentTimeslot = null; // Default duration in minutes
        this.calendarItem = params.calendarItem || null;
        this.initialDate = new Date();
        this.initialView = 'timeGridWeek';
        this.pendingTab = null;
        this.editable = params.editable == "True" ? true : false || false;
        // Incremented on every populateMotionSelectExcludingRestricted call so
        // older responses can be discarded when a newer call supersedes them.
        this._motionFetchVersion = 0;
        courtCalendarControllerInstance = this;
    }

    // Initialization Methods
    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const promCourt = this.fetchCourtData();
        const promEventType = this.populateEventTypeSelect();
        const promCaseTypes = this.populateCaseTypes();
        const promAttorney = this.populateAttorneySelects();
        if (this.editable) {
            $('#copyTimeslotsBtn').show();
            $('#deleteTimeslotsBtn').show();
            $('#editCourtBtn').show();
            $('#extendBtn').show();
            $('#userDefinedFieldsBtn').show();
        } else {
            $('#btnExtend').hide();
            $('#cancelHearingBtn').hide();
            $('#rescheduleBtn').hide();
            $('#saveEventPaneBtn').hide();
            $('#btnExtend').hide();
            $('#deleteTimeslotPaneBtn').hide();
            $('#saveTimeslotPaneBtn').hide();
        }
        if (isAdmin)
            $("#truncateBtn").show();
        if (this.calendarItem && this.calendarItem.start) {
            const itemStart = new Date(this.calendarItem.start);
            const itemEnd = new Date(this.calendarItem.end);
            const diffHours = (itemEnd - itemStart) / (1000 * 60 * 60);
            this.initialDate = itemStart;
            this.initialView = diffHours < 24 ? 'timeGridDay' : 'timeGridWeek';
        }
        this.initCalendar();
        this.bindEventHandlers();
        this.populateCourtMotions();
        const courtTypeSelect = $(".court-types option:selected").text();
        if (courtTypeSelect.length) {
            this.changeLabel(courtTypeSelect);
        }
        const timeslotModalElement = document.getElementById('TimeslotModal');
        if (timeslotModalElement) {
            timeslotModalElement.addEventListener('hidden.bs.modal', this.handleModalClose);
            timeslotModalElement.addEventListener('shown.bs.modal', this.handleTimeslotModalShow.bind(this));
        }
        const rescheduleModalElement = document.getElementById('RescheduleHearingModal');
        if (rescheduleModalElement) {
            rescheduleModalElement.addEventListener('hidden.bs.modal', () => {
                if (this.rescheduleCalendar) {
                    this.rescheduleCalendar.destroy();
                    this.rescheduleCalendar = null;
                }
            });
        }

        $.when(promCourt, promEventType, promCaseTypes, promAttorney).then(() => {
            // Populate courtroom select after court data is loaded so we have county_id.
            // Return the populate promise so the next .then() waits for the courtroom
            // options to exist BEFORE viewTimeslot tries to select a value on the
            // #timeslot_courtroom / #event_courtroom dropdowns.
            const courtroomPromise = this.populateCourtroomSelect();
            this.populateEventDefaults();
            return courtroomPromise;
        }).then(() => {
            if (this.calendarItem && this.calendarItem.timeslotId > 0) {
                this.showTimeslotModal(this.calendarItem);
            }
        }).fail(() => console.error('One or more data fetches failed'));
    }

    initCalendar() {
        const calendarEl = document.getElementById('calendar');
        this.calendar = new FullCalendar.Calendar(calendarEl, {
            initialDate: this.initialDate || new Date(),
            initialView: this.initialView,
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
            },
            selectable: this.editable,
            selectMirror: true,
            editable: this.editable,
            select: this.handleDateSelect.bind(this),
            eventClick: this.handleEventClick.bind(this),
            eventDrop: this.handleEventDrop.bind(this),
            eventDragStop: function (info) {
                $('#calendar input:checked').each(function () {
                    dragEvents.push($(this).val());
                });
            },
            selectAllow: function (selectInfo) {
                return selectInfo.start.getDay() !== 0 && selectInfo.start.getDay() !== 6;
            },
            eventResize: this.handleEventResize.bind(this),
            eventConstraint: {
                startTime: '07:00',
                endTime: '17:30',
                daysOfWeek: [1, 2, 3, 4, 5]
            },
            navLinks: true,
            weekends: false,
            slotMinTime: '08:00:00',
            slotMaxTime: '17:30:00',
            slotDuration: '00:05:00',
            allDaySlot: false,
            height: 'auto',
            hiddenDays: [0, 6],
            eventDidMount: (arg) => {
                if (arg.isMirror) {
                    arg.el.style.backgroundColor = 'rgb(0, 123, 255)';
                    arg.el.style.borderColor = 'rgb(0, 123, 255)';
                    arg.el.style.color = 'white';
                    arg.el.style.borderRadius = '5px';
                    const html = `<div class="fc-event-main"><span>${this.getTimeRange(arg.event.start, arg.event.end)}</span><div><br></div></div><div class="fc-event-resizer fc-event-resizer-start"></div><div class="fc-event-resizer fc-event-resizer-end"></div>`;
                    arg.el.innerHTML = html;

                    const harness = arg.el.closest('.fc-timegrid-event-harness');
                    const mirrorEl = arg.el;
                    const slotHeight = document.querySelector('.fc-timegrid-slot').getBoundingClientRect().height;
                    const totalDayMinutes = 9 * 60;

                    const observer = new MutationObserver((mutations) => {
                        mutations.forEach((mutation) => {
                            if (mutation.type === 'attributes' && mutation.attributeName === 'style') {
                                const col = harness.closest('.fc-timegrid-col');
                                const colRect = col.getBoundingClientRect();
                                const harnessRect = harness.getBoundingClientRect();
                                const topPixel = harnessRect.top - colRect.top;
                                const height = harnessRect.height;

                                const colMinutesPerPixel = totalDayMinutes / colRect.height;
                                const minutesFromTop = topPixel * colMinutesPerPixel;
                                const startMinutes = 8 * 60 + minutesFromTop;
                                const durationMinutes = height * colMinutesPerPixel;

                                const dataDate = col.dataset.date;
                                const dayStart = new Date(dataDate);
                                dayStart.setHours(0, 0, 0, 0);

                                const newStart = new Date(dayStart.getTime() + startMinutes * 60 * 1000);
                                const newEnd = new Date(newStart.getTime() + durationMinutes * 60 * 1000);

                                mirrorEl.querySelector('.fc-event-main span').textContent = this.getTimeRange(newStart, newEnd);
                            }
                        });
                    });
                    observer.observe(harness, { attributes: true });
                }
            },
            eventContent: function (arg) {
                let timeText = arg.timeText;
                let tsId = arg.event.id;
                let checkbox = `<input class="calendar-select" disabled="" type="checkbox" id="cb${tsId}" value="${tsId}">`;
                let titleSpan = `<span>${arg.event.title}</span>`;
                let weekText = arg.event.extendedProps.template_week_order;
                if (!weekText || weekText.length == 0) {
                    weekText = '';
                } else {
                    weekText = " (Week " + weekText + ")";
                }
                if (arg.view.type === "listMonth") {
                    return { html: titleSpan };
                } else {
                    if (arg.event.extendedProps.eventCount == 0) {
                        timeText = `<span>${timeText}${weekText}${checkbox}</span>`
                    } else {
                        timeText = `<span>${timeText}${weekText}</span>`
                    }
                    if (arg.event.extendedProps.total_length === "5 minutes") {
                        titleSpan = `<div> -- ${titleSpan}</div>`
                    } else {
                        titleSpan = `<div>${titleSpan}</div>`
                    }
                }
                return { html: timeText + titleSpan };
            },
            datesSet: (dateInfo) => {
                this.calendar.getEventSources().forEach(source => source.remove());
                switch (dateInfo.view.type) {
                    case 'dayGridMonth':
                        $("#printCalendarBtn").show();
                        this.calendar.addEventSource({
                            events: (fetchInfo, successCallback, failureCallback) => {
                                fetch(`${this.service.baseUrl}TimeslotAPI/GetMonthlyCourtTimeslots/${this.courtId}?start=${dateInfo.startStr}&end=${dateInfo.endStr}`)
                                    .then(response => response.json())
                                    .then(events => successCallback(events))
                                    .catch(error => failureCallback(error));
                            }
                        });
                        break;
                    case 'timeGridWeek':
                        $("#printCalendarBtn").hide();
                    case 'timeGridDay':
                        $("#printCalendarBtn").hide();
                    case 'listWeek':
                        $("#printCalendarBtn").hide();
                        this.calendar.addEventSource({
                            events: (fetchInfo, successCallback, failureCallback) => {
                                fetch(`${this.service.baseUrl}TimeslotAPI/GetCourtTimeslots/${this.courtId}?start=${dateInfo.startStr}&end=${dateInfo.endStr}`)
                                    .then(response => response.json())
                                    .then(events => successCallback(events))
                                    .catch(error => failureCallback(error));
                            }
                        });
                        break;
                }
            }
        });

        this.calendar.render();
    }

    showTimeslotModal(calItem) {
        // Chain: load the timeslot (including motion-dropdown population) to
        // completion BEFORE firing viewEvent. Otherwise viewEvent races ahead,
        // sets #event_motion.val() against an empty dropdown, reads an empty
        // #timeslot_courtroom for #event_courtroom, and has its .edited-by
        // show() undone when viewTimeslot finally resolves and calls hide().
        const p = this.viewTimeslot(calItem.timeslotId);
        if (calItem.eventId > 0) {
            $.when(p).always(() => this.viewEvent(calItem.eventId));
        }
    }

    initRescheduleCalendar() {
        const resCalendarEl = document.getElementById('reschedule-calendar');
        const startDate = this.formatLocalDateTime(this.currentTimeslot.start);
        this.rescheduleCalendar = new FullCalendar.Calendar(resCalendarEl, {
            initialView: 'listMonth',
            headerToolbar: {
                left: 'prev,next',
                center: 'title',
                right: ''
            }, navLinks: true,
            weekends: false,
            events: `${this.service.baseUrl}TimeslotAPI/GetAvailableCourtTimeslots/${this.courtId}/${this.currentEvent.motion_id}/${this.currentTimeslot.duration}?startDate=${startDate}`,
            eventClick: this.handleRescheduleClick.bind(this),
            height: 500
        });
        this.rescheduleCalendar.render();
    }

    // Data Fetch Methods
    fetchCourtData() {
        return $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    this.courtData = response.data;
                    this.populateCourtTemplateFields();
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load court data.', 'error');
            }
        });
    }

    fetchTemplateData() {
        let template = {};
        $('[name^="template["]').each(function () {
            let el = $(this);
            let key = el.attr('name').match(/template\[(.*?)\]/)[1];
            if (el.is(':radio')) {
                if (el.is(':checked')) {
                    template[key] = el.val();
                }
            } else {
                template[key] = el.val();
            }
        });
        return JSON.stringify(template);
    }

    // Populate Control Methods
    populateAttorneySelects() {
        const self = this;
        const selects = ['event_attorney', 'event_opposingAttorney'];
        selects.forEach(id => {
            const select = document.getElementById(id);
            if (!select) return;
            const ts = new TomSelect(select, {
                // Value = bar_num (matches what the clerk returns).
                // attorney_id is a custom field carrying the internal DB id.
                valueField: 'value',
                labelField: 'text',
                searchField: ['text'],
                maxItems: 1,
                placeholder: 'Type name or bar number',
                persist: false,
                create: false,
                // Remote search — fires on every keystroke after 2 chars
                load: (query, callback) => {
                    if (!query.length) return callback();
                    $.ajax({
                        url: `${self.service.baseUrl}AttorneyAPI/GetAttorneyDropDownItems`,
                        type: 'GET',
                        data: { q: query },
                        dataType: 'json',
                        beforeSend: xhr => self.setAjaxHeaders(xhr),
                        success: response => {
                            const options = (response.data || []).map(a => ({
                                value: a.bar_num,
                                text: a.label,
                                attorney_id: String(a.id)
                            }));
                            callback(options);
                        },
                        error: () => callback()
                    });
                },
                plugins: {
                    clear_button: { title: 'Clear' }
                },
                onItemAdd: (value) => {
                    // Stamp the internal DB id onto the underlying <option> so
                    // getEventFormData() can read it without another lookup.
                    const opt = select.querySelector(`option[value="${CSS.escape(value)}"]`);
                    if (opt) {
                        const chosen = ts.options[value];
                        if (chosen) opt.dataset.attorneyId = chosen.attorney_id;
                    }
                }
            });
            select.setAttribute('tabindex', '-1');
            select.setAttribute('autocomplete', 'off');
        });
        // Return a resolved deferred so the existing $.when() chain in init() still works
        return $.Deferred().resolve().promise();
    }

    /**
     * Ensures an attorney option identified by bar number exists in the given
     * TomSelect instance, fetching it from the server if necessary, then
     * selects it.  Used when populating from a clerk case search result.
     *
     * @param {TomSelect} tomInstance  The TomSelect instance to update.
     * @param {string}    barNum       The bar number returned by the clerk.
     */
    loadAndSetAttorney(tomInstance, barNum) {
        if (!tomInstance || !barNum) {
            if (tomInstance) tomInstance.clear();
            return;
        }
        // If the option is already loaded just select it
        if (tomInstance.options[barNum]) {
            tomInstance.setValue(barNum);
            return;
        }
        // Otherwise fetch by bar number, add the option, then select it
        $.ajax({
            url: `${this.service.baseUrl}AttorneyAPI/GetAttorneyDropDownItems`,
            type: 'GET',
            data: { q: barNum },
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const match = (response.data || []).find(a => a.bar_num === barNum);
                if (match) {
                    tomInstance.addOption({
                        value: match.bar_num,
                        text: match.label,
                        attorney_id: String(match.id)
                    });
                    tomInstance.setValue(match.bar_num);
                } else {
                    tomInstance.clear();
                    ShowNotification('Warning', `Attorney with bar number ${barNum} was not found in the system.`, 'warning');
                }
            },
            error: () => {
                tomInstance.clear();
                ShowNotification('Error', 'Failed to look up attorney by bar number.', 'error');
            }
        });
    }

    populateCourtMotions() {
        if (this.courtId === -1) return;
        return $.ajax({
            url: `${this.service.baseUrl}CourtMotionAPI/GetCourtMotionDropDownItems/${this.courtId}/true`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = document.getElementById('timeslot_restrictedMotions');
                if (select && response.data) {
                    const ts = new TomSelect(select, {
                        options: response.data.map(m => ({ value: m.Key, text: m.Value })),
                        items: [],
                        valueField: 'value',
                        labelField: 'text',
                        searchField: ['text'],
                        maxItems: null,
                        placeholder: 'Select motions to restrict...',
                        persist: false,
                        create: false,
                        plugins: {
                            remove_button: {
                                title: 'Remove this item'
                            }
                        },
                        onChange: () => this.populateMotionSelectExcludingRestricted()
                    });
                    select.setAttribute('tabindex', '-1');
                    select.setAttribute('autocomplete', 'off');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load motions to restrict.', 'error');
            }
        });
    }

    populateEventDefaults() {
        if (!this.courtData) return;
        const attorneyTom = $('#event_attorney')[0]?.tomselect;
        // Use bar_num from courtData — loadAndSetAttorney fetches the option if not yet loaded
        this.loadAndSetAttorney(attorneyTom, this.courtData.def_attorney_bar_num || null);
        const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
        this.loadAndSetAttorney(oppTom, this.courtData.opp_attorney_bar_num || null);

        $('#event_plaintiff').val(this.courtData.plaintiff || '');
        $('#event_defendant').val(this.courtData.defendant || '');

        $('#event_plaintiff').prop('required', this.courtData.plaintiff_required);
        $('#event_defendant').prop('required', this.courtData.defendant_required);
        $('#event_attorney').prop('required', this.courtData.plaintiff_attorney_required);
        $('#event_opposingAttorney').prop('required', this.courtData.defendant_attorney_required);

        const format = this.courtData.case_num_format;
        if (!format) return;
        const split = format.split('-');

        let typeSelectId;
        let typeVal;
        if (split.length === 3) {
            if (split[1].length === 2 || split[1] === '0') {
                typeSelectId = '#case_num_format_multiple2';
                typeVal = split[1];
            } else {
                $('#case_num_format_multiple1').val(split[0]);
                $('#case_num_format_multiple2').val(split[1]);
                $('#case_num_format_multiple3').val(split[2]);
            }
        } else if (split.length >= 4 && split.length <= 6) {
            typeSelectId = '#case_num_format_multiple3';
            typeVal = split[2];
            $('#case_num_format_multiple1').val(split[0]);
            if (split.length > 4) {
                $('#case_num_format_multiple5').val(split[4]);
            }
            if (split.length === 6) {
                $('#case_num_format_multiple6').val(split[5]);
            }
        }

        if (typeSelectId && typeVal) {
            const typeSelect = $(typeSelectId);
            if (typeSelect.length) {
                typeSelect.val(typeVal !== '0' ? typeVal : '');
            }
        }
        this.populateCourtTemplateFields();
    }

    populateCaseTypes() {
        return $.ajax({
            url: `${this.service.baseUrl}CourtTypeAPI/GetCourtTypeDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.data) {
                    courtCalendarControllerInstance.caseTypes = response.data;
                }
            },
            error: function (error) {
                console.error('Failed to fetch court types for dropdown');
                ShowNotification("Error", "Failed to load court types. Please try again later.", 'error');
            }
        });
    }

    populateCourtroomSelect() {
        // county_id is available once courtData is loaded; if not yet available fall back
        // to loading after fetchCourtData resolves (handled in init via $.when).
        const countyId = this.courtData?.county_id ?? 0;
        const url = countyId > 0
            ? `${this.service.baseUrl}CourtroomAPI/GetCourtroomDropDownItemsByCounty/${countyId}`
            : `${this.service.baseUrl}CourtroomAPI/GetCourtroomDropDownItems`;

        return $.ajax({
            url,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const selects = [
                    document.getElementById('timeslot_courtroom'),
                    document.getElementById('event_courtroom')
                ].filter(el => el);
                if (selects.length && response.data) {
                    selects.forEach(sel => {
                        sel.innerHTML = '<option value="">-</option>';
                        response.data.forEach(item => {
                            const option = document.createElement('option');
                            option.value = item.Key;
                            option.text = item.Value;
                            sel.appendChild(option);
                        });
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load courtrooms.', 'error');
            }
        });
    }

    populateEventTypeSelect() {
        return $.ajax({
            url: `${this.service.baseUrl}EventTypeAPI/GetEventTypeDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = document.getElementById('event_type');
                if (select && response.data) {
                    select.innerHTML = '<option value="">- Select Type -</option>';
                    response.data.forEach(item => {
                        const option = document.createElement('option');
                        option.value = item.Key;
                        option.text = item.Value;
                        select.appendChild(option);
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load event types.', 'error');
            }
        });
    }

    populateMotionSelectExcludingRestricted() {
        // Guard against the TomSelect not being ready yet — this function can
        // fire during timeslot load before populateCourtMotions finishes.
        // In that case we load all motions without the restricted-motions filter.
        const restrictedEl = $('#timeslot_restrictedMotions')[0];
        const restrictedTom = restrictedEl ? restrictedEl.tomselect : null;
        const restrictedIds = restrictedTom ? restrictedTom.getValue() : [];

        // Generation token: multiple callers can fire this in parallel
        // (tomSelect.addItem → onChange fires once per restricted motion, plus
        // our explicit calls). Whichever response lands last would otherwise
        // wipe the options via innerHTML, destroying any value we just set.
        // Only the most recent request is allowed to mutate the DOM.
        const myVersion = ++this._motionFetchVersion;

        return $.ajax({
            url: `${this.service.baseUrl}CourtMotionAPI/GetAvailableMotionDropDownItems/${this.courtId}?excludedIds=${restrictedIds.join(',')}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (myVersion !== this._motionFetchVersion) return; // stale response, newer one in flight
                const select = document.getElementById('event_motion');
                if (select && response.data) {
                    response.data.push({ Key: 221, Value: 'Other' });
                    select.innerHTML = '<option value="">- Select Motion -</option>';
                    response.data.forEach(item => {
                        const option = document.createElement('option');
                        option.value = item.Key;
                        option.text = item.Value;
                        select.appendChild(option);
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load motions.', 'error');
            }
        });
    }

    populateCourtTemplateFields() {
        const container = $('#court_template_fields');
        container.empty();
        if (this.courtData && this.courtData.user_defined_fields) {
            this.courtData.user_defined_fields.forEach((field, index) => {
                let key = `${field.field_name}_|${field.alignment}_|${field.field_type}`;
                let sanitizedId = key.replace(/[^A-Za-z0-9-]/g, '');
                let fieldHtml = '';
                let requiredAttr = '';
                let requiredLabel = '';
                if (field.field_type === 'yes_no') {
                    // The UDF admin enforces mutual exclusion between Required
                    // and "Yes Answer Required" — only one of them is set on a
                    // given field. Either flag means the user must pick *something*
                    // (asterisk + required attribute); the second flag additionally
                    // narrows the valid choice to "Yes".
                    const yesRequired = field.yes_answer_required == 1;
                    const anyRequired = yesRequired || field.required == 1;
                    if (anyRequired) {
                        requiredAttr = 'required';
                        requiredLabel = "<em>*</em>";
                    }
                    const errMsg = yesRequired
                        ? 'A Yes response is required for this field.'
                        : 'Please select Yes or No.';
                    fieldHtml = `
                        <div class="col-md-4 mb-3" data-udf-yes-required="${yesRequired}">
                            <label>${field.field_name}${requiredLabel}</label>
                            <div>
                                <label>
                                    <input type="radio" id="${sanitizedId}_yes" name="template[${key}]" value="yes" class="form-check-input" ${requiredAttr}>Yes
                                </label>
                                <label>
                                    <input type="radio" id="${sanitizedId}_no" name="template[${key}]" value="no" class="form-check-input" ${requiredAttr}>No
                                </label>
                            </div>
                            <small class="udf-required-msg text-danger" style="display:none;">${errMsg}</small>
                        </div>`;
                } else {
                    if (field.required == 1) {
                        requiredAttr = 'required';
                        requiredLabel = "<em>*</em>";
                    }
                    let type = field.field_type.toLowerCase() || 'text';
                    fieldHtml = `
                        <div class="col-md-4 mb-3">
                            <label for="${sanitizedId}">${field.field_name}${requiredLabel}</label>
                            <input type="${type}" class="form-control" name="template[${key}]" id="${sanitizedId}" ${requiredAttr}>
                        </div>`;
                }
                container.append(fieldHtml);
            });
        }
    }

    // Event Handlers
    bindEventHandlers() {
        $('#txtStartDate').datepicker({ autoclose: true, format: 'mm/dd/yyyy' });
        $('#btnExtend').on('click', this.handleAutoExtendCalendar.bind(this));
        $('#editCourtBtn').on('click', this.handleEditCourt.bind(this));
        $('#userDefinedFieldsBtn').on('click', this.handleUserDefinedFields.bind(this));
        $(document).on('change', '.case-num-part', (e) => this.changeLabel(e.target.value));
        $('#truncateBtn').on('click', this.handleTruncate.bind(this));
        $('#extendBtn').on('click', this.handleExtend.bind(this));
        $('#deleteTimeslotsBtn').on('click', this.handleDeleteTimeslots.bind(this));
        $('#copyTimeslotsBtn').on('click', this.handleCopyTimeslots.bind(this));
        $('#saveTimeslotPaneBtn').on('click', this.handleSaveTimeslot.bind(this));
        $('#deleteTimeslotPaneBtn').on('click', this.handleDeleteTimeslot.bind(this));
        $('#saveEventPaneBtn').on('click', this.handleSaveEvent.bind(this));
        // Two-way sync between the Timeslot and Event tabs' courtroom selects.
        // Guard with _syncingCourtroom to avoid change-event ping-pong.
        this._syncingCourtroom = false;
        $('#timeslot_courtroom').on('change', () => {
            if (this._syncingCourtroom) return;
            this._syncingCourtroom = true;
            $('#event_courtroom').val($('#timeslot_courtroom').val());
            this._syncingCourtroom = false;
        });
        $('#event_courtroom').on('change', () => {
            if (this._syncingCourtroom) return;
            this._syncingCourtroom = true;
            $('#timeslot_courtroom').val($('#event_courtroom').val());
            this._syncingCourtroom = false;
        });
        $('#cancelHearingBtn').on('click', this.handleCancelHearing.bind(this));
        $('#rescheduleBtn').on('click', this.handleReschedule.bind(this));
        $('#cattlecall_yes').on('change', () => $('.quantity-group').show());
        $('#cattlecall_no').on('change', () => $('.quantity-group').hide());
        $("#icalExportBtn").on('click', this.handleIcalExport.bind(this));
        $("#monthlyExportBtn").on('click', this.handleMonthlyExport.bind(this));
        $('#timeslot_block').on('change', (e) => {
            if (e.target.checked) {
                $('.block_reason').show();
                $('.public_block').show();
            } else {
                $('.block_reason').hide();
                $('.public_block').hide();
            }
        });
        $('#event_addon_check').on('change', () => {
            $('#event_addon').val($('#event_addon_check').is(':checked') ? '1' : '0');
        });
        $('#event_reminder_check').on('change', () => {
            $('#event_reminder').val($('#event_reminder_check').is(':checked') ? '1' : '0');
        });
        $('#event_motion').on('change', () => {
            $('#other_motion_row').toggle($('#event_motion').val() === '221');
        });
        $('#timeslot_duration').on('change', this.handleChangeTimeslotDuration.bind(this));
        // Tab change: update save button text and trigger motion load
        $('.nav-tabs a[data-toggle="tab"]').on('shown.bs.tab', (e) => {
            this.updateSaveButtonText();
        });
        $(document).on('shown.bs.tab', '#TimeslotModal .nav-tabs a', () => {
            this.updateSaveButtonText();
        });
        $('#timeslot_startTime').on('change', this.handleStartTimeChange.bind(this));
        $('#timeslot_endTime').on('change', this.handleEndTimeChange.bind(this));
        // NEW: Search Clerk button — only shown when adding a new event
        $('#searchClerkBtn').on('click', this.handleSearchClerk.bind(this));
        // Sequence field (multiple4): zero-pad to 6 digits on blur
        $(document).on('blur', '#case_num_format_multiple4', function () {
            const val = $(this).val().trim();
            if (val) $(this).val(val.padStart(6, '0'));
        });
        // Party/branch fields (multiple5, multiple6): uppercase as typed
        $(document).on('input', '#case_num_format_multiple5, #case_num_format_multiple6', function () {
            const el = this;
            const start = el.selectionStart;
            const end = el.selectionEnd;
            el.value = el.value.toUpperCase();
            el.setSelectionRange(start, end);
        });
    }

    /**
     * Updates the text of #saveEventPaneBtn to reflect the current context:
     * - On the Event tab: "Create Event" (new) or "Update Event" (existing)
     * - On any other tab: "Save Changes"
     */
    updateSaveButtonText() {
        const onEventTab = $('#eventTab').hasClass('active');
        if (onEventTab) {
            const isNew = !$('#edit_eventId').val();
            $('#saveEventPaneBtn').html(
                isNew
                    ? '<i class="fas fa-save"></i>Create Event'
                    : '<i class="fas fa-save"></i>Update Event'
            );
        } else {
            $('#saveEventPaneBtn').html('<i class="fas fa-save"></i>Save Changes');
        }
    }

    handleStartTimeChange() {
        const timeStr = $('#timeslot_startTime').val().trim();
        const dateStr = $('#t_start').val().trim();
        if (timeStr) {
            const newStart = this.parseTimeToDate(dateStr, timeStr);
            if (moment(newStart).isValid(newStart)) {
                $('#t_start').val(newStart);
                this.calculateTimeslotDetails();
                this.validateTimes();
            } else {
                new Noty({ type: 'error', text: 'Invalid start time format. Use e.g., 5:00 PM' }).show();
            }
        }
    }

    handleEndTimeChange() {
        const timeStr = $('#timeslot_endTime').val().trim();
        const dateStr = $('#t_end').val().trim();
        if (timeStr) {
            const newEnd = this.parseTimeToDate(dateStr, timeStr);
            if (moment(newEnd).isValid()) {
                $('#t_end').val(newEnd);
                this.calculateTimeslotDetails();
                this.validateTimes();
            } else {
                new Noty({ type: 'error', text: 'Invalid end time format. Use e.g., 5:00 PM' }).show();
            }
        }
    }

    handleEventResize(info) {
        let start_time = moment(info.event.start);
        let end_time = moment(info.event.end);
        let courtId = this.courtId;
        let timeslotId = info.event.id;
        const timeslotData = {
            id: timeslotId,
            start: start_time.format('YYYY-MM-DD HH:mm:ss'),
            end: end_time.format('YYYY-MM-DD HH:mm:ss'),
            courtId: courtId,
        };
        this.updateMoveTimeslot(timeslotData);
    }

    handleTimeslotModalShow() {
        $('#cancelHearingBtn').hide();
        $('#rescheduleBtn').hide();
        $('.public_block').hide();
        $('.block_reason').hide();
        $('.nav-tabs a').on('shown.bs.tab', (e) => {
            if (e.target.hash === '#eventTab') {
                // Only repopulate when the dropdown is empty — otherwise this
                // re-fires every time viewEvent programmatically activates the
                // tab and wipes the value viewEvent just set. Restricted-motion
                // changes already trigger populateMotion via the TomSelect
                // onChange, so we don't lose that refresh path.
                const sel = document.getElementById('event_motion');
                if (sel && sel.options.length <= 1) {
                    this.populateMotionSelectExcludingRestricted();
                }
                $('#other_motion_row').toggle($('#event_motion').val() === '221');
            }
        });
        const startTime = $('#t_start').val();
        if (startTime) {
            const startDate = new Date(startTime);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (startDate < today) {
                $('#saveEventPaneBtn').prop('disabled', true);
                ShowNotification('Warning', 'Events cannot be added to time slots before today\'s date.', 'warning');
                $('#cancelHearingBtn').hide();
                $('#rescheduleBtn').hide();
            } else {
                $('#saveEventPaneBtn').prop('disabled', false);
            }
        }
        if (this.pendingTab) {
            $('#timeslotTabs a[href="' + this.pendingTab + '"]').tab('show');
            this.pendingTab = null;
        }

        // Show Search Clerk button and lock clerk-populated fields only for new events.
        // For existing events (edit mode) all fields remain open and the button is hidden.
        const isNewEvent = !$('#edit_eventId').val();
        if (isNewEvent) {
            $('#searchClerkBtn').show();
            this._disableClerkFields(true);
        } else {
            $('#searchClerkBtn').hide();
            this._disableClerkFields(false);
        }
        this.updateSaveButtonText();
    }

    handleModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'TimeslotModal') {
            courtCalendarControllerInstance.clearTimeslotForm();
            courtCalendarControllerInstance.clearEventForm();
            $('#eventsTableBody').empty();
            $('.nav-tabs a[href="#timeslotTab"]').tab('show');
        }
    }

    handleCancelHearing(e) { }

    handleMonthlyExport(e) { }

    handleRescheduleClick(clickInfo) {
        const start = clickInfo.event.start;
        const end = clickInfo.event.end;
        const timeslotId = parseInt(clickInfo.event.id);
        const selectedDuration = clickInfo.event.extendedProps.duration ? clickInfo.event.extendedProps.duration : 0;
        if (selectedDuration !== this.currentEvent.duration) {
            Swal.fire('Invalid Selection', 'The selected duration must match the original hearing duration.', 'error');
            return;
        }
        Swal.fire({
            title: 'Reschedule Hearing?',
            text: `Reschedule to ${this.getDateRangeTitle(start, end)}?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.isConfirmed) {
                this.performReschedule(this.currentEvent.id, timeslotId);
            }
        });
    }

    handleCancelHearing(e) {
        e.preventDefault();
        const eventId = parseInt($('#edit_eventId').val());
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            input: 'textarea',
            inputLabel: 'Cancellation Reason',
            inputPlaceholder: 'Type your message here...',
            inputAttributes: { 'aria-label': 'Type your message here' },
            showCancelButton: true,
            confirmButtonText: 'Yes, cancel it!',
            cancelButtonText: 'Cancel',
            allowOutsideClick: false,
            allowEscapeKey: false,
            keydownListenerCapture: true,
            inputValidator: (value) => {
                if (!value) { return 'You need to provide a cancellation reason!' }
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}EventAPI/CancelEvent/${eventId}`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ cancellation_reason: result.value }),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: () => {
                        Swal.fire('Cancelled!', 'Your hearing has now been cancelled.', 'success').then(() => {
                            const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                            if (modal) modal.hide();
                        })
                    },
                    error: jqXHR => {
                        let response = {};
                        try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                        Swal.fire({ icon: 'error', title: 'Oops...', text: response.message })
                    },
                    complete: () => { this.calendar.refetchEvents(); }
                });
            }
        })
    }

    handleReschedule(e) {
        e.preventDefault();
        const eventId = this.currentEvent.id;
        if (!eventId) {
            ShowNotification('Error', 'No event selected for rescheduling.', 'error');
            return;
        }
        $('#TimeslotModal').modal('hide');
        $('#RescheduleHearingModal').modal('show');
        this.initRescheduleCalendar();
    }

    handleDateSelect(info) {
        const startTime = this.formatLocalTime(info.start);
        const endTime = this.formatLocalTime(info.end);
        const startDateTime = moment(info.start);
        const endDateTime = moment(info.end);
        this.clearTimeslotForm();
        $('#timeslot_startTime').val(startTime);
        $('#timeslot_endTime').val(endTime);
        $('#t_start').val(this.formatLocalDateTime(startDateTime));
        $('#t_end').val(this.formatLocalDateTime(endDateTime));
        $('#timeslot_allDay').val('false');
        $('.time-selection').show();
        $('.quantity-group').show();
        if (endDateTime.isValid() && startDateTime.isValid()) {
            const totalMinutes = endDateTime.diff(startDateTime, 'minutes');
            if (totalMinutes <= 5) {
                $('#timeslot_quantity').val('1');
                $('#cattlecall_no').prop('checked', true).trigger('change');
            } else {
                this.handleChangeTimeslotDuration();
                $('#cattlecall_yes').prop('checked', true).trigger('change');
            }
        }
        const title = this.getDateRangeTitle(new Date(startDateTime), new Date(endDateTime));
        $('#TimeslotModalLabel').text(title);
        const timeslotModal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
        timeslotModal.show();
        $('.nav-tabs li:not(:first)').hide();
        $('#timeslot_blockReason').closest('.row').hide();
        this.populateMotionSelectExcludingRestricted();
    }

    handleEventClick(info) {
        const checkbox = info.el.getElementsByClassName('calendar-select')[0];
        if (info.jsEvent.ctrlKey) {
            if (checkbox != null) {
                checkbox.checked = !checkbox.checked;
                if (checkbox.checked) {
                    multi_timeslots.push(info.event.id)
                } else {
                    const index = multi_timeslots.indexOf(info.event.id);
                    multi_timeslots.splice(index, 1);
                }
            }
        } else {
            this.viewTimeslot(parseInt(info.event.id));
        }
    }

    handleEventDrop(info) {
        let old_time = moment(info.oldEvent.start);
        let difference = moment(info.event.start).diff(old_time);
        let courtId = this.courtId;
        let initialId = info.event.id;
        if (dragEvents.length > 0) {
            dragEvents.forEach((element) => {
                var timeslotData = null;
                let event = this.calendar.getEventById(element);
                let timeslotId = null;
                if (initialId == event.id) {
                    timeslotId = parseInt(initialId);
                    timeslotData = {
                        id: timeslotId,
                        start: this.formatLocalDateTime(info.event.start),
                        end: this.formatLocalDateTime(info.event.end),
                        courtId: courtId,
                    };
                } else {
                    let newStart = moment(event.start).add(difference);
                    let newEnd = moment(event.end).add(difference);
                    timeslotId = parseInt(event.id);
                    if (newStart.day() > 5 || newEnd.day() > 5) { newStart = newStart.day(5); newEnd = newEnd.day(5); }
                    if (newStart.day() < 1 || newEnd.day() < 1) { newStart = newStart.day(1); newEnd = newEnd.day(1); }
                    if (newStart.hour() < 8) { newStart = newStart.hour(8).minute(0); }
                    if (newEnd.hour() > 17) { newEnd = newEnd.hour(17).minute(0); }
                    timeslotData = {
                        id: timeslotId,
                        start: newStart.format('YYYY-MM-DD HH:mm:ss'),
                        end: newEnd.format('YYYY-MM-DD HH:mm:ss'),
                        courtId: courtId,
                    };
                }
                this.updateMoveTimeslot(timeslotData);
            });
            dragEvents = [];
            multi_timeslots = [];
        } else {
            const timeslotId = parseInt(info.event.id);
            const timeslotData = {
                id: timeslotId,
                start: this.formatLocalDateTime(info.event.start),
                end: this.formatLocalDateTime(info.event.end),
                courtId: courtId,
            };
            this.updateMoveTimeslot(timeslotData);
        }
    }

    handleEditCourt(e) {
        e.preventDefault();
        if (this.courtId !== -1) {
            window.location.href = `${this.courtEditUrl}/cid/${this.courtId}`;
        } else {
            ShowNotification('Error', 'Court ID is not available.', 'error');
        }
    }

    handleUserDefinedFields(e) {
        e.preventDefault();
        if (this.courtId !== -1) {
            window.location.href = `${this.userDefinedFieldUrl}/cid/${this.courtId}`;
        } else {
            ShowNotification('Error', 'Court ID is not available.', 'error');
        }
    }

    handleTruncate(e) {
        e.preventDefault();
        if (this.courtId !== -1) {
            Swal.fire({
                title: 'Truncate Calendar?',
                text: 'Are you sure you wish to truncate the calendar?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = `${this.truncateCalendarUrl}/cid/${this.courtId}`;
                }
            });
        } else {
            ShowNotification('Error', 'Court ID is not available.', 'error');
        }
    }

    handleExtend(e) {
        e.preventDefault();
        if (this.courtId !== -1 && this.courtData != null) {
            Swal.fire({
                title: 'Extend Calendar?',
                text: 'Are you sure you wish to extend the calendar?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    if (this.courtData.auto_extension) {
                        const extendModal = new bootstrap.Modal(document.getElementById('ExtendCalendarModal'));
                        extendModal.show();
                    } else {
                        $.ajax({
                            url: `${this.service.baseUrl}CourtAPI/ExtendManual/${this.courtId}`,
                            type: 'GET',
                            dataType: 'json',
                            beforeSend: xhr => this.setAjaxHeaders(xhr),
                            success: response => {
                                if (response.status === 200) {
                                    ShowNotification('Successfully manually extended the calendar', response.message, 'success');
                                } else {
                                    ShowNotification('Error', response.message || 'Failed to extend the calendar manually.', 'error');
                                }
                            },
                            error: (error) => {
                                ShowNotification('Error', `Failed to extend the calendar manually. (${error.responseJSON.message})`, 'error');
                            },
                            complete: () => { this.calendar.refetchEvents(); }
                        });
                    }
                }
            });
        } else {
            ShowNotification('Error', 'Court ID is not available.', 'error');
        }
    }

    handleDeleteTimeslots(e) {
        e.preventDefault();
        Swal.fire({
            title: 'Delete Timeslots?',
            text: 'Are you sure you wish to delete selected timeslots?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}TimeslotAPI/DeleteMulti`,
                    method: 'DELETE',
                    data: JSON.stringify(multi_timeslots),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    success: function (data) {
                        ShowNotification('Success', `Timeslots Deleted Successfully.`, 'success');
                        multi_timeslots = [];
                        dragEvents = [];
                    },
                    error: function (error) {
                        ShowNotification('Error', `Failed to Delete Timeslots. (${error.responseJSON.message})`, 'error');
                    },
                    complete: () => { this.calendar.refetchEvents(); }
                });
            }
        });
    }

    handleDeleteTimeslot(e) {
        e.preventDefault();
        const timeslotId = parseInt($('#edit_timeslotId').val());
        if (!isNaN(timeslotId) && timeslotId > 0) {
            Swal.fire({
                title: 'Delete Timeslot?',
                text: 'Are you sure you wish to delete this timeslot?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: `${this.service.baseUrl}TimeslotAPI/DeleteTimeslot/${timeslotId}`,
                        type: 'DELETE',
                        data: JSON.stringify(timeslotId),
                        beforeSend: xhr => this.setAjaxHeaders(xhr),
                        success: () => {
                            const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                            if (modal) modal.hide();
                            ShowNotification('Success', 'Timeslot deleted successfully.', 'success');
                        },
                        error: jqXHR => {
                            let response = {};
                            try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                            ShowNotification('Error Deleting Timeslot', response.message || 'An unknown error occurred.', 'error');
                        },
                        complete: () => { this.calendar.refetchEvents(); }
                    });
                }
            });
        }
    }

    handleCopyTimeslots(e) {
        e.preventDefault();
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/CopyMulti`,
            method: 'POST',
            data: JSON.stringify(multi_timeslots),
            contentType: "application/json; charset=utf-8",
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            dataType: 'json',
            success: function (data) {
                ShowNotification('Success', `Timeslots Copied Successfully.`, 'success');
                multi_timeslots = [];
                dragEvents = [];
            },
            error: function (error) {
                ShowNotification('Error', `Failed to Copy Timeslots. (${error.responseJSON.message})`, 'error');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    handleSaveTimeslot(e) {
        e.preventDefault();
        if (this.validateTimeslotForm()) {
            const timeslotData = this.getTimeslotFormData();
            if (timeslotData.id <= 0) {
                this.createTimeslot(timeslotData);
            } else {
                this.updateTimeslot(timeslotData);
            }
        }
    }

    handleSaveEvent(e) {
        e.preventDefault();
        if (this.validateEventForm()) {
            const eventData = this.getEventFormData();
            const tsId = parseInt($('#edit_timeslotId').val());
            if (isNaN(tsId) || tsId <= 0) {
                // New timeslot + new event — createTimeslot picks up timeslot_courtroom
                // which is kept in sync with event_courtroom via the change handlers.
                const timeslotData = this.getTimeslotFormData();
                timeslotData.description = eventData.motion_id || 'Hearing';
                timeslotData.quantity = 1;
                timeslotData.duration = moment(timeslotData.end).diff(moment(timeslotData.start), 'minutes');
                this.createTimeslot(timeslotData, true);
            } else {
                eventData.timeslot_id = tsId;
                // Event_courtroom can differ from the timeslot's saved courtroom (the
                // selects are synced in-session but the timeslot hasn't been persisted
                // since the user touched event_courtroom).  Persist the timeslot first
                // so the courtroom update sticks, then save the event.
                this.saveTimeslotCourtroomThenEvent(tsId, eventData);
            }
        }
    }

    saveTimeslotCourtroomThenEvent(tsId, eventData) {
        const courtroomVal = $('#event_courtroom').val();
        const courtroomId = courtroomVal ? parseInt(courtroomVal) : null;
        const payload = { id: tsId, courtroom_id: courtroomId };
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateTimeslotCourtroom`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
        }).always(() => {
            if (eventData.id <= 0) {
                this.createEvent(eventData);
            } else {
                this.updateEvent(eventData);
            }
        });
    }

    handleAutoExtendCalendar(e) {
        $('#btnExtend').prop('disabled', true).find('i').removeClass('fas fa-save').addClass('fas fa-spinner fa-spin');
        e.preventDefault();
        var startTemplate = $('#ddlStartTemplate').val();
        var weeks = $('#txtWeeks').val();
        var startDate = $('#txtStartDate').val();
        if (!startTemplate || !weeks || !startDate) {
            Swal.fire({ icon: 'error', title: 'Validation Error', text: 'All fields are required.' });
            $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            return false;
        }
        if (weeks <= 0) {
            Swal.fire({ icon: 'error', title: 'Validation Error', text: 'Weeks to extend must be greater than 0.' });
            $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            return false;
        }
        const getUrl = `${this.service.baseUrl}CourtAPI/AutoExtend`;
        var formData = {
            CourtId: this.courtId,
            StartTemplateOrder: parseInt($('#ddlStartTemplate').val()),
            Weeks: parseInt($('#txtWeeks').val()),
            StartDate: $('#txtStartDate').val()
        };
        $.ajax({
            url: getUrl,
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.success) {
                    Swal.fire('Success', response.message, 'success').then(() => { window.location.href = `${this.calendarUrl}/cid/${this.courtId}`; });
                    const extendModal = new bootstrap.Modal(document.getElementById('ExtendCalendarModal'));
                    extendModal.hide();
                } else {
                    Swal.fire('Error', 'Extension failed', 'error');
                }
                $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            },
            error: error => {
                Swal.fire('Error', error.responseJSON.message, 'error');
                $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
        return false;
    }

    handleIcalExport(e) {
        e.preventDefault();
        const startDate = this.calendar.view.currentStart;
        const endDate = this.calendar.view.currentEnd;
        const fromDate = startDate.toISOString().split('T')[0];
        const toDate = endDate.toISOString().split('T')[0];
        const url = `/DesktopModules/tjc.Modules/JACS/Handlers/ExportCalendar.ashx?courtId=${this.courtId}&fromDate=${fromDate}&toDate=${toDate}`;
        window.location.href = url;
    }

    handleMonthlyExport(e) {
        e.preventDefault();
        const startDate = this.calendar.view.currentStart;
        const endDate = this.calendar.view.currentEnd;
        const fromDate = startDate.toISOString().split('T')[0];
        const toDate = endDate.toISOString().split('T')[0];
        const url = `/DesktopModules/tjc.Modules/JACS/Handlers/ExportHandler.ashx?courtId=${this.courtId}&fromDate=${fromDate}&toDate=${toDate}`;
        window.location.href = url;
    }

    // NEW: Clerk Search handler — delegates to searchCaseNumber()
    handleSearchClerk(e) {
        e.preventDefault();
        this.searchCaseNumber();
    }

    /**
     * Enable or disable the fields that should only be populated via a clerk
     * case lookup when adding a new event.
     * Disabled = user must click "Search Clerk" first.
     * Enabled  = clerk case selected, or editing an existing event.
     */
    _disableClerkFields(disabled) {
        $('#event_plaintiff').prop('disabled', disabled);
        $('#event_defendant').prop('disabled', disabled);
        $('#event_plaintiffEmail').prop('disabled', disabled);
        $('#event_defendantEmail').prop('disabled', disabled);

        const attyTom = $('#event_attorney')[0]?.tomselect;
        if (attyTom) { disabled ? attyTom.disable() : attyTom.enable(); }
        const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
        if (oppTom) { disabled ? oppTom.disable() : oppTom.enable(); }

        // Show/hide the "search first" notice
        if (disabled) {
            $('#clerk-fields-notice').show();
        } else {
            $('#clerk-fields-notice').hide();
        }
    }

    /**
     * Formats a raw case number into the canonical clerk format before sending
     * the search request. All spaces and hyphens are stripped first, then the
     * result is parsed into canonical segments:
     *
     *   Positions 0-1  : County code   (2 digits, zero-padded)
     *   Positions 2-5  : Year          (4 digits)
     *   Positions 6-7  : Case type     (2 alpha chars, uppercased)
     *   Positions 8-13 : Sequence      (6 digits, zero-padded)
     *   Positions 14+  : Optional tail (kept verbatim)
     *
     * Minimum output: 14 chars — "CCYYYYTTssssss"
     * Returns the stripped value unchanged when it is shorter than 14 chars
     * and cannot be reliably parsed.
     */
    formatCaseNumberForClerk(raw) {
        if (!raw) return '';
        // Strip all spaces and hyphens
        const stripped = raw.replace(/[\s\-]/g, '');
        if (stripped.length < 14) return stripped;

        const county = stripped.substring(0, 2).padStart(2, '0');
        const year = stripped.substring(2, 6).padStart(4, '0');
        const caseType = stripped.substring(6, 8).toUpperCase();
        const seq = stripped.substring(8, 14).padStart(6, '0');
        const tail = stripped.length > 14 ? stripped.substring(14) : '';

        return county + year + caseType + seq + tail;
    }

    // Timeslot Methods
    viewTimeslot(timeslotId) {
        const getUrl = `${this.service.baseUrl}TimeslotAPI/GetTimeslot/${timeslotId}`;
        $('#progress-timeslot').show();
        const mainReq = $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                this.currentTimeslot = response;
                if (response) {
                    $('#edit_timeslotId').val(response.id);
                    $('#timeslot_startTime').val(this.formatLocalTime(new Date(response.start)));
                    $('#timeslot_endTime').val(this.formatLocalTime(new Date(response.end)));
                    $('#t_start').val(response.start);
                    $('#t_end').val(response.end);
                    $('#timeslot_allDay').val('false');
                    $('.time-selection').show();
                    $('.quantity-group').show();
                    $('#timeslot_block').prop('checked', response.blocked);
                    $('#timeslot_publicBlock').prop('checked', response.publicBlock);
                    $('#timeslot_blockReason').val(response.blockReason);
                    $('#timeslot_duration').val(response.duration);
                    this.currentDuration = response.duration;
                    $('#timeslot_quantity').val(response.quantity);
                    $('#timeslot_description').val(response.description);
                    $('#timeslot_courtroom').val(response.courtroom);
                    $('#event_courtroom').val(response.courtroom);
                    $(`#cattlecall_${response.quantity > 1 ? 'yes' : 'no'}`).prop('checked', true);
                    $('.quantity-group').toggle(response.quantity >= 1);
                    // The restricted-motions TomSelect may not be initialised yet
                    // if populateCourtMotions hasn't resolved. Skip silently —
                    // the onInitComplete path will re-apply restrictedMotions
                    // once the TomSelect exists.
                    const tomSelect = $('#timeslot_restrictedMotions')[0]?.tomselect;
                    if (tomSelect) {
                        tomSelect.clear();
                        if (response.restrictedMotions && response.restrictedMotions.length > 0) {
                            response.restrictedMotions.forEach(id => tomSelect.addItem(id));
                        }
                    }
                    const title = this.getDateRangeTitle(new Date(response.start), new Date(response.end));
                    $('#TimeslotModalLabel').text(title);
                    this.loadEventsForTimeslot(timeslotId);
                    const $deleteBtn = $('#deleteTimeslotPaneBtn');
                    if (response.hasEvents) {
                        $deleteBtn.hide();
                        $deleteBtn.attr('title', 'Cannot delete timeslot with scheduled hearings');
                    } else {
                        $deleteBtn.show();
                        $deleteBtn.removeAttr('title');
                    }
                    const modal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
                    modal.show();
                    $('.nav-tabs li').show();
                    if (response.blocked) {
                        $('.block_reason').show();
                        $('.public_block').show();
                    } else {
                        $('.block_reason').hide();
                        $('.public_block').hide();
                    }
                    $('.cattle-call').hide();
                    $('.edited-by').hide();
                } else {
                    ShowNotification('Error', 'Failed to retrieve timeslot details.', 'error');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to retrieve timeslot details.', 'error');
                $('#progress-timeslot').hide();
            },
            complete: () => { $('#progress-timeslot').hide(); }
        });
        // Chain the motion-dropdown populate onto the returned promise so
        // callers (showTimeslotModal) can reliably wait for the #event_motion
        // options to exist before firing viewEvent.
        return mainReq.then(() => this.populateMotionSelectExcludingRestricted());
    }

    getTimeslotFormData() {
        const tsIdVal = $('#edit_timeslotId').val();
        const tsId = tsIdVal ? parseInt(tsIdVal) : 0;
        const durationVal = $('#timeslot_duration').val();
        const duration = durationVal ? parseInt(durationVal) : 0;
        const cattlecall = $('input[name="timeslot_cattlecall"]:checked').val();
        const quantityVal = $('#timeslot_quantity').val();
        const quantity = quantityVal ? parseInt(quantityVal) : 0;
        const courtroomVal = $('#timeslot_courtroom').val();
        const courtroom = courtroomVal ? parseInt(courtroomVal) : null;
        const restrictedTom = $('#timeslot_restrictedMotions')[0].tomselect;
        const restrictedMotions = restrictedTom ? restrictedTom.getValue().map(id => parseInt(id)).filter(id => !isNaN(id)) : [];
        return {
            id: tsId,
            start: $('#t_start').val(),
            end: $('#t_end').val(),
            allDay: false,
            blocked: $('#timeslot_block').is(':checked'),
            publicBlock: $('#timeslot_publicBlock').is(':checked'),
            blockReason: $('#timeslot_blockReason').val(),
            duration: duration,
            quantity: quantity,
            cattlecall: cattlecall === '1',
            description: $('#timeslot_description').val(),
            courtroom_id: courtroom,
            courtId: this.courtId,
            restrictedMotions: restrictedMotions
        };
    }

    createTimeslot(timeslotData, isForEvent = false) {
        return $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/CreateTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (typeof result === 'object' && result.id) {
                    if (isForEvent) {
                        const eventData = this.getEventFormData();
                        eventData.timeslot_id = result.id;
                        this.createEvent(eventData);
                    } else {
                        const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                        if (modal) modal.hide();
                        ShowNotification('Success', 'Timeslot created successfully.', 'success');
                    }
                } else {
                    ShowNotification('Error', 'Unexpected Error', 'error');
                }
            },
            error: jqXHR => {
                let response = {};
                try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                ShowNotification('Error Creating Timeslot', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    updateMoveTimeslot(timeslotData) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateMoveTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => { ShowNotification('Success', `Timeslots Moved Successfully.`, 'success'); },
            error: jqXHR => {
                let response = {};
                try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                ShowNotification('Error Updating Timeslots', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    updateTimeslot(timeslotData) {
        return $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (typeof result === 'object' && result.id) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Timeslot updated successfully.', 'success');
                } else {
                    ShowNotification('Error', 'Unexpected Error', 'error');
                }
            },
            error: jqXHR => {
                let response = {};
                try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                ShowNotification('Error Updating Timeslots', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    validateTimeslotForm() {
        const start = moment($('#t_start').val());
        const end = moment($('#t_end').val());
        const dayStart = start.clone().hour(8).minute(0);
        const dayEnd = end.clone().hour(17).minute(30);
        let isValid = true;
        const $startTime = $('#timeslot_startTime');
        const $startTimeError = $('.startTime-feedback');
        if (!$startTime.val()) {
            $startTime.addClass('is-invalid'); $startTimeError.show(); isValid = false;
        } else {
            if (!start.isSameOrAfter(dayStart) || !start.isSameOrBefore(dayEnd)) {
                $startTime.addClass('is-invalid'); $startTimeError.show();
                $startTimeError.html('Start time must be between 8:00 AM and 5:30 PM'); isValid = false;
            } else if (start.isSameOrAfter(end)) {
                $startTime.addClass('is-invalid'); $startTimeError.show();
                $startTimeError.html('Start time must be before the End time and must not be the same time as the end time'); isValid = false;
            } else {
                $startTime.removeClass('is-invalid'); $startTimeError.hide();
            }
        }
        const $endTime = $('#timeslot_endTime');
        const $endTimeError = $('.endTime-feedback');
        if (!$endTime.val()) {
            $endTime.addClass('is-invalid'); $endTimeError.show(); isValid = false;
        } else {
            if (!end.isSameOrAfter(start.clone().hour(8).minute(0)) || !end.isSameOrBefore(dayEnd)) {
                $endTime.addClass('is-invalid'); $endTimeError.show();
                $endTimeError.html('End time must be between 8:00 AM and 5:30 PM'); isValid = false;
            } else {
                $endTime.removeClass('is-invalid'); $endTimeError.hide();
            }
        }
        const $duration = $('#timeslot_duration');
        const $durationError = $('.duration-feedback');
        if ($duration.val() <= 0) {
            $duration.addClass('is-invalid'); $durationError.show(); isValid = false;
        } else {
            $duration.removeClass('is-invalid'); $durationError.hide();
        }
        const $quantity = $('#timeslot_quantity');
        const $quantityError = $('.quantity-feedback');
        if ($quantity.val() < 1 && $('.quantity-group').is(':visible')) {
            $quantity.addClass('is-invalid'); $quantityError.show(); isValid = false;
        } else {
            $quantity.removeClass('is-invalid'); $quantityError.hide();
        }
        return isValid;
    }

    clearTimeslotForm() {
        $('#edit_timeslotId').val('');
        $('#timeslot_startTime').val('');
        $('#timeslot_endTime').val('');
        $('#t_start').val('');
        $('#t_end').val('');
        $('#timeslot_block').prop('checked', false);
        $('#timeslot_publicBlock').prop('checked', false);
        $('#timeslot_blockReason').val('');
        $('#cattlecall_yes').prop('checked', true);
        $('.time-selection').show();
        $('.quantity-group').show();
        $('.cattle-call').show();
        $('.public_block').hide();
        $('.block_reason').hide();
        $('#timeslot_duration').val('15');
        $('#timeslot_quantity').val('1');
        $('#timeslot_description').val('');
        $('#timeslot_courtroom').val('');
        $('#event_courtroom').val('');
        const tomSelect = $('#timeslot_restrictedMotions')[0].tomselect;
        if (tomSelect) tomSelect.clear();
    }

    calculateTimeslotDetails() {
        const start = moment($('#t_start').val());
        const end = moment($('#t_end').val());
        if (!start.isValid() || !end.isValid() || start >= end) return;
        const diffMinutes = end.diff(start, 'minutes');
        const isConcurrent = $('#timeslot_cattlecall').val() === '1';
        if (isConcurrent) {
            $('#timeslot_duration').val(diffMinutes);
        } else {
            const duration = parseInt($('#timeslot_duration').val()) || 0;
            if (duration > 0) {
                const quantity = Math.floor(diffMinutes / duration);
                $('#timeslot_quantity').val(quantity);
            }
        }
    }

    handleChangeTimeslotDuration() {
        const toTime = moment($('#t_end').val());
        const fromTime = moment($('#t_start').val());
        if (toTime.isValid() && fromTime.isValid()) {
            const totalMinutes = toTime.diff(fromTime, 'minutes');
            const duration = parseInt($('#timeslot_duration').val());
            if (duration > 0 && totalMinutes >= 0) {
                $('#timeslot_quantity').val(Math.floor(totalMinutes / duration));
            } else {
                $('#timeslot_quantity').val('');
            }
        } else {
            $('#timeslot_quantity').val('');
        }
    }

    // Event Methods
    viewEvent(eventId) {
        $("#progress-timeslot").show();
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/GetEvent/${eventId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    const event = response.data;
                    this.currentEvent = event;
                    this.clearEventForm();

                    $('#edit_eventId').val(event.id);
                    // Motion options are populated from a court-scoped AJAX. Fire
                    // it here and defer the select-value assignment until it
                    // resolves — otherwise val() runs against an empty dropdown
                    // on first load and silently no-ops.
                    $.when(this.populateMotionSelectExcludingRestricted()).always(() => {
                        $('#event_motion').val(event.motion_id);
                        $('#event_type').val(event.type_id);
                        if (event.motion_id === 221) {
                            $('#event_customMotion').val(event.custom_motion || '');
                            $('#other_motion_row').show();
                        } else {
                            $('#other_motion_row').hide();
                        }
                    });

                    const attorneyTom = $('#event_attorney')[0].tomselect;
                    this.loadAndSetAttorney(attorneyTom, event.attorney_bar_num || null);

                    const oppTom = $('#event_opposingAttorney')[0].tomselect;
                    this.loadAndSetAttorney(oppTom, event.opp_attorney_bar_num || null);

                    $('#event_plaintiff').val(event.plaintiff || '');
                    $('#event_defendant').val(event.defendant || '');
                    $('#event_plaintiffEmail').val(event.plaintiff_email || '');
                    $('#event_defendantEmail').val(event.defendant_email || '');
                    $('#event_notes').val(event.notes || '');

                    $('#event_addon_check').prop('checked', !!event.addon);
                    $('#event_addon').val(event.addon || '0');
                    $('#event_reminder_check').prop('checked', !!event.reminder);
                    $('#event_reminder').val(event.reminder || '0');

                    const caseNum = event.case_num || '';
                    const parts = caseNum.split('-');
                    $('.case-num-part').each((index, el) => { $(el).val(parts[index] || ''); });

                    const template = event.template ? JSON.parse(event.template) : {};
                    $('#court_template_fields [name^="template["]').each(function () {
                        const el = $(this);
                        const key = el.attr('name').match(/template\[(.*?)\]/)[1];
                        const value = template[key] || '';
                        if (el.is(':radio') || el.is(':checkbox')) {
                            el.prop('checked', el.val() === value || (!!value && el.val() === '1'));
                        } else {
                            el.val(value);
                        }
                    });

                    if (event.updated_at) {
                        // Prefer owner_username (DNN username, portable across the
                        // public + internal portals). Fall back to updated_by_name
                        // for legacy rows that pre-date the column.
                        $('#event_editedBy').text(event.owner_username || event.updated_by_name || '');
                        $('#event_updatedAt').text(event.updated_at ? moment(event.updated_at).format('MM/DD/YYYY h:mm A') : '');
                        $('.edited-by').show();
                    }

                    if (this.editable) {
                        if (event.status_name && event.status_name.toLowerCase() === 'cancelled') {
                            $('#cancelHearingBtn').hide();
                            $('#rescheduleBtn').hide();
                        } else {
                            $('#cancelHearingBtn').show();
                            $('#rescheduleBtn').show();
                        }
                    }
                    // Flip the tab label to "Manage Event" now that we have an
                    // existing event loaded. clearEventForm resets it to
                    // "Create Event" when opening a fresh event. Use .text()
                    // because the jQuery object has no .innerText property.
                    $('#eventCreateTab').text('Manage Event');

                    // Editing an existing event — all fields enabled, search button hidden
                    this._disableClerkFields(false);
                    $('#searchClerkBtn').hide();
                    $('.cattle-call').hide();
                    $('.nav-tabs a[href="#eventTab"]').tab('show');
                    this.updateSaveButtonText();
                } else {
                    ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                }
            },
            error: () => {
                $("#progress-timeslot").hide();
                ShowNotification('Error', 'Failed to load event details.', 'error');
            },
            complete: () => { $("#progress-timeslot").hide(); }
        });
    }

    getEventFormData() {
        const evtIdVal = $('#edit_eventId').val();
        const clerkCaseIdVal = parseInt($('#edit_clerkCaseId').val()) || 0;
        const clerkEventIdVal = parseInt($('#edit_clerkEventId').val()) || 0;
        const evtId = evtIdVal ? parseInt(evtIdVal) : 0;
        const motionId = $('#event_motion').val();
        const typeId = $('#event_type').val();
        const courtIdVal = this.courtId;

        const attorneyTom = $('#event_attorney')[0].tomselect;
        const opposingAttorneyTom = $('#event_opposingAttorney')[0].tomselect;

        // Value is bar_num; read the internal DB id from the selected option's data attribute.
        // The onItemAdd handler stamps data-attorney-id on the <option> when an item is chosen.
        const getAttorneyId = (tomInstance, selectEl) => {
            const barNum = tomInstance ? tomInstance.getValue() : '';
            if (!barNum) return '';
            // Prefer the data attribute stamped by onItemAdd; fall back to scanning options map.
            const opt = selectEl.querySelector(`option[value="${CSS.escape(barNum)}"]`);
            if (opt && opt.dataset.attorneyId) return opt.dataset.attorneyId;
            const option = tomInstance.options[barNum];
            return option ? option.attorney_id : '';
        };

        const attorneySelectEl = document.getElementById('event_attorney');
        const oppSelectEl = document.getElementById('event_opposingAttorney');
        const attorney_id = getAttorneyId(attorneyTom, attorneySelectEl);
        const opp_attorney_id = getAttorneyId(opposingAttorneyTom, oppSelectEl);

        // Bar numbers for sending to the clerk (the TomSelect value field)
        const attorney_bar_num = attorneyTom ? attorneyTom.getValue() : '';
        const opp_attorney_bar_num = opposingAttorneyTom ? opposingAttorneyTom.getValue() : '';

        const caseNumParts = $('#event_caseNum_container .case-num-part').map(function () { return $(this).val(); }).get();
        // Remove trailing empty segments before joining so optional fields (multiple5/6)
        // don't produce trailing hyphens when left blank.
        while (caseNumParts.length && !caseNumParts[caseNumParts.length - 1].trim()) {
            caseNumParts.pop();
        }
        const caseNum = caseNumParts.join('-');
        return {
            id: evtId,
            court_id: courtIdVal,
            clerk_case_id: clerkCaseIdVal,
            clerk_event_id: clerkEventIdVal,
            case_num: caseNum,
            motion_id: motionId ? motionId : -1,
            type_id: typeId ? typeId : -1,
            custom_motion: $('#event_customMotion').val(),
            attorney_id: attorney_id,           // internal DB id — used when saving to our DB
            opp_attorney_id: opp_attorney_id,   // internal DB id — used when saving to our DB
            attorney_bar_num: attorney_bar_num,         // bar number — used when sending to clerk
            opp_attorney_bar_num: opp_attorney_bar_num, // bar number — used when sending to clerk
            plaintiff: $('#event_plaintiff').val(),
            defendant: $('#event_defendant').val(),
            plaintiff_email: $('#event_plaintiffEmail').val().replace(';', ','),
            defendant_email: $('#event_defendantEmail').val().replace(';', ','),
            notes: $('#event_notes').val(),
            addon: $('#event_addon_check').is(':checked') ? true : false,
            reminder: $('#event_reminder_check').is(':checked') ? true : false,
            owner_type: 'App\\Models\\User',
            owner_id: this.userId,
            template: this.fetchTemplateData(),
        };
    }

    createEvent(eventData) {
        return $.ajax({
            url: `${this.service.baseUrl}EventAPI/CreateEvent`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(eventData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (result.status === 200) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Event created successfully.', 'success');
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Creating Event',
                        text: result.message || `Unexpected error (status ${result.status}).`
                    });
                }
            },
            error: jqXHR => {
                let message = 'An unknown error occurred.';
                try {
                    const response = JSON.parse(jqXHR.responseText);
                    message = response.message || response.error || message;
                } catch { }
                Swal.fire({
                    icon: 'error',
                    title: 'Error Creating Event',
                    text: message
                });
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    updateEvent(eventData) {
        return $.ajax({
            url: `${this.service.baseUrl}EventAPI/UpdateEvent`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(eventData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (result.status === 200) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Event updated successfully.', 'success');
                } else {
                    ShowNotification('Error', `Unexpected Error: Status=${result}`, 'error');
                }
            },
            error: jqXHR => {
                let response = {};
                try { response = JSON.parse(jqXHR.responseText); } catch (e) { response.message = jqXHR.responseText || 'An unknown error occurred.'; }
                ShowNotification('Error Updating Event', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    performReschedule(eventId, timeslotId) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/RescheduleEvent`,
            type: 'POST',
            data: JSON.stringify({ event_id: eventId, timeslot_id: timeslotId }),
            contentType: 'application/json',
            beforeSend: this.setAjaxHeaders.bind(this),
            success: (response) => {
                if (response.status === 200) {
                    ShowNotification('Success', 'Hearing rescheduled successfully.', 'success');
                    $('#RescheduleHearingModal').modal('hide');
                    $('#TimeslotModal').modal('hide');
                } else {
                    ShowNotification('Error', response.message, 'error');
                }
            },
            error: () => ShowNotification('Error', 'Failed to reschedule hearing.', 'error'),
            complete: () => { this.calendar.refetchEvents(); }
        });
    }

    loadEventsForTimeslot(timeslotId) {
        const getUrl = `${this.service.baseUrl}EventAPI/GetEventListItemsForTimeslot/${timeslotId}`;
        return $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                $('#eventsTableBody').empty();
                if (response.data) {
                    $('#cancelHearingBtn').hide();
                    $('#rescheduleBtn').hide();
                    response.data.forEach(e => {
                        const row = `
                            <tr><td><a href="#" class="editEventBtn" data-id="${e.id}"><i class="fas fa-edit"></i></a></td>
                                <td>${e.case_num || ''}</td>
                                <td>${e.motion_name}</td>
                                <td>${e.attorney_name}</td>
                                <td>${e.plaintiff}</td>
                                <td>${e.opp_attorney_name}</td>
                                <td>${e.defendant}</td>
                                <td>${e.status_name}</td>
                            </tr>`;
                        $('#eventsTableBody').append(row);
                    });
                    $('.editEventBtn').on('click', (ev) => {
                        ev.preventDefault();
                        this.viewEvent(parseInt($(ev.target).closest('a').data('id')));
                        $('.nav-tabs a[href="#event"]').tab('show');
                    });
                }
            },
            error: () => { ShowNotification('Error', 'Failed to load events for timeslot.', 'error'); }
        });
    }

    validateEventForm() {
        let isValid = true;
        // Required fields per business rule: case number, event type, motion, courtroom.
        // Attorney, plaintiff, and defendant are no longer required.
        const $motion = $('#event_motion');
        if (!$motion.val()) { $motion.addClass('is-invalid'); isValid = false; } else { $motion.removeClass('is-invalid'); }
        const $type = $('#event_type');
        if (!$type.val()) { $type.addClass('is-invalid'); isValid = false; } else { $type.removeClass('is-invalid'); }
        const $courtroom = $('#event_courtroom');
        if (!$courtroom.val()) { $courtroom.addClass('is-invalid'); isValid = false; } else { $courtroom.removeClass('is-invalid'); }
        // Email format check is still enforced when a value is present.
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const $plaintiffEmail = $('#event_plaintiffEmail');
        if ($plaintiffEmail.val() && !emailRegex.test($plaintiffEmail.val())) { $plaintiffEmail.addClass('is-invalid'); isValid = false; } else { $plaintiffEmail.removeClass('is-invalid'); }
        const $defendantEmail = $('#event_defendantEmail');
        if ($defendantEmail.val() && !emailRegex.test($defendantEmail.val())) { $defendantEmail.addClass('is-invalid'); isValid = false; } else { $defendantEmail.removeClass('is-invalid'); }
        // Validate case number parts — only county, year, case type, and sequence are required;
        // party/defendant ID (multiple5) and branch/location (multiple6) are optional.
        let caseNumValid = true;
        const requiredCaseNumIds = [
            'case_num_format_multiple1',
            'case_num_format_multiple2',
            'case_num_format_multiple3',
            'case_num_format_multiple4'
        ];
        $('#event_caseNum_container .case-num-part').each(function () {
            const id = $(this).attr('id');
            const val = $(this).val().trim();
            const isRequired = !id || requiredCaseNumIds.includes(id);
            if (isRequired && !val) {
                $(this).addClass('is-invalid');
                caseNumValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (!caseNumValid) isValid = false;
        if ($('#event_motion').val() === '221' && !$('#event_customMotion').val().trim()) { $('#event_customMotion').addClass('is-invalid'); isValid = false; } else { $('#event_customMotion').removeClass('is-invalid'); }
        let templateValid = true;
        const seenRadioGroups = new Set();
        $('#court_template_fields [required]').each(function () {
            const $el = $(this);
            if ($el.is(':radio')) {
                // Yes/No UDF: validate the group ONCE (each radio is marked
                // required, but they share a name). $el.val() always returns the
                // input's value attribute regardless of :checked state, so the
                // old check accidentally passed every time.
                const name = $el.attr('name');
                if (seenRadioGroups.has(name)) return;
                seenRadioGroups.add(name);
                const $group = $(`[name="${name.replace(/"/g, '\\"')}"]`);
                const $wrapper = $group.closest('.col-md-4');
                // When the UDF is configured as "Yes Answer Required" we only
                // accept a Yes; otherwise any choice (Yes or No) is fine.
                const yesAnswerRequired = $wrapper.attr('data-udf-yes-required') === 'true';
                const passes = yesAnswerRequired
                    ? $group.filter('[value="yes"]').is(':checked')
                    : $group.is(':checked');
                $wrapper.toggleClass('udf-invalid', !passes);
                $wrapper.find('.udf-required-msg').toggle(!passes);
                if (!passes) templateValid = false;
            } else {
                const val = ($el.val() || '').trim();
                if (!val) { $el.addClass('is-invalid'); templateValid = false; }
                else { $el.removeClass('is-invalid'); }
            }
        });
        if (!templateValid) isValid = false;
        return isValid;
    }

    clearEventForm() {
        $('#edit_eventId').val('');
        $('#edit_clerkCaseId').val('');
        $('#edit_clerkEventId').val('');
        const fields = ['motion', 'type', 'attorney', 'opposingAttorney'];
        fields.forEach(field => {
            const tomSelect = $(`#event_${field}`)[0].tomselect;
            if (tomSelect) tomSelect.clear();
        });
        // Keep event_courtroom in sync with whatever is chosen on the Timeslot tab
        $('#event_courtroom').val($('#timeslot_courtroom').val() || '');
        $('#event_customMotion').val('');
        $('#event_plaintiff').val('');
        $('#event_defendant').val('');
        $('#event_plaintiffEmail').val('');
        $('#event_defendantEmail').val('');
        $('#event_notes').val('');
        $('#event_addon_check').prop('checked', false);
        $('#event_addon').val('0');
        $('#event_reminder_check').prop('checked', false);
        $('#event_reminder').val('0');
        $('#event_editedBy').val('');
        $('#event_updatedAt').text('');
        $('.edited-by').hide();
        $('#cancelHearingBtn').hide();
        $('#rescheduleBtn').hide();
        $('#eventCreateTab').text('Create Event');
        // For a new event: lock the clerk-populated fields until Search Clerk is used
        this._disableClerkFields(true);
        $('#searchClerkBtn').show();
        if (this.courtData) {
            this.populateEventDefaults();
        } else {
            ShowNotification('Error', 'Failed to load court data.', 'error');
        }
    }

    // Utility Methods
    validateTimes() {
        const start = moment($('#t_start').val());
        const end = moment($('#t_end').val());
        const dayStart = start.clone().hour(8).minute(0).second(0);
        const dayEnd = start.clone().hour(17).minute(30).second(0);
        let valid = true;
        if (start.isBefore(dayStart) || start.isAfter(dayEnd)) {
            new Noty({ type: 'error', text: 'Invalid Start Time: Must be after 8:00 AM and before 5:30 PM' }).show();
            valid = false;
        }
        if (end.isBefore(dayStart) || end.isAfter(dayEnd)) {
            new Noty({ type: 'error', text: 'Invalid End Time: Must be after 8:00 AM and before 5:30 PM' }).show();
            valid = false;
        }
        if (start.isSameOrAfter(end)) {
            new Noty({ type: 'error', text: 'Start time must be before the End time and must not be the same time as the end time' }).show();
            return;
        }
        return valid;
    }

    parseTimeToDate(baseDate, timeStr) {
        const dateStr = moment(baseDate).format('YYYY-MM-DD');
        return moment(`${dateStr} ${timeStr}`, 'YYYY-MM-DD h:mm A').format('YYYY-MM-DD HH:mm:ss');
    }

    changeLabel(courtType) {
        if (courtType == "GA") {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Ward";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Ward is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Ward Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Ward Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Petitioner";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Petitioner is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Petitioner Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Petitioner Email is Required";
        } else if (courtType == "DR") {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Petitioner";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Petitioner is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Petitioner Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Petitioner Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Respondent";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Respondent is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Respondent Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Respondent Email is Required";
        } else if (courtType == "MH") {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Petitioner";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Petitioner is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Petitioner Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Petitioner Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Patient";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Patient is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Patient Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Patient Email is Required";
        } else {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Plaintiff";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Plaintiff is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Plaintiff Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Plaintiff Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Defendant";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Defendant is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Defendant Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Defendant Email is Required";
        }
    }

    // Clerk case search — strips/formats the case number then calls the API
    searchCaseNumber() {
        // Only the four core parts (county, year, case type, sequence) must be filled.
        const requiredCaseNumIds = [
            'case_num_format_multiple1',
            'case_num_format_multiple2',
            'case_num_format_multiple3',
            'case_num_format_multiple4'
        ];
        const allRequiredFilled = requiredCaseNumIds.every(id => {
            const el = document.getElementById(id);
            return !el || el.value.trim() !== '';
        });
        if (!allRequiredFilled) {
            Swal.fire({
                icon: 'warning',
                title: 'Case Number Incomplete',
                text: 'Please enter the county, year, case type, and sequence before searching.'
            });
            return;
        }

        const rawCaseNum = $('#event_caseNum_container .case-num-part')
            .map(function () { return $(this).val(); }).get().join('-');
        const formattedCaseNum = this.formatCaseNumberForClerk(rawCaseNum);

        // Show progress indicator on the search button while the request is in flight
        const $btn = $('#searchClerkBtn');
        const originalHtml = $btn.html();
        $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Searching...');

        $.ajax({
            url: `${this.service.baseUrl}EventAPI/SearchCaseNumberDetails`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ caseNum: formattedCaseNum, courtId: this.courtId }),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (!response.data || response.data.length === 0) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'No Case Found',
                        text: `No matching case record was found for case number "${formattedCaseNum}". Please verify the case number and try again.`
                    });
                    return;
                }
                if (response.data.length === 1) {
                    this.populateEventFromClerkCase(response.data[0]);
                } else {
                    this.showCaseSelectionModal(response.data);
                }
            },
            error: jqXHR => {
                let msg = `No matching case record was found for case number "${formattedCaseNum}".`;
                try { msg = JSON.parse(jqXHR.responseText)?.error || msg; } catch { }
                Swal.fire({
                    icon: 'warning',
                    title: 'No Case Found',
                    text: msg
                });
            },
            complete: () => {
                // Restore button regardless of success or failure
                $btn.prop('disabled', false).html(originalHtml);
            }
        });
    }
    populateEventFromClerkCase(c) {
        // Enable all clerk-populated fields now that a case has been chosen
        this._disableClerkFields(false);

        // Populate the case number input fields from the clerk's case_number.
        // The clerk returns it as a raw string without hyphens (e.g. "412025CA000002AX").
        // We split it into segments matching the rendered inputs in order.
        if (c.case_number) {
            const parts = c.case_number.replace(/[-\s]/g, '');
            const segments = [
                parts.substring(0, 2),   // multiple1: county (2)
                parts.substring(2, 6),   // multiple2: year (4)
                parts.substring(6, 8),   // multiple3: case type (2)
                parts.substring(8, 14),  // multiple4: sequence (6)
                parts.substring(14, 18), // multiple5: party/defendant ID (optional, up to 4)
                parts.substring(18)      // multiple6: branch/location (optional)
            ];
            const inputs = $('#event_caseNum_container .case-num-part').toArray();
            inputs.forEach((el, i) => {
                if (segments[i] !== undefined) {
                    $(el).val(segments[i]);
                }
            });
        }

        $('#event_plaintiff').val(c.petitioner || '');
        $('#event_plaintiffEmail').val(c.petitioner_email || '');
        $('#event_defendant').val(c.respondent || '');
        $('#event_defendantEmail').val(c.respondent_email || '');
        $('#event_notes').val(c.notes || '');
        $('#edit_clerkCaseId').val(c.clerk_case_id || '');

        // Value field is bar_num, so we can set directly from the clerk's bar number.
        // loadAndSetAttorney handles the case where the option isn't loaded yet.
        const attyTom = $('#event_attorney')[0]?.tomselect;
        this.loadAndSetAttorney(attyTom, c.petitioner_atty_bar || null);
        const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
        this.loadAndSetAttorney(oppTom, c.respondent_atty_bar || null);
    }

    showCaseSelectionModal(cases) {
        const rows = cases.map((c, i) => `
        <tr style="cursor:pointer" data-idx="${i}">
            <td>${c.case_number || ''}</td>
            <td>${c.petitioner || ''}</td>
            <td>${c.respondent || ''}</td>
        </tr>`).join('');

        Swal.fire({
            title: 'Multiple Cases Found',
            html: `<p>Select the case to use:</p>
               <table class="table table-hover table-sm text-start">
                 <thead><tr><th>Case #</th><th>Petitioner</th><th>Respondent</th></tr></thead>
                 <tbody id="caseSelectBody">${rows}</tbody>
               </table>`,
            showConfirmButton: false,
            showCloseButton: true,
            didOpen: () => {
                document.querySelectorAll('#caseSelectBody tr').forEach(row => {
                    row.addEventListener('click', () => {
                        const idx = parseInt(row.dataset.idx);
                        Swal.close();
                        this.populateEventFromClerkCase(cases[idx]);
                    });
                });
            }
        });
    }

    formatLocalDateTime(date) { return moment(date).format('YYYY-MM-DD HH:mm:ss'); }
    formatLocalTime(date) { return moment(date).format('h:mm A'); }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }

    getCourtIdFromUrl() { return parseInt(getValueFromUrl('cid')) || -1; }

    getDateRangeTitle(startDate, endDate) {
        const day = startDate.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' });
        const startTime = startDate.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true }).toLowerCase();
        const endTime = endDate.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true }).toLowerCase();
        return `${day}, ${startTime} - ${endTime}`;
    }

    getTimeRange(startDate, endDate) {
        const startTime = startDate.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true }).toLowerCase();
        const endTime = endDate.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true }).toLowerCase();
        return `${startTime} - ${endTime}`;
    }
}