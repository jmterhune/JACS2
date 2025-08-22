<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtEdit.ascx.cs" Inherits="tjc.Modules.jacs.CourtEdit" %>
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
        <div class="col-md-8">
            <div id="edit_progress-court" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="alert alert-info" role="alert">
                <strong class="alert-heading"><i class="fas fa-info-circle"></i>&nbsp;Note</strong>
                In addition to the fields marked required, you must also fill out motions and hearing types on the Scheduling Tab.
            </div>
            <div class="tab-container mb-2">
                <div class="tabs" id="form_tabs">
                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="nav-item active">
                            <a href="#tab_main" aria-controls="tab_main" role="tab" data-toggle="tab" class="nav-link active">Main</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_scheduling" aria-controls="tab_scheduling" role="tab" data-toggle="tab" class="nav-link">Scheduling</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_custom-email" aria-controls="tab_custom-email" role="tab" data-toggle="tab" class="nav-link">Custom Email</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_templates" aria-controls="tab_templates" role="tab" data-toggle="tab" class="nav-link">Templates</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_timeslot-search-header" aria-controls="tab_timeslot-search-header" role="tab" data-toggle="tab" class="nav-link">Timeslot Search Header</a>
                        </li>
                        <li role="presentation" class="nav-item">
                            <a href="#tab_docket-print-header" aria-controls="tab_docket-print-header" role="tab" data-toggle="tab" class="nav-link">Docket Print Header</a>
                        </li>
                    </ul>
                    <div class="tab-content p-3">
                        <div role="tabpanel" class="tab-pane active" id="tab_main">
                            <input type="hidden" id="edit_hdCourtId">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Description<em>*</em></label>
                                        <input type="text" id="edit_courtDescription" class="form-control" required>
                                        <div class="invalid-feedback">Description is required.</div>
                                    </div>
                                    <div class="col-md-6">
                                        <label>County<em>*</em></label>
                                        <select id="edit_courtCounty" class="form-control" required>
                                        </select>
                                        <div class="invalid-feedback">Please Select a county.</div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>Case Number Format</label>
                                        <div>
                                            <div class="case-format-row">
                                                <input type="radio" name="case_format_type" value="1" id="radio_1">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" placeholder="xxxx">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="7" minlength="7" placeholder="xxxxxxx">
                                            </div>
                                            <div class="case-format-row">
                                                <input type="radio" name="case_format_type" value="2" id="radio_2">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" placeholder="xxxx">
                                                <select class="case-num-format-multi case-type-select">
                                                    <option value="0"></option>
                                                </select>
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="7" minlength="7" placeholder="xxxxxxx">
                                            </div>
                                            <div class="case-format-row">
                                                <input type="radio" name="case_format_type" value="3" id="radio_3" checked="">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="2" minlength="2" placeholder="County Code" id="case_format_input_first">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" placeholder="Year" id="case_format_input_second">
                                                <select class="case-num-format-multi case-type-select">
                                                    <option value="0"></option>
                                                </select>
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="6" minlength="6" placeholder="Case Number" id="case_format_input_three">
                                                <input type="text" value="XXXX" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" id="case_format_input_four">
                                                <input type="text" value="XX" class="col-md-2 case-num-format-multi" maxlength="2" minlength="2" id="case_format_input_five">
                                            </div>
                                            <div class="case-format-row">
                                                <input type="radio" name="case_format_type" value="4" id="radio_4">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" placeholder="xxxx">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="7" minlength="7" placeholder="xxxxxx">
                                                <input type="text" value="" class="col-md-2 case-num-format-multi" maxlength="4" minlength="4" placeholder="xxxx">
                                            </div>
                                            <div class="case-format-row">
                                                <input type="radio" name="case_format_type" value="5" id="radio_5">
                                                <input type="text" value="" class="col-md-4 case-num-format-multi" placeholder="xxxxxxxxxxxx">
                                                <input type="hidden" name="case_num_format" id="case_format_val" value="">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Default Prosecuting Attorney</label>
                                        <select id="edit_defAttorney" class="form-control"></select>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Default Plaintiff</label>
                                        <input type="text" id="edit_courtPlaintiff" class="form-control">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Default Opposing Attorney</label>
                                        <select id="edit_oppAttorney" class="form-control"></select>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Default Defendant</label>
                                        <input type="text" id="edit_courtDefendant" class="form-control">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_scheduling">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-4 pt-4">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_emailConfirmations" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_emailConfirmations">
                                            <label class="form-check-label" for="switch_emailConfirmations">Email Confirmations</label>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <label>Weeks on Calendar<em>*</em></label>
                                        <input type="number" id="edit_calendarWeeks" class="form-control" value="0" min="0" required>
                                        <div class="invalid-feedback">Please enter a valid number of weeks.</div>
                                    </div>
                                    <div class="col-md-3">
                                        <label>Extending Calendar</label>
                                        <div class="form-check form-check-inline">
                                            <input type="radio" name="auto_extension" id="edit_autoExtensionAuto" class="form-check-input" checked>
                                            <label class="radio-inline form-check-label" for="edit_autoExtensionAuto">Automatic</label>
                                        </div>
                                        <div class="form-check form-check-inline">
                                            <input type="radio" name="auto_extension" id="edit_autoExtensionManual" class="form-check-input">
                                            <label class="radio-inline form-check-label" for="edit_autoExtensionManual">Manual</label>
                                        </div>
                                    </div>
                                </div>
                                <h5 class="text-primary mt-2">Public Settings</h5>
                                <hr class="mb-0">
                                <div class="row">
                                    <div class="col-md-4 pt-4">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_allowWebScheduling" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_allowWebScheduling">
                                            <label class="form-check-label" for="switch_allowWebScheduling">Allow Web Scheduling</label>&nbsp; <i class="fa-lg fa fa-question-circle text-primary" data-toggle="tooltip" data-placement="top" title="Disabled if there is no Judge attached to the court."></i>
                                        </div>
                                        <div class="form-check form-switch pt-4">
                                            <input type="hidden" id="edit_publicAvailableTimeslots" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_publicAvailableTimeslots">
                                            <label class="form-check-label" for="switch_publicAvailableTimeslots">Public Available Timeslots</label>
                                        </div>
                                        <div class="form-check form-switch pt-4">
                                            <input type="hidden" id="edit_showDocketInternet" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_showDocketInternet">
                                            <label class="form-check-label" for="switch_showDocketInternet">Show Docket on Internet</label>
                                        </div>
                                    </div>
                                    <div class="col-md-8 pt-4">
                                        <div class="row">
                                            <div class="col-md-6" id="div_maxAvailableSlots">
                                                <label>Max Available Time Slots</label>
                                                <input type="number" id="edit_maxAvailableSlots" class="form-control">
                                            </div>
                                            <div class="col-md-6 d-none" id="div_lagTime">
                                                <label>Lagtime for Available Timeslots</label>
                                                <input type="number" id="edit_lagTime" class="form-control">
                                            </div>
                                            <div class="col-md-6 d-none" id="div_publicDocketDays">
                                                <label>Number of Days of Docket on Internet</label>
                                                <input type="number" id="edit_publicDocketDays" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <h5 class="text-primary mt-2">Required Fields</h5>
                                <hr class="mb-0">
                                <div class="row">
                                    <div class="col-md-6 pt-4">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_plaintiffRequired" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_plaintiffRequired">
                                            <label class="form-check-label" for="switch_plaintiffRequired">Plaintiff required</label>
                                        </div>
                                    </div>
                                    <div class="col-md-6 pt-4">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_defendantRequired" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_defendantRequired">
                                            <label class="form-check-label" for="switch_defendantRequired">Defendant required</label>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_plaintiffAttorneyRequired" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_plaintiffAttorneyRequired">
                                            <label class="form-check-label" for="switch_plaintiffAttorneyRequired">Plaintiff attorney required</label>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-check form-switch">
                                            <input type="hidden" id="edit_defendantAttorneyRequired" value="0">
                                            <input class="form-check-input" type="checkbox" id="switch_defendantAttorneyRequired">
                                            <label class="form-check-label" for="switch_defendantAttorneyRequired">Defendant attorney required</label>
                                        </div>
                                    </div>
                                    <div class="col-md-12">
                                        <label>Available Motions</label>
                                        <select id="edit_availableMotions" class="form-control" aria-describedby="helpAvailableMotions" name="available_motions[]" multiple="multiple"></select>
                                        <div id="helpAvailableMotions" class="form-text mb-0">Attorneys will be able to select the above motions when scheduling</div>
                                    </div>
                                    <div class="col-md-12">
                                        <label>Restricted Motions</label>
                                        <select id="edit_restrictedMotions" class="form-control" aria-describedby="helpRestrictedMotions" name="restricted_motions[]" multiple="multiple"></select>
                                        <div id="helpRestrictedMotions" class="form-text mb-0">Attorneys will be unable to select the above motions on all timeslots</div>
                                    </div>
                                    <div class="col-md-12">
                                        <label>Attorney Scheduling Available Hearing Types</label>
                                        <select id="edit_availableHearingTypes" class="form-control" aria-describedby="helpAvailableHearingTypes" name="available_hearing_types[]" multiple="multiple"></select>
                                        <div id="helpAvailableHearingTypes" class="form-text mb-0">Attorneys will only be able to select the above hearing type(s) when scheduling</div>
                                    </div>
                                    <div class="col-md-12 no-form-group">
                                        <label>Web Policy</label>
                                        <div id="editor_webPolicy" class="summernote mb-0"></div>
                                        <textarea id="edit_webPolicy" class="form-control hidden"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_custom-email">
                            <div class="row">
                                <div class="col-md-12 no-form-group">
                                    <label>Email Template</label>
                                    <div id="editor_customEmailBody" class="summernote"></div>
                                    <textarea id="edit_customEmailBody" class="form-control hidden"></textarea>
                                </div>
                                <div class="col-md-12 mt-3">
                                    <ul class="list mb-0">
                                        <li>Case Number: [case]</li>
                                        <li>Motion: [motion]</li>
                                        <li>Attorney: [attorney]</li>
                                        <li>Plaintiff: [plaintiff]</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_templates">
                            <div class="row">
                                <div class="col-md-12">
                                    <label id="auto_extension_label">Automatic</label>
                                    <table class="table" id="templates_table">
                                        <thead>
                                            <tr>
                                                <th>Week</th>
                                                <th colspan="2">Template</th>

                                            </tr>
                                        </thead>
                                        <tbody>
                                            <!-- Rows will be added dynamically via JavaScript -->
                                        </tbody>
                                    </table>
                                    <button type="button" class="btn btn-primary" id="add_template_row">+ New Item</button>
                                </div>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_timeslot-search-header">
                            <div class="col-md-12 no-form-group">
                                <label>Custom Header</label>
                                <div id="editor_timeslotHeader" class="summernote"></div>
                                <textarea id="edit_timeslotHeader" class="form-control hidden"></textarea>
                            </div>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="tab_docket-print-header">
                            <div class="col-md-12">
                                <label>Custom Docket Print Header</label>
                                <textarea id="edit_customHeader" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <p>
                <button type="button" class="btn btn-success" id="edit_cmdSave">
                    <i class="fas fa-save" aria-hidden="true"></i>&nbsp;Save
                </button>
                <a href='<%=CourtListUrl %>' class="btn btn-secondary">Cancel</a>
            </p>
        </div>
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/court-edit.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2-bootstrap-5-theme.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/select2/js/select2.full.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/summernote/summernote-lite.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/summernote/summernote-lite.min.js" />
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
            try {
                if (typeof CourtController === 'undefined') {
                    console.error('CourtController is not defined.');
                    return;
                }
                $('.summernote').summernote();
                const courtController = new CourtController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    viewUrl: "<%=CourtListUrl%>",
                    editUrl: "<%=CourtEditUrl%>",
                    calendarUrl: "<%=CourtCalendarUrl%>"
                });
                courtController.initEdit();
            } catch (e) {
                console.error('Error initializing CourtController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
