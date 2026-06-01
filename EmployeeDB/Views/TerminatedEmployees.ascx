<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TerminatedEmployees.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.TerminatedEmployees" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3><i class="fas fa-user-slash"></i>&nbsp;Terminated Employees</h3>

    <div class="card">
        <div class="card-header">
            <strong><i class="fas fa-calendar"></i>&nbsp;Date Range</strong>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3">
                    <label for="<%= dpStartDate.ClientID %>" class="fw-bold">Start Date</label>
                    <asp:TextBox ID="dpStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
                <div class="col-md-3">
                    <label for="<%= dpEndDate.ClientID %>" class="fw-bold">End Date</label>
                    <asp:TextBox ID="dpEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
            </div>

            <div class="row">
                <div class="col-12">
                    <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-primary" OnClick="btnSubmit_Click">
                        <i class="fas fa-search"></i>&nbsp;Submit
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <br />

    <asp:Repeater ID="rptTerminated" runat="server">
        <HeaderTemplate>
            <table id="table-terminated" class="table table-striped table-hover" style="width:100%">
                <thead>
                    <tr>
                        <th>Last Name</th>
                        <th>First Name</th>
                        <th>Termination Date</th>
                        <th>Job Title</th>
                        <th>Department</th>
                        <th>Hire Date</th>
                        <th>Length of Service</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("LastName") %></td>
                <td><%# Eval("FirstName") %></td>
                <td data-order='<%# Eval("TerminationDate", "{0:yyyyMMdd}") %>'><%# Eval("TerminationDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("JobTitle") %></td>
                <td><%# Eval("DepartmentName") %></td>
                <td data-order='<%# Eval("HireDate", "{0:yyyyMMdd}") %>'><%# Eval("HireDate", "{0:MM/dd/yyyy}") %></td>
                <td data-order='<%# Eval("ServiceDays") %>'><%# Eval("LengthOfService") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    function InitTerminatedTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-terminated')) {
                $('#table-terminated').DataTable({
                    "order": [[2, "desc"]],
                    "pageLength": 25
                });
            }
        });
    }
    InitTerminatedTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-terminated')) {
                jQuery('#table-terminated').DataTable().destroy();
            }
            InitTerminatedTable();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
