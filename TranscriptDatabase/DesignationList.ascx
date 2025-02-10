<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DesignationList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.DesignationList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#designation" data-toggle="tab">Designations</a>
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
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="designation" class="tab-pane active">
            <div class="bg-dark text-white border-dark rounded p-2 mb-2">
                <div class="row"><div class="col-auto me-2 pt-2"><strong>Filter By:</strong></div>
                    <div class="col">
                        <input type="text" id="txtLastName" class="form-control" maxlength="25" placeholder="Last Name" />
                    </div>
                    <div class="col">
                        <input type="text" id="txtFirstName" class="form-control" maxlength="25" placeholder="First Name" />
                    </div>
                    <div class="col">
                        <input type="text" id="txtCaseNumber" class="form-control" maxlength="25" placeholder="Case Number" />
                    </div>
                    <div class="col-auto">
                        <select id="drpCounty" class="form-control">
                            <option value="">< Filter By County ></option>
                            <option>DeSoto</option>
                            <option>Manatee</option>
                            <option>Sarasota</option>
                        </select>
                    </div>
                    <div class="col-auto pt-2">
                        <input type="checkbox" id="chkArchive" name="chkArchive" class="form-check-input"><label class="form-check-label ms-2" for="chkArchive">Show Archived</label>
                    </div>
                    <div class="col-auto">
                        <button type="button" class="btn btn-primary" id="cmdSearch">Filter</button></div>
                </div>
            </div>
            <button id="btnAdd" class="btn btn-primary me-3" data-toggle="modal" data-target="#designationModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Designation</button>
            <table id="tblDesignations" class="table table-striped">
                <thead>
                    <tr>
                        <th class="command-icon">&nbsp;</th>
                        <th class="command-icon">&nbsp;</th>
                        <th>ID</th>
                        <th>Last Name</th>
                        <th>First Name</th>
                        <th>Case Number</th>
                        <th>County</th>
                        <th>Service Date</th>
                        <th>Acknowledgment Filed</th>
                        <th>Due Date</th>
                        <th>Transcript Filed</th>
                        <th>Created By</th>
                        <th>Archived</th>
                        <th class="command-icon">&nbsp;</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var caseNumber = null;
    var lastName = null;
    var firstName = null;
    var county = null;
    var archived = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 2;
    var isAdmin = "<%=IsAdmin%>";
    var currentPage = 0;
    var service = {
        path: "TranscriptDatabase",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceDelete = {
        path: "TranscriptDelete",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceToggle = {
        path: "TranscriptToggle",
        framework: $.ServicesFramework(moduleId)
    };
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
        service.baseUrl = service.framework.getServiceRoot(service.path);
        serviceDelete.baseUrl = service.framework.getServiceRoot(serviceDelete.path);
        serviceToggle.baseUrl = service.framework.getServiceRoot(serviceToggle.path);

        var restUrl = `${service.baseUrl}DesignationListItem/GetDesignationListItems/${recordCount}`;
        var deleteUrl = `${serviceDelete.baseUrl}DesignationListItem/Delete/`;
        var archiveUrl = `${serviceToggle.baseUrl}DesignationListItem/Archive/`;
        var acknowledgeUrl = `${serviceToggle.baseUrl}DesignationListItem/Acknowledge/`;
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
                        var url = "<%=EditUrl("designation")%>";
                        return `<a title="Edit Designation" onclick="SetdesignationId(${data})" href="${url}/did/${data}"><i class="fas fa-pencil"></i></a>`;
                    }, className: "command-item", orderable: false
                },
                { data: "designationid" },
                { data: "lastname" },
                { data: "firstname" },
                { data: "casenumber" },
                { data: "county" },
                { data: "servicedate" },
                {
                    data: "acknowledgmentfiled", render: function (data, type, row, meta) {
                        return data == true ? `<a class="acknowledge" href="" title="Set Acknowledgment to Unfiled" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="acknowledge" href="#" title="Set Acknowledgment to Filed" data-id="${row.designationid}"><i class="fas fa-square"></i></a>`;
                    }, orderable: false
                },
                { data: "duedate" },
                { data: "transcriptfiled" },
                { data: "createdbyusername" },
                {
                    data: "archived", render: function (data, type, row, meta) {
                        return data == true ? `<a class="archive" href="#" title="Set Status to Unarchived" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="archive" href="#" title="Set Status to Archived" data-id="${row.designationid}" ><i class="fas fa-square"></i></a>`;
                    }, orderable: false
                },
                {
                    data: "designationId", render: function (data, type, row, meta) {
                        if (isAdmin == "True")
                            return `<a class="delete" aria-role="button" title="Delete Record" data-id="${row.designationid}" href="#"><i class="fas fa-trash"></i></a>`;
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
                var designationId = $(this).data("id");
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
                            ShowAlert("Error Deleting Designation", error);
                        }
                    });
                }
            });
            $(".archive").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("id");
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
                            ShowAlert("Error Changing Archive Status", error);
                        }
                    });
                }
            });
            $(".acknowledge").on("click", function (e) {
                e.preventDefault();
                var designationId = $(this).data("id");
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
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List", "Error while loading the table data. Please refresh");
        $('#btnAdd').on("click", function (e) {
            ClearEditLogForm();
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            caseNumber = $("#txtCaseNumber").val();
            lastName = $("#txtLastName").val();
            firstName = $("#txtFirstName").val();
            county = $("#drpCounty").val();
            archived = $("#chkArchive").is(':checked')
            designationTable.draw();
        });
        $("#tblDesignations_length").prepend($('#btnAdd'));
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
