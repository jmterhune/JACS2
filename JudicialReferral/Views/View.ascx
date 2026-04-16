<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid mt-3">
    <div class="mb-3">
        <asp:LinkButton ID="cmdReferral" runat="server" CssClass="btn btn-primary" OnClick="cmdReferral_Click">
            <i class="fas fa-plus"></i>&nbsp;Add Motion
        </asp:LinkButton>
    </div>

    <div class="card mb-3">
        <div class="card-header">
            <a data-toggle="collapse" href="#searchCollapse" role="button" aria-expanded="true" aria-controls="searchCollapse" class="text-decoration-none">
                <h5 class="mb-0"><i class="fas fa-search"></i>&nbsp;Search for Referral</h5>
            </a>
        </div>
        <div id="searchCollapse" class="collapse show">
            <div class="card-body p-4">
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <label for="<%=drpStatus.ClientID %>">Status</label>
                        <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; Select Status &gt;" Value=""></asp:ListItem>
                            <asp:ListItem Text="New" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Motion Type Set" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Referred to Court Counsel" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Retained by Judge" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Received &amp; Assigned" Value="5"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="6"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group col-md-4" id="divJudge" runat="server">
                        <label for="<%=drpJudge.ClientID %>">Judge</label>
                        <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; Select Judge &gt;" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group col-md-4">
                        <label for="<%=txtCaseNumber.ClientID %>">Case Number</label>
                        <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <label for="<%=txtMotionTitle.ClientID %>">Motion Title</label>
                        <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group col-md-4">
                        <label for="<%=txtStartDate.ClientID %>">Start Date</label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                    <div class="form-group col-md-4">
                        <label for="<%=txtEndDate.ClientID %>">End Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                </div>
                <div class="form-row">
                    <div class="col">
                        <asp:LinkButton ID="cmdSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="cmdSearch_Click" />
                        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="table-responsive">
        <asp:Repeater ID="rptReferral" runat="server" OnItemCommand="rptReferral_ItemCommand" OnItemDataBound="rptReferral_ItemDataBound">
            <HeaderTemplate>
                <table id="table-referrals" class="table table-striped table-bordered table-hover" style="width:100%">
                    <thead>
                        <tr>
                            <th></th>
                            <th>Case Number</th>
                            <th>Case Name</th>
                            <th>Motion Title</th>
                            <th>Judge</th>
                            <th>Created</th>
                            <th>Status</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:HyperLink ID="lnkReview" runat="server" ToolTip="View Referral">
                            <i class="fas fa-edit"></i>
                        </asp:HyperLink>
                    </td>
                    <td><%# Eval("CaseNumber") %></td>
                    <td><%# Eval("CaseParties") %></td>
                    <td><%# Eval("MotionTitle") %></td>
                    <td><%# Eval("JudgeName") %></td>
                    <td><%# Eval("JaCreatedDate", "{0:MM/dd/yyyy}") %></td>
                    <td><%# Eval("StatusName") %></td>
                    <td>
                        <asp:LinkButton ID="cmdComplete" runat="server" Visible="false" CssClass="btn btn-sm btn-success"
                            CausesValidation="false" CommandArgument='<%# Eval("ReferralId") %>' CommandName="complete"
                            OnClientClick="return confirm('Are you sure you wish to complete this referral?');">
                            <i class="fas fa-check"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>

<script type="text/javascript">
    function PageInit() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && $('#table-referrals').length) {
                $('#table-referrals').DataTable({
                    "order": [[5, "desc"]],
                    "pageLength": 25,
                    "columnDefs": [
                        { "orderable": false, "targets": [0, 7] }
                    ]
                });
            }
        });
    }
    PageInit();
    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            PageInit();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
