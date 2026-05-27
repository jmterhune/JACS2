<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div id="navigationLinks" class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary active" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <a class="btn btn-primary" id="lnkInquiry" href="<%=InquiryUrl %>">Inquiry</a>
    <a class="btn btn-primary" id="lnkDCR" href="<%=DCRUrl %>">
        <abbr title="Digital Court Reporting">DCR</abbr></a>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <a class="btn btn-primary" id="lnkComplete" href="<%=CompleteUrl %>">Complete</a>
</div>

<div class="btn-group float-end" id="SearchForm" role="group" aria-label="Search">
    <div class="btn-group" role="group">
        <button id="btnSearchType" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
            Search Type
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnSearchType">
            <li><a class="dropdown-item search-type" href="#" data-st="-1">None</a></li>
            <li><a class="dropdown-item search-type" href="#" data-st="0">Case Name</a></li>
            <li><a class="dropdown-item search-type" href="#" data-st="1">Case Number</a></li>
            <li><a class="dropdown-item search-type" href="#" data-st="2">Tracking Number</a></li>
            <li><a class="dropdown-item search-type" href="#" data-st="3">Requestor Number</a></li>
        </ul>
    </div>
    <div id="swSearchTerm" class="input-group">
        <input type="text" id="txtSearchTerm" class="form-control" placeholder="Search Term" aria-label="Search Term" />
    </div>
    <button id="btnSearch" type="button" class="btn btn-primary">
        Search
    </button>
</div>
<label id="filterCounty" class="me-4">
    Filter
    <select id="ddlCounty" class="form-select form-select-sm">
        <option value="0">All Counties</option>
        <option value="1">DeSoto</option>
        <option value="3">Manatee</option>
        <option value="2">Sarasota</option>
    </select>

</label>
<div class="heading heading-border heading-middle-border heading-middle-border-center heading-border-lg">
    <h2 id="headerText"></h2>
</div>
<table id="tblProceedings" class="table table-striped">
    <thead>
        <tr>
            <th class="command-icon">&nbsp;</th>
            <th>County - Req. Date</th>
            <th>Requestor</th>
            <th>Case Name</th>
            <th>Case Number</th>
            <th>Proceeding Date</th>
            <th class="command-icon">&nbsp;</th>
        </tr>
    </thead>
</table>

<dnn:dnncssinclude runat="server" filepath="~/Resources/Libraries/jQuery-UI/01_13_02/Themes/jquery-ui.css" />
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
    var userId = <%=UserId%>;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 0;
    var isAdmin = "<%=IsAdmin%>";
    var adminRole = "<%=AdminRole%>";
    var currentPage = 0;
    var listType = <%=(int)ListType%>;
    var searchTypeText = "";
    var searchType = <%=(int)SearchType%>;
    var searchText = "<%=SearchText%>";
    var countyId = <%=CountyId%>;
    var proceedingsUrl = null;
    var proceedingTable = null;
    var baseUrl ="<%=AccountingUrl%>";
    var deleteUrl = null;
    var service = {
        path: "DCR",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceProceeding = {
        path: "Proceeding",
        framework: $.ServicesFramework(moduleId)
    };

    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            UpdateHeader();
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        service.baseUrl = service.framework.getServiceRoot(service.path);
        serviceProceeding.baseUrl = serviceProceeding.framework.getServiceRoot(serviceProceeding.path);
        proceedingsUrl = `${service.baseUrl}ProceedingListItem/GetProceedingListItems/${recordCount}`;
        deleteUrl = `${serviceProceeding.baseUrl}ProceedingListItem/Delete/`;

        $("#dialog").dialog({
            autoOpen: false,
            modal: true
        });
        $("#btnSearch").on("click", function (e) {
            e.preventDefault();
            if (searchType >= 0) {
                if ($("#txtSearchTerm").val() != '') {
                    searchText = $("#txtSearchTerm").val();
                    UpdateHeader();
                    proceedingTable.draw();
                } else {
                    ShowAlert("Invalid Criteria", "Please Enter text to Search For");
                }
            } else {
                $("#txtSearchTerm").val('');
                searchText = 'null';
                searchType = -1;
                UpdateHeader();
                proceedingTable.draw();
            }
        });
        $("#ddlCounty").on("change", function () {
            countyId = parseInt($(this).val(), 10) || 0;
            proceedingTable.draw();
            UpdateHeader();
        });
        $(".search-type").on("click", function (e) {
            e.preventDefault();
            var st = Number($(this).data('st'));
            SetSearchType(st);
        });
        $("#lnkAccounting").on("click", function (e) {
            e.preventDefault();
            listType = 0; searchType = -1; searchText = 'null'; countyId = -1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkInquiry").on("click", function (e) {
            e.preventDefault();
            listType = 4; searchType = -1; searchText = 'null'; countyId = -1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkDCR").on("click", function (e) {
            e.preventDefault();
            listType = 2; searchType = -1; searchText = 'null'; countyId = -1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkNotification").on("click", function (e) {
            e.preventDefault();
            listType = 1; searchType = -1; searchText = 'null'; countyId = -1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkComplete").on("click", function (e) {
            e.preventDefault();
            listType = 3; searchType = -1; searchText = 'null'; countyId = -1;
            proceedingTable.draw();
            UpdateHeader();
        });
        $(".datepicker").datepicker();
        proceedingTable = $('#tblProceedings').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: proceedingsUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.listType = listType;
                    data.searchType = searchType;
                    data.searchText = searchText;
                    data.countyId = countyId;
                    delete data.columns;
                },
            },
            columns: [
                {
                    data: "proceedingid", render: function (data, type, row, meta) {
                        var control = GetEditUrl();
                        return `<a title="View Proceeding" href="${baseUrl}/ctl/${control}/mid/${moduleId}/pid/${data}/listtype/${listType}/searchType/${searchType}/searchText/${searchText}/cid/${countyId}"><i class="fas fa-search"></i></a>`;
                    }, className: "command-item", orderable: false
                },
                {
                    data: "requestdateformatted", render: function (data, type, row, meta) {
                        var requestDate = new Date(row.requesteddate);
                        var currentDate = new Date();
                        var dueDate = new Date(new Date(requestDate).setMonth(requestDate.getMonth() + 3));
                        if (dueDate < currentDate)
                            return `<span class='text-danger'>${data}</span>`;
                        return data;
                    }, className: "fw-bold",
                },
                { data: "requestor" },
                { data: "casename" },
                { data: "casenumber" },
                { data: "proceedingdate" },
                {
                    data: "proceedingid", render: function (data, type, row, meta) {
                        if (isAdmin == "True")
                            return `<a class="delete text-danger" aria-role="button" title="Delete Record" data-id="${row.proceedingid}" href="#"><i class="fas fa-trash"></i></a>`;
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
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: pageSize,
            displayStart: currentPage * pageSize,

        });
        proceedingTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".delete").on("click", function (e) {
                e.preventDefault();
                proceedingId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Record?', text: 'Are you sure you wish to delete this Record?', icon: 'warning',
                    showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                    confirmButtonColor: '#d33'
                }).then(function (r) { if (r.isConfirmed) DeleteProceeding(proceedingId); });
                function DeleteProceeding(proceedingId) {
                    e.preventDefault();
                    $.ajax({
                        url: deleteUrl + proceedingId,
                        type: 'GET',
                        success: function (result) {
                            proceedingTable.draw();
                        },
                        error: function (error) {
                            ShowAlert("Error Deleting Record", error);
                        }
                    });
                }
            });
        });
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List", "Error while loading the table data. Please refresh");
        $(".dt-length").prepend($('#filterCounty'));

    }

    function ShowAlert(title, text) {
        Swal.fire({ title: title, html: text, icon: 'info', confirmButtonText: 'OK' });
    }
    function GetEditUrl() {
        switch (listType) {
            case 0:
                return "accounting";
                break;
            case 1:
                return "notification";
                break;
            case 2:
                return "cd";
                break;
            case 3:
                return "completed";
                break;
            case 4:
                return "inquiry";
                break;
            default:
        }
    }
    function UpdateHeader() {
        var headerText = "Awaiting Payment";
        $("#navigationLinks a.btn").removeClass("active");
        switch (listType) {
            case 0:
                headerText = "Awaiting Payment";
                $("#lnkAccounting").addClass("active");
                break;
            case 1:
                headerText = "Awaiting Notification";
                $("#lnkNotification").addClass("active");
                break;
            case 2:
                headerText = "Awaiting CD Creation"
                $("#lnkDCR").addClass("active");
                break;
            case 3:
                headerText = "Completed";
                $("#lnkComplete").addClass("active");
                break;
            case 4:
                headerText = "Awaiting Inquiry Processing";
                $("#lnkInquiry").addClass("active");
                break;
            default:
        }
        switch (countyId) {
            case 1:
                headerText += " - DeSoto";
                break;
            case 2:
                headerText += " - Sarasota";
                break;
            case 3:
                headerText += " - Manatee";
                break;
            default:
        }
        if (searchTypeText != "" & searchText != "null" & searchText != '')
            headerText = "Search Results for " + searchTypeText + searchText;
        $("#headerText").text(headerText);
    }
    function SetSearchType(searchTypeValue) {
        searchType = searchTypeValue;
        switch (searchType) {
            case -1:
                $("#txtSearchTerm").val('');
                searchText = 'null';
                searchTypeText = '';
                $("#btnSearchType").text("Search Type");
                break;
            case 0:
                searchTypeText = "Case Name Matching ";
                $("#btnSearchType").text("Case Name");
                break;
            case 1:
                searchTypeText = "Case Number Matching ";
                $("#btnSearchType").text("Case Number");
                break;
            case 2:
                searchTypeText = "Tracking Number Matching ";
                $("#btnSearchType").text("Tracking Number");
                break;
            case 3:
                searchTypeText = "Requestor Name Matching ";
                $("#btnSearchType").text("Requestor Name");
                break;
            default:
        }
    }

</script>
