<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Courtroom.ascx.cs" Inherits="tjc.Modules.PressCart.Courtroom" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tjc-presscart-admin tabs">
    <ul class="nav nav-tabs mb-3">
        <li class="nav-item"><a class="nav-link" href="<%=ListUrl%>">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=DivisionUrl%>">Divisions</a></li>
        <li class="nav-item"><a class="nav-link active" href="<%=CourtroomUrl%>">Courtrooms</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=CourthouseUrl%>">Courthouses</a></li>
    </ul>
    <div class="tab-content">
        <div id="Courtroom" class="tab-pane active">
            <asp:UpdatePanel ID="pnlCourtroom" runat="server" RenderMode="Block" OnUnload="pnlCourtroom_Unload">
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
                    <asp:Repeater ID="rptCourtroom" runat="server" OnItemCommand="rptCourtroom_ItemCommand" OnItemCreated="rptCourtroom_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblCourtroom" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Courtroom</th>
                                        <th>Courthouse</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CourtroomId").ToString() %>'><i class="fa fa-pencil" aria-hidden="true"></i></asp:LinkButton>
                                </td>
                                <td><%#Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"CourtroomName").ToString()) %></td>
                                <td><%#Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"CourthouseName").ToString()) %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CourtroomId").ToString() %>'><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditCourtroomModal" tabindex="-1" role="dialog" aria-labelledby="EditCourtroomModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditCourtroomModalLabel">Add / Edit Courtroom Entry</h4>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <asp:Label ID="lblCourthouse" runat="server" AssociatedControlID="drpCourthouse" Text="Courthouse<em>*</em>" />
                                        <asp:DropDownList ID="drpCourthouse" runat="server" ClientIDMode="Static" CssClass="form-control" AppendDataBoundItems="true">
                                            <asp:ListItem Text="< Select Courthouse >" Value="" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCourthouse" ValidationGroup="Courtroom"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Courthouse is Required" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtCourtroomName" Text="Courtroom Name<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCourtroomName" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCourtroomName" ValidationGroup="Courtroom"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Courtroom Name is Required" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdCourtroomId" ClientIDMode="Static" runat="server" />
                                    <asp:Button OnClientClick="return ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server"
                                        Text="Save" ValidationGroup="Courtroom" OnClick="cmdSave_Click" />
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

<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
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
        var table = $('#tblCourtroom').DataTable({
            "bRetrieve": true,
            "bStateSave": true,
            "order": [[2, "asc"], [1, "asc"]],
            "oLanguage": { "sSearch": "Filter by Text" },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false }
            ]
        });
        if ($("#tblCourtroom_wrapper .dt-layout-start").find(".btn-primary").length === 0) {
            $("#tblCourtroom_wrapper .dt-layout-start").first().prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditCourtroomModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Courtroom Entry</button>');
        }
        table.draw(false);
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Courtroom Entry?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Courtroom Entry?'
        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditCourtroomModal').modal('show');
            return true;
        }
        // Validate before hiding, so a missing required field is still visible to correct.
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate('Courtroom');
        }
        if (typeof (Page_IsValid) != 'undefined' && !Page_IsValid) {
            return false;
        }
        $('#EditCourtroomModal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        return true;
    }
    function ClearForm() {
        $('#drpCourthouse').val("");
        $('#txtCourtroomName').val("");
        $('#hdCourtroomId').val("");
        return false;
    }
</script>
