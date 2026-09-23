<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Results.ascx.cs" Inherits="tjc.Modules.ExitSurvey.Admin.Results" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<div class="container-fluid exit-survey">

    <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>

    <h4>Exit Survey Results</h4>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
        <p>No exit surveys have been submitted yet.</p>
    </asp:Panel>

    <asp:Repeater ID="rptResults" runat="server" OnItemDataBound="rptResults_ItemDataBound" OnItemCommand="rptResults_ItemCommand">
        <HeaderTemplate>
            <table id="tblResults" class="table table-striped table-bordered table-hover exit-survey-results">
                <thead>
                    <tr>
                        <th class="no-sort"></th>
                        <th>Submitted</th>
                        <th>Name</th>
                        <th>Accepted Another Position</th>
                        <th>Would Return</th>
                        <th>Share With Supervisor</th>
                        <th class="no-sort"></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="text-nowrap exit-survey-actions">
                    <asp:HyperLink ID="lnkView" runat="server" CssClass="text-primary me-3" ToolTip="View"><i class="fas fa-search"></i></asp:HyperLink>
                    <asp:HyperLink ID="lnkPrint" runat="server" CssClass="text-secondary" ToolTip="Print / Save as PDF"><i class="fas fa-print"></i></asp:HyperLink>
                </td>
                <td data-order='<%# Eval("CreatedOnDate", "{0:yyyyMMddHHmmss}") %>'><%# Eval("CreatedOnDate", "{0:MM/dd/yyyy h:mm tt}") %></td>
                <td><asp:Literal ID="ltName" runat="server" /></td>
                <td><asp:Literal ID="ltAccepted" runat="server" /></td>
                <td><asp:Literal ID="ltReturn" runat="server" /></td>
                <td><asp:Literal ID="ltShare" runat="server" /></td>
                <td class="text-nowrap exit-survey-actions">
                    <asp:LinkButton ID="cmdDelete" runat="server" CssClass="text-danger" CommandName="delete" CommandArgument='<%# Eval("ResponseID") %>' ToolTip="Delete" OnClientClick="return confirm('Delete this exit survey? This cannot be undone.');"><i class="fas fa-trash"></i></asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

</div>

<script type="text/javascript">
    (function () {
        function init() {
            var $ = window.jQuery;
            if (!$ || !$.fn || !$.fn.DataTable) return;
            var tbl = document.getElementById("tblResults");
            if (!tbl || $.fn.DataTable.isDataTable(tbl)) return;
            $(tbl).DataTable({
                order: [[1, "desc"]],                 // Submitted, newest first
                columnDefs: [{ orderable: false, searchable: false, targets: "no-sort" }],
                pageLength: 25
            });
        }
        if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", init);
        else init();
    })();
</script>
