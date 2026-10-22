<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Search" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><asp:HyperLink CssClass="nav-link active" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="container-fluid mt-3">
    <div class="row mb-3">
        <div class="col-md-8">
            <div class="input-group">
                <button class="btn btn-outline-secondary dropdown-toggle" type="button" id="btnSearchType" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    Case Name
                </button>
                <ul class="dropdown-menu">
                    <li><a class="dropdown-item" href="#" onclick="SetSearchType(1); return false;">Case Name</a></li>
                    <li><a class="dropdown-item" href="#" onclick="SetSearchType(2); return false;">Case Number</a></li>
                    <li><a class="dropdown-item" href="#" onclick="SetSearchType(3); return false;">Attorney</a></li>
                </ul>
                <asp:TextBox ID="swSearchTerm" runat="server" CssClass="form-control" placeholder="Enter search term..." />
                <asp:DropDownList ID="swAttorney" runat="server" CssClass="form-control" style="display:none;" />
                <div class="dropdown action-dropdown" id="divActionTypes">
                    <button type="button" id="actionToggle" class="form-select text-start" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false" aria-label="Action type">
                        <span id="actionSelectedLabel">All Action Types</span>
                    </button>
                    <div class="dropdown-menu p-2 action-dropdown-menu">
                        <input type="text" class="form-control form-control-sm mb-2 action-filter" placeholder="Filter action types..." autocomplete="off" />
                        <div class="form-check border-bottom pb-1 mb-1">
                            <input type="checkbox" class="form-check-input action-select-all" id="chkActionAll" checked="checked" />
                            <label class="form-check-label fw-bold" for="chkActionAll">All Action Types</label>
                        </div>
                        <asp:CheckBoxList ID="cblActions" runat="server" RepeatLayout="Flow" CssClass="action-checklist" />
                        <div class="d-flex justify-content-between align-items-center border-top pt-2 mt-1 small text-muted">
                            <span id="actionCount">All selected</span>
                            <button type="button" class="btn btn-link btn-sm p-0 action-reset">Reset to All</button>
                        </div>
                    </div>
                </div>
                <asp:Button ID="cmdSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="cmdSearch_Click" />
            </div>
        </div>
        <div class="col-md-4">
            <div id="divStatusFilters" style="display:none;">
                <div class="form-check form-check-inline">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" Text="Active" CssClass="form-check" />
                </div>
                <div class="form-check form-check-inline">
                    <asp:CheckBox ID="chkPending" runat="server" Text="Inactive" CssClass="form-check" />
                </div>
                <div class="form-check form-check-inline">
                    <asp:CheckBox ID="chkClosed" runat="server" Text="Completed" CssClass="form-check" />
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdSearchType" runat="server" Value="1" />
</div>

<asp:UpdatePanel ID="upResults" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="table-responsive">
            <table id="log-list" class="table table-striped table-bordered table-hover" style="width:100%">
                <thead>
                    <tr>
                        <th></th>
                        <th>Case Number</th>
                        <th>Party Name</th>
                        <th>Case Type</th>
                        <th>Date Received</th>
                        <th>Responsible</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptResults" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <a href='<%#EditUrl("lid",Eval("LogId").ToString(),"EditHistory") %>'>
                                        <i class="fas fa-edit"></i>
                                    </a>
                                </td>
                                <td>
                                    <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") %>'>
                                        <%#Eval("CaseNumber") %>
                                    </a>
                                </td>
                                <td><%#Eval("PartyName") %></td>
                                <td><%#Eval("CaseType") %></td>
                                <td><%#Eval("DateReceived", "{0:d}") %></td>
                                <td><%#Eval("Responsible") %></td>
                                <td><%#GetStatus((tjc.Modules.CourtCounsel.Components.Models.HistoryInfo)Container.DataItem) %></td>
                                <td><%#Eval("Action") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    function SetSearchType(type) {
        document.getElementById('<%= hdSearchType.ClientID %>').value = type;
        var labels = ['', 'Case Name', 'Case Number', 'Attorney'];
        document.getElementById('btnSearchType').innerText = labels[type];

        var searchTerm = document.getElementById('<%= swSearchTerm.ClientID %>');
        var attorney = document.getElementById('<%= swAttorney.ClientID %>');
        var statusDiv = document.getElementById('divStatusFilters');
        var actionDiv = document.getElementById('divActionTypes');

        if (type === 3) {
            searchTerm.style.display = 'none';
            attorney.style.display = '';
            statusDiv.style.display = '';
        } else {
            searchTerm.style.display = '';
            attorney.style.display = 'none';
            statusDiv.style.display = 'none';
        }

        // Action Type only applies to Case Name searches.
        if (actionDiv) actionDiv.style.display = (type === 1) ? '' : 'none';
    }

    // Multi-select Action Type dropdown. Nothing ticked means "All Action Types".
    function InitActionDropdown() {
        var $ = jQuery;
        var $root = $('.action-dropdown');
        if (!$root.length) return;

        var $label = $('#actionSelectedLabel');
        var $count = $('#actionCount');
        var $all = $root.find('.action-select-all');
        var $filter = $root.find('.action-filter');
        function $boxes() { return $root.find('.action-checklist input[type=checkbox]'); }

        // Bootstrap 5 expects these classes; asp:CheckBoxList renders bare input/label pairs.
        $boxes().addClass('form-check-input').next('label').addClass('form-check-label');
        $root.find('.dropdown-menu').off('click.action').on('click.action', function (e) { e.stopPropagation(); });

        // Flag the two designations the court wants to find quickly.
        $root.find('.action-checklist label').each(function () {
            var t = $(this).text();
            if (/vexatious/i.test(t) || (/enjoin/i.test(t) && /pro se/i.test(t))) {
                if (!$(this).find('.action-flag').length) $(this).append('<span class="action-flag">Prohibited filer</span>');
            }
        });

        function UpdateLabel() {
            var selected = $boxes().filter(':checked').map(function () {
                return $(this).next('label').clone().children().remove().end().text().trim();
            }).get();

            $all.prop('checked', selected.length === 0);
            if (selected.length === 0) {
                $label.text('All Action Types');
                $count.text('All selected');
            } else {
                $label.text(selected.length <= 3 ? selected.join(', ') : selected.length + ' action types selected');
                $count.text(selected.length + ' of ' + $boxes().length + ' selected');
            }
        }

        $all.off('click.action').on('click.action', function () {
            $boxes().prop('checked', false);
            this.checked = true;
            UpdateLabel();
        });
        $boxes().off('change.action').on('change.action', UpdateLabel);
        $root.find('.action-reset').off('click.action').on('click.action', function () {
            $boxes().prop('checked', false);
            UpdateLabel();
        });
        $filter.off('input.action').on('input.action', function () {
            var term = $(this).val().toLowerCase();
            $root.find('.action-checklist label').each(function () {
                $(this).parent().toggle($(this).text().toLowerCase().indexOf(term) !== -1);
            });
        });

        UpdateLabel();
    }

    function InitializeAttorneyDropDown() {
        var ddl = document.getElementById('<%= swAttorney.ClientID %>');
        if (!ddl) return;

        var select = ddl;
        var options = Array.from(select.options);
        var newSelect = document.createElement('select');
        newSelect.id = select.id;
        newSelect.name = select.name;
        newSelect.className = select.className;
        newSelect.style.cssText = select.style.cssText;

        var currentGroup = null;
        options.forEach(function (opt) {
            if (opt.value === '<') {
                currentGroup = document.createElement('optgroup');
                currentGroup.label = 'Active';
                newSelect.appendChild(currentGroup);
            } else if (opt.value === '>') {
                currentGroup = document.createElement('optgroup');
                currentGroup.label = 'Inactive';
                newSelect.appendChild(currentGroup);
            } else {
                var newOpt = opt.cloneNode(true);
                if (currentGroup) {
                    currentGroup.appendChild(newOpt);
                } else {
                    newSelect.appendChild(newOpt);
                }
            }
        });
        select.parentNode.replaceChild(newSelect, select);
    }

    function GetCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
        return null;
    }

    function CookieFieldInitialization() {
        var searchType = GetCookie('cc_searchType');
        if (searchType) {
            SetSearchType(parseInt(searchType));
        }
    }

    function PageInit() {
        jQuery(document).ready(function ($) {
            // Bootstrap checkbox styling (exact methodology from ManageYears.ascx)
            $(".form-check input").addClass("form-check-input");
            $(".form-check label").addClass("form-check-label");

            if ($.fn.DataTable) {
                $('#log-list').DataTable({
                    "order": [[1, "asc"]],
                    "pageLength": 25,
                    "columnDefs": [
                        { "orderable": false, "targets": 0 }
                    ]
                });
            }
            InitializeAttorneyDropDown();
            InitActionDropdown();
            CookieFieldInitialization();
        });
    }
    PageInit();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            PageInit();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />