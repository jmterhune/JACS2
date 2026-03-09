<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.jacs.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="border-0 navbar mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Welcome to JACS!</h2>

</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="animated fadeIn">
            <div class="alert alert-info">
                <i class="fas fa-info-circle"></i>Use the sidebar to the left to create, edit or delete content.
            </div>
            <div class="input-group mb-2">
    <label for="case_num" class="input-group-text mb-0">Case #:</label>
    <input type="search" id="case_num" class="form-control" placeholder="Search by Case Number..." style="max-width: 200px;">
    
    <button type="button" id="search-button" class="btn btn-primary">
        <span class="btn-text">Find</span>
        <span class="spinner-border spinner-border-sm d-none" role="status" aria-hidden="true"></span>
    </button>
</div>
            <div class="animated">
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left">
                            <h4>Time Slots</h4>
                        </div>
                        <div style=""><span style=""><a href='<%=TimeSlotListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="tblTimeslot" class="table table-striped w-100 mb-3">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Court</th>
                                <th>Date/Time</th>
                                <th>Length</th>
                                <th>Available</th>
                                <th>Quantity</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left">
                            <h4>Events</h4>
                        </div>
                        <div><span><a href='<%=EventListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="tblEvent" class="table table-striped w-100 mb-3">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Case Number</th>
                                <th>Motion</th>
                                <th>Timeslot</th>
                                <th>Duration</th>
                                <th>Court</th>
                                <th>Status</th>
                                <th>Attorney</th>
                                <th>Opposing Attorney</th>
                                <th>Plaintiff</th>
                                <th>Defendant</th>
                                <th>Courtroom</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

    </main>
</div>
<!-- Modal for Case Search Results -->
<div class="modal fade" id="caseSearchModal" tabindex="-1" aria-labelledby="CaseSearchModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="CaseSearchModalLabel">Case Details</h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>

            </div>
            <div id="progress-search" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="Loading..." src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-body">
                <table id="caseDetailsTable" class="table table-striped mb-0">
                    <tbody>
                        <tr>
                            <td><strong>Case Number:</strong></td>
                            <td><span id="caseNumber"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Motion:</strong></td>
                            <td><span id="motionName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Timeslot:</strong></td>
                            <td><span id="timeslotDesc"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Court:</strong></td>
                            <td><span id="courtName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Status:</strong></td>
                            <td><span id="statusName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Attorney:</strong></td>
                            <td><span id="attorneyName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Opposing Attorney:</strong></td>
                            <td><span id="oppAttorneyName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Plaintiff:</strong></td>
                            <td><span id="plaintiffName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Defendant:</strong></td>
                            <td><span id="defendantName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Courtroom:</strong></td>
                            <td><span id="courtroomName"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="eventId" />
            </div>
            <div class="modal-footer  justify-content-between">
                <button type="button" class="btn btn-primary" id="cmdEditEvent"><i class="fas fa-edit me-2" aria-hidden="true"></i>&nbsp;Edit Event</button>
                <button type="button" id="cmdCancelEvent" class="btn btn-danger"><i class="fa fa-trash me-2" aria-hidden="true"></i>&nbsp;Cancel Event</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Page-level loading overlay -->
<div id="page-progress" class="modal-progress" style="display: none; position: fixed; z-index: 9999;">
    <div class="center-progress">
        <img alt="Loading..." src="/images/loading.gif" />
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="/DesktopModules/tjc.modules/JACS/js/dashboard.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/moment/moment-with-locales.js" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            setActiveLink("lnkMain");
            const dashboardController = new DashboardController({
                moduleId: moduleId,
                userId:<%=UserId%>,
                isJudge: "<%=IsJudge%>",
                service: service,
                eventEditUrl: "<%=CourtCalendarUrl%>",
                timeslotEditUrl: "<%=CourtCalendarUrl%>"
            });
            dashboardController.init();
        });
    }(jQuery, window.Sys));
</script>
