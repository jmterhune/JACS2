<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegionList.ascx.cs" Inherits="tjc.Modules.MediationStatistics.RegionList" %>
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
        <li class="nav-item active">
            <a class="nav-link" href="#regions" data-toggle="tab">Regions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=GroupListUrl %>">Case Type Groups</a>
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
        <div id="regions" class="tab-pane active">
            <asp:UpdatePanel ID="pnlRegions" runat="server" RenderMode="Block" OnUnload="pnlRegions_Unload">
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
                    <asp:Repeater ID="rptRegion" runat="server" OnItemCreated="rptRegion_ItemCreated" OnItemCommand="rptRegion_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblRegion" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Region</th>
                                        <th>Active</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RegionId").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td><%#Eval("Description")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Active").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RegionId").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditRegionModal" tabindex="-1" role="dialog" aria-labelledby="EditRegionModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditRegionModalLabel">Add / Edit Region</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtRegion" Text="Region" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtRegion" />
                                    </div>
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkActive" ClientIDMode="Static" runat="server" Text="Active" />
                                    </div>
                                    <asp:HiddenField ID="hdRegionId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblRegion').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#.dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditRegionModal"><i class="fa fa-plus"></i>&nbsp;Add Region</button>');
        table.on('draw', function () {
            $(".confirm").not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
                e.preventDefault();
                var href = this.href || '';
                Swal.fire({
                    title: 'Delete Region?', text: 'Are you sure you wish to Delete the selected Region?', icon: 'warning',
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
            $('#EditRegionModal').modal('show');
        } else {
            $('#EditRegionModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtRegion').val("");
        $('#chkActive').prop("checked", false);
        $('#hdRegionId').val("");
        return false;
    }
</script>

