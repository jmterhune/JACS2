<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Birthdays.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.Birthdays" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3><i class="fas fa-birthday-cake"></i>&nbsp;Birthdays</h3>

    <div class="card">
        <div class="card-header">
            <strong><i class="fas fa-filter"></i>&nbsp;Filter</strong>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3">
                    <label for="<%= drpMonth.ClientID %>" class="fw-bold">Month</label>
                    <asp:DropDownList ID="drpMonth" runat="server" CssClass="form-control">
                        <asp:ListItem Text="January" Value="1" />
                        <asp:ListItem Text="February" Value="2" />
                        <asp:ListItem Text="March" Value="3" />
                        <asp:ListItem Text="April" Value="4" />
                        <asp:ListItem Text="May" Value="5" />
                        <asp:ListItem Text="June" Value="6" />
                        <asp:ListItem Text="July" Value="7" />
                        <asp:ListItem Text="August" Value="8" />
                        <asp:ListItem Text="September" Value="9" />
                        <asp:ListItem Text="October" Value="10" />
                        <asp:ListItem Text="November" Value="11" />
                        <asp:ListItem Text="December" Value="12" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label for="<%= drpCounty.ClientID %>" class="fw-bold">County</label>
                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row">
                <div class="col-12">
                    <asp:LinkButton ID="cmdReport" runat="server" CssClass="btn btn-primary" OnClick="cmdReport_Click">
                        <i class="fas fa-play"></i>&nbsp;Report
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <br />

    <asp:Repeater ID="rptBirthdays" runat="server">
        <HeaderTemplate>
            <table id="table-birthdays" class="table table-striped table-hover" style="width:100%">
                <thead>
                    <tr>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Birth Date</th>
                        <th>Office Location</th>
                        <th>Department</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("FirstName") %></td>
                <td><%# Eval("LastName") %></td>
                <td data-order='<%# Eval("BirthOrder") %>'><%# Eval("BirthDate", "{0:MMM dd}") %></td>
                <td><%# Eval("LocationName") %></td>
                <td><%# Eval("DepartmentName") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    function InitBirthdaysTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-birthdays')) {
                $('#table-birthdays').DataTable({
                    "order": [[2, "asc"]],
                    "pageLength": 25
                });
            }
        });
    }
    InitBirthdaysTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-birthdays')) {
                jQuery('#table-birthdays').DataTable().destroy();
            }
            InitBirthdaysTable();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
