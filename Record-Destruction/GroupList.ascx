<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupList.ascx.cs" Inherits="tjc.Modules.RecordDestruction.GroupList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DestructionFormURL %>">Record Destruction Log</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SearchLogUrl %>">Search Log</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#groups" data-toggle="tab">Departments</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RecordTypeListUrl %>">Record Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RetentionPeriodListUrl %>">Retention Periods</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DestructionMethodListUrl %>">Destruction Methods</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="groups" class="tab-pane active">
            <div class="alert alert-info"><i class="fa fa-info-circle"></i> Modifications to Departments are mangaged through the Employee Database Application.</div>
            <asp:Repeater ID="rptGroups" runat="server">
                <HeaderTemplate>
                    <table id="tblGroup" class="table table-striped">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Department</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-item"><%#Eval("GroupID")%></td>
                        <td><%#Eval("GroupName")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblGroup').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": true },
                { "bSortable": true },],
            autoWidth: true,
        });
    }
</script>

