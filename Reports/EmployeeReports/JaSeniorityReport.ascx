<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JaSeniorityReport.ascx.cs" Inherits="tjc.Modules.Reports.EmployeeReports.JaSeniorityReport" %>

<%-- Judicial Assistant Seniority — replaces EmployeeDB\Documentation\
     JA seniority.xlsx. JAs become eligible for a salary adjustment on
     the first day of the month following completion of six years of
     judicial assistant service. Now includes BOTH active and terminated
     employees (terminated are color-coded so they're easy to filter
     visually). --%>
<div class="container-fluid">
    <div class="d-flex align-items-center mb-3">
        <h3 class="mb-0">Judicial Assistant Seniority</h3>
        <asp:Button ID="btnExport" runat="server" CssClass="btn btn-success btn-sm ms-auto"
                    Text="Download Excel" OnClick="btnExport_Click" />
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary btn-sm ms-2">
            <i class="fas fa-arrow-left"></i>&nbsp;Back to Reports
        </asp:HyperLink>
    </div>

    <p class="text-muted">
        Judicial Assistants ordered by 12th Circuit hire date. Six-year
        eligibility is the first day of the month following the six-year
        anniversary; "Incentive Active" is &quot;X&quot; while the employee is
        between 6 and 12 years of service (the $1,200 cap). Terminated
        employees are included for historical reference. Click any column
        header to sort.
    </p>

    <%-- Color key. Matches the row tints below. --%>
    <div class="empdb-status-legend">
        <span class="legend-item legend-completed">Completed (12+ yrs of service)</span>
        <span class="legend-item legend-eligible">Eligible (6&ndash;12 yrs)</span>
        <span class="legend-item legend-not-eligible">Not Yet Eligible (&lt;6 yrs)</span>
        <span class="legend-item legend-terminated">Terminated</span>
    </div>

    <asp:GridView ID="grdReport" runat="server" GridLines="None"
                  CssClass="table table-striped table-sm" AutoGenerateColumns="false"
                  AllowSorting="true" OnSorting="grdReport_Sorting"
                  OnRowDataBound="grdReport_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Judicial Assistant" SortExpression="LastName">
                <ItemTemplate><%# Eval("LastName") %>, <%# Eval("FirstName") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" SortExpression="Status">
                <ItemTemplate><%# DescribeStatus(Eval("IsActive"), Eval("StartDate")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StartDate" HeaderText="12th Circuit JA Start Date" SortExpression="StartDate" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:TemplateField HeaderText="6-Year Eligibility" SortExpression="StartDate">
                <ItemTemplate><%# FormatNullableDate(SixYearEligibility(Eval("StartDate"))) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Incentive Active">
                <ItemTemplate><%# IsInIncentiveWindow(Eval("StartDate"), 6, 12) ? "X" : "" %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Years of Service" SortExpression="StartDate">
                <ItemTemplate><%# YearsOfService(Eval("StartDate")) %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="alert alert-info">No Judicial Assistants found.</div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
