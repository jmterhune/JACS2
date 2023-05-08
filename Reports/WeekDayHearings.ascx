<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekDayHearings.ascx.cs" Inherits="tjc.Modules.Reports.WeekDayHearings" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal Visible="false" ID="ltMessage" runat="server"><div class="alert alert-{0}"><i class="fa fa-{1}"></i>&nbsp; {2}</div></asp:Literal>
<asp:Panel runat="server" ID="pnlWeekDayHearingCount">
    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Report Criteria">
        <div class="input-group" role="group" aria-label="Search">
            <div class="input-group-text bg-dark text-white" id="lblCounty">County:</div>
            <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpCounty_SelectedIndexChanged" aria-label="Select County" ClientIDMode="Static">
                <asp:ListItem Text="< Select County >" Value="" />
                <asp:ListItem Text="DeSoto" Value="D" />
                <asp:ListItem Text="Manatee" Value="M" />
                <asp:ListItem Text="Sarasota" Value="S" />
            </asp:DropDownList>
            <div class="input-group-text bg-dark text-white" id="lblJudge">Judge:</div>
            <asp:DropDownList Enabled="false" ID="drpJudges" runat="server" DataTextField="FormattedJudgeName" DataValueField="UserId" CssClass="form-control" aria-label="Select Judge" ClientIDMode="Static">
                <asp:ListItem Text="<Select Judge>" Value="" />
            </asp:DropDownList>
            <div class="input-group-text bg-dark text-white" id="lblStartDate">Start Date:</div>
            <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control datepicker" />
            <div class="input-group-text bg-dark text-white" id="lblEndDate">End Date:</div>
            <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control datepicker" />
            <asp:Button ID="cmdSubmit" OnClick="cmdSubmit_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />

        </div>
    </div>

</asp:Panel>
<h2>
    <asp:Literal ID="ltReportTitle" runat="server" Text="Number of Hearings by Weekday" />
</h2>
<asp:HiddenField ID="hdTitle" runat="server" Value="<abbr title='Judicial Automated Calendaring System'>JACS</abbr> Reports" ClientIDMode="Static" />
<asp:GridView ID="grdReport" GridLines="None" OnRowDataBound="OnRowDataBound" CssClass="table table-striped" runat="server" AutoGenerateColumns="true" AllowSorting="true" AllowPaging="false"></asp:GridView>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            var title = $("#hdTitle").val();
            $(".page-top-info h1").html(title);
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var title = $("#hdTitle").val();
        $(".page-top-info h1").html(title);
        $(".datepicker").datepicker();
    }

</script>
