<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApiLogView.ascx.cs" Inherits="tjc.Modules.jacs.ApiLogView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">API Log</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">

        <!-- Filter panel (collapsible) -->
        <div class="card p-0">
            <div class="card-header">
                <a data-bs-toggle="collapse" href="#apiLogFiltersCollapse" role="button" aria-expanded="true" aria-controls="apiLogFiltersCollapse"
                   class="text-decoration-none d-flex justify-content-between align-items-center collapse-toggle">
                    <strong><i class="fas fa-filter"></i>&nbsp;Search the API Log</strong>
                    <span class="collapse-indicator">
                        <i class="fas fa-minus icon-expanded"></i>
                        <i class="fas fa-plus icon-collapsed"></i>
                    </span>
                </a>
            </div>
            <div id="apiLogFiltersCollapse" class="collapse show">
                <div class="card-body p-3">
                    <div class="row g-2 align-items-end">
                        <div class="col-md-3">
                            <label class="form-label">From</label>
                            <input type="date" id="flt_fromDate" class="form-control" autocomplete="off" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">To</label>
                            <input type="date" id="flt_toDate" class="form-control" autocomplete="off" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">County</label>
                            <select id="flt_countyId" class="form-select">
                                <option value="">All counties</option>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Action</label>
                            <select id="flt_action" class="form-select">
                                <option value="">All actions</option>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Event ID</label>
                            <input type="number" id="flt_eventId" class="form-control" autocomplete="off" title="JACS event id (events.id)" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Case Number</label>
                            <input type="text" id="flt_caseNumber" class="form-control" autocomplete="off" placeholder="e.g. 58-2025-SC-006484-XXXA-SC" title="Resolved to clerk_case_id via JACS events" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Case ID</label>
                            <input type="number" id="flt_caseId" class="form-control" autocomplete="off" title="Clerk case id (events.clerk_case_id)" />
                        </div>
                        <div class="col-md-3 text-end">
                            <button type="button" class="btn btn-primary" id="btnApplyFilters"><i class="fa fa-search"></i>Search</button>
                            <button type="button" class="btn btn-outline-secondary" id="btnResetFilters"><i class="fa fa-xmark"></i>Reset</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <table id="tblApiLog" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th>Timestamp</th>
                    <th>County</th>
                    <th>Action</th>
                    <th>Event</th>
                    <th>Case</th>
                    <th>Endpoint</th>
                    <th>Status</th>
                </tr>
            </thead>
        </table>
    </main>
</div>

<!-- Detail Modal -->
<div class="modal fade" id="ApiLogDetailModal" tabindex="-1" aria-labelledby="ApiLogDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div id="progress-apilog" class="modal-progress" style="display: none;">
                <div class="center-progress"><img alt="" src="/images/loading.gif" /></div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="ApiLogDetailModalLabel">API Log Entry</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-sm m-0 w-100">
                    <tbody>
                        <tr><td><strong>Log ID:</strong></td><td><span id="dtl_logId"></span></td>
                            <td><strong>Timestamp:</strong></td><td><span id="dtl_createdAt"></span></td></tr>
                        <tr><td><strong>County:</strong></td><td><span id="dtl_county"></span></td>
                            <td><strong>Application:</strong></td><td><span id="dtl_application"></span></td></tr>
                        <tr><td><strong>Action:</strong></td><td><span id="dtl_action"></span></td>
                            <td><strong>User ID:</strong></td><td><span id="dtl_userId"></span></td></tr>
                        <tr><td><strong>Event ID:</strong></td><td><span id="dtl_eventId"></span></td>
                            <td><strong>Case ID:</strong></td><td><span id="dtl_caseId"></span></td></tr>
                        <tr><td colspan="4"><strong>Endpoint:</strong><br /><code id="dtl_endpoint" class="d-block text-break"></code></td></tr>
                        <tr><td colspan="4">
                            <strong>Error:</strong>
                            <pre id="dtl_error" class="bg-light p-2 mb-0" style="white-space: pre-wrap;"></pre>
                        </td></tr>
                        <tr><td colspan="4">
                            <strong>Request:</strong>
                            <pre id="dtl_request" class="bg-light p-2 mb-0 small" style="max-height: 300px; overflow: auto; white-space: pre-wrap;"></pre>
                        </td></tr>
                        <tr><td colspan="4">
                            <strong>Response:</strong>
                            <pre id="dtl_response" class="bg-light p-2 mb-0 small" style="max-height: 300px; overflow: auto; white-space: pre-wrap;"></pre>
                        </td></tr>
                    </tbody>
                </table>
            </div>
            <div class="modal-footer justify-content-end">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/apiLog.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = { path: "JACS", framework: $.ServicesFramework(moduleId) };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof ApiLogController === 'undefined') {
                    console.error('ApiLogController is not defined. Check if Script(apiLog.js) loaded correctly.');
                    return;
                }
                const ctl = new ApiLogController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    service: service,
                    pageSize: 25,
                });
                ctl.init();
            } catch (e) {
                console.error('Error initializing ApiLogController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
