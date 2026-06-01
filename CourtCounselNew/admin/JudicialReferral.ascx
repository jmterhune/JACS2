<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JudicialReferral.ascx.cs" Inherits="tjc.Modules.CourtCounsel.JudicialReferral" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="active nav-item">
                <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("referrals") %>"><i class="fas fa-gavel"></i>&nbsp;Referrals</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
            </li>
            <li class="nav-item" id="li1" runat="server" visible="false">
                <a class="nav-link" href="<%=MemberListUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=SharePointSiteURL %>"><i class="fas fa-home"></i>&nbsp;Team Site</a>
            </li>
        </ul>
    </div>
</nav>
<div class="mb-md">
    <table id="table-referrals" class="table table-striped">
        <thead>
            <tr>
                <th>&nbsp;</th>
                <th>Case Number</th>
                <th>Case Name</th>
                <th>Motion Title</th>
                <th>Judge</th>
                <th>Created</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptReferral" runat="server">
                <ItemTemplate>
                    <tr>
                        <td class="command-item"><a title="View Referral" href='<%#EditUrl("rid", Eval("ReferralId").ToString(), "review") %>'>
                            <i class="fa fa-search"></i></a></td>
                        <td><%#Eval("CaseNumber") %></td>
                        <td><%#Eval("CaseParties") %></td>
                        <td><%#Eval("MotionTitle") %></td>
                        <td><%#Eval("JudgeName") %></td>
                        <td><%#Eval("JaCreatedDate", "{0: MM/dd/yyyy}") %></td>
                        <td><%#Eval("StatusName") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <ul class="list-group list-group-horizontal-sm mt-lg">
        <asp:Literal ID="ltRecordMessage" runat="server" />
    </ul>
</div>
<div class="modal fade" id="searchModal" tabindex="-1" role="dialog" aria-labelledby="searchModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="searchModalLabel">Search for Referrals</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div type="post" id="referralSearch">
                    <div class="row form-group">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="drpStatus" Text="Status" />
                            <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Text="< Select Status >" Value=""></asp:ListItem>
                                <asp:ListItem Text="New" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Retained by Judge" Value="3"></asp:ListItem>
                                <asp:ListItem Text="Referred to Court Counsel" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Pending" Value="5"></asp:ListItem>
                                <asp:ListItem Text="Completed" Value="4"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-6">
                            <asp:Label ID="lblJudge" runat="server" AssociatedControlID="drpJudge" Text="Judge" />
                            <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                                <asp:ListItem Text="< Select Judge >" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number" />
                            <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtMotionTitle" Text="Motion Title" />
                            <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date" />
                            <asp:TextBox ID="txtStartDate" runat="server" MaxLength="15" CssClass="form-control datepicker" />

                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date" />
                            <asp:TextBox ID="txtEndDate" runat="server" MaxLength="15" CssClass="form-control datepicker" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <asp:LinkButton ID="cmdSearch" runat="server" CssClass="btn btn-default mr-2" Text="Search" OnClick="cmdSearch_Click" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            $(".datepicker").datepicker();
            var table = $('#table-referrals').DataTable({
                "order": [[3, "desc"]],
                "oLanguage": {
                    "sSearch": "Filter by Text"
                },
            });
            $(".dt-length").prepend('<button class="btn btn-primary me-2" data-bs-toggle="modal" data-bs-target="#searchModal"><i class="fa fa-search"></i>&nbsp;Search All Referrals</button>');
        });
    }(jQuery, window.Sys));

</script>
