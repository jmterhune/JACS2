<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditEmployee.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EditEmployee" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid mt-3">
    <h3>Employee: <asp:Label ID="lblEmployeeName" runat="server" /></h3>
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
    <asp:Literal ID="ltMessage" runat="server" />

    <ul class="nav nav-tabs" id="empTabs" role="tablist">
        <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tabDetails">Details</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabGroups">Groups</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabHistory">Employment History</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabPhones">Phones</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabContacts">Emergency Contacts</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabAccess">Access Cards</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabPhoto">Photo</a></li>
    </ul>

    <div class="tab-content pt-3">

        <!-- ======================= DETAILS TAB ======================= -->
        <div class="tab-pane fade show active" id="tabDetails" role="tabpanel">

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtFirstName.ClientID %>">First Name <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName"
                        ErrorMessage="First Name is required." Display="None" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtLastName.ClientID %>">Last Name <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName"
                        ErrorMessage="Last Name is required." Display="None" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtMiddleInitial.ClientID %>">Middle Initial</label>
                    <asp:TextBox ID="txtMiddleInitial" runat="server" CssClass="form-control" MaxLength="1" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtJobTitle.ClientID %>">Job Title</label>
                    <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtSsn.ClientID %>">SSN <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtSsn" runat="server" CssClass="form-control" MaxLength="9" />
                    <asp:RequiredFieldValidator ID="rfvSsn" runat="server" ControlToValidate="txtSsn"
                        ErrorMessage="SSN is required." Display="None" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtBirthDate.ClientID %>">Birth Date</label>
                    <asp:TextBox ID="txtBirthDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpRace.ClientID %>">Race</label>
                    <asp:DropDownList ID="drpRace" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpGender.ClientID %>">Gender</label>
                    <asp:DropDownList ID="drpGender" runat="server" CssClass="form-control">
                        <asp:ListItem Text="" Value="" />
                        <asp:ListItem Text="Male" Value="M" />
                        <asp:ListItem Text="Female" Value="F" />
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpAgency.ClientID %>">Agency of Employment</label>
                    <asp:DropDownList ID="drpAgency" runat="server" CssClass="form-control">
                        <asp:ListItem Text="" Value="" />
                        <asp:ListItem Text="Trial Court" Value="T" />
                        <asp:ListItem Text="State Attorney" Value="S" />
                        <asp:ListItem Text="Public Defender" Value="P" />
                        <asp:ListItem Text="Clerk" Value="C" />
                        <asp:ListItem Text="Other" Value="O" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-8">
                    <label for="<%=txtAddress.ClientID %>">Address</label>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" MaxLength="200" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtCity.ClientID %>">City</label>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtState.ClientID %>">State</label>
                    <asp:TextBox ID="txtState" runat="server" CssClass="form-control" MaxLength="2" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtZip.ClientID %>">Zip</label>
                    <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" MaxLength="10" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpCounty.ClientID %>">County</label>
                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-6">
                    <label for="<%=txtEmail.ClientID %>">Work Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="200" TextMode="Email" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="^\s*$|^[\w.!#$%&'*+\-/=?^_`{|}~]+@[\w\-]+(\.[\w\-]+)+\s*$"
                        ErrorMessage="Work Email is not a valid email address." Display="None" />
                </div>
                <div class="col-12 col-md-6 col-lg-6">
                    <label for="<%=txtPersonalEmail.ClientID %>">Personal Email</label>
                    <asp:TextBox ID="txtPersonalEmail" runat="server" CssClass="form-control" MaxLength="200" TextMode="Email" />
                    <asp:RegularExpressionValidator ID="revPersonalEmail" runat="server" ControlToValidate="txtPersonalEmail"
                        ValidationExpression="^\s*$|^[\w.!#$%&'*+\-/=?^_`{|}~]+@[\w\-]+(\.[\w\-]+)+\s*$"
                        ErrorMessage="Personal Email is not a valid email address." Display="None" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpOfficeLocation.ClientID %>">Office Location</label>
                    <asp:DropDownList ID="drpOfficeLocation" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtLocationName.ClientID %>">Location Name</label>
                    <asp:TextBox ID="txtLocationName" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpSupervisor.ClientID %>">Supervisor</label>
                    <asp:DropDownList ID="drpSupervisor" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpDepartment.ClientID %>">Department</label>
                    <asp:DropDownList ID="drpDepartment" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpJobGroup.ClientID %>">Job Group</label>
                    <asp:DropDownList ID="drpJobGroup" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpClass.ClientID %>">Class</label>
                    <asp:DropDownList ID="drpClass" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtPosition.ClientID %>">Position</label>
                    <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=drpEmploymentType.ClientID %>">Employment Type</label>
                    <asp:DropDownList ID="drpEmploymentType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="" Value="" />
                        <asp:ListItem Text="Full Time" Value="Full Time" />
                        <asp:ListItem Text="Part Time" Value="Part Time" />
                        <asp:ListItem Text="Temporary" Value="Temporary" />
                        <asp:ListItem Text="Contract" Value="Contract" />
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtSalary.ClientID %>">Salary</label>
                    <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtHireDate.ClientID %>">Hire Date</label>
                    <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtServiceDate.ClientID %>">Service Date</label>
                    <asp:TextBox ID="txtServiceDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtTerminationDate.ClientID %>">Termination Date</label>
                    <asp:TextBox ID="txtTerminationDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtAnnualLeave.ClientID %>">Annual Leave Balance</label>
                    <asp:TextBox ID="txtAnnualLeave" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtSickLeave.ClientID %>">Sick Leave Balance</label>
                    <asp:TextBox ID="txtSickLeave" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtBadgeNumber.ClientID %>">Badge Number</label>
                    <asp:TextBox ID="txtBadgeNumber" runat="server" CssClass="form-control" MaxLength="50" />
                </div>
            </div>

            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtSwnGroupId.ClientID %>">SWN Group Id</label>
                    <asp:TextBox ID="txtSwnGroupId" runat="server" CssClass="form-control" MaxLength="50" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label>DNN User</label>
                    <div class="input-group">
                        <asp:Label ID="lblUserId" runat="server" CssClass="form-control" />
                        <asp:LinkButton ID="cmdAssociateUser" runat="server" CssClass="btn btn-outline-secondary"
                            Text="Associate DNN User" CausesValidation="false" OnClick="cmdAssociateUser_Click" />
                    </div>
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <div class="form-check">
                        <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label" for="<%=chkIsActive.ClientID %>">Is Active</label>
                    </div>
                    <div class="form-check">
                        <asp:CheckBox ID="chkIsEmployee" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label" for="<%=chkIsEmployee.ClientID %>">Is Employee</label>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlSelectUser" runat="server" Visible="false" CssClass="card">
                <div class="card-header">Associate DNN User</div>
                <div class="card-body">
                    <label for="<%=txtSelectUserId.ClientID %>">DNN User Id</label>
                    <asp:TextBox ID="txtSelectUserId" runat="server" CssClass="form-control" />
                    <asp:LinkButton ID="cmdSelectUserSave" runat="server" CssClass="btn btn-primary"
                        Text="Set User Id" CausesValidation="false" OnClick="cmdSelectUserSave_Click" />
                    <asp:LinkButton ID="cmdSelectUserCancel" runat="server" CssClass="btn btn-secondary"
                        Text="Cancel" CausesValidation="false" OnClick="cmdSelectUserCancel_Click" />
                </div>
            </asp:Panel>
        </div>

        <!-- ======================= GROUPS TAB ======================= -->
        <div class="tab-pane fade" id="tabGroups" role="tabpanel">
            <label for="<%=lstGroups.ClientID %>">Group Membership</label>
            <asp:ListBox ID="lstGroups" runat="server" SelectionMode="Multiple" CssClass="form-control" Rows="12"
                DataTextField="GroupName" DataValueField="GroupID" />
            <div>
                <asp:LinkButton ID="cmdSaveGroups" runat="server" CssClass="btn btn-primary"
                    Text="Save Group Membership" CausesValidation="false" OnClick="cmdSaveGroups_Click" />
            </div>
        </div>

        <!-- ======================= EMPLOYMENT HISTORY TAB ======================= -->
        <div class="tab-pane fade" id="tabHistory" role="tabpanel">
            <h5>Position History</h5>
            <asp:Repeater ID="rptPositionHistory" runat="server" OnItemCommand="rptPositionHistory_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-striped table-sm">
                        <thead>
                            <tr>
                                <th>Start Date</th>
                                <th>End Date</th>
                                <th>Description</th>
                                <th>Entry Type</th>
                                <th>Internal</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("StartDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Eval("EndDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Eval("Description") %></td>
                        <td><%# Eval("EntryType") %></td>
                        <td><%# ((bool)Eval("IsInternal")) ? "Yes" : "No" %></td>
                        <td>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="Delete"
                                CommandArgument='<%# Eval("PositionId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Delete this position history entry?');" Text="Delete" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="card">
                <div class="card-header">Add Position</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtPosStartDate.ClientID %>">Start Date</label>
                            <asp:TextBox ID="txtPosStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtPosEndDate.ClientID %>">End Date</label>
                            <asp:TextBox ID="txtPosEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtPosEntryType.ClientID %>">Entry Type</label>
                            <asp:TextBox ID="txtPosEntryType" runat="server" CssClass="form-control" MaxLength="50" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <div class="form-check">
                                <asp:CheckBox ID="chkPosInternal" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%=chkPosInternal.ClientID %>">Is Internal</label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label for="<%=txtPosDescription.ClientID %>">Description</label>
                            <asp:TextBox ID="txtPosDescription" runat="server" CssClass="form-control" MaxLength="500" />
                        </div>
                    </div>
                    <div>
                        <asp:LinkButton ID="cmdAddPosition" runat="server" CssClass="btn btn-primary"
                            Text="Add Position" CausesValidation="false" OnClick="cmdAddPosition_Click" />
                    </div>
                </div>
            </div>

            <hr />

            <h5>Service History</h5>
            <asp:Repeater ID="rptServiceHistory" runat="server" OnItemCommand="rptServiceHistory_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-striped table-sm">
                        <thead>
                            <tr>
                                <th>Company Name</th>
                                <th>Hire Date</th>
                                <th>Termination Date</th>
                                <th>Last Pay Rate</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("CompanyName") %></td>
                        <td><%# Eval("HireDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Eval("TerminationDate", "{0:MM/dd/yyyy}") %></td>
                        <td><%# Eval("LastPayRate", "{0:C}") %></td>
                        <td>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="Delete"
                                CommandArgument='<%# Eval("ServiceId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Delete this service history entry?');" Text="Delete" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="card">
                <div class="card-header">Add Service</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtSvcCompanyName.ClientID %>">Company Name</label>
                            <asp:TextBox ID="txtSvcCompanyName" runat="server" CssClass="form-control" MaxLength="200" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtSvcHireDate.ClientID %>">Hire Date</label>
                            <asp:TextBox ID="txtSvcHireDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtSvcTerminationDate.ClientID %>">Termination Date</label>
                            <asp:TextBox ID="txtSvcTerminationDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtSvcLastPayRate.ClientID %>">Last Pay Rate</label>
                            <asp:TextBox ID="txtSvcLastPayRate" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div>
                        <asp:LinkButton ID="cmdAddService" runat="server" CssClass="btn btn-primary"
                            Text="Add Service" CausesValidation="false" OnClick="cmdAddService_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- ======================= PHONES TAB ======================= -->
        <div class="tab-pane fade" id="tabPhones" role="tabpanel">
            <asp:Repeater ID="rptPhones" runat="server" OnItemCommand="rptPhones_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-striped table-sm">
                        <thead>
                            <tr>
                                <th>Type</th>
                                <th>Number</th>
                                <th>Ext</th>
                                <th>Main</th>
                                <th>SWN Call</th>
                                <th>SWN Text</th>
                                <th>SWN Exclude Ext</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("PhoneType") %></td>
                        <td><%# Eval("PhoneNumber") %></td>
                        <td><%# Eval("Extension") %></td>
                        <td><%# ((bool)Eval("IsMain")) ? "Yes" : "" %></td>
                        <td><%# ((bool)Eval("SwnCall")) ? "Yes" : "" %></td>
                        <td><%# ((bool)Eval("SwnText")) ? "Yes" : "" %></td>
                        <td><%# ((bool)Eval("SwnExcludeExtension")) ? "Yes" : "" %></td>
                        <td>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="Delete"
                                CommandArgument='<%# Eval("PhoneId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Delete this phone?');" Text="Delete" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="card">
                <div class="card-header">Add Phone</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=drpPhoneType.ClientID %>">Type</label>
                            <asp:DropDownList ID="drpPhoneType" runat="server" CssClass="form-control">
                                <asp:ListItem Text="" Value="" />
                                <asp:ListItem Text="Home" Value="Home" />
                                <asp:ListItem Text="Mobile" Value="Mobile" />
                                <asp:ListItem Text="Work" Value="Work" />
                                <asp:ListItem Text="Pager" Value="Pager" />
                                <asp:ListItem Text="Fax" Value="Fax" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtPhoneNumber.ClientID %>">Number</label>
                            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" MaxLength="25" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtPhoneExtension.ClientID %>">Extension</label>
                            <asp:TextBox ID="txtPhoneExtension" runat="server" CssClass="form-control" MaxLength="10" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <div class="form-check">
                                <asp:CheckBox ID="chkPhoneIsMain" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%=chkPhoneIsMain.ClientID %>">Main</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="chkPhoneSwnCall" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%=chkPhoneSwnCall.ClientID %>">SWN Call</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="chkPhoneSwnText" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%=chkPhoneSwnText.ClientID %>">SWN Text</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="chkPhoneSwnExcludeExt" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%=chkPhoneSwnExcludeExt.ClientID %>">SWN Exclude Ext</label>
                            </div>
                        </div>
                    </div>
                    <div>
                        <asp:LinkButton ID="cmdAddPhone" runat="server" CssClass="btn btn-primary"
                            Text="Add Phone" CausesValidation="false" OnClick="cmdAddPhone_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- ======================= EMERGENCY CONTACTS TAB ======================= -->
        <div class="tab-pane fade" id="tabContacts" role="tabpanel">
            <asp:Repeater ID="rptContacts" runat="server" OnItemCommand="rptContacts_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-striped table-sm">
                        <thead>
                            <tr>
                                <th>First Name</th>
                                <th>Last Name</th>
                                <th>Relationship</th>
                                <th>Home</th>
                                <th>Work</th>
                                <th>Mobile</th>
                                <th>Order</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FirstName") %></td>
                        <td><%# Eval("LastName") %></td>
                        <td><%# Eval("Relationship") %></td>
                        <td><%# Eval("PhoneHome") %></td>
                        <td><%# Eval("PhoneWork") %></td>
                        <td><%# Eval("PhoneMobile") %></td>
                        <td><%# Eval("CallOrder") %></td>
                        <td>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="Delete"
                                CommandArgument='<%# Eval("ContactId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Delete this contact?');" Text="Delete" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="card">
                <div class="card-header">Add Emergency Contact</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtContactFirstName.ClientID %>">First Name</label>
                            <asp:TextBox ID="txtContactFirstName" runat="server" CssClass="form-control" MaxLength="100" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtContactLastName.ClientID %>">Last Name</label>
                            <asp:TextBox ID="txtContactLastName" runat="server" CssClass="form-control" MaxLength="100" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtContactRelationship.ClientID %>">Relationship</label>
                            <asp:TextBox ID="txtContactRelationship" runat="server" CssClass="form-control" MaxLength="50" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-3">
                            <label for="<%=txtContactCallOrder.ClientID %>">Call Order</label>
                            <asp:TextBox ID="txtContactCallOrder" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-4">
                            <label for="<%=txtContactPhoneHome.ClientID %>">Phone - Home</label>
                            <asp:TextBox ID="txtContactPhoneHome" runat="server" CssClass="form-control" MaxLength="25" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-4">
                            <label for="<%=txtContactPhoneWork.ClientID %>">Phone - Work</label>
                            <asp:TextBox ID="txtContactPhoneWork" runat="server" CssClass="form-control" MaxLength="25" />
                        </div>
                        <div class="col-12 col-md-6 col-lg-4">
                            <label for="<%=txtContactPhoneMobile.ClientID %>">Phone - Mobile</label>
                            <asp:TextBox ID="txtContactPhoneMobile" runat="server" CssClass="form-control" MaxLength="25" />
                        </div>
                    </div>
                    <div>
                        <asp:LinkButton ID="cmdAddContact" runat="server" CssClass="btn btn-primary"
                            Text="Add Contact" CausesValidation="false" OnClick="cmdAddContact_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- ======================= ACCESS CARDS TAB ======================= -->
        <div class="tab-pane fade" id="tabAccess" role="tabpanel">
            <div class="row">
                <div class="col-12 col-md-6 col-lg-4">
                    <div class="form-check">
                        <asp:CheckBox ID="chkManateeAccess" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label" for="<%=chkManateeAccess.ClientID %>">Manatee Access</label>
                    </div>
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtSarasotaAccess.ClientID %>">Sarasota Access</label>
                    <asp:TextBox ID="txtSarasotaAccess" runat="server" CssClass="form-control" MaxLength="50" />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <label for="<%=txtDesotoAccess.ClientID %>">DeSoto Access</label>
                    <asp:TextBox ID="txtDesotoAccess" runat="server" CssClass="form-control" MaxLength="50" />
                </div>
            </div>
        </div>

        <!-- ======================= PHOTO TAB ======================= -->
        <div class="tab-pane fade" id="tabPhoto" role="tabpanel">
            <div class="row">
                <div class="col-12 col-md-6">
                    <asp:Image ID="imgPhoto" runat="server" CssClass="img-fluid" />
                    <asp:HiddenField ID="hdnPhotoFileId" runat="server" />
                </div>
                <div class="col-12 col-md-6">
                    <label for="<%=fuPhoto.ClientID %>">Upload New Photo</label>
                    <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" />
                    <div class="form-text">Uploaded images are stored in the DNN "Employee-Photos" folder.</div>
                </div>
            </div>
        </div>

    </div>

    <hr />
    <div>
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
        <asp:LinkButton ID="cmdDelete" runat="server" CssClass="btn btn-danger" Text="Delete" Visible="false"
            CausesValidation="false" OnClick="cmdDelete_Click"
            OnClientClick="return confirm('Are you sure you want to delete this employee?');" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </div>
</div>
