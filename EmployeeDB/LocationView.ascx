<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocationView.ascx.cs" Inherits="tjc.Modules.EmployeeDB.LocationView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
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
                <asp:HyperLink runat="server" ID="cmdEdit" Target="_blank" NavigateUrl='<%#EditUrl("eid",DataBinder.Eval(Container.DataItem,"EmployeeId").ToString()) %>' ToolTip="Edit Employee Record"><i class="fa fa-pencil-alt"></i></asp:HyperLink>
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
<div id="swActive" class="input-group ml-md switch">
    <div class="custom-control custom-switch">
        <asp:CheckBox ID="chkInactiveEmployees" Checked="true" OnCheckedChanged="chkInactiveEmployees_CheckedChanged" AutoPostBack="true" ClientIDMode="Static" runat="server" />
        <asp:Label CssClass="custom-control-label" runat="server" ID="lblInactiveEmployees" AssociatedControlID="chkInactiveEmployees">Toggle Off for Inactive Employees</asp:Label>

    </div>
</div>
<div class="form-check form-switch">
</div>
<dnn:dnnjsInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />

<script type="text/javascript">
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

    (function ($, Sys) {

        $(document).ready(function () {
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

            $("#tblEmployees_length").prepend("<%=DrpSortHtml%>");
            table.draw();

            $('#drpfilter').change(function () {
                table.draw();
            });

        });

    }(jQuery, window.Sys));

</script>
