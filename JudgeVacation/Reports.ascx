<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.JudgeVacation.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="form-container">
    <div class="row g-3" id="vacation-form">
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="StartDatePicker" Text="Start Date" />
            <asp:TextBox runat="server" ID="StartDatePicker" CssClass="date-picker form-control" MaxLength="20" />
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="EndDatePicker" Text="End Date" />
            <asp:TextBox runat="server" ID="EndDatePicker" CssClass="date-picker form-control" MaxLength="20" />
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpJudges" Text="Select Judge" />
            <asp:DropDownList ID="drpJudges" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                <asp:ListItem Text="All" Value="0" />
            </asp:DropDownList>
        </div>
    </div>
    <p class="mt-3">
        <asp:LinkButton ID="cmdSubmit" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="cmdSubmit_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </p>
    <hr />
    <asp:Panel ID="pnlRecords" runat="server">
        <asp:Literal ID="ltMessage" runat="server" />
        <asp:Repeater ID="rptVacationDays" runat="server" OnItemDataBound="rptVacationDays_ItemDataBound">
            <HeaderTemplate>
                <div id="report">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Start Date</th>
                                <th>End Date</th>
                                <th>Vacation Days Used</th>
                                <th>Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("JudgeName") %>
                    </td>
                    <td><%# Eval("StartDate", "{0: MM/dd/yyyy}") %></td>
                    <td><%# Eval("EndDate", "{0: MM/dd/yyyy}") %></td>
                    <td style="text-align: right"><%# Eval("VacationDays") %></td>
                    <td style="text-align: right"><%# Int32.Parse(Eval("SubTotal").ToString()) == 0? "": Eval("SubTotal") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="4" style="text-align: right; font-weight: bold">Total</td>
                        <td style="text-align: right; font-weight: bold">
                            <asp:Literal ID="ltTotal" runat="server" /></td>
                    </tr>
                </tfoot>
                </table>
                </div>
                <p><a href="#" id="lnkPrint" class="btn btn-primary">Print Report</a></p>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script>
    (function ($, Sys) {

        $(document).ready(function () {
            $(".date-picker").datepicker();
            $("#lnkPrint").on("click", function (e) {
                e.preventDefault();
                PrintTable()
            });
        });

    }(jQuery, window.Sys));

    function PrintTable() {

        var divToPrint = document.getElementById("report");
        var newWin = window.open('PrintDiv', 'Print-Window', 'width=920,height=750,top=100,left=100');
        newWin.document.open();
        newWin.document.write('<html><title>Vacation Report</title><head><style>@media print {.noprint {visibility: hidden;}}.table{width:100%;border-collapse:collapse}.table tr:nth-of-type(odd){background:#eee}.table th{background:#333;color:#fff;font-weight:bold}.table td,table th{padding:6px;border:1px solid #ccc;text-align:left}</style><body>' + divToPrint.innerHTML + '<p><button type="button" class="noprint" onclick="window.print();window.close()">Print</button></p></body></html>');
    }
</script>
