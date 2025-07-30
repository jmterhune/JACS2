let templateConfigControllerInstance = null;

class TemplateConfigController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.templateId = params.templateId || -1;
        this.courtId = params.courtId || -1;
        this.service = params.service || null;
        this.calendar = null;
        this.multi_timeslots = [];
        this.dragEvents = [];
        templateConfigControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.initCalendar();
        this.bindEventHandlers();
        this.populateCategorySelect();
        this.initTomSelect();
        const timeslotModalElement = document.getElementById('TimeslotModal');
        if (timeslotModalElement) {
            timeslotModalElement.addEventListener('hidden.bs.modal', this.onModalClose.bind(this));
            timeslotModalElement.addEventListener('shown.bs.modal', this.onModalShow.bind(this));
        }
    }

    initCalendar() {
        const calendarEl = document.getElementById('calendar');
        this.calendar = new FullCalendar.Calendar(calendarEl, {
            headerToolbar: { left: '', center: '', right: 'timeGridWeekCustom,listMonthCustom' },
            initialView: 'timeGridWeekCustom',
            initialDate: '2021-11-01',
            views: {
                timeGridWeekCustom: {
                    type: 'timeGridWeek',
                    dayHeaderFormat: { weekday: 'long', omitCommas: true }
                },
                listMonthCustom: {
                    type: 'listMonth',
                    listDayFormat: { weekday: 'long', omitCommas: true },
                    listDaySideFormat: false
                }
            },
            editable: true,
            selectable: true,
            events: `${this.service.baseUrl}TemplateTimeslotAPI/GetTemplateTimeslots/${this.templateId}`,
            weekends: false,
            slotDuration: '00:05:00',
            slotMinTime: '06:00:00',
            slotMaxTime: '17:30:00',
            allDaySlot: false,
            selectMirror: true,
            select: this.handleDateSelect.bind(this),
            eventClick: this.handleEventClick.bind(this),
            eventResize: this.handleEventResize.bind(this),
            eventDrop: this.handleEventDrop.bind(this),
            eventDragStop: this.handleEventDragStop.bind(this),
            eventConstraint: {
                startTime: '08:00',
                endTime: '17:30',
                daysOfWeek: [1, 2, 3, 4, 5]
            },
            eventContent: function (arg) {
                let divEl = document.createElement('div');
                let timeEl = document.createElement('span');
                timeEl.innerHTML = arg.timeText + `<input style="top:.8rem;width:.95rem;height:.95rem;" class="m-1 float-right" disabled type="checkbox" id="cb${arg.event.id}" value="${arg.event.id}"/>`;
                let contentEl = document.createElement('div');
                contentEl.innerHTML = arg.event.extendedProps.total_length === "5 minutes" ? ' -- ' + arg.event.title : arg.event.title;
                divEl.appendChild(timeEl);
                divEl.appendChild(contentEl);
                return { domNodes: [divEl] };
            }
        });
        this.calendar.render();
    }

    bindEventHandlers() {
        $('#multiDeleteBtn').on('click', this.handleMultiDeleteTimeslots.bind(this));
        $('#multiCopyBtn').on('click', this.handleMultiCopyTimeslots.bind(this));
        $('#saveTimeslotBtn').on('click', this.handleFormSubmit.bind(this));
        $('#deleteTimeslotBtn').on('click', this.handleDeleteTimeslot.bind(this));
        $('#blocked').on('change', (e) => {
            $('.public_block').toggle(e.target.checked);
            $('.block_reason').toggle(e.target.checked);
        });
        $('#cattlecall_yes').on('change', () => $('.quantity-group').show());
        $('#cattlecall_no').on('change', () => $('.quantity-group').hide());
        $('#duration').on('change', this.updateQuantity.bind(this));
        $('#timeslot_start').on('change', (e) => {
            const timeStr = e.target.value; // HH:mm format from input type="time"
            if (timeStr) {
                const time = moment(timeStr, 'HH:mm');
                if (time.isValid()) {
                    const baseDate = moment(document.getElementById('t_start').value || '2021-11-01');
                    document.getElementById('t_start').value = baseDate.set({
                        hour: time.hour(),
                        minute: time.minute(),
                        second: 0
                    }).format('YYYY-MM-DD HH:mm:ss');
                    console.log(`Start time updated: ${document.getElementById('t_start').value}`);
                    this.updateQuantity();
                } else {
                    console.warn(`Invalid start time input: ${timeStr}`);
                }
            }
        });
        $('#timeslot_end').on('change', (e) => {
            const timeStr = e.target.value; // HH:mm format from input type="time"
            if (timeStr) {
                const time = moment(timeStr, 'HH:mm');
                if (time.isValid()) {
                    const baseDate = moment(document.getElementById('t_end').value || '2021-11-01');
                    document.getElementById('t_end').value = baseDate.set({
                        hour: time.hour(),
                        minute: time.minute(),
                        second: 0
                    }).format('YYYY-MM-DD HH:mm:ss');
                    console.log(`End time updated: ${document.getElementById('t_end').value}`);
                    this.updateQuantity();
                } else {
                    console.warn(`Invalid end time input: ${timeStr}`);
                }
            }
        });
    }

    initTomSelect() {
        $.ajax({
            url: `${this.service.baseUrl}CourtMotionAPI/GetCourtMotionDropDownItems/${this.courtId}/true`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                if (response.data) {
                    new TomSelect('#timeslot_motions', {
                        options: response.data.map(item => ({ value: item.Key, text: item.Value })),
                        persist: false,
                        plugins: { remove_button: { title: 'Remove this item' } }
                    });
                }
            },
            error: error => ShowNotification('Error Loading Motions', error.statusText, 'error')
        });
    }

    populateCategorySelect() {
        $.ajax({
            url: `${this.service.baseUrl}CategoryAPI/GetCategoryDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => {
                const select = $('#category_id');
                select.empty().append('<option value=""> - </option>');
                if (response.data) {
                    response.data.forEach(item => {
                        select.append(`<option value="${item.Key}">${item.Value}</option>`);
                    });
                }
            },
            error: error => ShowNotification('Error Loading Categories', error.statusText, 'error')
        });
    }

    handleDateSelect(info) {
        this.resetForm();
        const startDate = moment(info.start);
        const endDate = moment(info.end);
        document.getElementById('t_start').value = startDate.format('YYYY-MM-DD HH:mm:ss');
        document.getElementById('t_end').value = endDate.format('YYYY-MM-DD HH:mm:ss');
        $('#timeslot_start').val(startDate.format('HH:mm'));
        $('#timeslot_end').val(endDate.format('HH:mm'));
        document.getElementById('TimeslotModalLabel').textContent = `${startDate.format('dddd @ h:mm a')} - ${endDate.format('h:mm a')}`;
        $('.quantity-group, .cattle-call, .time-selection').show();
        $('.delete-button').hide();
        if (info.allDay) {
            $('.cattle-call, .time-selection').hide();
            $('#duration, #quantity').removeAttr('required');
        }
        $('#timeslotForm').attr('data-action', `${this.service.baseUrl}TemplateTimeslotAPI/CreateTemplateTimeslot`);
        new bootstrap.Modal(document.getElementById('TimeslotModal')).show();
    }

    handleEventClick(info) {
        if (info.jsEvent.ctrlKey) {
            const checkbox = info.el.querySelector('.m-1.float-right');
            if (checkbox) {
                checkbox.checked = !checkbox.checked;
                if (checkbox.checked) {
                    this.multi_timeslots.push(info.event.id);
                } else {
                    this.multi_timeslots = this.multi_timeslots.filter(id => id !== info.event.id);
                }
            }
        } else {
            this.editTimeslot(info);
        }
    }

    handleEventResize(info) {
        this.updateTimeslot(info);
    }

    handleEventDrop(info) {
        const oldTime = moment(info.oldEvent.start);
        const difference = moment(info.event.start).diff(oldTime);
        const index = this.dragEvents.indexOf(info.event.id);
        this.dragEvents.splice(index, 1);

        this.dragEvents.forEach(id => {
            let event = this.calendar.getEventById(id);
            let newStart = moment(event.start).add(difference);
            let newEnd = moment(event.end).add(difference);
            if (newStart.day() > 5 || newEnd.day() > 5) {
                newStart.day(5);
                newEnd.day(5);
            }
            if (newStart.day() < 1 || newEnd.day() < 1) {
                newStart.day(1);
                newEnd.day(1);
            }
            if (newStart.hour() < 8) {
                newStart.hour(8).minute(0);
            }
            if (newEnd.hour() > 17) {
                newEnd.hour(17).minute(0);
            }
            event.setDates(newStart.format(), newEnd.format());
            this.updateMoveTimeslot(event);
        });

        this.updateTimeslot(info);
    }

    handleEventDragStop(info) {
        $('#calendar input:checked').each((i, el) => this.dragEvents.push($(el).val()));
    }

    handleMultiDeleteTimeslots(e) {
        e.preventDefault();
        if (this.multi_timeslots.length === 0) {
            ShowNotification('No Timeslots Selected', 'Please select at least one timeslot to delete.', 'warning');
            return;
        }
        Swal.fire({
            title: 'Delete Timeslots?',
            text: 'Are you sure you wish to delete the selected timeslots?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${this.service.baseUrl}TemplateTimeslotAPI/DeleteMultiple`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(this.multi_timeslots),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    success: () => {
                        this.multi_timeslots = [];
                        this.dragEvents = [];
                        this.calendar.refetchEvents();
                        ShowNotification('Success', 'Timeslots deleted successfully.', 'success');
                    },
                    error: error => ShowNotification('Error Deleting Timeslots', error.statusText, 'error')
                });
            }
        });
    }

    handleMultiCopyTimeslots(e) {
        e.preventDefault();
        if (this.multi_timeslots.length === 0) {
            ShowNotification('No Timeslots Selected', 'Please select at least one timeslot to copy.', 'warning');
            return;
        }
        $.ajax({
            url: `${this.service.baseUrl}TemplateTimeslotAPI/CopyMultiple`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(this.multi_timeslots),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                this.multi_timeslots = [];
                this.dragEvents = [];
                this.calendar.refetchEvents();
                ShowNotification('Success', 'Timeslots copied successfully.', 'success');
            },
            error: error => ShowNotification('Error Copying Timeslots', error.statusText, 'error')
        });
    }

    handleFormSubmit(e) {
        e.preventDefault();
        if (this.validateForm()) {
            const formData = this.getFormData();
            const isConcurrent = $('[name="cattlecall"]:checked').val() === '1';
            const url = $('#timeslotForm').attr('data-action');
            const method = formData.id > 0 ? 'POST' : 'POST';
            $.ajax({
                url: url,
                type: method,
                contentType: 'application/json',
                data: JSON.stringify(formData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: response => {
                    if (response.status === 200) {
                        this.calendar.refetchEvents();
                        bootstrap.Modal.getInstance(document.getElementById('TimeslotModal')).hide();
                        ShowNotification('Success', response.message || 'Timeslot saved successfully.', 'success');
                    } else {
                        ShowNotification('Error', response.message || 'Failed to save timeslot.', 'error');
                    }
                },
                error: error => ShowNotification('Error Saving Timeslot', error.statusText, 'error')
            });
        }
    }

    handleDeleteTimeslot(e) {
        e.preventDefault();
        const timeslotId = parseInt($('#edit_timeslotId').val());
        if (timeslotId > 0) {
            Swal.fire({
                title: 'Delete Timeslot?',
                text: 'Are you sure you wish to delete this timeslot?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then(result => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: `${this.service.baseUrl}TemplateTimeslotAPI/DeleteTemplateTimeslot/${timeslotId}`,
                        type: 'GET',
                        beforeSend: xhr => this.setAjaxHeaders(xhr),
                        success: () => {
                            this.calendar.refetchEvents();
                            bootstrap.Modal.getInstance(document.getElementById('TimeslotModal')).hide();
                            ShowNotification('Success', 'Timeslot deleted successfully.', 'success');
                        },
                        error: error => ShowNotification('Error Deleting Timeslot', error.statusText, 'error')
                    });
                }
            });
        }
    }

    validateForm() {
        let isValid = true;
        const $start = $('#timeslot_start');
        const startVal = $start.val();
        if (!startVal || !moment(startVal, 'HH:mm', true).isValid()) {
            $start.addClass('is-invalid');
            isValid = false;
        } else {
            $start.removeClass('is-invalid');
        }
        const $end = $('#timeslot_end');
        const endVal = $end.val();
        if (!endVal || !moment(endVal, 'HH:mm', true).isValid()) {
            $end.addClass('is-invalid');
            isValid = false;
        } else {
            $end.removeClass('is-invalid');
        }
        const $duration = $('#duration');
        if (!$duration.val() && $('.time-selection').is(':visible')) {
            $duration.addClass('is-invalid');
            isValid = false;
        } else {
            $duration.removeClass('is-invalid');
        }
        const $quantity = $('#quantity');
        if ($quantity.val() < 1 && $('.quantity-group').is(':visible')) {
            $quantity.addClass('is-invalid');
            isValid = false;
        } else {
            $quantity.removeClass('is-invalid');
        }
        const $blockReason = $('#block_reason');
        if ($('#blocked').is(':checked') && !$blockReason.val()) {
            $blockReason.addClass('is-invalid');
            isValid = false;
        } else {
            $blockReason.removeClass('is-invalid');
        }
        return isValid;
    }

    getFormData() {
        const $form = $('#timeslotForm');
        const tomSelect = $('#timeslot_motions')[0].tomselect;
        return {
            id: parseInt($form.find('#edit_timeslotId').val()) || 0,
            court_template_id: parseInt($form.find('[name="template_id"]').val()),
            start: $form.find('#t_start').val(),
            end: $form.find('#t_end').val(),
            day: moment($form.find('#t_start').val()).isoWeekday(),
            duration: parseInt($form.find('#duration').val()) || 0,
            quantity: parseInt($form.find('#quantity').val()) || 0,
            allDay: false,
            blocked: $form.find('#blocked').is(':checked'),
            public_block: $form.find('#public_block').is(':checked'),
            block_reason: $form.find('#block_reason').val(),
            description: $form.find('#description').val(),
            category_id: parseInt($form.find('#category_id').val()) || null,
            timeslot_motions: tomSelect ? tomSelect.getValue().map(id => parseInt(id)) : []
        };
    }

    editTimeslot(info) {
        $.ajax({
            url: info.event.extendedProps.edit_url,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: data => {
                $('#edit_timeslotId').val(data.id);
                $('#t_start').val(data.start);
                $('#t_end').val(data.end);
                $('#timeslot_start').val(moment(data.start).format('HH:mm'));
                $('#timeslot_end').val(moment(data.end).format('HH:mm'));
                $('#duration').val(data.duration);
                $('#quantity').val(data.quantity);
                $('#description').val(data.description);
                $('#block_reason').val(data.block_reason);
                $('#category_id').val(data.category_id);
                $('#blocked').prop('checked', data.blocked);
                $('#public_block').prop('checked', data.public_block);
                $(`#cattlecall_${data.quantity > 1 ? 'yes' : 'no'}`).prop('checked', true);
                $('.public_block, .block_reason').toggle(data.blocked);
                $('.quantity-group').toggle(data.quantity > 1);
                $('.delete-button').show().attr('onclick', `templateConfigControllerInstance.handleDeleteTimeslot(event)`);
                $('#TimeslotModalLabel').text(`${moment(data.start).format('ddd, h:mm a')} - ${moment(data.end).format('h:mm a')}`);
                const tomSelect = $('#timeslot_motions')[0].tomselect;
                tomSelect.clear();
                data.motions.forEach(m => tomSelect.addItem(m.motion_id));
                $('#timeslotForm').attr('data-action', info.event.extendedProps.update_url);
                new bootstrap.Modal(document.getElementById('TimeslotModal')).show();
            },
            error: error => ShowNotification('Error Loading Timeslot', error.statusText, 'error')
        });
    }

    updateTimeslot(info) {
        const timeslotData = {
            id: parseInt(info.event.id),
            start: moment(info.event.start).format('YYYY-MM-DD HH:mm:ss'),
            end: moment(info.event.end).format('YYYY-MM-DD HH:mm:ss'),
            day: moment(info.event.start).isoWeekday()
        };
        $.ajax({
            url: `${this.service.baseUrl}TemplateTimeslotAPI/UpdateTemplateTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => this.calendar.refetchEvents(),
            error: error => ShowNotification('Error Updating Timeslot', error.statusText, 'error')
        });
    }

    updateMoveTimeslot(event) {
        const timeslotData = {
            id: parseInt(event.id),
            start: moment(event.start).format('YYYY-MM-DD HH:mm:ss'),
            end: moment(event.end).format('YYYY-MM-DD HH:mm:ss'),
            day: moment(event.start).isoWeekday()
        };
        $.ajax({
            url: `${this.service.baseUrl}TemplateTimeslotAPI/UpdateTemplateTimeslot`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(timeslotData),
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: () => {
                this.multi_timeslots = [];
                this.dragEvents = [];
            },
            error: error => ShowNotification('Error Updating Timeslot', error.statusText, 'error')
        });
    }

    updateQuantity() {
        const toTime = moment($('#timeslot_end').val(), 'HH:mm', true);
        const fromTime = moment($('#timeslot_start').val(), 'HH:mm', true);
        if (toTime.isValid() && fromTime.isValid()) {
            const totalMinutes = toTime.diff(fromTime, 'minutes');
            const duration = parseInt($('#duration').val());
            if (duration > 0 && totalMinutes >= 0) {
                $('#quantity').val(Math.floor(totalMinutes / duration));
            } else {
                $('#quantity').val('');
            }
        } else {
            $('#quantity').val('');
        }
    }

    resetForm() {
        const $form = $('#timeslotForm');
        $form.find('input, select').not('[type="hidden"]').val('');
        $('#edit_timeslotId').val('');
        $('#description').val('');
        $('#block_reason').val('');
        $('#category_id').val('');
        $('#cattlecall_yes').prop('checked', true);
        $('.public_block, .block_reason').hide();
        $('.quantity-group, .cattle-call, .time-selection').show();
        $('#duration').attr('required', true);
        $('#quantity').attr('required', true);
        const tomSelect = $('#timeslot_motions')[0].tomselect;
        if (tomSelect) tomSelect.clear();
    }

    onModalShow() {
        $('.public_block').hide();
        $('.block_reason').hide();
    }

    onModalClose() {
        this.resetForm();
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}