<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeList.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EmployeeList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>

<%-- SweetAlert2 + Noty for confirms / toast notifications. CDN-hosted so we
     don't have to ship them as part of the install package. --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-edit.js" Priority="200" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-list.js" Priority="210" />

<div class="container-fluid">

    <%-- DNN Web API context for the JS layer (TabId/ModuleId).
         The AntiForgery token is injected as a hidden __RequestVerificationToken
         field by ServicesFramework.RequestAjaxAntiForgerySupport in Page_Load. --%>
    <script type="text/javascript">
        window.__empdbCtx = {
            tabId: <%= TabId %>,
            moduleId: <%= ModuleId %>
        };
    </script>

    <div class="d-flex flex-wrap gap-2 align-items-center mb-3">
        <a class="btn btn-primary" href="https://jud12fl.sharepoint.com/SitePages/Employee-Directory.aspx" target="_blank" rel="noopener"><i class="fas fa-address-book"></i>&nbsp;Directory</a>
        <a class="btn btn-primary" href="<%=EditUrl("Details") %>"><i class="fas fa-list"></i>&nbsp;Details List</a>
        <a class="btn btn-primary" href="<%=EditUrl("EEO") %>"><i class="fas fa-balance-scale"></i>&nbsp;EEO Setup</a>
        <a class="btn btn-primary" href="/12th-Circuit-Services/Human-Resources/Employee-Reports"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
        <%-- The SWN buttons hit Components/Api/SwnController.cs via AJAX
             (see Scripts/empdb-list.js#swn). Buttons are plain HTML now —
             previously they were asp:LinkButton with postback handlers, but
             the postback was polluting the URL and there was no good place
             to surface progress while Sync churned through ~600 contacts.
             SWN Sync calls AddAllGroups internally, so the explicit
             "Add All Groups" button is no longer exposed here. --%>
        <button type="button" id="empdbSwnMissing"     class="btn btn-warning ms-auto"><i class="fas fa-user-slash"></i>&nbsp;Show Missing SWN Contacts</button>
        <button type="button" id="empdbSwnAddMissing"  class="btn btn-warning"><i class="fas fa-user-plus"></i>&nbsp;Add Missing SWN Contacts</button>
        <button type="button" id="empdbSwnSync"        class="btn btn-warning"><i class="fas fa-sync"></i>&nbsp;SWN Sync</button>
        <button type="button" id="empdbSwnExport"      class="btn btn-warning"><i class="fas fa-file-export"></i>&nbsp;SWN Export</button>
    </div>

    <%-- Full-screen busy overlay shown while the SWN endpoints are running.
         The Sync call can take several minutes; this gives the HR Admin a
         clear "still working" signal so they don't click the button again
         or navigate away mid-sync. Toggled by Scripts/empdb-list.js. --%>
    <div id="empdbBusyOverlay" class="empdb-busy-overlay" style="display:none;" aria-hidden="true">
        <div class="empdb-busy-card">
            <div class="spinner-border text-warning empdb-busy-spinner" role="status">
                <span class="visually-hidden">Working…</span>
            </div>
            <div class="empdb-busy-title">Working…</div>
            <div class="empdb-busy-detail text-muted">This may take a few minutes. Please don't close or refresh the page.</div>
        </div>
    </div>

    <%-- Banner shown after a successful Edit save / delete. The Edit page
         redirects with ?empSaved=1 (or ?empDeleted=1) and the inline JS
         below picks it up and flashes this banner. --%>
    <div id="empdbSavedBanner" class="alert alert-success" role="alert" style="display:none;">
        <span id="empdbSavedBannerText"></span>
    </div>

    <div class="tabs">
        <ul class="nav nav-tabs" id="employeeAdminTabs" role="tablist">
            <li class="nav-item active"><a class="nav-link" href="#pane-employees" data-bs-toggle="tab" data-toggle="tab">Employees</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-jobgroups" data-bs-toggle="tab" data-toggle="tab">Job Categories</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-jobclasses" data-bs-toggle="tab" data-toggle="tab">Classes</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-races" data-bs-toggle="tab" data-toggle="tab">Race</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-locations" data-bs-toggle="tab" data-toggle="tab">Office Locations</a></li>
            <% if (IsSiteAdmin) { %>
            <li class="nav-item"><a class="nav-link" href="#pane-departments" data-bs-toggle="tab" data-toggle="tab">Departments</a></li>
            <% } %>
        </ul>

        <div class="tab-content">

            <div class="tab-pane active" id="pane-employees">
                <a id="empdbEmployeeAdd" class="btn btn-success me-3" href="<%= EditEmployeeUrl(0) %>"><i class="fas fa-plus"></i>&nbsp;Add Employee</a>
                <table id="tblEmployees" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead>
                        <tr>
                            <th class="command-item no-sort"></th>
                            <th>Name</th>
                            <th>Title</th>
                            <th>Department</th>
                            <th>Location</th>
                            <th>Active</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptEmployees" runat="server">
                            <ItemTemplate>
                                <tr data-id='<%# Eval("EmployeeId") %>' data-active='<%# (bool?)(Eval("IsActive")) == true ? "1" : "0" %>' class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                    <td class="command-icon">
                                        <a class="text-primary" title="Edit" href='<%# EditEmployeeUrl((int)Eval("EmployeeId")) %>'><i class="fas fa-edit"></i></a>
                                    </td>
                                    <td><%#Eval("LastName") %>, <%#Eval("FirstName") %></td>
                                    <td><%#Eval("JobTitle") %></td>
                                    <td><%#Eval("AgencyOfEmployment") %></td>
                                    <td><%# GetLocationName(Eval("OfficeLocationId")) %></td>
                                    <td><%# (bool?)(Eval("IsActive")) == true ? "Y" : "N" %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <%-- Bootstrap form-switch — flips the DataTables filter between
                     active employees (default, switch ON) and inactive ones. --%>
                <div class="form-check form-switch mt-2">
                    <input class="form-check-input" type="checkbox" role="switch"
                           id="empdbActiveToggle" checked />
                    <label class="form-check-label" for="empdbActiveToggle">
                        <span class="empdb-active-label">Showing active employees</span>
                        <span class="text-muted small">&mdash; turn off to show inactive employees</span>
                    </label>
                </div>
            </div>

            <%-- ===== Job Categories (API-driven) ===== --%>
            <div class="tab-pane" id="pane-jobgroups">
                <button type="button" id="empdbJobGroupAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Category</button>
                <table id="tblJobGroups" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead><tr><th class="command-item no-sort"></th><th>Description</th><th class="command-item no-sort"></th></tr></thead>
                    <tbody>
                        <tr><td colspan="3" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>

            <%-- ===== Classes (API-driven) ===== --%>
            <div class="tab-pane" id="pane-jobclasses">
                <button type="button" id="empdbJobClassAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Class</button>
                <table id="tblJobClasses" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead>
                        <tr>
                            <th class="command-item no-sort"></th>
                            <th>Class Name</th>
                            <th>Code</th>
                            <th>Pay Grade</th>
                            <th>FLSA</th>
                            <th>EEO</th>
                            <th>MMin</th>
                            <th>MMax</th>
                            <th>AMin</th>
                            <th>AMax</th>
                            <th class="command-item no-sort"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="11" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>

            <%-- ===== Race (API-driven) ===== --%>
            <div class="tab-pane" id="pane-races">
                <button type="button" id="empdbRaceAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Race</button>
                <table id="tblRaces" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead><tr><th class="command-item no-sort"></th><th>Code</th><th>Description</th><th class="command-item no-sort"></th></tr></thead>
                    <tbody>
                        <tr><td colspan="4" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>

            <%-- ===== Office Locations (API-driven) ===== --%>
            <div class="tab-pane" id="pane-locations">
                <button type="button" id="empdbLocationAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Location</button>
                <table id="tblLocations" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead>
                        <tr>
                            <th class="command-item no-sort"></th>
                            <th>Description</th>
                            <th>Address</th>
                            <th>City</th>
                            <th>State</th>
                            <th>Zip</th>
                            <th class="command-item no-sort"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="7" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>

            <% if (IsSiteAdmin) { %>
            <%-- ===== Departments (API-driven, site admins only) ===== --%>
            <div class="tab-pane" id="pane-departments">
                <button type="button" id="empdbDepartmentAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Department</button>
                <table id="tblDepartments" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead>
                        <tr>
                            <th class="command-item no-sort"></th>
                            <th>Group Name</th>
                            <th class="text-center">SWN Group?</th>
                            <th class="command-item no-sort"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="4" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>
            <% } %>

        </div>
    </div>

    <%-- ============== Admin edit modals (API-driven, no postback) ============== --%>
    <div class="modal fade" id="JobGroupEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Job Category</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="JobGroupId" value="0" />
                    <div class="mb-3">
                        <label>Description:</label>
                        <input type="text" name="Description" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbJobGroupSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="JobClassEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Class</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="ClassId" value="0" />
                    <div class="row">
                        <div class="col-md-8 mb-3">
                            <label>Class Name:</label>
                            <input type="text" name="ClassName" class="form-control" maxlength="200" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label>Class Code:</label>
                            <input type="number" name="ClassCode" class="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label>Pay Grade:</label>
                            <input type="number" name="PayGrade" class="form-control" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label>FLSA:</label>
                            <input type="text" name="FLSA" class="form-control" maxlength="50" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label>EEO:</label>
                            <input type="number" name="EEO" class="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3 mb-3">
                            <label>MMin:</label>
                            <input type="number" name="MMin" step="0.01" class="form-control" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>MMax:</label>
                            <input type="number" name="MMax" step="0.01" class="form-control" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>AMin:</label>
                            <input type="number" name="AMin" step="0.01" class="form-control" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>AMax:</label>
                            <input type="number" name="AMax" step="0.01" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbJobClassSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="RaceEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Race</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="RaceId" value="0" />
                    <div class="mb-3">
                        <label>Race Code:</label>
                        <input type="text" name="RaceCode" class="form-control" maxlength="20" />
                    </div>
                    <div class="mb-3">
                        <label>Description:</label>
                        <input type="text" name="Description" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbRaceSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="LocationEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Office Location</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="OfficeLocationId" value="0" />
                    <div class="mb-3">
                        <label>Description:</label>
                        <input type="text" name="Description" class="form-control" maxlength="200" />
                    </div>
                    <div class="mb-3">
                        <label>Address:</label>
                        <input type="text" name="Address" class="form-control" maxlength="200" />
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label>City:</label>
                            <input type="text" name="City" class="form-control" maxlength="100" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>State:</label>
                            <input type="text" name="State" class="form-control" maxlength="2" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Zip:</label>
                            <input type="text" name="Zip" class="form-control" maxlength="10" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbLocationSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <% if (IsSiteAdmin) { %>
    <div class="modal fade" id="DepartmentEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Department</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="GroupID" value="0" />
                    <div class="mb-3">
                        <label>Group Name:</label>
                        <input type="text" name="GroupName" class="form-control" maxlength="200" />
                    </div>
                    <div class="form-check form-switch">
                        <input class="form-check-input" type="checkbox" role="switch" name="IsSwnGroup" id="empdbDepartmentSwn" />
                        <label class="form-check-label" for="empdbDepartmentSwn">Is SWN Group</label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbDepartmentSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
    <% } %>

</div>

<script type="text/javascript">

    function InitEmployeeAdminTables() {
        jQuery(document).ready(function ($) {
            // stateSave persists DataTables page/sort/filter/length per table in
            // localStorage, so navigating to the Edit page and back lands the
            // user on the same row they clicked.
            //
            // The four API-driven admin tabs (#tblJobGroups, #tblJobClasses,
            // #tblRaces, #tblLocations) are intentionally NOT initialised here —
            // empdb-list.js owns them and creates the DataTables instance after
            // the API load fills <tbody>. Initialising twice triggered both
            // "Cannot reinitialise DataTable" and "Requested unknown parameter"
            // warnings.
            //
            // addBtnId names the Add button (if any) that should be relocated
            // into DataTables' .dt-length container after the table is built.
            // The DesignationList view in transcriptDatabase uses the same
            // .dt-length prepend pattern.
            var tableConfigs = [
                { id: '#tblEmployees',    key: 'empdb-tblEmployees-v1',    addBtnId: '#empdbEmployeeAdd' }
            ];
            $.each(tableConfigs, function (i, cfg) {
                if ($(cfg.id).length && !$.fn.DataTable.isDataTable(cfg.id)) {
                    $(cfg.id).DataTable({
                        "order": [[1, "asc"]],
                        "pageLength": 25,
                        "stateSave": true,
                        "stateDuration": 60 * 60 * 24,
                        "stateSaveCallback": function (settings, data) {
                            try { localStorage.setItem(cfg.key, JSON.stringify(data)); } catch (e) { }
                        },
                        "stateLoadCallback": function (settings) {
                            try { return JSON.parse(localStorage.getItem(cfg.key)); } catch (e) { return null; }
                        },
                        "columnDefs": [
                            { "orderable": false, "targets": "no-sort" }
                        ]
                    });
                }
                // Park the Add button inside the .dt-length container so it
                // sits flush-left in front of the page-length selector.
                if (cfg.addBtnId) {
                    var $wrapper = $(cfg.id).closest('.dataTables_wrapper, .dt-container');
                    var $len = $wrapper.length ? $wrapper.find('.dt-length').first() : $();
                    if ($len.length) $len.prepend($(cfg.addBtnId));
                }
            });

            // ---- Active / Inactive toggle for the Employees tab ----
            // DataTables custom search filter scoped to #tblEmployees; reads
            // the form-switch's checked state and shows the matching rows.
            // The checkbox state is persisted in localStorage so navigating
            // away and back keeps the user's choice.
            var ACTIVE_KEY = 'empdb-employeesActiveFilter';
            var $toggle = $('#empdbActiveToggle');
            if ($toggle.length && $.fn.DataTable && !window.__empdbActiveFilterInstalled) {
                window.__empdbActiveFilterInstalled = true;

                // Restore saved state (default = active)
                try {
                    var saved = localStorage.getItem(ACTIVE_KEY);
                    if (saved === 'inactive') $toggle.prop('checked', false);
                } catch (e) { }

                function syncLabel() {
                    $('.empdb-active-label').text($toggle.is(':checked')
                        ? 'Showing active employees'
                        : 'Showing inactive employees');
                }
                syncLabel();

                $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
                    if (settings.nTable.id !== 'tblEmployees') return true;
                    var row = settings.aoData[dataIndex].nTr;
                    if (!row) return true;
                    var rowActive = $(row).attr('data-active') === '1';
                    return $toggle.is(':checked') ? rowActive : !rowActive;
                });

                $toggle.on('change', function () {
                    try { localStorage.setItem(ACTIVE_KEY,
                        $toggle.is(':checked') ? 'active' : 'inactive'); } catch (e) { }
                    syncLabel();
                    if ($.fn.DataTable.isDataTable('#tblEmployees')) {
                        $('#tblEmployees').DataTable().draw();
                    }
                });

                // Initial draw to apply the filter once the table is built.
                if ($.fn.DataTable.isDataTable('#tblEmployees')) {
                    $('#tblEmployees').DataTable().draw();
                }
            }
        });
    }

    // Surface a success banner when the user lands on this page after a save
    // or delete. The Edit page redirects with ?empSaved=1 or ?empDeleted=1.
    (function () {
        function show(text) {
            var b = document.getElementById('empdbSavedBanner');
            var t = document.getElementById('empdbSavedBannerText');
            if (!b || !t) return;
            t.textContent = text;
            b.style.display = '';
            setTimeout(function () { b.style.display = 'none'; }, 4000);
        }
        function strip(param) {
            try {
                var url = new URL(window.location.href);
                if (url.searchParams.has(param)) {
                    url.searchParams.delete(param);
                    history.replaceState(null, '', url.toString());
                }
            } catch (e) { /* old browser - ignore */ }
        }
        var p = new URLSearchParams(window.location.search);
        if (p.get('empSaved') === '1') {
            show('Employee saved.');
            strip('empSaved');
        } else if (p.get('empDeleted') === '1') {
            show('Employee deleted.');
            strip('empDeleted');
        }
    })();

    InitEmployeeAdminTables();

    // ---- Last-edited employee row highlight ----
    // The Edit redirect puts ?empId=N on the URL after a successful Save.
    // Pencil clicks also stash the row id in sessionStorage so coming back
    // from a Cancel still highlights the row the user was just on.
    (function ($) {
        var SS_KEY = "empdb-lastEditedId";

        // Pencil click → remember which employee we're heading off to edit.
        $(document).on("click", "#tblEmployees tbody a[title='Edit']", function () {
            var id = $(this).closest("tr").data("id");
            if (id != null) {
                try { sessionStorage.setItem(SS_KEY, String(id)); } catch (e) { }
            }
        });

        function readEmpIdFromUrl() {
            try {
                var p = new URLSearchParams(window.location.search);
                var v = p.get("empId");
                if (v) {
                    var url = new URL(window.location.href);
                    url.searchParams.delete("empId");
                    history.replaceState(null, "", url.toString());
                }
                return v;
            } catch (e) { return null; }
        }

        function highlight() {
            var $table = $("#tblEmployees");
            if (!$table.length) return;
            if (!$.fn.DataTable || !$.fn.DataTable.isDataTable($table)) {
                // DataTables not initialised yet — try again shortly.
                setTimeout(highlight, 100);
                return;
            }

            var empId = readEmpIdFromUrl();
            if (!empId) {
                try { empId = sessionStorage.getItem(SS_KEY); } catch (e) { }
            }
            // One-shot: don't keep highlighting on subsequent reloads.
            try { sessionStorage.removeItem(SS_KEY); } catch (e) { }
            if (!empId) return;

            var dt = $table.DataTable();
            var foundIdx = null;
            dt.rows().every(function () {
                if (String($(this.node()).data("id")) === String(empId)) {
                    foundIdx = this.index();
                    return false;
                }
            });
            if (foundIdx == null) return;

            // Switch to the row's page if DataTables has it on a different one.
            var ordered = dt.rows({ order: "applied", search: "applied" }).indexes().toArray();
            var visualIdx = ordered.indexOf(foundIdx);
            if (visualIdx >= 0) {
                var pageLen = dt.page.len();
                var pageNum = pageLen > 0 ? Math.floor(visualIdx / pageLen) : 0;
                if (dt.page() !== pageNum) {
                    dt.page(pageNum).draw("page");
                }
            }

            var node = dt.row(foundIdx).node();
            $(node).addClass("empdb-row-highlight");
            try { node.scrollIntoView({ behavior: "smooth", block: "center" }); } catch (e) { }
        }

        $(function () { setTimeout(highlight, 250); });
    })(jQuery);

    // Tab persistence across navigation
    var EMP_TAB_KEY = "empAdminActiveTab";
    function GetActiveEmpTab() {
        try { return sessionStorage.getItem(EMP_TAB_KEY) || "#pane-employees"; }
        catch (e) { return "#pane-employees"; }
    }
    function SetActiveEmpTab(href) {
        try { sessionStorage.setItem(EMP_TAB_KEY, href); } catch (e) { }
    }

    (function ($) {
        $(document).off("click.empTab").on("click.empTab", ".tabs .nav-link[data-toggle=tab], .tabs .nav-link[data-bs-toggle=tab]", function () {
            var href = this.getAttribute("href");
            if (href) { SetActiveEmpTab(href); }
        });
    })(jQuery);

    function RestoreActiveEmpTab() {
        var href = GetActiveEmpTab();
        if (!href) return;
        var link = document.querySelector('.tabs .nav-link[href="' + href + '"]');
        var pane = document.querySelector(href);
        if (!link || !pane) return;
        if (pane.classList.contains("active")) return;

        var tabs = document.querySelectorAll(".tabs .nav-item.active");
        for (var i = 0; i < tabs.length; i++) tabs[i].classList.remove("active");
        var links = document.querySelectorAll(".tabs .nav-link.active");
        for (var k = 0; k < links.length; k++) links[k].classList.remove("active");
        var panes = document.querySelectorAll(".tabs .tab-pane.active");
        for (var j = 0; j < panes.length; j++) panes[j].classList.remove("active");

        var li = link.closest(".nav-item");
        if (li) li.classList.add("active");
        link.classList.add("active");
        pane.classList.add("active");
    }

    jQuery(document).ready(RestoreActiveEmpTab);
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
