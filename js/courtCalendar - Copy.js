let courtCalendarControllerInstance = null;

class CourtCalendarController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.service = params.service || null;
        this.calendar = null;
        this.courtId = this.getCourtIdFromUrl();
        this.courtEditUrl = params.courtEditUrl || '/Court/Edit';
        this.userDefinedFieldUrl = params.userDefinedFieldUrl || '/Court/CustomFields';
        this.truncateCalendarUrl = params.truncateCalendarUrl || '/Court/Truncate';
        this.extendCalendarUrl = params.extendCalendarUrl || '/Court/Extend';
        this.courtData = null;
        courtCalendarControllerInstance = this;
    }

    getCourtIdFromUrl() {
        return parseInt(getValueFromUrl('cid')) || -1;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.fetchCourtData();
        this.initCalendar();
        this.bindEventHandlers();
        this.initTomSelect();
        this.populateCategorySelect();
        this.populateEventTypeSelect();
        const timeslotModalElement = document.getElementById('TimeslotModal');
        if (timeslotModalElement) {
            timeslotModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
            timeslotModalElement.addEventListener('shown.bs.modal', this.onTimeslotModalShow.bind(this));
        }
    }

    fetchCourtData() {
        $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    this.courtData = response.data;
                    this.populateAttorneySelects();
                    this.populateCaseNumFields();
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load court data.', 'error');
            }
        });
    }

    initTomSelect() {
        if (this.courtId === -1) return;
        $.ajax({
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

    populateCategorySelect() {
        $.ajax({
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
        $.ajax({
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
        $.ajax({
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

    populateAttorneySelects() {
        $.ajax({
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
                        if (this.courtData) {
                            if (id === 'event_attorney' && this.courtData.def_attorney_id) {
                                ts.setValue(this.courtData.def_attorney_id.toString());
                            } else if (id === 'event_opposingAttorney' && this.courtData.opp_attorney_id) {
                                ts.setValue(this.courtData.opp_attorney_id.toString());
                            }
                        }
                    }
                });
            },
            error: () => {
                ShowNotification('Error', 'Failed to load attorneys.', 'error');
            }
        });
    }

    populateCaseNumFields() {
        const container = $('#event_caseNum_container');
        container.empty();
        if (this.courtData && this.courtData.case_num_format) {
            const parts = this.courtData.case_num_format.split('-');
            parts.forEach((part, index) => {
                const input = $(`<input type="text" class="form-control case-num-part mr-1" id="case_num_format_multiple${index + 1}" placeholder="${part}" required>`);
                input.on('change keyup', () => this.evaluateCaseNumberFields());
                container.append(input);
                if (index < parts.length - 1) {
                    container.append('<span class="mr-1">-</span>');
                }
            });
        } else {
            const input = $('<input type="text" id="event_caseNum" class="form-control case-num-part" required>');
            input.on('change keyup', () => this.evaluateCaseNumberFields());
            container.append(input);
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
                        const motionTom = $('#event_motion')[0].tomselect;
                        if (motionTom) motionTom.setValue(evt.motion_id || '');
                        const typeTom = $('#event_type')[0].tomselect;
                        if (typeTom) typeTom.setValue(evt.type_id || '');
                        $('#event_customMotion').val(evt.custom_motion || '');
                        const attorneyTom = $('#event_attorney')[0].tomselect;
                        if (attorneyTom && evt.attorney_id) attorneyTom.setValue(evt.attorney_id);
                        const oppAttorneyTom = $('#event_opposingAttorney')[0].tomselect;
                        if (oppAttorneyTom && evt.opp_attorney_id) oppAttorneyTom.setValue(evt.opp_attorney_id);
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
                    ShowNotification('Error', 'Failed to fetch event details by case number.', 'error');
                }
            });
        }
    }

    populateCourtTemplateFields() {
        const container = $('#court_template_fields');
        container.empty();
        if (this.courtData && this.courtData.court_templates) {
            this.courtData.court_templates.forEach((template, index) => {
                const fieldName = `${template.name}_|${template.court_id}`;
                const sanitizedId = fieldName.replace(/[^A-Za-z0-9-]/g, '');
                let fieldHtml = '';
                if (template.field_type === 'yes_no') {
                    fieldHtml = `
                        <div class="col-md-4 mb-3">
                            <label>${template.name}</label>
                            <div>
                                <label style="margin-left: 30px;">
                                    <input type="radio" id="user_customer_field${index}" name="template[${fieldName}]" value="yes" class="form-check-input">Yes
                                </label>
                                <label style="margin-left: 30px;">
                                    <input type="radio" id="user_customer_field${index}" name="template[${fieldName}]" value="no" class="form-check-input">No
                                </label>
                            </div>
                        </div>`;
                } else {
                    fieldHtml = `
                        <div class="col-md-4 mb-3">
                            <label>${template.name}</label>
                            <input type="${template.field_type || 'text'}" class="form-control" name="template[${fieldName}]" id="${sanitizedId}">
                        </div>`;
                }
                container.append(fieldHtml);
            });
        }
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
            events: `${this.service.baseUrl}TimeslotAPI/GetCourtTimeslots/${this.courtId}`,
            selectable: true,
            selectMirror: true,
            editable: true,
            select: this.handleDateSelect.bind(this),
            eventClick: this.handleEventClick.bind(this),
            eventDrop: this.handleEventDrop.bind(this),
            selectAllow: function (selectInfo) {
                return selectInfo.start.getDay() !== 0 && selectInfo.start.getDay() !== 6;
            },
            slotMinTime: '06:00:00',
            slotMaxTime: '17:00:00',
            slotDuration: '00:15:00',
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
                    const minutesPerPixel = 15 / slotHeight;
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
                let available = arg.event.extendedProps.availableSlots;
                let tsId = arg.event.id;
                let checkbox = `<input style="top: .8rem;width: .95rem;height: .95rem;" class="m-1 float-right" disabled="" type="checkbox" id="cb${tsId}" value="${tsId}">`;
                let span = `<span>${timeText}${checkbox}</span>`;
                let div = `<div>${available} Available (Timeslot ${tsId})<br></div>`;
                return { html: span + div };
            }
        });
        this.calendar.render();
    }

    bindEventHandlers() {
        $('#editCourtBtn').on('click', this.handleEditCourt.bind(this));
        $('#userDefinedFieldsBtn').on('click', this.handleUserDefinedFields.bind(this));
        $('#truncateBtn').on('click', this.handleTruncate.bind(this));
        $('#extendBtn').on('click', this.handleExtend.bind(this));
        $('#deleteTimeslotsBtn').on('click', this.handleDeleteTimeslots.bind(this));
        $('#copyTimeslotsBtn').on('click', this.handleCopyTimeslots.bind(this));
        $('#saveTimeslotPaneBtn').on('click', this.handleSaveTimeslot.bind(this));
        $('#deleteTimeslotPaneBtn').on('click', this.handleDeleteTimeslot.bind(this));
        $('#saveEventPaneBtn').on('click', this.handleSaveEvent.bind(this));
        $('#cancelHearingBtn').on('click', this.handleCancelHearing.bind(this));
        $('#rescheduleBtn').on('click', this.handleReschedule.bind(this));
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
    }

    onTimeslotModalShow() {
        $('#cancelHearingBtn').hide();
        $('#rescheduleBtn').hide();
        const allDayChecked = $('#timeslot_allDay').val() === 'true';
        $('.time-selection').toggle(!allDayChecked);
        $('.quantity-group').toggle(!allDayChecked);
        $('.cattle-call').toggle(!allDayChecked);
        $('.public_block').hide();
        $('.block_reason').hide();
        $('.nav-tabs a').on('shown.bs.tab', (e) => {
            if (e.target.hash === '#eventTab') {
                this.populateMotionSelectExcludingRestricted();
                this.populateCourtTemplateFields();
                $('#other_motion_row').toggle($('#event_motion').val() === '221');
            }
        });
    }

    handleCancelHearing(e, cancellationReason) {
        e.preventDefault();
        const eventId = parseInt($('#edit_eventId').val());
        if (!isNaN(eventId) && eventId > 0) {
            $.ajax({
                url: `${this.service.baseUrl}EventAPI/CancelEvent/${eventId}`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ cancellation_reason: cancellationReason }),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: () => {
                    ShowNotification('Success', 'Hearing cancelled successfully.', 'success');
                    this.calendar.refetchEvents();
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                },
                error: error => {
                    ShowNotification('Error Cancelling Hearing', error.statusText, 'error');
                }
            });
        }
    }

    handleReschedule(e) {
        e.preventDefault();
        const timeslotModal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
        if (timeslotModal) {
            timeslotModal.hide();
        }
        const rescheduleModal = new bootstrap.Modal(document.getElementById('RescheduleHearingModal'));
        rescheduleModal.show();
    }

    handleDateSelect(info) {
        const start = this.formatLocalDateTime(info.start);
        const end = this.formatLocalDateTime(info.end);
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/GetOverlappingTimeslots/${this.courtId}?start=${start}&end=${end}`,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                this.clearTimeslotForm();
                $('#timeslot_startTime').val(start);
                $('#timeslot_endTime').val(end);
                $('#timeslot_allDay').val('false');
                $('.time-selection').show();
                $('.quantity-group').show();
                $('.cattle-call').toggle(response.length > 0);
                const title = this.getDateRangeTitle(new Date(start), new Date(end));
                $('#TimeslotModalLabel').text(title);
                const timeslotModal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
                timeslotModal.show();
                $('.nav-tabs li:not(:first)').hide();
                $('#timeslot_blockReason').closest('.row').hide();
                this.populateMotionSelectExcludingRestricted();
            },
            error: () => {
                ShowNotification('Error', 'Failed to check overlapping timeslots.', 'error');
            }
        });
    }

    handleEventClick(info) {
        this.viewTimeslot(parseInt(info.event.id));
    }

    handleEventDrop(info) {
        const timeslotId = parseInt(info.event.id);
        const newStart = this.formatLocalDateTime(info.event.start);
        const newEnd = this.formatLocalDateTime(info.event.end);

        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/GetTimeslot/${timeslotId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response) {
                    const timeslotData = {
                        id: timeslotId,
                        start: newStart,
                        end: newEnd,
                        allDay: response.allDay,
                        blocked: response.blocked,
                        publicBlock: response.publicBlock,
                        blockReason: response.blockReason,
                        duration: response.duration,
                        quantity: response.quantity,
                        description: response.description,
                        category: response.category,
                        courtId: this.courtId,
                        restrictedMotions: response.restrictedMotions
                    };
                    this.updateTimeslot(timeslotData);
                } else {
                    ShowNotification('Error', 'Failed to retrieve timeslot details for update.', 'error');
                    info.revert();
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to retrieve timeslot details.', 'error');
                info.revert();
            }
        });
    }

    formatLocalDateTime(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        return `${year}-${month}-${day}T${hours}:${minutes}`;
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
        if (this.courtId !== -1) {
            window.location.href = `${this.extendCalendarUrl}/cid/${this.courtId}`;
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
                // Implement timeslot deletion logic
            }
        });
    }

    handleCopyTimeslots(e) {
        e.preventDefault();
        // Implement timeslot copying logic
    }

    handleSaveTimeslot(e) {
        e.preventDefault();
        if (this.validateTimeslotForm()) {
            const timeslotData = this.getTimeslotFormData();
            if (timeslotData.id <= 0) {
                let hasOverlap = false;
                let overlapping = [];
                $.ajax({
                    url: `${this.service.baseUrl}TimeslotAPI/GetOverlappingTimeslots/${this.courtId}?start=${timeslotData.start}&end=${timeslotData.end}`,
                    type: 'GET',
                    async: false,
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: response => {
                        overlapping = response;
                        hasOverlap = response.length > 0;
                    }
                });
                const isConcurrent = $('input[name="timeslot_concurrent"]:checked').val() === '1';
                if (!hasOverlap || isConcurrent) {
                    this.createTimeslot(timeslotData);
                } else {
                    const S = new Date(timeslotData.start);
                    const E = new Date(timeslotData.end);
                    let intervals = overlapping.map(t => ({ start: new Date(t.start), end: new Date(t.end) }));
                    intervals.sort((a, b) => a.start - b.start);
                    let merged = [];
                    for (let int of intervals) {
                        if (merged.length === 0 || merged[merged.length - 1].end < int.start) {
                            merged.push(int);
                        } else {
                            merged[merged.length - 1].end = Math.max(merged[merged.length - 1].end, int.end);
                        }
                    }
                    let gaps = [];
                    let current = S;
                    for (let m of merged) {
                        if (current < m.start) {
                            gaps.push({ start: current, end: m.start });
                        }
                        current = Math.max(current, m.end);
                    }
                    if (current < E) {
                        gaps.push({ start: current, end: E });
                    }
                    if (gaps.length === 0) {
                        ShowNotification('Error', 'No available time in the selected range for consecutive timeslot.', 'error');
                        return;
                    }
                    const minDuration = timeslotData.duration;
                    let created = false;
                    gaps.forEach(gap => {
                        const gapDuration = (gap.end - gap.start) / 60000;
                        if (gapDuration >= minDuration) {
                            let newData = { ...timeslotData };
                            newData.start = this.formatLocalDateTime(gap.start);
                            newData.end = this.formatLocalDateTime(gap.end);
                            newData.duration = gapDuration;
                            this.createTimeslot(newData);
                            created = true;
                        }
                    });
                    if (created) {
                        const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                        if (modal) modal.hide();
                        this.calendar.refetchEvents();
                    } else {
                        ShowNotification('Error', 'No sufficient gaps for the selected duration.', 'error');
                    }
                }
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
                    this.deleteTimeslot(timeslotId);
                }
            });
        }
    }

    validateTimeslotForm() {
        let isValid = true;
        const $startTime = $('#timeslot_startTime');
        if (!$startTime.val()) {
            $startTime.addClass('is-invalid');
            isValid = false;
        } else {
            $startTime.removeClass('is-invalid');
        }
        const $endTime = $('#timeslot_endTime');
        if (!$endTime.val()) {
            $endTime.addClass('is-invalid');
            isValid = false;
        } else {
            $endTime.removeClass('is-invalid');
        }
        const $duration = $('#timeslot_duration');
        if ($duration.val() <= 0) {
            $duration.addClass('is-invalid');
            isValid = false;
        } else {
            $duration.removeClass('is-invalid');
        }
        const $quantity = $('#timeslot_quantity');
        if ($quantity.val() < 1) {
            $quantity.addClass('is-invalid');
            isValid = false;
        } else {
            $quantity.removeClass('is-invalid');
        }
        return isValid;
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

    getTimeslotFormData() {
        const tsIdVal = $('#edit_timeslotId').val();
        const tsId = tsIdVal ? parseInt(tsIdVal) : 0;
        const durationVal = $('#timeslot_duration').val();
        const duration = durationVal ? parseInt(durationVal) : 0;
        const quantityVal = $('#timeslot_quantity').val();
        const quantity = quantityVal ? parseInt(quantityVal) : 0;
        const categoryVal = $('#timeslot_category').val();
        const category = categoryVal ? parseInt(categoryVal) : null;
        const restrictedTom = $('#timeslot_restrictedMotions')[0].tomselect;
        const restrictedMotions = restrictedTom ? restrictedTom.getValue().map(id => parseInt(id)).filter(id => !isNaN(id)) : [];
        return {
            id: tsId,
            start: $('#timeslot_allDay').val() === 'true' ? $('#timeslot_startTime').val().slice(0, 10) + 'T00:00' : $('#timeslot_startTime').val(),
            end: $('#timeslot_allDay').val() === 'true' ? $('#timeslot_endTime').val().slice(0, 10) + 'T00:00' : $('#timeslot_endTime').val(),
            allDay: $('#timeslot_allDay').val() === 'true',
            blocked: $('#timeslot_block').is(':checked'),
            publicBlock: $('#timeslot_publicBlock').is(':checked'),
            blockReason: $('#timeslot_blockReason').val(),
            duration: duration,
            quantity: quantity,
            description: $('#timeslot_description').val(),
            category: category,
            courtId: this.courtId,
            restrictedMotions: restrictedMotions
        };
    }

    getEventFormData() {
        const evtIdVal = $('#edit_eventId').val();
        const evtId = evtIdVal ? parseInt(evtIdVal) : 0;
        const motionTom = $('#event_motion')[0].tomselect;
        const typeTom = $('#event_type')[0].tomselect;
        const attorneyTom = $('#event_attorney')[0].tomselect;
        const opposingAttorneyTom = $('#event_opposingAttorney')[0].tomselect;
        const caseNumParts = $('#event_caseNum_container .case-num-part').map(function () { return $(this).val(); }).get();
        const caseNum = caseNumParts.join('-');
        const templateData = {};
        $('#court_template_fields [name^="template["]').each(function () {
            const name = $(this).attr('name').match(/\[(.+?)\]/)[1];
            templateData[name] = $(this).is(':radio') ? $(`input[name="template[${name}]"]:checked`).val() : $(this).val();
        });
        return {
            id: evtId,
            case_num: caseNum,
            motion_id: motionTom ? motionTom.getValue() : '',
            type_id: typeTom ? typeTom.getValue() : '',
            custom_motion: $('#event_customMotion').val(),
            attorney_id: attorneyTom ? attorneyTom.getValue() : '',
            opp_attorney_id: opposingAttorneyTom ? opposingAttorneyTom.getValue() : '',
            plaintiff: $('#event_plaintiff').val(),
            defendant: $('#event_defendant').val(),
            plaintiff_email: $('#event_plaintiffEmail').val(),
            defendant_email: $('#event_defendantEmail').val(),
            notes: $('#event_notes').val(),
            addon: $('#event_addon').val(),
            reminder: $('#event_reminder').val(),
            template: JSON.stringify(templateData)
        };
    }

    createTimeslot(timeslotData, isForEvent = false) {
        $.ajax({
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
                        this.calendar.refetchEvents();
                        const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                        if (modal) modal.hide();
                        ShowNotification('Success', 'Timeslot created successfully.', 'success');
                    }
                } else {
                    ShowNotification('Error', 'Unexpected Error', 'error');
                }
            },
            error: error => {
                ShowNotification('Error Creating Timeslot', error.statusText, 'error');
            }
        });
    }

    updateTimeslot(timeslotData) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (typeof result === 'object' && result.id) {
                    this.calendar.refetchEvents();
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Timeslot updated successfully.', 'success');
                } else {
                    ShowNotification('Error', 'Unexpected Error', 'error');
                }
            },
            error: error => {
                ShowNotification('Error Updating Timeslot', error.statusText, 'error');
            }
        });
    }

    deleteTimeslot(timeslotId) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/DeleteTimeslot/${timeslotId}`,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                this.calendar.refetchEvents();
                const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                if (modal) modal.hide();
                ShowNotification('Success', 'Timeslot deleted successfully.', 'success');
            },
            error: error => {
                ShowNotification('Error Deleting Timeslot', error.statusText, 'error');
            }
        });
    }

    createEvent(eventData) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/CreateEvent`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(eventData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (result === 200) {
                    this.calendar.refetchEvents();
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Event created successfully.', 'success');
                } else {
                    ShowNotification('Error', `Unexpected Error: Status=${result}`, 'error');
                }
            },
            error: error => {
                ShowNotification('Error Creating Event', error.statusText, 'error');
            }
        });
    }

    updateEvent(eventData) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/UpdateEvent`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(eventData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: result => {
                if (result === 200) {
                    this.calendar.refetchEvents();
                    const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                    if (modal) modal.hide();
                    ShowNotification('Success', 'Event updated successfully.', 'success');
                } else {
                    ShowNotification('Error', `Unexpected Error: Status=${result}`, 'error');
                }
            },
            error: error => {
                ShowNotification('Error Updating Event', error.statusText, 'error');
            }
        });
    }

    cancelHearing(eventId, cancellationReason) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/CancelEvent/${eventId}`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ cancellation_reason: cancellationReason }),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                this.calendar.refetchEvents();
                const modal = bootstrap.Modal.getInstance(document.getElementById('TimeslotModal'));
                if (modal) modal.hide();
                ShowNotification('Success', 'Hearing cancelled successfully.', 'success');
            },
            error: error => {
                ShowNotification('Error Cancelling Hearing', error.statusText, 'error');
            }
        });
    }

    viewEvent(eventId) {
        const getUrl = `${this.service.baseUrl}EventAPI/GetEvent/${eventId}`;
        $('#progress-timeslot').show();

        $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    $('#edit_eventId').val(response.data.id);
                    const motionTom = $('#event_motion')[0].tomselect;
                    if (motionTom) motionTom.setValue(response.data.motion_id);
                    const typeTom = $('#event_type')[0].tomselect;
                    if (typeTom) typeTom.setValue(response.data.type_id);
                    $('#event_customMotion').val(response.data.custom_motion);
                    const attorneyTom = $('#event_attorney')[0].tomselect;
                    if (attorneyTom) attorneyTom.setValue(response.data.attorney_id);
                    const oppAttorneyTom = $('#event_opposingAttorney')[0].tomselect;
                    if (oppAttorneyTom) oppAttorneyTom.setValue(response.data.opp_attorney_id);
                    const caseNumParts = response.data.case_num.split('-');
                    $('#event_caseNum_container .case-num-part').each((index, el) => {
                        $(el).val(caseNumParts[index] || '');
                    });
                    $('#event_plaintiff').val(response.data.plaintiff);
                    $('#event_defendant').val(response.data.defendant);
                    $('#event_plaintiffEmail').val(response.data.plaintiff_email);
                    $('#event_defendantEmail').val(response.data.defendant_email);
                    $('#event_notes').val(response.data.notes);
                    $('#event_addon_check').prop('checked', response.data.addon === '1');
                    $('#event_addon').val(response.data.addon);
                    $('#event_reminder_check').prop('checked', response.data.reminder === '1');
                    $('#event_reminder').val(response.data.reminder);
                    $('#event_editedBy').val(response.data.editedBy || '');
                    $('#event_updatedOn').val(response.data.updatedOn || '');
                    $('.edited-by').show();

                    $('#cancelHearingBtn').show();
                    $('#rescheduleBtn').show();

                    const modal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
                    modal.show();
                    $('.nav-tabs a[href="#eventTab"]').tab('show');
                    $('.cattle-call').hide();
                    if (response.data.motion_id === 221) {
                        $('#other_motion_row').show();
                    } else {
                        $('#other_motion_row').hide();
                    }
                } else {
                    ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                }
                $('#progress-timeslot').hide();
            },
            error: () => {
                ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                $('#progress-timeslot').hide();
            }
        });
    }

    viewTimeslot(timeslotId) {
        const getUrl = `${this.service.baseUrl}TimeslotAPI/GetTimeslot/${timeslotId}`;
        $('#progress-timeslot').show();

        $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response) {
                    $('#edit_timeslotId').val(response.id);
                    $('#timeslot_startTime').val(this.formatLocalDateTime(new Date(response.start)));
                    $('#timeslot_endTime').val(this.formatLocalDateTime(new Date(response.end)));
                    $('#timeslot_allDay').val('false');
                    $('.time-selection').show();
                    $('.quantity-group').show();
                    $('#timeslot_block').prop('checked', response.blocked);
                    $('#timeslot_publicBlock').prop('checked', response.publicBlock);
                    $('#timeslot_blockReason').val(response.blockReason);
                    $('#timeslot_duration').val(response.duration);
                    $('#timeslot_quantity').val(response.quantity);
                    $('#timeslot_description').val(response.description);
                    $('#timeslot_category').val(response.category);
                    const tomSelect = $('#timeslot_restrictedMotions')[0].tomselect;
                    tomSelect.clear();
                    tomSelect.clearOptions();
                    if (response.restrictedMotions && response.restrictedMotions.length > 0) {
                        response.restrictedMotions.forEach(id => tomSelect.addItem(id));
                    }

                    const title = this.getDateRangeTitle(new Date(response.start), new Date(response.end));
                    $('#TimeslotModalLabel').text(title);

                    $.ajax({
                        url: `${this.service.baseUrl}TimeslotAPI/GetOverlappingTimeslots/${this.courtId}?start=${this.formatLocalDateTime(new Date(response.start))}&end=${this.formatLocalDateTime(new Date(response.end))}`,
                        type: 'GET',
                        beforeSend: xhr => this.setAjaxHeaders(xhr),
                        success: overlapResponse => {
                            if (overlapResponse.length > 0) {
                                $('.cattle-call').show();
                            } else {
                                $('.cattle-call').hide();
                            }
                        },
                        error: () => {
                            $('.cattle-call').hide();
                            ShowNotification('Error', 'Failed to check overlapping timeslots.', 'error');
                        }
                    });

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
                    $('.nav-tabs a[href="#eventTab"]').tab('show');
                    this.clearEventForm();
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

    loadEventsForTimeslot(timeslotId) {
        const getUrl = `${this.service.baseUrl}EventAPI/GetEventsForTimeslot/${timeslotId}`;

        $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                $('#eventsTableBody').empty();
                if (response.data) {
                    if (response.data.length === 1) {
                        const e = response.data[0];
                        $('#edit_eventId').val(e.id);
                        const motionTom = $('#event_motion')[0].tomselect;
                        if (motionTom) motionTom.setValue(e.motion_id);
                        const typeTom = $('#event_type')[0].tomselect;
                        if (typeTom) typeTom.setValue(e.type_id);
                        $('#event_customMotion').val(e.custom_motion);
                        const attorneyTom = $('#event_attorney')[0].tomselect;
                        if (attorneyTom) attorneyTom.setValue(e.attorney_id);
                        const oppAttorneyTom = $('#event_opposingAttorney')[0].tomselect;
                        if (oppAttorneyTom) oppAttorneyTom.setValue(e.opp_attorney_id);
                        const caseNumParts = e.case_num.split('-');
                        $('#event_caseNum_container .case-num-part').each((index, el) => {
                            $(el).val(caseNumParts[index] || '');
                        });
                        $('#event_plaintiff').val(e.plaintiff);
                        $('#event_defendant').val(e.defendant);
                        $('#event_plaintiffEmail').val(e.plaintiff_email);
                        $('#event_defendantEmail').val(e.defendant_email);
                        $('#event_notes').val(e.notes);
                        $('#event_addon_check').prop('checked', e.addon === '1');
                        $('#event_addon').val(e.addon);
                        $('#event_reminder_check').prop('checked', e.reminder === '1');
                        $('#event_reminder').val(e.reminder);
                        $('#event_editedBy').val(e.editedBy || '');
                        $('#event_updatedOn').val(e.updatedOn || '');
                        $('.edited-by').show();

                        $('#cancelHearingBtn').show();
                        $('#rescheduleBtn').show();
                    } else {
                        $('#cancelHearingBtn').hide();
                        $('#rescheduleBtn').hide();
                    }
                    response.data.forEach(e => {
                        const row = `
                            <tr><td><a href="#" class="editEventBtn" data-id="${e.id}"><i class="fas fa-edit"></i></a></td>
                                <td>${e.case_num || ''}</td>
                                <td>${e.motion}</td>
                                <td>${e.attorney}</td>
                                <td>${e.plaintiff}</td>
                                <td>${e.opposingAttorney}</td>
                                <td>${e.defendant}</td>
                                <td>
                                    <a href="#" class="deleteEventBtn" data-id="${e.id}"><i class="fas fa-trash"></i></a>
                                </td>
                            </tr>`;
                        $('#eventsTableBody').append(row);
                    });
                    $('.editEventBtn').on('click', (ev) => {
                        ev.preventDefault();
                        this.viewEvent(parseInt($(ev.target).closest('a').data('id')));
                    });
                    $('.deleteEventBtn').on('click', (ev) => {
                        ev.preventDefault();
                        const eventId = parseInt($(ev.target).closest('a').data('id'));
                        Swal.fire({
                            title: 'Delete Event?',
                            text: 'Are you sure you wish to delete this event?',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Yes',
                            cancelButtonText: 'No'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $.ajax({
                                    url: `${this.service.baseUrl}EventAPI/DeleteEvent/${eventId}`,
                                    type: 'GET',
                                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                                    success: () => {
                                        ShowNotification('Success', 'Event deleted successfully.', 'success');
                                        this.loadEventsForTimeslot(timeslotId);
                                    },
                                    error: error => {
                                        ShowNotification('Error Deleting Event', error.statusText, 'error');
                                    }
                                });
                            }
                        });
                    });
                }
                $('.cattle-call').hide();
            },
            error: () => {
                ShowNotification('Error', 'Failed to load events for timeslot.', 'error');
            }
        });
    }

    clearTimeslotForm() {
        $('#edit_timeslotId').val('');
        $('#timeslot_startTime').val('');
        $('#timeslot_endTime').val('');
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

    clearEventForm() {
        $('#edit_eventId').val('');
        const fields = ['motion', 'type', 'attorney', 'opposingAttorney'];
        fields.forEach(field => {
            const tomSelect = $(`#event_${field}`)[0].tomselect;
            if (tomSelect) tomSelect.clear();
        });
        $('#event_customMotion').val('');
        $('#event_caseNum_container .case-num-part').val('');
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
        $('#event_updatedOn').val('');
        $('.edited-by').hide();
        $('#cancelHearingBtn').hide();
        $('#rescheduleBtn').hide();
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'TimeslotModal') {
            courtCalendarControllerInstance.clearTimeslotForm();
            courtCalendarControllerInstance.clearEventForm();
            $('#eventsTableBody').empty();
        }
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
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
