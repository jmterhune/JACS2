<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseTypes.ascx.cs" Inherits="tjc.Modules.CourtRegistry.CaseTypes" %>
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
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#caseTypes" data-toggle="tab">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="caseTypes" class="tab-pane active">
            <asp:UpdatePanel ID="pnlCaseTypes" runat="server" RenderMode="Block">
                <ContentTemplate>
                    <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#caseTypeModal" onclick="ClearForm()"><i class="fas fa-plus"></i>&nbsp;Add Case Type</button>
<asp:Literal ID="ltModalScript" runat="server" EnableViewState="false" />
                    <asp:Repeater ID="rptCaseTypes" runat="server" OnItemCommand="rptCaseTypes_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Case Type</th>
                                        <th>Active</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="edit" CommandArgument='<%#Eval("CaseTypeID") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td><%#Eval("CaseTypeID") %></td>
                                <td><%#Eval("CaseTypeName") %></td>
                                <td><%#Convert.ToBoolean(Eval("Active")) ? "<i class='fas fa-square-check'></i>" : "<i class='fas fa-square'></i>" %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="false" CssClass="text-danger" OnClientClick="return Registry.confirmDelete(this,'Case Type');" CommandName="delete" CommandArgument='<%#Eval("CaseTypeID") %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="caseTypeModal" tabindex="-1" role="dialog" aria-labelledby="caseTypeModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="caseTypeModalLabel">Add / Edit Case Type</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdCaseTypeID" runat="server" />
                                    <div class="row form-group">
                                        <div class="col-md-12">
                                            <asp:Label runat="server" AssociatedControlID="txtCaseTypeName" Text="Case Type Name" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtCaseTypeName" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseTypeName" ValidationGroup="ct" Display="Dynamic" CssClass="label label-danger" ErrorMessage="Case Type Name is Required" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-md-12">
                                            <asp:CheckBox runat="server" ID="chkActive" Text="Active" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button Text="Save" ID="cmdSave" runat="server" CssClass="btn btn-primary" ValidationGroup="ct" OnClick="cmdSave_Click" />
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
        document.getElementById('<%=hdCaseTypeID.ClientID%>').value = '';
        document.getElementById('<%=txtCaseTypeName.ClientID%>').value = '';
        document.getElementById('<%=chkActive.ClientID%>').checked = false;
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
