<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditEmployee.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EditEmployee" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>


<!-- SweetAlert2 + Noty for confirms / toast notifications. CDN-hosted so we
     don't have to ship them as part of the install package. -->
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-edit.js" Priority="200" />

<div id="EmployeeEditForm" class="p-3">
    <%-- Header row: Employee name on the left, Back to List button on the
         right (ms-auto in a flex row pushes it flush-right). Lets the HR
         Admin bail out of an edit without scrolling to the bottom of the
         form for the Cancel button. --%>
    <div class="d-flex align-items-center mb-3">
        <h3 class="mb-0">Employee:&nbsp;<asp:Label ID="lblEmployeeName" runat="server" /></h3>
        <asp:HyperLink ID="cmdBackToList" runat="server" CssClass="btn btn-secondary ms-auto">
            <i class="fas fa-arrow-left"></i>&nbsp;Back to List
        </asp:HyperLink>
    </div>
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
    <asp:Literal ID="ltMessage" runat="server" />

    <%-- DNN Web API context for the JS layer (TabId/ModuleId/EmployeeId/Ssn).
         The AntiForgery token is injected as a hidden __RequestVerificationToken
         field by ServicesFramework.RequestAjaxAntiForgerySupport in Page_Load.
         Position/Service history are server-keyed by SSN, so we ship the SSN
         in the page context for the modal Save handlers to read. --%>
    <script type="text/javascript">
        window.__empdbCtx = {
            tabId: <%= TabId %>,
            moduleId: <%= ModuleId %>,
            employeeId: <%= EmployeeId %>,
            ssn: "<%= EmployeeSsn %>"
        };
    </script>

    <div class="tabs tabs-primary">
    <ul class="nav nav-tabs" id="empTabs" role="tablist">
        <li class="nav-item active"><a class="nav-link active" data-toggle="tab" data-bs-toggle="tab" href="#tabDetails">Details</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" data-bs-toggle="tab" href="#tabPhones">Phone Numbers</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" data-bs-toggle="tab" href="#tabGroups">Groups</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" data-bs-toggle="tab" href="#tabHistory">Employment</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" data-bs-toggle="tab" href="#tabPhoto">Photo</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" data-bs-toggle="tab" href="#tabContacts">Emergency Contacts</a></li>
    </ul>

    <div class="tab-content pt-3">

        <!-- ======================= DETAILS TAB ======================= -->
        <div class="tab-pane fade show active" id="tabDetails" role="tabpanel">
            <fieldset>
                <legend class="h5">Personal Information</legend>
                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtFirstName.ClientID %>">First Name <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName"
                            ErrorMessage="First Name is required." Display="None" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtLastName.ClientID %>">Last Name <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName"
                            ErrorMessage="Last Name is required." Display="None" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtMiddleInitial.ClientID %>">Middle Initial</label>
                        <asp:TextBox ID="txtMiddleInitial" runat="server" CssClass="form-control" MaxLength="1" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-6 col-lg-4">
                        <label for="<%=txtJobTitle.ClientID %>">Job Title</label>
                        <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtSsn.ClientID %>">SSN <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtSsn" runat="server" CssClass="form-control empdb-ssn-mask" MaxLength="11" placeholder="999-99-9999" />
                        <asp:RequiredFieldValidator ID="rfvSsn" runat="server" ControlToValidate="txtSsn"
                            ErrorMessage="SSN is required." Display="None" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtBirthDate.ClientID %>">Birth Date</label>
                        <asp:TextBox ID="txtBirthDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpRace.ClientID %>">Race</label>
                        <asp:DropDownList ID="drpRace" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label class="d-block">Gender</label>
                        <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-radio-row">
                            <asp:ListItem Text="Male" Value="M" />
                            <asp:ListItem Text="Female" Value="F" />
                        </asp:RadioButtonList>
                    </div>
                    <div class="col-12 col-md-6 col-lg-6">
                        <label class="d-block">Employee of</label>
                        <asp:RadioButtonList ID="rblAgency" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-radio-row">
                            <asp:ListItem Text="State" Value="S" />
                            <asp:ListItem Text="County" Value="C" />
                            <asp:ListItem Text="Other" Value="O" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </fieldset>
           
            <fieldset class="mt-3">
                <legend class="h5">Address / Phone</legend>
                <div class="row">
                    <div class="col-12 col-lg-6">
                        <label for="<%=txtAddressLine1.ClientID %>">Address Line 1</label>
                        <asp:TextBox ID="txtAddressLine1" runat="server" CssClass="form-control" MaxLength="200" />
                    </div>
                    <div class="col-12 col-lg-6">
                        <label for="<%=txtAddressLine2.ClientID %>">Address Line 2</label>
                        <asp:TextBox ID="txtAddressLine2" runat="server" CssClass="form-control" MaxLength="200" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtCity.ClientID %>">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpState.ClientID %>">State</label>
                        <asp:DropDownList ID="drpState" runat="server" CssClass="form-control">
                            <asp:ListItem Value="" Text="" />
                            <asp:ListItem Value="AL" Text="Alabama" />
                            <asp:ListItem Value="AK" Text="Alaska" />
                            <asp:ListItem Value="AZ" Text="Arizona" />
                            <asp:ListItem Value="AR" Text="Arkansas" />
                            <asp:ListItem Value="CA" Text="California" />
                            <asp:ListItem Value="CO" Text="Colorado" />
                            <asp:ListItem Value="CT" Text="Connecticut" />
                            <asp:ListItem Value="DE" Text="Delaware" />
                            <asp:ListItem Value="DC" Text="District of Columbia" />
                            <asp:ListItem Value="FL" Text="Florida" />
                            <asp:ListItem Value="GA" Text="Georgia" />
                            <asp:ListItem Value="HI" Text="Hawaii" />
                            <asp:ListItem Value="ID" Text="Idaho" />
                            <asp:ListItem Value="IL" Text="Illinois" />
                            <asp:ListItem Value="IN" Text="Indiana" />
                            <asp:ListItem Value="IA" Text="Iowa" />
                            <asp:ListItem Value="KS" Text="Kansas" />
                            <asp:ListItem Value="KY" Text="Kentucky" />
                            <asp:ListItem Value="LA" Text="Louisiana" />
                            <asp:ListItem Value="ME" Text="Maine" />
                            <asp:ListItem Value="MD" Text="Maryland" />
                            <asp:ListItem Value="MA" Text="Massachusetts" />
                            <asp:ListItem Value="MI" Text="Michigan" />
                            <asp:ListItem Value="MN" Text="Minnesota" />
                            <asp:ListItem Value="MS" Text="Mississippi" />
                            <asp:ListItem Value="MO" Text="Missouri" />
                            <asp:ListItem Value="MT" Text="Montana" />
                            <asp:ListItem Value="NE" Text="Nebraska" />
                            <asp:ListItem Value="NV" Text="Nevada" />
                            <asp:ListItem Value="NH" Text="New Hampshire" />
                            <asp:ListItem Value="NJ" Text="New Jersey" />
                            <asp:ListItem Value="NM" Text="New Mexico" />
                            <asp:ListItem Value="NY" Text="New York" />
                            <asp:ListItem Value="NC" Text="North Carolina" />
                            <asp:ListItem Value="ND" Text="North Dakota" />
                            <asp:ListItem Value="OH" Text="Ohio" />
                            <asp:ListItem Value="OK" Text="Oklahoma" />
                            <asp:ListItem Value="OR" Text="Oregon" />
                            <asp:ListItem Value="PA" Text="Pennsylvania" />
                            <asp:ListItem Value="RI" Text="Rhode Island" />
                            <asp:ListItem Value="SC" Text="South Carolina" />
                            <asp:ListItem Value="SD" Text="South Dakota" />
                            <asp:ListItem Value="TN" Text="Tennessee" />
                            <asp:ListItem Value="TX" Text="Texas" />
                            <asp:ListItem Value="UT" Text="Utah" />
                            <asp:ListItem Value="VT" Text="Vermont" />
                            <asp:ListItem Value="VA" Text="Virginia" />
                            <asp:ListItem Value="WA" Text="Washington" />
                            <asp:ListItem Value="WV" Text="West Virginia" />
                            <asp:ListItem Value="WI" Text="Wisconsin" />
                            <asp:ListItem Value="WY" Text="Wyoming" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtZip.ClientID %>">Zip</label>
                        <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" MaxLength="10" />
                    </div>

                </div>

                <div class="row">
                    <div class="col-12 col-lg-4">
                        <label for="<%=txtEmail.ClientID %>">Work Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="200" TextMode="Email" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                            ValidationExpression="^\s*$|^[\w.!#$%&'*+\-/=?^_`{|}~]+@[\w\-]+(\.[\w\-]+)+\s*$"
                            ErrorMessage="Work Email is not a valid email address." Display="None" />
                    </div>
                    <div class="col-12 col-lg-4">
                        <label for="<%=txtPersonalEmail.ClientID %>">Personal Email</label>
                        <asp:TextBox ID="txtPersonalEmail" runat="server" CssClass="form-control" MaxLength="200" TextMode="Email" />
                        <asp:RegularExpressionValidator ID="revPersonalEmail" runat="server" ControlToValidate="txtPersonalEmail"
                            ValidationExpression="^\s*$|^[\w.!#$%&'*+\-/=?^_`{|}~]+@[\w\-]+(\.[\w\-]+)+\s*$"
                            ErrorMessage="Personal Email is not a valid email address." Display="None" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpOfficeLocation.ClientID %>">Office Location</label>
                        <asp:DropDownList ID="drpOfficeLocation" runat="server" CssClass="form-control" />
                    </div>
                </div>

            </fieldset>
            <fieldset class="mt-3">
                <legend class="h5">Employment</legend>
                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtHireDate.ClientID %>">Hire Date</label>
                        <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtServiceDate.ClientID %>">Service Date</label>
                        <asp:TextBox ID="txtServiceDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtTerminationDate.ClientID %>">Termination Date</label>
                        <asp:TextBox ID="txtTerminationDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtBadgeNumber.ClientID %>">Badge Number</label>
                        <asp:TextBox ID="txtBadgeNumber" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpSupervisor.ClientID %>">Supervisor</label>
                        <asp:DropDownList ID="drpSupervisor" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpDepartment.ClientID %>">Department</label>
                        <asp:DropDownList ID="drpDepartment" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpJobGroup.ClientID %>">Job Group</label>
                        <asp:DropDownList ID="drpJobGroup" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpClass.ClientID %>">Class</label>
                        <asp:DropDownList ID="drpClass" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtPosition.ClientID %>">Position</label>
                        <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpCounty.ClientID %>">County</label>
                        <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=drpEmploymentType.ClientID %>">Employment Type</label>
                        <asp:DropDownList ID="drpEmploymentType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="" Value="" />
                            <asp:ListItem Text="Full-Time" Value="Full-Time" />
                            <asp:ListItem Text="Part-Time" Value="Part-Time" />
                            <asp:ListItem Text="OPS" Value="OPS" />
                            <asp:ListItem Text="Intern" Value="Intern" />
                            <asp:ListItem Text="Contract" Value="Contract" />
                            <asp:ListItem Text="Non-Employee" Value="Non-Employee" />
                            <asp:ListItem Text="Other" Value="Other" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtSalary.ClientID %>">Salary</label>
                        <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" />
                    </div>
                </div>

            </fieldset>
            <fieldset class="mt-3">
                <legend class="h5">Transferred Leave Balances</legend>
                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtAnnualLeave.ClientID %>">Annual Leave Balance</label>
                        <asp:TextBox ID="txtAnnualLeave" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtSickLeave.ClientID %>">Sick Leave Balance</label>
                        <asp:TextBox ID="txtSickLeave" runat="server" CssClass="form-control" />
                    </div>

                </div>



            </fieldset>
            <fieldset class="mt-3">
                <%-- DROP (Deferred Retirement Option Program) + Certified
                     Interpreter dates. All four fields are optional — most
                     employees have none of them. Feeds the new Employee
                     Reports module's DROP Participants and Certified
                     Interpreter Seniority reports. --%>
                <legend class="h5">DROP / Certification</legend>
                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtDropEntryDate.ClientID %>">DROP Entry Date</label>
                        <asp:TextBox ID="txtDropEntryDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtDropExitDate.ClientID %>">DROP Exit Date</label>
                        <asp:TextBox ID="txtDropExitDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtDropLeavePayout.ClientID %>">Leave Payout (hrs at DROP entry)</label>
                        <asp:TextBox ID="txtDropLeavePayout" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtCertificationDate.ClientID %>">Certification Date</label>
                        <asp:TextBox ID="txtCertificationDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                </div>
            </fieldset>
            <fieldset class="mt-3">
                <legend class="h5">Access Cards</legend>
                <div class="row">
                    <div class="col-12 col-md-6 col-lg-3">
                        <asp:CheckBox ID="chkManateeAccess" runat="server" Text="Manatee Access?" CssClass="form-check" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtSarasotaAccess.ClientID %>">Sarasota Access</label>
                        <asp:TextBox ID="txtSarasotaAccess" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <label for="<%=txtDesotoAccess.ClientID %>">DeSoto Access</label>
                        <asp:TextBox ID="txtDesotoAccess" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                </div>
            </fieldset>
            <div class="row">
                <div class="col-12 col-md-6 col-lg-3">
                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Is Active?" CssClass="form-check" Checked="true" />
                </div>
            </div>
        </div>

        <%-- ======================= GROUPS TAB (API-driven) ======================= --%>
        <div class="tab-pane fade" id="tabGroups" role="tabpanel">
            <h5 class="mb-2">Group Membership</h5>
            <div class="empdb-dual-list row">
                <div class="col-12 col-md-5">
                    <label class="empdb-dual-list-label">Selected Groups</label>
                    <ul id="empdbGroupsSelected" class="empdb-dual-list-box" data-list="selected">
                        <li class="empdb-dual-list-empty text-muted">No groups assigned.</li>
                    </ul>
                </div>
                <div class="col-12 col-md-2 empdb-dual-list-buttons">
                    <button type="button" class="btn btn-sm btn-secondary" id="empdbGroupAdd" title="Add selected">
                        <i class="fas fa-arrow-left"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-secondary" id="empdbGroupAddAll" title="Add all">
                        <i class="fas fa-angle-double-left"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-secondary" id="empdbGroupRemove" title="Remove selected">
                        <i class="fas fa-arrow-right"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-secondary" id="empdbGroupRemoveAll" title="Remove all">
                        <i class="fas fa-angle-double-right"></i>
                    </button>
                </div>
                <div class="col-12 col-md-5">
                    <label class="empdb-dual-list-label">Available Groups</label>
                    <ul id="empdbGroupsAvailable" class="empdb-dual-list-box" data-list="available">
                        <li class="empdb-dual-list-empty text-muted">Loading…</li>
                    </ul>
                </div>
            </div>
            <div class="empdb-dual-list-actions">
                <button type="button" id="empdbGroupSave" class="btn btn-primary">Save Group Membership</button>
            </div>
        </div>

        <!-- ======================= EMPLOYMENT TAB (API-driven) ======================= -->
        <div class="tab-pane fade" id="tabHistory" role="tabpanel">
            <h5 class="mb-2">Position History</h5>
            <button type="button" id="empdbPositionAdd" class="btn btn-sm btn-primary mb-2">
                <i class="fas fa-plus"></i>&nbsp;Add Position
            </button>
            <table id="empdbPositionsTable" class="table table-striped table-sm">
                <thead>
                    <tr>
                        <th class="command-item"></th>
                        <th>Entry Type</th>
                        <th>Description</th>
                        <th>Internal/External</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th class="command-item"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="7" class="text-muted text-center">Loading…</td></tr>
                </tbody>
            </table>

            <hr />

            <h5 class="mb-2">Service History</h5>
            <button type="button" id="empdbServiceAdd" class="btn btn-sm btn-primary mb-2">
                <i class="fas fa-plus"></i>&nbsp;Add Service
            </button>
            <table id="empdbServicesTable" class="table table-striped table-sm">
                <thead>
                    <tr>
                        <th class="command-item"></th>
                        <th>Company Name</th>
                        <th>Hire Date</th>
                        <th>Termination Date</th>
                        <th>Last Pay Rate</th>
                        <th class="command-item"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="6" class="text-muted text-center">Loading…</td></tr>
                </tbody>
            </table>
        </div>

        <!-- Add / Edit Position modal -->
        <div class="modal fade" id="empdbPositionModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add Position</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" name="PositionId" value="0" />
                        <div class="row">
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Start Date</label>
                                <input type="date" name="StartDate" class="form-control" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>End Date</label>
                                <input type="date" name="EndDate" class="form-control" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Entry Type</label>
                                <select name="EntryType" class="form-control">
                                    <option value=""></option>
                                    <option value="T">Transfer</option>
                                    <option value="P">Promotion</option>
                                    <option value="O">Other</option>
                                </select>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Type</label>
                                <select name="IsInternal" class="form-control">
                                    <option value="true">Internal</option>
                                    <option value="false">External</option>
                                </select>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-12">
                                <label>Description</label>
                                <input type="text" name="Description" class="form-control" maxlength="500" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="empdbPositionSave" class="btn btn-primary">Save</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Add / Edit Service modal -->
        <div class="modal fade" id="empdbServiceModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add Service</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" name="ServiceId" value="0" />
                        <div class="row">
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Company Name</label>
                                <input type="text" name="CompanyName" class="form-control" maxlength="200" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Hire Date</label>
                                <input type="date" name="HireDate" class="form-control" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Termination Date</label>
                                <input type="date" name="TerminationDate" class="form-control" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Last Pay Rate</label>
                                <input type="number" name="LastPayRate" step="0.01" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="empdbServiceSave" class="btn btn-primary">Save</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- ======================= PHONES TAB (API-driven, no postback) ======================= -->
        <div class="tab-pane fade" id="tabPhones" role="tabpanel">
            <h5 class="mb-2">Phone Numbers</h5>
            <button type="button" id="empdbPhoneAdd" class="btn btn-sm btn-primary mb-2">
                <i class="fas fa-plus"></i>&nbsp;Add Phone
            </button>
            <table id="empdbPhonesTable" class="table table-striped table-sm">
                <thead>
                    <tr>
                        <th class="command-item"></th>
                        <th>Type</th>
                        <th>Location</th>
                        <th>Number</th>
                        <th>Ext</th>
                        <th>Cascade</th>
                        <th class="text-center">SWN Call?</th>
                        <th class="text-center">SWN Text?</th>
                        <th class="text-center">SWN Exclude Ext?</th>
                        <th class="command-item"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="10" class="text-muted text-center">Loading…</td></tr>
                </tbody>
            </table>
            <%-- LocationName now ships on each row from the API
                 (PhoneInfo.LocationName is an [IgnoreColumn] populated by the
                 PhonesController), so no hidden lookup dropdown is needed. --%>
        </div>

        <!-- Add / Edit Phone modal -->
        <div class="modal fade" id="empdbPhoneModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add Phone</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" name="PhoneId" value="0" />
                        <div class="row">
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Type</label>
                                <%-- Phone type dropdown. The default option's value
                                     is "Other" (not empty) so unselected rows
                                     still satisfy the model's required PhoneType
                                     column and don't accidentally land in SWN's
                                     SMS-eligible bucket. Only "Work Cell" and
                                     "Mobile" match the cell/mobile SMS filter. --%>
                                <select name="PhoneType" class="form-control">
                                    <option value="Other">&lt; select type &gt;</option>
                                    <option value="Work">Work</option>
                                    <option value="Work Cell">Work Cell</option>
                                    <option value="Mobile">Mobile</option>
                                    <option value="Home">Home</option>
                                    <option value="Judicial Office">Judicial Office</option>
                                </select>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Location</label>
                                <%-- Plain HTML <select> so jQuery's [name="OfficeLocationId"]
                                     selector finds it (asp:DropDownList renders with an
                                     auto-generated ASP.NET name and would have ignored our
                                     custom name attribute, breaking fillForm/readForm). --%>
                                <select name="OfficeLocationId" class="form-control">
                                    <option value=""></option>
                                    <%= GetPhoneLocationOptions() %>
                                </select>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Number</label>
                                <input type="text" name="PhoneNumber" class="form-control empdb-phone-mask" maxlength="25" placeholder="(999) 999-9999" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Extension</label>
                                <input type="text" name="Extension" class="form-control" maxlength="10" />
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>SWN Cascade</label>
                                <input type="number" name="PhoneCascade" class="form-control" min="0" step="1" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-9">
                                <div class="form-check"><input type="checkbox" name="SwnCall" class="form-check-input" id="empdbPhCall" /><label class="form-check-label" for="empdbPhCall">SWN Call?</label></div>
                                <div class="form-check"><input type="checkbox" name="SwnText" class="form-check-input" id="empdbPhText" /><label class="form-check-label" for="empdbPhText">SWN Text?</label></div>
                                <div class="form-check"><input type="checkbox" name="SwnExcludeExtension" class="form-check-input" id="empdbPhExcl" /><label class="form-check-label" for="empdbPhExcl">SWN Exclude Ext?</label></div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="empdbPhoneSave" class="btn btn-primary">Save</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- ======================= EMERGENCY CONTACTS TAB (API-driven) ======================= -->
        <div class="tab-pane fade" id="tabContacts" role="tabpanel">
            <h5 class="mb-2">Emergency Contacts</h5>
            <button type="button" id="empdbContactAdd" class="btn btn-sm btn-primary mb-2">
                <i class="fas fa-plus"></i>&nbsp;Add Emergency Contact
            </button>
            <table id="empdbContactsTable" class="table table-striped table-sm">
                <thead>
                    <tr>
                        <th class="command-item"></th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Relationship</th>
                        <th>Home</th>
                        <th>Work</th>
                        <th>Mobile</th>
                        <th>Order</th>
                        <th class="command-item"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="9" class="text-muted text-center">Loading…</td></tr>
                </tbody>
            </table>
        </div>

        <!-- Add / Edit Emergency Contact modal -->
        <div class="modal fade" id="empdbContactModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add Emergency Contact</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" name="ContactId" value="0" />
                        <div class="row">
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>First Name</label>
                                <input type="text" name="FirstName" class="form-control" maxlength="100" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Last Name</label>
                                <input type="text" name="LastName" class="form-control" maxlength="100" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Relationship</label>
                                <input type="text" name="Relationship" class="form-control" maxlength="50" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <label>Call Order</label>
                                <input type="number" name="CallOrder" class="form-control" min="0" step="1" />
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-12 col-md-6 col-lg-4">
                                <label>Phone - Home</label>
                                <input type="text" name="PhoneHome" class="form-control empdb-phone-mask" maxlength="25" placeholder="(999) 999-9999" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-4">
                                <label>Phone - Work</label>
                                <input type="text" name="PhoneWork" class="form-control empdb-phone-mask" maxlength="25" placeholder="(999) 999-9999" />
                            </div>
                            <div class="col-12 col-md-6 col-lg-4">
                                <label>Phone - Mobile</label>
                                <input type="text" name="PhoneMobile" class="form-control empdb-phone-mask" maxlength="25" placeholder="(999) 999-9999" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="empdbContactSave" class="btn btn-primary">Save</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>

        <%-- ======================= PHOTO TAB (API-driven, drag-drop) ======================= --%>
        <div class="tab-pane fade" id="tabPhoto" role="tabpanel">
            <div class="row">
                <div class="col-12 col-md-6">
                    <div id="empdbPhotoPreview">
                        <img id="empdbPhotoImg" class="empdb-photo-img" alt="Employee photo"
                             src="<%= EmployeePhotoUrl %>"
                             style="<%= string.IsNullOrEmpty(EmployeePhotoUrl) ? "display:none;" : "" %>" />
                        <div id="empdbPhotoEmpty" class="empdb-photo-empty text-muted"
                             style="<%= string.IsNullOrEmpty(EmployeePhotoUrl) ? "" : "display:none;" %>">
                            No photo on file.
                        </div>
                    </div>
                    <div class="empdb-photo-actions">
                        <button type="button" id="empdbPhotoRemove" class="btn btn-sm btn-danger"
                                style="<%= string.IsNullOrEmpty(EmployeePhotoUrl) ? "display:none;" : "" %>">
                            <i class="fas fa-trash"></i>&nbsp;Remove Photo
                        </button>
                    </div>
                </div>
                <div class="col-12 col-md-6">
                    <label class="d-block">Upload Photo</label>
                    <div id="empdbPhotoDrop" class="empdb-photo-drop" tabindex="0">
                        <i class="fas fa-cloud-upload-alt empdb-photo-drop-icon"></i>
                        <div class="empdb-photo-drop-text">
                            Drag and drop an image here<br />
                            <span class="text-muted">or click to browse</span>
                        </div>
                        <input type="file" id="empdbPhotoFile" accept="image/*" hidden="hidden" />
                    </div>
                    <div class="form-text">Uploaded images are stored in the DNN &quot;Employee-Photos&quot; folder.</div>
                </div>
            </div>
        </div>

    </div>
    </div><%-- /tabs tabs-primary --%>

    <hr />
    <div>
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
        <asp:LinkButton ID="cmdDelete" runat="server" CssClass="btn btn-danger" Text="Delete" Visible="false"
            CausesValidation="false" OnClick="cmdDelete_Click"
            OnClientClick="return confirm('Are you sure you want to delete this employee?');" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </div>
</div>

<script type="text/javascript">
    // Wire tab clicks via jQuery — works for both Bootstrap 4 (loaded by Porto's
    // base.js) and Bootstrap 5. Some skins bundle BS4 which overrides any BS5,
    // so use jQuery's .tab() which is present in both versions.
    (function ($) {
        if (!$ || !$.fn) return;
        $(function () {
            // Tabs (BS4/BS5 both have .tab() via jQuery).
            if ($.fn.tab) {
                $('#empTabs').on('click', '.nav-link', function (e) {
                    e.preventDefault();
                    $(this).tab('show');
                });
            }

            // ASP.NET's CheckBox with Text= renders <span class="form-check">
            // <input/><label/></span>. BS5 expects form-check-input on the
            // input and form-check-label on the label, so we sprinkle the
            // classes on at runtime. Same trick used in CourtCounsel/Reports.
            $('#EmployeeEditForm .form-check input[type="checkbox"]').addClass('form-check-input');
            $('#EmployeeEditForm .form-check label').addClass('form-check-label');
        });
    })(window.jQuery);
</script>
<dnn:DnnCssInclude runat="server" FilePath="/DesktopModules/tjc.modules/EmployeeDB/module.css" />
