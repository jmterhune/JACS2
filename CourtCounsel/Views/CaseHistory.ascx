<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseHistory.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.CaseHistory" %>
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
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="mt-3">
    <div class="card mb-3">
        <div class="card-body p-3">
            <h5 class="card-title">
                Case Number: <strong><asp:Literal ID="litCaseNumber" runat="server" /></strong>
            </h5>
            <div>
                <strong>Party Names:</strong>
                <asp:Repeater ID="rptNames" runat="server">
                    <ItemTemplate>
                        <span class="badge bg-secondary me-1"><%#Container.DataItem %></span>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

    <div class="table-responsive">
        <table id="history-list" class="table table-striped table-bordered table-hover" style="width:100%">
            <thead>
                <tr>
                    <th>Action Date</th>
                    <th>Action Taken</th>
                    <th>Responsible</th>
                    <th>Status</th>
                    <th>Date Completed</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptHistory" runat="server" OnItemCommand="rptHistory_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td data-order='<%#FormatDateIso(Eval("DateReceived")) %>'>
                                <a href='<%#EditUrl("lid",Eval("LogId").ToString(),"EditHistory") %>'>
                                    <%#FormatDate(Eval("DateReceived")) %>
                                </a>
                            </td>
                            <td><%#Eval("Action") %></td>
                            <td><%#Eval("Responsible") %></td>
                            <td><%#Eval("StatusName") %></td>
                            <td data-order='<%#FormatDateIso(Eval("DateCompleted")) %>'><%#FormatDate(Eval("DateCompleted")) %></td>
                            <td>
                                <asp:LinkButton runat="server" CommandName="delete" CommandArgument='<%#Eval("LogId") %>'
                                    CausesValidation="false" CssClass="confirm btn btn-sm btn-danger"
                                    ToolTip="Delete this record">
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <div class="mt-3">
        <asp:HyperLink ID="lnkAddNew" runat="server" CssClass="btn btn-success">
            <i class="fas fa-plus"></i>&nbsp;Add New Record
        </asp:HyperLink>
    </div>
</div>

<script type="text/javascript">
    function PageInit() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable) {
                $('#history-list').DataTable({
                    "order": [[4, "asc"]],   // Date Completed ascending; empty data-order sorts first so open records stay at top
                    "pageLength": 25,
                    "columnDefs": [
                        { "orderable": false, "targets": -1 }
                    ]
                });
            }

            $('.confirm').on('click', function () {
                return confirm('Are you sure you want to delete this record?');
            });
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
