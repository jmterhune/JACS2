<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MemberList.ascx.cs" Inherits="tjc.Modules.CourtCounsel.MemberList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#members" data-toggle="tab">Judges & Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>
        </li>
        <li class="nav-item ">
            <a class="nav-link" href="<%=PhasesListUrl %>">Extended Statuses</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=TimeSpanListUrl %>">Time Spans</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ActionListUrl %>">Actions</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="members" class="tab-pane active">
            <asp:UpdatePanel ID="pnlMembers" runat="server" RenderMode="Block" OnUnload="pnlMembers_Unload">
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
                    <asp:Repeater ID="rptMember" runat="server" OnItemCreated="rptMember_ItemCreated" OnItemCommand="rptMember_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblMember" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>UserName</th>
                                        <th>Member Type</th>
                                        <th>Active</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"MemberId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("FullName")%></td>
                                <td><%#Eval("Email")%></td>
                                <td><%#Eval("UserName")%></td>
                                <td><%#Eval("MemberType")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Active").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"MemberId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditMemberModal" tabindex="-1" role="dialog" aria-labelledby="EditMemberModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditMemberModalLabel">Add / Edit Member</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="First Name is Required" />

                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Last Name is Required" />

                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtEmail" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Email is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtUserName" Text="Username" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtUserName" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpMember" Text="Member Type" />
                                        <asp:DropDownList runat="server" ID="drpMember" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Text="< Select Member Type >" Value="" />
                                            <asp:ListItem Text="Judge" Value="0" />
                                            <asp:ListItem Text="Attorney" Value="1" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpMember"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Member Type is Required" />
                                    </div>
                                    <div class="form-check form-switch">
                                        <asp:CheckBox ID="chkActive" ClientIDMode="Static" runat="server" Text="Active" />
                                    </div>
                                    <asp:HiddenField ID="hdMemberId" ClientIDMode="Static" runat="server" />
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
        var table = $('#tblMember').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditMemberModal"><i class="fa fa-plus"></i>&nbsp;Add Member</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Member?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Member?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditMemberModal').modal('show');
        } else {
            $('#EditMemberModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtFirstName').val("");
        $('#txtLastName').val("");
        $('#txtEmail').val("");
        $('#txtUserName').val("");
        $('#drpMember').val("");
        $('#chkActive').prop("checked", false);
        $('#hdMemberId').val("");
        return false;
    }
</script>

