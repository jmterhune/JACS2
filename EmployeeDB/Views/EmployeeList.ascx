<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeList.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EmployeeList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">

    <div class="d-flex flex-wrap gap-2 align-items-center mb-3">
        <asp:LinkButton ID="cmdShowMissingSWNContacts" runat="server" CssClass="btn btn-warning" OnClick="cmdShowMissingSWNContacts_Click"><i class="fas fa-user-slash"></i>&nbsp;Show Missing SWN Contacts</asp:LinkButton>
        <asp:LinkButton ID="cmdSWNSync" runat="server" CssClass="btn btn-warning" OnClick="cmdSWNSync_Click"><i class="fas fa-sync"></i>&nbsp;SWN Sync</asp:LinkButton>
        <asp:LinkButton ID="cmdAddAllGroups" runat="server" CssClass="btn btn-warning" OnClick="cmdAddAllGroups_Click"><i class="fas fa-users"></i>&nbsp;Add All Groups</asp:LinkButton>
        <a class="btn btn-primary" href="<%=EditUrl("Directory") %>"><i class="fas fa-address-book"></i>&nbsp;Directory</a>
        <a class="btn btn-primary" href="<%=EditUrl("Details") %>"><i class="fas fa-list"></i>&nbsp;Details List</a>
        <a class="btn btn-success" href="<%=EditUrl("EmployeeId","0","Edit") %>"><i class="fas fa-plus"></i>&nbsp;Add Employee</a>
    </div>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success" role="alert">
        <asp:Literal ID="ltMessage" runat="server" />
    </asp:Panel>

    <div class="tabs">
        <ul class="nav nav-tabs" id="employeeAdminTabs" role="tablist">
            <li class="nav-item active"><a class="nav-link" href="#pane-employees" data-bs-toggle="tab" data-toggle="tab">Employees</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-jobgroups" data-bs-toggle="tab" data-toggle="tab">Job Categories</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-jobclasses" data-bs-toggle="tab" data-toggle="tab">Classes</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-races" data-bs-toggle="tab" data-toggle="tab">Race</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-locations" data-bs-toggle="tab" data-toggle="tab">Office Locations</a></li>
            <li class="nav-item"><a class="nav-link" href="#pane-items" data-bs-toggle="tab" data-toggle="tab">Assigned Items</a></li>
        </ul>

        <div class="tab-content">

            <div class="tab-pane active" id="pane-employees">
                <asp:UpdatePanel ID="upEmployees" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="tblEmployees" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Title</th>
                                    <th>Department</th>
                                    <th>Location</th>
                                    <th>Active</th>
                                    <th class="command-item"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptEmployees" runat="server">
                                    <ItemTemplate>
                                        <tr class='<%# (bool?)(Eval("IsActive")) != true ? "inactive" : "" %>'>
                                            <td><%#Eval("LastName") %>, <%#Eval("FirstName") %></td>
                                            <td><%#Eval("JobTitle") %></td>
                                            <td><%#Eval("AgencyOfEmployment") %></td>
                                            <td><%#Eval("LocationName") %></td>
                                            <td><%# (bool?)(Eval("IsActive")) == true ? "Y" : "N" %></td>
                                            <td class="command-icon">
                                                <a class="text-primary" href='<%# EditUrl("EmployeeId", Eval("EmployeeId").ToString(), "Edit") %>'><i class="fas fa-edit"></i></a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="tab-pane" id="pane-jobgroups">
                <asp:UpdatePanel ID="upJobGroups" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-2">
                            <button type="button" class="btn btn-success" onclick="AddNewJobGroup(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Category</button>
                        </div>
                        <table id="tblJobGroups" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead><tr><th>Description</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="rptJobGroups" runat="server" OnItemCommand="rptJobGroups_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Description") %></td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdEditJobGroup" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("JobGroupId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                            </td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdDeleteJobGroup" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("JobGroupId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this category?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div class="modal fade" id="JobGroupEditModal" tabindex="-1" role="dialog">
                            <div class="modal-dialog" role="document">
                                <div class="modal-content">
                                    <div class="modal-header"><h5 class="modal-title">Job Category</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                    <div class="modal-body">
                                        <asp:HiddenField ID="hdJobGroupId" runat="server" Value="0" />
                                        <div class="mb-3">
                                            <label>Description:</label>
                                            <asp:TextBox ID="txtJobGroupDescription" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <asp:Button ID="cmdSaveJobGroup" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveJobGroup_Click" />
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="tab-pane" id="pane-jobclasses">
                <asp:UpdatePanel ID="upJobClasses" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-2">
                            <button type="button" class="btn btn-success" onclick="AddNewJobClass(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Class</button>
                        </div>
                        <table id="tblJobClasses" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead>
                                <tr>
                                    <th>Class Name</th>
                                    <th>Code</th>
                                    <th>Pay Grade</th>
                                    <th>FLSA</th>
                                    <th>EEO</th>
                                    <th>MMin</th>
                                    <th>MMax</th>
                                    <th>AMin</th>
                                    <th>AMax</th>
                                    <th class="command-item"></th>
                                    <th class="command-item"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptJobClasses" runat="server" OnItemCommand="rptJobClasses_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("ClassName") %></td>
                                            <td><%#Eval("ClassCode") %></td>
                                            <td><%#Eval("PayGrade") %></td>
                                            <td><%#Eval("FLSA") %></td>
                                            <td><%#Eval("EEO") %></td>
                                            <td><%#Eval("MMin", "{0:N2}") %></td>
                                            <td><%#Eval("MMax", "{0:N2}") %></td>
                                            <td><%#Eval("AMin", "{0:N2}") %></td>
                                            <td><%#Eval("AMax", "{0:N2}") %></td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdEditJobClass" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("ClassId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                            </td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdDeleteJobClass" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("ClassId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this class?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div class="modal fade" id="JobClassEditModal" tabindex="-1" role="dialog">
                            <div class="modal-dialog modal-lg" role="document">
                                <div class="modal-content">
                                    <div class="modal-header"><h5 class="modal-title">Class</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                    <div class="modal-body">
                                        <asp:HiddenField ID="hdJobClassId" runat="server" Value="0" />
                                        <div class="row">
                                            <div class="col-md-8 mb-3">
                                                <label>Class Name:</label>
                                                <asp:TextBox ID="txtClassName" runat="server" CssClass="form-control" MaxLength="200" />
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <label>Class Code:</label>
                                                <asp:TextBox ID="txtClassCode" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-4 mb-3">
                                                <label>Pay Grade:</label>
                                                <asp:TextBox ID="txtPayGrade" runat="server" CssClass="form-control" />
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <label>FLSA:</label>
                                                <asp:TextBox ID="txtFLSA" runat="server" CssClass="form-control" MaxLength="50" />
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <label>EEO:</label>
                                                <asp:TextBox ID="txtEEO" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-3 mb-3">
                                                <label>MMin:</label>
                                                <asp:TextBox ID="txtMMin" runat="server" CssClass="form-control" />
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label>MMax:</label>
                                                <asp:TextBox ID="txtMMax" runat="server" CssClass="form-control" />
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label>AMin:</label>
                                                <asp:TextBox ID="txtAMin" runat="server" CssClass="form-control" />
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label>AMax:</label>
                                                <asp:TextBox ID="txtAMax" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <asp:Button ID="cmdSaveJobClass" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveJobClass_Click" />
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="tab-pane" id="pane-races">
                <asp:UpdatePanel ID="upRaces" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-2">
                            <button type="button" class="btn btn-success" onclick="AddNewRace(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Race</button>
                        </div>
                        <table id="tblRaces" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead><tr><th>Code</th><th>Description</th><th class="command-item"></th><th class="command-item"></th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="rptRaces" runat="server" OnItemCommand="rptRaces_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("RaceCode") %></td>
                                            <td><%#Eval("Description") %></td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdEditRace" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("RaceId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                            </td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdDeleteRace" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("RaceId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this race entry?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div class="modal fade" id="RaceEditModal" tabindex="-1" role="dialog">
                            <div class="modal-dialog" role="document">
                                <div class="modal-content">
                                    <div class="modal-header"><h5 class="modal-title">Race</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                    <div class="modal-body">
                                        <asp:HiddenField ID="hdRaceId" runat="server" Value="0" />
                                        <div class="mb-3">
                                            <label>Race Code:</label>
                                            <asp:TextBox ID="txtRaceCode" runat="server" CssClass="form-control" MaxLength="20" />
                                        </div>
                                        <div class="mb-3">
                                            <label>Description:</label>
                                            <asp:TextBox ID="txtRaceDescription" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <asp:Button ID="cmdSaveRace" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveRace_Click" />
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="tab-pane" id="pane-locations">
                <asp:UpdatePanel ID="upLocations" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-2">
                            <button type="button" class="btn btn-success" onclick="AddNewLocation(); return false;"><i class="fas fa-plus"></i>&nbsp;Add Location</button>
                        </div>
                        <table id="tblLocations" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead>
                                <tr>
                                    <th>Description</th>
                                    <th>Address</th>
                                    <th>City</th>
                                    <th>State</th>
                                    <th>Zip</th>
                                    <th class="command-item"></th>
                                    <th class="command-item"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptLocations" runat="server" OnItemCommand="rptLocations_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Description") %></td>
                                            <td><%#Eval("Address") %></td>
                                            <td><%#Eval("City") %></td>
                                            <td><%#Eval("State") %></td>
                                            <td><%#Eval("Zip") %></td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdEditLocation" runat="server" CommandName="EditItem" CommandArgument='<%#Eval("OfficeLocationId") %>' CssClass="text-primary"><i class="fas fa-edit"></i></asp:LinkButton>
                                            </td>
                                            <td class="command-icon">
                                                <asp:LinkButton ID="cmdDeleteLocation" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("OfficeLocationId") %>' CssClass="text-danger" OnClientClick="return confirm('Are you sure you want to delete this location?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div class="modal fade" id="LocationEditModal" tabindex="-1" role="dialog">
                            <div class="modal-dialog modal-lg" role="document">
                                <div class="modal-content">
                                    <div class="modal-header"><h5 class="modal-title">Office Location</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                                    <div class="modal-body">
                                        <asp:HiddenField ID="hdLocationId" runat="server" Value="0" />
                                        <div class="mb-3">
                                            <label>Description:</label>
                                            <asp:TextBox ID="txtLocationDescription" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                        <div class="mb-3">
                                            <label>Address:</label>
                                            <asp:TextBox ID="txtLocationAddress" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 mb-3">
                                                <label>City:</label>
                                                <asp:TextBox ID="txtLocationCity" runat="server" CssClass="form-control" MaxLength="100" />
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label>State:</label>
                                                <asp:TextBox ID="txtLocationState" runat="server" CssClass="form-control" MaxLength="2" />
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label>Zip:</label>
                                                <asp:TextBox ID="txtLocationZip" runat="server" CssClass="form-control" MaxLength="10" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <asp:Button ID="cmdSaveLocation" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSaveLocation_Click" />
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="tab-pane" id="pane-items">
                <asp:UpdatePanel ID="upAssignedItems" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="tblAssignedItems" class="table table-striped table-bordered table-hover" style="width:100%">
                            <thead><tr><th>Employee</th><th>Item Type</th><th>Item Name</th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="rptAssignedItems" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td><%#Eval("ItemType") %></td>
                                            <td><%#Eval("ItemName") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
</div>

<script type="text/javascript">
    function InitEmployeeAdminTables() {
        jQuery(document).ready(function ($) {
            var tableIds = ['#tblEmployees', '#tblJobGroups', '#tblJobClasses', '#tblRaces', '#tblLocations', '#tblAssignedItems'];
            $.each(tableIds, function (i, id) {
                if ($(id).length && !$.fn.DataTable.isDataTable(id)) {
                    $(id).DataTable({
                        "order": [[0, "asc"]],
                        "pageLength": 25,
                        "columnDefs": [
                            { "orderable": false, "targets": "no-sort" }
                        ]
                    });
                }
            });
        });
    }

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
        var existing = bootstrap.Modal.getInstance(el);
        if (existing) { try { existing.dispose(); } catch (e) { } }
        new bootstrap.Modal(el).show();
    }

    function SetVal(clientId, value) {
        var el = document.getElementById(clientId);
        if (el) el.value = value;
    }

    function AddNewJobGroup() {
        SetVal('<%= hdJobGroupId.ClientID %>', '0');
        SetVal('<%= txtJobGroupDescription.ClientID %>', '');
        ShowModal('JobGroupEditModal');
    }
    function AddNewJobClass() {
        SetVal('<%= hdJobClassId.ClientID %>', '0');
        SetVal('<%= txtClassName.ClientID %>', '');
        SetVal('<%= txtClassCode.ClientID %>', '');
        SetVal('<%= txtPayGrade.ClientID %>', '');
        SetVal('<%= txtFLSA.ClientID %>', '');
        SetVal('<%= txtEEO.ClientID %>', '');
        SetVal('<%= txtMMin.ClientID %>', '');
        SetVal('<%= txtMMax.ClientID %>', '');
        SetVal('<%= txtAMin.ClientID %>', '');
        SetVal('<%= txtAMax.ClientID %>', '');
        ShowModal('JobClassEditModal');
    }
    function AddNewRace() {
        SetVal('<%= hdRaceId.ClientID %>', '0');
        SetVal('<%= txtRaceCode.ClientID %>', '');
        SetVal('<%= txtRaceDescription.ClientID %>', '');
        ShowModal('RaceEditModal');
    }
    function AddNewLocation() {
        SetVal('<%= hdLocationId.ClientID %>', '0');
        SetVal('<%= txtLocationDescription.ClientID %>', '');
        SetVal('<%= txtLocationAddress.ClientID %>', '');
        SetVal('<%= txtLocationCity.ClientID %>', '');
        SetVal('<%= txtLocationState.ClientID %>', '');
        SetVal('<%= txtLocationZip.ClientID %>', '');
        ShowModal('LocationEditModal');
    }

    InitEmployeeAdminTables();

    var EMP_TAB_KEY = "empAdminActiveTab";
    function GetActiveEmpTab() {
        try { return sessionStorage.getItem(EMP_TAB_KEY) || "#pane-employees"; }
        catch (e) { return "#pane-employees"; }
    }
    function SetActiveEmpTab(href) {
        try { sessionStorage.setItem(EMP_TAB_KEY, href); } catch (e) { }
    }

    (function ($) {
        $(document).off("click.empTab").on("click.empTab", ".tabs .nav-link[data-toggle=tab], .tabs .nav-link[data-bs-toggle=tab]", function () {
            var href = this.getAttribute("href");
            if (href) { SetActiveEmpTab(href); }
        });
    })(jQuery);

    function RestoreActiveEmpTab() {
        var href = GetActiveEmpTab();
        if (!href) return;
        var link = document.querySelector('.tabs .nav-link[href="' + href + '"]');
        var pane = document.querySelector(href);
        if (!link || !pane) return;
        if (pane.classList.contains("active")) return;

        var tabs = document.querySelectorAll(".tabs .nav-item.active");
        for (var i = 0; i < tabs.length; i++) tabs[i].classList.remove("active");
        var panes = document.querySelectorAll(".tabs .tab-pane.active");
        for (var j = 0; j < panes.length; j++) panes[j].classList.remove("active");

        var li = link.closest(".nav-item");
        if (li) li.classList.add("active");
        pane.classList.add("active");
    }

    jQuery(document).ready(RestoreActiveEmpTab);

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            ForceCloseModals();
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            RestoreActiveEmpTab();
            InitEmployeeAdminTables();
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
