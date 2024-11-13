<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocationView.ascx.cs" Inherits="tjc.Modules.EmployeeDB.LocationView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-id-badge"></i>&nbsp;Employees</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ContactUrl%>"><i class="fas fa-user"></i>&nbsp;Contacts</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentUrl%>"><i class="fas fa-sitemap"></i>&nbsp;Department</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobGroupUrl%>"><i class="fas fa-users"></i>&nbsp;Job Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobClassUrl%>"><i class="fas fa-user-tag"></i>&nbsp;Job Classes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RaceUrl%>"><i class="fas fa-users-cog"></i>&nbsp;Race</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CountyUrl%>"><i class="fas fa-map-marked-alt"></i>&nbsp;Counties</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=LocationUrl%>"><i class="fas fa-building"></i>&nbsp;Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SwnLogUrl%>"><i class="fas fa-exclamation-circle"></i>&nbsp;SWN Interface Log</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="OfficeLocations" class="tab-pane active">

            <asp:UpdatePanel ID="pnlOfficeLocations" runat="server" RenderMode="Block" OnUnload="pnlOfficeLocations_Unload">
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

                    <asp:Repeater ID="rptOfficeLocations" runat="server" OnItemCommand="rptOfficeLocations_ItemCommand" OnItemCreated="rptOfficeLocations_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblOfficeLocations" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Location</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"OfficeLocationId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"OfficeLocationId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditOfficeLocationModal" tabindex="-1" role="dialog" aria-labelledby="EditOfficeLocationModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditOfficeLocationModalLabel">Add / Edit Location</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Location<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtDescription" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Location is Required" />
                                        <asp:HiddenField ID="hdOfficeLocationId" runat="server" />
                                    </div>
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

<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />

<script type="text/javascript">

    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();

        });
    }(jQuery, window.Sys));
    function PageInit() {
        var table = $('#tblOfficeLocations').DataTable({

            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false }]

        });
        $("#tblOfficeLocations_length").prepend('<button class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditOfficeLocationModal"><i class="fa fa-plus"></i>&nbsp;Add Location</button>');
        table.draw();

        $(".confirm").dnnConfirm({

            text: 'Are you sure you wish to delete this Location?',

            yesText: 'Yes',

            noText: 'No',

            title: 'Delete Location?'

        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditOfficeLocationModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }

            if (Page_IsValid) {
                $('#EditOfficeLocationModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }

        return true;
    }
</script>
