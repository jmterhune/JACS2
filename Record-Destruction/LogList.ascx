<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogList.ascx.cs" Inherits="tjc.Modules.RecordDestruction.LogItemList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="set-width">
    <div class="tabs">
        <ul class="nav nav-tabs">
            <li class="nav-item">
                <a class="nav-link" href="<%=DestructionFormURL %>">Record Destruction Log</a>
            </li>
            <li class="nav-item active">
                <a class="nav-link" href="#logItems" data-toggle="tab">Search Log</a>
            </li>
             <asp:PlaceHolder ID="phAdminTabs" runat="server" Visible="false">
            <li class="nav-item">
                <a class="nav-link" href="<%=DepartmentListUrl %>">Departments</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=RecordTypeListUrl %>">Record Types</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=RetentionPeriodListUrl %>">Retention Periods</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=DestructionMethodListUrl %>">Destruction Methods</a>
            </li></asp:PlaceHolder>
        </ul>
        <div class="tab-content">
            <div id="logItems" class="tab-pane active">
                <asp:Repeater ID="rptLogItems" runat="server">
                    <HeaderTemplate>
                        <table id="tblLogItem" class="table table-striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Description</th>
                                    <th class="text-nowrap">Year Created</th>
                                    <th>Destroyed</th>
                                    <th>Department</th>
                                    <th>Name</th>
                                    <th class="text-nowrap">Record Type</th>
                                    <th class="text-nowrap">Retention Period</th>
                                    <th class="text-nowrap">Destruction Method</th>
                                    <th>Attachment</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="command-item"><%#Eval("LogID")%></td>
                            <td class="w-75"><%#Eval("Description")%></td>
                            <td><%#Eval("YearCreated")%></td>
                            <td><%#Eval("DateDestroyed","{0:MM/dd/yyyy}")%></td>
                            <td class="text-nowrap"><%#Eval("GroupName")%></td>
                            <td class="text-nowrap "><%#Eval("DisplayName")%></td>
                            <td class="text-nowrap"><%#Eval("RecordType")%></td>
                            <td><%#Eval("RetentionPeriod")%></td>
                            <td class="text-nowrap"><%#Eval("DestructionMethod")%></td>
                            <td><%#Eval("FileLink")%></td>
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
</div>
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();

        });

    }(jQuery, window.Sys));

    function PageInit() {
        $(".set-width").closest('.container').addClass("full-width");
        var table = $('#tblLogItem').DataTable({
            "order": [[0, "desc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },

            ],
            pageLength: 50
        });
        $("#tblLogItem_filter").prepend("<%=DepartmentFilterHtml%>");
        table.draw();

        $('#drpfilter').change(function () {
            table.draw();
        });
    }
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            var selectedValue = $("#drpfilter option:selected").text();
            var msdsSearch = selectedValue;
            var msdsValue = data[4] || "All"; // use data for the section column
            if (msdsSearch == "All") {
                return true;
            }

            if (msdsSearch == msdsValue) {
                return true;
            }

            return false;
        }
    );
</script>

