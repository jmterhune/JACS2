<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropParticipantsReport.ascx.cs" Inherits="tjc.Modules.Reports.EmployeeReports.DropParticipantsReport" %>

<%-- DROP (Deferred Retirement Option Program) Participants — replaces
     EmployeeDB\Documentation\DROP Participants.xlsx. Filters to
     tjc_employee.DropEntryDate IS NOT NULL. Rows are color-coded by
     DROP status so completed-and-retired vs. still-in-DROP vs.
     terminated-without-completing is visually obvious. --%>
<div class="container-fluid">
    <div class="d-flex align-items-center mb-3">
        <h3 class="mb-0">DROP Participants</h3>
        <asp:Button ID="btnExport" runat="server" CssClass="btn btn-success btn-sm ms-auto"
                    Text="Download Excel" OnClick="btnExport_Click" />
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary btn-sm ms-2">
            <i class="fas fa-arrow-left"></i>&nbsp;Back to Reports
        </asp:HyperLink>
    </div>

    <p class="text-muted">
        Every employee with a recorded DROP entry date, ordered by entry date.
        Click any column header to re-sort; click again to flip ascending/descending.
        Edit any employee on the EmployeeDB Edit form (Employment → DROP / Certification)
        to add or change these fields.
    </p>

    <%-- Color key. Matches the row tints below. DROP has 3 buckets (no
         "not yet eligible" — to appear on this report you must already
         have a DROP entry date). --%>
    <div class="empdb-status-legend">
        <span class="legend-item legend-completed">Completed (Retired)</span>
        <span class="legend-item legend-eligible">In DROP</span>
        <span class="legend-item legend-terminated">Terminated</span>
    </div>

    <asp:GridView ID="grdReport" runat="server" GridLines="None"
                  CssClass="table table-striped table-sm" AutoGenerateColumns="false"
                  AllowSorting="true" OnSorting="grdReport_Sorting"
                  OnRowDataBound="grdReport_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Employee" SortExpression="LastName">
                <ItemTemplate><%# Eval("LastName") %>, <%# Eval("FirstName") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Job Title" SortExpression="JobTitle">
                <ItemTemplate><%# Eval("JobTitle") %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DropEntryDate"   HeaderText="DROP Entry"           SortExpression="DropEntryDate"   DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="DropExitDate"    HeaderText="DROP Exit"            SortExpression="DropExitDate"    DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="TerminationDate" HeaderText="Termination"          SortExpression="TerminationDate" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="DropLeavePayout" HeaderText="Leave Payout (hrs)"   SortExpression="DropLeavePayout" DataFormatString="{0:N2}" />
            <asp:TemplateField HeaderText="Status" SortExpression="Status">
                <ItemTemplate><%# DescribeStatus(Eval("IsActive"), Eval("DropExitDate")) %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="alert alert-info">No DROP participants on record.</div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
