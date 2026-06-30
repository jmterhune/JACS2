<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicAudioRequest.ascx.cs" Inherits="tjc.Modules.AudioRequest.PublicAudioRequest" %>
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
        must be filled in or this form will not be processed.
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
                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" CssClass="form-control phone_us" placeholder="(555) 555-5555" AutoCompleteType="BusinessPhone" />
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
                    <asp:Label ID="lblFirm" runat="server" AssociatedControlID="txtFirm" Text="Firm Name" />
                    <asp:TextBox ID="txtFirm" runat="server" MaxLength="50" CssClass="form-control" AutoCompleteType="Company" />

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
                    <asp:Label ID="lblAddress" runat="server" AssociatedControlID="txtAddress" Text="Mailing Address<em>*</em>" />
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="100" CssClass="form-control" AutoCompleteType="BusinessStreetAddress" />
                    <asp:RequiredFieldValidator ID="valAddress" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Mailing Address is Required" CssClass="label label-danger"
                        ControlToValidate="txtAddress" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City<em>*</em>" />
                    <asp:TextBox ID="txtCity" runat="server" MaxLength="50" CssClass="form-control" AutoCompleteType="BusinessCity" />
                    <asp:RequiredFieldValidator ID="valCity" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="City is Required" ControlToValidate="txtCity" CssClass="label label-danger" />

                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblState" runat="server" AssociatedControlID="txtState" Text="State Code<em>*</em>" />
                    <asp:TextBox ID="txtState" runat="server" MaxLength="2" CssClass="form-control" Text="FL" AutoCompleteType="BusinessState" />
                    <asp:RequiredFieldValidator ID="valState" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="State is Required" ControlToValidate="txtState" CssClass="label label-danger" />
                </div>
                <div class="col-md-6">
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
        <legend>CD Information</legend>
        <div class="row">
            <div class="form-group">
                <div class="col-md-12">
                    <p>
                        <strong>Choice of CD (choose one): Fee is charged Per Proceeding
                    Per Day</strong>, and is further defined as: (1) Court proceedings involving group
                hearings on multiple individuals or (2) Court proceedings for each individual involved
                in separate hearings. Additional fees are charged for proceedings on multiple days.
                    </p>
                    <asp:RadioButtonList ID="rblCDInfo" runat="server" RepeatLayout="Flow" CssClass="dnnFormRadioButtons">
                        <asp:ListItem Value="PC">
                 <strong>PC VERSION CD -Fee $15.00 per proceeding*/$18.00 if mailing requested for request filed in
                    Sarasota and Manatee only</strong><br />PC Version CD is self-executing on personal computers 
                    and may be used to review an audio recording of the proceeding. This version includes the annotations 
                    (notes) of the Digital Court Reporter such as case name, case number, and events in the proceeding such 
                    as when the testimony of witnesses, direct examination, cross examination etc occurs. The PC Version 
                    allows the user to select all or specific microphone channels for review of the audio of individual 
                    speakers. The user may fast forward or rewind the CD in increments of 10 seconds, 30 seconds, 
                    5 minutes, or 30 minutes, as well as utilize a foot pedal for transcription purposes.
                <br /><br />The PC Version CD may be used by litigants in the courtroom for evidence production
                and is the only version available for use by a transcriptionist if a written transcript
                is desired or preferred.<br />
                        </asp:ListItem>
                        <asp:ListItem Value="Audio">
            <strong>AUDIO VERSION CD - Fee $25.00 per proceeding*/$28.00 if mailing requested for request filed in
                    Sarasota and Manatee only
                </strong><br />The Audio Version CD is suggested ONLY if the requestor does not have a computer
                available to play the PC Version CD listed above. Audio Version CD&#39;s are limited
                to use for audio listening only and cannot be used for transcription purposes. Microphone
                channels cannot be isolated for review audio of individual speakers and the playback
                of the Audio Version CD does not provide for incremental advancement or rewind of
                audio.
                        </asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valCDInfo" runat="server" ControlToValidate="rblCDInfo" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="CD Type is Required" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Proceeding Information</legend>
        <div class="alert alert-info">
            <strong><em class="fa fa-exclamation-circle"></em>Important:</strong> If Juvenile proceeding or adoption or TPR heard by a magistrate, please fax
                a copy of the Court Order.
                <br />
            Manatee (941) 749-3692 &#8226; Sarasota (941) 861-7924
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
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblCounty" runat="server" AssociatedControlID="rblCounty" Text="Jurisdiction<em>*</em>" />
                    <asp:RadioButtonList ID="rblCounty" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons">
                        <asp:ListItem Text="Manatee" />
                        <asp:ListItem Text="Sarasota" />
                        <asp:ListItem Text="Desoto" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valCounty" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Jurisdiction is Required" CssClass="label label-danger"
                        ControlToValidate="rblCounty" />

                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblLocation" runat="server" AssociatedControlID="rblLocation" Text="Location of Proceeding<em>*</em>" />
                    <asp:RadioButtonList ID="rblLocation" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="Desoto" />
                        <asp:ListItem Text="Manatee" />
                        <asp:ListItem Text="Sarasota" />
                        <asp:ListItem Text="Venice" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valLocation" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Location of Proceeding is Required"
                        ControlToValidate="rblLocation" CssClass="label label-danger" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblCaseName" runat="server" AssociatedControlID="txtCaseName" Text="Case Name<em>*</em>" />
                    <asp:TextBox ID="txtCaseName" runat="server" MaxLength="250" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valCaseName" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Case Name is Required" ControlToValidate="txtCaseName" CssClass="label label-danger" />
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
                    <asp:Label ID="lblDates" runat="server" AssociatedControlID="txtDates" Text="Date(s) of Proceeding<em>*</em>" />
                    <asp:TextBox ID="txtDates" runat="server" MaxLength="50" CssClass="form-control" placeholder="MM/DD/YY, MM/DD/YY, MM/DD/YY" />
                    <asp:RequiredFieldValidator ID="valDates" runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Date(s) of Proceeding is Required" ControlToValidate="txtDates" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblTime" runat="server" AssociatedControlID="txtTime" Text="Time of Proceeding" />
                    <asp:TextBox ID="txtTime" runat="server" MaxLength="100" CssClass="form-control" placeholder="HH:MM AM" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblType" runat="server" AssociatedControlID="cklType" Text="Type of Proceeding<em>*</em> <span class='italic'>(multiple selection)</span> " />
                    <asp:CheckBoxList ID="cklType" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="list-unstyled dnnFormRadioButtons">
                        <asp:ListItem Text="Adoption" />
                        <asp:ListItem Text="Baker Act" />
                        <asp:ListItem Text="Civil" />
                        <asp:ListItem Text="Circuit Criminal" />
                        <asp:ListItem Text="County Criminal" />
                        <asp:ListItem Text="Child Support-IV-D Hearing Officer" />
                        <asp:ListItem Text="Circuit Civil-Repeat Violence (Manatee)" />
                        <asp:ListItem Text="Family - Domestic Violence" />
                        <asp:ListItem Text="First Appearances" />
                        <asp:ListItem Text="Guardianship Hearing" />
                        <asp:ListItem Text="Magistrate Civil Hearing" />
                        <asp:ListItem Text="Magistrate Foreclosure" />
                        <asp:ListItem Text="Juvenile" />
                        <asp:ListItem Text="Magistrate Family" />
                        <asp:ListItem Text="TPR Hearing" />
                    </asp:CheckBoxList>
                    <asp:CustomValidator ID="valType" ErrorMessage="Please select at least one Proceeding Type." ClientValidationFunction="ValidateCheckBoxList"
                        runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblInvolvement" runat="server" AssociatedControlID="rblInvolvement" Text="Involvement in Case<em>*</em>" />
                    <asp:RadioButtonList ID="rblInvolvement" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="list-unstyled dnnFormRadioButtons">
                        <asp:ListItem Text="Counsel for Plaintiff/Petitioner" />
                        <asp:ListItem Text="Counsel for Defendant/Respondent" />
                        <asp:ListItem Text="Court Appointed Counsel" />
                        <asp:ListItem Text="Counsel for Juvenile" />
                        <asp:ListItem Text="Plaintiff/Petitioner" />
                        <asp:ListItem Text="Defendant/Respondent" />
                        <asp:ListItem Text="Parent of Juvenile (Copy of I.D. attached)" />
                        <asp:ListItem Text="Other (fill in field below)" Value="Other" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valInvolvement" runat="server"
                        ControlToValidate="rblInvolvement" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Involvement in Case is Required" />

                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblOther" runat="server" AssociatedControlID="txtOther" Text="Other Involvement" />
                    <asp:TextBox ID="txtOther" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Miscellaneous Instructions</legend>

        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblDelivery" runat="server" AssociatedControlID="rblDelivery" Text="Location of Proceeding<em>*</em>" />
                    <asp:RadioButtonList ID="rblDelivery" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="Will pick up in Court Administration" Value="1" />
                        <asp:ListItem Text="Please mail to address provided above" Value="0" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valDelivery" runat="server" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Delivery Method is Required"
                        ControlToValidate="rblDelivery" CssClass="label label-danger" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblPickup" runat="server" AssociatedControlID="rblPickup" Text="If Pickup Where?" />
                    <asp:RadioButtonList ID="rblPickup" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="Desoto" />
                        <asp:ListItem Text="Manatee" />
                        <asp:ListItem Text="Sarasota" />
                        <asp:ListItem Text="Venice" />
                    </asp:RadioButtonList>
                    <asp:CustomValidator ID="valPickup" runat="server" Display="Dynamic" CssClass="label label-danger" ErrorMessage="Pickup at Court Administration is Selected. Please Select the Pickup Location"
                        ControlToValidate="rblDelivery" ClientValidationFunction="ValidateDeliveryLocation" SetFocusOnError="true" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblTranscription" runat="server" AssociatedControlID="rblTranscription" Text="Transcription List" />
                    <asp:RadioButtonList ID="rblTranscription" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="Yes" />
                        <asp:ListItem Text="No" Selected="True" />
                    </asp:RadioButtonList>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">

                    <asp:Label ID="lblComment" runat="server" AssociatedControlID="txtComment" Text="Special Instructions" />
                    <asp:TextBox ID="txtComment" CssClass="form-control" runat="server" MaxLength="750" Rows="3" TextMode="MultiLine" />

                </div>
            </div>
        </div>
    </fieldset>
        <div class="alert alert-default">
            <h4>Upon submission, this form will be converted to an <a href="http://www.adobe.com/products/acrobat/readstep2.html"
                title="Opens in new window">Adobe Acrobat PDF form</a>

            </h4>
            <strong>Please print that form, sign it and return with payment to:
            </strong>
            <p>
                Digital Court Recording Office<br />
                2002 Ringling Blvd, Fourth Floor<br />
                Sarasota, FL 34237
            </p>
            <p>
                OR
            </p>
            <p>
                Court Administration<br />
                1051 Manatee Avenue West, Eighth Floor<br />
                Bradenton, FL 34205
            </p>

        </div>
        <asp:Literal runat="server" Visible="false" ID="ltMessage" />
        <asp:HiddenField ID="hdreCaptcha" runat="server" />
        <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary btn-lg" Text="Submit" data-loading-text="Loading..." />
        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary btn-lg"
            Text="Cancel" />
  

</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.13/jquery.mask.min.js" />

<script type="text/javascript">
    (function ($, Sys) {
        $('.phone_us').mask('(000) 000-0000');
        $("#<%=txtTime.ClientID%>").timepicker({ 'typeaheadHighlight': true, 'timeFormat': 'h:i A', 'minTime': '06:00 AM', 'maxTime': '06:00 PM' });
    }(jQuery, window.Sys));

    grecaptcha.ready(function () {
        document.getElementById('Form').addEventListener("submit", function (event) {
            event.preventDefault();
            if (Page_IsValid) {
                grecaptcha.execute('<%=ClientKey%>', { action: 'audiorequest' }).then(function (token) {
                    document.getElementById("<%=hdreCaptcha.ClientID%>").value = token;
                <%= Page.ClientScript.GetPostBackEventReference(cmdSubmit, String.Empty) %>;
                });
            }
        }, false);
    });


    function ValidateDeliveryLocation(sender, args) {
        var isValid = true;
        var deliveryType = $("#<%=rblDelivery.ClientID%>").find(":checked").val();
        var checkLength = $("#<%=rblPickup.ClientID%>").find(":checked").length;
        if (deliveryType == "1" && checkLength == 0) {
            isValid = false;
        }
        args.IsValid = isValid;
    }
    function ValidateCheckBoxList(sender, args) {
        var checkBoxList = document.getElementById("<%=cklType.ClientID %>");
        var checkboxes = checkBoxList.getElementsByTagName("input");
        var isValid = false;
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                isValid = true;
                break;
            }
        }
        args.IsValid = isValid;
    }
    function DisableButton() {
        document.getElementById("<%=cmdSubmit.ClientID %>").disabled = true;
        document.getElementById("<%=cmdSubmit.ClientID %>").value = "Processing...";
        setTimeout(() => {
            document.getElementById("<%=cmdSubmit.ClientID %>").disabled = false;
            document.getElementById("<%=cmdSubmit.ClientID %>").value = "Submit";

        }, "3000");
    }
    window.onbeforeunload = DisableButton;
</script>
