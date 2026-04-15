<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Search" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav">
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
                <div class="input-group-prepend">
                    <button class="btn btn-outline-secondary dropdown-toggle" type="button" id="btnSearchType" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        Case Name
                    </button>
                    <div class="dropdown-menu">
                        <a class="dropdown-item" href="#" onclick="SetSearchType(1); return false;">Case Name</a>
                        <a class="dropdown-item" href="#" onclick="SetSearchType(2); return false;">Case Number</a>
                        <a class="dropdown-item" href="#" onclick="SetSearchType(3); return false;">Attorney</a>
                    </div>
                </div>
                <asp:TextBox ID="swSearchTerm" runat="server" CssClass="form-control" placeholder="Enter search term..." />
                <asp:DropDownList ID="swAttorney" runat="server" CssClass="form-control" style="display:none;" />
                <div class="input-group-append">
                    <asp:Button ID="cmdSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="cmdSearch_Click" />
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div id="divStatusFilters" style="display:none;">
                <div class="form-check form-check-inline">
                    <input type="checkbox" class="form-check-input" id="swActive" checked="checked" />
                    <label class="form-check-label" for="swActive">Active</label>
                </div>
                <div class="form-check form-check-inline">
                    <input type="checkbox" class="form-check-input" id="swPending" />
                    <label class="form-check-label" for="swPending">Pending</label>
                </div>
                <div class="form-check form-check-inline">
                    <input type="checkbox" class="form-check-input" id="swClosed" />
                    <label class="form-check-label" for="swClosed">Closed</label>
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

        if (type === 3) {
            searchTerm.style.display = 'none';
            attorney.style.display = '';
            statusDiv.style.display = '';
        } else {
            searchTerm.style.display = '';
            attorney.style.display = 'none';
            statusDiv.style.display = 'none';
        }
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
