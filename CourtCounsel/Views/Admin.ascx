<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Admin.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Admin" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item"><a class="nav-link active" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="container-fluid mt-3">
    <ul class="nav nav-tabs" id="adminTabs" role="tablist">
        <li class="nav-item"><a class="nav-link active" id="tab-casetypes" data-toggle="tab" href="#pane-casetypes" role="tab">Case Types</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-attorneys" data-toggle="tab" href="#pane-attorneys" role="tab">Attorneys</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-counties" data-toggle="tab" href="#pane-counties" role="tab">Counties</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-phases" data-toggle="tab" href="#pane-phases" role="tab">Phases</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-requestors" data-toggle="tab" href="#pane-requestors" role="tab">Requestors</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-actions" data-toggle="tab" href="#pane-actions" role="tab">Actions</a></li>
        <li class="nav-item"><a class="nav-link" id="tab-timespent" data-toggle="tab" href="#pane-timespent" role="tab">Time Spent</a></li>
    </ul>

    <div class="tab-content mt-3">
        <!-- Case Types Tab -->
        <div class="tab-pane fade show active" id="pane-casetypes" role="tabpanel">
            <asp:UpdatePanel ID="upCaseTypes" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblCaseTypes" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Case Type</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptCaseTypes" runat="server" OnItemCommand="rptCaseTypes_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("CaseType") %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditCaseType" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("CaseTypeId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteCaseType" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("CaseTypeId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this case type?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <!-- Modal -->
                    <div class="modal fade" id="modalCaseType" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Case Type</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdCaseTypeId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Case Type:</label>
                                        <asp:TextBox ID="txtCaseType" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveCaseType" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveCaseType_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Attorneys Tab -->
        <div class="tab-pane fade" id="pane-attorneys" role="tabpanel">
            <asp:UpdatePanel ID="upAttorneys" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblAttorneys" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Attorney Name</th><th>Active</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptAttorneys" runat="server" OnItemCommand="rptAttorneys_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                        <td><%#Eval("AttorneyName") %></td>
                                        <td><%# (bool?)(Eval("IsActive")) == true ? "Yes" : "No" %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditAttorney" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("AttorneyId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteAttorney" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("AttorneyId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this attorney?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalAttorney" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Attorney</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdAttorneyId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Attorney Name:</label>
                                        <asp:TextBox ID="txtAttorneyName" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkAttorneyActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveAttorney" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveAttorney_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Counties Tab -->
        <div class="tab-pane fade" id="pane-counties" role="tabpanel">
            <asp:UpdatePanel ID="upCounties" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblCounties" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>County</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptCounties" runat="server" OnItemCommand="rptCounties_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("County") %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditCounty" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("CountyId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteCounty" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("CountyId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this county?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalCounty" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">County</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdCountyId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>County:</label>
                                        <asp:TextBox ID="txtCounty" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveCounty" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveCounty_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Phases Tab -->
        <div class="tab-pane fade" id="pane-phases" role="tabpanel">
            <asp:UpdatePanel ID="upPhases" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblPhases" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Phase</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptPhases" runat="server" OnItemCommand="rptPhases_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("Phase") %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditPhase" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("PhaseId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeletePhase" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("PhaseId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this phase?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalPhase" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Phase</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdPhaseId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Phase:</label>
                                        <asp:TextBox ID="txtPhase" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSavePhase" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSavePhase_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Requestors Tab -->
        <div class="tab-pane fade" id="pane-requestors" role="tabpanel">
            <asp:UpdatePanel ID="upRequestors" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblRequestors" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Requestor Name</th><th>Active</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptRequestors" runat="server" OnItemCommand="rptRequestors_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                        <td><%#Eval("RequestorName") %></td>
                                        <td><%# (bool?)(Eval("IsActive")) == true ? "Yes" : "No" %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditRequestor" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("RequestorId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteRequestor" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("RequestorId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this requestor?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalRequestor" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Requestor</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdRequestorId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Requestor Name:</label>
                                        <asp:TextBox ID="txtRequestorName" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkRequestorActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveRequestor" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveRequestor_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Actions Tab -->
        <div class="tab-pane fade" id="pane-actions" role="tabpanel">
            <asp:UpdatePanel ID="upActions" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblActions" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Action</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptActions" runat="server" OnItemCommand="rptActions_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("Action") %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditAction" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("ActionId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteAction" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("ActionId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this action?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalAction" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Action</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdActionId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Action:</label>
                                        <asp:TextBox ID="txtAction" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveAction" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveAction_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Time Spent Tab -->
        <div class="tab-pane fade" id="pane-timespent" role="tabpanel">
            <asp:UpdatePanel ID="upTimeSpent" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="tblTimeSpent" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th>Time Span</th><th>Active</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptTimeSpent" runat="server" OnItemCommand="rptTimeSpent_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# !(bool)Eval("IsActive") ? "inactive" : "" %>'>
                                        <td><%#Eval("TimeSpan") %></td>
                                        <td><%# (bool)Eval("IsActive") ? "Yes" : "No" %></td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditTimeSpent" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("TimeSpanId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdDeleteTimeSpent" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("TimeSpanId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this time span?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="modal fade" id="modalTimeSpent" tabindex="-1" role="dialog">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header"><h5 class="modal-title">Time Spent</h5><button type="button" class="close" data-dismiss="modal"><span>&times;</span></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdTimeSpentId" runat="server" Value="0" />
                                    <div class="form-group">
                                        <label>Time Span:</label>
                                        <asp:TextBox ID="txtTimeSpan" runat="server" CssClass="form-control" MaxLength="50" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkTimeSpentActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveTimeSpent" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveTimeSpent_Click" />
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
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
    function InitAdminTables() {
        jQuery(document).ready(function ($) {
            var tableIds = ['#tblCaseTypes', '#tblAttorneys', '#tblCounties', '#tblPhases', '#tblRequestors', '#tblActions', '#tblTimeSpent'];
            $.each(tableIds, function (i, id) {
                if ($(id).length && !$.fn.DataTable.isDataTable(id)) {
                    $(id).DataTable({
                        "order": [[0, "asc"]],
                        "pageLength": 25,
                        "columnDefs": [
                            { "orderable": false, "targets": [-1, -2] }
                        ]
                    });
                }
            });
        });
    }

    function ShowModal(modalId) {
        jQuery('#' + modalId).modal('show');
    }

    InitAdminTables();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            InitAdminTables();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/CourtCounsel/Styles/module.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
