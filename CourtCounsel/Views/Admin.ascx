<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Admin.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Admin" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
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

<div class="tabs mt-3">
    <ul class="nav nav-tabs">
        <li class="nav-item active"><a class="nav-link" href="#pane-casetypes" data-toggle="tab">Case Types</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-attorneys" data-toggle="tab">Attorneys</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-counties" data-toggle="tab">Counties</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-phases" data-toggle="tab">Phases</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-requestors" data-toggle="tab">Requestors</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-actions" data-toggle="tab">Actions</a></li>
        <li class="nav-item"><a class="nav-link" href="#pane-timespent" data-toggle="tab">Time Spent</a></li>
    </ul>

    <div class="tab-content">
        <!-- Case Types Tab -->
        <div class="tab-pane active" id="pane-casetypes">
            <asp:UpdatePanel ID="upCaseTypes" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewCaseType(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Case Type</button>
                    </div>
                    <table id="tblCaseTypes" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Case Type</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptCaseTypes" runat="server" OnItemCommand="rptCaseTypes_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditCaseType" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("CaseTypeId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("CaseType") %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Case Type</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdCaseTypeId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Case Type:</label>
                                        <asp:TextBox ID="txtCaseType" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveCaseType" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveCaseType_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Attorneys Tab -->
        <div class="tab-pane" id="pane-attorneys">
            <asp:UpdatePanel ID="upAttorneys" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewAttorney(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Attorney</button>
                    </div>
                    <table id="tblAttorneys" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Attorney Name</th><th>Active</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptAttorneys" runat="server" OnItemCommand="rptAttorneys_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditAttorney" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("AttorneyId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("AttorneyName") %></td>
                                        <td><%# (bool?)(Eval("IsActive")) == true ? "Yes" : "No" %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Attorney</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdAttorneyId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Attorney Name:</label>
                                        <asp:TextBox ID="txtAttorneyName" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:CheckBox ID="chkAttorneyActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveAttorney" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveAttorney_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Counties Tab -->
        <div class="tab-pane" id="pane-counties">
            <asp:UpdatePanel ID="upCounties" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewCounty(); return false;"><i class="fas fa-plus"></i>&nbsp;Add County</button>
                    </div>
                    <table id="tblCounties" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>County</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptCounties" runat="server" OnItemCommand="rptCounties_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditCounty" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("CountyId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("County") %></td>
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
                                <div class="modal-header"><h5 class="modal-title">County</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdCountyId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>County:</label>
                                        <asp:TextBox ID="txtCounty" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveCounty" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveCounty_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Phases Tab -->
        <div class="tab-pane" id="pane-phases">
            <asp:UpdatePanel ID="upPhases" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewPhase(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Phase</button>
                    </div>
                    <table id="tblPhases" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Phase</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptPhases" runat="server" OnItemCommand="rptPhases_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditPhase" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("PhaseId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("Phase") %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Phase</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdPhaseId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Phase:</label>
                                        <asp:TextBox ID="txtPhase" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSavePhase" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSavePhase_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Requestors Tab -->
        <div class="tab-pane" id="pane-requestors">
            <asp:UpdatePanel ID="upRequestors" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewRequestor(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Requestor</button>
                    </div>
                    <table id="tblRequestors" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Requestor Name</th><th>Active</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptRequestors" runat="server" OnItemCommand="rptRequestors_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditRequestor" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("RequestorId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("RequestorName") %></td>
                                        <td><%# (bool?)(Eval("IsActive")) == true ? "Yes" : "No" %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Requestor</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdRequestorId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Requestor Name:</label>
                                        <asp:TextBox ID="txtRequestorName" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:CheckBox ID="chkRequestorActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveRequestor" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveRequestor_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Actions Tab -->
        <div class="tab-pane" id="pane-actions">
            <asp:UpdatePanel ID="upActions" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewAction(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Action</button>
                    </div>
                    <table id="tblActions" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Action</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptActions" runat="server" OnItemCommand="rptActions_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditAction" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("ActionId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("Action") %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Action</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdActionId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Action:</label>
                                        <asp:TextBox ID="txtAction" runat="server" CssClass="form-control" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveAction" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveAction_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Time Spent Tab -->
        <div class="tab-pane" id="pane-timespent">
            <asp:UpdatePanel ID="upTimeSpent" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="mb-2">
                        <button type="button" class="btn btn-success" onclick="AddNewTimeSpent(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Time Span</button>
                    </div>
                    <table id="tblTimeSpent" class="table table-striped table-bordered table-hover" style="width:100%">
                        <thead><tr><th class="command-item"></th><th>Id</th><th>Time Span</th><th class="command-item"></th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptTimeSpent" runat="server" OnItemCommand="rptTimeSpent_ItemCommand">
                                <ItemTemplate>
                                    <tr class='<%# !(bool)Eval("IsActive") ? "inactive" : "" %>'>
                                        <td class="command-icon">
                                            <asp:LinkButton ID="cmdEditTimeSpent" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("TimeSpanId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                        </td>
                                        <td><%#Eval("TimeSpanId") %></td>
                                        <td><%#Eval("TimeSpan") %></td>
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
                                <div class="modal-header"><h5 class="modal-title">Time Spent</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdTimeSpentId" runat="server" Value="0" />
                                    <div class="mb-3">
                                        <label>Time Span:</label>
                                        <asp:TextBox ID="txtTimeSpan" runat="server" CssClass="form-control" MaxLength="50" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:CheckBox ID="chkTimeSpentActive" runat="server" Text=" Is Active" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="cmdSaveTimeSpent" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveTimeSpent_Click" />
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
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

    // Synchronously drop any open modal + its backdrop + body-lock state. Called BEFORE a partial
    // postback replaces the modal's DOM (so we don't orphan the backdrop), and also as a post-request
    // safety net for lingering artifacts. Safe to call when no modal is open.
    function ForceCloseModals() {
        var open = document.querySelectorAll(".modal.show");
        for (var i = 0; i < open.length; i++) {
            open[i].classList.remove("show");
            open[i].style.display = "none";
            if (typeof bootstrap !== "undefined") {
                var inst = bootstrap.Modal.getInstance(open[i]);
                if (inst) { try { inst.dispose(); } catch (e) { } }
            }
        }
        var backdrops = document.querySelectorAll(".modal-backdrop");
        for (var j = 0; j < backdrops.length; j++) {
            backdrops[j].parentNode.removeChild(backdrops[j]);
        }
        document.body.classList.remove("modal-open");
        document.body.style.paddingRight = "";
        document.body.style.overflow = "";
    }

    function ShowModal(modalId) {
        var el = document.getElementById(modalId);
        if (!el || typeof bootstrap === "undefined") return;

        // Dispose any stale Modal instance bound to a pre-postback copy of the element.
        var existing = bootstrap.Modal.getInstance(el);
        if (existing) { try { existing.dispose(); } catch (e) { } }

        new bootstrap.Modal(el).show();
    }

    function SetVal(clientId, value) {
        var el = document.getElementById(clientId);
        if (el) el.value = value;
    }
    function SetChecked(clientId, checked) {
        var el = document.getElementById(clientId);
        if (el) el.checked = checked;
    }

    function AddNewCaseType() {
        SetVal('<%= hdCaseTypeId.ClientID %>', '0');
        SetVal('<%= txtCaseType.ClientID %>', '');
        ShowModal('modalCaseType');
    }
    function AddNewAttorney() {
        SetVal('<%= hdAttorneyId.ClientID %>', '0');
        SetVal('<%= txtAttorneyName.ClientID %>', '');
        SetChecked('<%= chkAttorneyActive.ClientID %>', true);
        ShowModal('modalAttorney');
    }
    function AddNewCounty() {
        SetVal('<%= hdCountyId.ClientID %>', '0');
        SetVal('<%= txtCounty.ClientID %>', '');
        ShowModal('modalCounty');
    }
    function AddNewPhase() {
        SetVal('<%= hdPhaseId.ClientID %>', '0');
        SetVal('<%= txtPhase.ClientID %>', '');
        ShowModal('modalPhase');
    }
    function AddNewRequestor() {
        SetVal('<%= hdRequestorId.ClientID %>', '0');
        SetVal('<%= txtRequestorName.ClientID %>', '');
        SetChecked('<%= chkRequestorActive.ClientID %>', true);
        ShowModal('modalRequestor');
    }
    function AddNewAction() {
        SetVal('<%= hdActionId.ClientID %>', '0');
        SetVal('<%= txtAction.ClientID %>', '');
        ShowModal('modalAction');
    }
    function AddNewTimeSpent() {
        SetVal('<%= hdTimeSpentId.ClientID %>', '0');
        SetVal('<%= txtTimeSpan.ClientID %>', '');
        SetChecked('<%= chkTimeSpentActive.ClientID %>', true);
        ShowModal('modalTimeSpent');
    }

    InitAdminTables();

    // Porto's tab behavior is reset on every UpdatePanel partial postback (it re-reads the
    // hard-coded .nav-item.active from markup, which always points at Case Types). Persist
    // the user's chosen tab in sessionStorage so it survives script re-execution in DNN's
    // partial-postback response, and re-apply it after each async response.
    var ADMIN_TAB_KEY = "ccAdminActiveTab";
    function GetActiveAdminTab() {
        try { return sessionStorage.getItem(ADMIN_TAB_KEY) || "#pane-casetypes"; }
        catch (e) { return "#pane-casetypes"; }
    }
    function SetActiveAdminTab(href) {
        try { sessionStorage.setItem(ADMIN_TAB_KEY, href); } catch (e) { }
    }

    (function ($) {
        // Delegated click handler — safe to re-bind on each script evaluation because
        // jQuery de-duplicates identical selector+namespaced events.
        $(document).off("click.adminTab").on("click.adminTab", ".tabs .nav-link[data-toggle=tab], .tabs .nav-link[data-bs-toggle=tab]", function () {
            var href = this.getAttribute("href");
            if (href) { SetActiveAdminTab(href); }
        });
    })(jQuery);

    function RestoreActiveTab() {
        var href = GetActiveAdminTab();
        if (!href) return;
        var link = document.querySelector('.tabs .nav-link[href="' + href + '"]');
        var pane = document.querySelector(href);
        if (!link || !pane) return;
        if (pane.classList.contains("active")) return; // already correct

        var tabs = document.querySelectorAll(".tabs .nav-item.active");
        for (var i = 0; i < tabs.length; i++) tabs[i].classList.remove("active");
        var panes = document.querySelectorAll(".tabs .tab-pane.active");
        for (var j = 0; j < panes.length; j++) panes[j].classList.remove("active");

        var li = link.closest(".nav-item");
        if (li) li.classList.add("active");
        pane.classList.add("active");
    }

    // Also restore on fresh page load (covers full refreshes where the user last had a non-default tab open).
    jQuery(document).ready(RestoreActiveTab);

    if (typeof Sys !== 'undefined') {
        // Close any modal BEFORE the UpdatePanel swaps DOM (prevents orphan backdrops
        // left over from a Save click that kept body.modal-open locked).
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            ForceCloseModals();
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            RestoreActiveTab();          // put the user back on the tab they were editing
            InitAdminTables();
            // Safety net: if the response didn't register a ShowModal call and something
            // still left artifacts (e.g. an inline script errored), clean after the
            // BS5 fade-in window would have completed.
            setTimeout(function () {
                if (document.querySelectorAll(".modal.show").length === 0) {
                    var b = document.querySelectorAll(".modal-backdrop");
                    for (var i = 0; i < b.length; i++) b[i].parentNode.removeChild(b[i]);
                    document.body.classList.remove("modal-open");
                    document.body.style.paddingRight = "";
                    document.body.style.overflow = "";
                }
            }, 300);
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
