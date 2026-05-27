<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OfficeList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.OfficeList" %>
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
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#offices" data-toggle="tab">Offices</a>
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
        <div id="offices" class="tab-pane active">
            <asp:UpdatePanel ID="pnlOffices" runat="server" RenderMode="Block" OnUnload="pnlOffices_Unload">
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
                    <asp:Repeater ID="rptOffice" runat="server" OnItemCreated="rptOffices_ItemCreated" OnItemCommand="rptOffices_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblOffices" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Office Name</th>
                                        <th>Delivery Type</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"OfficeID").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td><%#Eval("Description")%></td>
                                <td><%#Eval("DeliveryTypeeName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"OfficeID").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditOfficeModal" tabindex="-1" role="dialog" aria-labelledby="EditOfficeModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditOfficeModalLabel">Add / Edit Office</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Office Description" />
                                                <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtDescription" />
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="office" CssClass="label label-danger"
                                                    ErrorMessage="Office Description Is Required" ControlToValidate="txtDescription" runat="server" />

                                            </div>
                                            <div class="col-auto">
                                                <asp:Label runat="server" AssociatedControlID="drpDeliveryType" Text="Delivery Type" />
                                                <asp:DropDownList runat="server" ID="drpDeliveryType" CssClass="form-control">
                                                    <asp:ListItem Value="" Text="< Delivery Type >" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="office" CssClass="label label-danger"
                                                    ErrorMessage="Delivery Type Is Required" ControlToValidate="drpDeliveryType" runat="server" />

                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdOfficeId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ValidationGroup="office" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<script type="text/javascript">
    var isAdmin = "<%=IsAdmin%>";
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        var table = $('#tblOffice').DataTable({
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
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditOfficeModal"><i class="fa fa-plus"></i>&nbsp;Add Office</button>');
        table.on('draw', function () {
            $(".confirm").not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
                e.preventDefault();
                var href = this.href || '';
                Swal.fire({
                    title: 'Delete Office?',
                    text: 'Are you sure you wish to Delete the selected Office?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No',
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
            $('#EditOfficeModal').modal('show');
        } else {
            $('#EditOfficeModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtDescription').val("");
        $('#drpDeliveryType').val("0");
        $('#hdOfficeId').val("");
        return false;
    }

    function ShowAlert(title, text) {
        Swal.fire({
            title: title,
            html: text,
            icon: 'info',
            confirmButtonText: 'OK'
        });
    }
</script>
