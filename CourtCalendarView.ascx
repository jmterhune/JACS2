<!-- Filename: Views/CourtCalendarView.ascx -->
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
            </div>
            <div class="calendar-actions d-inline-block">
                <a href="#" class="btn btn-default" id="deleteTimeslotsBtn"><i class="fas fa-trash"></i>Delete Timeslot(s)</a>
                <a href="#" class="btn btn-default" id="copyTimeslotsBtn"><i class="fas fa-copy"></i>Copy Timeslot(s)</a>
                <a href="#" class="btn btn-secondary" id="printCalendarBtn"><i class="fas fa-print"></i>Print Calendar View</a>
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
<!-- Reschedule Hearing Modal -->
<div class="modal fade" id="RescheduleHearingModal" tabindex="-1" aria-labelledby="RescheduleHearingModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="RescheduleHearingModalLabel">Reschedule Hearing</h5>
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
            <div class="modal-header">
                <h5 class="modal-title" id="TimeslotModalLabel">Create...</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <ul class="nav nav-tabs" id="myTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <button class="nav-link active" id="timeslot-tab" data-bs-toggle="tab" data-bs-target="#timeslotTab" type="button" role="tab" aria-controls="timeslotTab" aria-selected="true">Timeslot(s)</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button class="nav-link" id="event-tab" data-bs-toggle="tab" data-bs-target="#eventTab" type="button" role="tab" aria-controls="eventTab" aria-selected="false">Create Event</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button class="nav-link" id="events-tab" data-bs-toggle="tab" data-bs-target="#eventsTab" type="button" role="tab" aria-controls="eventsTab" aria-selected="false">Event(s)</button>
                    </li>
                </ul>
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active" id="timeslotTab" role="tabpanel" aria-labelledby="timeslot-tab">
                        <!-- Timeslot form content -->
                    </div>
                    <div class="tab-pane fade" id="eventTab" role="tabpanel" aria-labelledby="event-tab">
                        <!-- Event form content -->
                    </div>
                    <div class="tab-pane fade" id="eventsTab" role="tabpanel" aria-labelledby="events-tab">
                        <!-- Events table content -->
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/court.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courtcalendar.js" ForceProvider="DnnFormBottomProvider" Priority="102" />
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
                    adminRole: 'AdminRole',
                    service: service,
                    courtEditUrl: "/Court/Edit",
                    userDefinedFieldUrl: "/Court/CustomFields",
                    truncateCalendarUrl: "/Court/Truncate",
                    extendCalendarUrl: "/Court/Extend"
                });
                courtCalendarController.init();
            } catch (e) {
                console.error('Error initializing CourtCalendarController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
