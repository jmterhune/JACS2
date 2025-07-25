<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtCalendarView.ascx.cs" Inherits="tjc.Modules.jacs.CourtCalendarView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Court Calendar</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <h3 class="mb-2">Court Name: <span class="text-capitalize">
            <asp:Literal ID="ltCourtName" runat="server" /></span></h3>
        <div class="court-header d-flex mb-4">
            <div class="court-actions me-auto">
                <a href="#" class="btn btn-primary" id="editCourtBtn"><i class="fas fa-edit"></i>Edit</a>
                <a href="#" class="btn btn-primary" id="userDefinedFieldsBtn"><i class="fas fa-cog"></i>User Defined Fields</a>
                <a href="#" class="btn btn-primary" id="truncateBtn"><i class="fas fa-trash"></i>Truncate</a>
                <a href="#" class="btn btn-primary" id="icalExportBtn"><i class="fas fa-calendar"></i>iCal export</a>
                <a href="#" class="btn btn-primary" id="monthlyExportBtn"><i class="fas fa-file-export"></i>Monthly Export</a>
                <a href="#" class="btn btn-primary" id="extendBtn"><i class="fas fa-expand"></i>Extend</a>
                <a href="#" class="btn btn-primary" id="printCalendarBtn"><i class="fas fa-print"></i>Print Calendar View</a>
            </div>
            <div class="calendar-actions d-inline-block">
                <a href="#" class="btn btn-default" id="deleteTimeslotsBtn"><i class="fas fa-trash"></i>Delete Timeslot(s)</a>
                <a href="#" class="btn btn-default" id="copyTimeslotsBtn"><i class="fas fa-copy"></i>Copy Timeslot(s)</a>
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

        <!-- Reschedule Hearing Modal -->
        <div class="modal fade" id="RescheduleHearingModal" tabindex="-1" aria-labelledby="RescheduleHearingModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div id="progress-hearing" class="modal-progress" style="display: none;">
                        <div class="center-progress">
                            <img alt="" src="/images/loading.gif" />
                        </div>
                    </div>
                    <div class="modal-header">
                        <h4 class="modal-title" id="RescheduleHearingModalLabel">Reschedule Hearing</h4>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="container-fluid">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>Close</label>
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Timeslot Modal -->
        <div class="modal fade" id="TimeslotModal" tabindex="-1" aria-labelledby="TimeslotModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
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
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Edited By</label>
                                                <input type="text" id="event_editedBy" class="form-control" disabled autocomplete="off">
                                            </div>
                                            <div class="col-md-6">
                                                <label>Updated On</label>
                                                <input type="text" id="event_updatedOn" class="form-control" disabled autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Block</label>
                                                <input type="checkbox" id="timeslot_block" class="form-check-input" autocomplete="off">
                                            </div>
                                            <div class="col-md-6">
                                                <label>Public Block</label>
                                                <input type="checkbox" id="timeslot_publicBlock" class="form-check-input" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <label>Block Reason</label>
                                                <textarea id="timeslot_blockReason" class="form-control" autocomplete="off"></textarea>
                                            </div>
                                        </div>
                                        <div class="row cattle-call">
                                            <div class="col-md-6">
                                                <label>Concurrent/Consecutive</label>
                                                <select id="timeslot_concurrent" class="form-control" autocomplete="off">
                                                    <option value="yes">Yes (Concurrent)</option>
                                                    <option value="no">No (Consecutive)</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Start Time</label>
                                                <input type="datetime-local" id="timeslot_startTime" class="form-control" autocomplete="off">
                                            </div>
                                            <div class="col-md-6">
                                                <label>End Time</label>
                                                <input type="datetime-local" id="timeslot_endTime" class="form-control" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Duration</label>
                                                <select id="timeslot_duration" class="form-control" autocomplete="off">
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
                                                </select>
                                            </div>
                                            <div class="col-md-6">
                                                <label>Quantity</label>
                                                <input type="number" id="timeslot_quantity" class="form-control" min="1" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <label>Description</label>
                                                <textarea id="timeslot_description" class="form-control" autocomplete="off"></textarea>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <label>Category</label>
                                                <select id="timeslot_category" class="form-control" autocomplete="off">
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
                                                <button type="button" class="btn btn-danger" id="deleteTimeslotPaneBtn"><i class="fas fa-trash"></i>Delete</button>
                                                <button type="button" class="btn btn-success" id="saveTimeslotPaneBtn"><i class="fas fa-save"></i>Save changes</button>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="eventTab" class="tab-pane fade form-group mb-0">
                                        <input type="hidden" id="edit_eventId">
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
                                        <div class="row">
                                            <div class="col-md-12">
                                                <label>Other Motion</label>
                                                <input type="text" id="event_otherMotion" class="form-control" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Attorney</label>
                                                <select id="event_attorney" autocomplete="off"></select>
                                            </div>
                                            <div class="col-md-6">
                                                <label>Opposing Attorney</label>
                                                <select id="event_opposingAttorney" autocomplete="off"></select>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Plaintiff</label>
                                                <input type="text" id="event_plaintiff" class="form-control" autocomplete="off">
                                            </div>
                                            <div class="col-md-6">
                                                <label>Defendant</label>
                                                <input type="text" id="event_defendant" class="form-control" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <label>Plaintiff Email</label>
                                                <input type="email" id="event_plaintiffEmail" class="form-control" autocomplete="off">
                                            </div>
                                            <div class="col-md-6">
                                                <label>Defendant Email</label>
                                                <input type="email" id="event_defendantEmail" class="form-control" autocomplete="off">
                                            </div>
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
                                                    <th>Case #</th>
                                                    <th>Motion</th>
                                                    <th>Attorney</th>
                                                    <th>Plaintiff</th>
                                                    <th>Opposing Attorney</th>
                                                    <th>Defendant</th>
                                                    <th>Actions</th>
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
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/court.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courtCalendar.js" ForceProvider="DnnFormBottomProvider" Priority="102" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/TomSelect/tom-select.default.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/TomSelect/tom-select.complete.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/fullcalendar/dist/index.global.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
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
                if (typeof CourtCalendarController === 'undefined') {
                    console.error('CourtCalendarController is not defined.');
                    return;
                }
                const courtCalendarController = new CourtCalendarController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    courtEditUrl: "<%=CourtEditUrl%>",
                    userDefinedFieldUrl: "<%=UserDefinedFieldUrl%>",
                    truncateCalendarUrl: "<%=TruncateCalendarUrl%>",
                    extendCalendarUrl: "<%=ExtendCalendarUrl%>",
                });
                courtCalendarController.init();
            } catch (e) {
                console.error('Error initializing CourtCalendarController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>