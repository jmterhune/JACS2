<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RaceView.ascx.cs" Inherits="tjc.Modules.EmployeeDB.RaceView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-user"></i>&nbsp;Employees</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentUrl%>"><i class="fas fa-sitemap"></i>&nbsp;Departments</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobGroupUrl%>"><i class="fas fa-users"></i>&nbsp;Job Categories</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobClassUrl%>"><i class="fas fa-user-tag"></i>&nbsp;Job Classes</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=RaceUrl%>"><i class="fas fa-users-cog"></i>&nbsp;Race</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CountyUrl%>"><i class="fas fa-map-marked-alt"></i>&nbsp;Counties</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationUrl%>"><i class="fas fa-building"></i>&nbsp;Locations</a>
        </li>

    </ul>
    <div class="tab-content">
        <div id="Races" class="tab-pane active">
            <asp:UpdatePanel ID="pnlRaces" runat="server" RenderMode="Block" OnUnload="pnlRaces_Unload">
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

                    <asp:Repeater ID="rptRaces" runat="server" OnItemCommand="rptRaces_ItemCommand" OnItemCreated="rptRaces_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblRaces" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Race</th>
                                        <th>Race Code</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RaceId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"RaceCode") %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RaceId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditRaceModal" tabindex="-1" role="dialog" aria-labelledby="EditRaceModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditRaceModalLabel">Add / Edit Race Entry</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-race">
                                        <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Race Name<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtDescription" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Race Name is Required" />
                                    </div>
                                    <div class="form-race">
                                        <asp:Label runat="server" AssociatedControlID="txtRaceCode" Text="Race Code<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtRaceCode" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRaceCode"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Race Code is Required" />
                                        <asp:HiddenField ID="hdRaceId" runat="server" />
                                    </div>

                                </div>
                                <div class="modal-footer">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
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

<dnn:dnnjsInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />

<script type="text/javascript">

    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();

        });
    }(jQuery, window.Sys));
    function PageInit() {
        var table = $('#tblRaces').DataTable({

            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
            ]
        });
        $("#tblRaces_length").prepend('<button class="btn btn-primary btn-lg me-2" data-bs-toggle="modal" data-bs-target="#EditRaceModal"><i class="fa fa-plus"></i>&nbsp;Add Race Entry</button>');
        table.draw();

        $(".confirm").dnnConfirm({

            text: 'Are you sure you wish to delete this Race Entry?',

            yesText: 'Yes',

            noText: 'No',

            title: 'Delete Race Entry?'

        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditRaceModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }

            if (Page_IsValid) {
                $('#EditRaceModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }

        return true;
    }
</script>
