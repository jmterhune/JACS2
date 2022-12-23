<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseView.ascx.cs" Inherits="tjc.Modules.CourtCounsel.CaseView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">

            <li class="nav-item">
                <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("calendar") %>"><i class="fas fa-calendar"></i>&nbsp;Event Calendar</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("library") %>"><i class="fas fa-folder-open"></i>&nbsp;Document Repository</a>
            </li>
            <li class="nav-item" id="li1" runat="server" visible="false">
                <a class="nav-link" href="<%=EditUrl("admin") %>"><i class="fa fa-tools"></i>&nbsp;Admin</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="https://jud12fl.sharepoint.com/sites/CourtCounsel"><i class="fas fa-home"></i>&nbsp;Team Site</a>
            </li>
        </ul>

    </div>
</nav>
<asp:Literal ID="ltCaseHeading" runat="server" />
<asp:UpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:Repeater ID="rptLogEntries" runat="server">
            <HeaderTemplate>
                <table id="log-list" class="table table-striped">
                    <thead>
                        <tr>
                            <th>Action Date</th>                            
                            <th>Case Name</th>
                            <th>Action Taken</th>
                            <th>Responsible</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr data-id="<%#DataBinder.Eval(Container.DataItem,"LogId").ToString() %>">
                    <td><a href="<%#EditUrl("aid",DataBinder.Eval(Container.DataItem,"AssignmentId").ToString(),"logedit") %>"><%#DataBinder.Eval(Container.DataItem,"DateReceived", "{0:M/d/yy}") %></a></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"ActionName") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"AttorneyName") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"StatusType") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
    <Triggers>
    </Triggers>
</asp:UpdatePanel>

<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.12.1/js/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.12.1/css/dataTables.bootstrap5.min.css" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {

        var table = $('#log-list').DataTable({

            "order": [[0, "desc"]],
            "oLanguage": {

                "sSearch": "Filter by Text"

            },
        });

    }
</script>
