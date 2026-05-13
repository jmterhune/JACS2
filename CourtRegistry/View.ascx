<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.CourtRegistry.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />

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
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var applicationId = -1;
    var year = -1;
    var statusId = -1;
    var lastName = null;
    var firstName = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 1; /* ID column */
    var currentPage = 0;
    var STATE_KEY = 'tjcAppListState_' + moduleId;
    var LAST_VIEWED_KEY = 'tjcLastViewedAppId_' + moduleId;
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
                    return `<a class="view-app text-primary" title="View Application" data-applicationid="${row.applicationid}" href="${url}/aid/${row.applicationid}"><i class="fas fa-search"></i></a>`;
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
                            return `<a class="delete confirm text-danger" aria-role="button" title="Delete Record" data-applicationid="${row.applicationid}" href="#"><i class="fas fa-trash"></i></a>`;
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
            stateSave: true,
            stateDuration: -1, /* sessionStorage */
            stateSaveCallback: function (settings, data) {
                try { sessionStorage.setItem(STATE_KEY, JSON.stringify(data)); } catch (e) { }
            },
            stateLoadCallback: function (settings) {
                try { return JSON.parse(sessionStorage.getItem(STATE_KEY)); } catch (e) { return null; }
            }
        });
        appTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            /* Mark "Open in detail view" so we save context before navigation,
               and highlight the row that the user last opened. */
            var lastId = sessionStorage.getItem(LAST_VIEWED_KEY);
            $('#tblApplications tbody tr').removeClass('last-viewed');
            if (lastId) {
                $('#tblApplications tbody a.view-app[data-applicationid="' + lastId + '"]')
                    .closest('tr').addClass('last-viewed');
            }
            $('a.view-app').off('click.viewApp').on('click.viewApp', function () {
                sessionStorage.setItem(LAST_VIEWED_KEY, $(this).data('applicationid'));
            });
            $(".confirm").off("click.swalDelete").on("click.swalDelete", function (e) {
                e.preventDefault();
                var applicationId = $(this).data("applicationid");
                Registry.confirm({
                    title: 'Delete Application?',
                    text: 'This action cannot be undone.',
                    icon: 'warning',
                    confirmText: 'Yes, delete',
                    confirmColor: '#d33'
                }, function () {
                    $.ajax({
                        url: deleteUrl + applicationId,
                        type: 'DELETE',
                        success: function () {
                            appTable.draw(false);
                            Registry.notify('Application deleted.', 'success');
                        },
                        error: function (err) {
                            Registry.notify('Error attempting delete: ' + (err.statusText || ''), 'error');
                        }
                    });
                });
            });
        });
        $.fn.dataTable.ext.errMode = function () { Registry.notify('Error while loading the table data. Please refresh.', 'error'); };
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
        Registry.notify(text || title, 'info');
    }
</script>
