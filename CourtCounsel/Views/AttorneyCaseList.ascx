<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttorneyCaseList.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.AttorneyCaseList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
    <h2 class="mb-0"><asp:Label ID="lblAttorneyName" runat="server" /></h2>
    <div id="actionFilterHost" class="ms-auto d-flex align-items-center"></div>
</div>

<div class="table-responsive">
    <table id="attorney-case-list" class="table table-striped table-bordered table-hover" style="width:100%">
        <thead>
            <tr>
                <th>Case Name</th>
                <th>Case Number</th>
                <th>Case Type</th>
                <th>Attorney</th>
                <th>Action Date</th>
                <th>Status</th>
                <th id="thCompleted" runat="server">Completed</th>
                <th data-action-col>Action Taken</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptAttorneyCaseList" runat="server" OnItemDataBound="rptAttorneyCaseList_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td>
                            <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") + "?pn=" + Server.UrlEncode(Eval("PartyName").ToString()) %>'
                               title='<%# Eval("PartyName") %>'>
                                <%# FormatLongName(Eval("PartyName") as string) %>
                            </a>
                        </td>
                        <td>
                            <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") %>'>
                                <%# (Eval("CaseNumber") as string ?? string.Empty).ToUpper() %>
                            </a>
                        </td>
                        <td><%#Eval("CaseType") %></td>
                        <td><%#Eval("Responsible") %></td>
                        <td data-order='<%#FormatDateIso((DateTime?)Eval("DateReceived")) %>'><%#FormatDate((DateTime?)Eval("DateReceived")) %></td>
                        <td><%#GetStatusBadge((tjc.Modules.CourtCounsel.Components.Models.HistoryInfo)Container.DataItem) %></td>
                        <td runat="server" ID="tdCompleted" data-order='<%#FormatDateIso((DateTime?)Eval("DateCompleted")) %>'><%#FormatDate((DateTime?)Eval("DateCompleted")) %></td>
                        <td><%#Eval("Action") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>

<script type="text/javascript">
    function PageInit() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable) {
                $('#attorney-case-list').DataTable({
                    "order": [[4, "asc"]],   // Action Date ascending; date-aware via data-order
                    "pageLength": 25
                });
                if (window.CCActionFilter) CCActionFilter.init({ table: '#attorney-case-list', host: '#actionFilterHost' });
            }
        });
    }
    PageInit();
</script>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/CourtCounsel/Scripts/action-filter.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
