<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Edit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl %>"><i class="fas fa-list"></i>&nbsp;Back to  List</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#detail" data-toggle="tab"><i class="fas fa-user-edit"></i>&nbsp;Details</a>
        </li>
        <li class="nav-item" id="phoneItem">
            <a class="nav-link" href="<%=PhoneUrl%>"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item" id="groupItem">
            <a class="nav-link" href="#groups" data-toggle="tab"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
        <li class="nav-item" id="employmentItem">
            <a class="nav-link" href="<%=EmploymentUrl%>"><i class="fas fa-user-clock"></i>&nbsp;Employment History</a>
        </li>
        <li class="nav-item" id="contactItem">
            <a class="nav-link" href="<%=EmergencyContactUrl%>"><i class="fas fa-address-book"></i>&nbsp;Emergency Contacts</a>
        </li>
    </ul>
    <div class="tab-content edit-form">
        <div id="detail" class="tab-pane active">
            <asp:UpdatePanel runat="server" ID="upEmployee" OnUnload="upEmployee_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upEmployeeProgress" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <div class="row g-2">
                        <div class="col-4">
                            <fieldset>
                                <legend>Personal Info</legend>
                                <div class="mb-3">
                                    <button class="btn btn-primary btn-lg " data-bs-toggle="modal" data-bs-target="#photoModal">
                                        Select Employee Photo 
                                    </button>
                                    <asp:HyperLink runat="server" ID="lnkThumbnail" data-plugin-options="{'type':'image'}" ClientIDMode="Static" CssClass="employee-thumb img-thumbnail img-thumbnail-hover-icon lightbox">
                                        <asp:Image runat="server" ClientIDMode="Static" ID="imgEmployee" AlternateText="Employee Thumbnail" />
                                    </asp:HyperLink>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name<em>*</em>" ToolTip="Required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="First Name is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name<em>*</em>" ToolTip="Required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Last Name is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" Text="Middle Initial" />
                                    <asp:TextBox runat="server" CssClass="form-control col-2" MaxLength="2" ID="txtMiddleInitial" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtTitle" Text="Title" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtTitle" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSSN" Text="Social Security Number<em>*</em>" ToolTip="Required" />
                                    <asp:TextBox runat="server" CssClass="form-control ssn" MaxLength="9" ID="txtSSN" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSSN"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="SSN is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtBirthDate" Text="Birth Date<em>*</em>" ToolTip="Required" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtBirthDate" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBirthDate"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Birth Date is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpRace" Text="Race<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpRace" runat="server" CssClass="form-control" DataTextField="Description" DataValueField="RaceCode" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Race>" Value="" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpRace"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Race is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="rblGender" Text="Gender<em>*</em>" ToolTip="Required" />
                                    <asp:RadioButtonList ID="rblGender" RepeatLayout="Flow" CssClass="radio-button-list" RepeatDirection="Horizontal" runat="server">
                                        <asp:ListItem Text="Male" Value="M" />
                                        <asp:ListItem Text="Female" Value="F" />
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator Visible="false" ID="valGender" runat="server" ControlToValidate="rblGender"
                                        Display="Dynamic" ErrorMessage="Gender is Required" CssClass="label label-danger" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" RepeatLayout="Flow" AssociatedControlID="rblWorksFor" Text="Employee of<em>*</em>" ToolTip="Required" />
                                    <asp:RadioButtonList ID="rblWorksFor" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio-button-list" runat="server">
                                        <asp:ListItem Text="County" Value="C" />
                                        <asp:ListItem Text="State" Value="S" />
                                        <asp:ListItem Text="Other" Value="O" />
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator Visible="false" ID="valEmpOf" runat="server" CssClass="label label-danger" ControlToValidate="rblWorksFor"
                                        Display="Dynamic" ErrorMessage="Employee of is Required" />
                                </div>
                                <div class="form-check form-switch">
                                    <asp:CheckBox ID="chkActive" Checked="true" runat="server" Text="Active" />
                                </div>
                            </fieldset>
                        </div>
                        <div class="col-4">
                            <fieldset>
                                <legend>Address / Contact</legend>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAddress" Text="Street Address" />
                                    <asp:TextBox runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="300" ID="txtAddress" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtCity" Text="City" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="30" ID="txtCity" />
                                </div>
                                <div class="form-group row">
                                    <div class="col-7">
                                        <asp:Label runat="server" AssociatedControlID="drpState" Text="State" />
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
                                    <div class="col-5">
                                        <asp:Label runat="server" AssociatedControlID="txtZip" Text="Zip Code" />
                                        <asp:TextBox runat="server" CssClass="form-control zip" MaxLength="12" ID="txtZip" />

                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtPersonalEmail" Text="Personal Email" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="250" ID="txtPersonalEmail" />
                                    <asp:RegularExpressionValidator ID="valPersonalEmail" runat="server" CssClass="label label-danger" ControlToValidate="txtPersonalEmail"
                                        Display="Dynamic" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Work Email" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="250" ID="txtEmail" />
                                    <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail"
                                        Display="Dynamic" CssClass="label label-danger" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpLocation" Text="Office Location" />
                                    <asp:DropDownList ID="drpLocation" runat="server" CssClass="form-control" DataTextField="Description" DataValueField="OfficeLocationId" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Location>" Value="" />
                                    </asp:DropDownList>
                                </div>
                            </fieldset>
                            <fieldset class="mt-2">
                                <legend>Transferred Leave Balances</legend>
                                <div class="form-group row">
                                    <div class="col-7">
                                        <asp:Label runat="server" AssociatedControlID="txtAnnualLeave" Text="Annual Leave" />
                                        <asp:TextBox runat="server" CssClass="form-control" TextMode="Number" ID="txtAnnualLeave" />
                                    </div>
                                    <div class="col-5">
                                        <asp:Label runat="server" AssociatedControlID="txtSickLeave" Text="Sick Leave" />
                                        <asp:TextBox runat="server" CssClass="form-control" TextMode="Number" ID="txtSickLeave" />
                                    </div>
                                </div>
                            </fieldset>
                            <fieldset class="mt-2">
                                <legend>Access Card Info</legend>
                                <div class="form-group row">
                                    <div class="col-7">
                                        <asp:Label runat="server" AssociatedControlID="txtDeSotoAccess" Text="DeSoto" />
                                        <asp:TextBox runat="server" MaxLength="50" CssClass="form-control" ID="txtDeSotoAccess" />
                                    </div>
                                    <div class="col-5">
                                        <asp:Label runat="server" AssociatedControlID="txtSarasotaAccess" Text="Sarasota" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtSarasotaAccess" />
                                    </div>
                                </div>
                                <div class="form-check form-switch">
                                    <asp:CheckBox ID="chkManateeAccess" runat="server" Text="Manatee" />
                                </div>
                            </fieldset>
                        </div>
                        <div class="col-4">
                            <fieldset>
                                <legend>Employment Info</legend>
                                <div class="form-group row">
                                    <div class="col-6">
                                        <asp:Label runat="server" AssociatedControlID="txtHireDate" Text="Hire Date<em>*</em>" ToolTip="Required" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="10" ID="txtHireDate" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHireDate"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Hire Date is Required" />
                                    </div>
                                    <div class="col-6">
                                        <asp:Label runat="server" AssociatedControlID="txtServiceDate" Text="Service Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="10" ID="txtServiceDate" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtTerminationDate" Text="Termination Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="10" ID="txtTerminationDate" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpSupervisor" Text="Supervisor" />
                                    <asp:DropDownList ID="drpSupervisor" runat="server" CssClass="form-control" DataTextField="DataText" DataValueField="DataValue" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Supervisor>" Value="" />
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpDepartment" Text="Department / Unit / Group<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpDepartment" runat="server" CssClass="form-control" DataTextField="GroupName" DataValueField="GroupId" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Department>" Value="" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpDepartment"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Department is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpJobGroup" Text="Job Category<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpJobGroup" runat="server" CssClass="form-control" DataTextField="Description" DataValueField="JobGroupId" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Category>" Value="" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJobGroup"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Job Category is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpJobClass" Text="Job Class<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpJobClass" runat="server" CssClass="form-control" DataTextField="ClassName" DataValueField="ClassId" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select Job Class>" Value="" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJobClass"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Job Class is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtPosition" Text="Position Number" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="150" ID="txtPosition" />
                                </div>

                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County Works<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" DataTextField="CountyName" DataValueField="CountyId" AppendDataBoundItems="true">
                                        <asp:ListItem Text="<Select County>" Value="" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="drpEmploymentType" Text="Employment Type<em>*</em>" ToolTip="Required" />
                                    <asp:DropDownList ID="drpEmploymentType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="<Select Employment Type>" Value="" />
                                        <asp:ListItem Text="Full-Time" Value="Full-Time" />
                                        <asp:ListItem Text="Part-Time" Value="Part-Time" />
                                        <asp:ListItem Text="OPS" Value="OPS" />
                                        <asp:ListItem Text="Intern" Value="Intern" />
                                        <asp:ListItem Text="Contract" Value="Contract" />
                                        <asp:ListItem Text="Other" Value="Other" />

                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpEmploymentType"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Employment Type is Required" />
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSalary" Text="Salary" />
                                    <asp:TextBox runat="server" CssClass="form-control money" MaxLength="50" ID="txtSalary" />
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <hr />
                    <p>
                        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary me-md" Text="Save" OnClick="cmdSave_Click" />
                        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
                    </p>

                    <div class="modal fade" id="photoModal" tabindex="-1" role="dialog" aria-labelledby="photoModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="photoModalLabel">Employee Photo</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div>
                                        <asp:PlaceHolder ID="phFileUpload" runat="server" />
                                    </div>
                                    <div class="clearFix"></div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="groups" class="tab-pane">
            <asp:UpdatePanel runat="server" ID="upGroups" OnUnload="upGroups_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upGroupProgress" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <p class="alert alert-info"><i class="fa fa-info-circle"></i>Select items from the Available Groups list and click the left arrow to add the selected groups to the employees Group Membership. Select items from the Group Membership list and click the right arrow to remove items from the Group Membership.</p>
                    <asp:Literal ID="ltMessage" runat="server" />

                    <div class="row">
                        <div class="col-auto">
                            <asp:Label ID="lblMembership" runat="server" AssociatedControlID="lsMembership">Group Membersip</asp:Label>
                            <asp:ListBox ID="lsMembership" SelectionMode="Multiple" CssClass="group-list" DataTextField="GroupName" DataValueField="GroupId" runat="server" Rows="10" />
                        </div>
                        <div class="col-1 list-commands">
                            <div class="text-center mt-5 mb-3">
                                <asp:LinkButton CausesValidation="false" ToolTip="Add Selected items to the Group Membership" ID="cmdAddGroup" runat="server" OnClick="cmdAddGroup_Click"><i class="fas fa-arrow-alt-circle-left"></i></asp:LinkButton>
                            </div>
                            <div class="text-center">
                                <asp:LinkButton CausesValidation="false" ToolTip="Remove Selected items from the Group Membership" ID="cmdRemoveGroup" runat="server" OnClick="cmdRemoveGroup_Click"><i class="fas fa-arrow-alt-circle-right"></i></asp:LinkButton>
                            </div>
                        </div>
                        <div class="col-auto">
                            <asp:Label ID="lblGroups" runat="server" AssociatedControlID="lsGroups">Available Groups</asp:Label>
                            <asp:ListBox ID="lsGroups" SelectionMode="Multiple" CssClass="group-list" DataTextField="GroupName" DataValueField="GroupId" runat="server" Rows="10" />
                        </div>

                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdAddGroup" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="cmdRemoveGroup" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>


</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.js" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit()
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            PageInit()
        });
    }(jQuery, window.Sys));

    function PageInit() {
        var myModalEl = document.getElementById('photoModal');
        myModalEl.addEventListener('hidden.bs.modal', function (event) {
            var fileInfo = JSON.parse($(".file input").val());
            if (fileInfo != undefined & fileInfo != null) {
                var fileId = fileInfo.selectedItem.key;
                if (fileId != null) {
                    $("#lnkThumbnail").show();
                    $("#lnkThumbnail").attr("href", `/DnnImageHandler.ashx?mode=securefile&fileId=${fileId}&MaxHeight=25`);
                    $("#imgEmployee").attr("src", `/DnnImageHandler.ashx?mode=securefile&fileId=${fileId}&MaxHeight=25`);
                }
            }
        });
        if ($("#lnkThumbnail").attr("href") == "")
            $("#lnkThumbnail").hide();
        var qs = getUrlParameter('g');
        var tab = "";
        if (qs) {
            tab = `#${qs}`;
        }
        if (tab != "") {
            activeTab(tab);
        }
        var employeeId =<%=EmployeeId%>;
        if (employeeId === undefined || employeeId === null || employeeId <= 0) {
            $("#phoneItem").toggle();
            $("#groupItem").toggle();
            $("#employmentItem").toggle();
            $("#photoItem").toggle();
            $("#contactItem").toggle();
        }
        $(".datepicker").datepicker();
        $('.ssn').mask('000-00-0000');
        $('.zip').mask('00000-000');
    }
    var getUrlParameter = function getUrlParameter(sParam) {
        var sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
            }
        }
        return false;
    };
    function activeTab(tab) {
        $('.nav-tabs a[href="' + tab + '"]').tab('show');
    };
</script>
