<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StampOrder.ascx.cs" Inherits="tjc.Modules.Purchasing.StampOrder" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="<i class='fas fa-list-alt'></i> Manage Orders" CssClass="btn btn-danger btn-large mb-4" runat="server" />
<div class="stamp-form-container purchasing">
    <asp:Literal runat="server" ID="ltTopMessages">
    <div class="alert alert-info">
        <i class="fas fa-info-circle" aria-hidden="true"></i><strong>Note:</strong> The vendor needs at least <strong>7-10 working days</strong> to create a customized stamp. We have no way of RUSHING an order.  Thank you for your cooperation.</div>
    <div class="alert alert-warning">
        <i class="fas fa-warning" aria-hidden="true"></i><strong class="text-uppercase">Please use one form, per stamp request</strong></div>

    </asp:Literal>
    <div id="stamp-form">
        <fieldset class="mb-3">
            <div class="row">
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name<em>*</em>" />
                    <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor"
                        CssClass="label label-danger" ErrorMessage="Requester is Required" />
                </div>
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtPhone" Text="Phone<em>*</em>" />
                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone"
                        CssClass="label label-danger" ErrorMessage="Phone is Required" />
                </div>
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmailAddress" Text="Email Address<em>*</em>" />
                    <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="250" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailAddress"
                        CssClass="label label-danger" ErrorMessage="Email Address is Required" />
                    <asp:RegularExpressionValidator ID="valEmail" runat="server" CssClass="label label-danger" ControlToValidate="txtEmailAddress"
                        Display="Dynamic" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

                </div>
            </div>
            <div id="signature-alert" style="display: none" class="alert alert-danger" data-appear-animation="bounceIn" role="alert" aria-hidden="true">
                <i class="fa fa-exclamation-circle me-2"></i>For signature stamps, sign your name 3 times on a sheet of paper, in <strong>black ink only</strong> and send <a href="mailto:purchasing@jud12.flcourts.org">Linda</a> the original. Please interoffice the original signed paper to Linda Pluta.
            </div>
            <div class="row">
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpLocation" Text="Delivery Location<em>*</em>" />
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
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtConsumerName" Text="Who is the Stamp for?<em>*</em>" />
                    <asp:TextBox ID="txtConsumerName" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConsumerName"
                        CssClass="label label-danger" ErrorMessage="Phone is Required" />
                </div>
                <div class="col-lg-4">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpStampType" Text="Type of Stamp<em>*</em>" />
                    <asp:DropDownList ID="drpStampType" runat="server" CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Text="< Select Type >" Value=""></asp:ListItem>
                        <asp:ListItem Text="conforming"></asp:ListItem>
                        <asp:ListItem Text="signature"></asp:ListItem>
                        <asp:ListItem Text="other"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpStampType"
                        CssClass="label label-danger" ErrorMessage="Please Select a Type" />
                </div>
            </div>
            <div class="row">
                <div class="col-auto">
                    <asp:Label runat="server" ID="lblFontStyle" CssClass="form-label" ClientIDMode="Static" AssociatedControlID="drpFontStyle" Text="Font Style<em>*</em>" />
                    <asp:DropDownList ID="drpFontStyle" runat="server" CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Text="< Select Font Style >" Value=""></asp:ListItem>
                        <asp:ListItem Text="Arial" style="font-family: Arial; font-size: 2em"></asp:ListItem>
                        <asp:ListItem Text="Arial Narrow" style="font-family: Arial Narrow; font-size: 2em"></asp:ListItem>
                        <asp:ListItem Text="Calibri" style="font-family: Calibri; font-size: 2em"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ClientIDMode="Static" ID="valFontStyle" ControlToValidate="drpFontStyle"
                        CssClass="label label-danger" ErrorMessage="Font Style is Required" />
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" ID="lblFontSize" CssClass="form-label" ClientIDMode="Static" AssociatedControlID="txtFontSize" Text="Font Size<em>*</em>" />
                    <asp:TextBox ID="txtFontSize" runat="server" MaxLength="50" TextMode="Number" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    <div class="form-text"><i>(Numeric Point Size)</i></div>
                    <asp:RequiredFieldValidator runat="server" ClientIDMode="Static" ID="valFontSize" ControlToValidate="txtFontSize"
                        CssClass="label label-danger" ErrorMessage="Font Size is Required" />
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" ID="lblInkColor" CssClass="form-label" ClientIDMode="Static" AssociatedControlID="drpInkColor" Text="Ink Color<em>*</em>" />
                    <asp:DropDownList ID="drpInkColor" runat="server" CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Text="< Select Ink Color >" Value=""></asp:ListItem>
                        <asp:ListItem Text="Black"></asp:ListItem>
                        <asp:ListItem Text="Blue"></asp:ListItem>
                        <asp:ListItem Text="Red"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ClientIDMode="Static" ID="valFontColor" ControlToValidate="drpInkColor"
                        CssClass="label label-danger" ErrorMessage="Ink Color is Required" />

                </div>
                <div class="col-auto">
                    <asp:Label runat="server" ID="lblAlignment" CssClass="form-label" ClientIDMode="Static" AssociatedControlID="drpAlignment" Text="Text Alignment" />
                    <asp:DropDownList ID="drpAlignment" runat="server" CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Text="Center" Value="C"></asp:ListItem>
                        <asp:ListItem Text="Left" Value="L"></asp:ListItem>
                        <asp:ListItem Text="Right" Value="R"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity<em>*</em>" />
                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="50" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuantity"
                        CssClass="label label-danger" ErrorMessage="Quantity is Required" />

                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtSample" Text="Enter Text to Appear on Stamp<em>*</em>" />
                    <asp:TextBox ID="txtSample" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="4" CssClass="sample form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSample"
                        CssClass="label label-danger" ErrorMessage="Sample is Required" />
                </div>
                <div class="col-md-6">
                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtInstructions" Text="Additional Information for Purchasing" />
                    <asp:TextBox ID="txtInstructions" runat="server" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
            </div>
            <asp:Literal ID="ltUploadMessage" runat="server">
                <div class="alert alert-info"><i class="fas fa-info-circle" aria-hidden="true"></i>Click select and browse for files to upload. Acceptable file types are .docx, .doc, .xls, .xlsx, .pdf, .jpg, .gif, .png</div>
            </asp:Literal>
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplAttachments" Text="Upload a Page with Stamp if Available" />
                    <div style="position: relative;">
                        <div id="attach-overlay" class="overlay" style="display: none;">
                            <div class="spinner"></div>
                        </div>
                        <asp:FileUpload ID="uplAttachments" runat="server" ToolTip="Select File to Upload" ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx,.gif,.png,.jpg,.jpeg" />
                        <span id="fileAttachmentWarning" style="display: none" class="label label-danger attachment-warning">Please Choose File to Upload</span>
                    </div>
                </div>
            </div>
            <div>
                <div class="formFieldAdjust">
                    <span id="attachmentInfo"></span>
                    <asp:Literal ID="ltAttachments" Text="text" runat="server">
                    <ul id="attachmentList" class="attachments">
                    </ul>
                    </asp:Literal>
                </div>
            </div>
            <asp:HiddenField ClientIDMode="Static" ID="hdAttachmentIds" runat="server" />
        </fieldset>
        <hr />
        <p>
            <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Submit Order" OnClick="cmdSave_Click" />
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </p>
    </div>
    <asp:Literal ID="ltBottomMessage" runat="server">
        <div class="alert alert-info"><i class="fas fa-info-circle" aria-hidden="true"></i>Once your order arrives, it will be delivered to you the same day in person or via interoffice.</div>
    </asp:Literal>

</div>
<script type="text/javascript">
    var extensionHash = {
        'pdf': 1,
        'doc': 1,
        'docx': 1,
        'xls': 1,
        'xlsx': 1,
        'jpg': 1,
        'gif': 1,
        'png': 1,
        'jpeg': 1,
    };
    jQuery(function ($) {
        window.onload = function () {
            UpdateSampleStyle();
        };
        $("#stamp-form").on("click", "#uplAttachments", function (e) {
            $("#attach-overlay").show();
        });
        $("#stamp-form").on("change", "#uplAttachments", function (e) {
            check_extension($(this).val());
        });
        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        $('#drpAlignment').change(function () {
            UpdateSampleStyle();
        });
        $('#drpStampType').change(function () {
            var stampType = $(this).val();
            if (stampType == 'signature') {
                $('#signature-alert').show();
                $('#signature-alert').focus();
                $('#lblFontStyle').html("Font Style");
                $('#lblFontSize').html("Font Size");
                $('#lblInkColor').html("Ink Color");
                ValidatorEnable(document.getElementById("valFontStyle"), false);
                ValidatorEnable(document.getElementById("valFontColor"), false);
                ValidatorEnable(document.getElementById("valFontSize"), false);
            } else {
                $('#signature-alert').hide();
                $('#lblFontStyle').html("Font Style<em>*</em>");
                $('#lblFontSize').html("Font Size<em>*</em>");
                $('#lblInkColor').html("Ink Color<em>*</em>");
                ValidatorEnable(document.getElementById("valFontStyle"), true);
                ValidatorEnable(document.getElementById("valFontColor"), true);
                ValidatorEnable(document.getElementById("valFontSize"), true);
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
    });
    function UpdateSampleStyle() {
        var fontStyle = $('#drpFontStyle').val();
        var fontSize = $('#txtFontSize').val();
        var fontColor = $('#drpInkColor').val();
        var fontHexColor = '#000';
        var align = $('#drpAlignment').val();
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
        switch (align) {
            case "C":
                $('.sample').css("text-align", "center");
                break;
            case "R":
                $('.sample').css("text-align", "right");
                break;
            case "L":
                $('.sample').css("text-align", "left");
                break;
            default:
                $('.sample').css("text-align", "center");
                break;
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
        var html = upload.parent().html();
        upload.parent().html(html.replace(filename, "Choose File"));
        $("#attachmentInfo").html('');
    }
    function HandleUpload() {
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
            data.append("mid",<%=ModuleId%>);
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
                $("#attachmentList").append("<li data-fileId='" + fileId + "'><span>" + filename + "</span>&nbsp;<a class='float-end' onclick=\"DeleteAttachment('" + fileId + "')\"><em class='fa fa-trash'></em></></li>");
                WriteAttachmentMessage(filename);
            };
            options.error = function (err) {
                alert(err.statusText);
                setTimeout(function () {
                    $("#attach-overlay").hide();
                    $("#attachmentInfo").html('');
                }, 1000);
            };
            $.ajax(options);
        }
    }
    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        if (extensionHash[ext]) {
            $("#attachmentInfo").html("");
            HandleUpload();
            return true;
        } else {
            $("#attachmentInfo").html("<span class='text-danger'>Invalid File Type, please choose an allowed file type!</span>");
            return false;
        }
    }
    function CloseModal() {
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate("Form");
        }
        if (Page_IsValid) {
            $('#modFormOrder').modal('hide');
        }
    }
    function DeleteAttachment(fileId) {
        var data = new FormData();
        data.append("fid", fileId);
        var options = {};
        options.url = "<%=attachmentHandler%>";
        options.type = "POST";
        options.data = data;
        options.contentType = false;
        options.processData = false;
        options.success = function (result) {
            var listItem = $("li[data-fileid='" + fileId + "']");
            listItem.remove();
            var fileList = "";
            $('li[data-fileid]').each(function () {
                var id = $(this).data("fileid");
                if (fileList == "") {
                    fileList = id;
                } else {
                    fileList = fileList + "," + id;
                }
            });
            $("#hdAttachmentIds").val(fileList);
        };
        options.error = function (err) { alert(err.statusText); };
        $.ajax(options);
        return false;
    }
</script>
