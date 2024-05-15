<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StampOrder.ascx.cs" Inherits="tjc.Modules.Purchasing.StampOrder" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="Manage Orders" CssClass="SubHead" runat="server" />
<div class="stamp-form-container">

    <div class="alert alert-info"><i class="fas fa-info-circle" aria-hidden="true"></i><strong>Note:</strong> The vendor needs at least <strong>7-10 working days</strong> to create a customized stamp. We have no way of RUSHING an order.  Thank you for your cooperation.</div>
    <div class="alert alert-warning"><i class="fas fa-warning" aria-hidden="true"></i><strong class="text-uppercase">Please use one form, per stamp request</strong></div>
    <div id="referral-form">
        <fieldset class="row g-3">
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name<em>*</em>" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor"
                    CssClass="label label-danger" ErrorMessage="Requester is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtPhone" Text="Phone<em>*</em>" />
                <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone"
                    CssClass="label label-danger" ErrorMessage="Phone is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmailAddress" Text="Email Address<em>*</em>" />
                <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="250" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailAddress"
                    CssClass="label label-danger" ErrorMessage="Email Address is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpLocation" Text="Delivery Locatio<em>*</em>n" />
                <asp:DropDownList ID="drpLocation" runat="server" CssClass="form-control">
                    <asp:ListItem Text="< Select Location >" Value=""></asp:ListItem>
                    <asp:ListItem Text="CJC"></asp:ListItem>
                    <asp:ListItem Text="DeSoto"></asp:ListItem>
                    <asp:ListItem Text="Manatee"></asp:ListItem>
                    <asp:ListItem Text="Sarasota"></asp:ListItem>
                    <asp:ListItem Text="Venice"></asp:ListItem>
                    <asp:ListItem Text="1751 Mound Street"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpLocation"
                    CssClass="label label-danger" ErrorMessage="Please Select a Delivery Location" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtConsumerName" Text="Who is the Stamp for?<em>*</em>" />
                <asp:TextBox ID="txtConsumerName" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConsumerName"
                    CssClass="label label-danger" ErrorMessage="Phone is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpStampType" Text="Type of Stamp<em>*</em>" />
                <asp:DropDownList ID="drpStampType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpStampType_SelectedIndexChanged" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Type >" Value=""></asp:ListItem>
                    <asp:ListItem Text="conforming"></asp:ListItem>
                    <asp:ListItem Text="signature"></asp:ListItem>
                    <asp:ListItem Text="other"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpStampType"
                    CssClass="label label-danger" ErrorMessage="Please Select a Type" />
            </div>
            <div id="signature-alert" style="display: none" class="alert alert-danger" aria-hidden="true">**For signature stamps, sign your name 3 times on a sheet of paper, in <strong>black ink only</strong> and send <a href="mailto:purchasing@jud12.flcourts.org">Linda</a> the original. Please interoffice the original signed paper to Linda Pluta.</div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtSample" Text="Provide Sample of Stamp<em>*</em>" />
                <asp:TextBox ID="txtSample" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="3" CssClass="sample form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSample"
                    CssClass="label label-danger" ErrorMessage="Sample is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpFontStyle" Text="Font Style<em>*</em>" />
                <asp:DropDownList ID="drpFontStyle" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Font Style >" Value=""></asp:ListItem>
                    <asp:ListItem Text="Arial" style="font-family: Arial; font-size: 2em"></asp:ListItem>
                    <asp:ListItem Text="Arial Narrow" style="font-family: Arial Narrow; font-size: 2em"></asp:ListItem>
                    <asp:ListItem Text="Calibri" style="font-family: Calibri; font-size: 2em"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" Visible="false" ID="valFontStyle" ControlToValidate="drpFontStyle"
                    CssClass="label label-danger" ErrorMessage="Font Style is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFontSize" Text="Font Size<em>*</em>" />
                <asp:TextBox ID="txtFontSize" runat="server" MaxLength="50" TextMode="Number" CssClass="" ClientIDMode="Static"></asp:TextBox>
                <span class="field-note"><i>(Numeric Point Size)</i></span>
                <asp:RequiredFieldValidator runat="server" Visible="false" ID="valFontSize" ControlToValidate="txtFontSize"
                    CssClass="label label-danger" ErrorMessage="Font Size is Required" />
            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpInkColor" Text="Ink Color<em>*</em>" />
                <asp:DropDownList ID="drpInkColor" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Ink Color >" Value=""></asp:ListItem>
                    <asp:ListItem Text="Black"></asp:ListItem>
                    <asp:ListItem Text="Blue"></asp:ListItem>
                    <asp:ListItem Text="Red"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" Visible="false" ID="valFontColor" ControlToValidate="drpInkColor"
                    CssClass="label label-danger" ErrorMessage="Ink Color is Required" />

            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity<em>*</em>" />
                <asp:TextBox ID="txtQuantity" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuantity"
                    CssClass="label label-danger" ErrorMessage="Quantity is Required" />

            </div>
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtInstructions" Text="Additional Information" />
                <asp:TextBox ID="txtInstructions" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="5"></asp:TextBox>

            </div>
            <div class="alert alert-info"><i class="fas fa-info-circle" aria-hidden="true"></i>Click select and browse for files to upload. Acceptable file types are .docx, .doc, .xls, .xlsx, .pdf, .jpg, .gif, .png</div>
            <div class="col-md-12 clearfix">
                <asp:Label ID="lblupload" runat="server" AssociatedControlID="attachmentUpload" Text="Upload Attachments" />
                <div style="position: relative;">
                    <div id="attach-overlay" class="overlay" style="display: none;">
                        <div class="spinner"></div>
                    </div>
                    <asp:FileUpload ID="uplAttachments" runat="server" onchange='check_extension(this.value);' ClientIDMode="Static" Enabled="false" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx,.tiff,.tif,.jpg,.jpeg" />
                    <asp:Button ID="cmdAddAttachment" ClientIDMode="Static" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
                    <span id="fileAttachmentWarning" class="label label-danger attachment-warning">Please Choose File to Upload</span>
                </div>
            </div>
            <div class="dnnFormItem">
                <div class="formFieldAdjust">
                    <span id="attachmentInfo"></span>
                    <ul id="attachmentList" class="attachments">
                    </ul>
                </div>
            </div>
        </fieldset>
        <p>
            <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </p>
    </div>
    <div class="alert alert-info"><i class="fas fa-info-circle" aria-hidden="true"></i>Once your order arrives, it will be delivered to you the same day in person or via interoffice.</div>
</div>
<script type="text/javascript">
    var extensionHash = {
        'pdf': 1,
        'tif': 1,
        'tiff': 1,
        'doc': 1,
        'docx': 1,
        'xls': 1,
        'xlsx': 1,
    };
    jQuery(function ($) {
        $("#uplAttachments").on("click", function (e) {
            $("#attach-overlay").show();
        });
        $('#modFormOrder').on('hidden.bs.modal', function (e) {
            $("#btnAddForm").show();
        });
        $('#modFormOrder').on('shown.bs.modal', function (e) {
            $("#btnAddForm").hide();
        });
        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        $('#drpStampType').change(function () {
            var stampType = $(this).val();
            if (stampType == 'signature') {
                $('#signature-alert').fadeIn();
            } else {
                $('#signature-alert').fadeOut();
            }
        });
        $('#drpInkColor').change(function () {
            UpdateSampleStyle();
        });
        $('#txtFontSize').blur(function () {
            UpdateSampleStyle();
        });
        $('#drpFontStyle').change(function () {
            UpdateSampleStyle();
        });
        $("#cmdAddAttachment").click(function (evt) {
            $("#attach-overlay").show();
            var upload = $("#uplAttachments");
            if (upload.is(':enabled')) {
                var fileUpload = $("#uplAttachments").get(0);
                var files = fileUpload.files;
                if (files.length == 0) {
                    $("#attachmentInfo").html("<span class='text-danger'>Please Choose a File!</span>");
                    return false;
                }
                var filename = files[0].name;
                var data = new FormData();
                data.append(filename, files[0]);
                data.append("moduleId",<%=ModuleId%>);
                data.append("tabId",<%=TabId%>);
                var options = {};
                options.url = "<%=attachmentHandler%>";
                options.type = "POST";
                options.data = data;
                options.contentType = false;
                options.processData = false;
                options.success = function (fileId) {
                    $("#attach-overlay").hide();
                    var fileIdList = $("#hdAttachmentIds").val();
                    if (fileIdList.length == 0) {
                        $("#hdAttachmentIds").val(fileId);
                    } else {
                        $("#hdAttachmentIds").val(fileIdList + "," + fileId);
                    }
                    $("#attachmentList").append("<li data-fileType='" + selectedAttachmentType + "' data-fileId='" + fileId + "'><span class='fileType'>" + selectedAttachmentType + "</span>&nbsp;<a onclick=\"DeleteAttachment('" + fileId + "')\"><em class='fa fa-trash'></em></></li>");
                    WriteAttachmentMessage(filename);
                };
                options.error = function (err) {
                    alert(err.statusText);
                    setTimeout(function () {
                        $("#attach-overlay").hide();
                        upload.parent().addClass("disabledWrapper");
                        //upload.parent().text("Choose File");
                        upload.attr("disabled", true);
                        upload.attr("title", "Select Attachment Type First");
                        $("#attachmentInfo").html('');
                    }, 1000);
                };
                $.ajax(options);
                evt.preventDefault();
            }
        });
    });
    function UpdateSampleStyle() {
        var fontStyle = $('#drpFontStyle').val();
        var fontSize = $('#txtFontSize').val();
        var fontColor = $('#drpInkColor').val();
        var fontHexColor = '#000';
        if (fontColor == 'Blue') {
            fontHexColor = '#0000FF';
        }
        if (fontColor == 'Red') {
            fontHexColor = '#FF0000';
        }
        if (fontStyle != "") {
            $('.sample').css("font-family", fontStyle);
        }
        if (fontSize != "") {
            $('.sample').css("font-size", fontSize + 'pt');
        }
        if (fontColor != "") {
            $('.sample').css("color", fontHexColor)
        }
    }
    function WriteAttachmentMessage(filename) {
        if (filename == "") {
            $("#attachmentInfo").html("<span class='text-danger'>Unable to upload file. Please make sure the file is in an allowed format.</span>");
        } else {
            $("#fileAttachmentWarning").fadeOut();
            $("#attachmentInfo").html("<span class='text-danger'>Attachment Captured.</span>");
        }
        var upload = $("#uplAttachments");
        upload.parent().addClass("disabledWrapper");
        var html = upload.parent().html();
        upload.parent().html(html.replace(filename, "Choose File"));
        //upload.parent().text("Choose File");
        upload.attr("disabled", true);
        upload.attr("title", "Select Attachment Type First");
        $("#attachmentInfo").html('');
    }

    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        if (extensionHash[ext]) {
            $("#attachmentInfo").html("");
            $("#cmdAddForm").trigger("click");
            $("#cmdAddForm").prop("disabled", false);
            return true;
        } else {
            $("#attachmentInfo").html("<span class='text-danger'>Invalid File Type, please choose an allowed file type!</span>");
            $("#cmdAddForm").prop("disabled", true);
            return false;
        }
    }</script>
