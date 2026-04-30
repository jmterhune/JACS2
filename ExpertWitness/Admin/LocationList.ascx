<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocationList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.LocationList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=RequestListUrl %>">Requests</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExpertListUrl %>">Experts</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=EvaluationTypeListUrl %>">Evaluation Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#locations" data-toggle="tab">Locations</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="locations" class="tab-pane active">
            <asp:UpdatePanel ID="pnlLocations" runat="server" RenderMode="Block" OnUnload="pnlLocations_Unload">
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
                    <asp:Repeater ID="rptLocation" runat="server" OnItemCreated="rptLocation_ItemCreated" OnItemCommand="rptLocation_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblLocation" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th class="w-100">Location</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"LocationID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("LocationID")%></td>
                                <td><%#Eval("LocationName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"LocationID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditLocationModal" tabindex="-1" role="dialog" aria-labelledby="EditLocationModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditLocationModalLabel">Add / Edit Location</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtLocationName" Text="Location" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLocationName" />
                                    </div>
                                    <asp:HiddenField ID="hdLocationId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblLocation').DataTable({
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
        $("#.dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditLocationModal"><i class="fa fa-plus"></i>&nbsp;Add Location</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Location?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Location?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditLocationModal').modal('show');
        } else {
            $('#EditLocationModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtLocationName').val("");
        $('#hdLocationId').val("");
        return false;
    }
</script>

