<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.EmployeeDB.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-user"></i>&nbsp;Employees</a>
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

    </ul>
    <div class="tab-content">
        <div id="Employees" class="tab-pane active">
            <asp:Repeater ID="rptEmployees" runat="server">
                <HeaderTemplate>
                    <table id="tblEmployees" class="table table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Phone</th>
                                <th>Department</th>
                                <th>Active</th>
                                <th class="d-none"></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-icon">
                            <asp:HyperLink runat="server" ID="cmdEdit" NavigateUrl='<%#EditUrl("eid",DataBinder.Eval(Container.DataItem,"EmployeeId").ToString(),"Employee") %>' ToolTip="Edit Employee Record"><i class="fa fa-pencil-alt"></i></asp:HyperLink>
                        </td>
                        <td><%#DataBinder.Eval(Container.DataItem,"FullName") %></td>
                        <td><a href='mailto:<%#DataBinder.Eval(Container.DataItem,"Email") %>'><%#DataBinder.Eval(Container.DataItem,"Email") %></a></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"Phones") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"Department") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"IsActive").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                        <td class="d-none"><%#DataBinder.Eval(Container.DataItem,"DepartmentId") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table><hr />
                </FooterTemplate>
            </asp:Repeater>
            <div id="swActive" class="input-group ms-md switch">
                <div class="form-check form-switch">
                    <asp:CheckBox ID="chkInactiveEmployees" Checked="true" AutoPostBack="true" OnCheckedChanged="chkInactiveEmployees_CheckedChanged" ClientIDMode="Static" runat="server" Text="Active Employees" />
                </div>
            </div>
        </div>
    </div>
</div>
<dnn:dnnjsInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblEmployees').DataTable({

            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },]
        });

        $("#tblEmployees_length").prepend("<%=DepartmentFilterHtml%>");
        table.draw();

        $('#drpfilter').change(function () {
            table.draw();
        });

    }
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            var selectedValue = $("#drpfilter option:selected").val();
            var msdsSearch = parseInt(selectedValue);
            var msdsValue = parseFloat(data[7]) || 0; // use data for the section column
            if (msdsSearch == -1) {
                return true;
            }

            if (msdsSearch == msdsValue) {
                return true;
            }

            return false;
        }
    );
</script>
