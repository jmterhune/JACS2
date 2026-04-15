<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataSheet.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.DataSheet" %>
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
            <li class="nav-item"><a class="nav-link active" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="container-fluid mt-3">
    <div class="row mb-3">
        <div class="col-md-8">
            <label class="font-weight-bold">Filter by Attorney:</label>
            <asp:CheckBoxList ID="cblAttorneys" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-check-inline" />
        </div>
        <div class="col-md-4 text-right">
            <asp:Button ID="cmdFilter" runat="server" CssClass="btn btn-primary" Text="Filter" OnClick="cmdFilter_Click" />
            <asp:Button ID="cmdClear" runat="server" CssClass="btn btn-secondary ml-1" Text="Clear" OnClick="cmdClear_Click" />
        </div>
    </div>
</div>

<asp:UpdatePanel ID="upSheet" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div style="overflow-x:auto;">
            <table id="sheet-table" class="table table-striped table-bordered table-hover" style="width:100%">
                <thead>
                    <tr>
                        <th>Date Received</th>
                        <th>Case Number</th>
                        <th>Party Name</th>
                        <th>Case Type</th>
                        <th>Date Due</th>
                        <th>Requested By</th>
                        <th>Responsible</th>
                        <th>Motion Filed</th>
                        <th>County</th>
                        <th>Action</th>
                        <th>Follow Up</th>
                        <th>Date Completed</th>
                        <th>Time Spent</th>
                        <th>Status</th>
                        <th>Comments</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptSheet" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("DateReceived", "{0:d}") %></td>
                                <td>
                                    <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") %>'>
                                        <%#Eval("CaseNumber") %>
                                    </a>
                                </td>
                                <td><%#Eval("PartyName") %></td>
                                <td><%#Eval("CaseType") %></td>
                                <td><%#Eval("DateDue", "{0:d}") %></td>
                                <td><%#Eval("RequestedBy") %></td>
                                <td><%#Eval("Responsible") %></td>
                                <td><%#Eval("MotionFiled", "{0:d}") %></td>
                                <td><%#Eval("County") %></td>
                                <td><%#Eval("Action") %></td>
                                <td><%#Eval("FollowUp") %></td>
                                <td><%#Eval("DateCompleted", "{0:d}") %></td>
                                <td><%#Eval("TimeSpent") %></td>
                                <td><%#Eval("StatusName") %></td>
                                <td><%#Eval("Comments") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="cmdFilter" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="cmdClear" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

<script type="text/javascript">
    function PageInit() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable) {
                if ($.fn.DataTable.isDataTable('#sheet-table')) {
                    $('#sheet-table').DataTable().destroy();
                }
                $('#sheet-table').DataTable({
                    "order": [[0, "desc"]],
                    "pageLength": 50,
                    "dom": 'Bfrtip',
                    "buttons": ['excel', 'csv', 'pdf'],
                    "scrollX": true
                });
            }
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
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.buttons.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.html5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jszip.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/pdfmake.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/vfs_fonts.js" />
