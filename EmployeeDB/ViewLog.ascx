<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLog.ascx.cs" Inherits="tjc.Modules.EmployeeDB.ViewLog" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item ">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-id-badge"></i>&nbsp;Employees</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ContactUrl%>"><i class="fas fa-user"></i>&nbsp;Contacts</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentUrl%>"><i class="fas fa-sitemap"></i>&nbsp;Departments</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobGroupUrl%>"><i class="fas fa-users"></i>&nbsp;Job Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobClassUrl%>"><i class="fas fa-user-tag"></i>&nbsp;Job Classes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RaceUrl%>"><i class="fas fa-users-cog"></i>&nbsp;Race</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CountyUrl%>"><i class="fas fa-map-marked-alt"></i>&nbsp;Counties</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationUrl%>"><i class="fas fa-building"></i>&nbsp;Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link active" href="<%=SwnLogUrl%>"><i class="fas fa-exclamation-circle"></i>&nbsp;SWN Interface Log</a>
        </li>

    </ul>
    <div class="tab-content">
        <div id="Log" class="tab-pane active">
            <div class="btn-group date-filter mb-3" role="group" aria-label="SWN Log Toolbar">
                <div id="swStartDate" class="input-group" aria-label="Filter Criteria">
                    <div class="input-group-text" id="lblStartDate">Start Date</div>
                    <asp:TextBox ID="txtStartDate" ClientIDMode="Static" runat="server" CssClass="form-control datepicker" aria-label="Start Date" aria-describedby="lblStartDate"></asp:TextBox>
                    <div class="input-group-text" id="lblEndDate">End Date</div>
                    <asp:TextBox ID="txtEndDate" ClientIDMode="Static" runat="server" CssClass="form-control datepicker" aria-label="End Date" aria-describedby="lblEndDate"></asp:TextBox>
                </div>
                <asp:Button ID="cmdFilter" Text="Filter Results" CssClass="btn btn-primary" runat="server" OnClick="cmdFilter_Click" />
                <asp:Button ID="cmdClearLog" Text="Clear Log" CssClass="btn btn-secondary confirm" runat="server" OnClick="cmdClearLog_Click" />
            </div>
            <asp:Repeater ID="rptLog" runat="server">
                <HeaderTemplate>
                    <table id="tblLog" class="table table-striped">
                        <thead>
                            <tr>

                                <th>LogId</th>
                                <th>Process</th>
                                <th>Error</th>
                                <th>Created By</th>
                                <th>Created Date</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#DataBinder.Eval(Container.DataItem,"LogId") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"Process") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"Exception") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"CreatedByName") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"CreatedDate","{0:MM/dd/yyyy}") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        $(".datepicker").datepicker();
        var table = $('#tblLog').DataTable({

            "order": [[0, "desc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },]
        });

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to clear the log?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Clear Log',
            isButton: true
        });
        table.draw();

    }
</script>
