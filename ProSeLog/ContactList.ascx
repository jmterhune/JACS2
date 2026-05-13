<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactList.ascx.cs" Inherits="tjc.Modules.ProSeLog.ContactList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=LogListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>

        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#contacts" data-toggle="tab">Contacts</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="contacts" class="tab-pane active">
            <asp:UpdatePanel ID="pnlContacts" runat="server" RenderMode="Block" OnUnload="pnlContacts_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Literal ID="ltMessage" runat="server" />
                    <asp:Repeater ID="rptContact" runat="server" OnItemCreated="rptContact_ItemCreated" OnItemCommand="rptContact_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblContact" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Contact</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ContactID").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td><%#Eval("ContactName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ContactID").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditContactModal" tabindex="-1" role="dialog" aria-labelledby="EditContactModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditContactModalLabel">Add / Edit Contact</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtContact" Text="Contact" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtContact" />
                                    </div>
                                    <asp:HiddenField ID="hdContactId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblContact').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditContactModal"><i class="fa fa-plus"></i>&nbsp;Add Contact</button>');
        table.on('draw', function () {
            $(".confirm").not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
                e.preventDefault();
                var href = this.href || '';
                Swal.fire({
                    title: 'Delete Contact?', text: 'Are you sure you wish to Delete the selected Contact?', icon: 'warning',
                    showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
                    confirmButtonColor: '#d33'
                }).then(function (r) {
                    if (r.isConfirmed) {
                        var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
                        if (m && typeof __doPostBack === 'function') __doPostBack(m[1], m[2]);
                    }
                });
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditContactModal').modal('show');
        } else {
            $('#EditContactModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtContact').val("");
        $('#hdContactId').val("");
        return false;
    }
</script>

