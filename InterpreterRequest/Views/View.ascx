<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.InterpreterRequest.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tjc-interpreter-admin">
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:Label ID="lblStatusFilter" runat="server" AssociatedControlID="drpStatusFilter" Text="Status" />
            <asp:DropDownList ID="drpStatusFilter" runat="server" CssClass="form-control" AutoPostBack="true"
                OnSelectedIndexChanged="drpStatusFilter_SelectedIndexChanged" />
        </div>
        <div class="col-md-8 d-flex align-items-end justify-content-end">
            <asp:LinkButton ID="cmdExport" runat="server" CssClass="btn btn-secondary text-white" CausesValidation="false"
                OnClick="cmdExport_Click"><i class="fa fa-file-excel-o" aria-hidden="true"></i>&nbsp;Export to CSV</asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
        <i class="fa fa-info-circle" aria-hidden="true"></i>&nbsp;<asp:Literal ID="ltEmpty" runat="server" Text="No interpreter requests have been submitted yet." />
    </asp:Panel>

    <asp:Repeater ID="rptRequests" runat="server">
        <HeaderTemplate>
            <table id="tblRequests" class="table table-striped">
                <thead>
                    <tr>
                        <th class="col-icon"></th>
                        <th>Submitted</th>
                        <th>Requested by</th>
                        <th>Language</th>
                        <th>Case number</th>
                        <th>County</th>
                        <th>Needed</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="col-icon">
                    <%-- "fa" is the Font Awesome 4 class the DnnBootster skin loads; "fas" is the
                         Font Awesome 5+ one the CDSP Submissions list uses. Both are given so the
                         magnifier renders whichever version the site is serving. --%>
                    <a class="icon-btn text-primary" title="View details"
                       href='<%# RequestUrl((int)Eval("InterpreterRequestId")) %>'><i class="fa fas fa-search" aria-hidden="true"></i></a>
                </td>
                <td data-order='<%# Eval("CreatedOnDate", "{0:yyyy-MM-dd HH:mm}") %>'><%# Eval("CreatedOnDate", "{0:MM/dd/yyyy h:mm tt}") %></td>
                <td><%# Enc(Eval("RequestorName")) %></td>
                <td><%# Enc(Eval("InterpreterLanguage")) %></td>
                <td><%# Enc(Eval("CaseNumber")) %></td>
                <td><%# Enc(Eval("County")) %></td>
                <td><%# Enc(Eval("WhenNeeded")) %></td>
                <td><span class="badge <%# StatusBadgeClass(Eval("Status")) %>"><%# Enc(Eval("Status")) %></span></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    (function ($) {
        $(document).ready(function () {
            if (!$('#tblRequests').length) { return; }
            $('#tblRequests').DataTable({
                "bRetrieve": true,
                "bStateSave": true,
                /* Column 0 is the view icon, so the date is column 1. */
                "order": [[1, "desc"]],
                "oLanguage": { "sSearch": "Filter by Text" },
                "aoColumns": [
                    { "bSortable": false },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true }
                ]
            });
        });
    }(jQuery));
</script>
