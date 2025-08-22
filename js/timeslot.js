let timeslotControllerInstance = null;
class TimeslotController {
    constructor(options) {
        this.moduleId = options.moduleId || -1;
        this.userId = options.userId || -1;
        this.isAdmin = options.isAdmin === "True";
        this.adminRole = options.adminRole || 'Admin';
        this.editUrl = options.editUrl;
        this.service = options.service || null;
        this.currentPage = options.currentPage || 0;
        this.pageSize = options.pageSize || 25;
        this.recordCount = options.recordCount || 0;
        this.sortColumnIndex = options.sortColumnIndex || 1;
        this.sortDirection = options.sortDirection || 'asc';
        this.startDate = '';
        this.endDate = '';
        this.table = null;
        timeslotControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        if (this.isAdmin) {
            this.userId = 0; // Admins can view all timeslots, so set userId to 0
        }
        const baseUrl = this.service.baseUrl;
        this.initFilters(baseUrl);
        this.initDateRangePicker();
        this.initTable(baseUrl);
        this.bindEvents();
        this.toggleRemoveFilters();
    }

    initFilters(baseUrl) {
        const initializeSelect2 = (selector, url, placeholder) => {
            $(selector).select2({
                ajax: {
                    url: url,
                    dataType: 'json',
                    delay: 500,
                    data: params => ({
                        q: params.term,
                        page: params.page
                    }),
                    beforeSend: xhr => this.setAjaxHeaders(xhr),
                    processResults: response => {
                        if (response.data) {
                            return {
                                results: response.data.map(item => ({
                                    id: item.Key,
                                    text: item.Value
                                }))
                            };
                        }
                        return { results: [] };
                    },
                    error: () => {
                        ShowNotification("Error", `Failed to retrieve records for ${selector}. Please try again later.`, 'error');
                    },
                    cache: false
                },
                placeholder: placeholder,
                allowClear: true,
                theme: "bootstrap-5",
                minimumInputLength: 0,
                dropdownParent: $(selector).closest('.dropdown-menu'),
                width: '100%'
            });
        };

        initializeSelect2("#courtFilter", `${baseUrl}CourtAPI/GetCourtDropDownItems/`, "-");
    }

    initDateRangePicker() {
        $('#dateRange').daterangepicker({
            opens: 'center',
            showDropdowns: false,
            locale: {
                format: 'MM/DD/YYYY',
                cancelLabel: 'Clear'
            },
            ranges: {
                'Today': [moment(), moment()],
                'Tomorrow': [moment().add(1, 'days'), moment().add(1, 'days')],
                'Next 7 Days': [moment(), moment().add(6, 'days')],
                'Next 30 Days': [moment(), moment().add(29, 'days')],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Next Month': [moment().add(1, 'month').startOf('month'), moment().add(1, 'month').endOf('month')]
            }
        });

        // Set default to next 30 days
        var start_Date = moment().add(1, 'days').format('YYYY-MM-DD 00:00:00');
        var end_Date = moment().add(30, 'days').format('YYYY-MM-DD 00:00:00');
        $('#dateRange').data('daterangepicker').setStartDate(moment(start_Date));
        $('#dateRange').data('daterangepicker').setEndDate(moment(end_Date));
        $('#dateRange').val(moment(start_Date).format('MM/DD/YYYY') + ' - ' + moment(end_Date).format('MM/DD/YYYY'));

        $('#dateRange').on('apply.daterangepicker', (ev, picker) => {
            this.startDate = picker.startDate.format('YYYY-MM-DD 00:00:00');
            this.endDate = picker.endDate.format('YYYY-MM-DD 00:00:00');
            this.table.ajax.reload();
            this.toggleRemoveFilters();
        });

        $('#dateRange').on('cancel.daterangepicker', () => {
            this.startDate = '';
            this.endDate = '';
            $('#dateRange').val('');
            this.table.ajax.reload();
            this.toggleRemoveFilters();
        });
    }

    initTable(baseUrl) {
        const listUrl = `${baseUrl}TimeslotAPI/GetTimeslots/${this.recordCount}`;
        this.table = $("#tblTimeslot").DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                data: data => ({
                    userId: timeslotControllerInstance.userId,
                    courtId: $("#courtFilter").val(),
                    start_date: timeslotControllerInstance.startDate,
                    end_date: timeslotControllerInstance.endDate,
                    searchText: data.search?.value || '',
                    draw: data.draw,
                    start: data.start,
                    length: data.length
                }),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: (xhr, status, error) => {
                    ShowNotification("Error Retrieving Timeslots", error || "Failed to load timeslots.", 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: data => `<a href="${this.editUrl}/sid/${data}" class="btn-command edit-timeslot"><i class="fas fa-pencil"></i></a>`,
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                { data: "court_name" },
                { data: "FormattedStart" },
                { data: "duration" },
                { data: "available", render: function (data) { return data ? 'Yes' : 'No' } },
                { data: "quantity" },
            ],
            order: [[this.sortColumnIndex, this.sortDirection]],
            pageLength: this.pageSize,
            lengthMenu: [25, 50, 100, 200, 300],
            layout: {
                bottomStart: {
                    pageLength: {
                        menu: [25, 50, 100, 200, 300]
                    }
                },
                bottomEnd: 'paging'
            },
            info: false,
            responsive: true
        });
    }

    toggleRemoveFilters() {
        const hasFilter = !!$("#courtFilter").val() || !!this.startDate || !!this.endDate;
        $("#removeFilters").toggle(hasFilter);
    }

    bindEvents() {
        $("#courtFilter").on("change", () => {
            this.table.ajax.reload();
            this.toggleRemoveFilters();
        });

        $("#removeFilters").on("click", () => {
            $("#courtFilter").val("").trigger("change");
            $('#dateRange').val('');
            this.startDate = '';
            this.endDate = '';
            this.toggleRemoveFilters();
            this.table.ajax.reload();
        });

        $('.nav-item.dropdown').each((index, element) => {
            const $navItem = $(element);
            const $navLink = $navItem.find('.nav-link.dropdown-toggle');
            const $dropdownMenu = $navItem.find('.dropdown-menu');
            const $select = $navItem.find('select.select2');

            $navLink.on('click', e => {
                e.preventDefault();
                e.stopPropagation();

                // Close other dropdowns
                $('.nav-item.dropdown').not($navItem).removeClass('show');
                $('.dropdown-menu').not($dropdownMenu).removeClass('show');
                $('select.select2').not($select).select2('close');

                $dropdownMenu.addClass('show');
                $navItem.addClass('show');

                if ($select.length) {
                    $select.select2('open');
                }

                const $dateInput = $navItem.find('#dateRange');
                if ($dateInput.length) {
                    setTimeout(() => {
                        $dateInput.click();
                    }, 0);
                }

                $dropdownMenu.off('click').on('click', e => e.stopPropagation());
            });
        });

        $('select.select2').on('select2:select select2:clear', function () {
            const $select = $(this);
            const $dropdownMenu = $select.closest('.dropdown-menu');
            const $navItem = $dropdownMenu.closest('.nav-item.dropdown');
            $dropdownMenu.removeClass('show');
            $navItem.removeClass('show');
        });

        $(document).on('click', e => {
            if (!$(e.target).closest('.nav-item.dropdown').length) {
                $('.dropdown-menu').removeClass('show');
                $('.nav-item.dropdown').removeClass('show');
                $('select.select2').select2('close');
            }
        });
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}