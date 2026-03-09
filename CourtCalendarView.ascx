<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtCalendarView.ascx.cs" Inherits="tjc.Modules.jacs.CourtCalendarView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>Menu
    </button>
    <h2 class="mb-0">Court Calendar</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" id="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <h3 class="mb-2">Court Name: <span class="text-capitalize">
            <asp:Literal ID="ltCourtName" runat="server" /></span></h3>
        <div class="court-header d-flex mb-4">
            <div class="court-actions me-auto">
                <a href="#" class="btn btn-primary" id="editCourtBtn" style="display:none"><i class="fas fa-lg fa-edit"></i>Edit</a>
                <a href="#" class="btn btn-primary" id="userDefinedFieldsBtn" style="display:none"><i class="fas fa-lg fa-cog"></i>User Defined Fields</a>
                <a href="#" class="btn btn-primary" id="truncateBtn" style="display:none"><i class="fas fa-lg fa-trash"></i>Truncate</a>
                <a href="#" class="btn btn-primary" id="icalExportBtn"><i class="fas fa-lg fa-calendar"></i>iCal export</a>
                <a href="#" class="btn btn-primary" id="monthlyExportBtn"><i class="fas fa-lg fa-file-export"></i>Monthly Export</a>
                <a href="#" class="btn btn-primary" id="extendBtn" style="display:none"><i class="fas fa-lg fa-fast-forward"></i>Extend Calendar</a>
            </div>
            <div class="calendar-actions d-inline-block">
                <a href="#" class="btn btn-default" id="deleteTimeslotsBtn" style="display:none"><i class="fas fa-trash"></i>Delete Timeslot(s)</a>
                <a href="#" class="btn btn-default" id="copyTimeslotsBtn" style="display:none"><i class="fas fa-copy"></i>Copy Timeslot(s)</a>
                <button type="button" style="display: none;" id="printCalendarBtn" class="btn btn-secondary" onclick="window.print()"><i class="fas fa-print"></i>Print Calendar View</button>
            </div>
        </div>

        <div class="calendar-note alert alert-info">
            <i class="fa fa-info-circle"></i><strong>Note:</strong> Click and drag the mouse over period of time or just click on the day to create a timeslot.
        </div>

        <div class="calendar-judge">
            <h4>
                <asp:Literal ID="ltJudgeName" runat="server" /></h4>
        </div>

        <div id="calendar"></div>
    </main>
</div>
<!-- Extend Calendar Modal -->
<div class="modal fade" id="ExtendCalendarModal" tabindex="-1" aria-labelledby="ExtendCalendarModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content">
            <div id="progress-extend" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="ExtendCalendarModalLabel">Extend Calendar</h4>
                <button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="alert alert-info mb-4">
                    <i class="fa fa-info-circle"></i><strong>Note:</strong> This will extend the calendar based on the order of the automated templates.
                </div>
                <div class="container-fluid">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Literal ID="ltLastTimeslot" runat="server" />
                                <asp:Literal ID="ltLastTemplateTimeslot" runat="server" />
                                <asp:Literal ID="ltLastHearing" runat="server" />
                            </div>
                            <div class="col-md-6">
                                <label for="startTemplate">Starting Template<em>*</em></label>
                                <asp:DropDownList ClientIDMode="Static" ID="ddlStartTemplate" runat="server" CssClass="form-control" required="required" />
                            </div>
                            <div class="col-md-6">
                                <label for="weeks">Weeks to Extend<em>*</em></label>
                                <asp:TextBox ID="txtWeeks" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="Number" required="required" />
                            </div>
                            <div class="col-md-6">
                                <label for="startDate">Start Date<em>*</em></label>
                                <asp:TextBox ID="txtStartDate" ClientIDMode="Static" runat="server" CssClass="form-control datepicker" required="required" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer  justify-content-between">
                <button id="btnExtend" type="submit" class="btn btn-success"><i class="fa fa-save"></i>Extend</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal"><i class="fa fa-ban"></i>Close</button>
            </div>
        </div>
    </div>
</div>

<!-- Reschedule Hearing Modal -->
<div class="modal fade" id="RescheduleHearingModal" tabindex="-1" aria-labelledby="RescheduleHearingModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content">
            <div id="progress-hearing" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="RescheduleHearingModalLabel">Reschedule Hearing</h4>
                <button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">×</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-12">
                                <div id="reschedule-calendar" style="height: 500px;"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Timeslot Modal -->
<div class="modal fade" id="TimeslotModal" tabindex="-1" aria-labelledby="TimeslotModalLabel" aria-hidden="true" data-bs-focus="false">
    <div class="modal-dialog modal-xlg">
        <div class="modal-content">
            <input type="hidden" id="edit_timeslotId">
            <div id="progress-timeslot" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="TimeslotModalLabel">Create...</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid p-0">
                    <div class="tabs mb-0">
                        <ul class="nav nav-tabs">
                            <li class="nav-item active"><a class="nav-link" data-toggle="tab" href="#timeslotTab">Timeslot(s)</a></li>
                            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#eventTab">Create Event</a></li>
                            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#eventsTab">Event(s)</a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="timeslotTab" class="tab-pane active form-group mb-0">

                                <div class="row blocking">
                                    <div class="col-md-6">
                                        <label>Block</label>
                                        <input type="checkbox" id="timeslot_block" class="form-check-input" autocomplete="off">
                                    </div>
                                    <div class="col-md-6 public_block" style="display: none;">
                                        <label>Public Block</label>
                                        <input type="checkbox" id="timeslot_publicBlock" class="form-check-input" autocomplete="off">
                                    </div>
                                </div>
                                <div class="row block_reason" style="display: none;">
                                    <div class="col-md-12">
                                        <label>Block Reason</label>
                                        <textarea id="timeslot_blockReason" class="form-control" autocomplete="off"></textarea>
                                    </div>
                                </div>
                                <div class="row cattle-call">
                                    <div class="col-md-6">
                                        <label>Concurrent/Consecutive</label>
                                        <div class="form-check">
                                            <input type="radio" id="cattlecall_yes" name="timeslot_cattlecall" value="1" class="form-check-input" autocomplete="off" checked>
                                            <label class="form-check-label" for="cattlecall_yes">Concurrent</label>
                                        </div>
                                        <div class="form-check">
                                            <input type="radio" id="cattlecall_no" name="timeslot_cattlecall" value="0" class="form-check-input" autocomplete="off">
                                            <label class="form-check-label" for="cattlecall_no">Consecutive</label>
                                        </div>
                                        <input type="hidden" id="timeslot_allDay" value="false">
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-2 mb-0">
                                        <label>Start Time</label>
                                        <input type="text" id="timeslot_startTime" class="form-control" autocomplete="off">
                                        <input type="hidden" name="t_start" id="t_start" />
                                    </div>
                                    <div class="col-md-2 mb-0">
                                        <label>End Time</label>
                                        <input type="text" id="timeslot_endTime" class="form-control" autocomplete="off">

                                        <input type="hidden" name="t_end" id="t_end" />
                                    </div>
                                    <div class="col-md-3 mb-0">
                                        <label>Duration</label>
                                        <select id="timeslot_duration" class="form-control" autocomplete="off" required>
                                            <option value="5">5 mins</option>
                                            <option value="10">10 mins</option>
                                            <option value="15">15 mins</option>
                                            <option value="20">20 mins</option>
                                            <option value="30">30 mins</option>
                                            <option value="45">45 mins</option>
                                            <option value="60">1 hour</option>
                                            <option value="90">1.5 hours</option>
                                            <option value="120">2 hours</option>
                                            <option value="150">2.5 hours</option>
                                            <option value="165">2.75 hours</option>
                                            <option value="180">3 hours</option>
                                            <option value="210">3.5 hours</option>
                                            <option value="240">4 hours</option>
                                            <option value="300">5 hours</option>
                                            <option value="360">6 hours</option>
                                            <option value="480">8 hours</option>
                                            <option value="1440">All Day</option>
                                        </select>
                                    </div>
                                    <div class="col-md-3  quantity-group mb-0">
                                        <label>Quantity</label>
                                        <input type="number" id="timeslot_quantity" class="form-control" min="1" autocomplete="off" required>
                                    </div>
                                    <div class="invalid-feedback startTime-feedback">Start Time is Required</div>
                                    <div class="invalid-feedback endTime-feedback">End Time is Required</div>
                                    <div class="invalid-feedback duration-feedback">Duration is Required</div>
                                    <div class="invalid-feedback quantity-feedback">Quantity must be at least 1</div>

                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Description</label>
                                        <textarea id="timeslot_description" class="form-control" autocomplete="off"></textarea>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Courtroom</label>
                                        <select id="timeslot_courtroom" class="form-control" autocomplete="off">
                                            <option value="">-</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>Restricted Motions</label>
                                        <select id="timeslot_restrictedMotions" multiple autocomplete="off"></select>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-12 text-end mb-0">
                                        <button type="button" class="btn btn-danger delete-button" id="deleteTimeslotPaneBtn"><i class="fas fa-trash"></i>Delete</button>
                                        <button type="button" class="btn btn-success" id="saveTimeslotPaneBtn"><i class="fas fa-save"></i>Save changes</button>
                                    </div>
                                </div>
                            </div>
                            <div id="eventTab" class="tab-pane fade form-group mb-0">
                                <input type="hidden" id="edit_eventId">
                                <div class="row edited-by" style="display: none;">
                                    <div class="col-md-6">
                                        <label>Edited By</label>
                                        <span id="event_editedBy"></span>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Updated On</label>
                                        <span id="event_updatedAt"></span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Addon</label>
                                        <input type="checkbox" id="event_addon_check" class="form-check-input" autocomplete="off">
                                        <input type="hidden" id="event_addon" value="0">
                                    </div>
                                    <div class="col-md-6">
                                        <label>Reminder</label>
                                        <input type="checkbox" id="event_reminder_check" class="form-check-input" autocomplete="off">
                                        <input type="hidden" id="event_reminder" value="0">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Motion</label>
                                        <select id="event_motion" class="form-control" autocomplete="off"></select>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Type</label>
                                        <select id="event_type" class="form-control" autocomplete="off"></select>
                                    </div>
                                </div>
                                <div class="row" id="other_motion_row" style="display: none;">
                                    <div class="col-md-12">
                                        <label>Other Motion</label>
                                        <input type="text" id="event_customMotion" class="form-control" autocomplete="off">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>Case Number</label>
                                        <div id="event_caseNum_container" class="d-flex">
                                            <asp:Literal ID="ltCaseNumber" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Attorney</label>
                                        <select id="event_attorney" autocomplete="off"></select>
                                        <div class="invalid-feedback">This Attorney is Required.</div>
                                    </div>
                                    <div class="col-md-6">
                                        <label>Opposing Attorney</label>
                                        <select id="event_opposingAttorney" autocomplete="off"></select>
                                        <div class="invalid-feedback">This Attorney is Required.</div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label class="plaintiff-label">Plaintiff</label>
                                        <input type="text" id="event_plaintiff" class="form-control" autocomplete="off">
                                        <div class="invalid-feedback plaintiff-feedback">Plaintiff is Required.</div>

                                    </div>
                                    <div class="col-md-6">
                                        <label class="defendant-label">Defendant</label>
                                        <input type="text" id="event_defendant" class="form-control" autocomplete="off">
                                        <div class="invalid-feedback defendant-feedback">Defendant is Required.</div>

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label class="plaintiff-email-label">Plaintiff Email</label>
                                        <input type="email" id="event_plaintiffEmail" class="form-control" autocomplete="off">
                                        <div class="invalid-feedback plaintiff-email-feedback">Plaintiff Email is Required.</div>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="defendant-email-label">Defendant Email</label>
                                        <input type="email" id="event_defendantEmail" class="form-control" autocomplete="off">
                                        <div class="invalid-feedback defendant-email-feedback">Defendant Email is Required.</div>
                                    </div>
                                </div>
                                <div id="court_template_fields" class="row">
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>Notes</label>
                                        <textarea id="event_notes" class="form-control" autocomplete="off"></textarea>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-12 text-end mb-0">
                                        <button type="button" class="btn btn-danger" id="cancelHearingBtn" style="display: none;"><i class="fas fa-times"></i>Cancel Hearing</button>
                                        <button type="button" class="btn btn-primary" id="rescheduleBtn" style="display: none;"><i class="fas fa-calendar"></i>Re-Schedule</button>
                                        <button type="button" class="btn btn-success" id="saveEventPaneBtn"><i class="fas fa-save"></i>Save changes</button>
                                    </div>
                                </div>
                            </div>
                            <div id="eventsTab" class="tab-pane fade mb-0">
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th>&nbsp;</th>
                                            <th>Case #</th>
                                            <th>Motion</th>
                                            <th>Attorney</th>
                                            <th>Plaintiff</th>
                                            <th>Opposing Attorney</th>
                                            <th>Defendant</th>
                                        </tr>
                                    </thead>
                                    <tbody id="eventsTableBody"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" forceprovider="DnnFormBottomProvider" priority="100" />
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/JACS/js/courtcalendar.js" forceprovider="DnnFormBottomProvider" priority="102" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/moment/moment.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/Bootstrap/bootstrap-datepicker.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/Bootstrap/bootstrap-datepicker.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/TomSelect/tom-select.default.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/TomSelect/tom-select.complete.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/fullcalendar/dist/index.global.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/Noty/noty.min.css" />
<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtCalendarController === 'undefined') {
                    console.error('CourtCalendarController is not defined.');
                    return;
                }
                const courtCalendarController = new CourtCalendarController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole:"<%=AdminRole%>",
                    service: service,
                    courtEditUrl: "<%=CourtEditUrl%>",
                    calendarUrl: "<%=CourtCalendarUrl%>",
                    userDefinedFieldUrl: "<%=UserDefinedFieldUrl%>",
                    truncateCalendarUrl: "<%=TruncateCalendarUrl%>",
                    courtId:<%=courtIdParam%>,
                    editable:"<%=Editable%>",
                    calendarItem:<%=JsonCalendarItem%>,
                });
                courtCalendarController.init();
            } catch (e) {
                console.error('Error initializing CourtCalendarController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
