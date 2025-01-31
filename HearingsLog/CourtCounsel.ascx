<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtCounsel.ascx.cs" Inherits="tjc.Modules.HearingLog.CourtCounsel" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs tabs-center">
    <ul class="nav nav-tabs justify-content-center">
        <li class="nav-item">
            <asp:HyperLink ID="lnkHearingLog" CssClass="nav-link" runat="server" Text="Hearing Log" ToolTip="Select to View the 60 Day Hearing Log" />
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#" data-toggle="tab">Court Counsel Log</a>
        </li>
    </ul>
    <div class="tab-content">
        <div class="tab-pane active">
            <div class="ms-2 me-2">
                <div class="p-2 bg-light mb-3 border rounded border-secondary d-inline-block">
                    <div class="row g-3 align-items-center">
                        <label class="col-auto col-form-label" for="txtStartDate">Start Date</label>
                        <div class="col-auto">
                            <asp:TextBox ID="txtStartDate" ClientIDMode="Static" runat="server" Width="150" CssClass="form-control date-picker" MaxLength="15" aria-label="Start Date"></asp:TextBox>
                        </div>
                        <label class="col-auto col-form-label" for="txtEndDate">End Date</label>
                        <div class="col-auto">
                            <asp:TextBox ID="txtEndDate" ClientIDMode="Static" runat="server" Width="150" CssClass="form-control date-picker" MaxLength="15" aria-label="End Date"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="p-2 bg-light mb-3 ms-3 border rounded border-secondary d-inline-block float-end">
                    <div class="row g-3 align-items-center">
                        <label class="col-auto col-form-label" for="txtSearch">Search Text</label>
                        <div class="col-auto">
                            <input id="txtSearch" type="text" maxlength="50" class="form-control" placeholder="Enter Search Text" />
                        </div>
                        <div id="dvShowJudges" class="col-auto" style="display: none">
                            <div class="form-check form-switch">
                                <input class="form-check-input" type="checkbox" id="showAllJudges">
                                <label class="form-check-label" for="showAllJudges">Show All Judges</label>
                            </div>
                        </div>
                        <div id="dvChiefJudge" class="col-auto" style="display: none">
                            <asp:DropDownList ID="drpJudges" Visible="false" ClientIDMode="Static" CssClass="form-control" runat="server">
                                <asp:ListItem Text="< All Judges >" Value="0" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div id="statusOptionContainer" class="text-end">
                    <div class="dropdown ms-2 d-inline-block">
                        <button class="btn btn-default dropdown-toggle" type="button" id="columnVisibility" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                            Hidden Columns
                        <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu checkbox-menu allow-focus" aria-labelledby="columnVisibility">
                            <li>
                                <label>
                                    <input value="0" type="checkbox">
                                    Motion Filed
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="1" type="checkbox">
                                    60<sup>th</sup> Day
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="2" type="checkbox">
                                    Received By CC
                                </label>
                            </li>

                            <li>
                                <label>
                                    <input value="3" type="checkbox">
                                    Case Name
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="4" type="checkbox">
                                    Case #
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="5" type="checkbox">
                                    Type
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="6" type="checkbox">
                                    Case Status
                                </label>
                            </li>
                            <li>
                                <label>
                                    <input value="7" type="checkbox">
                                    Attorney
                                </label>
                            </li>
                        </ul>
                    </div>
                </div>
                <div id="process-overlay" class="overlay" style="display: none;">
                    <div class="spinner"></div>
                </div>
                <table id="tblHearing" class="table table-striped" width="100%">
                    <thead>
                        <tr>
                            <th>Motion Filed</th>
                            <th>60<sup>th</sup> Day</th>
                            <th>Received by
                    <abbr title="Court Counsel">CC</abbr></th>
                            <th>Case Name</th>
                            <th>Case #</th>
                            <th>Type</th>
                            <th>Case Status</th>
                            <th>Attorney</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/Bootstrap/bootstrap5-toggle.jquery.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/Bootstrap/bootstrap5-toggle.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var jaRole = '<%=JaRole%>';
    var hasChiefJudgeRole = <%=HasChiefJudgeRole%>;
    var selectedJudge = 0;
    var showAllJudges = false;
    var startDate = "";
    var endDate = "";
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "asc";
    var sortColumnIndex = 2;
    var currentPage = 0;
    var hearingTable = null;
    var hiddenColumns = null;
    var searchText = null;
    var service = {
        path: "HearingsLog",
        framework: $.ServicesFramework(moduleId)
    };
    (function ($, Sys) {

        $(document).ready(function () {
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        GetLocalStorage();
        $(".date-picker").datepicker();
        if (hasChiefJudgeRole)
            $("#dvShowJudges").show();
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var hearingAction = "GetCourtCounselItems";
        var restUrl = `${service.baseUrl}Hearing/${hearingAction}/${recordCount}`;
        $('#process-overlay').show();
        hearingTable = $('#tblHearing').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: restUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.startDate = startDate;
                    data.endDate = endDate;
                    data.jaRole = jaRole;
                    data.searchText = searchText;
                    if (showAllJudges) {
                        data.selectedJudge = selectedJudge;
                    }
                    delete data.columns;
                },
            },
            columns:
                [
                    { data: "motionfiled" },
                    { data: "sixtiethdaydate", orderable: false },
                    { data: "datereceived" },
                    { data: "casename" },
                    { data: "casenumber" },
                    { data: "casetype" },
                    { data: "casestatus" },
                    { data: "attorney" },
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
            fixedHeader: true,
        });
        hearingTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#process-overlay').hide();
        });
        $.fn.dataTable.ext.errMode = () => ShowAlert('Error Loading Data', 'Error while loading the table data. Please refresh');
        hearingTable.on('order.dt', function () {
            // This will show: "Ordering on column 1 (asc)", for example
            var order = hearingTable.order();
            localStorage.setItem('courtCounselLog.sortDirection', order[0][1]);
            localStorage.setItem('courtCounselLog.sortColumnIndex', order[0][0]);
        });
        hearingTable.on('page.dt', function () {
            var info = hearingTable.page.info();
            localStorage.setItem('courtCounselLog.currentPageIndex', info.page);
        });
        hearingTable.on('length.dt', function (e, settings, len) {
            currentPage = 0;
            localStorage.setItem('courtCounselLog.currentPageIndex', currentPage);
            localStorage.setItem('courtCounselLog.pageSize', len);
        });
        if (hiddenColumns != null && hiddenColumns != "") {
            var $checkboxMenu = $('.checkbox-menu input');
            hiddenColumns.split(',').forEach((checkboxValue) => {
                $checkboxMenu.filter('[value=' + checkboxValue + ']').prop('checked', true);
                hearingTable.column(parseInt(checkboxValue, 10)).visible(false);
            });
        }
        $("#txtStartDate").on("change", function (e) {
            CompareDates();
        });
        $("#txtEndDate").on("change", function (e) {
            CompareDates();
        });
        $('.checkbox-menu input').on("change", function (e) {
            e.preventDefault();
            var hiddenColumns = [];
            $('.checkbox-menu input').each(function (index) {
                if (this.checked)
                    hiddenColumns.push(this.value);
            });
            localStorage.setItem('courtCounselLog.hiddenColumns', hiddenColumns);

            let columnIdx = e.target.value;
            let isChecked = e.target.checked;
            let column = hearingTable.column(columnIdx);
            // Toggle the visibility
            column.visible(!isChecked);
        });
        $('#showAllJudges').on("click", function (e) {
            currentPage = 0;
            if (this.checked) {
                $('#dvChiefJudge').show();
                selectedJudge = 0;
                $('#drpJudges').val('0');
                showAllJudges = true;
            } else {
                $('#dvChiefJudge').hide();
                selectedJudge = 0;
                $('#drpJudges').val('0');
                showAllJudges = false;
            }
            hearingTable.draw();
        });
        $('#drpJudges').on("change", function (e) {
            $('#process-overlay').show();
            selectedJudge = parseInt(this.value, 10);
            hearingTable.draw();
        });
        $('#txtSearch').on("input", function (e) {
            e.preventDefault();
            $('#process-overlay').show();
            searchText = $(this).val();
            hearingTable.draw();
        });
        $('input[data-bs-toggle="toggle"]').bootstrapToggle();
    }
    function GetLocalStorage() {
        //var storageStartDate = localStorage.getItem('courtCounselLog.startDate');
        //var storageEndDate = localStorage.getItem('courtCounselLog.endDate');
        var storageCurrentPage = localStorage.getItem('courtCounselLog.currentPageIndex');
        var storagePageSize = localStorage.getItem('courtCounselLog.pageSize');
        var storageSortDirection = localStorage.getItem('courtCounselLog.sortDirection');
        var storageSortColumnIndex = localStorage.getItem('courtCounselLog.sortColumnIndex');
        var storageHiddenColumns = localStorage.getItem('courtCounselLog.hiddenColumns');
        if (storageHiddenColumns != null && storageHiddenColumns != undefined) {
            hiddenColumns = storageHiddenColumns;
        }
        //if (storageStartDate != null && storageStartDate != undefined)
        //    $("#txtStartDate").val(storageStartDate);
        //if (storageEndDate != null && storageEndDate != undefined)
        //    $("#txtEndDate").val(storageEndDate);
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
    function CompareDates() {
        var tempStartDate = $("#txtStartDate").val();
        var tempEndDate = $("#txtEndDate").val();
        if (isValidDate(tempStartDate) && isValidDate(tempEndDate)) {
            $('#process-overlay').show();
            if ((Date.parse(tempEndDate) < Date.parse(tempStartDate))) {
                $("#txtEndDate").val(endDate);
                $("#txtStartDate").val(startDate);
                $('#process-overlay').hide();
                ShowAlert("Invalid Date", "End Date MUST be greater than Start Date");
            } else {
                startDate = tempStartDate;
                endDate = tempEndDate;
                currentPage = 0;
                localStorage.setItem('courtCounselLog.currentPageIndex', currentPage);
                hearingTable.draw();
            }
        }
    }
    function isValidDate(dateString) {
        // Attempt to parse the string into a date
        const date = new Date(dateString);

        // Check if the resulting date is valid
        return !isNaN(date.getTime());
    }

</script>
