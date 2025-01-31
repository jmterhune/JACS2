<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#designation" data-toggle="tab">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportUrl%>">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="designation" class="tab-pane active">
            <table id="tblDesignations" class="table table-striped">
                <thead>
                    <tr>
                        <th class="command-icon">&nbsp;</th>
                        <th class="command-icon">&nbsp;</th>
                        <th>ID</th>
                        <th>Last Name</th>
                        <th>Case Number</th>
                        <th>County</th>
                        <th>Service Date</th>
                        <th>Acknowledgment Filed</th>
                        <th>Due Date</th>
                        <th>Transcript Filed</th>
                        <th>Created By</th>
                        <th>Archived</th>
                        <th>Archived</th>
                        <th class="command-icon" style="display: none">&nbsp;</th>
                    </tr>
                    <tr class="table-secondary">
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <input type="text" id="txtLastName" class="form-control" maxlength="25" /></td>
                        <td>
                            <input type="text" id="txtFirstName" class="form-control" maxlength="25" /></td>
                        <td>
                            <input type="text" id="txtCaseNumber" class="form-control" maxlength="25" /></td>
                        <td>
                            <select id="drpCounty" class="form-control">
                                <option value="">< Filter By County ></option>
                                <option>DeSoto</option>
                                <option>Manatee</option>
                                <option>Sarasota</option>
                            </select>
                        </td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                         <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</div>

<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var caseNumber = null;
    var lastName = null;
    var firstName = null;
    var county = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 3;
    var isAdmin = "<%=IsAdmin%>";
    var currentPage = 1;
    GetLocalStorage();
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
        var restUrl = `${service.baseUrl}Designation/GetDesignations/${recordCount}`;
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
                    delete data.columns;
                },
            },
            columns: [{
                data: "designationid", render: function (data, type, row, meta) {
                    var url = "<%=EditUrl("status")%>";
                    return `<a title="Change Status" onclick="SetdesignationId(${data})" href="${url}/did/${data}"><i class="fas fa-search"></i></a>`;
                }, className: "command-item", orderable: false
            },
                {
                    data: "designationid", render: function (data, type, row, meta) {
                        var url = "<%=EditUrl("designation")%>";
                        return `<a title="Edit Designation" onclick="SetdesignationId(${data})" href="${url}/did/${data}"><i class="fas fa-pencil"></i></a>`;
                    }, className: "command-item", orderable: false
                },
                { data: "lastname" },
                { data: "firstname" },
                { data: "casenumber" },
                { data: "county" },
                { data: "servicedate" },
                {
                    data: "acknowledgmentfiled", render: function (data, type, row, meta) {
                        return data == 'true' ? `<a class="acknowledge" href="" title="Set Acknowledgment to Unfiled" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="acknowledge" href="#" title="Set Acknowledgment to Filed" data-id="${row.designationid}"><i class="fas fa-square"></i></a>`;
                    }, orderable: false
                },
                { data: "duedate" },
                { data: "transcriptfiled" },
                { data: "createdusername" },
                {
                    data: "archived", render: function (data, type, row, meta) {
                        return data == 'true' ? `<a class="archive" href="#" title="Set Status to Unarchived" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="archive" href="#" title="Set Status to Unarchived" data-id="${row.designationid}" ><i class="fas fa-square"></i></a>`;
                    }, orderable: false
                },
                {
                    data: "comment", render: function (data, type, row, meta) {
                            return data == '' ? '' : '<i class="fas fa-comment-alt" data-html="true" title="' + data + '" data-toggle="tooltip" ></i>';
                    }, className: "command-item", orderable: false
                },
                {
                    data: "designationId", render: function (data, type, row, meta) {
                        if (isAdmin == "true")
                            return '<a class="delete" aria-role="button" title="Delete Record" data-designationId="' + data + '" href="#""><i class="fas fa-trash"></i></a>';
                        return '';
                    }, className: "command-item", orderable: false
                },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[sortColumnIndex, sortDirection]],
            serverSide: true,
            process: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: pageSize,
            displayStart: currentPage * pageSize,
        });
        designationTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".delete").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                $.dnnConfirm({
                    text: 'Are you sure you wish to delete this Designation?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Delete Designation?',
                    callbackTrue: function () {
                        DeleteDesignation(designationId);
                    }
                });
                function DeleteDesignation(designationId) {
                    e.preventDefault();
                    $.ajax({
                        url: deleteUrl + designationId,
                        type: 'GET',
                        success: function (result) {
                            designationTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Deleting Designation",error);
                        }
                    });
                }
            });
            $(".archive").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                $.dnnConfirm({
                    text: 'Are you sure you wish to change the Archive status?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Change Archive Status?',
                    callbackTrue: function () {
                        ToggleArchiveStatus(designationId);
                    }
                });
                function ToggleArchiveStatus(designationId) {
                    $.ajax({
                        url: archiveUrl + designationId,
                        type: 'GET',
                        success: function (result) {
                            designationTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Changing Archive Status",error);
                        }
                    });
                }
            });
            $(".acknowledge").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("designationId");
                $.dnnConfirm({
                    text: 'Are you sure you wish to change the Acknowledgement status?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Change Acknowledgement Status?',
                    callbackTrue: function () {
                        ToggleAcknowledgmentStatus(designationId);
                    }
                });
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
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List","Error while loading the table data. Please refresh");
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
    function GetLocalStorage() {
        storageCurrentPage = localStorage.getItem('tranascript.currentPageIndex');
        storagePageSize = localStorage.getItem('tranascript.pageSize');
        storageSortDirection = localStorage.getItem('tranascript.sortDirection');
        storageSortColumnIndex = localStorage.getItem('tranascript.sortColumnIndex');
        if (storageCurrentPage != null && storageCurrentPage != undefined)
            currentPage = storageCurrentPage;
        if (storagePageSize != null && storagePageSize != undefined)
            pageSize = storagePageSize;
        if (storageSortDirection != null && storageSortDirection != undefined)
            sortDirection = storageSortDirection;
        if (storageSortColumnIndex != null && storageSortColumnIndex != undefined)
            sortColumnIndex = storageSortColumnIndex;
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
