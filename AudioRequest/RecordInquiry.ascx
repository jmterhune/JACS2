<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordInquiry.ascx.cs" Inherits="tjc.Modules.AudioRequest.RecordInquiry" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div type="post">
    <fieldset>
        <legend>Requestor Information</legend>
        <div class="alert alert-warning">
            <strong><em class="fa fa-warning"></em></strong>All fields marked with an asterisk (<em class="lbl">*</em>) are required and
        must be filled in or this form may not be processed.
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblReqName" runat="server" AssociatedControlID="txtReqName" Text="Requestor Name<em>*</em>" />
                    <asp:TextBox ID="txtReqName" runat="server" MaxLength="50" CssClass="form-control" AutoCompleteType="DisplayName" />
                    <asp:RequiredFieldValidator ID="valReqName" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Requestor Name is Required" ControlToValidate="txtReqName" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblPhone" runat="server" AssociatedControlID="txtPhone" Text="Phone<em>*</em>" />
                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="15" CssClass="form-control phone_us" placeholder="(555) 555-5555" AutoCompleteType="BusinessPhone" />
                    <asp:RequiredFieldValidator ID="valPhone" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Phone is Required" ControlToValidate="txtPhone" CssClass="label label-danger" />
                    <asp:RegularExpressionValidator ID="valIsPhone" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Phone is not in Correct Format i.e. (555) 555-5555." CssClass="label label-danger"
                        ControlToValidate="txtPhone" ValidationExpression="((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}"></asp:RegularExpressionValidator>

                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblReqAddress" runat="server" AssociatedControlID="txtReqAddress" Text="Mailing Address<em>*</em>" />
                    <asp:TextBox ID="txtReqAddress" runat="server" MaxLength="100" CssClass="form-control" AutoCompleteType="BusinessStreetAddress" />
                    <asp:RequiredFieldValidator ID="valAddress" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Mailing Address is Required" CssClass="label label-danger"
                        ControlToValidate="txtReqAddress" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEMail" Text="Email Address<em>*</em>" />
                    <asp:TextBox ID="txtEMail" runat="server" MaxLength="150" CssClass="form-control" AutoCompleteType="Email" />
                    <asp:RequiredFieldValidator ID="valEmailReq" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Email Address is Required"
                        ControlToValidate="txtEMail" CssClass="label label-danger" />
                    <asp:RegularExpressionValidator ID="valEmail" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="The Value Entered is not a Valid Email Address"
                        ControlToValidate="txtEMail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="label label-danger" />

                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City<em>*</em>" />
                    <asp:TextBox ID="txtCity" runat="server" MaxLength="50" CssClass="form-control" AutoCompleteType="BusinessCity" />
                    <asp:RequiredFieldValidator ID="valCity" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="City is Required" ControlToValidate="txtCity" CssClass="label label-danger" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblState" runat="server" AssociatedControlID="txtState" Text="State Code<em>*</em>" />
                    <asp:TextBox ID="txtState" runat="server" MaxLength="2" CssClass="form-control" Text="FL" AutoCompleteType="BusinessState" />
                    <asp:RequiredFieldValidator ID="valState" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="State is Required" ControlToValidate="txtState" CssClass="label label-danger" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblZip" runat="server" AssociatedControlID="txtZip" Text="Zip<em>*</em>" />
                    <asp:TextBox ID="txtZip" runat="server" MaxLength="11" CssClass="form-control" AutoCompleteType="BusinessZipCode" />
                    <asp:RequiredFieldValidator ID="valZip" runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Zip Code is Required" ControlToValidate="txtZip" />
                    <asp:RegularExpressionValidator ID="valIsZip" runat="server" ControlToValidate="txtzip"
                        SetFocusOnError="true" Display="Dynamic" ErrorMessage="Zip Code is Invalid" CssClass="label label-danger"
                        ValidationExpression="\d{5}(-\d{4})?"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Proceeding Information</legend>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblDefendant" runat="server" AssociatedControlID="txtDefendant" Text="Defendant's Name<em>*</em>" />
                    <asp:TextBox ID="txtDefendant" runat="server" MaxLength="50" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valDefendant" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Defendant's Name is Required"
                        ControlToValidate="txtDefendant" CssClass="label label-danger" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblCaseNumber" runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number<em>*</em>" />
                    <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="20" CssClass="form-control" placeholder="One per request!" />
                    <asp:RequiredFieldValidator ID="valCaseNumber" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Case Number is Required" ControlToValidate="txtCaseNumber" CssClass="label label-danger" />
                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblJudge" runat="server" AssociatedControlID="txtJudge" Text="Presiding Judge/Magistrate<em>*</em>" />
                    <asp:TextBox ID="txtJudge" runat="server" MaxLength="50" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valJudge" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Presiding Judge/Magistrate is Required"
                        ControlToValidate="txtJudge" CssClass="label label-danger" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblCounty" runat="server" AssociatedControlID="rblCounty" Text="Location<em>*</em>" />
                    <asp:RadioButtonList ID="rblCounty" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons">
                        <asp:ListItem Text="Manatee" />
                        <asp:ListItem Text="Sarasota" />
                        <asp:ListItem Text="Desoto" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valCounty" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Jurisdiction is Required" CssClass="label label-danger"
                        ControlToValidate="rblCounty" />

                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblDates" runat="server" AssociatedControlID="txtDates" Text="Date(s) of Proceeding<em>*</em>" />
                    <asp:TextBox ID="txtDates" runat="server" MaxLength="50" CssClass="form-control" placeholder="MM/DD/YY, MM/DD/YY, MM/DD/YY" />
                    <asp:RequiredFieldValidator ID="valDates" runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Date(s) of Proceeding is Required" ControlToValidate="txtDates" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblTime" runat="server" AssociatedControlID="txtTime" Text="Time of Proceeding" />
                    <asp:TextBox ID="txtTime" runat="server" MaxLength="50" CssClass="form-control" placeholder="HH:MM AM" />
                    <%--                    <asp:RequiredFieldValidator ID="valTime" runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Time of Proceeding is Required" ControlToValidate="txtTime" />
                    <asp:RegularExpressionValidator ID="valTimeFormat" runat="server" ControlToValidate="txtTime"
                        SetFocusOnError="true" Display="Dynamic" ErrorMessage="Time is not in the Correct Format i.e. HH:MM AM" CssClass="label label-danger"
                        ValidationExpression="^(^([0-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])(\s{0,1})([AM|PM|am|pm]{2,2})$)|(^([0-9]|[1][0-9]|[2][0-3])(\s{0,1})([AM|PM|am|pm]{2,2})$) "></asp:RegularExpressionValidator>--%>
                </div>
            </div>
        </div>
    </fieldset>
    <div class="row">
        <div class="col-md-12">
            <div class="alert alert-default">
                <strong>Please Note: </strong>Upon submission, this form will be sent to Digital Recording Group. Print this form for your records and then hit SUBMIT.
            </div>
            <asp:Literal runat="server" Visible="false" ID="ltMessage" />
            <asp:HiddenField ID="hdreCaptcha" runat="server" />
            <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary btn-lg" Text="Submit" data-loading-text="Loading..." />
            <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary btn-lg"
                Text="Cancel" />
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.13/jquery.mask.min.js" />

<script type="text/javascript">
    (function ($, Sys) {
        $("#<%=txtTime.ClientID%>").timepicker({ 'typeaheadHighlight': true, 'timeFormat': 'h:i A', 'minTime': '06:00 AM', 'maxTime': '06:00 PM' });
        $('.phone_us').mask('(000) 000-0000');
    }(jQuery, window.Sys));

    grecaptcha.ready(function () {
        document.getElementById('Form').addEventListener("submit", function (event) {
            event.preventDefault();
            if (Page_IsValid) {
                grecaptcha.execute('<%=ClientKey%>', { action: 'inquiry' }).then(function (token) {
                    document.getElementById("<%=hdreCaptcha.ClientID%>").value = token;
                <%= Page.ClientScript.GetPostBackEventReference(cmdSubmit, String.Empty) %>;
                });
            }
        }, false);
    });
    function DisableButton() {
        document.getElementById("<%=cmdSubmit.ClientID %>").disabled = true;
        document.getElementById("<%=cmdSubmit.ClientID %>").value = "Processing...";
        setTimeout(() => {
            document.getElementById("<%=cmdSubmit.ClientID %>").disabled = false;
            document.getElementById("<%=cmdSubmit.ClientID %>").value = "Submit";

        }, "1000");
    }
    window.onbeforeunload = DisableButton;
</script>
