<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventView.ascx.cs" Inherits="tjc.Modules.jacs.EventView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Events</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
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
                            <div class="form-group backpack-filter mb-0">
                                <select id="courtFilter" name="courtFilter" class="form-control input-sm select2" placeholder="-" data-filter-key="courtId" data-filter-type="select2" data-filter-name="court" data-language="en" data-filter-enabled="true">
                                    <option value="">-</option>
                                </select>
                            </div>
                        </div>
                    </li>
                    <li filter-name="category" filter-type="select2" filter-key="categoryId" class="nav-item dropdown">
                        <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Category <span class="caret"></span></a>
                        <div class="dropdown-menu p-0">
                            <div class="form-group backpack-filter mb-0">
                                <select id="categoryFilter" name="categoryFilter" class="form-control input-sm select2" placeholder="-" data-filter-key="categoryId" data-filter-type="select2" data-filter-name="category" data-language="en" data-filter-enabled="true">
                                    <option value="">-</option>
                                </select>
                            </div>
                        </div>
                    </li>
                    <li filter-name="status" filter-type="select2" filter-key="statusId" class="nav-item dropdown">
                        <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Status <span class="caret"></span></a>
                        <div class="dropdown-menu p-0">
                            <div class="form-group backpack-filter mb-0">
                                <select id="statusFilter" name="statusFilter" class="form-control input-sm select2" placeholder="-" data-filter-key="statusId" data-filter-type="select2" data-filter-name="status" data-language="en" data-filter-enabled="true">
                                    <option value="">-</option>
                                </select>
                            </div>
                        </div>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="removeFilters" class="nav-link" style="display:none"><i class="fa fa-eraser"></i>Remove Filters</a>
                    </li>
                </ul>
            </div>
        </nav>
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
                    <th>Category</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/event.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/buttons.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.buttons.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/buttons.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/buttons.colVis.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/jszip.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/pdfmake.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/vfs_fonts.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/buttons.html5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/buttons.print.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2-bootstrap-5-theme.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/select2/js/select2.full.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/Noty/noty.min.css" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof EventController === 'undefined') {
                    console.error('EventController is not defined. Check if event.js loaded correctly.');
                    return;
                }
                const eventController = new EventController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    editUrl: "<%=EventEditUrl%>",
                    calendarUrl: "<%=EventCalendarUrl%>",
                    revisionsUrl: "<%=EventRevisionUrl%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 1,
                    sortDirection: "asc"
                });
                eventController.init();
            } catch (e) {
                console.error('Error initializing EventController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>