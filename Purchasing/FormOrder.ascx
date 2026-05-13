<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormOrder.ascx.cs" Inherits="tjc.Modules.Purchasing.FormOrder" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="<i class='fas fa-list-alt'></i> Manage Orders" CssClass="btn btn-danger btn-large mb-4" runat="server" />

<div class="form-order-container purchasing">
    <div id="form-order-form">
        <fieldset class="row g-3">
            <asp:HiddenField ID="hdOrderId" ClientIDMode="Static" runat="server" />
            <div class="col-md-4">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name<em>*</em>" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor" ValidationGroup="Order"
                    CssClass="label label-danger" ErrorMessage="Requester is Required" />
            </div>
            <div class="col-lg-4">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmailAddress" Text="Email Address<em>*</em>" />
                <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="250" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailAddress"
                    CssClass="label label-danger" ErrorMessage="Email Address is Required" />
                <asp:RegularExpressionValidator ID="valEmail" runat="server" CssClass="label label-danger" ControlToValidate="txtEmailAddress"
                    Display="Dynamic" ErrorMessage="Incorrect e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            </div>

            <div class="col-md-4">
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
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpLocation" ValidationGroup="Order"
                    CssClass="label label-danger" ErrorMessage="Please Select a Delivery Location" />
            </div>
        </fieldset>
        <button type="button" id="btnAddForm" role="button" data-toggle="modal" class="btn btn-success" data-target="#modFormOrder"><i class="fas fa-plus" aria-hidden="true"></i>&nbsp;Add Form to Order</button>
        <div class="bg-light ps-3 pe-3 rounded">
            <asp:HiddenField ClientIDMode="Static" ID="hdAttachmentIds" runat="server" />
            <asp:Repeater ID="rptForms" runat="server" OnItemCommand="rptForms_ItemCommand" OnItemDataBound="rptForms_ItemDataBound">
                <HeaderTemplate>
                    <div class="heading heading-border heading-bottom-border mt-3">
                        <h4>Form Order Lines</h4>
                    </div>
                    <table id="tblFormOrderLines" class="table table-striped">
                        <thead>
                            <tr>
                                <th>Form #</th>
                                <th># Sets</th>
                                <th># Parts</th>
                                <th>Page Size</th>
                                <th>Description</th>
                                <th>End User</th>
                                <th>Comments</th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("FormNumber") %>
                        </td>
                        <td>
                            <%#Eval("Quantity") %>
                        </td>
                        <td>
                            <%#Eval("NumberParts") %>
                        </td>
                        <td>
                            <%#Eval("PageType") %>
                        </td>
                        <td><%#Eval("Description") %></td>
                        <td><%#Eval("Recipient") %></td>
                        <td><%#Eval("Comments") %></td>
                        <td>
                            <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDeleted" CssClass="text-danger confirm" CommandName="delete" CommandArgument='<%#Eval("FormId") %>'><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table><asp:Literal ID="ltEmptyMessage" runat="server" Visible="false"><div class="alert alert-info"><i class="fa fa-info-circle"></i> Use the "Add Form to Order" button above to add a form to your order!</div></asp:Literal>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modFormOrder" tabindex="-1" role="dialog" aria-labelledby="lblFormOrder" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="lblFormOrder">Add one or more forms to the order</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                    <div class="modal-body">
                        <fieldset id="Form-item-form" class="row g-3">
                            <asp:HiddenField ID="hdFormId" ClientIDMode="Static" runat="server" />
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormNumber" Text="Form #<em>*</em>" />
                                <asp:TextBox ID="txtFormNumber" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                <div class="form-text">Enter NA if no form number exists</div>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtFormNumber"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Form Number is Required. Enter NA if there is none." />
                            </div>
                            <div class="col-md-8">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormName" Text="Exact Title of Form<em>*</em>" />
                                <asp:TextBox ID="txtFormName" ClientIDMode="Static" runat="server" MaxLength="200" CssClass="form-control"></asp:TextBox><div class="form-text">Tell us what it says on the bottom left-hand footer of form</div>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtFormName"
                                    CssClass="label label-danger" ErrorMessage="Form Title is Required" />
                            </div>
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtNumberSets" Text="Number of Sets<em>*</em>" />
                                <asp:TextBox ID="txtNumberSets" ClientIDMode="Static" runat="server" MaxLength="5" step="25" Min="0" Max="1000" TextMode="Number" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberSets"
                                    CssClass="label label-danger" ErrorMessage="Please Select the Number of Sets" />
                                <asp:CompareValidator Display="Dynamic" ValidationGroup="Form" CssClass="label label-danger" runat="server" ErrorMessage="The Value must be number only" ControlToValidate="txtNumberSets" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
                            </div>
                            <div class="col-md-8">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRecipient" Text="Recipient Name<em>*</em>" />
                                <asp:TextBox ID="txtRecipient" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtRecipient"
                                    CssClass="label label-danger" ErrorMessage="Recipient is Required" />
                            </div>
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpNumberParts" Text="Number of Parts<em>*</em>" />
                                <asp:DropDownList ID="drpNumberParts" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="< Select Number >" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="1"></asp:ListItem>
                                    <asp:ListItem Text="2"></asp:ListItem>
                                    <asp:ListItem Text="3"></asp:ListItem>
                                    <asp:ListItem Text="4"></asp:ListItem>
                                    <asp:ListItem Text="5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpNumberParts"
                                    CssClass="label label-danger" ErrorMessage="Please Select Number of Parts" />
                            </div>
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpPageType" Text="Page Size<em>*</em>" />
                                <asp:DropDownList ID="drpPageType" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="< Select Page Size >" Value=""></asp:ListItem>
                                    <asp:ListItem Text="1-sided, black"></asp:ListItem>
                                    <asp:ListItem Text="1-sided, color"></asp:ListItem>
                                    <asp:ListItem Text="2-sided, black"></asp:ListItem>
                                    <asp:ListItem Text="2-sided, color"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpPageType"
                                    CssClass="label label-danger" ErrorMessage="Please Select Page Type" />
                            </div>
                            <div class="col-md-12">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtDescription" Text="Description<em>*</em>" />
                                <asp:TextBox ID="txtDescription" ClientIDMode="Static" TextMode="MultiLine" Rows="3" runat="server" MaxLength="2000" CssClass="form-control"></asp:TextBox>
                                <div class="form-text">Purchasing does not keep copies of the forms on hand; therefore, we need as much detail as possible.</div>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtDescription"
                                    CssClass="label label-danger" ErrorMessage="Description is Required" />
                            </div>
                            <div class="col-md-12" id="divComments" runat="server" visible="false">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtComments" Text="Comments" />
                                <asp:TextBox ID="txtComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-12 clearfix">
                                <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplAttachments" Text="Upload Attachments<em>*</em>" />
                                <div style="position: relative;">
                                    <div id="attach-overlay" class="overlay" style="display: none;">
                                        <div class="spinner"></div>
                                    </div>
                                    <asp:FileUpload ID="uplAttachments" runat="server" ToolTip="Select File to Upload" ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx,.tiff,.tif,.jpg,.jpeg" />
                                    <span id="fileAttachmentWarning" style="display: none" class="label label-danger attachment-warning">Please Choose File to Upload</span>
                                    <asp:CustomValidator ID="valUpload" Display="Dynamic" ValidationGroup="Form" runat="server" CssClass="label label-danger" ClientValidationFunction="validateUpload"
                                        ErrorMessage="Please select at least one file" OnServerValidate="valUpload_ServerValidate"></asp:CustomValidator>
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
                    </div>
                    <div class="modal-footer justify-content-between">
                        <asp:LinkButton ID="cmdAddForm" ClientIDMode="Static" runat="server" OnClientClick="CloseModal()" ValidationGroup="Form" CssClass="btn btn-primary" Text="Save Form" OnClick="cmdAddForm_Click" />
                        <asp:HyperLink ID="lnkCancelLine" data-dismiss="modal" runat="server" CssClass="btn btn-secondary" Text="Cancel Form" />
                    </div>
                </div>
            </div>
        </div>

        <hr />
        <p class="mt-3">
            <asp:Button ID="cmdSave" Enabled="false" ClientIDMode="Static" runat="server" ValidationGroup="Order" CssClass="btn btn-primary" OnClick="cmdSave_Click" Text="Submit" />
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </p>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

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
    (function ($, Sys) {

        $("#form-order-form").on("click", "#uplAttachments", function (e) {
            $("#attach-overlay").show();
        });
        $("#form-order-form").on("change", "#uplAttachments", function (e) {
            check_extension($(this).val());
        });
        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        $("#cmdSave").on("click", function (e) {
            if ($('#cmdSave').val() == "Please Wait") {
                e.preventDefault();
                return false;
            }
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate('Order');
            }
            if (Page_IsValid) {
                $('#cmdSave').prop("disabled", false);
                $('#cmdSave').val("Please Wait");
            } else {
                return false
            }
        });

    }(jQuery, window.Sys));
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
                $("#attachmentList").append("<li data-fileId='" + fileId + "'><span>" + filename + "</span>&nbsp;<a class='float-end text-danger' onclick=\"DeleteAttachment('" + fileId + "')\"><i class='fas fa-trash'></i></a></li>");
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
    function validateUpload(source, e) {
        $("#attachmentList").children().length > 0 ? e.IsValid = true : e.IsValid = false;
    }  
</script>
