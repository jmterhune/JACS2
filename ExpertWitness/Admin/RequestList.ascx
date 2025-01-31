<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.RequestList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#requests" data-toggle="tab">Requests</a>
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
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl %>">Locations</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="requests" class="tab-pane active">
            <asp:UpdatePanel ID="pnlRequests" runat="server" RenderMode="Block" OnUnload="pnlRequests_Unload">
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
                    <asp:Repeater ID="rptRequest" runat="server" OnItemCreated="rptRequest_ItemCreated" OnItemCommand="rptRequest_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblRequest" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Case Number</th>
                                        <th>Template Type</th>
                                        <th>Location</th>
                                        <th>Submitted By</th>
                                        <th>Date Submitted</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RequestID").ToString() %>'><i class="fa fa-search"></i></asp:LinkButton>
                                <td><%#Eval("RequestID")%></td>
                                <td><%#Eval("CaseNumber")%></td>
                                <td><%#Eval("TemplateName")%></td>
                                <td><%#Eval("LocationName")%></td>
                                <td><%#Eval("CreatedBy")%></td>
                                <td><%#Eval("CreatedDate","{0:MM/dd/yyyy}")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RequestID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="ShowRequestModal" tabindex="-1" role="dialog" aria-labelledby="ShowRequestModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="ShowRequestModalLabel">View Request</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdRequestId" ClientIDMode="Static" runat="server" />
                                    <div class="row mb-2">
                                        <div class="col-6">
                                            <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number" />
                                            <asp:TextBox runat="server" ReadOnly="true" ClientIDMode="Static" CssClass="form-control" ID="txtCaseNumber" />
                                        </div>
                                        <div class="col-6">
                                            <asp:Label runat="server" AssociatedControlID="txtLocation" Text="Location" />
                                            <asp:TextBox runat="server" ReadOnly="true" ClientIDMode="Static" CssClass="form-control" ID="txtLocation" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:Label runat="server" AssociatedControlID="txtTemplate" Text="Evaluation Type" />
                                            <asp:TextBox runat="server" ReadOnly="true" ClientIDMode="Static" CssClass="form-control" ID="txtTemplate" />
                                        </div>
                                    </div>
                                <h4 class="mt-2">Requirements</h4>
                                <ul>
                                    <asp:Literal ID="ltRequirements" runat="server"></asp:Literal>
                                </ul>
                                <h4>Experts Selected</h4>
                                <asp:Repeater ID="rptExperts" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblExperts" class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>Requirement #</th>
                                                    <th>Expert Name</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Sequence")%></td>
                                            <td><%#Eval("Description")%></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody>
                                            </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblRequest').DataTable({
            "order": [[1, "desc"]],
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
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Request?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Request?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#ShowRequestModal').modal('show');
        } else {
            $('#ShowRequestModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtCaseNumber').val("");
        $('#txtTemplate').val("");
        $('#txtLocation').val("");
        $('#txtCaseNumber').val("");
        $('#hdRequestId').val("");
        return false;
    }
</script>

