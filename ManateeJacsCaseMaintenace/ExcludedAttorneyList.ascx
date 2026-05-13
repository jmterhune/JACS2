<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExcludedAttorneyList.ascx.cs" Inherits="tjc.Modules.JacsCaseMaint.ExcludedAttorneyList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div class="alert alert-info"><i class="fa fa-info-circle"></i>&nbsp;The list below displays the barnumbers that have been excluded from the Florida Bar Import routine.</div>

<button class="btn btn-primary" data-toggle="modal" data-target="#formModal">Add Attorney to Exclude </button>
<hr />

<asp:Repeater ID="rptAttorneyList" runat="server" OnItemCommand="rptAttorneyList_ItemCommand">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Bar Number</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Active</th>
                    <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="command-icon-container"><%#Eval("RecordId") %></td>
            <td><%#Eval("barnumber") %></td>
            <td><%#Eval("NAME") %></td>
            <td><%#Eval("EMAIL") %></td>
            <td><%#Eval("ACTIVE") %></td>
            <td class="command-icon-container">
                <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDelete" ToolTip="Delete Record" CssClass="command-icon text-danger" OnClientClick="return Jud12ConfirmPostback(this, 'Delete Bar Number?', 'Delete?');" CommandArgument='<%#Eval("RecordId") %>' CommandName="delete"><i class="fas fa-trash"></i></asp:LinkButton>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>
<div class="modal fade" id="formModal" tabindex="-1" role="dialog" aria-labelledby="formModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="formModalLabel">Add Bar Number to Exclude</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group mt-lg">
                    <div class="row">
                        <div class="col-sm-12">
                            <asp:Label ID="lblBarnumber" runat="server" Text="Bar Number" AssociatedControlID="txtBarNumber"></asp:Label>
                            <asp:TextBox runat="server" ID="txtBarNumber" CssClass="form-control" MaxLength="10" />
                            <asp:RequiredFieldValidator ErrorMessage="Bar Number is Required" ControlToValidate="txtBarNumber" CssClass="label label-danger" Display="Dynamic" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <asp:Button ID="cmdSave" CssClass="btn btn-primary" runat="server" OnClick="cmdSave_Click" Text="Save" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && btn.tagName === 'INPUT' && (btn.type === 'submit' || btn.type === 'button')) {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>

