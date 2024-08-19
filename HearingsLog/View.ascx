<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.HearingLog.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="ms-2 me-2">
    <asp:HyperLink ID="lnkAdmin" runat="server" Visible="false" CssClass="btn btn-primary"><i class="fas fa-cog"></i> Admin</asp:HyperLink>
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
            <div class="col-auto">
                <button id="btnImport" class="btn btn-tertiary ms-2 me-2">
                    <i class="fa-solid fa-file-import" aria-hidden="true"></i>&nbsp;Import Hearings
                </button>
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
    <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
    <button id="btnAdd" class="btn btn-primary me-3" data-toggle="modal" data-target="#logModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Hearing</button>
    <div id="statusOptionContainer" class="text-end">
        <div class="d-inline-block">
            <input type="radio" class="btn-check" name="statusOptions" value="0" id="newOption" autocomplete="off" checked>
            <label class="btn btn-secondary mt-2" for="newOption">New</label>

            <input type="radio" class="btn-check" name="statusOptions" value="1" id="archiveOption" autocomplete="off">
            <label class="btn btn-secondary mt-2" for="archiveOption">Archived</label>

            <input type="radio" class="btn-check" name="statusOptions" value="2" id="excludedOption" autocomplete="off">
            <label class="btn btn-secondary mt-2" for="excludedOption">Excluded</label>
        </div>
        <div class="dropdown ms-2 d-inline-block">
            <button class="btn btn-default dropdown-toggle" type="button" id="columnVisibility" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                Hidden Columns
                        <span class="caret"></span>
            </button>
            <ul class="dropdown-menu checkbox-menu allow-focus" aria-labelledby="columnVisibility">
                <li>
                    <label>
                        <input value="1" type="checkbox">
                        Order Signed
                    </label>
                </li>
                <li>
                    <label>
                        <input value="2" type="checkbox">
                        Hearing Date
                    </label>
                </li>
                <li>
                    <label>
                        <input value="3" type="checkbox">
                        60<sup>th</sup> Day
                    </label>
                </li>
                <li>
                    <label>
                        <input value="4" type="checkbox">
                        County
                    </label>
                </li>
                <li>
                    <label>
                        <input value="5" type="checkbox">
                        Case Name
                    </label>
                </li>
                <li>
                    <label>
                        <input value="6" type="checkbox">
                        Case #
                    </label>
                </li>
                <li>
                    <label>
                        <input value="7" type="checkbox">
                        <abbr title="Document Identification Number">DIN</abbr>
                    </label>
                </li>
                <li>
                    <label>
                        <input value="8" type="checkbox">
                        Motion Title
                    </label>
                </li>
                <li>
                    <label>
                        <input value="9" type="checkbox">
                        Drafted By
                    </label>
                </li>
                <li>
                    <label>
                        <input value="10" type="checkbox">
                        Judge
                    </label>
                </li>

                <li>
                    <label>
                        <input value="12" type="checkbox">
                        Court Notes
                    </label>
                </li>
                <li>
                    <label>
                        <input value="11" type="checkbox">
                        Delay Reason
                    </label>
                </li>
            </ul>
        </div>
    </div>
    <table id="tblHearing" class="table table-striped" width="100%">
        <thead>
            <tr>
                <th>&nbsp;</th>
                <th>Order Signed</th>
                <th>Hearing Date</th>
                <th>60<sup>th</sup> Day</th>
                <th>County</th>
                <th>Case Name</th>
                <th>Case #</th>
                <th>
                    <abbr title="Document Identification Number">DIN</abbr></th>
                <th>Motion Title</th>
                <th>Drafted By</th>
                <th>Judge</th>
                <th>
                    <abbr title="Court Notes" data-toggle="tooltip">CN</abbr></th>
                <th>
                    <abbr title="Delay Reason" data-toggle="tooltip">DR</abbr></th>
                <th>&nbsp;</th>
            </tr>
        </thead>
    </table>
</div>
<div class="modal fade" id="logModal" tabindex="-1" role="dialog" aria-labelledby="logModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="logModalLabel">Edit Hearing Log Item</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-auto">
                        <label for="txtOrderSigned">Order Signed</label>
                        <asp:TextBox ID="txtOrderSigned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                    </div>
                    <div class="col-auto">
                        <label for="txtHearingDate">Hearing Date</label>
                        <asp:TextBox ID="txtHearingDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger" ErrorMessage="Hearing Date Is Required" ControlToValidate="txtHearingDate" runat="server" />
                    </div>
                    <div class="col-auto">
                        <label for="drpCounty">County</label>
                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpCounty" CssClass="form-control">
                            <asp:ListItem Text="< Select County >" Value="" />
                            <asp:ListItem Text="DeSoto" />
                            <asp:ListItem Text="Manatee" />
                            <asp:ListItem Text="Sarasota" />
                            <asp:ListItem Text="Benchmark" />
                            <asp:ListItem Text="Clericus" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger" ErrorMessage="County Is Required" ControlToValidate="drpCounty" runat="server" />
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            <label for="txtCaseName">Case Name</label>
                            <asp:TextBox ID="txtCaseName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                        </div>
                        <div class="col-auto">
                            <label for="txtCaseNumber">Case Number</label>
                            <asp:TextBox ID="txtCaseNumber" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                        </div>
                        <div class="col-auto">
                            <label for="txtDIN">
                                <abbr title="Document Identification Number">DIN</abbr></label>
                            <asp:TextBox ID="txtDIN" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            <label for="txtMotionTitle">Motion Title</label>
                            <asp:TextBox ID="txtMotionTitle" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                        </div>
                        <div class="col-auto">
                            <label for="txtDraftedBy">Drafted By</label>
                            <asp:TextBox ID="txtDraftedBy" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label for="txtCourtNotes">Court Notes</label>
                            <asp:TextBox ID="txtCourtNotes" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label for="txtDelayReason">Delay Reason</label>
                            <asp:TextBox ID="txtDelayReason" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdLogId" runat="server" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="cmdSaveLogItem" class="btn btn-primary">Save</button>
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.jsdelivr.net/npm/bootstrap5-toggle@5.1.1/js/bootstrap5-toggle.jquery.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.jsdelivr.net/npm/bootstrap5-toggle@5.1.1/css/bootstrap5-toggle.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var jaRole = '<%=JaRole%>';
    var hasChiefJudgeRole = <%=HasChiefJudgeRole%>;
    var selectedJudge = 0;
    var showAllJudges = false;
    var statusValue = 0;
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
    var serviceExclude = {
        path: "ExcludeLog",
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
        serviceExclude.baseUrl = serviceExclude.framework.getServiceRoot(serviceExclude.path);
        var hearingAction = "GetLogItems";
        var excludeAction = "toggle-excluded";
        var restUrl = `${service.baseUrl}Hearing/${hearingAction}/${recordCount}`;
        //hearing table config
        hearingTable = $('#tblHearing').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: restUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.status = statusValue;
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
                    {
                        data: "logid", render: function (data, type, row, meta) {
                            return `<a title="View Record" data-id="${row.logid}" data-judgeid="${row.judgeid}" data-draftedby="${row.draftedby}" data-delayreason="${row.delayreason}" data-courtnotes="${row.courtnotes}"  data-motiontitle="${row.motiontitle}" data-din="${row.din}" data-casenumber="${row.casenumber}" data-casename="${row.casename}" data-county="${row.county}" data-hearingdate="${row.hearingdate}"  data-ordersigned="${row.ordersigned}" onclick="ViewRecord(event,this)" data-toggle="tooltip" class="search-link"><i class="fas fa-search" aria-hidden="true"></i></a>`;
                        }, className: "command-item", orderable: false
                    },
                    { data: "ordersigned" },
                    { data: "hearingdate" },
                    { data: "sixtiethdaydate", orderable: false },
                    { data: "county" },
                    { data: "casename" },
                    { data: "casenumber" },
                    { data: "din" },
                    { data: "motiontitle" },
                    { data: "draftedby" },
                    { data: "judgeid" },
                    {
                        data: "courtnotes", render: function (data, type, row, meta) {
                            return data == null ? '' : `<a data-bs-html="true" title="<strong>Court Notes:</strong><p>${data}</p>" data-toggle="tooltip"><i class="fas fa-comment-alt" aria-hidden="true"></i></a>`;

                        }, className: "command-item", orderable: false
                    },
                    {
                        data: "delayreason", render: function (data, type, row, meta) {
                            return data == null ? '' : `<a data-bs-html="true"  title="<strong>Delay Reason:</strong><p>${data}</p>" data-toggle="tooltip"><i class="fas fa-comment-alt" aria-hidden="true"></i></a>`;
                        }, className: "command-item", orderable: false
                    },
                    {
                        data: "logid", render: function (data, type, row, meta) {
                            return statusValue == 0 ? `<a class="exclude confirm" aria-role="button" title="Exclude Log Item" data-toggle="tooltip" data-logid="${data}"> <i class="text-danger fas fa-ban" aria-hidden="true"></i></a>` : statusValue == 2 ? `<a class="exclude confirm" aria-role="button" title="Include Log Item" data-toggle="tooltip" data-logid="${data}"><i class="text-success fa-solid fa-rotate-left" aria-hidden="true"></i></a>` : '';
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
        hearingTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".confirm").on("click", function (e) {
                e.preventDefault();
                var logid = $(this).data("logid");
                var prompt = 'Are you sure you wish to exclude this Log Item?'
                var title = 'Exclude Log Item';
                if (statusValue > 0) {
                    prompt = 'Are you sure you wish to Include this Log Item?';
                    title = 'Include Log Item'
                }
                $.dnnConfirm({
                    text: prompt,
                    yesText: 'Yes',
                    noText: 'No',
                    title: title,
                    callbackTrue: function () {
                        ExcludeHearing(e, logid);
                    }
                });
            });
        });
        $.fn.dataTable.ext.errMode = () => ShowAlert('Error Loading Data', 'Error while loading the table data. Please refresh');
        hearingTable.on('order.dt', function () {
            // This will show: "Ordering on column 1 (asc)", for example
            var order = hearingTable.order();
            localStorage.setItem('hearingLog.sortDirection', order[0][1]);
            localStorage.setItem('hearingLog.sortColumnIndex', order[0][0]);
        });
        hearingTable.on('page.dt', function () {
            var info = hearingTable.page.info();
            localStorage.setItem('hearingLog.currentPageIndex', info.page);
        });
        hearingTable.on('length.dt', function (e, settings, len) {
            currentPage = 0;
            localStorage.setItem('hearingLog.currentPageIndex', currentPage);
            localStorage.setItem('hearingLog.pageSize', len);
        });
        if (hiddenColumns != null && hiddenColumns != "") {
            var $checkboxMenu = $('.checkbox-menu input');
            hiddenColumns.split(',').forEach((checkboxValue) => {
                $checkboxMenu.filter('[value=' + checkboxValue + ']').prop('checked', true);
                hearingTable.column(parseInt(checkboxValue, 10)).visible(false);
            });
        }
        $("#txtStartDate").on("focusout", function (e) {
            CompareDates();
        });
        $("#txtEndDate").on("focusout", function (e) {
            CompareDates();
        });
        $('input[type=radio][name=statusOptions]').change(function () {
            statusValue = $('input[name = "statusOptions"]:checked').val();
            currentPage = 0;
            localStorage.setItem('hearingLog.status', statusValue);
            localStorage.setItem('hearingLog.currentPageIndex', currentPage);
            hearingTable.draw();
        });
        $('.checkbox-menu input').on("change", function (e) {
            e.preventDefault();
            var hiddenColumns = [];
            $('.checkbox-menu input').each(function (index) {
                if (this.checked)
                    hiddenColumns.push(this.value);
            });
            localStorage.setItem('hearingLog.hiddenColumns', hiddenColumns);

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
            selectedJudge = parseInt(this.value, 10);
            hearingTable.draw();
        });
        $('#txtSearch').on("input", function (e) {
            e.preventDefault();
            searchText = $(this).val();
            hearingTable.draw();
        });
        $('#btnAdd').on("click", function (e) {
            ClearEditLogForm();
            $('#drpCounty').val('');
            $('#drpCounty').prop("disabled", false);
        });
        $('#cmdSaveLogItem').on("click", function (e) {
            e.preventDefault();
            if (!Page_ClientValidate("new")) {
                return;
            }
            var logId = $("#hdLogId").val();
            if (logId != '') {
                UpdateHearingLog();
            } else {
                AddHearingLog();
            }
        });
        $('#btnImport').on("click", function (e) {
            e.preventDefault();
            ImportHearing();
        });
        $("#tblHearing_length").prepend($('#btnAdd'));
        $("#tblHearing_length").parent().siblings('div').first().prepend($('#statusOptionContainer'));
        $('input[data-bs-toggle="toggle"]').bootstrapToggle();
    }
    function ExcludeHearing(e, logid) {
        var excludeAction = "toggle-excluded";
        var excludeUrl = `${serviceExclude.baseUrl}Hearing/${excludeAction}/${logid}?jarole=${jaRole}`;
        e.preventDefault();
        $.ajax({
            url: excludeUrl,
            type: 'GET',
            success: function (result) {
                hearingTable.draw();
            },
            error: function (error) {
                ShowAlert('Error Excluding Log Item', error);
            }
        });
    }
    function ImportHearing() {
        var importAction = "import-hearings";
        var importUrl = `${service.baseUrl}Hearing/${importAction}?jarole=${jaRole}&startDate=${startDate}&endDate=${endDate}`;
        $.ajax({
            url: importUrl,
            type: 'GET',
            success: function (result) {
                hearingTable.draw();
            },
            error: function (error) {
                ShowAlert('Error Importing Hearings', error);
            }
        });
    }
    function UpdateHearingLog() {
        var updateAction = "update-hearing";
        var updateUrl = `${service.baseUrl}Hearing/${updateAction}?jarole=${jaRole}`;
        var logId = $("#hdLogId").val();
        var orderSigned = $("#txtOrderSigned").val();
        var hearingDate = $("#txtHearingDate").val();
        var caseName = $("#txtCaseName").val();
        var caseNumber = $("#txtCaseNumber").val();
        var din = $("#txtDIN").val();
        var motionTitle = $("#txtMotionTitle").val();
        var draftedBy = $("#txtDraftedBy").val();
        var delayReason = $("#txtDelayReason").val();
        var courtNotes = $("#txtCourtNotes").val();
        var hearing = {
            logid: logId, ordersigned: orderSigned, hearingdate: hearingDate,
            casename: caseName, casenumber: caseNumber, din: din,
            motiontitle: motionTitle, draftedby: draftedBy,
            delayreason: delayReason, courtnotes: courtNotes
        }
        try {
            $.ajax({
                type: "PUT",
                url: updateUrl,
                beforeSend: service.framework.setModuleHeaders,
                data: hearing,
                success: function (result) {
                    ClearEditLogForm();
                    var hearingUpdateModal = document.querySelector('#logModal');
                    var modal = bootstrap.Modal.getInstance(hearingUpdateModal);
                    if (!modal) {
                        modal = new bootstrap.Modal(document.getElementById('logModal'));
                    }
                    modal.hide();
                    hearingTable.ajax.reload();
                    hearingTable.draw();
                },
                error: function (xhr, status, error) {
                    ShowAlert("Error Updating Record", "Unable to update hearing.\n\nMake sure you are logged in and try again. \n\nError Details: " + error);
                }
            });
        } catch (err) {
            ShowAlert("Error Updating Record", "Unable to update Hearing.\n\nMake sure you are logged in and try again. \n\nError Details: " + err);
        }
        return false;
    }
    function AddHearingLog() {
        var addAction = "add-hearing";
        var addUrl = `${service.baseUrl}Hearing/${addAction}?jarole=${jaRole}`;
        var orderSigned = $("#txtOrderSigned").val();
        var hearingDate = $("#txtHearingDate").val();
        var caseName = $("#txtCaseName").val();
        var caseNumber = $("#txtCaseNumber").val();
        var county = $("#drpCounty").val();
        var din = $("#txtDIN").val();
        var motionTitle = $("#txtMotionTitle").val();
        var draftedBy = $("#txtDraftedBy").val();
        var delayReason = $("#txtDelayReason").val();
        var courtNotes = $("#txtCourtNotes").val();
        var hearing = {
            ordersigned: orderSigned, hearingdate: hearingDate, county: county,
            casename: caseName, casenumber: caseNumber, din: din,
            motiontitle: motionTitle, draftedby: draftedBy,
            delayreason: delayReason, courtnotes: courtNotes
        }
        try {
            $.ajax({
                type: "POST",
                url: addUrl,
                beforeSend: service.framework.setModuleHeaders,
                data: hearing,
                success: function (result) {
                    ClearEditLogForm();
                    var hearingUpdateModal = document.querySelector('#logModal');
                    var modal = bootstrap.Modal.getInstance(hearingUpdateModal);
                    if (!modal) {
                        modal = new bootstrap.Modal(document.getElementById('logModal'));
                    }
                    modal.hide();
                    hearingTable.ajax.reload();
                    hearingTable.draw();
                },
                error: function (xhr, status, error) {
                    ShowAlert("Error Adding Record", "Unable to add hearing.\n\nMake sure you are logged in and try again. \n\nError Details: " + error);
                }
            });
        } catch (err) {
            ShowAlert("Error Adding Record", "Unable to add Hearing.\n\nMake sure you are logged in and try again. \n\nError Details: " + err);
        }
        return false;
    }
    function fnJSOnFormSubmit(e) {
        var isGrpOneValid = Page_ClientValidate("CaseSearch");
        var isGrpTwoValid = Page_ClientValidate("CaseNew");

        var i;
        for (i = 0; i < Page_Validators.length; i++) {
            ValidatorValidate(Page_Validators[i]); //this forces validation in all groups
        }

        //display all summaries.
        for (i = 0; i < Page_ValidationSummaries.length; i++) {
            summary = Page_ValidationSummaries[i];
            //does this summary need to be displayed?
            if (fnJSDisplaySummary(summary.validationGroup)) {
                summary.style.display = ""; //"none"; "inline";
            }
        }

        if (isGrpOneValid && isGrpTwoValid)

            return true; //postback only when BOTH validations pass.
        else
            return false;
    }
    function fnJSDisplaySummary(valGrp) {
        var rtnVal = false;
        for (i = 0; i < Page_Validators.length; i++) {
            if (Page_Validators[i].validationGroup == valGrp) {
                if (!Page_Validators[i].isvalid) { //at least one is not valid.
                    rtnVal = true;
                    break; //exit for-loop, we are done.
                }
            }
        }
        return rtnVal;
    }
    function ViewRecord(e, item) {
        e.preventDefault();
        ClearEditLogForm();
        var logId = item.dataset.id;
        var orderSigned = item.dataset.ordersigned;
        var hearingDate = item.dataset.hearingdate;
        var caseName = item.dataset.casename;
        var caseNumber = item.dataset.casenumber;
        var din = item.dataset.din;
        var county = item.dataset.county;
        var motionTitle = item.dataset.motiontitle;
        var draftedBy = item.dataset.draftedby;
        var delayReason = item.dataset.delayreason;
        var courtNotes = item.dataset.courtnotes;
        if (logId && logId != "undefined")
            $("#hdLogId").val(logId);
        if (orderSigned && orderSigned != "undefined" && orderSigned != "null")
            $("#txtOrderSigned").val(orderSigned);
        if (hearingDate && hearingDate != "undefined" && hearingDate != "null")
            $("#txtHearingDate").val(hearingDate);
        if (caseName && caseName != "undefined" && caseName != "null")
            $("#txtCaseName").val(caseName);
        if (caseNumber && caseNumber != "undefined" && caseNumber != "null")
            $("#txtCaseNumber").val(caseNumber);
        if (county && county != "undefined" && county != "null")
            $("#drpCounty").val(county);
        if (din && din != "undefined" && din != "null")
            $("#txtDIN").val(din);
        if (motionTitle && motionTitle != "undefined" && motionTitle != "null")
            $("#txtMotionTitle").val(motionTitle);
        if (draftedBy && draftedBy != "undefined" && draftedBy != "null")
            $("#txtDraftedBy").val(draftedBy);
        if (delayReason && delayReason != "undefined" && delayReason != "null")
            $("#txtDelayReason").val(delayReason);
        if (courtNotes && courtNotes != "undefined" && courtNotes != "null")
            $("#txtCourtNotes").val(courtNotes);
        var hearingUpdateModal = document.querySelector('#logModal');
        var modal = bootstrap.Modal.getInstance(hearingUpdateModal);
        if (!modal) {
            modal = new bootstrap.Modal(document.getElementById('logModal'));
        }
        modal.show();
    }
    function ClearEditLogForm() {
        $("#hdLogId").val('');
        $("#txtOrderSigned").val('');
        $("#txtHearingDate").val('');
        $("#txtCaseName").val('');
        $("#txtCaseNumber").val('');
        $("#txtDIN").val('');
        $("#txtMotionTitle").val('');
        $("#txtDraftedBy").val('');
        $("#txtDelayReason").val('');
        $("#txtCourtNotes").val('');
        $('#drpCounty').prop("disabled", true);
    }
    function GetLocalStorage() {
        var storageStatusId = localStorage.getItem('hearingLog.status');
        var storageStartDate = localStorage.getItem('hearingLog.startDate');
        var storageEndDate = localStorage.getItem('hearingLog.endDate');
        var storageCurrentPage = localStorage.getItem('hearingLog.currentPageIndex');
        var storagePageSize = localStorage.getItem('hearingLog.pageSize');
        var storageSortDirection = localStorage.getItem('hearingLog.sortDirection');
        var storageSortColumnIndex = localStorage.getItem('hearingLog.sortColumnIndex');
        var storageHiddenColumns = localStorage.getItem('hearingLog.hiddenColumns');
        if (storageHiddenColumns != null && storageHiddenColumns != undefined) {
            hiddenColumns = storageHiddenColumns;
        }
        if (storageStatusId != null && storageStatusId != undefined) {
            statusValue = storageStatusId;
            var $radios = $('input:radio[name=statusOptions]');
            $radios.filter('[value=' + statusValue + ']').prop('checked', true);
        }
        if (storageStartDate != null && storageStartDate != undefined)
            $("#txtStartDate").val(storageStartDate);
        if (storageEndDate != null && storageEndDate != undefined)
            $("#txtEndDate").val(storageEndDate);
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
        if ((Date.parse(tempEndDate) < Date.parse(tempStartDate))) {
            $("#txtEndDate").val(endDate);
            $("#txtStartDate").val(startDate);
            ShowAlert("Invalid Date", "End Date MUST be greater than Start Date");
        } else {
            startDate = tempStartDate;
            endDate = tempEndDate;
            currentPage = 0;
            localStorage.setItem('hearingLog.currentPageIndex', currentPage);
            localStorage.setItem('hearingLog.startDate', startDate);
            localStorage.setItem('hearingLog.endDate', endDate);
            hearingTable.draw();
        }
    }
</script>
