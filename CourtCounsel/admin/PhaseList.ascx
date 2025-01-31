<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PhaseList.ascx.cs" Inherits="tjc.Modules.CourtCounsel.PhaseList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=MemberListUrl %>">Judges & Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#phases" data-toggle="tab">Extended Statuses</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=TimeSpanListUrl %>">Time Spans</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ActionListUrl %>">Actions</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="phases" class="tab-pane active">
            <asp:UpdatePanel ID="pnlPhases" runat="server" RenderMode="Block" OnUnload="pnlPhases_Unload">
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
                    <asp:Repeater ID="rptPhase" runat="server" OnItemCreated="rptPhase_ItemCreated" OnItemCommand="rptPhase_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblPhase" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Extended Status</th>
                                        <th>Group Name</th>
                                        <th>Group Order</th>
                                        <th>Is Future Status</th>
                                        <th>Active</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhaseId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("PhaseName")%></td>
                                <td><%#Eval("GroupName")%></td>
                                <td><%#Eval("GroupIndex")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"IsPending").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Active").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhaseId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditPhaseModal" tabindex="-1" role="dialog" aria-labelledby="EditPhaseModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditPhaseModalLabel">Add / Edit Extended Status</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtPhaseName" Text="Extended Status" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtPhaseName" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtGroupName" Text="Group Name" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtGroupName" />
                                        <div id="groupNameHelp" class="form-text">Use Existing Name or add New Group.</div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtGroupIndex" Text="Group Index" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" ID="txtGroupIndex" />
                                        <div id="groupIndexHelp" class="form-text">Ensure that the index matches existing group index</div>

                                    </div>
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkPending" ClientIDMode="Static" runat="server" Text="Future Status" />
                                    </div>
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkActive" ClientIDMode="Static" runat="server" Text="Active" />
                                    </div>
                                    <asp:HiddenField ID="hdPhaseId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblPhase').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblPhase_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditPhaseModal"><i class="fa fa-plus"></i>&nbsp;Add Extended Status</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Extended Status?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Extended Status?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditPhaseModal').modal('show');
        } else {
            $('#EditPhaseModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtPhaseName').val("");
        $('#txtGroupName').val("");
        $('#txtGroupIndex').val("");
        $('#chkActive').prop("checked", false);
        $('#chkPending').prop("checked", false);
        $('#hdPhaseId').val("");
        return false;
    }
</script>

