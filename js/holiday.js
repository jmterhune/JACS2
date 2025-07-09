let holidayControllerInstance = null;

class HolidayController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.holidayId = -1;
        this.searchTerm = "";
        this.holidayTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        holidayControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}HolidayAPI/DeleteHoliday/`;

        const listUrl = `${this.service.baseUrl}HolidayAPI/GetHolidays/${this.recordCount}`;
        const detailModalElement = document.getElementById('HolidayDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('HolidayEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.holidayTable = $('#tblHoliday').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                data(data) {
                    data.searchText = data.search.value;
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblHoliday_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Holidays', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Holidays', 'The following error occurred attempting to retrieve holiday information. Error: ' + error.statusText, 'error');
                    }
                }

            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="hol-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Holiday" data-toggle="tooltip" data-id="${data}" class="hol-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "date",
                    render: function (data) {
                        return data ? new Date(data).toLocaleDateString() : '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Holiday" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.holidayTable.on('draw', function () {

            $(".delete").on("click", function (e) {
                e.preventDefault();
                const holidayId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Holiday?',
                    text: 'Are you sure you wish to delete this Holiday?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        holidayControllerInstance.DeleteHoliday(holidayId);
                    }
                });
            });
        });

        $(document).on('click', '.hol-detail', function (e) {
            e.preventDefault();
            var holidayId = $(this).data("id");
            holidayControllerInstance.ViewHoliday(holidayId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('HolidayEditModal'));
        $(document).on('click', '.hol-edit, #editHolidayBtn', function (e) {
            e.preventDefault();
            var holidayId = $(this).data("id") || $("#hdHolidayId").val();
            holidayControllerInstance.holidayId = holidayId;
            if (holidayId) {
                holidayControllerInstance.ViewHoliday(holidayId, true);
                $("#HolidayEditModalLabel").html(`Edit Holiday`);
            } else {
                holidayControllerInstance.ClearEditForm();
                $("#HolidayEditModalLabel").html("Create New Holiday");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            holidayControllerInstance.ClearEditForm();
            $("#HolidayEditModalLabel").html("Create New Holiday");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var holidayId = $("#hdHolidayId").val();
            Swal.fire({
                title: 'Delete Holiday?',
                text: 'Are you sure you wish to delete this Holiday?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    holidayControllerInstance.DeleteHoliday(holidayId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $holName = $("#edit_holName");
            const $holNameError = $holName.next(".invalid-feedback");
            if ($holName.val().trim() === "") {
                $holNameError.show();
                $holName.addClass("is-invalid");
                isValid = false;
            } else {
                $holNameError.hide();
                $holName.removeClass("is-invalid");
            }

            const $holDate = $("#edit_holDate");
            const $holDateError = $holDate.next(".invalid-feedback");
            if ($holDate.val().trim() === "") {
                $holDateError.show();
                $holDate.addClass("is-invalid");
                isValid = false;
            } else {
                $holDateError.hide();
                $holDate.removeClass("is-invalid");
            }

            if (isValid) {
                holidayControllerInstance.SaveHoliday();
            }
        });

        $("#edit_holName, #edit_holDate").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.holidayTable) {
            this.holidayTable.state.clear();
            window.location.reload();
        }
    }

    DeleteHoliday(holidayId) {
        $.ajax({
            url: this.deleteUrl + holidayId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (holidayControllerInstance.holidayTable) {
                    holidayControllerInstance.holidayTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('HolidayEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('HolidayDetailModal'));
                if (detailModal) {
                    detailModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Holiday deleted successfully.'
                });
            },
            error: function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error Deleting Holiday',
                    text: error.statusText
                });
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'HolidayDetailModal') {
            holidayControllerInstance.ClearDetailForm();
        } else if (modalId === 'HolidayEditModal') {
            holidayControllerInstance.ClearEditForm();
            holidayControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#holName").html("");
        $("#holDate").html("");
        $("#hdHolidayId").val("");
    }

    ClearEditForm() {
        $("#edit_holName").val("");
        $("#edit_holDate").val("");
        $("#edit_hdHolidayId").val("");
    }

    ClearEditValidations() {
        $("#edit_holName, #edit_holDate").removeClass("is-invalid");
        $("#edit_holName, #edit_holDate").next(".invalid-feedback").hide();
    }

    ViewHoliday(holidayId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}HolidayAPI/GetHoliday/${holidayId}`;
        const progressId = isEditMode ? "#edit_progress-holiday" : "#progress-holiday";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('HolidayDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (holidayId) {
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
                            $("#edit_hdHolidayId").val(response.data.id);
                            $("#edit_holName").val(response.data.name);
                            $("#edit_holDate").val(new Date(response.data.date).toISOString().split('T')[0]);
                            $("#HolidayEditModalLabel").html(`Edit Holiday: ${response.data.name}`);
                        } else {
                            $("#holName").html(response.data.name);
                            $("#holDate").html(new Date(response.data.date).toLocaleDateString());
                            $("#hdHolidayId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', 'Failed to retrieve holiday details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch holiday details');
                    ShowNotification('Error', 'Failed to retrieve holiday details. Please try again later.', 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveHoliday() {
        if ($("#edit_hdHolidayId").val() === "") {
            this.CreateHoliday();
        } else {
            this.UpdateHoliday();
        }
        if (holidayControllerInstance.holidayTable) {
            holidayControllerInstance.ClearEditForm();
            holidayControllerInstance.holidayTable.draw();
        }
    }

    CreateHoliday() {
        try {
            $("#edit_progress-holiday").show();
            const holidayData = {
                name: $("#edit_holName").val(),
                date: $("#edit_holDate").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}HolidayAPI/CreateHoliday`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(holidayData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-holiday").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Holiday created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('HolidayEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-holiday").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-holiday").hide();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Creating Holiday',
                        text: error.statusText
                    });
                }
            });
        } catch (e) {
            $("#edit_progress-holiday").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Creating Holiday',
                text: e.statusText
            });
        }
    }

    UpdateHoliday() {
        try {
            $("#edit_progress-holiday").show();
            const holidayData = {
                id: $("#edit_hdHolidayId").val(),
                name: $("#edit_holName").val(),
                date: $("#edit_holDate").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}HolidayAPI/UpdateHoliday`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(holidayData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-holiday").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Holiday updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('HolidayEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-holiday").hide();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Unexpected Error: Status=' + result
                        });
                    }
                },
                error: function (error) {
                    $("#edit_progress-holiday").hide();
                    Swal.fire({
                        icon: 'error',
                        title: 'Error Updating Holiday',
                        text: error.statusText
                    });
                }
            });
        } catch (e) {
            $("#edit_progress-holiday").hide();
            Swal.fire({
                icon: 'error',
                title: 'Error Updating Holiday',
                text: e.statusText
            });
        }
    }
}