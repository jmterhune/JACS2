<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.LPP_Complaint.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tjc-lpp-admin">
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
        <i class="fa fa-info-circle" aria-hidden="true"></i>&nbsp;<asp:Literal ID="ltEmpty" runat="server" Text="No complaints have been submitted yet." />
    </asp:Panel>

    <asp:Repeater ID="rptComplaints" runat="server">
        <HeaderTemplate>
            <table id="tblComplaints" class="table table-striped">
                <thead>
                    <tr>
                        <th>Submitted</th>
                        <th>Referring party</th>
                        <th>Attorney referred</th>
                        <th>Bar number</th>
                        <th>County</th>
                        <th>Violation date</th>
                        <th>Status</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td data-order='<%# Eval("CreatedOnDate", "{0:yyyy-MM-dd HH:mm}") %>'><%# Eval("CreatedOnDate", "{0:MM/dd/yyyy h:mm tt}") %></td>
                <td><%# Enc(Eval("ReferrerName")) %></td>
                <td><%# Enc(Eval("AttorneyName")) %></td>
                <td><%# Enc(Eval("AttorneyBarNumber")) %></td>
                <td><%# Enc(Eval("County")) %></td>
                <td><%# Enc(Eval("ViolationDate")) %></td>
                <td><span class="badge <%# StatusBadgeClass(Eval("Status")) %>"><%# Enc(Eval("Status")) %></span></td>
                <td class="command-icon">
                    <a class="btn btn-primary btn-sm text-white" href='<%# ComplaintUrl((int)Eval("LppComplaintId")) %>'>
                        <i class="fa fa-search" aria-hidden="true"></i>&nbsp;View</a>
                </td>
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
            if (!$('#tblComplaints').length) { return; }
            $('#tblComplaints').DataTable({
                "bRetrieve": true,
                "bStateSave": true,
                "order": [[0, "desc"]],
                "oLanguage": { "sSearch": "Filter by Text" },
                "aoColumns": [
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": true },
                    { "bSortable": false }
                ]
            });
        });
    }(jQuery));
</script>
