<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupList.ascx.cs" Inherits="tjc.Modules.MediationStatistics.GroupList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl %>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=MediatorListUrl %>">Mediators</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RegionListUrl %>">Regions</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#groups" data-toggle="tab">Case Type Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AppearanceListUrl %>">Appearance Values</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=IssueListUrl %>">Issues</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ActionListUrl %>">Stage of Action Items</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="groups" class="tab-pane active">
            <asp:UpdatePanel ID="pnlGroups" runat="server" RenderMode="Block" OnUnload="pnlGroups_Unload">
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
                    <asp:Repeater ID="rptGroup" runat="server" OnItemCreated="rptGroup_ItemCreated" OnItemCommand="rptGroup_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblGroup" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th colspan="2">&nbsp;</th>
                                        <th>Case Type Group</th>
                                        <th>Court Ordered</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"GroupId").ToString() %>'><i class="fa fa-pencil" title="Edit Case Type Group"></i></asp:LinkButton>
                                </td>
                                <td class="command-item">
                                    <asp:HyperLink ID="lnkRelation" runat="server" NavigateUrl='<%#EditUrl("gid",DataBinder.Eval(Container.DataItem,"GroupId").ToString(),"GroupRelation") %>'><i class="fa fa-list ms-2" title="Set Case Type Group Associations"></i></asp:HyperLink>
                                <td><%#Eval("Description")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"CourtOrdered").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"GroupId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditGroupModal" tabindex="-1" role="dialog" aria-labelledby="EditGroupModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditGroupModalLabel">Add / Edit Case Type Group</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body groups">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtGroup" Text="Group" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtGroup" />
                                    </div>
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkCourtOrdered" ClientIDMode="Static" runat="server" Text="Court Ordered" />
                                    </div>
                                    <asp:HiddenField ID="hdGroupId" ClientIDMode="Static" runat="server" />
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
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblGroup').DataTable({
            "order": [[3, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblGroup_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditGroupModal"><i class="fa fa-plus"></i>&nbsp;Add Case Type Group</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Case Type Group?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Case Type Group?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditGroupModal').modal('show');
        } else {
            $('#EditGroupModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtGroup').val("");
        $('#chkCourtOrdered').prop("checked", false);
        $('#hdGroupId').val("");
        return false;
    }
</script>

