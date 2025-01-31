<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmergencyContacts.ascx.cs" Inherits="tjc.Modules.EmployeeDB.EmergencyContacts" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltMessage" Visible="false" runat="server"><div class="alert alert-warning"><i class="fa fa-warning"></i> {0}</div></asp:Literal>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl %>"><i class="fas fa-list"></i>&nbsp;Back to  List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>"><i class="fas fa-user-edit"></i>&nbsp;Details</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=PhoneUrl%>"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>?g=groups"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=EmploymentUrl%>"><i class="fas fa-user-clock"></i>&nbsp;Employment History</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=EmergencyContactUrl%>"><i class="fas fa-address-book"></i>&nbsp;Emergency Contacts</a>
        </li>
    </ul>
    <div class="tab-content edit-form">
        <div id="emergencyContact" class="tab-pane active">
            <asp:UpdatePanel ID="pnlEmergencyContact" runat="server" RenderMode="Block" OnUnload="pnlEmergencyContact_Unload">
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
                    <asp:Repeater ID="rptEmergencyContact" runat="server" OnItemCommand="rptEmergencyContact_ItemCommand" OnItemCreated="rptEmergencyContact_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblEmergencyContact" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Relationship</th>
                                        <th>Home Phone</th>
                                        <th>Work Phone</th>
                                        <th>Mobile Phone</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ContactId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"FirstName") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"LastName") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Relationship") %></td>
                                <td><a href='tel:<%#DataBinder.Eval(Container.DataItem,"PhoneHome") %>'><%#DataBinder.Eval(Container.DataItem,"PhoneHomeFormatted") %></a></td>
                                <td><a href='tel:<%#DataBinder.Eval(Container.DataItem,"PhoneWork") %>'><%#DataBinder.Eval(Container.DataItem,"PhoneWorkFormatted") %></a></td>
                                <td><a href='tel:<%#DataBinder.Eval(Container.DataItem,"PhoneMobile") %>'><%#DataBinder.Eval(Container.DataItem,"PhoneMobileFormatted") %></a></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ContactId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditEmergencyContactModal" tabindex="-1" role="dialog" aria-labelledby="EditEmergencyContactModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditEmergencyContactModalLabel">Add / Edit Position History</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtFirstName" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtLastName" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtRelationship" Text="Relationship" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtRelationship" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtHomePhone" Text="Home Phone" />
                                        <asp:TextBox runat="server" CssClass="form-control phone" MaxLength="20" ID="txtHomePhone" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtWorkPhone" Text="Work Phone" />
                                        <asp:TextBox runat="server" CssClass="form-control phone" MaxLength="20" ID="txtWorkPhone" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtMobilePhone" Text="Mobile Phone" />
                                        <asp:TextBox runat="server" CssClass="form-control phone" MaxLength="20" ID="txtMobilePhone" ClientIDMode="Static" />
                                    </div> <asp:HiddenField ID="hdEmergencyContactId" runat="server" ClientIDMode="Static" />
                                </div>
                                <div class="modal-footer justify-content-between" >                               
                                    <asp:Button OnClientClick="ToggleForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));
    function PageInit() {
        $('.phone').mask('(000) 000-0000');
        var table = $('#tblEmergencyContact').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
            ]
        });
        $("#tblEmergencyContact_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditEmergencyContactModal"><i class="fa fa-plus"></i>&nbsp;Add Contact</button>');
        table.draw();

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Contact?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Contact?'
        });
    }
    function ClearForm() {
        $('#txtFirstName').val("");
        $('#txtLastName').val("");
        $('#txtRelationship').val("");
        $('#txtHomePhone').val("");
        $('#txtWorkPhone').val("");
        $('#txtMobilePhone').val("");
        $('#hdEmergencyContactId').val("");
        return false;
    }
    function ToggleForm(toggleValue) {
        if (toggleValue) {
            $('#EditEmergencyContactModal').modal('show');
        }
        else {
            $('#EditEmergencyContactModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
</script>
