<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffAttorneySeniorityReport.ascx.cs" Inherits="tjc.Modules.Reports.EmployeeReports.StaffAttorneySeniorityReport" %>

<%-- Trial Court Staff Attorney / Law Clerk Seniority — replaces the
     "Trial Court Staff Attorneys" sheet in EmployeeDB\Documentation\
     JA seniority.xlsx. Now includes BOTH active and terminated employees
     (terminated are color-coded so they're easy to filter visually). --%>
<div class="container-fluid">
    <div class="d-flex align-items-center mb-3">
        <h3 class="mb-0">Trial Court Staff Attorney Seniority</h3>
        <asp:Button ID="btnExport" runat="server" CssClass="btn btn-success btn-sm ms-auto"
                    Text="Download Excel" OnClick="btnExport_Click" />
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary btn-sm ms-2">
            <i class="fas fa-arrow-left"></i>&nbsp;Back to Reports
        </asp:HyperLink>
    </div>

    <p class="text-muted">
        Trial court law clerks / staff attorneys ordered by start date.
        2-year incentive is the third-anniversary date (per policy); senior-promo
        eligibility is the fifth-anniversary date. Terminated employees are
        included for historical reference. Click any column header to sort.
    </p>

    <%-- Color key. Matches the row tints below. --%>
    <div class="empdb-status-legend">
        <span class="legend-item legend-completed">Completed (5+ yrs of service)</span>
        <span class="legend-item legend-eligible">Eligible (2&ndash;5 yrs)</span>
        <span class="legend-item legend-not-eligible">Not Yet Eligible (&lt;2 yrs)</span>
        <span class="legend-item legend-terminated">Terminated</span>
    </div>

    <asp:GridView ID="grdReport" runat="server" GridLines="None"
                  CssClass="table table-striped table-sm" AutoGenerateColumns="false"
                  AllowSorting="true" OnSorting="grdReport_Sorting"
                  OnRowDataBound="grdReport_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Staff Attorney" SortExpression="LastName">
                <ItemTemplate><%# Eval("LastName") %>, <%# Eval("FirstName") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" SortExpression="Status">
                <ItemTemplate><%# DescribeStatus(Eval("IsActive"), Eval("StartDate")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StartDate" HeaderText="Attorney Start Date" SortExpression="StartDate" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:TemplateField HeaderText="2-yr Incentive Begins" SortExpression="StartDate">
                <ItemTemplate><%# FormatNullableDate(AddYears(Eval("StartDate"), 2)) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Eligible for Senior Promo" SortExpression="StartDate">
                <ItemTemplate><%# FormatNullableDate(AddYears(Eval("StartDate"), 5)) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="2-yr Reached?">
                <ItemTemplate><%# HasYearsPassed(Eval("StartDate"), 2) ? "X" : "" %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="5-yr Reached?">
                <ItemTemplate><%# HasYearsPassed(Eval("StartDate"), 5) ? "X" : "" %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="alert alert-info">No staff attorneys found.</div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
