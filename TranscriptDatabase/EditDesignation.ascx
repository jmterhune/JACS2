<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDesignation.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.EditDesignation" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
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
        <div id="editDesignation" class="tab-pane active">
            <div class="row">
                <div class="col-md-3">
                    <label for="txtLastName">Last Name</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtLastName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                        ErrorMessage="Last Name Is Required" ControlToValidate="txtLastName" runat="server" />
                </div>
                <div class="col-md-3">
                    <label for="txtFirstName">First Name</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtFirstName" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                        ErrorMessage="First Name Is Required" ControlToValidate="txtFirstName" runat="server" />
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
                    <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="new" CssClass="label label-danger"
                        ErrorMessage="County Is Required" ControlToValidate="drpCounty" runat="server" />
                </div>
            </div>
            <fieldset class="outline-fieldset mb-2">
                <h5 class="mb-0">Add one or more attorneys to designation</h5>
                <div class="form-text ms-2 mb-2">Select the attorney from the drop down then click the "Add Selected Attorney". If the attorney does not exist in the drop down, click Add New Attorney to add them.</div>

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
                        <button type="button" id="cmdAttorney" class="btn btn-dark float-end" data-toggle="modal" data-target="#EditAttorneyModal">Add New Attorney</button>
                    </div>
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
                <div class="col-md-4">
                    <label for="txtTribunalCaseNumber">Tribunal Case Number</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtTribunalCaseNumber" ClientIDMode="Static" TextMode="MultiLine" Rows="2" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtAppellateCaseNumber">Appellate Case Number</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtAppellateCaseNumber" ClientIDMode="Static" TextMode="MultiLine" Rows="2" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-auto">
                    <label for="txtServiceDate">Service Date</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtServiceDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                </div>
                <div class="col-auto">
                    <label for="txtReceiptDate">Receipt Date</label>
                    <asp:TextBox AutoCompleteType="Disabled" ID="txtReceiptDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                </div>
                <div class="col-auto">
                    <label for="txtDueDate">Due Date</label>
                    <asp:TextBox AutoCompleteType="Disabled" style="display:none" ID="txtDueDate" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                     <asp:TextBox  ID="txtDueDateReadonly" ClientIDMode="Static" Enabled="false" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-auto pt-4">
                    <button id="cmdChangeDueDate" class="btn btn-dark">Change Due Date</button>

                </div>
            </div>
            <div class="row form-group mt-2 checkbox">
                <div class="col-auto">
                    <asp:CheckBox ID="chkPublicDefender" Text="Has the Public Defender been appointed / Special-Appointed?" runat="server" />
                </div>
                <div class="col-auto">
                    <asp:CheckBox ID="chkCourtAppointed" Text="Has a Court Appointed Attorney been appointed?" runat="server" />
                </div>
                <div class="col-auto">
                    <asp:CheckBox ID="chkIndigent" Text="Has the defendant been Declared Indigent for Costs?" runat="server" />
                </div>

            </div>
            <asp:HiddenField ID="hdDesignationId" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hdAttorneyIds" runat="server" ClientIDMode="Static" />
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
            <hr />
            <div class="mb-3">
                <asp:Button ID="cmdUpdate" ClientIDMode="Static" CssClass="btn btn-primary" Text="Update Designation" runat="server" OnClick="cmdUpdate_Click" />
                <asp:HyperLink ID="lnkCancel" CssClass="btn btn-dark" Text="Cancel" runat="server" />
                <asp:LinkButton ID="cmdDelete" ClientIDMode="Static" CssClass="btn btn-secondary float-end" Text="Delete Designation" runat="server" OnClick="cmdDelete_Click" />
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdOldDueDate" runat="server" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var isAdmin = "<%=IsAdmin%>";
    var attorneyId = null;
    var designationId = null;
    var attorneyTable = null;
    var attorneyArray = [<%=AttorneyArray%>];
    var attorneyDropDownUrl = null;
    var attorneyAddUrl = null;
    var attyOptions = [];
    var serviceAttorney = {
        path: "TranscriptAttorney",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            $(".date-picker").on("blur", function (e) {
                var date = $(this).val();
                $(this).val(date.replace(/\.|-/g, "/"));
            });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        designationId = <%=DesignationId%>;
        serviceAttorney.baseUrl = serviceAttorney.framework.getServiceRoot(serviceAttorney.path);
        attorneyAddUrl = `${serviceAttorney.baseUrl}Attorney/CreateAttorney/`;
        attorneyDropDownUrl = `${serviceAttorney.baseUrl}Attorney/GetAttorneyDropDown/`;
        attyOptions = fetchAttorneyOptions();
        attorneyTable = $('#tblAttorneys').DataTable({
            searching: false,
            autoWidth: true,
            columns: [
                { data: "name" },
                { data: "office" },
                {
                    data: "id", render: function (data, type, row, meta) {
                        return `<a title="Remove Attorney" data-id="${row.id}" class="remove-attorney"  href="#"><i class="fas fa-trash"></i></a>`;
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
        $.fn.dataTable.ext.errMode = () => ShowAlert("Error Building Record List", "Error while loading the table data. Please refresh");
        $(document).on('click', '.remove-attorney', function (e) {
            e.preventDefault();
            var attorneyId = $(this).data("id");
            $.dnnConfirm({
                text: 'Are you sure you wish to remove this Attorney from the list?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Remove Attorney?',
                callbackTrue: function () {
                    RemoveAttorney(attorneyId);
                }
            });
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
        $('#txtTribunalCaseNumber').on('blur', function () {
            this.value = this.value.toUpperCase();
        });
        $('#txtAppellateCaseNumber').on('blur', function () {
            this.value = this.value.toUpperCase();
        });
        $("#cmdUpdate").on("click", function (e) {
            $("#txtDueDate").hide();
            $("#txtDueDateReadonly").show();
            $("#cmdChangeDueDate").show();
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
        $("#cmdDelete").dnnConfirm({
            text: 'Are you sure you wish to Delete this Designation?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Designation?'
        });
        $("#cmdChangeDueDate").on("click", function (e) {
            e.preventDefault();
            $.dnnConfirm({
                text: 'Are you sure you want to change the due date?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Change Due Date?',
                callbackTrue: function () {
                    $("#txtDueDate").show();
                    $("#txtDueDateReadonly").hide();
                    $("#cmdChangeDueDate").hide();
                }
            });
           // $("#txtDueDate").removeAttr("disabled");
        });
        $("#cmdAddAttorney").on("click", function (e) {
            e.preventDefault();
            InsertAttorney();
        });
        $("#cmdSaveAttorney").on("click", function (e) {
            e.preventDefault();
            AddAttorney(e);
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
    function RemoveAttorney(attorneyId) {
        var attyRow = attorneyArray.find(row => row.id === attorneyId);
        if (attyRow.id > -1) {
            attorneyArray.splice(attyRow, 1);
            attorneyTable.clear().rows.add(attorneyArray).draw();
        }
        var attorneyIds = attorneyArray.map(atty => atty.id);
        $("#hdAttorneyIds").val(attorneyIds.toString());

    }
    function InsertAttorney() {
        const attorney = GetSelectedAttoney();
        if (attorney.id) {
            attorneyArray.push(attorney);
            attorneyTable.clear().rows.add(attorneyArray).draw();
            var attorneyIds = attorneyArray.map(atty => atty.id);
            $("#hdAttorneyIds").val(attorneyIds.toString());
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
        var attorney = { firstname: firstName, lastname: lastName, middlename: middleName, officeid: office, address1: address1, address2: address2, city: city, state: state, zip: zip, CreatedByUserID: userId };
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
                    attorneyArray.push(attorneyAdd);
                    attorneyTable.clear().rows.add(attorneyArray).draw();
                    var attorneyIds = attorneyArray.map(atty => atty.id);
                    $("#hdAttorneyIds").val(attorneyIds.toString());
                },
                error: function (xhr, status, error) {
                    // ShowAlert(xhr.responseText);
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

    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditNameModal').modal('show');
        } else {
            $('#EditNameModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
