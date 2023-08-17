<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditContact.ascx.cs" Inherits="tjc.Modules.EmployeeDB.EditContact" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ContactUrl %>"><i class="fas fa-list"></i>&nbsp;Back to  List</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#detail" data-toggle="tab"><i class="fas fa-user-edit"></i>&nbsp;Contact Details</a>
        </li>
        <li class="nav-item" id="phoneItem">
            <a class="nav-link" href="<%=PhoneUrl%>"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item" id="groupItem">
            <a class="nav-link" href="#groups" data-toggle="tab"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
    </ul>
    <asp:Literal ID="ltMessage" runat="server" />
    <div class="tab-content edit-form">
        <div id="detail" class="tab-pane active">
            <div class="row g-2">
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name<em>*</em>" ToolTip="Required" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="First Name is Required" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name<em>*</em>" ToolTip="Required" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Last Name is Required" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" Text="Middle Initial" />
                        <asp:TextBox runat="server" CssClass="form-control col-2" MaxLength="2" ID="txtMiddleInitial" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtTitle" Text="Title" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtTitle" />
                    </div>
                </div>
            </div>
            <div class="row g-2">
                <div class="col-5">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtAddress" Text="Street Address" />
                        <asp:TextBox runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="300" ID="txtAddress" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtCity" Text="City" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="30" ID="txtCity" />
                    </div>
                </div>
                <div class="col-2">
                    <div class="form-group">
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
                </div>
                <div class="col-2">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtZip" Text="Zip Code" />
                        <asp:TextBox runat="server" CssClass="form-control zip" MaxLength="12" ID="txtZip" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtPersonalEmail" Text="Personal Email" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="250" ID="txtPersonalEmail" />
                        <asp:RegularExpressionValidator ID="valPersonalEmail" runat="server" CssClass="label label-danger" ControlToValidate="txtPersonalEmail"
                            Display="Dynamic" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Work Email" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="250" ID="txtEmail" />
                        <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail"
                            Display="Dynamic" CssClass="label label-danger" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtLocation" Text="Location" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="150" ID="txtLocation" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County Works<em>*</em>" ToolTip="Required" />
                        <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" DataTextField="CountyName" DataValueField="CountyId" AppendDataBoundItems="true">
                            <asp:ListItem Text="<Select County>" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                    </div>
                </div>
                <div class="col-3">
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="chkActive" Checked="true" runat="server" Text="Active" />
                    </div>
                </div>

            </div>
            <hr />
            <p>
                <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary me-md" Text="Save" OnClick="cmdSave_Click" />
                <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
            </p>
        </div>
        <div id="groups" class="tab-pane">
            <p class="alert alert-info"><i class="fa fa-info-circle"></i>Select items from the Available Groups list and click the left arrow to add the selected groups to the employees Group Membership. Select items from the Group Membership list and click the right arrow to remove items from the Group Membership.</p>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgress" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
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
            PageInit();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {

            });
        });

    }(jQuery, window.Sys));
    function PageInit() {
        var tab = getUrlParameter('g');
        activaTab(tab);
        var employeeId =<%=EmployeeId%>;
        if (employeeId === undefined || employeeId === null || employeeId <= 0) {
            $("#phoneItem").toggle();
            $("#groupItem").toggle();
        }
        $(".datepicker").datepicker();
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
    function activaTab(tab) {
        $('.nav-tabs a[href="#' + tab + '"]').tab('show');
    };
</script>
