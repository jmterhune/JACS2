<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.Reports.View" %>
<asp:Panel runat="server" ID="pnlReportList">
    <ul class="list list-icons">
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/1"><i class="fas fa-birthday-cake"></i> Birthday Report</a></li>
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/2"><i class="fas fa-star"></i> Service Reports</a></li>
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/3"><i class="fas fa-user-minus"></i> Termination Report</a></li>
        <li>
            <asp:HyperLink ID="lnkDataCard" runat="server"><i class="fas fa-id-badge"></i> Data Card</asp:HyperLink></li>

    </ul>
</asp:Panel>
<asp:Panel runat="server" ID="pnlBirthdays" Visible="false">
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div id="swBirthMonth" class="input-group">
                <asp:DropDownList ID="drpBirthMonth" runat="server" CssClass="form-control" aria-label="Select Month" ClientIDMode="Static">
                    <asp:ListItem Text="January" Value="1" />
                    <asp:ListItem Text="February" Value="2" />
                    <asp:ListItem Text="March" Value="3" />
                    <asp:ListItem Text="April" Value="4" />
                    <asp:ListItem Text="May" Value="5" />
                    <asp:ListItem Text="June" Value="6" />
                    <asp:ListItem Text="July" Value="7" />
                    <asp:ListItem Text="August" Value="8" />
                    <asp:ListItem Text="September" Value="9" />
                    <asp:ListItem Text="October" Value="10" />
                    <asp:ListItem Text="November" Value="11" />
                    <asp:ListItem Text="December" Value="12" />
                </asp:DropDownList>
            </div>
            <div id="swCounty" class="input-group" runat="server">
                <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" aria-label="Select County" ClientIDMode="Static">
                    <asp:ListItem Text="All Counties" Value="" />
                    <asp:ListItem Text="DeSoto"/>
                    <asp:ListItem Text="Manatee"/>
                    <asp:ListItem Text="Sarasota"/>
                </asp:DropDownList>
            </div>

        </div>
        <asp:Button ID="cmdSubmitBirthReport" OnClick="cmdSubmitBirthReport_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlServiceAward" Visible="false">
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div id="swServiceMonth" class="input-group">
                <asp:DropDownList ID="drpServiceMonth" runat="server"  CssClass="form-control" aria-label="Select Month" ClientIDMode="Static">
                    <asp:ListItem Text="January" Value="1" />
                    <asp:ListItem Text="February" Value="2" />
                    <asp:ListItem Text="March" Value="3" />
                    <asp:ListItem Text="April" Value="4" />
                    <asp:ListItem Text="May" Value="5" />
                    <asp:ListItem Text="June" Value="6" />
                    <asp:ListItem Text="July" Value="7" />
                    <asp:ListItem Text="August" Value="8" />
                    <asp:ListItem Text="September" Value="9" />
                    <asp:ListItem Text="October" Value="10" />
                    <asp:ListItem Text="November" Value="11" />
                    <asp:ListItem Text="December" Value="12" />
                </asp:DropDownList>
            </div>
            <div id="swReportType" runat="server" class="input-group">
                <asp:DropDownList ID="drpReportType" runat="server" CssClass="form-control" aria-label="Select Report Type" ClientIDMode="Static">
                    <asp:ListItem Text="Service Date" Value="1" />
                    <asp:ListItem Text="Hire Date" Value="0" />
                </asp:DropDownList>
            </div>
        </div>
        <asp:Button ID="cmdSubmitServiceReport" OnClick="cmdSubmitServiceReport_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlTerminationReport" Visible="false">
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div id="swStartDate" class="input-group">
                <asp:TextBox runat="server" CssClass="form-control" TextMode="date" ID="txtStartDate" ClientIDMode="Static" />
            </div>
            <div id="swEndDate" class="input-group">
                <asp:TextBox runat="server" CssClass="form-control" TextMode="date" ID="txtEndDate" ClientIDMode="Static" />
            </div>
        </div>
        <asp:Button ID="cmdTerminationReport" OnClick="cmdTerminationReport_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<h2>
    <asp:Literal ID="ltReportTitle" runat="server" />
</h2>
<asp:HiddenField  id="hdTitle" runat="server" Value="Employee Reports" ClientIDMode="Static" />
<asp:GridView ID="grdReport" GridLines="None" OnRowDataBound="OnRowDataBound" CssClass="table table-striped" runat="server" AutoGenerateColumns="true" AllowSorting="true" AllowPaging="false"></asp:GridView>
<asp:HyperLink CssClass="btn btn-primary" ID="lnkReport" runat="server" Text="Return to Report List" /> 
<script>
    $(function () {
        var title = $("#hdTitle").val();
        $(".page-top-info h1").html(title);
    });
   
</script>