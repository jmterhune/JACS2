
$('#create').on('shown.bs.modal', function () {
    $(document).off('focusin.modal');
});
let modal = '#create';
let events = null;
let multi_timeslots = [];

$("#blocked").change(function () {
    if (this.checked) {
        $('.public_block').show();
        $('.block_reason').show();
    } else {
        $('.public_block').hide();
        $('#public_block').prop('checked', false)
        $('.block_reason').hide();
        $('#block_reason').val('');
    }
});

// Timeslot motions select
let timeslotmotions_select = new TomSelect("#timeslot_motions", {
    persist: false,
    plugins: {
        remove_button: {
            title: 'Remove this item',
        },
    },
})

// Javascript Attorney Fetch
let attorney_select = new TomSelect("#attorney", {
    valueField: 'id',
    labelField: 'name',
    plugins: ['clear_button'],
    placeholder: 'Type Bar Number',
    searchField: ['name', 'bar_num'],
    load: function (query, callback) {
        var url = 'http://jacsja.jud12.local/api/attorney?q=' + encodeURIComponent(query);
        fetch(url)
            .then(response => response.json())
            .then(json => {
                callback(json.data);
            }).catch(() => {
                callback();
            });
    },
    render: {
        option: function (item) {
            return `<div> ${item.name} - ${item.bar_num} </div>`;
        },
        item: function (item) {
            return `<div> ${item.name} - ${item.bar_num} </div>`;
        }
    },
    sortField: {
        field: "text",
        direction: "asc",
    }

})

// Javascript Opposing Attorney Fetch
let opp_attorney_select = new TomSelect("#opp_attorney", {
    valueField: 'id',
    labelField: 'name',
    plugins: ['clear_button'],
    placeholder: 'Type Bar Number',
    searchField: ['name', 'bar_num'],
    load: function (query, callback) {
        var url = 'http://jacsja.jud12.local/api/attorney?q=' + encodeURIComponent(query);
        fetch(url)
            .then(response => response.json())
            .then(json => {
                callback(json.data);
            }).catch(() => {
                callback();
            });
    },
    render: {
        option: function (item) {
            return `<div> ${item.name} - ${item.bar_num} </div>`;
        },
        item: function (item) {
            return `<div> ${item.name} - ${item.bar_num} </div>`;
        }
    },
    sortField: {
        field: "text",
        direction: "asc",
    }
});

// Event Form Submit
let event_form = '#newevent';
$(event_form).on('submit', function (event) {
    event.preventDefault();
    $(this).find(':input[type=submit]').prop('disabled', true);

    let url = $(this).attr('data-action');

    $.ajax({
        url: url,
        method: 'POST',
        data: new FormData(this),
        dataType: 'JSON',
        contentType: false,
        cache: false,
        processData: false,
        success: function (response) {
            $(event_form).trigger("reset");
            $(modal).modal('hide');
            $('#newevent input[name=id]').remove()
            $('#newevent input[name=timeslot_id]').remove()
            $(event_form).find(':input[type=submit]').prop('disabled', false);
            var source = calendar.getEventSources();
            source[0].refetch();
        },
        error: function (response) {
            $(event_form).find(':input[type=submit]').prop('disabled', false);
            var errors = response.responseJSON;

            console.log(errors.errors);
            errorsHtml = '<div class="alert alert-danger"><ul>';

            $.each(errors.errors, function (key, value) {
                if (key.includes('plaintiff')) {
                    $('#plaintiff_email').addClass('is-invalid');
                    $('#plaintiff_email_label').addClass('text-danger');
                }
                if (key.includes('defendant')) {
                    $('#defendant_email').addClass('is-invalid');
                    $('#defendant_email_label').addClass('text-danger');
                }
                errorsHtml += '<li>' + value[0] + '</li>'; //showing only the first error.
            });
            errorsHtml += '</ul></div>';

            $('#form-errors').html(errorsHtml);
        }
    });

});

// Timeslot Form Submit Timeslot from Submit
let timeslot_form = '#timeslot';
$(timeslot_form).on('submit', function (event) {
    event.preventDefault();

    var url = $(this).attr('data-action');

    $.ajax({
        url: url,
        method: 'POST',
        data: new FormData(this),
        dataType: 'JSON',
        contentType: false,
        cache: false,
        processData: false,
        success: function (response) {
            $(timeslot_form).trigger("reset");
            $(modal).modal('hide');
            var source = calendar.getEventSources();
            source[0].refetch();
        },
        error: function (response) {
            var errors = response.responseJSON;

            console.log(errors.errors);
            errorsHtml = '<div class="alert alert-danger"><ul>';

            $.each(errors.errors, function (key, value) {
                if (key.includes('t_start')) {
                    $('#timeslot_start_input').addClass('is-invalid');
                    $('#timeslot_start_label').addClass('text-danger');
                }
                if (key.includes('t_end')) {
                    $('#timeslot_end_input').addClass('is-invalid');
                    $('#timeslot_end_label').addClass('text-danger');
                }
                errorsHtml += '<li>' + value[0] + '</li>'; //showing only the first error.
            });
            errorsHtml += '</ul></div>';

            $('#timeslot-errors').html(errorsHtml);
        }
    });
});

function downloadCalendarEvents(courtId) {
    const link = document.createElement('a');
    link.setAttribute('href', "http://jacsja.jud12.local/court/event/calendar/download/" + courtId + "/" + dateConvert($("#cal_from_date").val()) + "/" + dateConvert($("#cal_to_date").val()));
    link.setAttribute('target', "_blank");
    link.click();
}

function downloadCalendarEventsPDF(courtId) {

    const link = document.createElement('a');
    link.setAttribute('href', "http://jacsja.jud12.local/court-timeslots/print/" + courtId + "/" + dateConvert($("#cal_from_date").val()) + "/" + dateConvert($("#cal_to_date").val()));
    link.setAttribute('target', "_blank");
    link.click();
}

function commonDelete() {
    var listarray = new Array();
    var casearray = new Array();
    $('input[name="multiple[]"]:checked').each(function () {
        listarray.push($(this).val());
        casearray.push($(this).attr("data-id") + "  ");
    });
    var checklist = "" + listarray;

    var caseList = casearray.join('<br>');

    if (checklist != '') {
        $("#futureEvents").modal('hide');
        swal.fire({
            title: 'Are you sure?',
            html: "You won't be able to revert this!<br/>" + caseList,
            icon: 'warning',
            input: 'textarea',
            inputLabel: 'Cancellation Reason',
            inputPlaceholder: 'Type your message here...',
            inputAttributes: {
                'aria-label': 'Type your message here'
            },
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, cancel it!',
            customClass: {
                validationMessage: 'my-validation-message'
            },
            preConfirm: (value) => {
                if (!value) {
                    Swal.showValidationMessage(
                        '<i class="fa fa-info-circle"></i> Cancellation reason is required!'
                    )
                }
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    dataType: 'JSON',
                    type: 'post',
                    data: { 'updatelist': checklist, 'cancellation_reason': result.value },
                    url: "http://jacsja.jud12.local/event/future/bulk-delete",
                    success: function () {
                        $(".inline-checkbox:checked").closest('tr').remove();
                        Swal.fire(
                            'Cancelled!',
                            'Your hearing has now been cancelled.',
                            'success'
                        ).then(() => {
                            let source = calendar.getEventSources();
                            source[0].refetch();
                        })
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Something went wrong!',
                        })
                    }
                });
            }
        })
    }
}

$(function () {
    $('#multiAction').click(function () {
        if ($('#multiAction').is(':checked')) {

            $('#multiAction').prop('checked', true);
            $('[name="multiple[]"]').prop('checked', true);
        } else {

            $('#multiAction').prop('checked', false);
            $('[name="multiple[]"]').prop('checked', false);

        }
    });

    $('.inline-checkbox').click(function () {
        var checkedNum = $('input[name="multiple[]"]:checked').length;
        if (checkedNum > 0) {
            $('#event_delete_btn').show();
        } else {
            $('#event_delete_btn').hide();
        }
    });
});

function dateConvert(str) {
    var date = new Date(str),
        mnth = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return [date.getFullYear(), mnth, day].join("-");
}

// Update timeslot time and duration when dragged
function updateTimeslot(e) {
    var url = e.event.extendedProps.update_url;

    $.ajax({
        url: url,
        method: 'PUT',
        data: JSON.stringify({ start: e.event.start, end: e.event.end, court_id: 1 }),
        dataType: 'JSON',
        contentType: "application/json; charset=utf-8",
        cache: false,
        processData: false,
        success: function (response) {
            var source = calendar.getEventSources();
            source[0].refetch();
        },
        error: function (response) {
            console.log('Error');
        }
    });
}

// Update other timeslots on multi move
function updateMoveTimeslot(event) {
    var url = event.extendedProps.update_url;

    $.ajax({
        url: url,
        method: 'PUT',
        data: JSON.stringify({ start: event.start, end: event.end, court_id: 1 }),
        dataType: 'JSON',
        contentType: "application/json; charset=utf-8",
        cache: false,
        processData: false,
        success: function (response) {
            multi_timeslots = [];
            dragEvents = [];
        },
        error: function (response) {
            console.log('Error');
        }
    });
}

function editTimeslot(event) {
    var url = "";
    var e = "";
    if (!isNaN(event)) {
        e = {
            event: {
                id: event,
                extendedProps: {
                    edit_url: 'http://jacsja.jud12.local/timeslot/' + event + "/edit",
                    update_url: 'http://jacsja.jud12.local/timeslot/' + event
                }
            }
        };
        url = e.event.extendedProps.edit_url;
    }
    else {
        e = event;
        url = event.event.extendedProps.edit_url;
    }

    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'JSON',
        success: function (data) {
            $('#newevent input[name=timeslot_id]').remove();
            let start_formatted = moment(data["start"]);
            let end_formatted = moment(data["end"]);
            events = data['events'];

            // Formatting Modal for new data
            $('.public_block').hide();
            $('.block_reason').hide();
            $('.time-selection').show();
            $('.cattle-call').hide();
            $('.delete-button').show();
            $('#modal-title').text(moment(data["start"]).format('ddd MMM D, h:mm a') + ' - ' + moment(data["end"]).format('h:mm a'));
            $('#timeslot_start').datetimepicker('date', start_formatted.format('h:mm a'));
            $('#timeslot_end').datetimepicker('date', end_formatted.format('h:mm a'));

            // Setting Timeslot data within Modal
            $("#quantity").val(data["quantity"]);
            $('#t_start').val(data["start"]);
            $("#t_end").val(data["end"]);
            $("#description").attr('value', data["description"]);
            $("#block_reason").attr('value', data["block_reason"]);
            $('#category option[value="' + data["category_id"] + '"]').prop('selected', true);


            if (jQuery.inArray(data["duration"], [5, 10, 15, 20, 30, 45, 60, 90, 120, 150, 165, 180, 210, 240, 300, 360, 480]) == -1) {
                $('#duration').append($('<option>', {
                    value: data["duration"],
                    text: 'Other (' + data["duration"] + ' mins)'
                }));
            }

            $('#duration option[value="' + data["duration"] + '"]').prop('selected', true);

            data["motions"].forEach(element => {
                if (element.timeslotable_type == "App\\Models\\Timeslot") {
                    timeslotmotions_select.addItem(element.motion_id);
                }
            })

            if (data["blocked"]) {
                $('#blocked').prop('checked', 'checked')
            }
            if (data["public_block"]) {
                $('#public_block').prop('checked', 'checked')
            }


            $('.delete-button').attr('onclick', 'deleteTimeslot(' + e.event.id + ')');

            if (data["events"].length !== 0) {
                $('.delete-button').hide();
            }

            if (data["allDay"]) {
                $('.cattle-call').hide();
                $('.time-selection').hide();
                $('#duration').removeAttr('required');
                $('#quantity').removeAttr('required');
            }
            $('#timeslot-tab-link').tab('show');

            // Setting background data for Event creation
            $('#timeslot').attr('data-action', e.event.extendedProps.update_url);
            $('#timeslot').append('<input type="hidden" id="method" name="_method" value="PUT" />');
            $('#newevent').append('<input type="hidden" name="timeslot_id" value="' + data["id"] + '" />')


            // Singleton event check
            if (data['quantity'] === 1) {
                $('#event-nav').show();
                $('#events-nav').hide();
                $('#event-delete').hide();
                $('#reschedule_button').hide();

                // If event exist, only show event tab in modal
                if (data['events'].length !== 0) {

                    editEvent(0, end_formatted);

                    $('#events-tab').css('display', 'none');
                    //$('#timeslot-tab-link').css('display','none');
                }

            } else {
                $('#event-nav').show();
                $('#event-delete').hide();
                $('#reschedule_button').hide();

                // Fill in table of Events
                if (data["events"].length !== 0) {
                    $('#events-nav').show();
                    // Populating Events tab if there are any
                    let table = document.getElementById("events_table").getElementsByTagName('tbody')[0];
                    data['events'].forEach(function (element, index) {
                        if (element.status_id != 1) {
                            var row = table.insertRow(0);
                            var cell0 = row.insertCell(0);
                            var cell1 = row.insertCell(1);
                            var cell2 = row.insertCell(2);
                            var cell3 = row.insertCell(3);
                            var cell4 = row.insertCell(4);
                            var cell5 = row.insertCell(5);
                            var cell6 = row.insertCell(6);
                            var cell7 = row.insertCell(7);

                            cell0.innerHTML = '<input type="checkbox"  name="multiple[]" class="inline-checkbox" data-id="' + element.case_num + '" value="' + element.id + '">';
                            cell1.innerHTML = element.case_num;
                            cell2.innerHTML = element.motion.description;

                            cell3.innerHTML = element.attorney ? element.attorney.name : '';
                            cell4.innerHTML = element.plaintiff;
                            cell5.innerHTML = element.opp_attorney ? element.opp_attorney.name : '';
                            cell6.innerHTML = element.defendant;
                            cell7.innerHTML = '<a href="#" onclick="editEvent(' + index + ',' + end_formatted + ')"><i class="las la-edit"></i></a>'
                        }
                    })

                    $('.inline-checkbox').click(function () {
                        var checkedNum = $('input[name="multiple[]"]:checked').length;
                        if (checkedNum > 0) {
                            $('#event_delete_btn').show();
                        } else {
                            $('#event_delete_btn').hide();
                        }
                    });
                }
            }

            attorney_select.addOption({
                id: 2,
                name: 'Joe Terhune',
                bar_num: '1028416',
            });
            attorney_select.setValue(2);

            opp_attorney_select.addOption({
                id: 1,
                name: 'Aab, Raymond J',
                bar_num: '1028415',
            });
            opp_attorney_select.setValue(1);

            $("#create").modal('show');
            if (end_formatted >= moment()) {
                if (!1) {
                    $('.past-event').prop('disabled', true);
                } else {
                    $('.past-event').prop('disabled', false);
                }
            } else {
                $('.past-event').prop('disabled', true);
            }

        },
        error: function (response) {
            console.log('Error');
        }
    });
}

// Reschedule Event Function
function rescheduleEvent(e) {

    let url = 'http://jacsja.jud12.local/timeslot-events/' + e.event.id;

    let date = dayjs(e.event.start).format('MM/DD/YYYY - h:mm a');

    let event_id = $('#newevent input[name=id]').val();

    let old_timeslot_id = $('#newevent input[name=timeslot_id]').val();

    Swal.fire({
        title: 'Are you sure?',
        html: "This hearing at <strong> " + $('#create #modal-title').html() + " </strong> is about to be rescheduled to <strong> " + date + " </strong>",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, reschedule it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: 'PUT',
                data: JSON.stringify({ old_timeslot_id: old_timeslot_id, event_id: event_id }),
                dataType: 'JSON',
                contentType: "application/json; charset=utf-8",
                cache: false,
                processData: false,
                success: function () {
                    Swal.fire(
                        'Reschedule!',
                        'Your hearing has now been rescheduled.',
                        'success'
                    ).then(() => {
                        $('#newevent input[name=id]').remove()
                        $('#newevent input[name=timeslot_id]').remove()
                        $('#reschedule').modal('hide');
                        var source = calendar.getEventSources();
                        source[0].refetch();
                    })
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!',
                    })
                }
            });
        }
    })
}

// Display modal on timeslot selection
function setupModal(info) {

    $('.quantity-group').show();
    $('.cattle-call').show();
    $('.time-selection').show();
    $('.public_block').hide();
    $('.block_reason').hide();

    $('#cattlecall_yes').prop('checked', 'checked');


    $('.delete-button').hide();

    $('#event-nav').hide();
    $('#events-nav').hide();


    $("#duration").attr("required", true);
    $("#quantity").attr("required", true);

    $('#modal-title').text(moment(info.start).format('ddd MMM D, h:mm a') + ' - ' + moment(info.end).format('h:mm a'));
    $('#timeslot_start').datetimepicker('date', info.start);
    $('#t_start').val(moment(info.start).format('YYYY-MM-DD HH:mm:ss'));
    $('#timeslot_end').datetimepicker('date', info.end);
    $('#t_end').val(moment(info.end).format('YYYY-MM-DD HH:mm:ss'));

    $('#timeslot').attr('data-action', 'http://jacsja.jud12.local/timeslot');


    if (info.allDay) {
        $('.blocking').css('display', '');
        $('.cattle-call').css('display', 'none');
        $('.time-selection').css('display', 'none');
        $('#duration').removeAttr('required');
        $('#quantity').removeAttr('required');
    }

    // Finally display Modal and set Timesolt as default
    $("#create").modal('show');
    $('#timeslot-tab-link').tab('show');
}

// AJAX Timeslot Deletion Function
function deleteTimeslot(e) {
    let url = 'http://jacsja.jud12.local/timeslot/' + e
    $.ajax({
        url: url,
        method: 'DELETE',
        dataType: 'JSON',
        success: function (data) {
            $("#create").modal('hide');
            let source = calendar.getEventSources();
            source[0].refetch();
        },
        error: function (response) {
            console.log('Error');
        }
    });
}

// AJAX Timeslot Deletion Function
function multiDeleteTimeslot() {
    let url = 'http://jacsja.jud12.local/timeslot/multi'
    $.ajax({
        url: url,
        method: 'DELETE',
        data: JSON.stringify(multi_timeslots),
        contentType: "application/json; charset=utf-8",
        cache: false,
        processData: false,

        dataType: 'JSON',
        success: function (data) {
            $("#create").modal('hide');
            let source = calendar.getEventSources();
            source[0].refetch();
            multi_timeslots = [];
            dragEvents = [];
        },
        error: function (response) {
            console.log('Error');
        }
    });
}

function multiCopyTimeslot() {
    let url = 'http://jacsja.jud12.local/timeslot/copy'
    $.ajax({
        url: url,
        method: 'POST',
        data: JSON.stringify(multi_timeslots),
        contentType: "application/json; charset=utf-8",
        cache: false,
        processData: false,

        dataType: 'JSON',
        success: function (data) {
            $("#create").modal('hide');
            let source = calendar.getEventSources();
            source[0].refetch();
            multi_timeslots = [];
            dragEvents = [];
        },
        error: function (response) {
            console.log('Error');
        }
    });
}

// AJAX Event Deletion Function
function deleteEvent(e) {
    let url = 'http://jacsja.jud12.local/event/' + e

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
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, cancel it!',
        customClass: {
            validationMessage: 'my-validation-message'
        },
        preConfirm: (value) => {
            if (!value) {
                Swal.showValidationMessage(
                    '<i class="fa fa-info-circle"></i> Cancellation reason is required!'
                )
            }
            if (value.length > 255) {
                Swal.showValidationMessage(
                    '<i class="fa fa-info-circle"></i> Cancellation reason is length is too long!'
                )
            }
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: 'DELETE',
                dataType: 'JSON',
                data: result,
                success: function () {
                    Swal.fire(
                        'Cancelled!',
                        'Your hearing has now been cancelled.',
                        'success'
                    ).then(() => {
                        $('#newevent input[name=id]').remove()
                        $('#newevent input[name=timeslot_id]').remove()
                        $("#create").modal('hide');
                        let source = calendar.getEventSources();
                        source[0].refetch();
                    })
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!',
                    })
                }
            });
        }
    })
}

// Reset Form values when modal is Hidden
$('#create').on('hidden.bs.modal', function () {
    $('#timeslot').trigger("reset");
    $('#notes').val('');
    $("#description").attr('value', '');
    $("#block_reason").attr('value', '');

    $('#plaintiff_email').removeClass('is-invalid');
    $('#plaintiff_email_label').removeClass('text-danger');

    $('#defendant_email').removeClass('is-invalid');
    $('#defendant_email_email_label').removeClass('text-danger');

    $('#timeslot_start_input').removeClass('is-invalid');
    $('#timeslot_start_label').removeClass('text-danger');

    $('#timeslot_end_input').removeClass('is-invalid');
    $('#timeslot_end_label').removeClass('text-danger');
    $('#timeslot-errors').hide();

    $('#timeslot #method').remove();
    $("#events_table tbody tr").remove();
    $('#timeslot-tab-link').tab('show')
    $('#event-tab').text('Create Event');
    $('#events-tab').css('display', '');
    $('#timeslot-tab-link').css('display', '');
    $(event_form).trigger("reset");
    attorney_select.clear();
    opp_attorney_select.clear();
    timeslotmotions_select.clear();

    $('#form-errors .alert').remove()
    $('#newevent #method').remove()
    $('#newevent #id').remove()

    $('#otherMotionShow').hide();
    $("#last_updated").hide();
    $('#newevent').attr('data-action', 'http://jacsja.jud12.local/timeslot-events');
})

// Remove Quantity if Cattle call is consecutive
function hideQuantity() {
    $('.quantity-group').hide();
}
function showQuantity() {
    $('.quantity-group').show();
}

// Timeslot Automation
let quantity = $('input[type=number][name=quantity]');
let start = $('input[type=text][name=timeslot_start]');
let end = $('input[type=text][name=timeslot_end]');
let duration = $('select[name=duration]');

end.change(function () {
    updateForm();
});

duration.change(function () {
    updateForm();
});

function updateForm(e) {
    let to_time = moment(end.val(), 'HH:mm A');
    let from_time = moment(start.val(), 'HH:mm A');
    let total_hours = to_time.diff(from_time, 'minutes');

    if (duration.val() !== '') {
        quantity.val(Math.floor(total_hours / duration.val()));
    }

}

// Updating hidden Datetime input for server side
$('#timeslot_start').on("change.datetimepicker", function (e) {
    let time = moment($('#t_start').val());
    let change = moment(e.date);
    time.hour(change.hour());
    time.minutes(change.minutes());
    $('#t_start').val(time.format('YYYY-MM-DD HH:mm:ss'));
})

// Updating hidden Datetime input for server side
$('#timeslot_end').on("change.datetimepicker", function (e) {
    let time = moment($('#t_end').val());
    let change = moment(e.date);
    time.hour(change.hour());
    time.minutes(change.minutes());
    $('#t_end').val(time.format('YYYY-MM-DD HH:mm:ss'));
})

// AJAX Edit Event (Hearing)
function editEvent(id, time) {
    $('#event-tab').tab('show').text('Edit Event');
    $("#case_num").val(events[id]['case_num']);

    var j = 1;
    get_dynamic_case_number_format_fields(events[id]['case_num']);
    $.each(events[id]['case_num'].split('-'), function (index, casevalue) {
        if ($("#case_num_format_multiple" + j).is("select")) {
            $("#case_num_format_multiple" + j + " select").val(casevalue).change();
        }
        else if ($("#case_num_format_multiple" + j).is("input")) {
            $("#case_num_format_multiple" + j).val(casevalue);
        }
        j++;
    })
    $("#notes").val(events[id]['notes']);
    // $("#custom_email_body").html(events[id]['custom_email_body']);
    $("#plaintiff").val(events[id]['plaintiff']);
    $("#otherMotion").val(events[id]['custom_motion']);
    $("#plaintiff_email").val(events[id]['plaintiff_email']);
    $("#defendant").val(events[id]['defendant']);
    $("#plaintiff_email").val(events[id]['plaintiff_email']);
    $("#defendant_email").val(events[id]['defendant_email']);

    $("#last_updated").show();
    var updatedDate = new Date(events[id]['updated_at']);
    var formattedDate = (updatedDate.getMonth() + 1) + '/' + updatedDate.getDate() + '/' + updatedDate.getFullYear();
    var formattedTime = updatedDate.toLocaleTimeString('en-US', { hour: 'numeric', minute: 'numeric', timeZone: 'America/New_York' });
    $("#updated_at").html(formattedDate + ' ' + formattedTime);


    if (events[id]['ownerable'] !== null) {
        $("#updated_by").html(events[id]['ownerable']['name']);
    }

    var j = 0;
    let court_templates = [{ "id": 1, "court_id": "1", "field_name": "Test UDF", "field_type": "TEXT", "alignment": "CENTER", "default_value": "Bill", "required": "1", "yes_answer_required": "0", "display_on_docket": "1", "display_on_schedule": "1", "use_in_attorany_scheduling": "1", "old_id": null, "created_at": "2025-08-25T13:23:09.520000Z", "updated_at": "2025-08-25T13:23:09.520000Z" }];
    $.each(court_templates, function (index, court_template) {

        var key = "";
        var template = "";
        $.each(JSON.parse(events[id]['template']), function (index1, value1) {
            if (index1 === (court_template.field_name + index + "_|" + court_template.alignment + "_|" + court_template.field_type) && court_template.field_type == "yes_no") {
                key = index1;
                template = value1;
                return true;
            } else if (index1 === (court_template.field_name + "_|" + court_template.alignment + "_|" + court_template.field_type)) {
                key = index1;
                template = value1;
                return true;
            }
        });
        if (key != "") {
            let stringArray = key.split("_|");
            if (stringArray[2] == "yes_no") {
                $('input[id=user_customer_field' + index + ']').removeAttr("checked");
                $('input[id=user_customer_field' + index + '][value=' + template + ']').attr('checked', true);
            } else {
                $("#" + key.replace(/([^A-Za-z0-9-])/ig, "")).val(template);
            }
        }
        j++;
    })


    $('#motion option[value="' + events[id]['motion_id'] + '"]').prop('selected', true);
    $('#event_type option[value="' + events[id]['type_id'] + '"]').prop('selected', true);

    // Cancelling/Rescheduling should only be possible if timeslot is greater than or equal to the current time
    if (time >= moment()) {
        $('.past-event').prop('disabled', false);
        $('#event-delete').attr('onclick', 'deleteEvent(' + events[id]['id'] + ')');
        $('#event-delete').show();
        $('#reschedule_button').show();
    } else {
        $('.past-event').prop('disabled', true);
    }

    $('#otherMotionShow').hide();

    if (events[id]['motion_id'] == 221) {
        $('#otherMotionShow').show();
    }

    if (events[id]['attorney'] != null) {
        attorney_select.addOption({
            id: events[id]['attorney_id'],
            name: events[id]['attorney']['name'],
            bar_num: events[id]['attorney']['bar_num'],
        });
        attorney_select.setValue(events[id]['attorney_id']);
    }

    if (events[id]['addon']) {
        $("#addon").prop('checked', true);
    }

    if (events[id]['reminder']) {
        $("#reminder").prop('checked', true);
    }

    if (events[id]['opp_attorney'] != null) {
        opp_attorney_select.addOption({
            id: events[id]['opp_attorney_id'],
            name: events[id]['opp_attorney']['name'],
            bar_num: events[id]['opp_attorney']['bar_num'],
        });
        opp_attorney_select.setValue(events[id]['opp_attorney_id']);
    }

    $('#newevent').attr('data-action', 'http://jacsja.jud12.local/event/' + events[id]['id']);
    $('#newevent').append('<input type="hidden" id="method" name="_method" value="PUT" />');

    if ($('input[name="id"]').val() != null) {
        $('input[name="id"]').val(events[id]['id'])
    } else {
        $('#newevent').append('<input type="hidden" name="id" value="' + events[id]['id'] + '" />')
    }

}

$(document).on("change", ".case_num_format_multiple", function () {
    $case_num = [];
    var allemptylength = $(".case_num_format_multiple").filter(function () {
        $case_num.push(this.value);
        return this.value.length !== 0;
    })
    // console.log("case_num_format_multiple",allemptylength.length)
    if ($('.case_num_format_multiple').length == allemptylength.length) {
        $.ajax({
            data: { case_number: $case_num.join('-') },
            url: "http://jacsja.jud12.local/event/casenum",
            method: 'POST',
            success: function (response) {
                if (response != null) {
                    $("#motion").val(response.motion_id);
                    $("#event_type").val(response.type_id);
                    $("#plaintiff").val(response.plaintiff);
                    $("#plaintiff_email").val(response.plaintiff_email);
                    $("#defendant").val(response.defendant);
                    $("#defendant_email").val(response.defendant_email);
                    $("#notes").val(response.notes);
                    var i = 0;
                    $.each(JSON.parse(response.template), function (index, template) {
                        let stringArray = index.split("_|");
                        if (stringArray[2] == "yes_no") {
                            $('input[id=user_customer_field' + i + '][value=' + template + ']').attr('checked', true);
                        } else {
                            $("#" + index.replace(/[^A-Z0-9]/ig, "")).val(template);
                        }
                        i++;
                    })

                    $('#otherMotionShow').hide();

                    if (response.motion_id == 221) {
                        $('#otherMotionShow').show();
                    }

                    if (response.attorney != null) {
                        attorney_select.addOption({
                            id: response.attorney_id,
                            name: response.attorney.name,
                            bar_num: response.attorney.bar_num,
                        });
                        attorney_select.setValue(response.attorney_id);
                    }
                    if (response.addon) {
                        $("#addon").prop('checked', true);
                    }
                    if (response.reminder) {
                        $("#reminder").prop('checked', true);
                    }

                    if (response.opp_attorney != null) {
                        opp_attorney_select.addOption({
                            id: response.opp_attorney_id,
                            name: response.opp_attorney.name,
                            bar_num: response.opp_attorney.bar_num,
                        });
                        opp_attorney_select.setValue(response.opp_attorney_id);
                    }
                }
            },
            error: function (response) {
                console.log('Error');
            }
        });
    }
});

$('#motion').change(function () {
    $('#otherMotionShow').hide();

    if ($(this).val() == 221) {
        $('#otherMotionShow').show();
    }
});

$('#reschedule_button').on('click', function () {

    let calendar_reschedule = document.getElementById('reschedule-calendar');
    // Reschedule Calendar IO
    reschedule = new FullCalendar.Calendar(calendar_reschedule, {
        initialView: 'listMonth',
        height: 500,
        headerToolbar: {
            left: 'prev,next',
            center: 'title',
            right: ''
        },
        schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
        navLinks: true,
        weekends: false,
        slotDuration: '00:05:00',
        slotMinTime: '09:00:00',
        slotMaxTime: '17:00:00',

        selectMirror: true,
        events: 'http://jacsja.jud12.local/court-timeslots/1',
        eventConstraint: {
            startTime: '09:00',
            endTime: '17:00',
            daysOfWeek: [1, 2, 3, 4, 5]
        },
        eventContent: function (arg) {
            return { html: '<div class="fc-event-main-frame"><div class="fc-event-time"> ' + arg.timeText + '</div><div class="fc-event-title-container"> ' + arg.event.title + '</div> </div>' }
        },
        eventClick: function (info) {
            rescheduleEvent(info);
        },

    });

    let filtered = 'http://jacsja.jud12.local/available-timeslots/1?';

    filtered = filtered.concat('&duration=' + $('#timeslot #duration').val());

    filtered = filtered.concat('&motion=' + $('#newevent #motion').val());

    var source = reschedule.getEventSources();
    source[0].remove();
    reschedule.addEventSource(filtered)

    $('#create').modal('hide');

    $('#reschedule').modal('show');
    reschedule.render();

});

function evaluateformfields($changedfield) {

    var case_format_val = [];
    var case_num = '05--GA--XXXX-XX ';
    var valTokens = case_num.split("-");

    for (var i = 1; i <= valTokens.length; i++) {
        var value = $("#case_num_format_multiple" + i).val();
        case_format_val.push(value);

    }
    $("#case_num").val(case_format_val.join('-'));
}

function changeLabel(courtType) {
    if (courtType == "GA") {
        document.getElementsByClassName("plaintiff_label")[0].innerHTML = "Ward";
        document.getElementsByClassName("plaintiff_email_label")[0].innerHTML = "Ward Email";
        document.getElementsByClassName("defendant_label")[0].innerHTML = "Petitioner";
        document.getElementsByClassName("defendant_email_label")[0].innerHTML = "Petitioner Email";
    }
    else if (courtType == "DR") {
        document.getElementsByClassName("plaintiff_label")[0].innerHTML = "Petitioner";
        document.getElementsByClassName("plaintiff_email_label")[0].innerHTML = "Petitioner Email";
        document.getElementsByClassName("defendant_label")[0].innerHTML = "Respondent";
        document.getElementsByClassName("defendant_email_label")[0].innerHTML = "Respondent Email";
    }
    else if (courtType == "MH") {
        document.getElementsByClassName("plaintiff_label")[0].innerHTML = "Petitioner";
        document.getElementsByClassName("plaintiff_email_label")[0].innerHTML = "Petitioner Email";
        document.getElementsByClassName("defendant_label")[0].innerHTML = "Patient";
        document.getElementsByClassName("defendant_email_label")[0].innerHTML = "Patient Email";
    }
    else {
        document.getElementsByClassName("plaintiff_label")[0].innerHTML = "Plaintiff";
        document.getElementsByClassName("plaintiff_email_label")[0].innerHTML = "Plaintiff Email";
        document.getElementsByClassName("defendant_label")[0].innerHTML = "Defendant";
        document.getElementsByClassName("defendant_email_label")[0].innerHTML = "Defendant Email";
    }
}


function get_dynamic_case_number_format_fields(case_num_format) {
    var fields = '';
    if (case_num_format != null) {
        var format = case_num_format;
        var split_format = format.split('-');
        if (split_format.length == 1) {
            fields = '<label for="case_num">Case Number</label>' +

                '<div class="form-row col-md-12 case-format-row" style="margin:-23px 0px 0px -20px;">' +

                '<div class="col-md-12 mb-3">' +
                '<label for="case_num"></label>' +
                '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple1" required value="' + split_format[0] + '">' +
                '<div class="valid-feedback">' +
                'Looks good!' +
                '</div>' +
                '</div>';

        }
        else if (split_format.length == 2) {

            fields = '<label for="case_num">Case Number</label>' +
                '<div class="form-row col-md-12 case-format-row" style="margin:-23px 0px 0px -20px;">' +
                '<div class="col-md-4 mb-4">' +
                '<label for="case_num"></label>' +
                '<input type="text" class="form-control case_num_format_multiple" maxlength="4" id="case_num_format_multiple1" required value="' + split_format[0] + '">' +
                '<div class="valid-feedback">' +
                'Looks good!' +
                '</div>' +
                '</div>' +
                '<div class="col-md-4 mb-4">' +
                '<label for="case_num"></label>' +
                '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple2" maxlength="7" required value="' + split_format[1] + '">' +
                '<div class="valid-feedback">' +
                'Looks good!' +
                '</div>' +
                '</div>' +
                '</div>';
        }
        else if (split_format.length == 3) {

            if (split_format[1].length == 2 || split_format[1] == 0) {
                fields = '<label for="case_num">Case Number</label>' +
                    '<div class="form-row col-md-12 case-format-row" style="margin:0px 0px 0px -20px;">' +
                    '<div class="col-md-2 mb-2">' +

                    '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple1"  maxlength="4" required value="' + split_format[0] + '">' +
                    '<div class="valid-feedback">' +
                    ' Looks good!' +
                    '</div>' +
                    '</div>' +
                    '<div class="col-md-2 mb-2">' +
                    '<select class="form-control col-md-12 case_num_format_multiple court_type_change_label" id="case_num_format_multiple2" required onChange="changeLabel(this.value);">';
                var court_types = [{ "old_id": "CA" }, { "old_id": "CC" }, { "old_id": "CF" }, { "old_id": "CJ" }, { "old_id": "CM" }, { "old_id": "CO" }, { "old_id": "CP" }, { "old_id": "CT" }, { "old_id": "DP" }, { "old_id": "DR" }, { "old_id": "GA" }, { "old_id": "IN" }, { "old_id": "MH" }, { "old_id": "MM" }, { "old_id": "MO" }, { "old_id": "SC" }, { "old_id": "TR" }];
                $.each(court_types, function (key, court_type) {
                    var selected = (court_type["old_id"] == split_format[1]) ? "selected" : "";
                    fields += '<option value="' + court_type["old_id"] + '"' + selected + '>' + court_type["old_id"] + '</option>';
                })

                fields += '</select>' +
                    '</div>' +
                    '<div class="col-md-2 mb-2">' +
                    '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple3"  maxlength="7" required value="' + split_format[2] + '">' +
                    '<div class="valid-feedback">' +
                    'Looks good!' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
            else {
                fields = '<label for="case_num">Case Number</label>' +
                    '<div class="form-row col-md-12 case-format-row" style="margin:-23px 0px 0px -20px;">' +

                    '<div class="col-md-3 mb-3">' +
                    '<label for="case_num"></label>' +
                    ' <input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple1" maxlength="4" required value="' + split_format[0] + '">' +
                    ' <div class="valid-feedback">' +
                    ' Looks good!' +
                    ' </div>' +
                    ' </div>' +

                    ' <div class="col-md-3 mb-3">' +
                    ' <label for="case_num"></label>' +
                    ' <input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple2" maxlength="7" required value="' + split_format[1] + '">' +
                    ' <div class="valid-feedback">' +
                    ' Looks good!' +
                    ' </div>' +
                    ' </div>' +

                    ' <div class="col-md-3 mb-3">' +
                    '<label for="case_num"></label>' +
                    '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple3"   maxlength="4" required value="' + split_format[2] + '">' +
                    '<div class="valid-feedback">' +
                    ' Looks good!' +
                    ' </div>' +
                    '</div>' +
                    ' </div>';
            }
        }
        else if (split_format.length == 6 || split_format.length == 5 || split_format.length == 4) {
            var input_type = (split_format.length == 6) ? "hidden" : "text";
            fields = '<label for="case_num">Case Number</label>' +
                '<div class="form-row col-md-12 case-format-row" style="margin:-23px 0px 0px -20px;">' +

                '<div class="col-md-1 mb-1">' +
                '<label for="case_num"></label>' +
                ' <input type= "' + input_type + '"  class="form-control case_num_format_multiple" id="case_num_format_multiple1" maxlength="2" required value="' + split_format[0] + '">' +
                ' <div class="valid-feedback">' +
                ' Looks good!' +
                ' </div>' +
                '</div>' +
                '<div class="col-md-2 mb-2">' +
                ' <label for="case_num"></label>' +
                ' <input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple2"  maxlength="4" required value="' + split_format[1] + '" placeholder="Complete Year" style="font-weight: bold">' +
                ' <div class="valid-feedback">' +
                ' Looks good!' +
                ' </div>' +
                '</div>' +
                ' <div class="col-md-2 mb-2">' +
                ' <label for="case_num"></label>' +
                '<select class="form-control col-md-12 case_num_format_multiple court_type_change_label" id="case_num_format_multiple3" required onChange="changeLabel(this.value);">';
            var selected = (split_format[2] == 0) ? "selected" : "";
            var court_types = [{ "old_id": "CA" }, { "old_id": "CC" }, { "old_id": "CF" }, { "old_id": "CJ" }, { "old_id": "CM" }, { "old_id": "CO" }, { "old_id": "CP" }, { "old_id": "CT" }, { "old_id": "DP" }, { "old_id": "DR" }, { "old_id": "GA" }, { "old_id": "IN" }, { "old_id": "MH" }, { "old_id": "MM" }, { "old_id": "MO" }, { "old_id": "SC" }, { "old_id": "TR" }];
            fields += '<option value=""' + selected + '>' + '</option>';
            $.each(court_types, function (key, court_type) {
                selected = (court_type["old_id"] == split_format[2]) ? "selected" : "";
                fields += '<option value="' + court_type["old_id"] + '"' + selected + '>' + court_type["old_id"] + '</option>';
            })

            fields += ' </select>' +
                '</div>' +

                '<div class="col-md-2 mb-2">' +
                '<label for="case_num"></label>' +
                '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple4" maxlength="6" required value="' + split_format[3] + '" placeholder="Case Number" style="font-weight: bold;">' +
                ' <div class="valid-feedback">' +
                'Looks good!' +
                '</div>' +
                '</div>' +
                ' <div class="col-md-2 mb-2">' +
                '<label for="case_num"></label>' +
                '<input type="text" class="form-control case_num_format_multiple" id="case_num_format_multiple5" maxlength="4" required value="' + split_format[4] + '">' +
                '<div class="valid-feedback">' +
                ' Looks good!' +
                '</div>' +
                ' </div>' +
                ' <div class="col-md-1 mb-1">' +
                '<label for="case_num"></label>' +
                '<input type= "' + input_type + '" class="form-control case_num_format_multiple" id="case_num_format_multiple6"  maxlength="2" required value="' + split_format[5] + '">' +
                '<div class="valid-feedback">' +
                ' Looks good!' +
                '</div>' +
                '</div>' +
                '</div>';
        }
    }
    else {
        fields = '<label for="case_num">Case Number</label>' +
            ' <div class="form-row col-md-12 case-format-row" style="margin:-23px 0px 0px -20px;">' +
            '<div class="col-md-4 mb-3">' +

            '<input type="text" class="form-control case_num_format_multiple" required>' +
            '<div class="valid-feedback">' +
            ' Looks good!' +
            '</div>' +
            '</div>' +
            '</div>';
    }

    $(".dynamic_case_number_format").html(fields);

    changeLabel(document.getElementsByClassName("court_type_change_label")[0].value);

    $('.case_num_format_multiple').keyup(function () {
        evaluateformfields($(this));
    });

    $('.case_num_format_multiple').change(function () {
        evaluateformfields($(this));
    });
}
