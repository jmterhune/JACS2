<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtCalendarView.ascx.cs" Inherits="tjc.Modules.jacs.CourtCalendarView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Edit Court</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="container-fluid">
            <div id="edit_progress-court" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            
            <div class="alert alert-info" role="alert">
                <h4 class="alert-heading">Note</h4>
                <p>In addition to the fields marked required, you must also fill out motions and hearing types on the Scheduling Tab.</p>
            </div>

            <div class="tab-container mb-2">
                <div class="nav-tabs-custom" id="form_tabs">
                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="nav-item">
                            <a href="#tab_main" aria-controls="tab_main" role="tab" tab_name="main" data-toggle="tab" class="nav-link active">Main</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_scheduling" aria-controls="tab_scheduling" role="tab" tab_name="scheduling" data-toggle="tab" class="nav-link">Scheduling</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_custom-email" aria-controls="tab_custom-email" role="tab" tab_name="custom-email" data-toggle="tab" class="nav-link">Custom Email</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_timeslot-search-header" aria-controls="tab_timeslot-search-header" role="tab" tab_name="timeslot-search-header" data-toggle="tab" class="nav-link">Timeslot Search Header</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_docket-print-header" aria-controls="tab_docket-print-header" role="tab" tab_name="docket-print-header" data-toggle="tab" class="nav-link">Docket Print Header</a>
                        </li>
                    </ul>

                    <div class="tab-content p-0">
                        <div role="tabpanel" class="tab-pane active" id="tab_main">
                            <input type="hidden" id="edit_hdCourtId">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-6 required">
                                        <label>Description<em>*</em></label>
                                        <input type="text" id="edit_courtDescription" class="form-control" required>
                                        <div class="invalid-feedback">Description is required.</div>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Case Number Format</label>
                                        <input type="text" id="edit_courtCaseNumFormat" class="form-control">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 required">
                                        <label>County<em>*</em></label>
                                        <select id="edit_courtCounty" class="form-control" required>
                                        </select>
                                        <div class="invalid-feedback">Please Select a county.</div>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Default Plaintiff</label>
                                        <input type="text" id="edit_courtPlaintiff" class="form-control">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Default Defendant</label>
                                        <input type="text" id="edit_courtDefendant" class="form-control">
                                    </div>
                                    <div class="col-md-6">
                                        <label>Default Prosecuting Attorney</label>
                                        <select id="edit_defAttorney" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Default Opposing Attorney</label>
                                        <select id="edit_oppAttorney" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_scheduling">
                            <div class="row">
                                <div class="form-group col-md-4 pt-4">
                                    <div class="d-inline-flex">
                                        <label class="switch switch-sm switch-label switch-pill switch-primary mb-0">
                                            <input type="hidden" id="edit_emailConfirmations" value="0">
                                            <input type="checkbox" class="switch-input" id="switch_email_confirmations">
                                            <span class="switch-slider" data-checked="On" data-unchecked="Off"></span>
                                        </label>
                                        <label class="font-weight-normal mb-0 ml-2" for="switch_email_confirmations">Email Confirmations</label>
                                    </div>
                                </div>
                                <div class="form-group col-md-3 required">
                                    <label>Weeks on Calendar</label>
                                    <input type="number" id="edit_calendarWeeks" class="form-control" value="0">
                                </div>
                                <div class="form-group col-md-4 required">
                                    <label class="d-block">Extending Calendar</label>
                                    <div class="form-check form-check-inline">
                                        <input type="radio" class="form-check-input" id="edit_autoExtensionAuto" name="auto_extension" value="1" checked>
                                        <label class="form-check-label font-weight-normal" for="edit_autoExtensionAuto">Automatic</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input type="radio" class="form-check-input" id="edit_autoExtensionManual" name="auto_extension" value="0">
                                        <label class="form-check-label font-weight-normal" for="edit_autoExtensionManual">Manual</label>
                                    </div>
                                </div>
                            </div>
                            <!-- Add other scheduling fields from court.html -->
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_custom-email">
                            <div class="form-group col-md-12">
                                <label>Custom Email Body</label>
                                <textarea id="edit_customEmailBody" class="form-control summernote"></textarea>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_timeslot-search-header">
                            <div class="form-group col-md-12">
                                <label>Timeslot Search Header</label>
                                <textarea id="edit_timeslotHeader" class="form-control"></textarea>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_docket-print-header">
                            <div class="form-group col-md-12">
                                <label>Custom Docket Print Header</label>
                                <textarea id="edit_customHeader" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <button type="button" class="btn btn-success" id="edit_cmdSave">
                    <i class="fas fa-save" aria-hidden="true"></i>&nbsp;Save
                </button>
                <a href="/court" class="btn btn-secondary">Cancel</a>
            </div>
        </div>
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/court.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2-bootstrap-5-theme.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/select2/js/select2.full.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/summernote/summernote-bs5.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/summernote/summernote-bs5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" /><script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtController === 'undefined') {
                    console.error('CourtController is not defined.');
                    return;
                }
                const courtController = new CourtController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service
                });
                courtController.initEdit();
            } catch (e) {
                console.error('Error initializing CourtController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>