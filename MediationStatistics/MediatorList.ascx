<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MediatorList.ascx.cs" Inherits="tjc.Modules.MediationStatistics.MediatorList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl %>">Attorneys</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#mediators" data-toggle="tab">Mediators</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RegionListUrl %>">Regions</a>
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
            <a class="nav-link" href="#<%=ActionListUrl %>">Stage of Action Items</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="mediators" class="tab-pane active">
            <asp:UpdatePanel ID="pnlMediators" runat="server" RenderMode="Block" OnUnload="pnlMediators_Unload">
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
                    <asp:Repeater ID="rptMediator" runat="server" OnItemCreated="rptMediator_ItemCreated" OnItemCommand="rptMediator_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblMediator" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Phone</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"MediatorId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("MediatorName")%></td>
                                <td><%#Eval("Email") %></td>
                                <td><%#Eval("Phone") %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"MediatorId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditMediatorModal" tabindex="-1" role="dialog" aria-labelledby="EditMediatorModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditMediatorModalLabel">Add / Edit Mediator</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row g-3">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Phone" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="50" ID="txtPhone" />
                                        </div>
                                    </div>
                                    <div class="row g-3">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email Address" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="250" ID="txtEmail" />
                                            <asp:RegularExpressionValidator ID="valEmail" runat="server"
                                                ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic" ControlToValidate="txtEmail"
                                                ErrorMessage="Invalid Email Address Format" SetFocusOnError="true" CssClass="label label-danger" />

                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdMediatorId" ClientIDMode="Static" runat="server" />
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
        <div id="regions" class="tab-pane">
        </div>
        <div id="groups" class="tab-pane">
        </div>
        <div id="types" class="tab-pane">
        </div>
        <div id="appearances" class="tab-pane">
        </div>
        <div id="issues" class="tab-pane">
        </div>
        <div id="actions" class="tab-pane">
        </div>
        <div id="type-group" class="tab-pane">
        </div>
        <div id="appearance-group" class="tab-pane">
        </div>
        <div id="issue-group" class="tab-pane">
        </div>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jquery/jquery.mask.js" />
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
        $('.phone').mask('(000) 000-0000');
        var table = $('#tblMediator').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblMediator_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditMediatorModal"><i class="fa fa-plus"></i>&nbsp;Add Mediator</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Mediator?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Mediator?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditMediatorModal').modal('show');
        } else {
            $('#EditMediatorModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtFirstName').val("");
        $('#txtLastName').val("");
        $('#txtEmail').val("");
        $('#txtPhone').val("");
        $('#hdMediatorId').val("");
        return false;
    }
</script>

