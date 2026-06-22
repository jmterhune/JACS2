<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DesignationList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.DesignationList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs ms-2 me-2">
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
        </li>        <li class="nav-item">
    <a class="nav-link" href="https://jud12fl.sharepoint.com/sites/CourtReporting">Team Site</a>
</li>
    </ul>
    <div class="tab-content pb-0">
        <div id="designation" class="tab-pane active">
            <div class="bg-dark text-white border-dark rounded p-2 mb-2">
                <div class="row">
                    <div class="col-auto me-2 pt-2"><strong>Filter By:</strong></div>
                    <div class="col">
                        <input type="text" id="txtLastNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="Last Name" />
                    </div>
                    <div class="col">
                        <input type="text" id="txtFirstNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="First Name" />
                    </div>
                    <div class="col">
                        <input type="text" id="txtCaseNumberSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="Case Number" />
                    </div>
                    <div class="col-auto">
                        <select id="drpCountySearch" class="form-control" tabindex="0">
                            <option value="">< Filter By County ></option>
                            <option>DeSoto</option>
                            <option>Manatee</option>
                            <option>Sarasota</option>
                        </select>
                    </div>
                    <div class="col-auto pt-2">
                        <input type="checkbox" tabindex="0" id="chkArchive" name="chkArchive" class="form-check-input"><label class="form-check-label ms-2" for="chkArchive">Show Archived</label>
                    </div>
                    <div class="col-auto">
                        <button type="button" tabindex="-1" class="btn btn-primary" id="cmdSearch">Filter</button>
                    </div>
                </div>
            </div>
            <button id="btnAdd" class="btn btn-primary me-3" tabindex="-1" data-toggle="modal" data-target="#designationModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Designation</button>
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
<div class="modal fade" id="designationModal" tabindex="-1" role="dialog" aria-labelledby="designationModalLabel">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="designationModalLabel">Add Designation</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-md-3">
                        <label for="txtLastName">Last Name</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtLastName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="Last Name Is Required" ControlToValidate="txtLastName" EnableClientScript="true" runat="server" />
                    </div>
                    <div class="col-md-3">
                        <label for="txtFirstName">First Name</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtFirstName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="First Name Is Required" ControlToValidate="txtFirstName" EnableClientScript="true" runat="server" />
                    </div>
                    <div class="col-md-3">
                        <label for="txtMiddleName">Middle Name</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtMiddleName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label for="drpCounty">County</label>
                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpCounty" CssClass="form-control">
                            <asp:ListItem Text="< Select County >" Value="" />
                            <asp:ListItem Text="DeSoto" />
                            <asp:ListItem Text="Manatee" />
                            <asp:ListItem Text="Sarasota" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" EnableClientScript="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="County Is Required" ControlToValidate="drpCounty" runat="server" />
                    </div>
                </div>
                <fieldset class="outline-fieldset mb-2">
                    <h5 class="mb-0">Add one or more attorneys to designation</h5>
                    <div class="form-text ms-2">Select the attorney from the drop down then click the "Add Selected Attorney". If the attorney does not exist in the drop down, click Add New Attorney to add them.</div>
                    <div class="row">
                        <div class="col-auto">
                            <div class="attydropdown">
                                <input type="text" id="attorneySearch" class="form-control" placeholder="Type to search...">
                                <input type="hidden" id="selectedAttorneyId">
                                <div id="attyDropDown" class="list-group position-absolute w-100" style="display: none;"></div>
                            </div>
                        </div>
                        <div class="col">
                            <button type="button" id="cmdAddAttorney" class="btn btn-primary">Add Selected Attorney</button>
                        </div>
                        <input type="hidden" id="attorneyCount">
                    </div>
                    <table id="tblAttorneys" class="table table-striped w-100">
                        <thead>
                            <tr>
                                <th>Attorney Name</th>
                                <th>Office</th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                    </table>
                </fieldset>
                <div class="row form-group">
                    <div class="col-md-3">
                        <label for="txtTribunalCaseNumber">Tribunal Case Number</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtTribunalCaseNumber" ClientIDMode="Static" TextMode="MultiLine" Rows="2" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="Tribunal Case Number Is Required" ControlToValidate="txtTribunalCaseNumber" EnableClientScript="true" runat="server" />
                    </div>
                    <div class="col-md-3">
                        <label for="txtAppellateCaseNumber">Appellate Case Number</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtAppellateCaseNumber" ClientIDMode="Static" TextMode="MultiLine" Rows="2" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label for="txtServiceDate">Service Date</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtServiceDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="Service Date Is Required" ControlToValidate="txtServiceDate" EnableClientScript="true" runat="server" />

                    </div>
                    <div class="col-md-3">
                        <label for="txtReceiptDate">Receipt Date</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtReceiptDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                            ErrorMessage="Receipt Date Is Required" ControlToValidate="txtReceiptDate" EnableClientScript="true" runat="server" />

                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-3">
                        <label for="txtHearingDate">Hearing Date</label>
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtHearingDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="hearingDate" CssClass="label label-danger"
                            ErrorMessage="Hearing Date Is Required" ControlToValidate="txtHearingDate" EnableClientScript="true" runat="server" />

                    </div>
                    <div class="col">
                        <label for="drpPresidingJudge">Presiding Judge</label>
                        <div class="judgedropdown">
                            <input type="text" id="judgeSearch" class="form-control" placeholder="Type to search...">
                            <input type="hidden" id="selectedjudgeId">
                            <div id="judgeDropDown" class="list-group position-absolute w-100" style="display: none;"></div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <label for="drpHearingType">Hearing Type</label>
                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpHearingType" CssClass="form-control">
                            <asp:ListItem Text="< Select Hearing Type >" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-auto">
                        <label>&nbsp;</label>
                        <button type="button" id="cmdAddEvent" class="btn btn-primary d-block">Add Event</button>
                    </div>
                </div>
                <table id="tblEvents" class="table table-striped w-100">
                    <thead>
                        <tr>
                            <th>Hearing Date</th>
                            <th>Presiding Judge</th>
                            <th>Hearing Type</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                </table>
                <asp:HiddenField ID="hdDesignationId" runat="server" ClientIDMode="Static" />
                <input type="hidden" id="hearingCount">
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" id="cmdSaveDesignationItem" class="btn btn-primary"><i class="fas fa-floppy-disk"></i>&nbsp;Submit Designation</button>
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="EditAttorneyModal" tabindex="-1" role="dialog" aria-labelledby="EditAttorneyModalLabel">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="EditAttorneyModalLabel">Add Attorney</h4>
                <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div class="row form-group">
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtFirstNameAtty" Text="First Name" />
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstNameAtty" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtMiddleNameAtty" Text="Middle Name" />
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtMiddleNameAtty" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtLastNameAtty" Text="Last Name" />
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastNameAtty" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-12">
                        <asp:Label runat="server" AssociatedControlID="drpOffice" Text="Office Location" />
                        <asp:DropDownList runat="server" ID="drpOffice" CssClass="form-control" AppendDataBoundItems="true" ClientIDMode="Static">
                            <asp:ListItem Value="0" Text="< Select Office Location >" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row form-group">

                    <div class="col-12">
                        <label for="txtAddress" class="form-label">Address</label>
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress" placeholder="1234 Main St" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-12">
                        <label for="txtAddress2" class="form-label">Address 2</label>
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress2" placeholder="Apartment, studio, or floor" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-5">
                        <label for="txtCity" class="form-label">City</label>
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCity" />
                    </div>
                    <div class="col-md-4">
                        <label for="drpState" class="form-label">State</label>
                        <asp:DropDownList runat="server" ID="drpState" CssClass="form-control" ClientIDMode="Static" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Text="< Select State >" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label for="txtZip" class="form-label">Zip</label>
                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="10" ID="txtZip" />
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" id="cmdSaveAttorney" class="btn btn-primary">Save Attorney</button>
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="NameSearchModal" tabindex="-1" role="dialog" aria-labelledby="NameSearchModalLabel">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="NameSearchModalLabel">Requested Name with Matching Records</h4>
                <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <table id="tblMatchingNames" class="table table-striped w-100">
                    <thead>
                        <tr>
                            <th>Last Name</th>
                            <th>First Name</th>
                            <th>Hearing Date</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
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
    var userId = <%=UserId%>;
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
    var adminRole = "<%=AdminRole%>";
    var currentPage = 0;
    var attorneyId = null;
    var designationId = null;
    var eventId = null;
    var attorneyTable = null;
    var matchingNameTable = null;
    var designationTable = null;
    var eventTable = null;
    var eventArray = [];
    var attorneyArray = [];
    var restUrl = null;
    var matchingNameUrl = null;
    var attorneyDropDownUrl = null;
    var employeeDropDownUrl = null;
    var deleteUrl = null;
    var archiveUrl = null;
    var acknowledgeUrl = null;
    var designationAddUrl = null;
    var attorneyAddUrl = null;
    var eventAddUrl = null;
    var eventMessage = null;
    var attyOptions = [];
    var judgeOptions = [];
    var matchingNames = [];
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
    var serviceAttorney = {
        path: "TranscriptAttorney",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceEvent = {
        path: "TranscriptEvent",
        framework: $.ServicesFramework(moduleId)
    };
    var matchingModal = null;
    (function ($, Sys) {
        $(document).ready(function () {
            $(".date-picker").on("blur", function (e) {
                var date = $(this).val();
                $(this).val(date.replace(/\.|-/g, "/"));
            });
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
        serviceToggle.baseUrl = serviceToggle.framework.getServiceRoot(serviceToggle.path);
        serviceAttorney.baseUrl = serviceAttorney.framework.getServiceRoot(serviceAttorney.path);
        serviceEvent.baseUrl = serviceEvent.framework.getServiceRoot(serviceEvent.path);
        restUrl = `${service.baseUrl}DesignationListItem/GetDesignationListItems/${recordCount}`;
        matchingNameUrl = `${service.baseUrl}DesignationListItem/GetMatchingNames/`;
        deleteUrl = `${serviceDelete.baseUrl}DesignationListItem/Delete/`;
        archiveUrl = `${serviceToggle.baseUrl}DesignationListItem/Archive/`;
        acknowledgeUrl = `${serviceToggle.baseUrl}DesignationListItem/Acknowledge/`;
        designationAddUrl = `${service.baseUrl}DesignationListItem/CreateDesignation/`;
        attorneyAddUrl = `${serviceAttorney.baseUrl}Attorney/CreateAttorney/`;
        eventAddUrl = `${serviceEvent.baseUrl}Event/CreateEvent/`;
        attorneyDropDownUrl = `${service.baseUrl}Attorney/GetAttorneyDropDown/`;
        employeeDropDownUrl = `${service.baseUrl}Attorney/GetJudgeDropDown/`;
        attyOptions = fetchAttorneyOptions();
        judgeOptions = fetchEmployeeOptions();
        designationTable = $('#tblDesignations').DataTable({
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
                { data: "casenumber" },
                { data: "county" },
                { data: "servicedate" },
                {
                    data: "acknowledgmentfiled", render: function (data, type, row, meta) {
                        return data == true ? `<a class="acknowledge" href="" title="Set Acknowledgment to Unfiled" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="acknowledge" href="#" title="Set Acknowledgment to Filed" data-id="${row.designationid}"><i class="fas fa-square"></i></a>`;
                    }
                },
                { data: "duedate" },
                { data: "transcriptfiled" },
                { data: "createdbyname" },
                {
                    data: "archived", render: function (data, type, row, meta) {
                        return data == true ? `<a class="archive" href="#" title="Set Status to Unarchived" data-id="${row.designationid}"><i class="fas fa-check-square"></i></a>` : `<a class="archive" href="#" title="Set Status to Archived" data-id="${row.designationid}" ><i class="fas fa-square"></i></a>`;
                    }, orderable: false
                },
                {
                    data: "designationId", render: function (data, type, row, meta) {
                        if (isAdmin == "True")
                            return `<a class="delete text-danger" aria-role="button" title="Delete Record" data-id="${row.designationid}" href="#"><i class="fas fa-trash"></i></a>`;
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
        designationTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".delete").on("click", function (e) {
                e.preventDefault();
                designationId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Designation?', text: 'Are you sure you wish to delete this Designation?', icon: 'warning',
                    showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
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
                designationId = $(this).data("id");
                Swal.fire({
                    title: 'Change Archive Status?', text: 'Are you sure you wish to change the Archive status?', icon: 'warning',
                    showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                    confirmButtonColor: '#d33'
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
                designationId = $(this).data("id");
                Swal.fire({
                    title: 'Change Acknowledgement Status?', text: 'Are you sure you wish to change the Acknowledgement status?', icon: 'warning',
                    showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                    confirmButtonColor: '#d33'
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
        eventTable = $('#tblEvents').DataTable({
            searching: false,
            autoWidth: true,
            columns: [
                { data: "date" },
                { data: "judgename" },
                { data: "type" },
                {
                    data: "eventid", render: function (data, type, row, meta) {
                        return `<a title="Delete Event" data-id="${row.eventid}" class="delete-event text-danger" href="#"><i class="fas fa-trash"></i></a>`;
                    }, className: "command-item", orderable: false
                },
            ],
            data: eventArray,
            info: false,
            ordering: false,
            paging: false,
            language: {
                emptyTable: "No Events Added.",
                zeroRecords: "No Events Added."
            },
        });
        attorneyTable = $('#tblAttorneys').DataTable({
            searching: false,
            autoWidth: true,
            layout: {
                topEnd: function () {
                    var btn = document.createElement('button');
                    btn.type = 'button';
                    btn.id = 'cmdAttorney';
                    btn.className = 'btn btn-dark';
                    btn.setAttribute('data-bs-toggle', 'modal');
                    btn.setAttribute('data-bs-target', '#EditAttorneyModal');
                    btn.setAttribute('data-toggle', 'modal');
                    btn.setAttribute('data-target', '#EditAttorneyModal');
                    btn.textContent = 'Add New Attorney';
                    return btn;
                }
            },
            columns: [
                { data: "name" },
                { data: "office" },
                {
                    data: "id", render: function (data, type, row, meta) {
                        return `<a title="Remove Attorney" data-id="${row.id}" class="remove-attorney text-danger"  href="#"><i class="fas fa-trash"></i></a>`;
                    }, className: "command-item"
                },
            ],
            data: attorneyArray,
            info: false,
            ordering: false,
            paging: false,
            language: {
                emptyTable: "No Attorneys Added.",
                zeroRecords: "No Attorneys Added."
            },
        });
        matchingNameTable = $('#tblMatchingNames').DataTable({
            searching: false,
            autoWidth: true,
            columns: [
                { data: "lastname" },
                { data: "firstname" },
                { data: "hearingdate" },
            ],
            data: matchingNames,
            info: true,
            ordering: true,
            paging: true,
            pageLength: 10,
            language: {
                emptyTable: "No Matching Names.",
                zeroRecords: "No Matching Names."
            },
        });
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List", "Error while loading the table data. Please refresh");
        $(document).on('click', '.delete-event', function (e) {
            e.preventDefault();
            var eventId = $(this).data("id");
            Swal.fire({
                title: 'Delete Event?', text: 'Are you sure you wish to delete this Event?', icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                confirmButtonColor: '#d33'
            }).then(function (r) { if (r.isConfirmed) DeleteEvent(eventId); });
        });
        matchingModal = new bootstrap.Modal(document.getElementById('NameSearchModal'), {
            keyboard: false
        });
        $(document).on('click', '.remove-attorney', function (e) {
            e.preventDefault();
            var attorneyId = $(this).data("id");
            Swal.fire({
                title: 'Remove Attorney?', text: 'Are you sure you wish to remove this Attorney from the list?', icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                confirmButtonColor: '#d33'
            }).then(function (r) { if (r.isConfirmed) RemoveAttorney(attorneyId); });
        });
        $(document).on('show.bs.modal', '.modal', function (event) {
            var zIndex = 50 + (10 * $('.modal:visible').length);
            $(this).css('z-index', zIndex);
            setTimeout(function () {
                $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
            }, 0);
        });
        $(document).on('click', function (event) {
            if (!$(event.target).closest('.attydropdown').length) {
                $('#attyDropDown').hide();
            }
            if (!$(event.target).closest('.judgedropdown').length) {
                $('#judgeDropDown').hide();
            }
        });
        $("#drpCountySearch,#chkArchive").on("change", function (e) {
            $("#cmdSearch").trigger("click");
        });
        $(".search").on("blur", function (e) {
            $("#cmdSearch").trigger("click");
        });
        $("#txtLastName").on("blur", function (e) {
            var last = $(this).val();
            if (last)
                fetchMatchingNames(last);
        });
        $('#txtTribunalCaseNumber').on('blur', function () {
            this.value = this.value.toUpperCase();
        });
        $('#txtAppellateCaseNumber').on('blur', function () {
            this.value = this.value.toUpperCase();
        });
        $('#attorneySearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = attyOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#attyDropDown').empty();
            if (filteredOptions.length > 0) {
                $('#attyDropDown').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                        .attr('data-office', option.office);
                    div.on('click', function () {
                        $('#attorneySearch').val(option.name);
                        $('#selectedAttorneyId').val(option.id);
                        $('#attyDropDown .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#attyDropDown').hide();
                    });
                    $('#attyDropDown').append(div);
                });
            } else {
                $('#attyDropDown').hide();
            }
        });
        $('#judgeSearch').on('keyup', function () {
            let input = $(this).val().toLowerCase();
            let filteredOptions = judgeOptions.filter(option => option.name.toLowerCase().includes(input));
            $('#judgeDropDown').empty();
            if (filteredOptions.length > 0) {
                $('#judgeDropDown').show();
                filteredOptions.forEach(option => {
                    let div = $('<div></div>').text(option.name)
                        .addClass('list-group-item list-group-item-action')
                        .attr('data-id', option.id)
                    div.on('click', function () {
                        $('#judgeSearch').val(option.name);
                        $('#selectedjudgeId').val(option.id);
                        $('#judgeDropDown .list-group-item').removeClass('active');
                        $(this).addClass('active');
                        $('#judgeDropDown').hide();
                    });
                    $('#judgeDropDown').append(div);
                });
            } else {
                $('#judgeDropDown').hide();
            }
        });
        $('#btnAdd').on("click", function (e) {
            e.preventDefault();
            ClearDesignation();
        });
        $("#cmdSaveDesignationItem").on("click", function (e) {
            $(this).prop("disabled", true);
            e.preventDefault();
            var events = $("#hearingCount").val();
            var attys = $("#attorneyCount").val();
            var hasEvents = (!isNaN(events) && events.trim() !== "");
            var hasAttys = (!isNaN(attys) && attys.trim() !== "");
            if (validateAddDesignation()) {
                if (hasEvents && hasAttys) {
                    $("#cmdSaveDesignationItem i").removeClass("floppy-disk");
                    $("#cmdSaveDesignationItem i").addClass("fa-arrows-rotate fa-spin");
                    AddDesignation();
                } else {
                    if (!hasEvents & !hasAttys) {
                        ShowAlert("No Hearing or Attorney Added", "Please Add at least one hearing and at least one attorney");
                    } else if (!hasEvents) {
                        ShowAlert("No Hearing Added", "Please Add at least one hearing");
                    } else {
                        ShowAlert("No Attorney Selected", "Please Select at least one attorney");
                    }
                    $(this).prop("disabled", false);
                }
            } else {
                $(this).prop("disabled", false);
            }
        });
        $('#cmdAddEvent').on("click", function (e) {
            if (validateHearingDate())
                InsertEvent();
        });
        $("#cmdAddAttorney").on("click", function (e) {
            InsertAttorney();
        });
        $("#cmdSaveAttorney").on("click", function (e) {
            AddAttorney(e);
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            caseNumber = $("#txtCaseNumberSearch").val();
            lastName = $("#txtLastNameSearch").val();
            firstName = $("#txtFirstNameSearch").val();
            county = $("#drpCountySearch").val();
            archived = $("#chkArchive").is(':checked')
            designationTable.draw();
        });
        $(".dt-length").prepend($('#btnAdd'));
    }
    function validateAddDesignation() {
        var isValid = Page_ClientValidate('new');
        if (isValid) {
            return true;
        } else {
            return false;
        }
    }
    function validateHearingDate() {
        var isValid = Page_ClientValidate('hearingDate');
        if (isValid) {
            return true;
        } else {
            return false;
        }
    }
    function fetchMatchingNames(name) {
        $.ajax({
            url: matchingNameUrl,
            method: 'GET',
            dataType: 'json',
            data: {
                lastName: name,
            },
            success: function (response) {
                matchingNames = response.data;
                if (matchingNames.length > 0) {
                    matchingNameTable.clear().rows.add(matchingNames).draw();
                    matchingModal.show();
                }
            },
            error: function () {
                console.error('Failed to fetch options');
            }
        });
    }
    function fetchAttorneyOptions() {
        $.ajax({
            url: attorneyDropDownUrl,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                attyOptions = response.data;
            },
            error: function () {
                console.error('Failed to fetch options');
            }
        });
    }
    function fetchEmployeeOptions() {
        $.ajax({
            url: employeeDropDownUrl,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                judgeOptions = response.data;
            },
            error: function () {
                console.error('Failed to fetch options');
            }
        });
    }
    function DeleteEvent(eventId) {
        var eventRow = eventArray.find(row => row.eventid === eventId);
        if (eventRow.eventid > -1) {
            eventArray.splice(eventRow, 1);
            eventTable.clear().rows.add(eventArray).draw();
        }
        $("#hearingCount").val(eventArray.length);
    }
    function RemoveAttorney(attorneyId) {
        var attyRow = attorneyArray.find(row => row.id === attorneyId);
        if (attyRow.id > -1) {
            attorneyArray.splice(attyRow, 1);
            attorneyTable.clear().rows.add(attorneyArray).draw();
        }
        $('#attorneyCount').val(attorneyArray.length);
    }
    function InsertAttorney() {
        const attorney = GetSelectedAttoney();
        if (attorney.id) {
            attorneyArray.push(attorney);
            attorneyTable.clear().rows.add(attorneyArray).draw();
            $('#attorneyCount').val(attorneyArray.length);
        }
        $('#selectedAttorneyId').val("");
        $('#selectedAttorneyId').removeAttr("data-office");
        $('#attorneySearch').val("");
    }
    function GetSelectedAttoney() {
        let selectedElement = $('#attyDropDown .list-group-item.active');
        return {
            id: Number($('#selectedAttorneyId').val()),
            office: selectedElement.length ? selectedElement.attr('data-office') : "",
            name: $('#attorneySearch').val()
        };
    }
    function GetSelectedJudge() {
        let selectedElement = $('#judgeDropDown .list-group-item.active');
        return {
            id: Number($('#selectedjudgeId').val()),
            name: $('#judgeSearch').val()
        };
    }
    function ClearDesignation() {
        $('#selectedAttorneyId').val("");
        $('#selectedHearingId').val("");
        $("#txtLastName").val("");
        $("#txtFirstName").val("");
        $("#txtMiddleName").val("");
        $("#drpCounty").val("");
        $("#txtTribunalCaseNumber").val("");
        $("#txtAppellateCaseNumber").val("");
        $("#txtServiceDate").val("");
        $("#txtReceiptDate").val("");
        $("#hdDesignationId").val("");
        $("#hearingCount").val("");
        $("#attorneyCount").val("");

        eventArray = [];
        eventTable.clear().rows.add(eventArray).draw();
        attorneyArray = [];
        attorneyTable.clear().rows.add(attorneyArray).draw();
    }
    function AddAttorney() {
        var action = "CreateAttorney";
        var firstName = $("#txtFirstNameAtty").val();
        var lastName = $("#txtLastNameAtty").val();
        var middleName = $("#txtMiddleNameAtty").val();
        var office = $("#drpOffice").val();
        var address1 = $("#txtAddress").val();
        var address2 = $("#txtAddress2").val();
        var city = $("#txtCity").val();
        var state = $("#drpState").val();
        var zip = $("#txtZip").val();
        var attorney = { firstname: firstName, lastname: lastName, middlename: middleName, officeid: office, address1: address1, address2: address2, city: city, state: state, zip: zip, createdbyuserid: userId };
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: attorneyAddUrl,
                beforeSend: serviceAttorney.framework.setModuleHeaders,
                data: (attorney),
                success: function (result) {
                    ClearAttorney();
                    var attyAddModal = document.querySelector('#EditAttorneyModal');
                    var modal = bootstrap.Modal.getInstance(attyAddModal);
                    if (!modal) {
                        modal = new bootstrap.Modal(document.getElementById('EditAttorneyModal'));
                    }
                    modal.hide();
                    const attorneyAdd = { id: result.data.attorneyid, name: result.data.listname, office: result.data.officename };
                    attyOptions.push(attorneyAdd);
                    attorneyArray.push(attorneyAdd);
                    attorneyTable.clear().rows.add(attorneyArray).draw();
                },
                error: function (xhr, status, error) {
                    ShowAlert("Error Attempting to Add Attorney", "Unable to add attorney.\n\nMake sure you are logged in and try again. \n\nError:" + error);
                }
            });
        } catch (error) {
            ShowAlert("Error Attempting to Add Attorney", "Unable to add attorney.\n\nMake sure you are logged in and try again.\n\n" + error);
        }
        return false;
    }
    function ClearAttorney() {
        $("#txtFirstNameAtty").val("");
        $("#txtLastNameAtty").val("");
        $("#txtMiddleNameAtty").val("");
        $("#drpOffice").val("0");
        $("#txtAddress").val("");
        $("#txtAddress2").val("");
        $("#txtCity").val("");
        $("#drpState").val("");
        $("#txtZip").val("");
    }
    function InsertEvent() {
        var hearingDate = $("#txtHearingDate").val();
        var judgeId = $('#selectedjudgeId').val();
        var judgeName = $('#judgeSearch').val();
        var type = $("#drpHearingType").val();
        if (eventId == null)
            eventId = 0;
        else
            eventId = eventId + 1;
        const event = { date: hearingDate, judgeid: judgeId, type: type, eventid: eventId, judgename: judgeName, createdbyuserid: userId };
        eventArray.push(event);
        $("#hearingCount").val(eventArray.length);
        eventTable.clear().rows.add(eventArray).draw();
        ClearEvent();
    }
    function AddDesignation() {
        var action = "CreateDesignation";
        var firstName = $("#txtFirstName").val();
        var lastName = $("#txtLastName").val();
        var middleName = $("#txtMiddleName").val();
        var county = $("#drpCounty").val();
        var tribunalCaseNumber = $("#txtTribunalCaseNumber").val();
        var appellateCaseNumber = $("#txtAppellateCaseNumber").val();
        var serviceDate = $("#txtServiceDate").val();
        var receiptDate = $("#txtReceiptDate").val();
        var attorneyIds = attorneyArray.map(atty => atty.id);
        var designation = {
            firstname: firstName,
            lastname: lastName, middlename: middleName, county: county,
            tribunalcasenumber: tribunalCaseNumber, appellatecasenumber: appellateCaseNumber,
            servicedate: serviceDate, receiptdate: receiptDate, createdbyuserid: userId,
            attorneys: attorneyIds.toString(), adminRole: adminRole,
        };
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: designationAddUrl,
                beforeSend: service.framework.setModuleHeaders,
                data: (designation),
                success: function (result) {
                    designationId = result.designationId;
                    if (designationId <= 0) {
                        ShowAlert("Error Attempting to Add Designation", result.error);
                    } else {
                        eventArray.forEach(evt => {
                            var newEvent = { hearingdate: evt.date, presidingjudgeid: evt.judgeid, hearingtype: evt.type, createdbyuserid: userId, designationid: designationId };
                            AddEvent(newEvent);
                        });
                        ClearDesignation();
                        $("#cmdSaveDesignationItem i").removeClass("fa-arrows-rotate fa-spin");
                        $("#cmdSaveDesignationItem i").addClass("floppy-disk");
                        $("#cmdSaveDesignationItem").prop("disabled", false);
                        if (eventMessage != null && eventMessage.length > 0)
                            ShowAlert("Errors Attempting to add Hearings: \n" + eventMessage);
                        else {
                            var designationAddModal = document.querySelector('#designationModal');
                            var modal = bootstrap.Modal.getInstance(designationAddModal);
                            if (!modal) {
                                modal = new bootstrap.Modal(document.getElementById('designationModal'));
                            }
                            modal.hide();
                            designationTable.draw();
                        }
                    }
                },
                error: function (xhr, status, error) {
                    ShowAlert("Error Attempting to Add Designation", "Unable to add Designation.\n\nMake sure you are logged in and try again. \n\nError:" + error);
                },
                always: function () {
                    $("#cmdSaveDesignationItem i").removeClass("fa-arrows-rotate fa-spin");
                    $("#cmdSaveDesignationItem i").addClass("floppy-disk");
                    $("#cmdSaveDesignationItem").prop("disabled", false);
                }
            });
        } catch (error) {
            ShowAlert("Error Attempting to Add Designation", "Unable to add Designation.\n\nMake sure you are logged in and try again.\n\n" + error);
        }
        return false;
    }
    function AddEvent(event) {
        try {
            $.ajax({
                type: "POST",
                cache: false,
                url: eventAddUrl,
                beforeSend: serviceEvent.framework.setModuleHeaders,
                data: (event),
                success: function (result) {
                    var eventId = result.EventId;
                    if (eventId <= 0) {
                        eventMessage += result.Message + ";";
                    }
                },
                error: function (xhr, status, error) {
                    eventMessage += error + " \n\n\n";
                }
            });
        } catch (error) {
            eventMessage += error + " \n\n\n";
        }
        return false;
    }
    function ClearEvent() {
        $("#txtHearingDate").val("");
        $("#txtDraftedBy").val("");
        $("#drpPresidingJudge").val("");
        $("#drpHearingType").val($("#drpHearingType option:first").val());
        $('#selectedjudgeId').val("");
        $('#judgeSearch').val("");
    }
    function ShowAlert(title, text) {
        Swal.fire({ title: title, html: text, icon: 'info', confirmButtonText: 'OK' });
    }
</script>
