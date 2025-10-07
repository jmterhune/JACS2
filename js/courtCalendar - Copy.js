// Filename: Resources/js/courtcalendar.js
let courtCalendarControllerInstance = null;
let multi_timeslots = [];
let dragEvents = [];
class CourtCalendarController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.service = params.service || null;
        this.calendar = null;
        this.rescheduleCalendar = null;
        this.courtId = this.getCourtIdFromUrl();
        this.courtEditUrl = params.courtEditUrl || '/court-edit';
        this.userDefinedFieldUrl = params.userDefinedFieldUrl || '/user-fields';
        this.truncateCalendarUrl = params.truncateCalendarUrl || '/truncate-calendar';
        this.calendarUrl = params.calendarUrl || '/court-calendar';
        this.courtData = null;
        this.caseTypes = null;
        this.currentEvent = null;
        this.currentTimeslot = null; // Default duration in minutes
        this.calendarItem = params.calendarItem || null;
        courtCalendarControllerInstance = this;
    }
    // Initialization Methods
    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const promCourt = this.fetchCourtData();
        const promCategory = this.populateCategorySelect();
        const promEventType = this.populateEventTypeSelect();
        const promCaseTypes = this.populateCaseTypes();
        const promAttorney = this.populateAttorneySelects();
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

        $.when(promCourt, promCategory, promEventType, promCaseTypes, promAttorney).then(
            () => this.populateEventDefaults()).fail(() => console.error('One or more data fetches failed'));
    }

    initCalendar() {
        const calendarEl = document.getElementById('calendar');
        this.calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'timeGridWeek',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
            },
            selectable: true,
            selectMirror: true,
            editable: true,
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
                    //const minutesPerPixel = 15 / slotHeight;
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
                if (arg.view.type === "listMonth") {
                    return { html: titleSpan };
                } else {
                    if (arg.event.extendedProps.eventCount == 0) {
                        timeText = `<span>${timeText}${checkbox}</span>`
                    } else {
                        timeText = `<span>${timeText}</span>`
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
                // Remove all current event sources to prevent duplication
                this.calendar.getEventSources().forEach(source => source.remove());

                // Add the event source specific to the current view
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
                    case 'listWeek': // List view uses the same event source
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
        return $.ajax({
            url: `${this.service.baseUrl}AttorneyAPI/GetAttorneyDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const selects = ['event_attorney', 'event_opposingAttorney'];
                selects.forEach(id => {
                    const select = document.getElementById(id);
                    if (select && response.data) {
                        const ts = new TomSelect(select, {
                            options: response.data.map(a => ({ value: a.Key, text: a.Value })),
                            valueField: 'value',
                            labelField: 'text',
                            searchField: ['text'],
                            maxItems: 1,
                            placeholder: 'Type Bar Number',
                            persist: false,
                            create: false,
                            plugins: {
                                clear_button: {
                                    title: 'Clear'
                                }
                            }
                        });
                        select.setAttribute('tabindex', '-1');
                        select.setAttribute('autocomplete', 'off');
                    }
                });
            },
            error: () => {
                ShowNotification('Error', 'Failed to load attorneys.', 'error');
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
        if (attorneyTom && this.courtData.def_attorney_id) {
            attorneyTom.setValue(String(this.courtData.def_attorney_id));
        }
        const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
        if (oppTom && this.courtData.opp_attorney_id) {
            oppTom.setValue(String(this.courtData.opp_attorney_id));
        }

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

    populateCategorySelect() {
        return $.ajax({
            url: `${this.service.baseUrl}CategoryAPI/GetCategoryDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = document.getElementById('timeslot_category');
                if (select && response.data) {
                    select.innerHTML = '<option value="">-</option>';
                    response.data.forEach(item => {
                        const option = document.createElement('option');
                        option.value = item.Key;
                        option.text = item.Value;
                        select.appendChild(option);
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load categories.', 'error');
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
                    select.innerHTML = '';
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
        const restrictedTom = $('#timeslot_restrictedMotions')[0].tomselect;
        const restrictedIds = restrictedTom ? restrictedTom.getValue() : [];
        return $.ajax({
            url: `${this.service.baseUrl}CourtMotionAPI/GetAvailableMotionDropDownItems/${this.courtId}?excludedIds=${restrictedIds.join(',')}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = document.getElementById('event_motion');
                if (select && response.data) {
                    response.data.push({ Key: 221, Value: 'Other' });
                    select.innerHTML = '';
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
                    if (field.yes_answer_required == 1) {
                        requiredAttr = 'required';
                        requiredLabel = "<em>*</em>";
                    }
                    fieldHtml = `
                        <div class="col-md-4 mb-3">
                            <label>${field.field_name}${requiredLabel}</label>
                            <div>
                                <label>
                                    <input type="radio" id="${sanitizedId}_yes" name="template[${key}]" value="yes" class="form-check-input" ${requiredAttr}>Yes
                                </label>
                                <label>
                                    <input type="radio" id="${sanitizedId}_no" name="template[${key}]" value="no" class="form-check-input" ${requiredAttr}>No
                                </label>
                            </div>
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
 
    //Event Handlers
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
        $('#timeslot_startTime').on('change', this.handleStartTimeChange.bind(this));
        $('#timeslot_endTime').on('change', this.handleEndTimeChange.bind(this));
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

    handleTimeslotModalShow() {
        $('#cancelHearingBtn').hide();
        $('#rescheduleBtn').hide();
        $('.public_block').hide();
        $('.block_reason').hide();
        $('.nav-tabs a').on('shown.bs.tab', (e) => {
            if (e.target.hash === '#eventTab') {
                this.populateMotionSelectExcludingRestricted();
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
    }

    handleModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'TimeslotModal') {
            courtCalendarControllerInstance.clearTimeslotForm();
            courtCalendarControllerInstance.clearEventForm();
            $('#eventsTableBody').empty();
            $('.nav-tabs a[href="#timeslotTab"]').tab('show'); //show event tab
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
            inputAttributes: {
                'aria-label': 'Type your message here'
            },
            showCancelButton: true,
            confirmButtonText: 'Yes, cancel it!',
            cancelButtonText: 'Cancel',
            allowOutsideClick: false,
            allowEscapeKey: false,
            keydownListenerCapture: true,

            inputValidator: (value) => {
                if (!value) {
                    return 'You need to provide a cancellation reason!'
                }
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
                        Swal.fire(
                            'Cancelled!',
                            'Your hearing has now been cancelled.',
                            'success'
                        ).then(() => {
                            const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                            if (modal) modal.hide();
                        })
                    },
                    error: jqXHR => {
                        let response = {};
                        try {
                            response = JSON.parse(jqXHR.responseText);
                        } catch (e) {
                            response.message = jqXHR.responseText || 'An unknown error occurred.';
                        }
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: response.message,
                        })
                    },
                    complete: () => {
                        this.calendar.refetchEvents();
                    }
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
                $('#timeslot_cattlecall_no').prop('checked', true).trigger('change');
            } else {
                this.handleChangeTimeslotDuration();
                $('#timeslot_cattlecall_yes').prop('checked', true).trigger('change');
            }
        }
        const title = this.getDateRangeTitle(new Date(startTime), new Date(endTime));
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
                    // Validation to make sure timeslot doesn't fall off calendar
                    if (newStart.day() > 5 || newEnd.day() > 5) {
                        newStart = newStart.day(5);
                        newEnd = newEnd.day(5);
                    }
                    if (newStart.day() < 1 || newEnd.day() < 1) {
                        newStart = newStart.day(1);
                        newEnd = newEnd.day(1);
                    }
                    if (newStart.hour() < 8) {
                        newStart = newStart.hour(8).minute(0);
                    }
                    if (newEnd.hour() > 17) {
                        newEnd = newEnd.hour(17).minute(0);
                    }
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
                            complete: () => {
                                this.calendar.refetchEvents();
                            }
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
                    complete: () => {
                        this.calendar.refetchEvents();
                    }
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
                            try {
                                response = JSON.parse(jqXHR.responseText);
                            } catch (e) {
                                response.message = jqXHR.responseText || 'An unknown error occurred.';
                            }
                            ShowNotification('Error Deleting Timeslot', response.message || 'An unknown error occurred.', 'error');
                        },
                        complete: () => {
                            this.calendar.refetchEvents();
                        }
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
            complete: () => {
                this.calendar.refetchEvents();
            }
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
                const timeslotData = this.getTimeslotFormData();
                timeslotData.description = eventData.motion_id || 'Hearing';
                timeslotData.quantity = 1;
                timeslotData.duration = moment(timeslotData.end).diff(moment(timeslotData.start), 'minutes');
                this.createTimeslot(timeslotData, true);
            } else {
                eventData.timeslot_id = tsId;
                if (eventData.id <= 0) {
                    this.createEvent(eventData);
                } else {
                    this.updateEvent(eventData);
                }
            }
        }
    }

    handleAutoExtendCalendar(e) {
        $('#btnExtend').prop('disabled', true).find('i').removeClass('fas fa-save').addClass('fas fa-spinner fa-spin');
        e.preventDefault();
        var startTemplate = $('#ddlStartTemplate').val();
        var weeks = $('#txtWeeks').val();
        var startDate = $('#txtStartDate').val();
        if (!startTemplate || !weeks || !startDate) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'All fields are required.'
            });
            $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            return false;
        }

        if (weeks <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Weeks to extend must be greater than 0.'
            });
            $('#btnExtend').prop('disabled', false).find('i').removeClass('fas fa-spinner fa-spin').addClass('fas save');
            return false;
        }

        const getUrl = `${this.service.baseUrl}CourtAPI/AutoExtend`;
        var formData = {
            CourtId: this.courtId,
            StartTemplateId: parseInt($('#ddlStartTemplate').val()),
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
            complete: () => {
                this.calendar.refetchEvents();
            }
        });
        return false; // Prevent default form submission
    }

    handleIcalExport(e) {
        e.preventDefault();
        // Get the current view's start and end dates from the calendar
        const startDate = this.calendar.view.currentStart;
        const endDate = this.calendar.view.currentEnd;

        // Format dates as yyyy-MM-dd
        const fromDate = startDate.toISOString().split('T')[0];
        const toDate = endDate.toISOString().split('T')[0];

        // Construct the handler URL with parameters
        const url = `/DesktopModules/tjc.Modules/JACS/Handlers/ExportCalendar.ashx?courtId=${this.courtId}&fromDate=${fromDate}&toDate=${toDate}`;

        // Trigger the download by setting window location
        window.location.href = url;
    }

    handleMonthlyExport(e) {
        e.preventDefault();
        // Get the current view's start and end dates from the calendar
        const startDate = this.calendar.view.currentStart;
        const endDate = this.calendar.view.currentEnd;

        // Format dates as yyyy-MM-dd
        const fromDate = startDate.toISOString().split('T')[0];
        const toDate = endDate.toISOString().split('T')[0];

        // Construct the handler URL with parameters
        const url = `/DesktopModules/tjc.Modules/JACS/Handlers/ExportHandler.ashx?courtId=${this.courtId}&fromDate=${fromDate}&toDate=${toDate}`;

        // Trigger the download by setting window location
        window.location.href = url;

    }

    //Timeslot Methods
    viewTimeslot(timeslotId) {
        const getUrl = `${this.service.baseUrl}TimeslotAPI/GetTimeslot/${timeslotId}`;
        $('#progress-timeslot').show();
        return $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                this.currentTimeslot = response;
                if (response) {
                    $('#edit_timeslotId').val(response.id);
                    $('#timeslot_startTime').val(this.formatLocalDateTime(new Date(response.start)));
                    $('#timeslot_endTime').val(this.formatLocalDateTime(new Date(response.end)));
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
                    $('#timeslot_category').val(response.category);
                    $(`#cattlecall_${response.quantity > 1 ? 'yes' : 'no'}`).prop('checked', true);
                    $('.quantity-group').toggle(response.quantity > 1);
                    const tomSelect = $('#timeslot_restrictedMotions')[0].tomselect;
                    tomSelect.clear();
                    if (response.restrictedMotions && response.restrictedMotions.length > 0) {
                        response.restrictedMotions.forEach(id => tomSelect.addItem(id));
                    }

                    const title = this.getDateRangeTitle(new Date(response.start), new Date(response.end));
                    $('#TimeslotModalLabel').text(title);
                    this.loadEventsForTimeslot(timeslotId);
                    this.populateMotionSelectExcludingRestricted();

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
                    // $('.nav-tabs a[href="#eventTab"]').tab('show');
                    $('.cattle-call').hide();
                    $('.edited-by').hide();
                } else {
                    ShowNotification('Error', 'Failed to retrieve timeslot details.', 'error');
                }
                $('#progress-timeslot').hide();
            },
            error: () => {
                ShowNotification('Error', 'Failed to retrieve timeslot details.', 'error');
                $('#progress-timeslot').hide();
            }
        });
    }

    getTimeslotFormData() {
        const tsIdVal = $('#edit_timeslotId').val();
        const tsId = tsIdVal ? parseInt(tsIdVal) : 0;
        const durationVal = $('#timeslot_duration').val();
        const duration = durationVal ? parseInt(durationVal) : 0;
        const cattlecall = $('input[name="timeslot_cattlecall"]:checked').val();
        const quantityVal = $('#timeslot_quantity').val();
        const quantity = quantityVal ? parseInt(quantityVal) : 0;
        const categoryVal = $('#timeslot_category').val();
        const category = categoryVal ? parseInt(categoryVal) : null;
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
            category_id: category,
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
                try {
                    response = JSON.parse(jqXHR.responseText);
                } catch (e) {
                    response.message = jqXHR.responseText || 'An unknown error occurred.';
                }
                ShowNotification('Error Creating Timeslot', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => {
                this.calendar.refetchEvents();
            }
        });
    }

    updateMoveTimeslot(timeslotData) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateMoveTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                ShowNotification('Success', `Timeslots Moved Successfully.`, 'success');

            },
            error: jqXHR => {
                let response = {};
                try {
                    response = JSON.parse(jqXHR.responseText);
                } catch (e) {
                    response.message = jqXHR.responseText || 'An unknown error occurred.';
                }
                ShowNotification('Error Updating Timeslots', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => {
                this.calendar.refetchEvents();
            }
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
                try {
                    response = JSON.parse(jqXHR.responseText);
                } catch (e) {
                    response.message = jqXHR.responseText || 'An unknown error occurred.';
                }
                ShowNotification('Error Updating Timeslots', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => {
                this.calendar.refetchEvents();
            }
        });
    }

    validateTimeslotForm() {
        const start = moment($('#t_start').val());
        const end = moment($('#t_end').val());
        const dayStart = start.clone().hour(8).minute(0); // After 6:59 AM
        const dayEnd = end.clone().hour(17).minute(30); // Before 5:30 PM
        let isValid = true;
        const $startTime = $('#timeslot_startTime');
        const $startTimeError = $('.startTime-feedback');
        if (!$startTime.val()) {
            $startTime.addClass('is-invalid');
            $startTimeError.show();
            isValid = false;
        } else {
            if (!start.isSameOrAfter(dayStart) || !start.isSameOrBefore(dayEnd)) {
                $startTime.addClass('is-invalid');
                $startTimeError.show();
                $startTimeError.html('Start time must be between 8:00 AM and 5:30 PM');
                isValid = false;
            } else if (start.isSameOrAfter(end)) {
                $startTime.addClass('is-invalid');
                $startTimeError.show();
                $startTimeError.html('Start time must be before the End time and must not be the same time as the end time');
                isValid = false;
            } else {
                $startTime.removeClass('is-invalid');
                $startTimeError.hide();
            }
        }
        const $endTime = $('#timeslot_endTime');
        const $endTimeError = $('.endTime-feedback');
        if (!$endTime.val()) {
            $endTime.addClass('is-invalid');
            $endTimeError.show();
            isValid = false;
        } else {
            if (!end.isSameOrAfter(start.clone().hour(8).minute(0)) || !end.isSameOrBefore(dayEnd)) {
                $endTime.addClass('is-invalid');
                $endTimeError.show();
                $endTimeError.html('End time must be between 8:00 AM and 5:30 PM');
                isValid = false;
            } else {
                $endTime.removeClass('is-invalid');
                $endTimeError.hide();
            }
        }

        const $duration = $('#timeslot_duration');
        const $durationError = $('.duration-feedback');
        if ($duration.val() <= 0) {
            $duration.addClass('is-invalid');
            $durationError.show();
            isValid = false;
        } else {
            $duration.removeClass('is-invalid');
            $durationError.hide();
        }
        const $quantity = $('#timeslot_quantity');
        const $quantityError = $('.quantity-feedback');
        if ($quantity.val() < 1 && $('.quantity-group').is(':visible')) {
            $quantity.addClass('is-invalid');
            $quantityError.show();
            isValid = false;
        } else {
            $quantity.removeClass('is-invalid');
            $quantityError.hide();
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
        $('#timeslot_category').val('');
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

    //Event Methods
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
                    this.clearEventForm(); // Clear form first to reset defaults

                    $('#edit_eventId').val(event.id);
                    $('#event_motion').val(event.motion_id);
                    $('#event_type').val(event.type_id);
                    if (event.motion_id === 221) {
                        $('#event_customMotion').val(event.custom_motion || '');
                        $('#other_motion_row').show();
                    } else {
                        $('#other_motion_row').hide();
                    }

                    // Attorney
                    const attorneyTom = $('#event_attorney')[0].tomselect;
                    attorneyTom.setValue(event.attorney_id ? event.attorney_id.toString() : '');

                    // Opposing Attorney
                    const oppTom = $('#event_opposingAttorney')[0].tomselect;
                    oppTom.setValue(event.opp_attorney_id ? event.opp_attorney_id.toString() : '');

                    // Plaintiff and Defendant
                    $('#event_plaintiff').val(event.plaintiff || '');
                    $('#event_defendant').val(event.defendant || '');
                    $('#event_plaintiffEmail').val(event.plaintiff_email || '');
                    $('#event_defendantEmail').val(event.defendant_email || '');

                    // Notes
                    $('#event_notes').val(event.notes || '');

                    // Addon
                    $('#event_addon_check').prop('checked', !!event.addon);
                    $('#event_addon').val(event.addon || '0');

                    // Reminder
                    $('#event_reminder_check').prop('checked', !!event.reminder);
                    $('#event_reminder').val(event.reminder || '0');

                    // Case Number Parts
                    const caseNum = event.case_num || '';
                    const parts = caseNum.split('-');
                    $('.case-num-part').each((index, el) => {
                        $(el).val(parts[index] || '');
                    });

                    // Template Fields
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

                    // Edited By
                    if (event.updated_at) {
                        $('#event_editedBy').text(event.updated_by_name);
                        $('#event_updatedAt').text(event.updated_at ? moment(event.updated_at).format('MM/DD/YYYY h:mm A') : '');
                        $('.edited-by').show();
                    }

                    // Show buttons
                    $('#cancelHearingBtn').show();
                    $('#rescheduleBtn').show();
                    $('.cattle-call').hide();
                    $('.nav-tabs a[href="#eventTab"]').tab('show'); //show event tab
                    $("#progress-timeslot").hide();
                } else {
                    $("#progress-timeslot").hide();
                    ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                }
            },
            error: () => {
                $("#progress-timeslot").hide();
                ShowNotification('Error', 'Failed to load event details.', 'error');
            }
        });
    }

    getEventFormData() {
        const evtIdVal = $('#edit_eventId').val();
        const evtId = evtIdVal ? parseInt(evtIdVal) : 0;
        const motionId = $('#event_motion').val();
        const typeId = $('#event_type').val();
        const attorneyTom = $('#event_attorney')[0].tomselect;
        const opposingAttorneyTom = $('#event_opposingAttorney')[0].tomselect;
        const caseNumParts = $('#event_caseNum_container .case-num-part').map(function () { return $(this).val(); }).get();
        const caseNum = caseNumParts.join('-');
        return {
            id: evtId,
            case_num: caseNum,
            motion_id: motionId ? motionId : -1,
            type_id: typeId ? typeId : -1,
            custom_motion: $('#event_customMotion').val(),
            attorney_id: attorneyTom ? attorneyTom.getValue() : '',
            opp_attorney_id: opposingAttorneyTom ? opposingAttorneyTom.getValue() : '',
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
                    ShowNotification('Error', `Unexpected Error: Status=${result}`, 'error');
                }
            },
            error: jqXHR => {
                let response = {};
                try {
                    response = JSON.parse(jqXHR.responseText);
                } catch (e) {
                    response.message = jqXHR.responseText || 'An unknown error occurred.';
                }
                ShowNotification('Error Creating Event', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => {
                this.calendar.refetchEvents();
            }
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
                try {
                    response = JSON.parse(jqXHR.responseText);
                } catch (e) {
                    response.message = jqXHR.responseText || 'An unknown error occurred.';
                }
                ShowNotification('Error Updating Event', response.message || 'An unknown error occurred.', 'error');
            },
            complete: () => {
                this.calendar.refetchEvents();
            }
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
            complete: () => {
                this.calendar.refetchEvents();
            }
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
            error: () => {
                ShowNotification('Error', 'Failed to load events for timeslot.', 'error');
            }
        });
    }

    validateEventForm() {
        let isValid = true;
        const $motion = $('#event_motion');
        if (!$motion.val()) {
            $motion.addClass('is-invalid');
            isValid = false;
        } else {
            $motion.removeClass('is-invalid');
        }
        const $type = $('#event_type');
        if (!$type.val()) {
            $type.addClass('is-invalid');
            isValid = false;
        } else {
            $type.removeClass('is-invalid');
        }
        const attorneyTom = $('#event_attorney')[0].tomselect;
        if (!attorneyTom.getValue()) {
            $('#event_attorney').addClass('is-invalid');
            isValid = false;
        } else {
            $('#event_attorney').removeClass('is-invalid');
        }
        const $plaintiff = $('#event_plaintiff');
        if (!$plaintiff.val().trim()) {
            $plaintiff.addClass('is-invalid');
            isValid = false;
        } else {
            $plaintiff.removeClass('is-invalid');
        }
        const $defendant = $('#event_defendant');
        if (!$defendant.val().trim()) {
            $defendant.addClass('is-invalid');
            isValid = false;
        } else {
            $defendant.removeClass('is-invalid');
        }
        const $plaintiffEmail = $('#event_plaintiffEmail');
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if ($plaintiffEmail.val() && !emailRegex.test($plaintiffEmail.val())) {
            $plaintiffEmail.addClass('is-invalid');
            isValid = false;
        } else {
            $plaintiffEmail.removeClass('is-invalid');
        }
        const $defendantEmail = $('#event_defendantEmail');
        if ($defendantEmail.val() && !emailRegex.test($defendantEmail.val())) {
            $defendantEmail.addClass('is-invalid');
            isValid = false;
        } else {
            $defendantEmail.removeClass('is-invalid');
        }
        // Validate case number parts
        let caseNumValid = true;
        $('#event_caseNum_container .case-num-part').each(function () {
            const val = $(this).val().trim();
            if (!val) {
                $(this).addClass('is-invalid');
                caseNumValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (!caseNumValid) isValid = false;
        // Validate other motion
        if ($('#event_motion').val() === '221' && !$('#event_customMotion').val().trim()) {
            $('#event_customMotion').addClass('is-invalid');
            isValid = false;
        } else {
            $('#event_customMotion').removeClass('is-invalid');
        }
        // Validate court template fields
        let templateValid = true;
        $('#court_template_fields [required]').each(function () {
            const val = $(this).val().trim();
            if (!val) {
                $(this).addClass('is-invalid');
                templateValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (!templateValid) isValid = false;
        return isValid;
    }

    clearEventForm() {
        $('#edit_eventId').val('');
        const fields = ['motion', 'type', 'attorney', 'opposingAttorney'];
        fields.forEach(field => {
            const tomSelect = $(`#event_${field}`)[0].tomselect;
            if (tomSelect) tomSelect.clear();
        });
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
        // Repopulate case number fields with initial values from courtData
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
        }
        else if (courtType == "DR") {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Petitioner";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Petitioner is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Petitioner Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Petitioner Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Respondent";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Respondent is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Respondent Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Respondent Email is Required";
        }
        else if (courtType == "MH") {
            document.getElementsByClassName("plaintiff-label")[0].innerHTML = "Petitioner";
            document.getElementsByClassName("plaintiff-feedback")[0].innerHTML = "Petitioner is Required";
            document.getElementsByClassName("plaintiff-email-label")[0].innerHTML = "Petitioner Email";
            document.getElementsByClassName("plaintiff-email-feedback")[0].innerHTML = "Petitioner Email is Required";
            document.getElementsByClassName("defendant-label")[0].innerHTML = "Patient";
            document.getElementsByClassName("defendant-feedback")[0].innerHTML = "Patient is Required";
            document.getElementsByClassName("defendant-email-label")[0].innerHTML = "Patient Email";
            document.getElementsByClassName("defendant-email-feedback")[0].innerHTML = "Patient Email is Required";
        }
        else {
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

    evaluateCaseNumberFields() {
        const caseNumParts = $('#event_caseNum_container .case-num-part').map(function () { return $(this).val(); }).get();
        const caseNum = caseNumParts.join('-');
        if (caseNumParts.every(part => part.trim() !== '')) {
            $.ajax({
                url: `${this.service.baseUrl}EventAPI/SearchCaseNumberDetails`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ searchTerm: caseNum }),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: response => {
                    if (response.data) {
                        const evt = response.data;
                        $('#event_motion').val(evt.motion_id || '');
                        $('#event_type').val(evt.type_id || '');
                        $('#event_customMotion').val(evt.custom_motion || '');
                        const evtAttorney = $('#event_attorney')[0].tomselect;
                        if (evtAttorney && evt.attorney_id) evtAttorney.setValue(evt.attorney_id);
                        const evtOppAttorney = $('#event_opposingAttorney')[0].tomselect;
                        if (evtOppAttorney && evt.opp_attorney_id) evtOppAttorney.setValue(evt.opp_attorney_id);
                        $('#event_plaintiff').val(evt.plaintiff || '');
                        $('#event_defendant').val(evt.defendant || '');
                        $('#event_plaintiffEmail').val(evt.plaintiff_email || '');
                        $('#event_defendantEmail').val(evt.defendant_email || '');
                        $('#event_notes').val(evt.notes || '');
                        $('#event_addon_check').prop('checked', evt.addon === '1');
                        $('#event_addon').val(evt.addon || '0');
                        $('#event_reminder_check').prop('checked', evt.reminder === '1');
                        $('#event_reminder').val(evt.reminder || '0');
                        if (evt.template) {
                            const templateData = JSON.parse(evt.template);
                            Object.keys(templateData).forEach(key => {
                                const field = $(`#${key.replace(/[^A-Za-z0-9-]/g, '')}`);
                                if (field.is(':radio')) {
                                    $(`input[name="template[${key}]"][value="${templateData[key]}"]`).prop('checked', true);
                                } else {
                                    field.val(templateData[key]);
                                }
                            });
                        }
                        $('#other_motion_row').toggle(evt.motion_id === 221);
                    }
                },
                error: () => {
                    ShowNotification('Case Events', 'No events for selected case number.', 'alert');
                }
            });
        }
    }

    formatLocalDateTime(date) {
        return moment(date).format('YYYY-MM-DD HH:mm:ss');
    }

    formatLocalTime(date) {
        return moment(date).format('h:mm A');
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }

    getCourtIdFromUrl() {
        return parseInt(getValueFromUrl('cid')) || -1;
    }

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