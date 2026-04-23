<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailsList.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.DetailsList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <div class="d-flex flex-wrap gap-2 align-items-center mb-3">
        <h3 class="mb-0"><i class="fas fa-list"></i>&nbsp;Employee Details</h3>
        <div class="ms-auto">
            <button type="button" id="btnDetailsExcel" class="btn btn-success">
                <i class="fas fa-file-excel"></i>&nbsp;Export to Excel
            </button>
        </div>
    </div>

    <asp:Repeater ID="rptDetails" runat="server">
        <HeaderTemplate>
            <table id="table-details" class="table table-striped table-hover table-bordered" style="width:100%">
                <thead>
                    <tr>
                        <th>First Name</th>
                        <th>MI</th>
                        <th>Last Name</th>
                        <th>Job Title</th>
                        <th>SSN</th>
                        <th>Birth Date</th>
                        <th>Race</th>
                        <th>Gender</th>
                        <th>Hire Date</th>
                        <th>Service Date</th>
                        <th>Supervisor</th>
                        <th>Department</th>
                        <th>Class</th>
                        <th>Category</th>
                        <th>Position</th>
                        <th>Agency</th>
                        <th>County</th>
                        <th>Location</th>
                        <th>Employment Type</th>
                        <th>Salary</th>
                        <th>Annual Leave</th>
                        <th>Sick Leave</th>
                        <th>Address</th>
                        <th>City</th>
                        <th>State</th>
                        <th>Zip</th>
                        <th>Email</th>
                        <th>Personal Email</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("FirstName") %></td>
                <td><%# Eval("MiddleInitial") %></td>
                <td><%# Eval("LastName") %></td>
                <td><%# Eval("JobTitle") %></td>
                <td><%# Eval("MaskedSsn") %></td>
                <td data-order='<%# Eval("BirthDate", "{0:yyyyMMdd}") %>'><%# Eval("BirthDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("Race") %></td>
                <td><%# Eval("Gender") %></td>
                <td data-order='<%# Eval("HireDate", "{0:yyyyMMdd}") %>'><%# Eval("HireDate", "{0:MM/dd/yyyy}") %></td>
                <td data-order='<%# Eval("ServiceDate", "{0:yyyyMMdd}") %>'><%# Eval("ServiceDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("SupervisorName") %></td>
                <td><%# Eval("DepartmentName") %></td>
                <td><%# Eval("ClassName") %></td>
                <td><%# Eval("JobGroupName") %></td>
                <td><%# Eval("Position") %></td>
                <td><%# Eval("AgencyOfEmployment") %></td>
                <td><%# Eval("CountyName") %></td>
                <td><%# Eval("LocationName") %></td>
                <td><%# Eval("EmploymentType") %></td>
                <td data-order='<%# Eval("SalaryOrder") %>'><%# Eval("Salary", "{0:C}") %></td>
                <td data-order='<%# Eval("AnnualLeaveOrder") %>'><%# Eval("AnnualLeaveBalance", "{0:N2}") %></td>
                <td data-order='<%# Eval("SickLeaveOrder") %>'><%# Eval("SickLeaveBalance", "{0:N2}") %></td>
                <td><%# Eval("Address") %></td>
                <td><%# Eval("City") %></td>
                <td><%# Eval("State") %></td>
                <td><%# Eval("Zip") %></td>
                <td><%# Eval("Email") %></td>
                <td><%# Eval("PersonalEmail") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    function InitDetailsTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-details')) {
                var dt = $('#table-details').DataTable({
                    "order": [[2, "asc"]],
                    "pageLength": 25,
                    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    "scrollX": true,
                    "dom": 'Bfrtip',
                    "buttons": [
                        { extend: 'excelHtml5', title: 'EmployeeDetails' },
                        { extend: 'csvHtml5', title: 'EmployeeDetails' },
                        { extend: 'print', title: 'Employee Details' }
                    ]
                });

                $('#btnDetailsExcel').off('click.det').on('click.det', function () {
                    dt.button('.buttons-excel').trigger();
                });
            }
        });
    }
    InitDetailsTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-details')) {
                jQuery('#table-details').DataTable().destroy();
            }
            InitDetailsTable();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.buttons.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jszip.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.html5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/buttons.print.min.js" />
