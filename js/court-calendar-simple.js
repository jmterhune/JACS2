// Filename: Resources/js/courtcalendar.js
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
        this.courtEditUrl = params.courtEditUrl || '/court-edit';
        this.userDefinedFieldUrl = params.userDefinedFieldUrl || '/user-fields';
        this.truncateCalendarUrl = params.truncateCalendarUrl || '/truncate-calendar';
        this.extendCalendarUrl = params.extendCalendarUrl || '/extend-calendar';
        this.courtData = null;
        this.caseTypes = null;
        courtCalendarControllerInstance = this;
    }

    getCourtIdFromUrl() {
        return parseInt(getValueFromUrl('cid')) || -1;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const promCourt = this.fetchCourtData();
        const promCategory = this.populateCategorySelect();
        const promEventType = this.populateEventTypeSelect();
        const promCaseTypes = this.populateCaseTypes();
        const promAttorney = this.populateAttorneySelects();
        const promTemplate = this.populateCourtTemplateFields();
        this.initCalendar();
        this.bindEventHandlers();
        this.initTomSelect();
        const courtTypeSelect = $(".court-types option:selected").text();
        if (courtTypeSelect.length) {
            this.changeLabel(courtTypeSelect);
        }
        $('.case-num-part').on('keyup change', () => this.evaluateCaseNumberFields());
        const timeslotModalElement = document.getElementById('TimeslotModal');
        if (timeslotModalElement) {
            timeslotModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
            timeslotModalElement.addEventListener('shown.bs.modal', this.onTimeslotModalShow.bind(this));
        }
        $.when(promCourt, promCategory, promEventType, promCaseTypes, promAttorney, promTemplate).then(() => this.populateEventDefaults());
    }

    fetchCourtData() {
        return $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourt/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    this.courtData = response.data;
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load court data.', 'error');
            }
        });
    }

    initCalendar() {
        const calendarEl = document.getElementById('calendar');
        if (calendarEl) {
            this.calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'timeGridWeek',
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
                },
                editable: true,
                selectable: true,
                select: info => this.handleCalendarSelect(info),
                eventClick: info => this.handleEventClick(info),
                events: (fetchInfo, successCallback, failureCallback) => {
                    this.fetchCalendarEvents(fetchInfo, successCallback, failureCallback);
                },
                eventDrop: info => this.handleEventDrop(info),
                eventResize: info => this.handleEventResize(info),
                height: 'auto',
                slotMinTime: '07:00:00',
                slotMaxTime: '17:00:00',
                slotDuration: '00:15:00',
                allDaySlot: false,
                nowIndicator: true,
                hiddenDays: [0, 6],
                businessHours: {
                    daysOfWeek: [1, 2, 3, 4, 5],
                    startTime: '07:00',
                    endTime: '17:00'
                },
                eventTimeFormat: {
                    hour: 'numeric',
                    minute: '2-digit',
                    meridiem: 'short'
                }, eventDidMount: (arg) => {
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
    }

    fetchCalendarEvents(fetchInfo, successCallback, failureCallback) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/GetCourtTimeslots/${this.courtId}`,
            type: 'GET',
            data: {
                start: fetchInfo.startStr,
                end: fetchInfo.endStr
            },
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    const events = response.data.map(ts => ({
                        id: ts.id,
                        title: ts.description || this.getTimeRange(new Date(ts.start), new Date(ts.end)),
                        start: ts.start,
                        end: ts.end,
                        allDay: ts.allDay,
                        backgroundColor: ts.blocked ? '#ff4444' : '#3788d8',
                        borderColor: ts.public_block ? '#ffbb33' : '#3788d8',
                        extendedProps: {
                            timeslot: ts
                        }
                    }));
                    successCallback(events);
                } else {
                    successCallback([]);
                }
            },
            error: () => {
                failureCallback();
                ShowNotification('Error', 'Failed to load calendar events.', 'error');
            }
        });
    }

    handleCalendarSelect(info) {
        if (!this.isAdmin) return;
        this.clearTimeslotForm();
        this.clearEventForm();
        $('#timeslot_startTime').val(info.startStr.slice(0, 16));
        $('#timeslot_endTime').val(info.endStr.slice(0, 16));
        $('#TimeslotModal').modal('show');
        $('#TimeslotModalLabel').text('Create Timeslot');
        $('.nav-tabs a[href="#timeslotTab"]').tab('show');
    }

    handleEventClick(info) {
        const timeslotId = info.event.id;
        this.loadTimeslot(timeslotId);
    }

    handleEventDrop(info) {
        if (!this.isAdmin) return;
        const timeslotId = info.event.id;
        const data = {
            id: timeslotId,
            start: info.event.start.toISOString(),
            end: info.event.end.toISOString()
        };
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateTimeslot`,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                ShowNotification('Success', 'Timeslot updated.', 'success');
            },
            error: () => {
                info.revert();
                ShowNotification('Error', 'Failed to update timeslot.', 'error');
            }
        });
    }

    handleEventResize(info) {
        if (!this.isAdmin) return;
        const timeslotId = info.event.id;
        const data = {
            id: timeslotId,
            start: info.event.start.toISOString(),
            end: info.event.end.toISOString(),
            duration: Math.round((info.event.end - info.event.start) / 60000)
        };
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/UpdateTimeslot`,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                ShowNotification('Success', 'Timeslot updated.', 'success');
            },
            error: () => {
                info.revert();
                ShowNotification('Error', 'Failed to update timeslot.', 'error');
            }
        });
    }

    bindEventHandlers() {
        $('#editCourtBtn').on('click', () => {
            window.location.href = `${this.courtEditUrl}?cid=${this.courtId}`;
        });
        $('#userDefinedFieldsBtn').on('click', () => {
            window.location.href = `${this.userDefinedFieldUrl}?cid=${this.courtId}`;
        });
        $('#truncateBtn').on('click', () => this.truncateCalendar());
        $('#icalExportBtn').on('click', () => this.exportICal());
        $('#monthlyExportBtn').on('click', () => this.exportMonthly());
        $('#deleteTimeslotsBtn').on('click', () => this.deleteSelectedTimeslots());
        $('#copyTimeslotsBtn').on('click', () => this.copySelectedTimeslots());
        $('#printCalendarBtn').on('click', () => this.printCalendar());
        $('#saveEventPaneBtn').on('click', () => this.saveEvent());
        $('#timeslot_block').on('change', () => {
            const isChecked = $('#timeslot_block').is(':checked');
            $('.public_block, .block_reason').toggle(isChecked);
        });
        $('#cattlecall_yes, #cattlecall_no').on('change', () => {
            const isCattleCall = $('#cattlecall_no').is(':checked');
            $('.time-selection, .quantity-group').toggle(!isCattleCall);
        });
        $('.nav-tabs a').on('click', function (e) {
            e.preventDefault();
            $(this).tab('show');
        });
    }

    populateAttorneySelects() {
        return $.ajax({
            url: `${this.service.baseUrl}AttorneyAPI/GetAttorneyDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    ['event_attorney', 'event_opposingAttorney'].forEach(id => {
                        const select = document.getElementById(id);
                        if (select) {
                            const ts = new TomSelect(select, {
                                options: response.data.map(a => ({ value: a.Key, text: a.Value })),
                                items: [],
                                valueField: 'value',
                                labelField: 'text',
                                searchField: ['text'],
                                maxItems: 1,
                                placeholder: id === 'event_attorney' ? 'Select attorney...' : 'Select opposing attorney...',
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
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load attorneys.', 'error');
            }
        });
    }

    populateCourtTemplateFields() {
        return $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetUserDefinedFields/${this.courtId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const container = $('#court_template_fields');
                container.empty();
                if (response.data) {
                    response.data.forEach(field => {
                        container.append(`
                            <div class="col-md-6">
                                <label>${field.name}</label>
                                <input type="text" class="form-control udf-field" id="udf_${field.id}" autocomplete="off">
                            </div>
                        `);
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load user-defined fields.', 'error');
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

    populateMotionSelectExcludingRestricted() {
        const restricted = $('#timeslot_restrictedMotions').val() || [];
        $.ajax({
            url: `${this.service.baseUrl}CourtMotionAPI/GetAvailableMotionDropDownItems/${this.courtId}/${restricted.join(',')}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = document.getElementById('event_motion');
                if (select && response.data) {
                    const ts = select.tomselect;
                    if (ts) ts.destroy();
                    new TomSelect(select, {
                        options: response.data.map(m => ({ value: m.Key, text: m.Value })),
                        items: [],
                        valueField: 'value',
                        labelField: 'text',
                        searchField: ['text'],
                        maxItems: 1,
                        placeholder: 'Select motion...',
                        persist: false,
                        create: false
                    });
                    select.setAttribute('tabindex', '-1');
                    select.setAttribute('autocomplete', 'off');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load available motions.', 'error');
            }
        });
    }

    populateCaseTypes() {
        return $.ajax({
            url: `${this.service.baseUrl}CourtTypeAPI/GetCourtTypeDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    this.caseTypes = response.data;
                    const select = $('#case_num_format_multiple2, #case_num_format_multiple3');
                    select.each((i, el) => {
                        const $el = $(el);
                        $el.empty();
                        $el.append('<option value="">Select Type</option>');
                        response.data.forEach(item => {
                            $el.append(`<option value="${item.Value}">${item.Value}</option>`);
                        });
                    });
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load court types.', 'error');
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
                if (response.data) {
                    const select = $('#timeslot_category');
                    select.empty();
                    select.append('<option value="">Select Category</option>');
                    response.data.forEach(item => {
                        select.append(`<option value="${item.Key}">${item.Value}</option>`);
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
                    const ts = new TomSelect(select, {
                        options: response.data.map(m => ({ value: m.Key, text: m.Value })),
                        items: [],
                        valueField: 'value',
                        labelField: 'text',
                        searchField: ['text'],
                        maxItems: 1,
                        placeholder: 'Select event type...',
                        persist: false,
                        create: false
                    });
                    select.setAttribute('tabindex', '-1');
                    select.setAttribute('autocomplete', 'off');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load event types.', 'error');
            }
        });
    }

    changeLabel(label) {
        $('.court-type-label').text(label || 'Case Type');
    }

    evaluateCaseNumberFields() {
        const parts = $('.case-num-part').map((i, el) => $(el).val().trim()).get();
        const caseNum = parts.filter(p => p).join('-');
        $('#event_caseNum').val(caseNum);
        if (this.courtData && this.courtData.case_num_format) {
            const formatParts = this.courtData.case_num_format.split('-');
            const valid = parts.length === formatParts.length && parts.every((p, i) => {
                const fp = formatParts[i];
                if (fp === 'YYYY') return /^\d{4}$/.test(p);
                if (fp === 'YY') return /^\d{2}$/.test(p);
                if (fp === '0' || this.caseTypes.some(ct => ct.Value === p)) return true;
                return p.length <= 7; // Assuming max length for other parts
            });
            $('.case-num-part').toggleClass('is-invalid', !valid);
            return valid;
        }
        return true;
    }

    loadTimeslot(timeslotId) {
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/GetTimeslot/${timeslotId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    const ts = response.data;
                    $('#edit_timeslotId').val(ts.id);
                    $('#timeslot_startTime').val(ts.start.slice(0, 16));
                    $('#timeslot_endTime').val(ts.end.slice(0, 16));
                    $('#timeslot_block').prop('checked', ts.blocked);
                    $('#timeslot_publicBlock').prop('checked', ts.public_block);
                    $('#timeslot_blockReason').val(ts.block_reason);
                    $('#timeslot_duration').val(ts.duration);
                    $('#timeslot_quantity').val(ts.quantity);
                    $('#timeslot_description').val(ts.description);
                    $('#timeslot_category').val(ts.category_id);
                    const restrictedTom = $('#timeslot_restrictedMotions')[0]?.tomselect;
                    if (restrictedTom) restrictedTom.setValue(ts.restrictedMotions || []);
                    $('.public_block, .block_reason').toggle(ts.blocked);
                    $('#cattlecall_yes').prop('checked', ts.quantity > 0);
                    $('#cattlecall_no').prop('checked', ts.quantity === 0);
                    $('.time-selection, .quantity-group').toggle(ts.quantity > 0);
                    this.loadEventsForTimeslot(ts.id);
                    $('#TimeslotModal').modal('show');
                    $('#TimeslotModalLabel').text('Edit Timeslot');
                    $('.nav-tabs a[href="#timeslotTab"]').tab('show');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load timeslot.', 'error');
            }
        });
    }

    saveTimeslot() {
        const data = {
            id: $('#edit_timeslotId').val() || null,
            court_id: this.courtId,
            start: $('#timeslot_startTime').val(),
            end: $('#timeslot_endTime').val(),
            blocked: $('#timeslot_block').is(':checked'),
            public_block: $('#timeslot_publicBlock').is(':checked'),
            block_reason: $('#timeslot_blockReason').val(),
            duration: parseInt($('#timeslot_duration').val()) || 15,
            quantity: parseInt($('#timeslot_quantity').val()) || 1,
            description: $('#timeslot_description').val(),
            category_id: $('#timeslot_category').val(),
            restrictedMotions: $('#timeslot_restrictedMotions').val() || []
        };
        if (!data.start || !data.end) {
            ShowNotification('Error', 'Start and end times are required.', 'error');
            return;
        }
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/${data.id ? 'UpdateTimeslot' : 'CreateTimeslot'}`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                ShowNotification('Success', 'Timeslot saved successfully.', 'success');
                $('#TimeslotModal').modal('hide');
                this.calendar.refetchEvents();
            },
            error: error => {
                ShowNotification('Error', 'Failed to save timeslot: ' + error.statusText, 'error');
            }
        });
    }

    loadEventsForTimeslot(timeslotId) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/GetEventsForTimeslot/${timeslotId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const tbody = $('#eventsTableBody');
                tbody.empty();
                if (response.data && response.data.length > 0) {
                    response.data.forEach(event => {
                        const row = `
                            <tr>
                                <td><a href="#" class="editEventBtn" data-id="${event.id}"><i class="fas fa-edit"></i></a></td>
                                <td>${event.case_num || ''}</td>
                                <td>${event.motion || ''}</td>
                                <td>${event.attorney || ''}</td>
                                <td>${event.plaintiff || ''}</td>
                                <td>${event.opposing_attorney || ''}</td>
                                <td>${event.defendant || ''}</td>
                                <td><a href="#" class="deleteEventBtn" data-id="${event.id}"><i class="fas fa-trash"></i></a></td>
                            </tr>
                        `;
                        tbody.append(row);
                    });
                    $('.editEventBtn').off('click').on('click', (ev) => {
                        ev.preventDefault();
                        const eventId = parseInt($(ev.target).closest('a').data('id'));
                        this.loadEvent(eventId);
                    });
                    $('.deleteEventBtn').off('click').on('click', (ev) => {
                        ev.preventDefault();
                        const eventId = parseInt($(ev.target).closest('a').data('id'));
                        Swal.fire({
                            title: 'Delete Event?',
                            text: 'Are you sure you wish to delete this event?',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Yes',
                            cancelButtonText: 'No'
                        }).then(result => {
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
                                        ShowNotification('Error', 'Failed to delete event: ' + error.statusText, 'error');
                                    }
                                });
                            }
                        });
                    });
                }
                $('.cattle-call').toggle(response.data.length === 0);
            },
            error: () => {
                ShowNotification('Error', 'Failed to load events for timeslot.', 'error');
            }
        });
    }

    loadEvent(eventId) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/GetEvent/${eventId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    const evt = response.data;
                    $('#edit_eventId').val(evt.id);
                    const motionTom = $('#event_motion')[0]?.tomselect;
                    if (motionTom) motionTom.setValue(evt.motion_id || '');
                    const typeTom = $('#event_type')[0]?.tomselect;
                    if (typeTom) typeTom.setValue(evt.type_id || '');
                    const attorneyTom = $('#event_attorney')[0]?.tomselect;
                    if (attorneyTom) attorneyTom.setValue(evt.attorney_id || '');
                    const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
                    if (oppTom) oppTom.setValue(evt.opposing_attorney_id || '');
                    $('#event_customMotion').val(evt.custom_motion || '');
                    const caseParts = evt.case_num ? evt.case_num.split('-') : [];
                    $('.case-num-part').each((i, el) => {
                        $(el).val(caseParts[i] || '');
                    });
                    $('#event_plaintiff').val(evt.plaintiff || '');
                    $('#event_defendant').val(evt.defendant || '');
                    $('#event_plaintiffEmail').val(evt.plaintiff_email || '');
                    $('#event_defendantEmail').val(evt.defendant_email || '');
                    $('#event_notes').val(evt.notes || '');
                    $('#event_addon_check').prop('checked', evt.addon > 0);
                    $('#event_addon').val(evt.addon || 0);
                    $('#event_reminder_check').prop('checked', evt.reminder > 0);
                    $('#event_reminder').val(evt.reminder || 0);
                    $('#event_editedBy').val(evt.edited_by || '');
                    $('#event_updatedOn').val(evt.updated_on || '');
                    $('.edited-by').toggle(!!evt.updated_on);
                    $('#cancelHearingBtn').toggle(this.isAdmin);
                    $('#rescheduleBtn').toggle(this.isAdmin);
                    $('.nav-tabs a[href="#eventTab"]').tab('show');
                }
            },
            error: () => {
                ShowNotification('Error', 'Failed to load event.', 'error');
            }
        });
    }

    saveEvent() {
        const data = {
            id: $('#edit_eventId').val() || null,
            timeslot_id: $('#edit_timeslotId').val(),
            motion_id: $('#event_motion').val() || null,
            type_id: $('#event_type').val() || null,
            attorney_id: $('#event_attorney').val() || null,
            opposing_attorney_id: $('#event_opposingAttorney').val() || null,
            custom_motion: $('#event_customMotion').val(),
            case_num: $('.case-num-part').map((i, el) => $(el).val().trim()).get().filter(v => v).join('-'),
            plaintiff: $('#event_plaintiff').val(),
            defendant: $('#event_defendant').val(),
            plaintiff_email: $('#event_plaintiffEmail').val(),
            defendant_email: $('#event_defendantEmail').val(),
            notes: $('#event_notes').val(),
            addon: $('#event_addon_check').is(':checked') ? parseInt($('#event_addon').val()) || 0 : 0,
            reminder: $('#event_reminder_check').is(':checked') ? parseInt($('#event_reminder').val()) || 0 : 0,
            template: $('.udf-field').map((i, el) => ({
                id: el.id.replace('udf_', ''),
                value: $(el).val()
            })).get()
        };
        if (!this.evaluateCaseNumberFields()) {
            ShowNotification('Error', 'Invalid case number format.', 'error');
            return;
        }
        if (this.courtData.plaintiff_required && !data.plaintiff) {
            ShowNotification('Error', 'Plaintiff is required.', 'error');
            return;
        }
        if (this.courtData.defendant_required && !data.defendant) {
            ShowNotification('Error', 'Defendant is required.', 'error');
            return;
        }
        if (this.courtData.plaintiff_attorney_required && !data.attorney_id) {
            ShowNotification('Error', 'Plaintiff attorney is required.', 'error');
            return;
        }
        if (this.courtData.defendant_attorney_required && !data.opposing_attorney_id) {
            ShowNotification('Error', 'Defendant attorney is required.', 'error');
            return;
        }
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/${data.id ? 'UpdateEvent' : 'CreateEvent'}`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                ShowNotification('Success', 'Event saved successfully.', 'success');
                this.loadEventsForTimeslot(data.timeslot_id);
                $('.nav-tabs a[href="#eventsTab"]').tab('show');
            },
            error: error => {
                ShowNotification('Error', 'Failed to save event: ' + error.statusText, 'error');
            }
        });
    }

    cancelHearing() {
        const eventId = $('#edit_eventId').val();
        if (!eventId) return;
        Swal.fire({
            title: 'Cancel Hearing?',
            text: 'Are you sure you want to cancel this hearing?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}EventAPI/CancelEvent/${eventId}`,
                    type: 'GET',
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: () => {
                        ShowNotification('Success', 'Hearing cancelled successfully.', 'success');
                        $('#TimeslotModal').modal('hide');
                        this.calendar.refetchEvents();
                    },
                    error: error => {
                        ShowNotification('Error', 'Failed to cancel hearing: ' + error.statusText, 'error');
                    }
                });
            }
        });
    }

    reschedule() {
        const eventId = $('#edit_eventId').val();
        if (!eventId) return;
        $('#RescheduleHearingModal').modal('show');
        // Populate reschedule modal with event data if needed
    }

    truncateCalendar() {
        Swal.fire({
            title: 'Truncate Calendar?',
            text: 'This will delete selected timeslots and their events. Proceed?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            input: 'select',
            inputOptions: {
                all: 'All',
                reserved: 'Reserved Only',
                unreserved: 'Unreserved Only'
            },
            inputPlaceholder: 'Select filter'
        }).then(result => {
            if (result.isConfirmed) {
                const filter = result.value || 'all';
                Swal.fire({
                    title: 'Select Date',
                    text: 'Truncate from which date?',
                    input: 'date',
                    inputValue: new Date().toISOString().slice(0, 10)
                }).then(dateResult => {
                    if (dateResult.isConfirmed) {
                        $.ajax({
                            url: `${this.service.baseUrl}CourtAPI/TruncateCalendar`,
                            type: 'POST',
                            contentType: 'application/json',
                            data: JSON.stringify({
                                courtId: this.courtId,
                                date: dateResult.value,
                                filter: filter
                            }),
                            beforeSend: xhr => this.setAjaxHeaders(xhr),
                            success: () => {
                                ShowNotification('Success', 'Calendar truncated successfully.', 'success');
                                this.calendar.refetchEvents();
                            },
                            error: error => {
                                ShowNotification('Error', 'Failed to truncate calendar: ' + error.statusText, 'error');
                            }
                        });
                    }
                });
            }
        });
    }

    exportICal() {
        window.location.href = `${this.service.baseUrl}TimeslotAPI/ExportICal/${this.courtId}`;
    }

    exportMonthly() {
        Swal.fire({
            title: 'Export Month',
            text: 'Select month to export:',
            input: 'month',
            inputValue: new Date().toISOString().slice(0, 7)
        }).then(result => {
            if (result.isConfirmed) {
                window.location.href = `${this.service.baseUrl}TimeslotAPI/ExportMonthly/${this.courtId}?month=${result.value}`;
            }
        });
    }

    deleteSelectedTimeslots() {
        const selected = this.calendar.getEvents().filter(e => e.extendedProps.selected).map(e => e.id);
        if (selected.length === 0) {
            ShowNotification('Warning', 'No timeslots selected.', 'warning');
            return;
        }
        Swal.fire({
            title: 'Delete Timeslots?',
            text: `Are you sure you want to delete ${selected.length} timeslot(s)?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}TimeslotAPI/DeleteTimeslots`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(selected),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: () => {
                        ShowNotification('Success', 'Timeslots deleted successfully.', 'success');
                        this.calendar.refetchEvents();
                    },
                    error: error => {
                        ShowNotification('Error', 'Failed to delete timeslots: ' + error.statusText, 'error');
                    }
                });
            }
        });
    }

    copySelectedTimeslots() {
        const selected = this.calendar.getEvents().filter(e => e.extendedProps.selected).map(e => e.id);
        if (selected.length === 0) {
            ShowNotification('Warning', 'No timeslots selected.', 'warning');
            return;
        }
        Swal.fire({
            title: 'Copy Timeslots',
            text: 'Select target date for copying:',
            input: 'date',
            inputValue: new Date().toISOString().slice(0, 10)
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}TimeslotAPI/TempCopy`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(selected),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: response => {
                        ShowNotification('Success', 'Timeslots copied successfully.', 'success');
                        this.calendar.refetchEvents();
                    },
                    error: error => {
                        ShowNotification('Error', 'Failed to copy timeslots: ' + error.statusText, 'error');
                    }
                });
            }
        });
    }

    printCalendar() {
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <html>
                <head>
                    <title>Calendar Print</title>
                    <link rel="stylesheet" href="/Resources/Libraries/fullcalendar/dist/index.global.min.css">
                    <style>
                        body { font-family: Arial, sans-serif; }
                        #calendar { max-width: 1100px; margin: 20px auto; }
                    </style>
                </head>
                <body>
                    <div id="calendar"></div>
                    <script src="/Resources/Libraries/fullcalendar/dist/index.global.min.js"></script>
                    <script>
                        document.addEventListener('DOMContentLoaded', function() {
                            var calendarEl = document.getElementById('calendar');
                            var calendar = new FullCalendar.Calendar(calendarEl, {
                                initialView: 'dayGridMonth',
                                events: ${JSON.stringify(this.calendar.getEvents().map(e => ({
            id: e.id,
            title: e.title,
            start: e.start.toISOString(),
            end: e.end.toISOString(),
            backgroundColor: e.backgroundColor,
            borderColor: e.borderColor
        })))},
                                headerToolbar: {
                                    left: 'title',
                                    center: '',
                                    right: ''
                                }
                            });
                            calendar.render();
                            setTimeout(() => window.print(), 500);
                        });
                    </script>
                </body>
            </html>
        `);
        printWindow.document.close();
    }

    onTimeslotModalShow() {
        $('#timeslot_startTime').focus();
        $('.public_block, .block_reason').toggle($('#timeslot_block').is(':checked'));
        $('.time-selection, .quantity-group').toggle($('#cattlecall_yes').is(':checked'));
    }

    clearTimeslotForm() {
        $('#edit_timeslotId').val('');
        $('#timeslot_startTime').val('');
        $('#timeslot_endTime').val('');
        $('#timeslot_block').prop('checked', false);
        $('#timeslot_publicBlock').prop('checked', false);
        $('#timeslot_blockReason').val('');
        $('#cattlecall_yes').prop('checked', true);
        $('#cattlecall_no').prop('checked', false);
        $('.time-selection').show();
        $('.quantity-group').show();
        $('.cattle-call').show();
        $('.public_block').hide();
        $('.block_reason').hide();
        $('#timeslot_duration').val('15');
        $('#timeslot_quantity').val('1');
        $('#timeslot_description').val('');
        $('#timeslot_category').val('');
        const tomSelect = $('#timeslot_restrictedMotions')[0]?.tomselect;
        if (tomSelect) tomSelect.clear();
    }

    populateEventDefaults() {
        if (!this.courtData) return;

        const attorneyTom = $('#event_attorney')[0]?.tomselect;
        if (attorneyTom && this.courtData.def_attorney_id) {
            attorneyTom.setValue(this.courtData.def_attorney_id);
        }

        const oppTom = $('#event_opposingAttorney')[0]?.tomselect;
        if (oppTom && this.courtData.opp_attorney_id) {
            oppTom.setValue(this.courtData.opp_attorney_id);
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
    }

    clearEventForm() {
        $('#edit_eventId').val('');
        const fields = ['motion', 'type', 'attorney', 'opposingAttorney'];
        fields.forEach(field => {
            const tomSelect = $(`#event_${field}`)[0]?.tomselect;
            if (tomSelect) tomSelect.clear();
        });
        $('#event_customMotion').val('');
        $('#event_caseNum_container .case-num-part:not([disabled])').val('');
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
        $('.udf-field').val('');
        this.populateEventDefaults();
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'TimeslotModal') {
            this.clearTimeslotForm();
            this.clearEventForm();
            $('#eventsTableBody').empty();
            $('.nav-tabs a[href="#timeslotTab"]').tab('show');
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