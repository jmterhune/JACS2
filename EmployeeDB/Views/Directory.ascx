<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Directory.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.Directory" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <div class="d-flex flex-wrap gap-2 align-items-center mb-3">
        <h3 class="mb-0"><i class="fas fa-address-book"></i>&nbsp;Employee Directory</h3>
        <div class="ms-auto">
            <button type="button" id="btnDirectoryPrint" class="btn btn-outline-secondary">
                <i class="fas fa-print"></i>&nbsp;Print
            </button>
            <button type="button" id="btnDirectoryPdf" class="btn btn-outline-secondary">
                <i class="fas fa-file-pdf"></i>&nbsp;PDF
            </button>
        </div>
    </div>

    <div class="card">
        <div class="card-header">
            <a data-bs-toggle="collapse" href="#directorySearchCollapse" role="button" aria-expanded="true" aria-controls="directorySearchCollapse"
               class="text-decoration-none d-flex justify-content-between align-items-center collapse-toggle">
                <strong><i class="fas fa-filter"></i>&nbsp;Search</strong>
                <span class="collapse-indicator">
                    <i class="fas fa-minus icon-expanded"></i>
                    <i class="fas fa-plus icon-collapsed"></i>
                </span>
            </a>
        </div>
        <div id="directorySearchCollapse" class="collapse show">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-3">
                        <label for="<%= txtFirstName.ClientID %>" class="fw-bold">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                    <div class="col-md-3">
                        <label for="<%= txtLastName.ClientID %>" class="fw-bold">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                    <div class="col-md-3">
                        <label for="<%= drpDepartment.ClientID %>" class="fw-bold">Department</label>
                        <asp:DropDownList ID="drpDepartment" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; All Departments &gt;" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label for="<%= drpCounty.ClientID %>" class="fw-bold">County</label>
                        <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; All Counties &gt;" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row">
                    <div class="col-12">
                        <asp:LinkButton ID="cmdSearch" runat="server" CssClass="btn btn-primary" OnClick="cmdSearch_Click">
                            <i class="fas fa-search"></i>&nbsp;Search
                        </asp:LinkButton>
                        <asp:LinkButton ID="cmdReset" runat="server" CssClass="btn btn-secondary" OnClick="cmdReset_Click">
                            <i class="fas fa-undo"></i>&nbsp;Reset
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <br />

    <asp:Repeater ID="rptDirectory" runat="server">
        <HeaderTemplate>
            <table id="table-directory" class="table table-striped table-hover" style="width:100%">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Title</th>
                        <th>Department</th>
                        <th>Work Phone</th>
                        <th>Work Email</th>
                        <th>Office Location</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <a href='<%# EditUrl("eid", Eval("EmployeeId").ToString(), "DetailPopUp") %>'
                       class="directory-name" data-id='<%# Eval("EmployeeId") %>'>
                        <%# Eval("LastName") %>, <%# Eval("FirstName") %>
                    </a>
                </td>
                <td><%# Eval("JobTitle") %></td>
                <td><%# Eval("DepartmentName") %></td>
                <td class="text-nowrap"><%# Eval("WorkPhoneLink") %></td>
                <td>
                    <%# string.IsNullOrEmpty(Eval("Email") as string) ? "" :
                        string.Format("<a href=\"mailto:{0}\">{0}</a>", Eval("Email")) %>
                </td>
                <td><%# Eval("LocationName") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    function InitDirectoryTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-directory')) {
                var dt = $('#table-directory').DataTable({
                    "order": [[0, "asc"]],
                    "pageLength": 25,
                    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]]
                });

                $('#btnDirectoryPrint').off('click.dir').on('click.dir', function () {
                    window.print();
                });
                $('#btnDirectoryPdf').off('click.dir').on('click.dir', function () {
                    window.print();
                });
            }
        });
    }
    InitDirectoryTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-directory')) {
                jQuery('#table-directory').DataTable().destroy();
            }
            InitDirectoryTable();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
