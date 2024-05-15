<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormOrder.ascx.cs" Inherits="tjc.Modules.Purchasing.FormOrder" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="Manage Orders" CssClass="SubHead" runat="server" />

<div class="form-order-container">
    <div id="form-order-form">
        <fieldset class="row g-3">
            <asp:HiddenField ID="hdOrderId" ClientIDMode="Static" runat="server" />
            <div class="col-md-4 col-sm-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor"
                    CssClass="label label-danger" ErrorMessage="Requester is Required" />
            </div>
            <div class="col-md-4 col-sm-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpLocation" Text="Delivery Location" />
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
        </fieldset>
        <div class="heading heading-border heading-bottom-border">
            <h2>Form Order Lines</h2>
        </div>
        <asp:HiddenField ClientIDMode="Static" ID="hdAttachmentIds" runat="server" />
        <button type="button" id="btnAddForm" role="button" data-toggle="modal" class="btn btn-success" data-target="#modFormOrder"><i class="fas fa-plus" aria-hidden="true"></i>&nbsp;Add Form to Order</button>
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
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormNumber" Text="Form #" />
                                <asp:TextBox ID="txtFormNumber" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                <div class="form-text">Enter NA if no form number exists</div>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtFormNumber"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Form Number is Required. Enter NA if there is none." />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormName" Text="Exact Title of Form" />
                                <asp:TextBox ID="txtFormName" ClientIDMode="Static" runat="server" MaxLength="200" CssClass="form-control"></asp:TextBox><div class="form-text">Tell us what it says on the bottom left-hand footer of form</div>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtFormName"
                                    CssClass="label label-danger" ErrorMessage="Form Title is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity" />
                                <datalist id="dlQuantity">
                                    <option value="250">
                                    <option value="500">
                                    <option value="1000">
                                    <option value="NA">
                                </datalist>
                                <asp:TextBox ID="txtQuantity" ClientIDMode="Static" runat="server" list="dlQuantity" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:CompareValidator ID="valIsNumber" Display="Dynamic" ValidationGroup="Form" CssClass="label label-danger" runat="server" ErrorMessage="The Value must be number only" ControlToValidate="txtQuantity" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtQuantity"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Quantity is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRecipient" Text="Recipient Name" />
                                <asp:TextBox ID="txtRecipient" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtRecipient"
                                    CssClass="label label-danger" ErrorMessage="Recipient is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtDescription" Text="Description" />
                                <asp:TextBox ID="txtDescription" ClientIDMode="Static" TextMode="MultiLine" Rows="4" runat="server" MaxLength="2000" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Form" ControlToValidate="txtDescription"
                                    CssClass="label label-danger" ErrorMessage="Description is Required" />
                            </div>
                            <div class="col-md-6" id="divComments" runat="server" visible="false">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtComments" Text="Comments" />
                                <asp:TextBox ID="txtComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-12 clearfix">

                                <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplAttachments" Text="Upload Attachments" />
                                <div style="position: relative;">
                                    <div id="attach-overlay" class="overlay" style="display: none;">
                                        <div class="spinner"></div>
                                    </div>
                                    <asp:FileUpload ID="uplAttachments" runat="server" ToolTip="Select File to Upload" onchange='check_extension(this.value);' ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx,.tiff,.tif,.jpg,.jpeg" />
                                    <asp:Button ID="cmdAddAttachment" ClientIDMode="Static" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
                                    <span id="fileAttachmentWarning" style="display: none" class="label label-danger attachment-warning">Please Choose File to Upload</span>
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
                    <div class="modal-footer">
                        <asp:LinkButton ID="cmdAddForm" ClientIDMode="Static" runat="server" ValidationGroup="Form" CssClass="btn btn-primary pull-left" Text="Save Form" OnClick="cmdAddForm_Click" />
                        <asp:HyperLink ID="lnkCancelLine"  data-dismiss="modal" runat="server" CssClass="btn btn-secondary" Text="Cancel Form" />
                    </div>
                </div>
            </div>
        </div>
        <asp:Repeater ID="rptForms" runat="server" OnItemCommand="rptForms_ItemCommand" OnItemDataBound="rptForms_ItemDataBound">
            <HeaderTemplate>
                <table id="tblFormOrderLines" class="table table-striped">
                    <thead>
                        <tr>
                            <th>Form #</th>
                            <th>Description</th>
                            <th>Qty</th>
                            <th>End User</th>
                            <th>Comments</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="SubHead">
                        <asp:HyperLink ToolTip="Click to view details of the Form" ID="lnkItemEdit" runat="server"><%#Eval("FormNumber") %></asp:HyperLink>
                    </td>
                    <td><%#Eval("Description") %></td>
                    <td><%#Eval("Quantity") %></td>
                    <td><%#Eval("Recipient") %></td>
                    <td><%#Eval("Comments") %></td>
                    <td>
                        <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDeleted" CssClass="confirm" CommandName="delete" CommandArgument='<%#Eval("FormId") %>'><img title="Delete this record" src="/images/action_delete.gif" /></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
        <hr />
        <p class="mt-3">
            <asp:LinkButton ID="cmdSave" ClientIDMode="Static" runat="server" ValidationGroup="Order" CssClass="btn btn-primary" Text="Save Order" OnClick="cmdSave_Click" />
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </p>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />

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
        var table = $('#tblFormOrderLines').DataTable({
            "order": [[0, "desc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
        });
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
        var orderid = $('#hdOrderId').val();
        if (orderid == "") {
            $('#cmdSave').hide();
        }
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
                    $("#attachmentList").append("<li 'data-fileId='" + fileId + "'><span>" + filename + "</span>&nbsp;<a class='float-end' onclick=\"DeleteAttachment('" + fileId + "')\"><em class='fa fa-trash'></em></></li>");
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
    }(jQuery, window.Sys));

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
            $("#cmdAddAttachment").trigger("click");
            $("#cmdAddForm").prop("disabled", false);
            return true;
        } else {
            $("#attachmentInfo").html("<span class='text-danger'>Invalid File Type, please choose an allowed file type!</span>");
            $("#cmdAddForm").prop("disabled", true);
            return false;
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
