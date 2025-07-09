(function ($, window, document, undefined) {
    function EventController(options) {
        this.moduleId = options.moduleId;
        this.userId = options.userId;
        this.isAdmin = options.isAdmin === "True";
        this.adminRole = options.adminRole;
        this.editUrl = options.editUrl;
        this.calendarUrl = options.calendarUrl;
        this.service = options.service;
        this.currentPage = options.currentPage;
        this.pageSize = options.pageSize;
        this.recordCount = options.recordCount;
        this.sortColumnIndex = options.sortColumnIndex;
        this.sortDirection = options.sortDirection;
        this.revisionsUrl = options.revisionsUrl;
        this.table = null;
        eventControllerInstance = this;
    }

    EventController.prototype = {
        init: function () {
            const isAdmin = this.isAdmin;
            this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
            const baseUrl = this.service.baseUrl;
            this.initFilters(baseUrl);
            this.initTable(baseUrl); // Initialize DataTable
            this.bindEvents();
            this.toggleRemoveFilters(); // Check filter visibility on init
        },

        initFilters: function (baseUrl) {
            var self = this;

            // Helper function to initialize Select2 with common settings
            function initializeSelect2(selector, url, placeholder) {
                $(selector).select2({
                    ajax: {
                        url: url,
                        dataType: 'json',
                        delay: 500,
                        data: function (params) {
                            return {
                                q: params.term,
                                page: params.page
                            };
                        },
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("ModuleId", self.moduleId);
                            xhr.setRequestHeader("TabId", self.service.framework.getTabId());
                            xhr.setRequestHeader("RequestVerificationToken", self.service.framework.getAntiForgeryValue());
                        },
                        processResults: function (response) {
                            if (response.data) {
                                return {
                                    results: response.data.map(item => ({
                                        id: item.Key,
                                        text: item.Value
                                    }))
                                };
                            } else {
                                return { results: [] };
                            }
                        },
                        error: function () {
                            console.error(`Failed to fetch dropdown entries for ${selector}`);
                            Swal.fire({
                                icon: "error",
                                title: "Error",
                                text: `Failed to retrieve records for ${selector}. Make sure you are logged in and try again.`
                            });
                        },
                        cache: true
                    },
                    placeholder: placeholder,
                    allowClear: true,
                    theme: "bootstrap-5",
                    minimumInputLength: 0,
                    dropdownParent: $(selector).closest('.dropdown-menu'), // Attach Select2 dropdown to dropdown-menu
                    width: '100%' // Ensure dropdown fits within the menu
                });
            }

            // Initialize Select2 for each filter
            initializeSelect2(
                "#courtFilter",
                `${baseUrl}CourtAPI/GetCourtDropDownItems/`,
                "-"
            );

            initializeSelect2(
                "#categoryFilter",
                `${baseUrl}CategoryAPI/GetCategoryDropDownItems/`,
                "-"
            );

            initializeSelect2(
                "#statusFilter",
                `${baseUrl}EventTypeAPI/GetEventTypeDropDownItems/`,
                "-"
            );
        },

        initTable: function (baseUrl) {
            var self = this;
            const listUrl = `${baseUrl}EventAPI/GetEvents/${eventControllerInstance.recordCount}`;
            this.table = $("#tblEvent").DataTable({
                serverSide: true,
                processing: true,
                ajax: {
                    url: listUrl,
                    type: "GET",
                    datatype: 'json',
                    data(data) {
                        data.courtId = $("#courtFilter").val();
                        data.categoryId = $("#categoryFilter").val();
                        data.statusId = $("#statusFilter").val();
                        data.searchText = data.search.value; // Use DataTables search value
                        delete data.columns; // Remove columns from request
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("ModuleId", self.moduleId);
                        xhr.setRequestHeader("TabId", self.service.framework.getTabId());
                        xhr.setRequestHeader("RequestVerificationToken", self.service.framework.getAntiForgeryValue());
                    },
                    error: function (xhr, status, error) {
                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: "Failed to load events: " + error
                        });
                    }
                },
                columns: [
                    {
                        data: "id",
                        render: function (data) {
                            return '<a href="' + self.editUrl + '/eid/' + data + '" class="btn-command revisions"><i class="fas fa-pencil"></i></a>';
                        },
                        className: "command-item",
                        orderable: false,
                        searchable: false,
                    },
                    { data: "CaseNumber" },
                    { data: "Motion" },
                    { data: "Timeslot" },
                    { data: "Duration" },
                    { data: "Court" },
                    { data: "Status" },
                    { data: "Attorney" },
                    { data: "OpposingAttorney" },
                    { data: "Plaintiff" },
                    { data: "Defendant" },
                    { data: "Category" },
                    {
                        data: "id",
                        orderable: false,
                        searchable: false,
                        render: function (data) {
                            return '<a href="' + self.revisionsUrl + '/eid/' + data + '" class="btn-command revisions"><i class="fas fa-history"></i> Revisions</a>';
                        }
                    }
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
                            {
                                extend: 'copy',
                                text: 'Copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'csv',
                                text: 'CSV',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                text: 'Excel',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                text: 'PDF',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'print',
                                text: 'Print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            }
                        ]
                    },
                    {
                        extend: 'colvis',
                        text: '<i class="fa fa-eye-slash"></i> Column visibility',
                        className: 'btn btn-default btn-sm',
                        columns: ':gt(1)' // Exclude first two columns (expand/collapse, checkbox)
                    }
                ],
                responsive: true,
                initComplete: function () {
                    console.log('DataTable initComplete: Checking buttons container');
                    var $buttonsContainer = self.table.buttons().container();
                    console.log('Buttons container:', $buttonsContainer.length ? 'Found' : 'Not found', $buttonsContainer.html());

                    $buttonsContainer
                        .addClass('dt-buttons d-xs-block d-sm-inline-block d-md-inline-block d-lg-inline-block');

                    var $target = $('#tblEvent_wrapper .row:last .col-sm-4.text-center');
                    console.log('Target container for buttons:', $target.length ? 'Found' : 'Not found');
                    $buttonsContainer.appendTo($target);
                    console.log('Buttons appended. Final container HTML:', $target.html());
                    var $pagination = $('#tblEvent_wrapper ul.pagination');
                    console.log('Pagination UL:', $pagination.length ? 'Found' : 'Not found');
                    $pagination.addClass('float-end');
                    console.log('Pagination UL after adding float-end:', $pagination.hasClass('float-end') ? 'Class added' : 'Class not added');
                }
            });
        },

        toggleRemoveFilters: function () {
            // Check if any filter has a value
            var hasFilter = !!$("#courtFilter").val() || !!$("#categoryFilter").val() || !!$("#statusFilter").val();
            // Show or hide the removeFilters button
            $("#removeFilters").toggle(hasFilter);
        },

        bindEvents: function () {
            var self = this;

            // Handle nav link click to open Select2 dropdown
            $('.nav-item.dropdown').each(function () {
                var $navItem = $(this);
                var $navLink = $navItem.find('.nav-link.dropdown-toggle');
                var $dropdownMenu = $navItem.find('.dropdown-menu');
                var $select = $navItem.find('select.select2');

                // Prevent Bootstrap's default toggle behavior
                $navLink.on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    // Show the dropdown menu
                    $dropdownMenu.addClass('show');
                    $navItem.addClass('show');

                    // Open the Select2 dropdown
                    $select.select2('open');

                    // Ensure the dropdown menu stays open
                    $dropdownMenu.off('click').on('click', function (e) {
                        e.stopPropagation(); // Prevent closing on click inside dropdown
                    });
                });
            });

            // Close dropdown when selecting an option or clearing
            $('select.select2').on('select2:select select2:clear', function () {
                var $select = $(this);
                var $dropdownMenu = $select.closest('.dropdown-menu');
                var $navItem = $dropdownMenu.closest('.nav-item.dropdown');
                $dropdownMenu.removeClass('show');
                $navItem.removeClass('show');
            });

            // Filter change
            $("#courtFilter, #categoryFilter, #statusFilter").on("change", function () {
                self.table.ajax.reload();
                self.toggleRemoveFilters(); // Update visibility on filter change
            });

            // Remove filters
            $("#removeFilters").on("click", function () {
                $("#courtFilter").val("").trigger("change");
                $("#categoryFilter").val("").trigger("change");
                $("#statusFilter").val("").trigger("change");
                self.toggleRemoveFilters(); // Update visibility after clearing
            });

            // Close dropdowns when clicking outside
            $(document).on('click', function (e) {
                if (!$(e.target).closest('.nav-item.dropdown').length) {
                    $('.dropdown-menu').removeClass('show');
                    $('.nav-item.dropdown').removeClass('show');
                    $('select.select2').select2('close');
                }
            });
        },

        callService: function (method, data, callback) {
            var self = this;
            $.ajax({
                url: this.getServiceUrl(method),
                type: "POST",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("ModuleId", self.moduleId);
                    xhr.setRequestHeader("TabId", self.service.framework.getTabId());
                    xhr.setRequestHeader("RequestVerificationToken", self.service.framework.getAntiForgeryValue());
                },
                success: function (response) {
                    callback(response);
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "Failed to call service: " + error
                    });
                }
            });
        },

        getServiceUrl: function (method) {
            return this.service.framework.getServiceRoot(this.service.path) + method;
        }
    };

    window.EventController = EventController;
})(jQuery, window, document);