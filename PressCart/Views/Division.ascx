<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Division.ascx.cs" Inherits="tjc.Modules.PressCart.Division" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tjc-presscart-admin tabs">
    <ul class="nav nav-tabs mb-3">
        <li class="nav-item"><a class="nav-link" href="<%=ListUrl%>">Requests</a></li>
        <li class="nav-item"><a class="nav-link active" href="<%=DivisionUrl%>">Divisions</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=CourtroomUrl%>">Courtrooms</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=CourthouseUrl%>">Courthouses</a></li>
    </ul>
    <div class="tab-content">
        <div id="Division" class="tab-pane active">
            <asp:UpdatePanel ID="pnlDivision" runat="server" RenderMode="Block" OnUnload="pnlDivision_Unload">
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
                    <asp:Repeater ID="rptDivision" runat="server" OnItemCommand="rptDivision_ItemCommand" OnItemCreated="rptDivision_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblDivision" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Division</th>
                                        <th>Judge</th>
                                        <th>County</th>
                                        <th>Limit Days?</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"DivisionId").ToString() %>'><i class="fa fa-pencil" aria-hidden="true"></i></asp:LinkButton>
                                </td>
                                <td><%#Enc(Eval("DivisionName")) %></td>
                                <td><%#Enc(Eval("Judge")) %></td>
                                <td><%#Enc(Eval("County")) %></td>
                                <td><%#Convert.ToString(Eval("IsCircuit"))=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"DivisionId").ToString() %>'><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditDivisionModal" tabindex="-1" role="dialog" aria-labelledby="EditDivisionModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditDivisionModalLabel">Add / Edit Division Entry</h4>
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
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" ValidationGroup="Division"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtDivisionName" Text="Division Name<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtDivisionName" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDivisionName" ValidationGroup="Division"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Division Name is Required" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtJudge" Text="Judge<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtJudge" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJudge" ValidationGroup="Division"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Judge is Required" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkIsCircuit" runat="server" Text="Limit Days?" ClientIDMode="Static" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdDivisionId" ClientIDMode="Static" runat="server" />
                                    <asp:Button OnClientClick="return ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server"
                                        Text="Save" ValidationGroup="Division" OnClick="cmdSave_Click" />
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
        var table = $('#tblDivision').DataTable({
            "bRetrieve": true,
            "bStateSave": true,
            "order": [[1, "asc"]],
            "oLanguage": { "sSearch": "Filter by Text" },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false }
            ]
        });
        if ($("#tblDivision_wrapper .dt-layout-start").find(".btn-primary").length === 0) {
            $("#tblDivision_wrapper .dt-layout-start").first().prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditDivisionModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Division Entry</button>');
        }
        table.draw(false);
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Division Entry?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Division Entry?'
        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditDivisionModal').modal('show');
            return true;
        }
        // Validate before hiding, so a missing required field is still visible to correct.
        if (typeof (Page_ClientValidate) == 'function') {
            Page_ClientValidate('Division');
        }
        if (typeof (Page_IsValid) != 'undefined' && !Page_IsValid) {
            return false;
        }
        $('#EditDivisionModal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        return true;
    }
    function ClearForm() {
        $('#drpCounty').val("");
        $('#txtDivisionName').val("");
        $('#txtJudge').val("");
        $('#chkIsCircuit').prop('checked', false);
        $('#hdDivisionId').val("");
        return false;
    }
</script>
