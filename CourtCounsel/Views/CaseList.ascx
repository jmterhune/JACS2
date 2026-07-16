<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseList.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.CaseList" %>
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

<h2><asp:Literal ID="litResultsHeading" runat="server" Text="Case Search Results" /></h2>

<div class="table-responsive">
    <table id="case-list" class="table table-striped table-bordered table-hover" style="width:100%">
        <thead>
            <tr>
                <th>Case Name</th>
                <th>Case Number</th>
                <th>Case Type</th>
                <th>Attorney</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptCaseList" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") + "?pn=" + Server.UrlEncode(Eval("PartyName").ToString()) %>'>
                                <%#FormatLongName(Eval("PartyName").ToString()) %>
                            </a>
                        </td>
                        <td>
                            <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") + "?pn=" + Server.UrlEncode(Eval("PartyName").ToString()) %>'>
                                <%#Eval("CaseNumber") %>
                            </a>
                        </td>
                        <td><%#Eval("CaseType") %></td>
                        <td><%#Eval("Responsible") %></td>
                        <td><%#GetStatus((tjc.Modules.CourtCounsel.Components.Models.HistoryInfo)Container.DataItem) %></td>
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
                $('#case-list').DataTable({
                    "order": [[0, "asc"]],
                    "pageLength": 25
                });
            }
        });
    }
    PageInit();
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
