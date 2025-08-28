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
        this.tempEventId = 'temp-new-timeslot'; // Added to identify temporary event
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
        this.populateCaseTypes();
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

        // Update labels for plaintiff and defendant
        const plaintiffLabel = $('label[for="event_plaintiff"]');
        const defendantLabel = $('label[for="event_defendant"]');
        plaintiffLabel.html(this.courtData.plaintiff_required ? 'Plaintiff <em>*</em>' : 'Plaintiff');
        defendantLabel.html(this.courtData.defendant_required ? 'Defendant <em>*</em>' : 'Defendant');

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
                    this.populateCourtTemplateFields(...(truncated 64768 characters)....deleteEventBtn').on('click', (ev) => {
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
        // Added: Remove temporary event if it exists when modal closes without saving
        const tempEvent = this.calendar.getEventById(this.tempEventId);
        if (tempEvent) {
            tempEvent.remove();
        }
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