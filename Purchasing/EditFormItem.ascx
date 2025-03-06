<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFormItem.ascx.cs" Inherits="tjc.Modules.Purchasing.EditFormItem" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" Text="Manage Orders" CssClass="SubHead" runat="server" />
<div class="form-order-container purchasing">
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

        <div class="bg-light ps-3 pe-3 rounded">
            <asp:Repeater ID="rptForms" runat="server" OnItemCommand="rptForms_ItemCommand" OnItemDataBound="rptForms_ItemDataBound">
                <HeaderTemplate>
                    <div class="heading heading-border heading-bottom-border">
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
                                <th>Attachments</th>
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
                            <ul class="list-unstyled ps-0 mb-0">
                                <asp:Literal ID="ltAttachments" runat="server" />
                            </ul>
                        </td>
                        <td>
                            <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDeleted" CssClass="confirm" CommandName="delete" CommandArgument='<%#Eval("FormId") %>'><i class="fa fa-trash"></i></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <p>
            <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Return" />
        </p>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $('.confirm').dnnConfirm({
            text: 'Are you Sure you wish to delete this record?',
            title: 'Delete Record?'
        });
    }(jQuery, window.Sys));

    function CloseModal() {
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate("Form");
        }
        if (Page_IsValid) {
            $('#modFormOrder').modal('hide');
        }
    }
</script>
