<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttorneyView.ascx.cs" Inherits="tjc.Modules.jacs.AttorneyView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars" aria-hidden="true"></i>
    </button>
    <h2 class="mb-0">Attorneys</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="alert alert-info mt-3" role="alert">
            <strong><i class="fas fa-info-circle" aria-hidden="true"></i>&nbsp;Note:</strong>
            All Attorneys are imported from the Florida Bar. Attorney User Accounts are enabled automatically upon verification of bar number.
        </div>
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#AttorneyEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Attorney</a>
        <table id="tblAttorney" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>Status</th>
                    <th>Name</th>
                    <th>Bar Number</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="AttorneyDetailModal" tabindex="-1" aria-labelledby="AttorneyDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="progress-attorney" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="AttorneyDetailModalLabel">Attorney Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td class="border-top-0"><strong>Name:</strong></td>
                            <td class="border-top-0"><span id="attyName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>User ID:</strong></td>
                            <td><span id="attyUserId"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Bar Number:</strong></td>
                            <td><span id="attyBar"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Phone:</strong></td>
                            <td><span id="attyPhone"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Email:</strong></td>
                            <td><span id="attyEmails"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Scheduling:</strong></td>
                            <td><span id="attySchedule"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Enabled:</strong></td>
                            <td><span id="attyEnabled"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Notes:</strong></td>
                            <td><span id="attyNotes"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdAttorneyId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#AttorneyEditModal" id="editAttorneyBtn"><i class="fas fa-edit me-2" aria-hidden="true"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2" aria-hidden="true"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="AttorneyEditModal" tabindex="-1" aria-labelledby="AttorneyEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress-attorney" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="AttorneyEditModalLabel">Edit Attorney</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdAttorneyId">
                    <input type="hidden" id="edit_hdScheduling">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-12">
                                <label class="d-block">Account Status<span class="text-danger">*</span></label>
                                <div class="form-check form-check-inline">
                                    <input type="radio" name="edit_enabled" class="form-check-input" value="1" id="edit_radio_enabled">
                                    <label class="form-check-label" for="edit_radio_enabled">Enabled</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input type="radio" name="edit_enabled" class="form-check-input" value="0" id="edit_radio_disabled">
                                    <label class="form-check-label" for="edit_radio_disabled">Disabled</label>
                                </div>
                                <div class="invalid-feedback" id="edit_radio-error">Please select an account status.</div>
                            </div>
                            <div class="col-md-4">
                                <label>Bar Number<span class="text-danger">*</span></label>
                                <input type="text" id="edit_attyBar" class="form-control" required>
                                <div class="invalid-feedback">Florida Bar Number is required.</div>
                            </div>
                            <div class="col-md-4">
                                <label>Jud12.flcourts.org User ID<span class="text-danger">*</span></label>
                                <div class="input-group mb-0">
                                    <input type="number" id="edit_attyUserId" class="form-control">
                                    <div class="input-group-append mb-0">
                                        <button id="edit_user_lookup" type="button" title="Lookup UserId from Jud12 site" tabindex="-1" class="btn btn-primary">
                                            <i class="fas fa-search" aria-hidden="true"></i>&nbsp;Lookup</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <label>Name<span class="text-danger">*</span></label>
                                <input type="text" id="edit_attyName" class="form-control" placeholder="Last Name, First Name" required>
                                <div class="invalid-feedback">Name is required.</div>
                            </div>
                            <div class="col-md-4">
                                <label>Phone Number</label>
                                <input type="text" id="edit_attyPhone" class="form-control phone">
                            </div>
                            <div class="col-md-12">
                                <label>Notes</label>
                                <textarea id="edit_attyNotes" class="form-control"></textarea>
                            </div>
                            <div class="col-md-12">
                                <label>Email Addresses<span class="text-danger">*</span></label>
                                <input type="hidden" id="edit_hdEmails">
                                <ul id="edit_email-list" class="list-unstyled"></ul>
                                <button type="button" id="edit_new-email" class="btn btn-outline-primary btn-sm ms-1">
                                    <i class="fas fa-plus" aria-hidden="true"></i>&nbsp;New Email
                                </button>
                                <div class="invalid-feedback" id="edit_email-error">At least one email address is required.</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" class="btn btn-success" id="edit_cmdSave">
                    <i class="fas fa-save" aria-hidden="true"></i>&nbsp;Save
                </button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/imask.js" ForceProvider="DnnFormBottomProvider" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/attorney.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            const element = document.getElementById('edit_attyPhone');
            const maskOptions = {
                mask: '(000) 000-0000'
            };
            const mask = IMask(element, maskOptions);
            try {
                if (typeof AttorneyController === 'undefined') {
                    console.error('AttorneyController is not defined. Check if Script(attorney.js) loaded correctly.');
                    return;
                }
                const attorneyController = new AttorneyController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 0,
                    sortDirection: "asc"
                });
                attorneyController.init();
            } catch (e) {
                console.error('Error initializing AttorneyController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
