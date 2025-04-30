<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HearingTypeList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.HearingTypeList" %>
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
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormListUrl%>">Forms</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#hearings" data-toggle="tab">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="hearings" class="tab-pane active">
            <asp:UpdatePanel ID="pnlHearings" runat="server" RenderMode="Block" OnUnload="pnlHearings_Unload">
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
                    <asp:Repeater ID="rptHearing" runat="server" OnItemCreated="rptHearing_ItemCreated" OnItemCommand="rptHearing_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblHearingType" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Hearing Type</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"HearingTypeID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("HearingTypeName")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"HearingTypeID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditHearingTypeModal" tabindex="-1" role="dialog" aria-labelledby="EditHearingTypeModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditHearingTypeModalLabel">Add / Edit Hearing Type</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtHearingType" Text="Hearing Type" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtHearingType" />
                                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="hearing" CssClass="label label-danger"
                                            ErrorMessage="Hearing Is Required" ControlToValidate="txtHearingType" runat="server" />

                                    </div>
                                    <asp:HiddenField ID="hdHearingTypeId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ValidationGroup="hearing" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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

<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    var isAdmin = "<%=IsAdmin%>";
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        var table = $('#tblHearingType').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblHearingType_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditHearingTypeModal"><i class="fa fa-plus"></i>&nbsp;Add Hearing Type</button>');       
    }
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to Delete the Hearing Type?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Hearing Type?'
        });
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditHearingTypeModal').modal('show');
        } else {
            $('#EditHearingTypeModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtHearingType').val("");
        $('#hdHearingTypeId').val("");
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
