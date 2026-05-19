let eventControllerInstance = null;
class EventController {
    constructor(options) {
        this.moduleId = options.moduleId || -1;
        this.userId = options.userId || -1;
        this.isAdmin = options.isAdmin == "True" ? true : false || false;
        this.adminRole = options.adminRole || 'Admin';
        this.editUrl = options.editUrl;
        this.calendarUrl = options.calendarUrl;
        this.service = options.service || null;
        this.currentPage = options.currentPage || 0;
        this.pageSize = options.pageSize || 25;
        this.recordCount = options.recordCount || 0;
        this.sortColumnIndex = options.sortColumnIndex || 1;
        this.sortDirection = options.sortDirection || 'asc';
        this.table = null;
        eventControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        if (this.isAdmin) {
            this.userId = 0; // Admins can view all timeslots, so set userId to 0
        }
        const baseUrl = this.service.baseUrl;
        this.initFilters(baseUrl);
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
        initializeSelect2("#courtroomFilter", `${baseUrl}CourtroomAPI/GetCourtroomDropDownItems/`, "-");
        // Status filter populates from event_statuses (Cancelled/Rescheduled/
        // Scheduled/Past), NOT event_types — those are separate concerns.
        initializeSelect2("#statusFilter", `${baseUrl}EventStatusAPI/GetEventStatusDropDownItems/`, "-");
    }

    initTable(baseUrl) {
        const listUrl = `${baseUrl}EventAPI/GetEvents/${this.recordCount}`;
        this.table = $("#tblEvent").DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                // Mutate the DataTables-supplied request object so the
                // serialised `order[0].column` / `order[0].dir` fields survive
                // — returning a brand-new object dropped them, which is why
                // the server fell back to case_num on every header click.
                data: data => {
                    data.userId = eventControllerInstance.userId;
                    data.courtId = $("#courtFilter").val();
                    data.courtroomId = $("#courtroomFilter").val();
                    data.statusId = $("#statusFilter").val();
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                    delete data.search;
                    return data;
                },
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                error: (xhr, status, error) => {
                    ShowNotification("Error Retrieving Events", error || "Failed to load events.", 'error');
                }
            },
            columns: [
                {
                    data: "id",
                    render: (data, type, row) => row.editable ? `<a href="${this.editUrl}/eid/${data}" class="btn-command revisions"><i class="fas fa-pencil" title="Select to Edit Record" ></i></a>` :'<i class="fas fa-ban text-danger" title="You do not have edit permissions for this record"></i>',
                    className: "command-item",
                    orderable: false,
                    searchable: false
                },
                { data: "case_num" },
                { data: "motion_name" },
                { data: "start_formatted" },
                { data: "duration" },
                { data: "court_name" },
                { data: "status_name" },
                { data: "attorney_name" },
                { data: "opp_attorney_name" },
                { data: "plaintiff" },
                { data: "defendant" },
                { data: "courtroom_name" },
            ],
            order: [[this.sortColumnIndex, this.sortDirection]],
            pageLength: this.pageSize,
            lengthMenu: [25, 50, 100, 200, 300],
            dom: '<"row"<"col-sm-12"tr>>' +
                '<"row"<"col-sm-4"l><"col-sm-4 text-center"B><"col-sm-4 float-end"p>>',
            buttons: [
                {
                    extend: 'collection',
                    text: '<i class="fa fa-download"></i> Export',
                    className: 'btn btn-default btn-sm',
                    autoClose: true,
                    buttons: [
                        { extend: 'copy', text: 'Copy', exportOptions: { columns: ':visible' } },
                        { extend: 'csv', text: 'CSV', exportOptions: { columns: ':visible' } },
                        { extend: 'excel', text: 'Excel', exportOptions: { columns: ':visible' } },
                        { extend: 'pdf', text: 'PDF', exportOptions: { columns: ':visible' } },
                        { extend: 'print', text: 'Print', exportOptions: { columns: ':visible' } }
                    ]
                },
                {
                    extend: 'colvis',
                    text: '<i class="fa fa-eye-slash"></i> Column visibility',
                    className: 'btn btn-default btn-sm',
                    columns: ':gt(1)'
                }
            ],
            responsive: true,
            initComplete: () => {
                const $buttonsContainer = this.table.buttons().container();
                $buttonsContainer.addClass('dt-buttons d-xs-block d-sm-inline-block d-md-inline-block d-lg-inline-block');
                const $target = $('#tblEvent_wrapper .row:last .col-sm-4.text-center');
                $buttonsContainer.appendTo($target);
                // Notice that the export buttons only act on the rows currently
                // rendered (server-side paging hides everything else from the
                // browser). Tell the user to bump the page size if they need more.
                $('<small class="d-block text-muted mt-1 export-note">'
                    + 'Only the rows currently shown will be exported. '
                    + 'Increase the page size to export more rows.'
                    + '</small>').appendTo($target);
                const $pagination = $('#tblEvent_wrapper ul.pagination');
                $pagination.addClass('float-end');
            }
        });
    }

    toggleRemoveFilters() {
        const hasFilter = !!$("#courtFilter").val() || !!$("#courtroomFilter").val() || !!$("#statusFilter").val();
        $("#removeFilters").toggle(hasFilter);
    }

    bindEvents() {
        $("#courtFilter, #courtroomFilter, #statusFilter").on("change", () => {
            this.table.ajax.reload();
            this.toggleRemoveFilters();
        });

        $("#removeFilters").on("click", () => {
            $("#courtFilter").val("").trigger("change");
            $("#courtroomFilter").val("").trigger("change");
            $("#statusFilter").val("").trigger("change");
            this.toggleRemoveFilters();
        });

        $('.nav-item.dropdown').each((index, element) => {
            const $navItem = $(element);
            const $navLink = $navItem.find('.nav-link.dropdown-toggle');
            const $dropdownMenu = $navItem.find('.dropdown-menu');
            const $select = $navItem.find('select.select2');

            $navLink.on('click', e => {
                e.preventDefault();
                e.stopPropagation();
                $dropdownMenu.addClass('show');
                $navItem.addClass('show');
                $select.select2('open');
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

    callService(method, data, callback) {
        $.ajax({
            url: this.getServiceUrl(method),
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: response => callback(response),
            error: (xhr, status, error) => {
                ShowNotification("Error Calling Service", error || "Failed to call service.", 'error');
            }
        });
    }

    getServiceUrl(method) {
        return this.service.framework.getServiceRoot(this.service.path) + method;
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}
