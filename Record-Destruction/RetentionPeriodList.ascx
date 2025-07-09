<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RetentionPeriodList.ascx.cs" Inherits="tjc.Modules.RecordDestruction.RetentionPeriodList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

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
        <li class="nav-item  active">
            <a class="nav-link" href="#retentionPeriods" data-toggle="tab">Retention Periods</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DestructionMethodListUrl %>">Destruction Methods</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="retentionPeriods" class="tab-pane active">
            <asp:UpdatePanel ID="pnlRetentionPeriods" runat="server" RenderMode="Block" OnUnload="pnlRetentionPeriods_Unload">
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
                    <asp:Repeater ID="rptRetentionPeriods" runat="server" OnItemCommand="rptRetentionPeriods_ItemCommand" OnItemCreated="rptRetentionPeriods_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblRetentionPeriod" class="table table-striped">
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
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RetentionPeriodID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td class="command-item"><%#Eval("RetentionPeriodID")%></td>
                                <td><%#Eval("Description")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RetentionPeriodID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditRetentionPeriodModal" tabindex="-1" role="dialog" aria-labelledby="EditRetentionPeriodModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditRetentionPeriodModalLabel">Add / Edit Retention Periods</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtRetentionPeriod" Text="Retention Period" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtRetentionPeriod" />
                                    </div>
                                    <asp:HiddenField ID="hdRetentionPeriodId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblRetentionPeriod').DataTable({
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
        $("#tblRetentionPeriod_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditRetentionPeriodModal"><i class="fa fa-plus"></i>&nbsp;Add Retention Period</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Retention Period?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Retention Period?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditRetentionPeriodModal').modal('show');
        } else {
            $('#EditRetentionPeriodModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtRetentionPeriod').val("");
        $('#hdRetentionPeriodId').val("");
        return false;
    }
</script>

