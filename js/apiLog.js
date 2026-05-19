let apiLogControllerInstance = null;

class ApiLogController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin == "True";
        this.pageSize = params.pageSize || 25;
        this.service = params.service || null;
        this.table = null;
        apiLogControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);

        // Default the date filters to today so the first page load shows only
        // today's activity instead of the whole log.
        const today = this._todayIso();
        $('#flt_fromDate').val(today);
        $('#flt_toDate').val(today);

        this.populateCounties();
        this.populateActions();

        this.table = $('#tblApiLog').DataTable({
            searching: true,
            autoWidth: false,
            stateSave: false,
            serverSide: true,
            processing: true,
            lengthMenu: [[25, 50, 100, 200], [25, 50, 100, 200]],
            pageLength: this.pageSize,
            order: [[1, 'desc']],
            ajax: {
                url: `${this.service.baseUrl}ApiLogAPI/GetApiLogs/0`,
                type: "GET",
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data: data => {
                    // Pull filter values into the DataTables request
                    data.searchText = data.search?.value || '';
                    data.countyId = $('#flt_countyId').val() || '';
                    data.action = $('#flt_action').val() || '';
                    data.eventId = $('#flt_eventId').val() || '';
                    data.caseId = $('#flt_caseId').val() || '';
                    data.caseNumber = ($('#flt_caseNumber').val() || '').trim();
                    data.fromDate = $('#flt_fromDate').val() || '';
                    // Make toDate inclusive of the whole day
                    const td = $('#flt_toDate').val();
                    data.toDate = td ? td + 'T23:59:59' : '';
                    delete data.columns;
                },
                error: err => {
                    if (err.status === 401) {
                        ShowNotification('Error Retrieving API Log', 'Please sign in and try again.', 'error');
                    } else {
                        ShowNotification('Error Retrieving API Log', 'The following error occurred: ' + err.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "log_id",
                    render: id => `<button type="button" class="apilog-detail btn-command" data-id="${id}" title="View details"><i class="fas fa-eye"></i></button>`,
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "created_at",
                    render: v => v ? new Date(v).toLocaleString() : ''
                },
                { data: "county_name", render: v => v || '' },
                { data: "action", render: v => v || '' },
                { data: "event_id", render: v => v || '' },
                { data: "case_id", render: v => v || '' },
                {
                    data: "api_end_point",
                    render: v => {
                        if (!v) return '';
                        const max = 80;
                        return v.length > max
                            ? `<span title="${$('<div>').text(v).html()}">${$('<div>').text(v.substring(0, max) + '…').html()}</span>`
                            : $('<div>').text(v).html();
                    }
                },
                {
                    data: "error",
                    render: v => v
                        ? `<span class="text-danger" title="${$('<div>').text(v).html()}">ERROR</span>`
                        : `<span class="text-success">OK</span>`,
                    orderable: false
                },
            ],
            language: {
                emptyTable: "No log entries.",
                zeroRecords: "No records match the filters."
            }
        });

        $('#btnApplyFilters').on('click', () => this.table.draw());
        $('#btnResetFilters').on('click', () => {
            // Reset matches the page's initial state — date range defaults to today.
            const today = this._todayIso();
            $('#flt_fromDate').val(today);
            $('#flt_toDate').val(today);
            $('#flt_eventId, #flt_caseId, #flt_caseNumber').val('');
            $('#flt_countyId, #flt_action').val('');
            this.table.draw();
        });

        $(document).on('click', '.apilog-detail', e => {
            e.preventDefault();
            this.showDetail($(e.currentTarget).data('id'));
        });
    }

    populateCounties() {
        $.ajax({
            url: `${this.service.baseUrl}CountyAPI/GetCountyDropDownItems`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: res => {
                const $s = $('#flt_countyId');
                if (res?.data && Array.isArray(res.data)) {
                    res.data.forEach(item => $s.append(`<option value="${item.Key}">${item.Value}</option>`));
                }
            }
        });
    }

    populateActions() {
        $.ajax({
            url: `${this.service.baseUrl}ApiLogAPI/GetActions`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: res => {
                const $s = $('#flt_action');
                if (res?.data && Array.isArray(res.data)) {
                    // "AddEvent" -> "Add Event", "GetClerkCourtrooms" -> "Get Clerk Courtrooms"
                    const pretty = v => (v || '').replace(/([a-z])([A-Z])/g, '$1 $2');
                    res.data.forEach(a => {
                        const esc = $('<div>').text(a).html();
                        const label = $('<div>').text(pretty(a)).html();
                        $s.append(`<option value="${esc}">${label}</option>`);
                    });
                }
            }
        });
    }

    showDetail(logId) {
        $('#progress-apilog').show();
        const modal = new bootstrap.Modal(document.getElementById('ApiLogDetailModal'));
        modal.show();
        $.ajax({
            url: `${this.service.baseUrl}ApiLogAPI/GetApiLog/${logId}`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: res => {
                $('#progress-apilog').hide();
                if (!res.data) {
                    ShowNotification('Not Found', res.error || 'Log row not found.', 'error');
                    return;
                }
                const r = res.data;
                $('#dtl_logId').text(r.log_id);
                $('#dtl_createdAt').text(r.created_at ? new Date(r.created_at).toLocaleString() : '');
                $('#dtl_county').text(r.county_name || (r.county_id || ''));
                $('#dtl_application').text(r.application_name || '');
                $('#dtl_action').text(r.action || '');
                $('#dtl_userId').text(r.user_id || '');
                $('#dtl_eventId').text(r.event_id || '');
                $('#dtl_caseId').text(r.case_id || '');
                $('#dtl_endpoint').text(r.api_end_point || '');
                $('#dtl_error').text(r.error || '');
                $('#dtl_request').text(this._prettyJson(r.request_json));
                $('#dtl_response').text(this._prettyJson(r.response_json));
            },
            error: err => {
                $('#progress-apilog').hide();
                ShowNotification('Error', err.statusText || 'Failed to load log entry.', 'error');
            }
        });
    }

    _prettyJson(raw) {
        if (!raw) return '';
        try { return JSON.stringify(JSON.parse(raw), null, 2); }
        catch (e) { return raw; }
    }

    /** Returns today's date in yyyy-MM-dd (local tz) for a <input type=date> value. */
    _todayIso() {
        const d = new Date();
        const yyyy = d.getFullYear();
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const dd = String(d.getDate()).padStart(2, '0');
        return `${yyyy}-${mm}-${dd}`;
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}
