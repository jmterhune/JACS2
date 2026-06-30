<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditLog.ascx.cs" Inherits="tjc.Modules.FamilySelfHelp.EditLog" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="btn-group mb-2">
    <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary" runat="server">Search</asp:HyperLink>
    <asp:HyperLink ID="lnkDataEntry" CssClass="btn btn-primary active" runat="server">Data Entry</asp:HyperLink>
    <asp:HyperLink ID="lnkMerge" CssClass="btn btn-primary" Visible="false" runat="server">Merge Clients</asp:HyperLink>
    <asp:HyperLink ID="lnkReports" CssClass="btn btn-primary" Visible="false" runat="server">Reports</asp:HyperLink>
</div>
<asp:Panel ID="pnlExistingClient" runat="server" Visible="false" CssClass="pnlExist">
    <asp:LinkButton ID="cmdUpdateExisting" runat="server" CausesValidation="false" OnClick="cmdUpdateExisting_Click" CssClass="btn btn-primary" Text="Add Record to the Existing Client">
    </asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdChangeName" runat="server" CausesValidation="False" OnClick="cmdChangeName_Click" CssClass="btn btn-primary" Text="Go Back and Change the Name">
    </asp:LinkButton>
</asp:Panel>
<asp:Panel ID="pnlForm" runat="server">
    <div class="form-group row">
        <div class="col-4">
            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name<em>*</em>" ToolTip="Required" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="50" ID="txtLastName" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Last Name is Required" />
        </div>
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name<em>*</em>" ToolTip="Required" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="50" ID="txtFirstName" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="First Name is Required" />
        </div>
        <div class="col-1">
            <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" Text="<abbr title='Middle Initial'>MI</abbr>" />
            <asp:TextBox runat="server" CssClass="form-control  form-control-sm" MaxLength="1" ID="txtMiddleInitial" />
        </div>
        <div class="col-4">
            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="250" ID="txtEmail" />
            <asp:RegularExpressionValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ErrorMessage="Invalid email address" ControlToValidate="txtEmail" runat="server" />
        </div>
    </div>
    <div class="form-group row">
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Phone" />
            <asp:TextBox runat="server" CssClass="form-control  form-control-sm phone" MaxLength="50" ID="txtPhone" />
        </div>
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="txtServiceDate" Text="Service Date<em>*</em>" ToolTip="Required" />
            <asp:TextBox runat="server" CssClass="form-control  form-control-sm datepicker" MaxLength="50" ID="txtServiceDate" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtServiceDate"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Service Date is Required" />
            <asp:CompareValidator ID="valIsServiceDate" ControlToValidate="txtServiceDate" Type="Date" Operator="DataTypeCheck" runat="server" Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Invalid Date"></asp:CompareValidator>
        </div>
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="drpDivision" Text="Division" />
            <asp:DropDownList runat="server" ID="drpDivision" CssClass="form-control form-control-sm">
                <asp:ListItem Text="< Select Division >" Value="" />
                <asp:ListItem Text="Family Division 1" />
                <asp:ListItem Text="Family Division 2" />
                <asp:ListItem Text="Family Division 3" />
                <asp:ListItem Text="Family Division 4" />
                <asp:ListItem Text="South County Family Division 1" />
                 <asp:ListItem Text="South County Family Division 2" />
                <asp:ListItem Text="Family Division DeSoto" />
            </asp:DropDownList>
        </div>
        <div class="col-3">
            <asp:Label ID="lblTimeSpent" runat="server" AssociatedControlID="txtTimeSpent">Time Spent with Person<em>*</em></asp:Label>
            <asp:TextBox runat="server" ID="txtTimeSpent" CssClass="form-control  form-control-sm" TextMode="Number" min=".1" step=".01" MaxLength="50" />
            <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valTimeSpent" runat="server" ControlToValidate="txtTimeSpent" ErrorMessage="Required"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="form-group row">
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="rblHasAppointment" Text="Has Appointment<em>*</em>" ToolTip="Required" />
            <asp:RadioButtonList ID="rblHasAppointment" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control radio-button-list" runat="server">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valHasAppointment" runat="server" ControlToValidate="rblHasAppointment" ErrorMessage="Selection Required"></asp:RequiredFieldValidator>
        </div>
        <div class="col-4">
            <asp:Label runat="server" AssociatedControlID="rblClientType" Text="Client Type<em>*</em>" />
            <asp:RadioButtonList ID="rblClientType" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control radio-button-list" runat="server">
                <asp:ListItem Text="Self Help" />
                <asp:ListItem Text="Law Library" />
                <asp:ListItem Text="Both" />
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valClientType" runat="server" ControlToValidate="rblClientType" ErrorMessage="Selection Required"></asp:RequiredFieldValidator>
        </div>
        <div class="col-5">
            <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Numbers <span class='text-muted fst-italic'>(Leave Blank for New Case)</span>" />
            <asp:TextBox runat="server" CssClass="form-control" MaxLength="250" ID="txtCaseNumber" />
        </div>
    </div>
    <div class="row form-group">
        <div class="col-4">
            <fieldset class="fieldset-bordered d-block mb-2">
                <legend>How did they come to us?<em>*</em></legend>
                <asp:RadioButtonList ID="rblContactMethod" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="radio-button-list column-2" runat="server">
                    <asp:ListItem Text="Walk-in" />
                    <asp:ListItem Text="Telephone" />
                    <asp:ListItem Text="Appointment" />
                    <asp:ListItem Text="Court Ordered" />
                    <asp:ListItem Text="Email Question" />
                    <asp:ListItem Text="Other" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valContactMethod" runat="server" ControlToValidate="rblContactMethod" ErrorMessage="Selection Required"></asp:RequiredFieldValidator>
                <div class="other" id="ContactMethod">
                    <asp:Label ID="lblOtherContactMethod" runat="server" AssociatedControlID="txtContactMethodOther">If Other Please Complete</asp:Label>
                    <asp:TextBox runat="server" ID="txtContactMethodOther" CssClass="form-control form-control-sm" Text="Other" MaxLength="50" />
                    <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valContact" runat="server" ControlToValidate="txtContactMethodOther" ErrorMessage="Required"></asp:RequiredFieldValidator>
                </div>
            </fieldset>
            <fieldset class="fieldset-bordered d-block mb-2">
                <legend>Location<em>*</em></legend>
                <asp:RadioButtonList ID="rblLocation" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="radio-button-list column-2" runat="server">
                    <asp:ListItem Text="DeSoto" />
                    <asp:ListItem Text="Manatee" />
                    <asp:ListItem Text="Sarasota" />
                    <asp:ListItem Text="Other FL County" />
                    <asp:ListItem Text="Out of State" />
                    <asp:ListItem Text="Other / Unknown" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valLocation" runat="server" ControlToValidate="rblLocation" ErrorMessage="Selection Required"></asp:RequiredFieldValidator>
            </fieldset>
            <asp:CheckBox runat="server" CssClass="form-check ms-1" ID="chkInterpreterProvided" Text="Interpreter Provided?" />

        </div>
        <div class="col-8">
            <fieldset class="fieldset-bordered d-block mb-2">
                <legend>Case Type<em>*</em></legend>
                <asp:CheckBoxList ID="cblCaseType" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="radio-button-list column-4" runat="server">
                    <asp:ListItem Text="Adoption" />
                    <asp:ListItem Text="Attorney" />
                    <asp:ListItem Text="Child Support" />
                    <asp:ListItem Text="Civil" />
                    <asp:ListItem Text="Contempt" />
                    <asp:ListItem Text="Criminal" />
                    <asp:ListItem Text="Dissolution of Marriage" />
                    <asp:ListItem Text="Evictions" />
                    <asp:ListItem Text="Modification" />
                    <asp:ListItem Text="Paternity" />
                    <asp:ListItem Text="Probate/Guardianship" />
                    <asp:ListItem Text="Unlawful Detainer" />
                    <asp:ListItem Text="Small Claims" />
                    <asp:ListItem Text="Other Family" />
                    <asp:ListItem Text="Other" Value="O" />
                </asp:CheckBoxList>
                <asp:CustomValidator ID="valCaseType" runat="server" Display="Dynamic" ErrorMessage="Select at least one Case Type" CssClass="label label-danger"
                    ClientValidationFunction="ValidateCaseTypes" OnServerValidate="valCaseType_ServerValidate" />
                <div class="other" id="CaseType">
                    <asp:Label ID="lblOtherCaseType" runat="server" AssociatedControlID="txtCaseTypeOther">If Other Please Complete</asp:Label>
                    <asp:TextBox runat="server" CssClass="form-control form-control-sm" ID="txtCaseTypeOther" Text="Other" MaxLength="50" />
                    <asp:CustomValidator ID="valCaseTypeOther" ErrorMessage="Required when Other is selected above" CssClass="label label-danger" Display="Dynamic" ClientValidationFunction="ValidateCaseTypeOther" OnServerValidate="valCaseTypeOther_ServerValidate" runat="server" />
                </div>
            </fieldset>
            <fieldset class="fieldset-bordered d-block">
                <legend>Service Provided<em>*</em></legend>
                <asp:CheckBoxList ID="cblServicesProvided" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4" RepeatLayout="UnorderedList">
                    <asp:ListItem Text="CLE CD’s" Value="CLE-CD" />
                    <asp:ListItem Text="Explained Ct Procedures" Value="ExplainedCtProcedures" />
                    <asp:ListItem Text="e-Portal Sign Up" Value="ePortal" />
                    <asp:ListItem Text="Forms" Value="Forms" />
                    <asp:ListItem Text="Internal Service" Value="InternalService" />
                    <asp:ListItem Text="Law Library" Value="Library" />
                    <asp:ListItem Text="Legal Referrals" Value="Referral" />
                    <asp:ListItem Text="Notary" Value="Notary" />
                    <asp:ListItem Text="Online Research" Value="OnlineResearch" />
                    <asp:ListItem Text="Set Appointment" Value="SetAppointment" />
                    <asp:ListItem Text="Quiet Study" Value="QuietStudy" />
                    <asp:ListItem Text="Other" Value="O" />
                </asp:CheckBoxList>
                <asp:CustomValidator ID="valServiceProvided" runat="server" Display="Dynamic" ErrorMessage="Select at least one Value" CssClass="label label-danger"
                    ClientValidationFunction="ValidateServiceProvided" OnServerValidate="valServiceProvided_ServerValidate" />
                <div class="other" id="ServiceProvided">
                    <asp:Label ID="lblServiceProvidedOther" runat="server" AssociatedControlID="txtServiceProvidedOther">If Other Please Complete</asp:Label>
                    <asp:TextBox runat="server" CssClass="form-control form-control-sm" ID="txtServiceProvidedOther" Text="Other" MaxLength="50" />
                    <asp:CustomValidator ID="valOtherServiceProvided" ErrorMessage="Required when Other is selected above" CssClass="label label-danger" Display="Dynamic" ClientValidationFunction="ValidateOther" OnServerValidate="valOtherServiceProvided_ServerValidate" runat="server" />
                </div>
            </fieldset>
            <asp:HiddenField ID="hdClientId" runat="server" />
        </div>
    </div>
    <hr />
    <p>
        <asp:Button Text="Submit" ID="cmdSubmit" runat="server" CssClass="btn btn-primary me-2" OnClick="cmdSubmit_Click" />
        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-danger" Text="Cancel" />
    </p>
</asp:Panel>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            InitPage();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                InitPage();
            });
        });
    }(jQuery, window.Sys));
    function InitPage() {
        $(".other").hide();
        $(".datepicker").datepicker();
        $('.phone').mask('(000) 000-0000');
        $('#<%=cblServicesProvided.ClientID%> input:checked').each(function () {
            var item = $(this);
            if (item.val() == "O") {
                $("#ServiceProvided").fadeIn();
            }
        });
        $('#<%=cblCaseType.ClientID%> input:checked').each(function () {
            var item = $(this);
            if (item.val() == "O")
                $("#CaseType").fadeIn();
        });
        if ($('#<%=rblContactMethod.ClientID%> input:radio:checked').val() == "Other") {
            $("#ContactMethod").fadeIn();
        }

        $('#<%=cblServicesProvided.ClientID%> input:checkbox').click(function () {
            if ($(this).val() == "O") {
                if ($(this).is(':checked')) {
                    $('#<%=txtServiceProvidedOther.ClientID%>').val("");
                    $('#ServiceProvided').fadeIn();
                } else {
                    $('#ServiceProvided').fadeOut();
                    $('#<%=txtServiceProvidedOther.ClientID%>').val("");
                }
            }
        });
        $("#<%=cblCaseType.ClientID%> input:checkbox").click(function () {
            if ($(this).val() == "O") {
                if ($(this).is(':checked')) {
                    $('#<%=txtCaseTypeOther.ClientID%>').val("");
                    $('#CaseType').fadeIn();
                } else {
                    $('#CaseType').fadeOut();
                    $('#<%=txtCaseTypeOther.ClientID%>').val("");
                }
            }
        });
        $("#<%=rblContactMethod.ClientID%> input").change(function () {
            var selectedValue = $(this).val();
            if (selectedValue == "Other") {
                $("#ContactMethod").fadeIn();
                $("#<%=txtContactMethodOther.ClientID%>").val("");

            } else {
                $("#ContactMethod").fadeOut();
                $("#<%=txtContactMethodOther.ClientID%>").val("Other");
            }
        });
    }
    function ValidateOther(sender, args) {
        var radioButtons = $('#<%=cblServicesProvided.ClientID%>');
        var hasOtherChecked = false;
        var otherText = $('#<%=txtServiceProvidedOther.ClientID%>');

        radioButtons.find('input:checked').each(function () {
            if ($(this).val() == "O") {
                hasOtherChecked = true;
            }
        });
        if (hasOtherChecked) {
            if (otherText.val().length > 0) {
                args.IsValid = true;
            } else {
                args.IsValid = false;
            }
        } else {
            args.IsValid = true;
        }
    }
    function ValidateServiceProvided(source, args) {
        var chkListModules = document.getElementById('<%= cblServicesProvided.ClientID %>');
        var chkListinputs = chkListModules.getElementsByTagName("input");
        for (var i = 0; i < chkListinputs.length; i++) {
            if (chkListinputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
    function ValidateCaseTypeOther(sender, args) {
        var radioButtons = $('#<%=cblCaseType.ClientID%>');
        var hasOtherChecked = false;
        var otherText = $('#<%=txtCaseTypeOther.ClientID%>');

        radioButtons.find('input:checked').each(function () {
            if ($(this).val() == "O") {
                hasOtherChecked = true;
            }
        });
        if (hasOtherChecked) {
            if (otherText.val().length > 0) {
                args.IsValid = true;
            } else {
                args.IsValid = false;
            }
        } else {
            args.IsValid = true;
        }
    }
    function ValidateCaseTypes(source, args) {
        var cblCaseType = document.getElementById('<%= cblCaseType.ClientID %>');
        var cblCaseTypeInputs = cblCaseType.getElementsByTagName("input");
        for (var i = 0; i < cblCaseTypeInputs.length; i++) {
            if (cblCaseTypeInputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
</script>
