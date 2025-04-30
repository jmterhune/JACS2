<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropInquiry" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Inquiry
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropInquiry">
            <li>
                <a class="dropdown-item" id="lnkInquiry" href="<%=InquiryUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqDesoto" href="<%=InquiryDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqManatee" href="<%=InquiryManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqSarasota" href="<%=InquirySarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <div class="btn-group" role="group">
        <button id="btnGroupDropDCR" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            DCR
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropDCR">
            <li>
                <a class="dropdown-item" id="lnkDCR" href="<%=DCRUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRDesoto" href="<%=DCRDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRManatee" href="<%=DCRManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRSarasota" href="<%=DCRSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropComplete" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Complete
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropComplete">
            <li>
                <a class="dropdown-item" id="lnkComplete" href="<%=CompleteUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompDesoto" href="<%=CompleteDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompManatee" href="<%=CompleteManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompSarasota" href="<%=CompleteSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
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
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/Datatables/datatables.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/Datatables/datatables.min.css" />
<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var userId = <%=UserId%>;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 2;
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
        $("#lnkInqDesoto").on("click", function (e) {
            e.preventDefault();
            listType = 4; searchType = -1; searchText = 'null'; countyId = 1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkInqManatee").on("click", function (e) {
            e.preventDefault();
            listType = 4; searchType = -1; searchText = 'null'; countyId = 3;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkInqSarasota").on("click", function (e) {
            e.preventDefault();
            listType = 4; searchType = -1; searchText = 'null'; countyId = 2;
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
        $("#lnkDCRDesoto").on("click", function (e) {
            e.preventDefault();
            listType = 2; searchType = -1; searchText = 'null'; countyId = 1;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkDCRManatee").on("click", function (e) {
            e.preventDefault();
            listType = 2; searchType = -1; searchText = 'null'; countyId = 3;
            SetSearchType(-1);
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkDCRSarasota").on("click", function (e) {
            e.preventDefault();
            listType = 2; searchType = -1; searchText = 'null'; countyId = 2;
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
        $("#lnkCompDesoto").on("click", function (e) {
            e.preventDefault();
            listType = 3; searchType = -1; searchText = 'null'; countyId = 1;
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkCompManatee").on("click", function (e) {
            e.preventDefault();
            listType = 3; searchType = -1; searchText = 'null'; countyId = 3;
            proceedingTable.draw();
            UpdateHeader();
        });
        $("#lnkCompSarasota").on("click", function (e) {
            e.preventDefault();
            listType = 3; searchType = -1; searchText = 'null'; countyId = 2;
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
                            return `<a class="delete" aria-role="button" title="Delete Record" data-id="${row.proceedingid}" href="#"><i class="fas fa-trash"></i></a>`;
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
                $.dnnConfirm({
                    text: 'Are you sure you wish to delete this Record?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Delete Record?',
                    callbackTrue: function () {
                        DeleteProceeding(proceedingId);
                    }
                });
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

    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
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
        switch (listType) {
            case 0:
                headerText = "Awaiting Payment";
                break;
            case 1:
                headerText = "Awaiting Notification";
                break;
            case 2:
                headerText = "Awaiting CD Creation";
                break;
            case 3:
                headerText = "Completed";
                break;
            case 4:
                headerText = "Awaiting Inquiry Processing";
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
