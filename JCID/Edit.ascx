<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="tjc.Modules.JCID.Edit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal Visible="false" ID="ltReplacementMessage" runat="server"><div class="alert alert-warning display-5"><i class="fa fa-warning" aria-hidden="true"></i> This is a Replacement Card Request</div></asp:Literal>
<div type="post" id="dvForm" runat="server">
    <div class="mb-3">
        <div class="row">
            <div class="col-md-12">
                <asp:Label ID="lblPhotoType" runat="server" AssociatedControlID="rblPhotoType" Text="Please Select One<em>*</em>" />
                <asp:RadioButtonList ID="rblPhotoType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Text="Upload Recent Photo*" Value="Uploaded Recent Photo" />
                    <asp:ListItem Text="New Photo taken in Manatee County Judicial Center**" Value="Taken in Manatee" />
                    <asp:ListItem Text="New Photo taken in Sarasota Judicial Center" Value="Taken in Sarasota" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="valPhotoType" runat="server" SetFocusOnError="true"
                    Display="Dynamic" ErrorMessage="Photo Option Required" CssClass="label label-danger"
                    ControlToValidate="rblPhotoType" />

            </div>
        </div>

    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-6">
                <asp:Label ID="lblPhotoTaken" runat="server" AssociatedControlID="rblPhotoTaken" Text="Where would you like to pick up your JCID card? <em>*</em>" />
                <asp:RadioButtonList ID="rblPhotoTaken" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Text="Manatee County Judicial Center" />
                    <asp:ListItem Text="Silvertooth Judicial Center " />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="valPhotoTaken" runat="server" SetFocusOnError="true"
                    Display="Dynamic" ErrorMessage="Pickup Location Requiredd" CssClass="label label-danger"
                    ControlToValidate="rblPhotoTaken" />
            </div>
            <div class="col-md-6">
                <asp:Label ID="lblPhoto" runat="server" AssociatedControlID="imgPhoto" Text="Photo Uploaded (click photo to view)" />
                <div>

                    <asp:Image ID="imgPhoto" runat="server" AlternateText="ID photo" CssClass="image-btn" data-bs-toggle="modal" data-bs-target="#imageModal" />
                </div>

            </div>
        </div>
    </div>
    <div class="modal fade" id="imageModal" tabindex="-1" role="dialog" aria-labelledby="imageModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="imageModalLabel">Attorney Photo</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" tabindex="-1"></button>
                </div>

                <div class="modal-body">
                    <asp:Image CssClass="img-responsive image-zoom" ID="imgZoom" runat="server" AlternateText="ID photo, enlarged" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-image" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <fieldset>
        <legend>Workflow Completion Dates</legend>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-3">
                    <asp:Label ID="lblCriminalHistory" runat="server" AssociatedControlID="txtCriminalHistoryCompleted" Text="Criminal History Run" />
                    <asp:TextBox ID="txtCriminalHistoryCompleted" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:CompareValidator ID="valCriminalHistoryCompleted" runat="server" ControlToValidate="txtCriminalHistoryCompleted" CssClass="label label-danger"
                        Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Date is not in Correct Format i.e.(mm/dd/yyyy)" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblCardCreated" runat="server" AssociatedControlID="txtCardCreated" Text="JCID Card Created" />
                    <asp:TextBox ID="txtCardCreated" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:CompareValidator ID="valCardCreated" runat="server" ControlToValidate="txtCardCreated" CssClass="label label-danger"
                        Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Date is not in Correct Format i.e.(mm/dd/yyyy)" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblApplicantNotified" runat="server" AssociatedControlID="txtApplicantNotified" Text="Applicant Notified ID Card is Available" />
                    <asp:TextBox ID="txtApplicantNotified" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:CompareValidator ID="valApplicantNotified" runat="server" ControlToValidate="txtApplicantNotified" CssClass="label label-danger"
                        Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Date is not in Correct Format i.e.(mm/dd/yyyy)" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblPickedUp" runat="server" AssociatedControlID="txtPickedUp" Text="ID Card Picked Up" />
                    <asp:TextBox ID="txtPickedUp" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:CompareValidator ID="valPickedUp" runat="server" ControlToValidate="txtPickedUp" CssClass="label label-danger"
                        Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Date is not in Correct Format i.e.(mm/dd/yyyy)" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Requestor Information</legend>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-4">
                    <asp:Label ID="lblFirstName" runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                    <asp:TextBox ID="txtFirstName" runat="server" MaxLength="100" CssClass="form-control" AutoCompleteType="FirstName" />
                </div>
                <div class="col-md-4">
                    <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="100" CssClass="form-control" AutoCompleteType="LastName" />
                </div>
                <div class="col-md-4">
                    <asp:Label ID="lblBarNumber" runat="server" AssociatedControlID="txtBarNumber" Text="State Bar Number<em>*</em>" />
                    <asp:TextBox ID="txtBarNumber" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valBarNumber" runat="server" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger"
                        ErrorMessage="Bar Number is Required" ControlToValidate="txtBarNumber" />
                </div>
            </div>
        </div>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblAddress" runat="server" AssociatedControlID="txtAddress" Text="Mailing Address<em>*</em>" />
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="250" CssClass="form-control" AutoCompleteType="BusinessStreetAddress" />
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
        <div class="mb-3">
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
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblPhone" runat="server" AssociatedControlID="txtPhone" Text="Business Telephone<em>*</em>" />
                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" CssClass="form-control phone_us" placeholder="(555) 555-5555" AutoCompleteType="BusinessPhone" />
                    <asp:RequiredFieldValidator ID="valPhone" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Phone is Required" ControlToValidate="txtPhone" CssClass="label label-danger" />
                    <asp:RegularExpressionValidator ID="valIsPhone" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Phone is not in Correct Format i.e. (555) 555-5555." CssClass="label label-danger"
                        ControlToValidate="txtPhone" ValidationExpression="((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}"></asp:RegularExpressionValidator>

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
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:CheckBox ID="chkWaiver" CssClass="checkbox" runat="server" Text="Government Attorney Waiver?" />
                </div>
            </div>
        </div>


    </fieldset>
    <fieldset>
        <legend>Required for Background Check</legend>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblDOB" runat="server" AssociatedControlID="txtDOB" Text="Date of Birth<em>*</em>" />
                    <asp:TextBox ID="txtDOB" runat="server" MaxLength="20" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valDOB" runat="server" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Date of Birth is Required" ControlToValidate="txtDOB" CssClass="label label-danger" />
                    <asp:CompareValidator ID="valIsDOB" runat="server" ControlToValidate="txtDOB" CssClass="label label-danger"
                        Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Date is not in Correct Format i.e.(mm/dd/yyyy)" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblSNN" runat="server" AssociatedControlID="txtSSN" Text="<abbr title='Social Security Number'>SSN</abbr>" />
                    <asp:TextBox ID="txtSSN" runat="server" MaxLength="250" CssClass="form-control" placeholder="000-00-0000" />
                    <asp:RegularExpressionValidator ID="valIsSSN" runat="server" SetFocusOnError="true" ControlToValidate="txtSSN"
                        ValidationExpression="^(?!219-09-9999|078-05-1120)(?!666|000|9\d{2})\d{3}-(?!00)\d{2}-(?!0{4})\d{4}$" Display="Dynamic" CssClass="label label-danger" ErrorMessage="Invalid format '000-00-0000'" />
                </div>
            </div>
        </div>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblRace" runat="server" AssociatedControlID="drpRace" Text="Race<em>*</em>" />
                    <asp:DropDownList ID="drpRace" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="< Select Race >" Value=""></asp:ListItem>
                        <asp:ListItem Text="African American Not Hispanic" />
                        <asp:ListItem Text="Asian / Pacific Islander" />
                        <asp:ListItem Text="Hispanic" />
                        <asp:ListItem Text="Native American / Alaskan" />
                        <asp:ListItem Text="White" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="valRace" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Race is Required"
                        ControlToValidate="drpRace" CssClass="label label-danger" />
                </div>
                <div class="col-md-6">
                    <asp:Label ID="lblSex" runat="server" AssociatedControlID="rblSex" Text="Gender<em>*</em>" />
                    <asp:RadioButtonList ID="rblSex" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Text="Male" />
                        <asp:ListItem Text="Female" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valSex" runat="server" SetFocusOnError="true"
                        Display="Dynamic" ErrorMessage="Gender is Required"
                        ControlToValidate="rblSex" CssClass="label label-danger" />
                </div>
            </div>
        </div>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblSign" runat="server" AssociatedControlID="txtSign" Text="Signature" />
                    <asp:TextBox ID="txtSign" ReadOnly="true" runat="server" MaxLength="150" CssClass="form-control" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Reject Application</legend>
        <div class="mb-3">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblRejectionReason" runat="server" AssociatedControlID="txtRejectReason" Text="Rejection Reason" />
                    <asp:TextBox ID="txtRejectReason" runat="server" TextMode="MultiLine" MaxLength="2000" CssClass="form-control" />
                </div>
            </div>
        </div>
    </fieldset>
    <hr />
    <asp:Literal runat="server" Visible="false" ID="ltMessage" />
    <div class="row">
        <div class="col-md-12">
            <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary btn-lg" Text="Save" data-loading-text="Loading..." />
            <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary btn-lg"
                Text="Cancel" />
            <asp:Button ID="cmdReject" runat="server" OnClick="cmdReject_Click" CssClass="btn btn-danger btn-lg ms-5" Text="Reject" />

        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/jQuery/jquery.mask.js" />

<script type="text/javascript">
    (function ($, Sys) {
        $('.phone_us').mask('(000) 000-0000');

        // Apply SSN mask by element ID rather than CSS class.
        // The mask plugin clears a pre-populated value that doesn't already match
        // the pattern at init time, so we capture the server-rendered value first,
        // apply the mask, then restore it so the field shows the decrypted SSN.
        var $ssn = $('#<%=txtSSN.ClientID%>');
        var existingSSN = $ssn.val();
        $ssn.mask('000-00-0000');
        if (existingSSN) {
            $ssn.val(existingSSN);
        }

        $("#<%=txtDOB.ClientID%>").datepicker();
        $("#<%=txtApplicantNotified.ClientID%>").datepicker();
        $("#<%=txtCardCreated.ClientID%>").datepicker();
        $("#<%=txtCriminalHistoryCompleted.ClientID%>").datepicker();
        $("#<%=txtPickedUp.ClientID%>").datepicker();

    }(jQuery, window.Sys));
    var extensionHash = {
        '.png': 1,
        '.jpg': 1,
        '.jpeg': 1,
    };

    function check_extension(filename) {
        var re = /\..+$/;
        var ext = filename.match(re);
        if (extensionHash[ext]) {
            $(".info").html("");
            return true;
        } else {
            $(".info").html("<span class='text-danger'>Invalid File Type, please choose a valid image type (png, jpg, or jpeg extension)!</span>");
            return false;
        }
    }
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
