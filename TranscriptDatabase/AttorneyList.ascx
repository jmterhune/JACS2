<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttorneyList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.AttorneyList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CalendartUrl%>">Calendar</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#attorneys" data-toggle="tab">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
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
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="attorneys" class="tab-pane active">
            <asp:UpdatePanel ID="pnlAttorney" runat="server" RenderMode="Block" OnUnload="pnlAttorney_Unload">
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
                    <asp:Repeater ID="rptAttorney" runat="server" OnItemCreated="rptAttorney_ItemCreated" OnItemCommand="rptAttorney_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblAttorney" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>First Name</th>
                                        <th>Middle Name</th>
                                        <th>Last Name</th>
                                        <th>Office</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttorneyID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("FirstName")%></td>
                                <td><%#Eval("MiddleName")%></td>
                                <td><%#Eval("LastName")%></td>
                                <td><%#Eval("OfficeName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttorneyID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditAttorneyModal" tabindex="-1" role="dialog" aria-labelledby="EditAttorneyModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditAttorneyModalLabel">Add / Edit Attorney</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-4">
                                                <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="atty" CssClass="label label-danger"
                                                    ErrorMessage="First Name Is Required" ControlToValidate="txtFirstName" runat="server" />

                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" AssociatedControlID="txtMiddleName" Text="Middle Name" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtMiddleName" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="atty" CssClass="label label-danger"
                                                    ErrorMessage="Last Name Is Required" ControlToValidate="txtLastName" runat="server" />
                                            </div>

                                        </div>
                                        <div class="row">
                                            <div class="col-12">
                                                <asp:Label runat="server" AssociatedControlID="drpOffice" Text="Office Location" />
                                                <asp:DropDownList runat="server" ID="drpOffice" CssClass="form-control" AppendDataBoundItems="true">
                                                    <asp:ListItem Value="0" Text="< Select Office Location >" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">

                                            <div class="col-12">
                                                <label for="txtAddress" class="form-label">Address</label>
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress" placeholder="1234 Main St" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtAddress2" class="form-label">Address 2</label>
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress2" placeholder="Apartment, studio, or floor" />
                                            </div>
                                            <div class="col-md-6">
                                                <label for="txtCity" class="form-label">City</label>
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCity" />
                                            </div>
                                            <div class="col-md-4">
                                                <label for="drpState" class="form-label">State</label>
                                                <asp:DropDownList runat="server" ID="drpState" CssClass="form-control" ClientIDMode="Static" AppendDataBoundItems="true">
                                                    <asp:ListItem Value="" Text="< Select State >" />
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <label for="txtZip" class="form-label">Zip</label>
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="10" ID="txtZip" />
                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdAttorneyId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" ValidationGroup="atty" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
        var table = $('#tblAttorney').DataTable({
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
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblAttorney_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditAttorneyModal"><i class="fa fa-plus"></i>&nbsp;Add Attorney</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Attorney?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Attorney?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditAttorneyModal').modal('show');
        } else {
            $('#EditAttorneyModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtFirstName').val("");
        $('#txtMiddleName').val("");
        $('#txtLastName').val("");
        $('#drpOffice').val("");
        $('#txtAddress').val("");
        $('#txtAddress2').val("");
        $('#txtCity').val("");
        $('#drpState').val("");
        $('#txtZip').val("");
        $('#hdAttorneyId').val("");
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
