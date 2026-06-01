<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewHireITWorksheet.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.NewHireITWorksheet" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<%-- SweetAlert2 + Noty for confirms / toast notifications. Same libs the
     EditEmployee view uses so anything else loaded on the same page
     (DataTables, Bootstrap, etc.) doesn't conflict. --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-edit.js" Priority="200" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-nhit.js" Priority="201" />

<div id="NhitForm" class="container-fluid p-3">

    <div class="d-flex align-items-center mb-3">
        <h3 class="mb-0"><i class="fas fa-laptop-medical"></i>&nbsp;New Hire IT Worksheet</h3>
    </div>

    <%-- DNN Web API context (TabId / ModuleId / IsAdmin) plus an optional
         preload payload populated when the page is reached via the
         new-hire flow (?EmployeeId=N). The AntiForgery token is injected as
         __RequestVerificationToken by the page's
         ServicesFramework.RequestAjaxAntiForgerySupport call in Page_Load. --%>
    <script type="text/javascript">
        window.__empdbCtx = {
            tabId: <%= TabId %>,
            moduleId: <%= ModuleId %>,
            isAdmin: <%= IsAdminFlagJs %>,
            preload: <%= PreloadJson %>,
            mainViewUrl: <%= MainViewUrlJson %>
        };
    </script>

    <%-- Just-saved banner: only renders when the page was reached via the
         New Hire IT Worksheet redirect from the EditEmployee Save handler. --%>
    <% if (HasPreload) { %>
        <div class="alert alert-success">
            <i class="fas fa-check-circle"></i>&nbsp;
            <strong><%= Server.HtmlEncode(PreloadEmployeeName ?? string.Empty) %></strong>
            saved. Now fill out their IT setup worksheet below and submit when ready.
        </div>
    <% } %>

    <%-- =============== Profile selector + admin actions =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Profile</legend>
        <div class="row">
            <div class="col-12 col-md-6 col-lg-4">
                <label for="empdbNhitProfile">Load profile</label>
                <select id="empdbNhitProfile" class="form-control">
                    <option value="">(no profile — start blank)</option>
                </select>
            </div>
            <div class="col-12 col-md-6 col-lg-8 d-flex align-items-end empdb-nhit-actions">
                <%-- Only "Manage Profiles" sits next to the dropdown now.
                     Saving a profile is folded into the bottom "Save as Profile"
                     button (which also submits to the helpdesk); deleting is
                     done from the Manage Profiles modal. --%>
                <button type="button" id="empdbNhitProfileManage" class="btn btn-warning admin-only">
                    <i class="fas fa-folder-open"></i>&nbsp;Manage Profiles
                </button>
            </div>
        </div>
    </fieldset>

    <%-- =============== Employee Information =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Employee Information</legend>
        <div class="row">
            <div class="col-12 col-md-6 col-lg-4">
                <label>Position Title</label>
                <input type="text" name="PositionTitle" class="form-control" maxlength="150" />
            </div>
            <div class="col-12 col-md-6 col-lg-4">
                <label>Supervisor Name / Title</label>
                <input type="text" name="SupervisorName" class="form-control" maxlength="200" />
            </div>
            <div class="col-12 col-md-6 col-lg-4">
                <label>Department / Unit / Group</label>
                <input type="text" name="DepartmentUnitGroup" class="form-control" maxlength="150" />
            </div>
        </div>
        <div class="row">
            <div class="col-12 col-md-6 col-lg-4">
                <label>Employee Name (Including Middle Initial) <span class="text-danger">*</span></label>
                <input type="text" name="EmployeeName" class="form-control" maxlength="200" required />
            </div>
            <div class="col-12 col-md-6 col-lg-4">
                <label>AKA</label>
                <input type="text" name="AKA" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="row">
            <div class="col-12 col-md-6 col-lg-3">
                <label>Office / Suite #</label>
                <input type="text" name="OfficeSuiteNumber" class="form-control" maxlength="50" />
            </div>
            <div class="col-12 col-md-6 col-lg-3">
                <label>Desk Phone Number</label>
                <input type="text" name="DeskPhoneNumber" class="form-control empdb-phone-mask" maxlength="25" placeholder="(999) 999-9999" />
            </div>
            <div class="col-12 col-md-6 col-lg-2">
                <label>Today&#39;s Date</label>
                <input type="date" name="TodaysDate" class="form-control" />
            </div>
            <div class="col-12 col-md-6 col-lg-2">
                <label>Effective Date</label>
                <input type="date" name="EffectiveDate" class="form-control" />
            </div>
            <div class="col-12 col-md-6 col-lg-2">
                <label>Temp/Intern End Date</label>
                <input type="date" name="TempInternEndDate" class="form-control" />
            </div>
        </div>
    </fieldset>

    <%-- =============== Building / Location =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Building / Location</legend>
        <div class="form-radio-row">
            <div class="form-check form-check-inline"><input type="radio" name="BuildingLocation" id="bldgMound" value="Mound Street" class="form-check-input" /><label class="form-check-label" for="bldgMound">Mound Street</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="BuildingLocation" id="bldgManatee" value="Manatee" class="form-check-input" /><label class="form-check-label" for="bldgManatee">Manatee</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="BuildingLocation" id="bldgSarasota" value="Sarasota/CJC" class="form-check-input" /><label class="form-check-label" for="bldgSarasota">Sarasota/CJC</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="BuildingLocation" id="bldgVenice" value="Venice" class="form-check-input" /><label class="form-check-label" for="bldgVenice">Venice</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="BuildingLocation" id="bldgDeSoto" value="DeSoto" class="form-check-input" /><label class="form-check-label" for="bldgDeSoto">DeSoto</label></div>
        </div>
    </fieldset>

    <%-- =============== Employee Type =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Employee Type</legend>
        <div class="form-radio-row">
            <div class="form-check form-check-inline"><input type="radio" name="EmployeeType" id="etState" value="State" class="form-check-input" /><label class="form-check-label" for="etState">State</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="EmployeeType" id="etCounty" value="County" class="form-check-input" /><label class="form-check-label" for="etCounty">County</label></div>
            <div class="form-check form-check-inline"><input type="radio" name="EmployeeType" id="etOther" value="Other" class="form-check-input" /><label class="form-check-label" for="etOther">Other</label></div>
        </div>
    </fieldset>

    <%-- =============== Equipment Needed =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Equipment Needed</legend>
        <div class="form-radio-row">
            <div class="form-check form-check-inline"><input type="checkbox" name="EquipmentLaptop" id="eqLaptop" class="form-check-input" /><label class="form-check-label" for="eqLaptop">Laptop</label></div>
            <div class="form-check form-check-inline"><input type="checkbox" name="EquipmentTwoInOne" id="eqTwoInOne" class="form-check-input" /><label class="form-check-label" for="eqTwoInOne">2-in-1</label></div>
            <div class="form-check form-check-inline"><input type="checkbox" name="EquipmentDesktop" id="eqDesktop" class="form-check-input" /><label class="form-check-label" for="eqDesktop">Desktop</label></div>
            <div class="form-check form-check-inline"><input type="checkbox" name="EquipmentCellPhone" id="eqCell" class="form-check-input" /><label class="form-check-label" for="eqCell">Cell Phone</label></div>
        </div>
    </fieldset>

    <%-- =============== Keys / Access =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Keys / Access</legend>
        <div class="row">
            <div class="col-12 col-md-6">
                <label>Access card to</label>
                <input type="text" name="AccessCardTo" class="form-control" />
            </div>
            <div class="col-12 col-md-6">
                <label>Keys needed</label>
                <input type="text" name="KeysNeeded" class="form-control" />
            </div>
        </div>
        <div class="row">
            <div class="col-12 col-md-6">
                <label>Parking access</label>
                <input type="text" name="ParkingAccess" class="form-control" />
            </div>
            <div class="col-12 col-md-6">
                <label>Email distribution groups</label>
                <input type="text" name="EmailDistributionGroups" class="form-control" />
            </div>
        </div>
        <div class="row">
            <div class="col-12 col-md-6">
                <label>Calendars / share calendar access</label>
                <input type="text" name="CalendarAccess" class="form-control" />
            </div>
            <div class="col-12 col-md-6">
                <label>Share drive access</label>
                <input type="text" name="ShareDriveAccess" class="form-control" />
            </div>
        </div>
        <div class="row">
            <div class="col-12 col-md-12">
                <label>Additional printer access</label>
                <input type="text" name="AdditionalPrinterAccess" class="form-control" />
            </div>
        </div>
    </fieldset>

    <%-- =============== Manager Access =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Manager Access</legend>
        <div class="form-check"><input type="checkbox" name="ManagerBlog" id="mgrBlog" class="form-check-input" /><label class="form-check-label" for="mgrBlog">Manager&#39;s Blog and Manager&#39;s Guide</label></div>
        <div class="form-check"><input type="checkbox" name="AddToSupervisorDropdown" id="mgrSupervisor" class="form-check-input" /><label class="form-check-label" for="mgrSupervisor">Add to supervisor drop-down menu on database</label></div>
        <div class="form-check"><input type="checkbox" name="WorkCellphoneSetup" id="mgrCell" class="form-check-input" /><label class="form-check-label" for="mgrCell">Work cellphone set up</label></div>
    </fieldset>

    <%-- =============== Catalog sections (rendered by JS from /api/NhitItems/Active) =============== --%>
    <fieldset class="mb-3">
        <%-- "Manage Applications" lives in the Software section's legend
             because that's where the catalog lists start. The button still
             manages every category (Software / Intranet / Judicial) — its
             modal has a category filter — but anchoring it visually to the
             first list keeps the link between button and lists obvious. --%>
        <legend class="h5 d-flex align-items-center">
            <span>Software Applications</span>
            <button type="button" id="empdbNhitManageApps" class="btn btn-sm btn-warning ms-3 admin-only">
                <i class="fas fa-list"></i>&nbsp;Manage Applications
            </button>
        </legend>
        <div id="empdbNhitItemsSoftware" class="empdb-nhit-checklist"><span class="text-muted">Loading…</span></div>
    </fieldset>

    <fieldset class="mb-3">
        <legend class="h5">Intranet Application Access</legend>
        <div id="empdbNhitItemsIntranet" class="empdb-nhit-checklist"><span class="text-muted">Loading…</span></div>
    </fieldset>

    <fieldset class="mb-3">
        <legend class="h5">Judicial Applications</legend>
        <div id="empdbNhitItemsJudicial" class="empdb-nhit-checklist"><span class="text-muted">Loading…</span></div>
    </fieldset>

    <%-- =============== Notes =============== --%>
    <fieldset class="mb-3">
        <legend class="h5">Additional Notes</legend>
        <textarea name="Notes" class="form-control" rows="3" placeholder="Anything else the helpdesk should know"></textarea>
    </fieldset>

    <hr />
    <div>
        <button type="button" id="empdbNhitSubmit" class="btn btn-primary">
            <i class="fas fa-paper-plane"></i>&nbsp;Submit &amp; Send to Helpdesk
        </button>
        <%-- "Save as Profile" does Submit+Email AND saves the profile in
             one go. If the dropdown above has a selection, that profile is
             updated; otherwise the user is prompted for a new profile name.
             Admin-only because saving / updating a profile requires it. --%>
        <button type="button" id="empdbNhitSubmitSaveProfile" class="btn btn-success admin-only">
            <i class="fas fa-save"></i>&nbsp;Save as Profile
        </button>
    </div>
</div>

<%-- =============== Manage Applications modal =============== --%>
<div class="modal fade" id="empdbNhitItemsModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Manage Applications</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label>Filter by category</label>
                        <select id="empdbNhitItemsFilter" class="form-control">
                            <option value="">(all)</option>
                            <option value="Software">Software Applications</option>
                            <option value="Intranet">Intranet Application Access</option>
                            <option value="Judicial">Judicial Applications</option>
                        </select>
                    </div>
                    <div class="col-12 col-md-6 col-lg-9 d-flex align-items-end">
                        <button type="button" id="empdbNhitItemAdd" class="btn btn-primary btn-sm">
                            <i class="fas fa-plus"></i>&nbsp;Add Application
                        </button>
                    </div>
                </div>
                <table id="empdbNhitItemsTable" class="table table-striped table-sm">
                    <thead>
                        <tr>
                            <th class="command-item"></th>
                            <th>Category</th>
                            <th>Name</th>
                            <th>Notes</th>
                            <th>Sort Order</th>
                            <th class="text-center">Active</th>
                            <th class="command-item"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="7" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<%-- =============== Manage Profiles modal =============== --%>
<%-- Lists every saved profile with a delete icon per row. The profile
     dropdown at the top of the form is the "load" UI; this modal is
     strictly for delete (per the spec). Refreshing the list also
     refreshes the dropdown so a deleted entry disappears immediately. --%>
<div class="modal fade" id="empdbNhitProfilesModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Manage Profiles</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table id="empdbNhitProfilesTable" class="table table-striped table-sm">
                    <thead>
                        <tr>
                            <th>Profile Name</th>
                            <th class="command-item"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="2" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<%-- =============== Application editor modal (used inside Manage Applications) =============== --%>
<div class="modal fade" id="empdbNhitItemModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Application</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="NhitItemId" value="0" />
                <div class="row">
                    <div class="col-12 col-md-6">
                        <label>Category <span class="text-danger">*</span></label>
                        <select name="Category" class="form-control">
                            <option value="Software">Software Applications</option>
                            <option value="Intranet">Intranet Application Access</option>
                            <option value="Judicial">Judicial Applications</option>
                        </select>
                    </div>
                    <div class="col-12 col-md-6">
                        <label>Sort Order</label>
                        <input type="number" name="SortOrder" class="form-control" min="0" step="10" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-12">
                        <label>Name <span class="text-danger">*</span></label>
                        <input type="text" name="Name" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-12">
                        <label>Notes (optional, shown beside the name on the form)</label>
                        <input type="text" name="Notes" class="form-control" maxlength="500" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-12">
                        <div class="form-check">
                            <input type="checkbox" name="IsActive" id="nhitItemIsActive" class="form-check-input" checked="checked" />
                            <label class="form-check-label" for="nhitItemIsActive">Active (uncheck to hide from the form without deleting)</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="empdbNhitItemSave" class="btn btn-primary">Save</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>

<%-- The old "Save as New Profile" Bootstrap modal was replaced by a
     SweetAlert input prompt so the bottom "Save as Profile" button can
     ask for a name inline as part of its single-click submit-and-save
     flow. See empdb-nhit.js → submitAndSaveProfile. --%>

<dnn:DnnCssInclude runat="server" FilePath="/DesktopModules/tjc.modules/EmployeeDB/module.css" />
