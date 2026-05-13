<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpertList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.ExpertList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=RequestListUrl %>">Requests</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#experts" data-toggle="tab">Experts</a>
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
        <div id="experts" class="tab-pane active">
            <asp:UpdatePanel ID="pnlExperts" runat="server" RenderMode="Block" OnUnload="pnlExperts_Unload">
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
                    <asp:Repeater ID="rptExpert" runat="server" OnItemCreated="rptExpert_ItemCreated" OnItemCommand="rptExpert_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblExpert" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Expert Description</th>
                                        <th>Field of Expertise</th>
                                        <th>Locations</th>
                                        <th>Contract Ends</th>
                                        <th>&nbsp;</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ExpertId").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td><%#Eval("ExpertID")%></td>
                                <td><%#Eval("Description")%></td>
                                <td><%#Eval("TypeDisplay")%></td>
                                <td><%#Eval("LocationDisplay")%></td>
                                <td><%#Eval("ContractEnds","{0:MM/dd/yyyy}")%></td>
                                <td><%#Eval("CommentDisplay")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ExpertId").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditExpertModal" tabindex="-1" role="dialog" aria-labelledby="EditExpertModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditExpertModalLabel">Add / Edit Expert</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row form-group">
                                        <div class="col-8">
                                            <asp:Label runat="server" AssociatedControlID="txtExpertName" Text="Expert" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtExpertName" />
                                        </div>
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="txtContractEnds" Text="Contract Ends" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control date-picker" MaxLength="50" ID="txtContractEnds" />
                                        </div>
                                    </div>
                                    <fieldset class="outline-fieldset">
                                        <legend>Locations</legend>
                                        <asp:CheckBoxList ID="clsLocations" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch locations" RepeatLayout="UnorderedList">
                                        </asp:CheckBoxList>
                                    </fieldset>
                                    <fieldset class="outline-fieldset">
                                        <legend>Types</legend>
                                        <asp:CheckBoxList ID="clsTypes" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list column-4 form-check form-switch types" RepeatLayout="UnorderedList">
                                        </asp:CheckBoxList>
                                    </fieldset>
                                    <fieldset class="outline-fieldset">
                                        <legend>Evaluation Types</legend>
                                        <asp:CheckBoxList ID="clsEvaluationTypes" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list form-check form-switch evaluation-types" RepeatLayout="UnorderedList">
                                        </asp:CheckBoxList>
                                    </fieldset>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtComments" Text="Comments" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="MultiLine" Rows="4" ID="txtComments" />
                                    </div>
                                    <asp:HiddenField ID="hdExpertId" ClientIDMode="Static" runat="server" />
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
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        $(".date-picker").datepicker();
        $('[data-toggle="tooltip"]').tooltip();
        var table = $('#tblExpert').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#.dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditExpertModal"><i class="fa fa-plus"></i>&nbsp;Add Expert</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Expert?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Expert?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditExpertModal').modal('show');
        } else {
            $('#EditExpertModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#txtExpertName').val("");
        $('#txtContractEnds').val("");
        $('#txtContractEnds').val("");
        $('#txtComments').val("");
        $("#EditExpertModal input[type='checkbox']").prop('checked', false); 
        return false;
    }
</script>

