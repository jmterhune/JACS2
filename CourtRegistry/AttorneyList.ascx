<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttorneyList.ascx.cs" Inherits="tjc.Modules.CourtRegistry.AttorneyList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
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
        <li class="nav-item active">
            <a class="nav-link" href="#attorneys" data-toggle="tab">Attorneys</a>
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
        <div id="attorneys" class="tab-pane active">
            <div class="bg-dark text-white border-dark rounded p-2 mb-2">
                <div class="row">
                    <div class="col-auto me-2 pt-2"><strong>Filter By:</strong></div>
                    <div class="col-auto">
                        <input type="number" id="txtBarNumberSearch" tabindex="0" min="1" class="form-control search bar-filter" placeholder="Bar Number" maxlength="7" />
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtLastNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="Last Name" />
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtFirstNameSearch" tabindex="0" class="form-control search" maxlength="25" placeholder="First Name" />
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtEmailSearch" tabindex="0" class="form-control search" maxlength="50" placeholder="Email" />
                    </div>
                    <div class="col-auto">
                        <input type="text" id="txtFirmSearch" tabindex="0" class="form-control search" maxlength="50" placeholder="Law Firm" />
                    </div>
                    <div class="col-auto">
                        <button type="button" tabindex="-1" class="btn btn-primary" id="cmdSearch">Filter</button>
                    </div>
                </div>
            </div>
            <table id="tblAttorneys" class="table table-striped">
                <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>ID</th>
                        <th>Bar Number</th>
                        <th>Last Name</th>
                        <th>First Name</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Cell</th>
                        <th>Fax</th>
                        <th>Law Firm</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div class="modal fade" id="AttorneyModal" tabindex="-1" role="dialog" aria-labelledby="AttorneyModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="AttorneyModalLabel">Add / Edit Attorney</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="row form-group">
                            <div class="col-md-6">
                                <label for="txtBarNumber">Bar Number</label>
                                <input id="txtBarNumber" name="txtBarNumber" type="text" aria-describedby="txtBarNumberHelpBlock" required="required" class="form-control">
                                <span id="txtBarNumberHelpBlock" class="form-text text-muted">Enter the 7 digit Florida Bar Number</span>
                            </div>
                            <div class="col-md-6">
                                <label for="txtLawFirm">Law Firm</label>
                                <input id="txtLawFirm" name="txtLawFirm" type="text" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-6">
                                <label for="txtLastName">Last Name</label>
                                <input id="txtLastName" name="txtLastName" type="text" required="required" class="form-control">
                            </div>
                            <div class="col-md-6">
                                <label for="txtFirstName">First Name</label>
                                <input id="txtFirstName" name="txtFirstName" type="text" required="required" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-6">
                                <label for="txtEmail">Email</label>
                                <input id="txtEmail" name="txtEmail" type="text" class="form-control" required="required">
                            </div>
                            <div class="col-md-6">
                                <label for="txtPhone">Phone</label>
                                <input id="txtPhone" name="txtPhone" type="text" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-6">
                                <label for="txtCell">Cell Phone</label>
                                <input id="txtCell" name="txtCell" type="text" class="form-control">
                            </div>
                            <div class="col-md-6">
                                <label for="txtFax">Fax Machine</label>
                                <input id="txtFax" name="txtFax" type="text" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-12">
                                <label for="txtStreet">Steet Address</label>
                                <input id="txtStreet" name="txtStreet" type="text" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-4">
                                <label for="txtCity">City</label>
                                <input id="txtCity" name="txtCity" type="text" class="form-control">
                            </div>
                            <div class="col-md-4">
                                <label for="drpState">State</label>
                                <select id="drpState" name="drpState" class="form-control">
                                    <option value=""><-Select State-></option>
                                    <option value="AL">Alabama</option>
                                    <option value="AK">Alaska</option>
                                    <option value="AZ">Arizona</option>
                                    <option value="AR">Arkansas</option>
                                    <option value="CA">California</option>
                                    <option value="CO">Colorado</option>
                                    <option value="CT">Connecticut</option>
                                    <option value="DE">Delaware</option>
                                    <option value="DC">District Of Columbia</option>
                                    <option value="FL">Florida</option>
                                    <option value="GA">Georgia</option>
                                    <option value="HI">Hawaii</option>
                                    <option value="ID">Idaho</option>
                                    <option value="IL">Illinois</option>
                                    <option value="IN">Indiana</option>
                                    <option value="IA">Iowa</option>
                                    <option value="KS">Kansas</option>
                                    <option value="KY">Kentucky</option>
                                    <option value="LA">Louisiana</option>
                                    <option value="ME">Maine</option>
                                    <option value="MD">Maryland</option>
                                    <option value="MA">Massachusetts</option>
                                    <option value="MI">Michigan</option>
                                    <option value="MN">Minnesota</option>
                                    <option value="MS">Mississippi</option>
                                    <option value="MO">Missouri</option>
                                    <option value="MT">Montana</option>
                                    <option value="NE">Nebraska</option>
                                    <option value="NV">Nevada</option>
                                    <option value="NH">New Hampshire</option>
                                    <option value="NJ">New Jersey</option>
                                    <option value="NM">New Mexico</option>
                                    <option value="NY">New York</option>
                                    <option value="NC">North Carolina</option>
                                    <option value="ND">North Dakota</option>
                                    <option value="OH">Ohio</option>
                                    <option value="OK">Oklahoma</option>
                                    <option value="OR">Oregon</option>
                                    <option value="PA">Pennsylvania</option>
                                    <option value="RI">Rhode Island</option>
                                    <option value="SC">South Carolina</option>
                                    <option value="SD">South Dakota</option>
                                    <option value="TN">Tennessee</option>
                                    <option value="TX">Texas</option>
                                    <option value="UT">Utah</option>
                                    <option value="VT">Vermont</option>
                                    <option value="VA">Virginia</option>
                                    <option value="WA">Washington</option>
                                    <option value="WV">West Virginia</option>
                                    <option value="WI">Wisconsin</option>
                                    <option value="WY">Wyoming</option>
                                </select>
                            </div>
                            <div class="col-md-4">
                                <label for="txtZipCode">Zip Code</label>
                                <input id="txtZipCode" name="txtZipCode" type="text" class="form-control">
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-12">
                                <label for="txtLanguages">Languages</label>
                                <input id="txtLanguages" name="txtLanguages" type="text" aria-describedby="txtLanguagesHelpBlock" class="form-control">
                                <span id="txtLanguagesHelpBlock" class="form-text text-muted">Enter each language spoken separated by a semicolon (;)</span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer justify-content-between">
                        <input type="hidden" id="hdAttorneyId" name="hdAttorneyId" />
                        <button type="button" id="cmdSave" class="btn btn-primary">Save</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
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
    var barNumber = -1;
    var lastName = null;
    var firstName = null;
    var email = null;
    var lawFirm = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 6;
    var currentPage = 0;
    var attorneyUrl = null;
    var deleteUrl = null;
    var service = {
        path: "Attorney",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceManage = {
        path: "AttorneyManage",
        framework: $.ServicesFramework(moduleId)
    };
    var serviceDelete = {
        path: "AttorneyDelete",
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
        serviceManage.baseUrl = serviceManage.framework.getServiceRoot(serviceManage.path);
        serviceDelete.baseUrl = serviceDelete.framework.getServiceRoot(serviceDelete.path);
        attorneyUrl = `${service.baseUrl}AttorneyAPI/GetAttorneyListItems/${recordCount}`;
        attorneySaveUrl = `${service.baseUrl}AttorneyAPI/SaveAttorney/`;
        deleteUrl = `${serviceDelete.baseUrl}AttorneyAPI/Delete/`;
        var appTable = $('#tblAttorneys').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: attorneyUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.barNumber = barNumber;
                    data.firstName = firstName;
                    data.lastName = lastName;
                    data.email = email;
                    data.lawFirm = lawFirm;
                    delete data.columns;
                },
            },
            columns: [{
                data: "attorneyid", render: function (data, type, row, meta) {
                    return `<a class="edit-atty text-primary" href="#" data-id="${row.attorneyid}"><i class="fas fa-edit"></i></a>`;
                }, className: "command-item", orderable: false
            },
                { data: "attorneyid" },
                { data: "barnumber" },
                { data: "lastname" },
                { data: "firstname" },
                { data: "email" },
                { data: "phone" },
                { data: "cell" },
                { data: "fax" },
                { data: "lawfirm" },
                {
                    data: "attorneyid", render: function (data, type, row, meta) {
                        return `<a class="delete confirm text-danger" aria-role="button" title="Delete Attorney" data-attorneyid="${row.attorneyid}" href="#"><i class="fas fa-trash"></i></a>`;
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
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#AttorneyModal"><i class="fa fa-plus"></i>&nbsp;Add Attorney</button>');

        appTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".confirm").off("click.swalDelete").on("click.swalDelete", function (e) {
                e.preventDefault();
                var attorneyId = $(this).data("attorneyid");
                Registry.confirm({
                    title: 'Delete Attorney?',
                    text: 'This action cannot be undone.',
                    icon: 'warning',
                    confirmText: 'Yes, delete',
                    confirmColor: '#d33'
                }, function () {
                    $.ajax({
                        url: deleteUrl + attorneyId,
                        type: 'DELETE',
                        success: function () {
                            appTable.draw(false);
                            Registry.notify('Attorney deleted.', 'success');
                        },
                        error: function (err) {
                            Registry.notify('Error attempting delete: ' + (err.statusText || ''), 'error');
                        }
                    });
                });
            });
        });
        $.fn.dataTable.ext.errMode = function () { Registry.notify('Error while loading the table data. Please refresh.', 'error'); };
        $("#txtBarNumberSearch,#txtLastNameSearch,#txtFirstNameSearch,#txtEmailSearch,#txtFirmSearch").on("blur", function (e) {
            $("#cmdSearch").trigger("click");
        });
         
        $(document).on('click', '.edit-atty', function (e) {
            e.preventDefault();
            var attyId = $(this).data("id");
            if (attyId > 0) {
                GetAttorney(attyId);
                $("#AttorneyModal").modal("show");
            }
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            var barNumberString = $("#txtBarNumberSearch").val();
            if (barNumberString.length > 0)
                barNumber = Number(barNumberString);
            else
                barNumber = -1;

            lastName = $("#txtLastNameSearch").val();
            firstName = $("#txtFirstNameSearch").val();
            email = $("#txtEmailSearch").val();
            lawFirm = $("#txtFirmSearch").val();
            appTable.draw();
        });
        $("#cmdSave").on("click", function (e) {
            e.preventDefault();
            SaveAttorney();
        });
        $(".date-picker").datepicker();
    }
    function GetAttorney(attyId) {
        $.ajax({
            url: `${serviceManage.baseUrl}AttorneyAPI/GetAttorney/${attyId}`,
            type: 'GET',
            success: function (result) {
                if (result) {
                    $('#txtBarNumber').val(result.attorney.barnumber);
                    $('#txtLastName').val(result.attorney.lastname);
                    $('#txtFirstName').val(result.attorney.firstname);
                    $('#txtEmail').val(result.attorney.email);
                    $('#txtLawFirm').val(result.attorney.lawfirm);
                    $('#txtLanguages').val(result.attorney.languages);
                    $('#txtStreet').val(result.attorney.street);
                    $('#txtCity').val(result.attorney.city);
                    $('#drpState').val(result.attorney.state);
                    $('#txtZipCode').val(result.attorney.zipcode);
                    $('#txtPhone').val(result.attorney.phone);
                    $('#txtCell').val(result.attorney.cell);
                    $('#txtFax').val(result.attorney.fax);
                    $('#hdAttorneyId').val(attyId);
                    ['txtPhone', 'txtCell', 'txtFax'].forEach(function (id) {
                        var el = document.getElementById(id);
                        if (el) el.dispatchEvent(new Event('input'));
                    });
                }
            },
            error: function (err) {
                Registry.notify('Failed to load attorney: ' + (err.statusText || ''), 'error');
            }
        });
    }
    function SaveAttorney() {
        var attorney = {
            BarNumber: $('#txtBarNumber').val(),
            LastName: $('#txtLastName').val(),
            FirstName: $('#txtFirstName').val(),
            Email: $('#txtEmail').val(),
            LawFirm: $('#txtLawFirm').val(),
            Languages: $('#txtLanguages').val(),
            Street: $('#txtStreet').val(),
            City: $('#txtCity').val(),
            State: $('#drpState').val(),
            ZipCode: $('#txtZipCode').val(),
            Phone: $('#txtPhone').val(),
            Cell: $('#txtCell').val(),
            Fax: $('#txtFax').val(),
            AttorneyId: $('#hdAttorneyId').val()
        };
        $.ajax({
            url: attorneySaveUrl,
            type: 'POST',
            dataType: 'json',
            beforeSend: service.framework.setModuleHeaders,
            data: attorney,
            success: function (result) {
                if (result) {
                    $("#AttorneyModal").modal("hide");
                    ClearForm();
                    $('#tblAttorneys').DataTable().draw(false);
                    Registry.notify(Number(attorney.AttorneyId) > 0
                        ? 'Attorney updated.' : 'Attorney created.', 'success');
                }
            },
            error: function (err) {
                $("#AttorneyModal").modal("hide");
                Registry.notify('Save failed: ' + (err.statusText || ''), 'error');
            }
        });
    }
    function ClearForm() {
        $('#txtBarNumber').val("");
        $('#txtLastName').val("");
        $('#txtFirstName').val("");
        $('#txtEmail').val("");
        $('#txtLawFirm').val("");
        $('#txtLanguages').val("");
        $('#txtStreet').val("");
        $('#txtCity').val("");
        $('#drpState').val("");
        $('#txtZipCode').val("");
        $('#txtPhone').val("");
        $('#txtCell').val("");
        $('#txtFax').val("");
        $('#hdAttorneyId').val("");
    }
    function ShowAlert(title, text) {
        Registry.notify(text || title, 'info');
    }
    function applyPhoneMask(input) {
        function format() {
            var digits = input.value.replace(/\D/g, '').slice(0, 10);
            var part = digits.match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
            if (!part) { return; }
            input.value = !part[2]
                ? part[1]
                : '(' + part[1] + ') ' + part[2] + (part[3] ? '-' + part[3] : '');
        }
        input.addEventListener('input', format);
        format();
    }
    (function () {
        function bindMasks() {
            ['txtPhone', 'txtCell', 'txtFax'].forEach(function (id) {
                var el = document.getElementById(id);
                if (el && !el.dataset.masked) {
                    el.dataset.masked = '1';
                    applyPhoneMask(el);
                }
            });
        }
        if (document.readyState !== 'loading') bindMasks();
        else document.addEventListener('DOMContentLoaded', bindMasks);
        if (window.Sys && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindMasks);
        }
    })();
</script>
