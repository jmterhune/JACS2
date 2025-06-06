<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.CourtRegistry.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#applications" data-toggle="tab">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="applications" class="tab-pane active">

            <div class="bg-dark text-white border-dark rounded p-2 mb-2">
                <div class="row">
                    <div class="col-auto me-2 pt-2"><strong>Filter By:</strong></div>
                    <div class="col-auto">
                        <input type="number" id="txtApplicationId" tabindex="0" min="1" class="form-control search id-filter" placeholder="ID" maxlength="25" />
                    </div>
                    <div class="col-auto">
                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpYear" CssClass="form-control search">
                        </asp:DropDownList>
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtLastNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="Last Name" />
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtFirstNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="First Name" />
                    </div>

                    <div class="col-auto">
                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpStatus" CssClass="form-control search">
                        </asp:DropDownList>
                    </div>

                    <div class="col-auto">
                        <button type="button" tabindex="-1" class="btn btn-primary" id="cmdSearch">Filter</button>
                    </div>
                </div>
            </div>
            <table id="tblApplications" class="table table-striped">
                <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>ID</th>
                        <th>Period</th>
                        <th>Last Name</th>
                        <th>First Name</th>
                        <th>Created</th>
                        <th>Reviewed</th>
                        <th>Years On</th>
                        <th>Renewal</th>
                        <th>Guardian</th>
                        <th>Status</th>
                        <th>&nbsp;</th>
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
    var applicationId = -1;
    var year = -1;
    var statusId = 0;
    var lastName = null;
    var firstName = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 6;
    var currentPage = 0;
    var applicationUrl = null;
    var deleteUrl = null;
    var service = {
        path: "CourtRegistry",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceDelete = {
        path: "CourtRegistryDelete",
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
        serviceDelete.baseUrl = serviceDelete.framework.getServiceRoot(serviceDelete.path);
        applicationUrl = `${service.baseUrl}ApplicationAPI/GetApplicationListItems/${recordCount}`;
        deleteUrl = `${serviceDelete.baseUrl}ApplicationAPI/Delete/`;

        var appTable = $('#tblApplications').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: applicationUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.applicationId = applicationId;
                    data.status = statusId;
                    data.firstName = firstName;
                    data.lastName = lastName;
                    data.year = year;
                    delete data.columns;
                },
            },
            columns: [{
                data: "applicationid", render: function (data, type, row, meta) {
                    var url = "<%=EditUrl("app")%>";
                    return `<a title="View Application" href="${url}/aid/${row.applicationId}"><i class="fas fa-search"></i></a>`;
                }, className: "command-item", orderable: false
            },
                { data: "applicationid" },
                { data: "periodyear" },
                { data: "lastname" },
                { data: "firstname" },
                { data: "datecreated" },
                { data: "datereviewed" },
                { data: "yearsonregistry" },
                {
                    data: "isrenewal", render: function (data, type, row, meta) {
                        return data == true ? '<i class="fas fa-check-square"></i>' : '<i class="fas fa-square"></i>';
                    }
                },
                {
                    data: "isguardian", render: function (data, type, row, meta) {
                        return data == true ? '<i class="fas fa-check-square"></i>' : '<i class="fas fa-square"></i>';
                    }
                },
                {
                    data: "statusname", render: function (data, type, row, meta) {
                        return `<span class="${row.statusname.toLowerCase()}">${row.statusname}</span>`;
                    }
                },
                {
                    data: "applicationid", render: function (data, type, row, meta) {
                            return `<a class="delete confirm" aria-role="button" title="Delete Record" data-applicationid="${row.applicationId}" href="#""><i class="fas fa-trash"></i></a>`;
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
        appTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".confirm").on("click", function (e) {
                e.preventDefault();
                var appId = $(this).data("appId");
                $.dnnConfirm({
                    text: 'Are you sure you wish to delete this Application?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Delete Application?',
                    callbackTrue: function () {
                        deleteApplication(appId);
                    }
                });
                function deleteApplication(appId) {
                    $.ajax({
                        url: deleteUrl + appId,
                        type: 'DELETE',
                        success: function (result) {
                            appTable.draw();
                        },
                        error: function (error) {
                            alert(error);
                        }
                    });
                }
            });
            function deleteApplication(e, appId) {
                e.preventDefault();
                $.ajax({
                    url: deleteUrl + appId,
                    type: 'DELETE',
                    success: function (result) {
                        appTable.draw();
                    },
                    error: function (error) {
                        ShowAlert("Error Attempting Delete!", error);
                    }
                });
            }
        });
        $.fn.dataTable.ext.errMode = () => alert('Error while loading the table data. Please refresh');
        $("#drpStatus,#drpYear").on("change", function (e) {
            $("#cmdSearch").trigger("click");
        });
        $("#txtApplicationId,#txtLastNameSearch,#txtFirstNameSearch").on("blur", function (e) {
            $("#cmdSearch").trigger("click");
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            var applicationIdString = $("#txtApplicationId").val();
            var yearString = $("#drpYear").val();
            var statusString = $("#drpStatus").val();
            if (applicationIdString.length > 0)
                applicationId = Number(applicationIdString);
            else
                applicationId = -1;
            if (yearString.length > 0)
                year = Number(yearString);
            else
                year = -1
            if (statusString.length > 0)
                statusId = Number(statusString);
            else
                statusId = -1;
            lastName = $("#txtLastNameSearch").val();
            firstName = $("#txtFirstNameSearch").val();
            appTable.draw();
        });

        $(".date-picker").datepicker();
    }
   
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
