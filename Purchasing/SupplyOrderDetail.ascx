<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupplyOrderDetail.ascx.cs" Inherits="tjc.Modules.Purchasing.SupplyOrderDetail" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>

<div class="supply-order-detail-container purchasing">
    <div id="supply-order-form">
        <fieldset class="row g-3">
            <asp:HiddenField ID="hdOrderId" ClientIDMode="Static" runat="server" />
            <div class="col-md-3">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name<em>*</em>" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-4 ">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmail" Text="Requester Email<em>*</em>" />
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-3 ">
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
            </div>
        </fieldset>
        <button type="button" id="btnAddSupply" role="button" data-toggle="modal" class="btn btn-success mt-3 mb-3" data-target="#modSupplyOrder"><i class="fas fa-plus"></i>&nbsp;Add Supply to Order</button>
        <div class="bg-light ps-3 pe-3 rounded">
            <asp:HiddenField ClientIDMode="Static" ID="hdAttachmentIds" runat="server" />
            <asp:Repeater ID="rptSupplies" runat="server" OnItemCommand="rptSupplies_ItemCommand" OnItemDataBound="rptSupplies_ItemDataBound">
                <HeaderTemplate>
                    <h4>Supply Order Lines</h4>
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
                            <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDeleted" CssClass="confirm item-link" CommandName="delete" CommandArgument='<%#Eval("SupplyId") %>'><i class="fa fa-trash"></i></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table><asp:Literal ID="ltEmptyMessage" runat="server" Visible="false"><div class="alert alert-info"><i class="fa fa-info-circle"></i> Use the "Add Supply to Order" button above to add an item to your order!</div></asp:Literal>
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
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtSupplyNumber" Text="Item Number<em>*</em>" />
                                <asp:TextBox ID="txtSupplyNumber" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" ControlToValidate="txtSupplyNumber"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Item Number is Required. Enter NA if there is none." />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRecipient" Text="Recipient Name<em>*</em>" />
                                <asp:TextBox ID="txtRecipient" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtRecipient"
                                    CssClass="label label-danger" ErrorMessage="Recipient is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtStore" Text="Store<em>*</em>" />
                                <asp:TextBox ID="txtStore" runat="server" list="storeList" MaxLength="250" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                <datalist id="storeList">
                                    <option value="Amazon">
                                    <option value="Office Depot">
                                </datalist>
                                <div class="form-text">Select from list or type</div>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtStore"
                                    CssClass="label label-danger" ErrorMessage="Store is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtLink" Text="Paste Hyperlink to Item" />
                                <asp:TextBox ID="txtLink" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                <div class="form-text"><a href="https://youtu.be/PFI7OJoUn34" target="_blank">How do I do that?</a></div>
                            </div>

                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity<em>*</em>" />
                                <asp:TextBox ID="txtQuantity" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <asp:CompareValidator ID="valIsNumber" Display="Dynamic" ValidationGroup="Supply" CssClass="label label-danger" runat="server" ErrorMessage="The Value must be number only" ControlToValidate="txtQuantity" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" ControlToValidate="txtQuantity"
                                    CssClass="label label-danger" Display="Dynamic" ErrorMessage="Quantity is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtUnitsOfMeasure" Text="Unit of Measure<em>*</em>" />
                                <asp:TextBox ID="txtUnitsOfMeasure" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                <div class="form-text">Number of pieces per quantity ordered (individual, dozen, case, pack, box, etc.)</div>
                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Supply" Display="Dynamic" ControlToValidate="txtUnitsOfMeasure"
                                    CssClass="label label-danger" ErrorMessage="Unit of Measure is Required" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtDescription" Text="Description<em>*</em>" />
                                <asp:TextBox ID="txtDescription" ClientIDMode="Static" TextMode="MultiLine" Rows="3" runat="server" MaxLength="2000" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ValidationGroup="Supply" ControlToValidate="txtDescription"
                                    CssClass="label label-danger" ErrorMessage="Description is Required" />
                            </div>
                            <div class="col-md-6" id="divComments" runat="server" visible="false">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtComments" Text="Comments" />
                                <asp:TextBox ID="txtComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                            </div>
                        </fieldset>
                    </div>
                    <div class="modal-footer justify-content-between">
                        <asp:LinkButton ID="cmdAddSupply" ClientIDMode="Static" runat="server" OnClientClick="CloseModal()" ValidationGroup="Supply" CssClass="btn btn-primary" Text="Add Supply Items" OnClick="cmdAddSupply_Click" />
                        <button id="lnkCancelLine" data-dismiss="modal" class="btn btn-secondary">Cancel Supply</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="dnnSupplyItem">
                <ul id="attachmentList" class="attachments">
                    <asp:Literal id="ltAttachments" runat="server" />
                </ul>
        </div>

        <hr />
        <p class="mt-3">
            <asp:LinkButton ID="cmdSave" ClientIDMode="Static" runat="server" ValidationGroup="Order" CssClass="btn btn-primary" Text="Save Order" OnClick="cmdSave_Click" />
            <asp:LinkButton ID="cmdCancel" CausesValidation="false" runat="server" CssClass="btn btn-secondary" Text="Cancel" OnClick="cmdCancel_Click" />
        </p>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $("#supply-order-form").on("change", "#uplAttachments", function (e) {
            check_extension($(this).val());
        });
        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
        var orderid = $('#hdOrderId').val();
        if (orderid == "") {
            $('#cmdSave').hide();
        }
        $("#lnkCancelLine").on("click", function (e) {
            e.preventDefault();
            $('#modSupplyOrder').modal('hide');
            ClearForm();
        });
        var myModalEl = document.getElementById('modSupplyOrder')
        myModalEl.addEventListener('hidden.bs.modal', function (event) {
            ClearForm();
        })
    }(jQuery, window.Sys));

    function CloseModal() {
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate("Supply");
        }
        if (Page_IsValid) {
            $('#modSupplyOrder').modal('hide');
        }
    }
    function ClearForm() {
        $("#hdSupplyId").val("");
        $("#txtRecipient").val("");
        $("#txtSupplyNumber").val("");
        $("#txtStore").val("");
        $("#txtLink").val("");
        $("#txtQuantity").val("");
        $("#txtUnitsOfMeasure").val("");
        $("#txtDescription").val("");
        $("#txtComments").val("");
    }
</script>
