let courtCalendarControllerInstance = null;

class CourtCalendarController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.service = params.service || null;
        this.calendar = null;
        // Initialize courtId from URL parameter 'cid'
        this.courtId = this.getCourtIdFromUrl();
        // Store navigation URLs
        this.courtEditUrl = params.courtEditUrl || '/Court/Edit';
        this.userDefinedFieldUrl = params.userDefinedFieldUrl || '/Court/CustomFields';
        this.truncateCalendarUrl = params.truncateCalendarUrl || '/Court/Truncate';
        this.extendCalendarUrl = params.extendCalendarUrl || '/Court/Extend';
        courtCalendarControllerInstance = this;
    }

    // Extract courtId from URL parameter 'cid'
    getCourtIdFromUrl() {
        return parseInt(getValueFromUrl('cid')) || -1;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.initCalendar();
        this.bindEventHandlers();

        const timeslotModalElement = document.getElementById('TimeslotModal');
        if (timeslotModalElement) {
            timeslotModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const eventModalElement = document.getElementById('EventModal');
        if (eventModalElement) {
            eventModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
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
            selectAllow: function (selectInfo) {
                return selectInfo.start.getDay() !== 0 && selectInfo.start.getDay() !== 6; // Exclude weekends
            },
            slotMinTime: '08:00:00',
            slotMaxTime: '17:00:00',
            slotDuration: '00:15:00',
            allDaySlot: true,
            height: 'auto',
            eventDidMount: (arg) => {
                if (arg.isMirror) {
                    arg.el.style.backgroundColor = 'rgb(0, 123, 255)';
                    arg.el.style.borderColor = 'rgb(0, 123, 255)';
                    arg.el.style.color = 'white';
                    arg.el.style.borderRadius = '5px';
                    const startTime = arg.event.start.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
                    const endTime = arg.event.end.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
                    const durationMs = arg.event.end - arg.event.start;
                    const slotDurationMs = 15 * 60 * 1000;
                    const numSlots = Math.round(durationMs / slotDurationMs);
                    const html = `<div class="fc-event-main"><span>${startTime} - ${endTime}</span><div><br></div></div><div class="fc-event-resizer fc-event-resizer-end"></div>`;
                    arg.el.innerHTML = html;
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
        $('#saveTimeslotBtn').on('click', this.handleSaveTimeslot.bind(this));
        $('#deleteTimeslotBtn').on('click', this.handleDeleteTimeslot.bind(this));
        $('#saveEventBtn').on('click', this.handleSaveEvent.bind(this));
        $('#cancelHearingBtn').on('click', this.handleCancelHearing.bind(this));
        $('#rescheduleBtn').on('click', this.handleReschedule.bind(this));
        $('#timeslot_block').on('change', (e) => {
            if (e.target.checked) {
                $('#timeslot_blockReason').closest('.row').show();
            } else {
                $('#timeslot_blockReason').closest('.row').hide();
            }
        });
    }

    handleDateSelect(info) {
        const start = info.startStr;
        const end = info.endStr;
        $.ajax({
            url: `${this.service.baseUrl}TimeslotAPI/GetOverlappingTimeslots/${this.courtId}?start=${start}&end=${end}`,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                this.clearTimeslotForm();
                $('#timeslot_startTime').val(start.slice(0, 16));
                $('#timeslot_endTime').val(end.slice(0, 16));
                if (response.length > 0) {
                    $('#timeslot_concurrent').closest('.row').show();
                } else {
                    $('#timeslot_concurrent').closest('.row').hide();
                }
                const title = this.getDateRangeTitle(new Date(start), new Date(end));
                $('#TimeslotModalLabel').text(title);
                const timeslotModal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
                timeslotModal.show();
                $('.nav-tabs li:not(:first)').hide();
                $('#timeslot_blockReason').closest('.row').hide();
            },
            error: () => {
                ShowNotification('Error', 'Failed to check overlapping timeslots.', 'error');
            }
        });
    }

    handleEventClick(info) {
        this.viewTimeslot(parseInt(info.event.id));
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
        const activeTab = $('.tab-content .tab-pane.active').attr('id');
        if (activeTab === 'timeslotTab') {
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
                    if (!hasOverlap || timeslotData.concurrent) {
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
                                newData.start = gap.start.toISOString().slice(0, 16);
                                newData.end = gap.end.toISOString().slice(0, 16);
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
        } else if (activeTab === 'eventTab') {
            if (this.validateEventForm()) {
                const eventData = this.getEventFormData();
                const tsId = parseInt($('#edit_timeslotId').val());
                if (isNaN(tsId) || tsId <= 0) {
                    // Create default timeslot first
                    const timeslotData = this.getTimeslotFormData();
                    timeslotData.description = eventData.motion || 'Hearing';
                    timeslotData.concurrent = false;
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

    handleSaveEvent(e) {
        e.preventDefault();
        if (this.validateEventForm()) {
            const eventData = this.getEventFormData();
            if (eventData.id <= 0) {
                this.createEvent(eventData);
            } else {
                this.updateEvent(eventData);
            }
        }
    }

    handleCancelHearing(e) {
        e.preventDefault();
        const eventId = parseInt($('#edit_eventId').val());
        if (!isNaN(eventId) && eventId > 0) {
            Swal.fire({
                title: 'Cancel Hearing?',
                text: 'Are you sure you wish to cancel this hearing?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.cancelHearing(eventId);
                }
            });
        }
    }

    handleReschedule(e) {
        e.preventDefault();
        const eventModal = bootstrap.Modal.getInstance(document.getElementById('EventModal'));
        if (eventModal) {
            eventModal.hide();
        }
        const rescheduleModal = new bootstrap.Modal(document.getElementById('RescheduleHearingModal'));
        rescheduleModal.show();
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
        return isValid;
    }

    validateEventForm() {
        let isValid = true;
        const $motion = $('#event_motion');
        if (!$motion.val().trim()) {
            $motion.addClass('is-invalid');
            isValid = false;
        } else {
            $motion.removeClass('is-invalid');
        }
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
        return {
            id: tsId,
            start: $('#timeslot_startTime').val(),
            end: $('#timeslot_endTime').val(),
            allDay: false,
            block: $('#timeslot_block').is(':checked'),
            publicBlock: $('#timeslot_publicBlock').is(':checked'),
            blockReason: $('#timeslot_blockReason').val(),
            concurrent: $('#timeslot_concurrent').val() === 'yes',
            duration: duration,
            quantity: quantity,
            description: $('#timeslot_description').val(),
            category: category,
            restrictedMotions: $('#timeslot_restrictedMotions').val(),
            courtId: this.courtId
        };
    }

    getEventFormData() {
        const evtIdVal = $('#edit_eventId').val();
        const evtId = evtIdVal ? parseInt(evtIdVal) : 0;
        return {
            id: evtId,
            motion: $('#event_motion').val(),
            type: $('#event_type').val(),
            otherMotion: $('#event_otherMotion').val(),
            attorney: $('#event_attorney').val(),
            opposingAttorney: $('#event_opposingAttorney').val(),
            plaintiff: $('#event_plaintiff').val(),
            defendant: $('#event_defendant').val(),
            plaintiffEmail: $('#event_plaintiffEmail').val(),
            defendantEmail: $('#event_defendantEmail').val(),
            notes: $('#event_notes').val(),
            addon: $('#event_addon').val(),
            reminder: $('#event_reminder').val()
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
                    const modal = bootstrap.Modal.getInstance(document.getElementById('EventModal'));
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

    cancelHearing(eventId) {
        $.ajax({
            url: `${this.service.baseUrl}EventAPI/CancelHearing/${eventId}`,
            type: 'GET',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                this.calendar.refetchEvents();
                const modal = bootstrap.Modal.getInstance(document.getElementById('EventModal'));
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
        $('#progress-event').show();

        $.ajax({
            url: getUrl,
            method: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    $('#edit_eventId').val(response.data.id);
                    $('#event_motion').val(response.data.motion);
                    $('#event_type').val(response.data.type);
                    $('#event_otherMotion').val(response.data.otherMotion);
                    $('#event_attorney').val(response.data.attorney);
                    $('#event_opposingAttorney').val(response.data.opposingAttorney);
                    $('#event_plaintiff').val(response.data.plaintiff);
                    $('#event_defendant').val(response.data.defendant);
                    $('#event_plaintiffEmail').val(response.data.plaintiffEmail);
                    $('#event_defendantEmail').val(response.data.defendantEmail);
                    $('#event_notes').val(response.data.notes);
                    $('#event_addon').val(response.data.addon);
                    $('#event_reminder').val(response.data.reminder);
                    $('#event_editedBy').val(response.data.editedBy);
                    $('#event_updatedOn').val(response.data.updatedOn);

                    const modal = new bootstrap.Modal(document.getElementById('EventModal'));
                    modal.show();
                } else {
                    ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                }
                $('#progress-event').hide();
            },
            error: () => {
                ShowNotification('Error', 'Failed to retrieve event details.', 'error');
                $('#progress-event').hide();
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
                    $('#timeslot_startTime').val(response.start.toISOString().slice(0, 16));
                    $('#timeslot_endTime').val(response.end.toISOString().slice(0, 16));
                    $('#timeslot_block').prop('checked', response.block);
                    $('#timeslot_publicBlock').prop('checked', response.publicBlock);
                    $('#timeslot_blockReason').val(response.blockReason);
                    $('#timeslot_concurrent').val(response.concurrent ? 'yes' : 'no');
                    $('#timeslot_duration').val(response.duration);
                    $('#timeslot_quantity').val(response.quantity);
                    $('#timeslot_description').val(response.description);
                    $('#timeslot_category').val(response.category);
                    $('#timeslot_restrictedMotions').val(response.restrictedMotions);

                    const title = this.getDateRangeTitle(new Date(response.start), new Date(response.end));
                    $('#TimeslotModalLabel').text(title);

                    this.loadEventsForTimeslot(timeslotId);

                    const modal = new bootstrap.Modal(document.getElementById('TimeslotModal'));
                    modal.show();
                    $('.nav-tabs li').show();
                    if (response.block) {
                        $('#timeslot_blockReason').closest('.row').show();
                    } else {
                        $('#timeslot_blockReason').closest('.row').hide();
                    }
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
                        $('#event_motion').val(e.motion);
                        $('#event_type').val(e.type);
                        $('#event_otherMotion').val(e.otherMotion);
                        $('#event_attorney').val(e.attorney);
                        $('#event_opposingAttorney').val(e.opposingAttorney);
                        $('#event_plaintiff').val(e.plaintiff);
                        $('#event_defendant').val(e.defendant);
                        $('#event_plaintiffEmail').val(e.plaintiffEmail);
                        $('#event_defendantEmail').val(e.defendantEmail);
                        $('#event_notes').val(e.notes);
                        $('#event_addon').val(e.addon);
                        $('#event_reminder').val(e.reminder);
                        $('#event_editedBy').val(e.editedBy || '');
                        $('#event_updatedOn').val(e.updatedOn || '');
                    }
                    response.data.forEach(e => {
                        const row = `
                            <tr>
                                <td>${e.caseNumber || ''}</td>
                                <td>${e.motion}</td>
                                <td>${e.attorney}</td>
                                <td>${e.plaintiff}</td>
                                <td>${e.opposingAttorney}</td>
                                <td>${e.defendant}</td>
                                <td>
                                    <a href="#" class="editEventBtn" data-id="${e.id}"><i class="fas fa-edit"></i></a>
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
                        // implement delete event
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
                                // call delete event API
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
        $('#timeslot_concurrent').val('yes');
        $('#timeslot_concurrent').closest('.row').hide();
        $('#timeslot_duration').val('15');
        $('#timeslot_quantity').val('1');
        $('#timeslot_description').val('Looks good!');
        $('#timeslot_category').val('');
        $('#timeslot_restrictedMotions').val('');
        $('#timeslot_blockReason').closest('.row').hide();
    }

    clearEventForm() {
        $('#edit_eventId').val('');
        $('#event_motion').val('');
        $('#event_type').val('inperson');
        $('#event_otherMotion').val('');
        $('#event_attorney').val('');
        $('#event_opposingAttorney').val('');
        $('#event_plaintiff').val('');
        $('#event_defendant').val('');
        $('#event_plaintiffEmail').val('');
        $('#event_defendantEmail').val('');
        $('#event_notes').val('');
        $('#event_addon').val('');
        $('#event_reminder').val('');
        $('#event_editedBy').val('');
        $('#event_updatedOn').val('');
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'TimeslotModal') {
            courtCalendarControllerInstance.clearTimeslotForm();
            courtCalendarControllerInstance.clearEventForm();
            $('#eventsTableBody').empty();
        } else if (modalId === 'EventModal') {
            courtCalendarControllerInstance.clearEventForm();
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
}