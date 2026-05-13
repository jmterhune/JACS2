<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DestructionMethodList.ascx.cs" Inherits="tjc.Modules.RecordDestruction.DestructionMethodList" %>
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
            <a class="nav-link" href="<%=DestructionFormURL %>">Record Destruction Log</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SearchLogUrl %>">Search Log</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentListUrl %>">Departments</a>
        </li>
        <li class="nav-item">
           <a class="nav-link" href="<%=RecordTypeListUrl %>">Record Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RetentionPeriodListUrl %>">Retention Periods</a>
        </li>
        <li class="nav-item active">
            
            <a class="nav-link" href="#destructionMethods" data-toggle="tab">Destruction Methods</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="destructionMethods" class="tab-pane active">
            <asp:UpdatePanel ID="pnlDestructionMethods" runat="server" RenderMode="Block" OnUnload="pnlDestructionMethods_Unload">
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
                    <asp:Repeater ID="rptDestructionMethods" runat="server" OnItemCommand="rptDestructionMethods_ItemCommand" OnItemCreated="rptDestructionMethods_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblDestructionMethod" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Description</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"DestructionMethodID").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td class="command-item"><%#Eval("DestructionMethodID")%></td>
                                <td><%#Eval("Description")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"DestructionMethodID").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditDestructionMethodModal" tabindex="-1" role="dialog" aria-labelledby="EditDestructionMethodModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditDestructionMethodModalLabel">Add / Edit Destruction Methods</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtDestructionMethod" Text="Destruction Method" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtDestructionMethod" />
                                    </div>
                                    <asp:HiddenField ID="hdDestructionMethodId" ClientIDMode="Static" runat="server" />
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
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblDestructionMethod').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditDestructionMethodModal"><i class="fa fa-plus"></i>&nbsp;Add Destruction Method</button>');
        table.on('draw', function () {
            $(".confirm").not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
                e.preventDefault();
                var href = this.href || '';
                Swal.fire({
                    title: 'Delete Destruction Method?', text: 'Are you sure you wish to Delete the selected Destruction Method?', icon: 'warning',
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
            $('#EditDestructionMethodModal').modal('show');
        } else {
            $('#EditDestructionMethodModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtDestructionMethod').val("");
        $('#hdDestructionMethodId').val("");
        return false;
    }
</script>

