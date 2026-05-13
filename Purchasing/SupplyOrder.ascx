<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupplyOrder.ascx.cs" Inherits="tjc.Modules.Purchasing.SupplyOrder" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="<i class='fas fa-list-alt'></i> Manage Orders" CssClass="btn btn-danger btn-large mb-4" runat="server" />

<div class="supply-order-container purchasing">
    <div id="supply-order-form">
        <div class="alert alert-info"><i class="fa fa-info-circle"></i>Add one or more supply items to your order! Then use the Submit Order button to send the order to Purchasing</div>
        <fieldset class="row g-3">
            <asp:HiddenField ID="hdOrderId" ClientIDMode="Static" runat="server" />
            <div class="col-md-4">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name<em>*</em>" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ValidationGroup="Order" ControlToValidate="txtRequestor"
                    CssClass="label label-danger" ErrorMessage="Requester is Required" />
            </div>
            <div class="col-md-4">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmail" Text="Requester Email<em>*</em>" />
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                    CssClass="label label-danger" ErrorMessage="Email is Required" ValidationGroup="Order" />
                <asp:RegularExpressionValidator ID="valEmail" runat="server" CssClass="label label-danger" ControlToValidate="txtEmail"
                    Display="Dynamic" ErrorMessage="Incorrect e-mail format" ValidationGroup="Order" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>


            </div>
            <div class="col-auto">
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
        <button type="button" id="btnAddSupply" role="button" data-toggle="modal" class="btn btn-success" data-target="#modSupplyOrder"><i class="fas fa-plus" aria-hidden="true"></i>&nbsp;Add Supply to Order</button>

        <div class="bg-light ps-3 pe-3 rounded">
            <asp:HiddenField ClientIDMode="Static" ID="hdAttachmentIds" runat="server" />
            <asp:Repeater ID="rptSupplies" runat="server" OnItemCommand="rptSupplies_ItemCommand" OnItemDataBound="rptSupplies_ItemDataBound">
                <HeaderTemplate>
                    <div class="heading heading-border heading-bottom-border mt-3">
                        <h4>Supply Order Lines</h4>
                    </div>
                    <table id="tblSupplyOrderLines" class="table table-striped">
                        <thead>
                            <tr>
                                <th>Item #</th>
                                <th>Store</th>
                                <th>Description</th>
                                <th>Qty</th>
                                <th>Units of Measure</th>
                                <th>End User</th>
                                <th>Comments</th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("ItemNumber") %></td>
                        <td><%#Eval("Store") %></td>
                        <td><%#Eval("LinkedDescription") %></td>
                        <td><%#Eval("Quantity") %></td>
                        <td><%#Eval("UnitOfMeasure") %></td>
                        <td><%#Eval("Recipient") %></td>
                        <td><%#Eval("Comments") %></td>
                        <td>
                            <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDeleted" CssClass="text-danger confirm item-link" CommandName="delete" CommandArgument='<%#Eval("SupplyId") %>'><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table><asp:Literal ID="ltEmptyMessage" runat="server" Visible="false"><div class="pb-3">No Items Added! Use the "Add Supply to Order" button above to add an item to your order!</div></asp:Literal>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modSupplyOrder" tabindex="-1" role="dialog" aria-labelledby="lblSupplyOrder" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="lblSupplyOrder">Add one or more Supply Items to the order</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                    <div class="modal-body">
                        <fieldset id="Supply-item" class="row g-3">
                            <asp:HiddenField ID="hdSupplyId" ClientIDMode="Static" runat="server" />
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtSupplyNumber" Text="Item Number<em>*</em>" />
                                <asp:TextBox ID="txtSupplyNumber" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" ControlToValidate="txtSupplyNumber"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Item Number is Required. Enter NA if there is none." />
                                <div class="form-text">Enter NA if no item number exists</div>
                            </div>
                            <div class="col-md-8">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRecipient" Text="Recipient Name<em>*</em>" />
                                <asp:TextBox ID="txtRecipient" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtRecipient"
                                    CssClass="label label-danger" ErrorMessage="Recipient is Required" />
                            </div>
                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtStore" Text="Store<em>*</em>" />
                                <asp:TextBox ID="txtStore" runat="server" list="storeList" MaxLength="250" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                <datalist id="storeList">
                                    <option value="Amazon">
                                    <option value="Office Depot">
                                </datalist>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtStore"
                                    CssClass="label label-danger" ErrorMessage="Store is Required" />
                                <div class="form-text">Select from list or type</div>
                            </div>
                            <div class="col-md-8">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtLink" Text="Paste Hyperlink to Item" />
                                <asp:TextBox ID="txtLink" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                <div class="form-text"><a href="https://youtu.be/PFI7OJoUn34" target="_blank">How do I do that?</a></div>
                            </div>

                            <div class="col-md-4">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity<em>*</em>" />
                                <asp:TextBox ID="txtQuantity" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:CompareValidator ID="valIsNumber" Display="Dynamic" ValidationGroup="Supply" CssClass="label label-danger" runat="server" ErrorMessage="The Value must be number only" ControlToValidate="txtQuantity" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" ControlToValidate="txtQuantity"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Quantity is Required" />
                            </div>
                            <div class="col-md-8">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtUnitsOfMeasure" Text="Unit of Measure<em>*</em>" />
                                <asp:TextBox ID="txtUnitsOfMeasure" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" Display="Dynamic" ControlToValidate="txtUnitsOfMeasure"
                                    CssClass="label label-danger" ErrorMessage="Unit of Measure is Required" />
                                <div class="form-text">Number of pieces per quantity ordered (each, dozen, case, pack, box, etc.)</div>

                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtDescription" Text="Description of item(s)<em>*</em>" />
                                <asp:TextBox ID="txtDescription" ClientIDMode="Static" TextMode="MultiLine" Rows="3" runat="server" MaxLength="2000" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtDescription"
                                    CssClass="label label-danger" ErrorMessage="Description is Required" />
                            </div>
                            <div class="col-md-6" id="divComments" runat="server" visible="false">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtComments" Text="Comments" />
                                <asp:TextBox ID="txtComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-12 clearfix">
                                <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplAttachments" Text="Upload Attachments" />
                                <div style="position: relative;">
                                    <div id="attach-overlay" class="overlay" style="display: none;">
                                        <div class="spinner"></div>
                                    </div>
                                    <asp:FileUpload ID="uplAttachments" runat="server" ToolTip="Select File to Upload" ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx,.tiff,.tif,.jpg,.jpeg" />
                                    <span id="fileAttachmentWarning" style="display: none" class="label label-danger attachment-warning">Please Choose File to Upload</span>
                                </div>
                            </div>
                            <div class="dnnSupplyItem">
                                <div class="formFieldAdjust">
                                    <span id="attachmentInfo"></span>
                                    <ul id="attachmentList" class="attachments">
                                    </ul>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                    <div class="modal-footer justify-content-between">
                        <asp:LinkButton ID="cmdAddSupply" ClientIDMode="Static" runat="server" OnClientClick="CloseModal()" ValidationGroup="Supply" CssClass="btn btn-primary" Text="Add Supply Items" OnClick="cmdAddSupply_Click" />
                        <asp:HyperLink ID="lnkCancelLine" data-dismiss="modal" runat="server" CssClass="btn btn-secondary" Text="Cancel Supply" />
                    </div>
                </div>
            </div>
        </div>

        <hr />
        <p class="mt-3">
            <asp:Button ID="cmdSave" Enabled="false" ClientIDMode="Static" runat="server" ValidationGroup="Order" CssClass="btn btn-primary" Text="Submit Order" OnClick="cmdSave_Click" />
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

        $("#supply-order-form").on("change", "#uplAttachments", function (e) {
            check_extension($(this).val());
        });
        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this item?',
            title: 'Delete line item?'
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
                    $("#hdAttachmentIds").val(fileIdList + "|" + fileId);
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
            Page_ClientValidate("Supply");
        }
        if (Page_IsValid) {
            $('#modSupplyOrder').modal('hide');
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
