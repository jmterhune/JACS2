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
                <i class="fas fa-info-circle"></i> Use the sidebar to the left to create, edit or delete content.
            </div>

            <div class="mb-3">
                <label class="form-label fw-bold">Search Case Number</label>
                <div id="event_caseNum_container" class="d-flex flex-wrap align-items-center gap-2">
                    <asp:DropDownList runat="server" ID="case_num_part1" ClientIDMode="Static" AppendDataBoundItems="true" cssclass="form-control case-num-part" style="max-width: 90px;" ToolTip="County (optional)">
                        <asp:ListItem Text="-" Value="" />
                    </asp:DropDownList>                    
                    <span class="align-self-center">-</span>
                    <input type="text" class="form-control case-num-part" id="case_num_part2" maxlength="4" placeholder="YYYY" style="max-width: 60px;" required title="Year (4 digits)" />
                    <span class="align-self-center">-</span>
                    <asp:DropDownList runat="server" ClientIDMode="Static" ToolTip="Case Type" AppendDataBoundItems="true" ID="case_num_part3" CssClass="form-control case-num-part" style="max-width: 60px;">
                        <asp:ListItem Text="Type" Value="" />
                    </asp:DropDownList>
                    
                    <span class="align-self-center">-</span>
                    <input type="text" class="form-control case-num-part" id="case_num_part4" maxlength="6" placeholder="######" style="max-width: 80px;" required title="Sequence (will be padded to 6 digits)" />
                    <span class="align-self-center">-</span>
                    <input type="text" class="form-control case-num-part" id="case_num_part5" maxlength="4" placeholder="xxxx" style="max-width: 60px;" title="Defendant/Party ID (optional)" />
                    <span class="align-self-center">-</span>
                    <input type="text" class="form-control case-num-part" id="case_num_part6" maxlength="2" placeholder="xx" style="max-width: 45px;" title="Branch/Location (optional)" />
                    <button type="button" id="search-button" class="btn btn-primary ms-2">
                        <span class="btn-text"><i class="fas fa-search me-1"></i>Search</span>
                        <span class="spinner-border spinner-border-sm d-none" role="status" aria-hidden="true"></span>
                    </button>
                </div>
                <small class="form-text text-muted mt-1 d-block">
                    Leave county blank if unknown. Partial matches are supported (at least year + type or sequence recommended).
                </small>
            </div>

            <div class="animated">
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left"><h4>Time Slots</h4></div>
                        <div style=""><span style=""><a href='<%=TimeSlotListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="tblTimeslot" class="table table-striped w-100 mb-3">
                        <thead>
                            <tr>
                                <th></th><th>Court</th><th>Date/Time</th><th>Length</th><th>Available</th><th>Quantity</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>

                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left"><h4>Events</h4></div>
                        <div><span><a href='<%=EventListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="tblEvent" class="table table-striped w-100 mb-3">
                        <thead>
                            <tr>
                                <th></th><th>Case Number</th><th>Motion</th><th>Timeslot</th><th>Duration</th>
                                <th>Court</th><th>Status</th><th>Attorney</th><th>Opposing Attorney</th>
                                <th>Plaintiff</th><th>Defendant</th><th>Courtroom</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
    </main>
</div>

<!-- Modal for Case Search Results -->
<div class="modal fade" id="caseSearchModal" tabindex="-1" aria-labelledby="CaseSearchModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="CaseSearchModalLabel">Search Results</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table id="tblCaseSearchResults" class="table table-striped table-hover w-100">
                    <thead>
                        <tr>
                            <th>Case Number</th>
                            <th>Motion</th>
                            <th>Court</th>
                            <th>Status</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
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
                userId: <%=UserId%>,
                isJudge: "<%=IsJudge%>",
                service: service,
                eventEditUrl: "<%=CourtCalendarUrl%>",
                timeslotEditUrl: "<%=CourtCalendarUrl%>"
            });
            dashboardController.init();
        });
    }(jQuery, window.Sys));
</script>