<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordTypeList.ascx.cs" Inherits="tjc.Modules.RecordDestruction.RecordTypeList" %>
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
        <li class="nav-item active">
            <a class="nav-link" href="#recordTypes" data-toggle="tab">Record Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RetentionPeriodListUrl %>">Retention Periods</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DestructionMethodListUrl %>">Destruction Methods</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="recordTypes" class="tab-pane active">
            <asp:UpdatePanel ID="pnlRecordTypes" runat="server" RenderMode="Block" OnUnload="pnlRecordTypes_Unload">
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
                    <asp:Repeater ID="rptRecordTypes" runat="server" OnItemCommand="rptRecordTypes_ItemCommand" OnItemCreated="rptRecordTypes_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblRecordType" class="table table-striped">
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
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RecordTypeID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td class="command-item"><%#Eval("RecordTypeID")%></td>
                                <td><%#Eval("Description")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RecordTypeID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditRecordTypeModal" tabindex="-1" role="dialog" aria-labelledby="EditRecordTypeModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditRecordTypeModalLabel">Add / Edit Record Types</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtRecordType" Text="Record Type" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtRecordType" />
                                    </div>
                                    <asp:HiddenField ID="hdRecordTypeId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblRecordType').DataTable({
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
        $("#tblRecordType_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditRecordTypeModal"><i class="fa fa-plus"></i>&nbsp;Add Record Type</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Record Type?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Record Type?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditRecordTypeModal').modal('show');
        } else {
            $('#EditRecordTypeModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtRecordType').val("");
        $('#hdRecordTypeId').val("");
        return false;
    }
</script>

