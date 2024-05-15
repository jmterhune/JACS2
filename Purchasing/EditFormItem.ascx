<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFormItem.ascx.cs" Inherits="tjc.Modules.Purchasing.EditFormItem" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="Manage Orders" CssClass="SubHead" runat="server" />
<div class="form-order-container">
    <div id="form-order-form">
        <fieldset class="row g-3">
            <asp:HiddenField ID="hdOrderId" ClientIDMode="Static" runat="server" />
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name" />
                <asp:TextBox ID="txtRequestor" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor"
                    CssClass="label label-danger" ErrorMessage="Requester is Required" />
            </div>
            <div class="col-md-6">
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
            <h2>Order Form Lines</h2>
        </div>
        <button type="button" id="btnAddForm" role="button" data-toggle="modal" data-target="#modFormOrder"><i class="fas fa-plus" aria-hidden="true"></i>Add Form to Order</button>
        <div class="modal fade" id="modFormOrder" tabindex="-1" role="dialog" aria-labelledby="lblFormOrder" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="lblFormOrder">Large Modal Title</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                    <div class="modal-body">
                        <fieldset id="Form-item-form " class="row g-3">
                            <asp:HiddenField ID="hdFormId" ClientIDMode="Static" runat="server" />
                            <div class="alert alert-default">Add one or more forms to the order.</div>
                            <fieldset>
                                <div class="col-md-6">
                                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormNumber" Text="Form #" />
                                    <asp:TextBox ID="txtFormNumber" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox><span class="field-note-block">Enter NA if no form number exists</span>
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtFormNumber"
                                        CssClass="label label-danger" ErrorMessage="Form Number is Required. Enter NA if there is none." />
                                </div>
                                <div class="col-md-6">
                                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFormName" Text="Exact Title of Form" />
                                    <asp:TextBox ID="txtFormName" ClientIDMode="Static" runat="server" MaxLength="200" CssClass="form-control"></asp:TextBox><span class="field-note-block">Tell us what it says on the bottom left-hand footer of form</span>
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtFormName"
                                        CssClass="label label-danger" ErrorMessage="Form Title is Required" />
                                </div>
                                <div class="col-md-6">
                                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtDescription" Text="Description" />
                                    <asp:TextBox ID="txtDescription" ClientIDMode="Static" TextMode="MultiLine" Rows="4" runat="server" MaxLength="2000" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtDescription"
                                        CssClass="label label-danger" ErrorMessage="Description is Required" />
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
                                    <asp:CompareValidator ID="valIsNumber" ValidationGroup="Form" CssClass="label label-danger" runat="server" ErrorMessage="The Value must be number only" ControlToValidate="txtQuantity" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtQuantity"
                                        CssClass="label label-danger" ErrorMessage="Quantity is Required" />
                                </div>
                                <div class="col-md-6">
                                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRecipient" Text="Recipient Name" />
                                    <asp:TextBox ID="txtRecipient" ClientIDMode="Static" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Form" ControlToValidate="txtRecipient"
                                        CssClass="label label-danger" ErrorMessage="Recipient is Required" />
                                </div>
                                <div class="col-md-12">
                                    <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtComments" Text="Comments" />
                                    <asp:TextBox ID="txtComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                </div>
                            </fieldset>
                            <p>
                                <asp:LinkButton ID="cmdAddForm" runat="server" ValidationGroup="Form" CssClass="btn btn-primary" Text="Save Form" OnClick="cmdAddForm_Click" />
                                <asp:HyperLink ID="lnkCancelLine" runat="server" CssClass="btn btn-secondary" Text="Cancel Form" />
                            </p>
                        </fieldset>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
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
        <div class="attachment-container">
            <asp:Repeater ID="rptFiles" runat="server">
                <HeaderTemplate>
                    <h3>Attachments</h3>
                    <div class="attachment-list">
                        <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href='/portals/0/<%# Eval("Path") %>'>
                        <%# Eval("FileName") %>
                    </a>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></div></FooterTemplate>
            </asp:Repeater>
        </div>
        <p>
            <asp:LinkButton ID="cmdSave" ClientIDMode="Static" runat="server" ValidationGroup="Order" CssClass="btn btn-primary" Text="Save Order" OnClick="cmdSave_Click" />
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </p>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {
        var table = $('#tblFormOrderLines').DataTable({
            "order": [[0, "desc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
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
    }(jQuery, window.Sys));
</script>
