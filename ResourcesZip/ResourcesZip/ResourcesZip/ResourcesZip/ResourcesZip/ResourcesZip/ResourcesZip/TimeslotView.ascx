<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeslotView.ascx.cs" Inherits="tjc.Modules.jacs.TimeslotView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Timeslots</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="alert alert-info"><strong class="fs-3"><i class="fas fa-info-circle fs-4"></i>Note</strong><div class="mt-3">This section allows a user to see all timeslots for a particular calender.<br />
            By utilizing the filters a user can find timeslots that are available. </div>
        </div>
        <nav class="navbar navbar-expand-lg navbar-filters mb-0 pb-0 pt-0">
            <a class="nav-item d-none d-lg-block"><i class="fa fa-filter"></i></a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#bp-filters-navbar" aria-controls="bp-filters-navbar" aria-expanded="false" aria-label="Toggle filters">
                <i class="fa fa-filter"></i>Filters
            </button>
            <div class="collapse navbar-collapse" id="bp-filters-navbar">
                <ul class="nav navbar-nav">
                    <li filter-name="court" filter-type="select2" filter-key="courtId" class="nav-item dropdown">
                        <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Court <span class="caret"></span></a>
                        <div class="dropdown-menu p-0">
                            <div class="form-group border-filter mb-0">
                                <select id="courtFilter" name="courtFilter" class="form-control input-sm select2" placeholder="-" data-filter-key="courtId" data-filter-type="select2" data-filter-name="court" data-language="en" data-filter-enabled="true">
                                    <option value="">-</option>
                                </select>
                            </div>
                        </div>
                    </li>
                    <li class="nav-item dropdown" filter-name="from_to" filter-type="date_range" filter-key="fromTo">
                        <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Date Range <span class="caret"></span></a>
                        <div class="dropdown-menu p-0">
                            <div class="form-group border-filter mb-0">
                                <input type="text" id="dateRange" class="form-control" placeholder="Select date range">
                            </div>
                        </div>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="removeFilters" class="nav-link" style="display: none"><i class="fa fa-eraser"></i>Remove Filters</a>
                    </li>
                </ul>
            </div>
        </nav>
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
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/timeslot.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2-bootstrap-5-theme.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/select2/js/select2.full.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/moment/moment-with-locales.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/DatePicker/daterangepicker.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/DatePicker/daterangepicker.min.js" />


<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof TimeslotController === 'undefined') {
                    console.error('TimeslotController is not defined. Check if timeslot.js loaded correctly.');
                    return;
                }
                const timeslotController = new TimeslotController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    editUrl: "<%=CourtCalendarUrl%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 1,
                    sortDirection: "asc"
                });
                timeslotController.init();
            } catch (e) {
                console.error('Error initializing TimeslotController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
