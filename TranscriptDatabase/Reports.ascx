<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CalendartUrl%>">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormListUrl%>">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=HearingListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#reports" data-toggle="tab">Reporting</a>
        </li>        <li class="nav-item">
    <a class="nav-link" href="https://jud12fl.sharepoint.com/sites/CourtReporting">Team Site</a>
</li>
    </ul>
    <div id="reports" class="tab-content pb-0">
        <div id="designation" class="tab-pane active">
            <table id="tblDesignations" class="table table-striped">
                <thead>
                    <tr>
                        <th class="command-icon">&nbsp;</th>
                        <th class="command-icon">&nbsp;</th>
                        <th>ID</th>
                        <th>Last Name</th>
                        <th>First Name</th>
                        <th>County</th>
                        <th>Service Date</th>
                        <th>Acknowledgment Filed</th>
                        <th>Due Date</th>
                        <th>Transcript Filed</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</div>

<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var caseNumber = null;
    var lastName = null;
    var firstName = null;
    var county = null;
    var pageSize = 25;
    var recordCount = 0;
    var archived = null;
    var sortDirection = "desc";
    var sortColumnIndex = 3;
    var isAdmin = "<%=IsAdmin%>";
    var currentPage = 1;
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit();
        });
    }(jQuery, window.Sys));

    // Execute a function when the user presses a key on the keyboard
    document.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            document.getElementById("cmdSearch").click();
        }
    });
    function PageInit() {
        var service = {
            path: "TranscriptDatabase",
            framework: $.ServicesFramework(moduleId)
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);
        restUrl = `${service.baseUrl}DesignationListItem/GetDesignationListItems/${recordCount}`;
        var deleteUrl = `${service.baseUrl}Designation/Delete/`;
        var archiveUrl = `${service.baseUrl}Designation/Archive/`;
        var acknowledgeUrl = `${service.baseUrl}Designation/Acknowledge/`;
        var designationTable = $('#tblDesignations').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: restUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.firstName = firstName;
                    data.lastName = lastName;
                    data.caseNumber = caseNumber;
                    data.county = county;
                    data.archived = archived;
                    delete data.columns;
                },
            },
            columns: [
                {
                    data: "designationid", render: function (data, type, row, meta) {
                        var url = "<%=EditUrl("status")%>";
                        return `<a title="Change Status" onclick="SetdesignationId(${data})" href="${url}/did/${data}"><i class="fas fa-search"></i></a>`;
                    }, className: "command-item", orderable: false
                },
                {
                    data: "designationid", render: function (data, type, row, meta) {
                        var url = "<%=EditUrl()%>";
                        return `<a class="text-primary" title="Edit Designation" onclick="SetdesignationId(${data})" href="${url}/did/${data}"><i class="fas fa-edit"></i></a>`;
                    }, className: "command-item", orderable: false
                },
                { data: "designationid" },
                { data: "lastname" },
                { data: "firstname" },
                { data: "county" },
                { data: "servicedate" },
                {
                    data: "acknowledgmentfiled", render: function (data, type, row, meta) {
                        return data == true ? `<i class="fas fa-check-square"></i>` : `<i class="fas fa-square"></i>`;
                    }
                },
                { data: "duedate" },
                { data: "transcriptfiled" },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[sortColumnIndex, sortDirection]],
            serverSide: true,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: pageSize,
            displayStart: currentPage * pageSize,
        });
        designationTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".delete").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                Swal.fire({
                    title: 'Delete Designation?',
                    text: 'Are you sure you wish to delete this Designation?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No',
                    confirmButtonColor: '#d33'
                }).then(function (r) { if (r.isConfirmed) DeleteDesignation(designationId); });
                function DeleteDesignation(designationId) {
                    e.preventDefault();
                    $.ajax({
                        url: deleteUrl + designationId,
                        type: 'GET',
                        success: function (result) {
                            designationTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Deleting Designation", error);
                        }
                    });
                }
            });
            $(".archive").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                Swal.fire({
                    title: 'Change Archive Status?',
                    text: 'Are you sure you wish to change the Archive status?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then(function (r) { if (r.isConfirmed) ToggleArchiveStatus(designationId); });
                function ToggleArchiveStatus(designationId) {
                    $.ajax({
                        url: archiveUrl + designationId,
                        type: 'GET',
                        success: function (result) {
                            designationTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Changing Archive Status", error);
                        }
                    });
                }
            });
            $(".acknowledge").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                Swal.fire({
                    title: 'Change Acknowledgement Status?',
                    text: 'Are you sure you wish to change the Acknowledgement status?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then(function (r) { if (r.isConfirmed) ToggleAcknowledgmentStatus(designationId); });
                function ToggleAcknowledgmentStatus(designationId) {
                    $.ajax({
                        url: acknowledgeUrl + designationId,
                        type: 'GET',
                        success: function (result) {
                            designationTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Changing Acknowledgement Status", error);
                        }
                    });
                }
            });
        });
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List", "Error while loading the table data. Please refresh");
        designationTable.on('order.dt', function () {
            // This will show: "Ordering on column 1 (asc)", for example
            var order = designationTable.order();
            localStorage.setItem('tranascript.sortDirection', order[0][1]);
            localStorage.setItem('tranascript.sortColumnIndex', order[0][0]);
        });
        designationTable.on('page.dt', function () {
            var info = designationTable.page.info();
            localStorage.setItem('tranascript.currentPageIndex', info.page);
        });
        designationTable.on('length.dt', function (e, settings, len) {
            localStorage.setItem('tranascript.pageSize', len);
        });
    }
    function SetDesignationId(designationId) {
        localStorage.setItem('transcript.designationId', designationId);
    }
    function ShowAlert(title, text) {
        Swal.fire({
            title: title,
            html: text,
            icon: 'info',
            confirmButtonText: 'OK'
        });
    }
</script>
