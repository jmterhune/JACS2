<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.HearingLog.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="ms-2 me-2">
    <asp:HyperLink ID="lnkAdmin" runat="server" Visible="false" CssClass="btn btn-primary"><i class="fas fa-cog"></i> Admin</asp:HyperLink>
    <div class="p-2 bg-light mb-3 ms-3 border rounded border-secondary d-inline-block">
        <div class="row g-3 align-items-center">
            <label class="col-auto col-form-label" for="txtCutoffDate">Cutoff Date</label>
            <div class="col-auto">
                <asp:TextBox ID="txtCutoffDate" ClientIDMode="Static" runat="server" Width="150" CssClass="form-control date-picker" MaxLength="15" placeholder="Cutoff Date" aria-label="Cutoff Date"></asp:TextBox>
            </div>
            <div class="col-auto">
                <input type="radio" class="btn-check" name="statusOptions" value="0" id="newOption" autocomplete="off" checked>
                <label class="btn btn-secondary" for="newOption">New</label>

                <input type="radio" class="btn-check" name="statusOptions" value="1" id="archiveOption" autocomplete="off">
                <label class="btn btn-secondary" for="archiveOption">Archived</label>

                <input type="radio" class="btn-check" name="statusOptions" value="2" id="excludedOption" autocomplete="off">
                <label class="btn btn-secondary" for="excludedOption">Excluded</label>
            </div>
        </div>
    </div>
    <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
    <table id="tblHearing" class="table table-striped">
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
                <th>Created By</th>
                <th title="Court Notes" data-toggle="tooltip"><span class="sr-only">Court Notes</span></th>
                <th title="Delay Reason" data-toggle="tooltip"><span class="sr-only">Delay Reason</span></th>

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
                    </div>
                    <div class="col-auto">
                        <label for="drpCounty">County</label>
                        <asp:DropDownList ID="drpCounty" Enabled="false" runat="server" ClientIDMode="Static" CssClass="form-control">
                            <asp:ListItem Text="DeSoto" />
                            <asp:ListItem Text="Manatee" />
                            <asp:ListItem Text="Sarasota" />
                        </asp:DropDownList>
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
                            <label for="txtDelayReason">Delay Reason</label>
                            <asp:TextBox ID="txtDelayReason" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label for="txtCourtNotes">Court Notes</label>
                            <asp:TextBox ID="txtCourtNotes" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdLogId" runat="server" ClientIDMode="Static" />

                </div>
            </div>
            <div class="modal-footer">
                <asp:Button OnClientClick="UpdateHearingLog(event)" CssClass="btn btn-primary" ID="cmdSaveLogItem" runat="server" Text="Save" />
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var jaRole ='<%=JaRole%>';
    var statusValue = 0;
    var cutoffDateValue = "";
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "asc";
    var sortColumnIndex = 2;
    var currentPage = 0;
    var hearingTable = null;
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
        cutoffDateValue = $("#txtCutoffDate").val();
        $(".date-picker").datepicker();
        service.baseUrl = service.framework.getServiceRoot(service.path);
        serviceExclude.baseUrl = serviceExclude.framework.getServiceRoot(serviceExclude.path);
        var hearingAction = "GetLogItems";
        var saveAction = "update-hearing";
        var excludeAction = "toggle-excluded";
        var restUrl = `${service.baseUrl}Hearing/${hearingAction}/${recordCount}`;
        var saveUrl = `${service.baseUrl}Hearing/${saveAction}/`;

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
                    data.cutoffDate = cutoffDateValue;
                    data.jaRole = jaRole;
                    delete data.columns;
                },
            },
            columns: [{
                data: "logid", render: function (data, type, row, meta) {
                    return `<a title="View Record" data-id="${row.logid}" data-judgeid="${row.judgeid}" data-draftedby="${row.draftedby}" data-delayreason="${row.delayreason}" data-courtnotes="${row.courtnotes}"  data-motiontitle="${row.motiontitle}" data-din="${row.din}" data-casenumber="${row.casenumber}" data-casename="${row.casename}" data-county="${row.county}" data-hearingdate="${row.hearingdate}"  data-ordersigned="${row.ordersigned}" onclick="ViewRecord(event,this)" data-toggle="tooltip" class="search-link"><i class="fas fa-search"></i></a>`;
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
                    return data == null ? '' : '<i class="fas fa-comment-alt" data-html="true" title="Court Notes: ' + data + '" data-toggle="tooltip" ></i></a>';
                }, className: "command-item", orderable: false
            },
            {
                data: "delayreason", render: function (data, type, row, meta) {
                    return data == null ? '' : '<i class="fas fa-comment-alt" data-html="true" title="Delay Reason: ' + data + '" data-toggle="tooltip" ></i></a>';
                }, className: "command-item", orderable: false
            },
            {
                data: "logid", render: function (data, type, row, meta) {
                    return '<a class="delete confirm" aria-role="button" title="Exclude Log Item" data-toggle="tooltip" data-logid="' + data + '" href="#""><i class="text-danger fas fa-ban"></i></a>';
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
                $.dnnConfirm({
                    text: 'Are you sure you wish to exclude this Log Item?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Exclude Log Item?',
                    callbackTrue: function () {
                        ExcludeHearing(e,logid);
                    }
                });
            });
        });
        $.fn.dataTable.ext.errMode = () => alert('Error while loading the table data. Please refresh');
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
            localStorage.setItem('hearingLog.pageSize', len);
        });
        $("#txtCutoffDate").on("focusout", function (e) {
            cutoffDateValue = $("#txtCutoffDate").val();
            hearingTable.draw();
        });
        $('input[type=radio][name=statusOptions]').change(function () {
            statusValue = $('input[name = "statusOptions"]:checked').val();
            hearingTable.draw();
        });
    }
    function ExcludeHearing(e, logid) {
        var excludeAction = "toggle-excluded";
        var excludeUrl = `${serviceExclude.baseUrl}Hearing/${excludeAction}/`;
        e.preventDefault();
        $.ajax({
            url: excludeUrl + logid,
            type: 'GET',
            success: function (result) {
                hearingTable.draw();
            },
            error: function (error) {
                alert(error);
            }
        });
    }
    function UpdateHearingLog(e) {
        e.preventDefault();
        var updateAction = "update-hearing";
        var updateUrl = `${service.baseUrl}Hearing/${updateAction}/`;
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
                    alert("Unable to update hearing.\n\nMake sure you are logged in and try again.");
                }
            });
        } catch (e) {
            alert("Unable to update Hearing.\n\nMake sure you are logged in and try again.");
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

    }
    function GetLocalStorage() {
        storageStatusId = localStorage.getItem('hearingLog.status');
        storageCutoffDate = localStorage.getItem('hearingLog.cutoffDate');
        storageCurrentPage = localStorage.getItem('hearingLog.currentPageIndex');
        storagePageSize = localStorage.getItem('hearingLog.pageSize');
        storagePageSize = localStorage.getItem('hearingLog.pageSize');
        storageSortDirection = localStorage.getItem('hearingLog.sortDirection');
        storageSortColumnIndex = localStorage.getItem('hearingLog.sortColumnIndex');
        if (storageStatusId != null && storageStatusId != undefined) {
            statusValue = storageStatusId;
            var $radios = $('input:radio[name=statusOptions]');
            $radios.filter('[value=' + statusValue +']').prop('checked', true);
        }
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
