<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Courthouse.ascx.cs" Inherits="tjc.Modules.PressCart.Courthouse" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tjc-presscart-admin tabs">
    <ul class="nav nav-tabs mb-3">
        <li class="nav-item"><a class="nav-link" href="<%=ListUrl%>">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=DivisionUrl%>">Divisions</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=CourtroomUrl%>">Courtrooms</a></li>
        <li class="nav-item"><a class="nav-link active" href="<%=CourthouseUrl%>">Courthouses</a></li>
    </ul>
    <div class="tab-content">
        <div id="Courthouse" class="tab-pane active">
            <asp:UpdatePanel ID="pnlCourthouse" runat="server" RenderMode="Block" OnUnload="pnlCourthouse_Unload">
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
                    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert alert-warning d-block" />
                    <asp:Repeater ID="rptCourthouse" runat="server" OnItemCommand="rptCourthouse_ItemCommand" OnItemCreated="rptCourthouse_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblCourthouse" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Courthouse</th>
                                        <th>County</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CourthouseId").ToString() %>'><i class="fa fa-pencil" aria-hidden="true"></i></asp:LinkButton>
                                </td>
                                <td><%#Enc(Eval("CourthouseName")) %></td>
                                <td><%#Enc(Eval("County")) %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CourthouseId").ToString() %>'><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditCourthouseModal" tabindex="-1" role="dialog" aria-labelledby="EditCourthouseModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditCourthouseModalLabel">Add / Edit Courthouse Entry</h4>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <asp:Label ID="lblCounty" runat="server" AssociatedControlID="drpCounty" Text="County<em>*</em>" />
                                        <asp:DropDownList ID="drpCounty" runat="server" ClientIDMode="Static" CssClass="form-control">
                                            <asp:ListItem Text="< Select County >" Value="" />
                                            <asp:ListItem Text="Manatee" />
                                            <asp:ListItem Text="Sarasota" />
                                            <asp:ListItem Text="DeSoto" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" ValidationGroup="Courthouse"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtCourthouseName" Text="Courthouse Name<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCourthouseName" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCourthouseName" ValidationGroup="Courthouse"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Courthouse Name is Required" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdCourthouseId" ClientIDMode="Static" runat="server" />
                                    <asp:Button OnClientClick="return ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server"
                                        Text="Save" ValidationGroup="Courthouse" OnClick="cmdSave_Click" />
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
        var table = $('#tblCourthouse').DataTable({
            "bRetrieve": true,
            "bStateSave": true,
            "order": [[1, "asc"]],
            "oLanguage": { "sSearch": "Filter by Text" },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false }
            ]
        });
        if ($("#tblCourthouse_wrapper .dt-layout-start").find(".btn-primary").length === 0) {
            $("#tblCourthouse_wrapper .dt-layout-start").first().prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditCourthouseModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Courthouse Entry</button>');
        }
        table.draw(false);
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Courthouse Entry?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Courthouse Entry?'
        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditCourthouseModal').modal('show');
            return true;
        }
        // Validate before hiding, so a missing required field is still visible to correct.
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate('Courthouse');
        }
        if (typeof (Page_IsValid) != 'undefined' && !Page_IsValid) {
            return false;
        }
        $('#EditCourthouseModal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        return true;
    }
    function ClearForm() {
        $('#drpCounty').val("");
        $('#txtCourthouseName').val("");
        $('#hdCourthouseId').val("");
        return false;
    }
</script>
