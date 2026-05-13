<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Locations.ascx.cs" Inherits="tjc.Modules.CourtRegistry.Locations" %>
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
        <li class="nav-item active">
            <a class="nav-link" href="#locations" data-toggle="tab">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="locations" class="tab-pane active">
            <asp:UpdatePanel ID="pnlLocations" runat="server" RenderMode="Block">
                <ContentTemplate>
                    <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#locationModal" onclick="ClearForm()"><i class="fas fa-plus"></i>&nbsp;Add Location</button>
<asp:Literal ID="ltModalScript" runat="server" EnableViewState="false" />
                    <asp:Repeater ID="rptLocations" runat="server" OnItemCommand="rptLocations_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Name</th>
                                        <th>Abbreviation</th>
                                        <th>County Number</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="edit" CommandArgument='<%#Eval("LocationID") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td><%#Eval("LocationID") %></td>
                                <td><%#Eval("LocationName") %></td>
                                <td><%#Eval("Abbreviation") %></td>
                                <td><%#Eval("CountyNumber") %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="false" CssClass="text-danger" OnClientClick="return Registry.confirmDelete(this,'Location');" CommandName="delete" CommandArgument='<%#Eval("LocationID") %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="locationModal" tabindex="-1" role="dialog" aria-labelledby="locationModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="locationModalLabel">Add / Edit Location</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdLocationID" runat="server" />
                                    <div class="row form-group">
                                        <div class="col-md-12">
                                            <asp:Label runat="server" AssociatedControlID="txtLocationName" Text="Location Name" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtLocationName" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLocationName" ValidationGroup="loc" Display="Dynamic" CssClass="label label-danger" ErrorMessage="Name is Required" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-md-6">
                                            <asp:Label runat="server" AssociatedControlID="txtAbbreviation" Text="Abbreviation" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtAbbreviation" MaxLength="10" />
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label runat="server" AssociatedControlID="txtCountyNumber" Text="County Number" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtCountyNumber" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button Text="Save" ID="cmdSave" runat="server" CssClass="btn btn-primary" ValidationGroup="loc" OnClick="cmdSave_Click" />
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
        document.getElementById('<%=hdLocationID.ClientID%>').value = '';
        document.getElementById('<%=txtLocationName.ClientID%>').value = '';
        document.getElementById('<%=txtAbbreviation.ClientID%>').value = '';
        document.getElementById('<%=txtCountyNumber.ClientID%>').value = '';
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
