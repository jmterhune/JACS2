<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid mt-3">
    <div class="mb-3">
        <asp:LinkButton ID="cmdReferral" runat="server" CssClass="btn btn-primary" OnClick="cmdReferral_Click">
            <i class="fas fa-plus"></i>&nbsp;Add Motion
        </asp:LinkButton>
    </div>

    <div class="card">
        <div class="card-header">
            <a data-bs-toggle="collapse" href="#searchFiltersCollapse" role="button" aria-expanded="true" aria-controls="searchFiltersCollapse"
               class="text-decoration-none d-flex justify-content-between align-items-center collapse-toggle">
                <strong><i class="fas fa-filter"></i>&nbsp;Search for Referral</strong>
                <span class="collapse-indicator">
                    <i class="fas fa-minus icon-expanded"></i>
                    <i class="fas fa-plus icon-collapsed"></i>
                </span>
            </a>
        </div>
        <div id="searchFiltersCollapse" class="collapse show">
            <div class="card-body p-3">
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label for="<%=drpStatus.ClientID %>" class="fw-bold">Status</label>
                        <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; Select Status &gt;" Value=""></asp:ListItem>
                            <asp:ListItem Text="New" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Pending" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Referred to Court Counsel" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Retained by Judge" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="5"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3" id="divJudge" runat="server">
                        <label for="<%=drpJudge.ClientID %>" class="fw-bold">Judge</label>
                        <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; Select Judge &gt;" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label for="<%=txtCaseNumber.ClientID %>" class="fw-bold">Case Number</label>
                        <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label for="<%=txtMotionTitle.ClientID %>" class="fw-bold">Motion Title</label>
                        <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-3">
                        <label for="<%=dpStartDate.ClientID %>" class="fw-bold">Start Date</label>
                        <asp:TextBox ID="dpStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="col-md-3">
                        <label for="<%=dpEndDate.ClientID %>" class="fw-bold">End Date</label>
                        <asp:TextBox ID="dpEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                </div>

                <div class="mb-0">
                    <asp:LinkButton ID="cmdSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="cmdSearch_Click" />
                    <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
                </div>
            </div>
        </div>
    </div>
    <br />

    <asp:Repeater ID="rptReferral" runat="server">
        <HeaderTemplate>
            <table id="table-referrals" class="table table-striped" style="width:100%">
                <thead>
                    <tr>
                        <th class="no-sort">&nbsp;</th>
                        <th>Case Number</th>
                        <th>Case Name</th>
                        <th>Motion Title</th>
                        <th>Judge</th>
                        <th>Created</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="command-item">
                    <a title="View Referral" href='<%#EditUrl("rid", Eval("ReferralId").ToString(), "review") %>'>
                        <i class="fas fa-search"></i>
                    </a>
                </td>
                <td class="text-nowrap"><%# Eval("CaseNumber") %></td>
                <td><%# Eval("CaseParties") %></td>
                <td><%# Eval("MotionTitle") %></td>
                <td><%# Eval("JudgeName") %></td>
                <td data-order='<%# Eval("JaCreatedDate", "{0:yyyyMMdd}") %>'><%# Eval("JaCreatedDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("StatusName") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    function InitReferralsTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-referrals')) {
                $('#table-referrals').DataTable({
                    "order": [[5, "desc"]],
                    "pageLength": 25,
                    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    "columnDefs": [
                        { "orderable": false, "searchable": false, "targets": 0 }
                    ]
                });
            }
        });
    }
    InitReferralsTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-referrals')) {
                jQuery('#table-referrals').DataTable().destroy();
            }
            InitReferralsTable();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
