<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NameList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.NameList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CalendartUrl%>">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#names" data-toggle="tab">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormListUrl%>">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=HearingListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>        <li class="nav-item">
    <a class="nav-link" href="https://jud12fl.sharepoint.com/sites/CourtReporting">Team Site</a>
</li>
    </ul>
    <div class="tab-content pb-0">
        <div id="names" class="tab-pane active">
            <asp:UpdatePanel ID="pnlNames" runat="server" RenderMode="Block" OnUnload="pnlNames_Unload">
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
                    <asp:Repeater ID="rptName" runat="server" OnItemCreated="rptName_ItemCreated" OnItemCommand="rptName_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblName" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Title</th>
                                        <th>Type</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"EmployeeID").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td><%#Eval("EmployeeID")%></td>
                                    <td><%#Eval("FirstName")%></td>
                                <td><%#Eval("LastName")%></td>
                                <td><%#Eval("Title")%></td>
                                <td><%#Eval("EmployeeTypeName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"EmployeeID").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                 </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditNameModal" tabindex="-1" role="dialog" aria-labelledby="EditNameModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditNameModalLabel">Add / Edit Name</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="name" CssClass="label label-danger"
                                                    ErrorMessage="First Name Is Required" ControlToValidate="txtFirstName" runat="server" />

                                            </div>

                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="name" CssClass="label label-danger"
                                                    ErrorMessage="Last Name Is Required" ControlToValidate="txtLastName" runat="server" />

                                            </div>

                                        </div>
                                        <div class="row">
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtTitle" Text="Title" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtTitle" />
                                            </div>
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="drpNameType" Text="Name Type" />
                                                <asp:DropDownList runat="server" ID="drpNameType" CssClass="form-control">
                                                    <asp:ListItem Value="" Text="< Select Type >" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="name" CssClass="label label-danger"
                                                    ErrorMessage="Name Type Is Required" ControlToValidate="drpNameType" runat="server" />

                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdNameId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ValidationGroup="name" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
    var isAdmin = "<%=IsAdmin%>";
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        var table = $('#tblName').DataTable({
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
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditNameModal"><i class="fa fa-plus"></i>&nbsp;Add Name</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Name?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Name?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditNameModal').modal('show');
        } else {
            $('#EditNameModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtFirstName').val("");
        $('#txtLastName').val("");
        $('#txtTitle').val("");
        $('#drpNameType').val("0");
        $('#hdNameId').val("");
        return false;
    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
