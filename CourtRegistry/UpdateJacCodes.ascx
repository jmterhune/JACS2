<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateJacCodes.ascx.cs" Inherits="tjc.Modules.CourtRegistry.UpdateJacCodes" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#updateJac" data-toggle="tab">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="updateJac" class="tab-pane active">
            <asp:UpdatePanel ID="pnlUpdates" runat="server" RenderMode="Block">
                <ContentTemplate>
                    <asp:Literal ID="ltMessage" runat="server" />
                    <div class="alert alert-info"><i class="fas fa-info-circle"></i>&nbsp;Enter JAC codes that need to be updated, added, or removed. Once you are confident in your changes, click the Update button to apply them.</div>
                    <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#updateModal" onclick="ClearForm()"><i class="fas fa-plus"></i>&nbsp;Add Pending Update</button>
<asp:Literal ID="ltModalScript" runat="server" EnableViewState="false" />
                    <asp:Repeater ID="rptUpdates" runat="server" OnItemCommand="rptUpdates_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Code</th>
                                        <th>Case Type</th>
                                        <th>Category</th>
                                        <th>Update Type</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="edit" CommandArgument='<%#Eval("JacCodeID") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td><%#Eval("JacCodeID") %></td>
                                <td><%#GetCaseTypeName(Eval("CaseTypeID")) %></td>
                                <td><%#Eval("Category") %></td>
                                <td><%#GetUpdateType(Eval("UpdateType")) %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="false" CssClass="text-danger" OnClientClick="return Registry.confirmDelete(this,'Pending Update');" CommandName="delete" CommandArgument='<%#Eval("JacCodeID") %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <p>
                        <asp:Button ID="cmdApply" runat="server" Text="Apply Updates" CssClass="btn btn-success" OnClientClick="return confirm('Are you sure you wish to apply these JAC code updates?');" OnClick="cmdApply_Click" />
                    </p>
                    <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="updateModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="updateModalLabel">Add / Edit Pending JAC Code Update</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdJacCodeID" runat="server" />
                                    <div class="row form-group">
                                        <div class="col-md-6">
                                            <asp:Label runat="server" AssociatedControlID="txtJacCodeID" Text="JAC Code" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtJacCodeID" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJacCodeID" ValidationGroup="upd" Display="Dynamic" CssClass="label label-danger" ErrorMessage="JAC Code is Required" />
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Case Type" />
                                            <asp:DropDownList runat="server" ID="drpCaseType" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-md-12">
                                            <asp:Label runat="server" AssociatedControlID="txtCategory" Text="Category" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtCategory" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-md-12">
                                            <asp:Label runat="server" AssociatedControlID="drpUpdateType" Text="Update Type" />
                                            <asp:DropDownList runat="server" ID="drpUpdateType" CssClass="form-control">
                                                <asp:ListItem Text="-- Select --" Value="" />
                                                <asp:ListItem Text="New" Value="0" />
                                                <asp:ListItem Text="Update" Value="1" />
                                                <asp:ListItem Text="Remove" Value="2" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpUpdateType" ValidationGroup="upd" Display="Dynamic" CssClass="label label-danger" ErrorMessage="Update Type is Required" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button Text="Save" ID="cmdSave" runat="server" CssClass="btn btn-primary" ValidationGroup="upd" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<script type="text/javascript">
    function ClearForm() {
        document.getElementById('<%=hdJacCodeID.ClientID%>').value = '';
        document.getElementById('<%=txtJacCodeID.ClientID%>').value = '';
        document.getElementById('<%=txtCategory.ClientID%>').value = '';
        document.getElementById('<%=drpCaseType.ClientID%>').selectedIndex = 0;
        document.getElementById('<%=drpUpdateType.ClientID%>').selectedIndex = 0;
        document.getElementById('<%=txtJacCodeID.ClientID%>').readOnly = false;
    }
    (function () {
        function cleanupOrphanModals() {
            if (document.querySelectorAll('.modal.show').length === 0) {
                document.querySelectorAll('.modal-backdrop').forEach(function (b) { b.remove(); });
                document.body.classList.remove('modal-open');
                document.body.style.overflow = '';
                document.body.style.paddingRight = '';
            }
        }
        if (window.Sys && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(cleanupOrphanModals);
        }
    })();
</script>
