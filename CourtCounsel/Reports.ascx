<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">

            <li class="active nav-item">
                <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
            </li>
            <li class="nav-item" id="li1" runat="server" visible="false">
                <a class="nav-link" href="<%=EditUrl("admin") %>"><i class="fa fa-tools"></i>&nbsp;Admin</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=SharePointSiteURL %>"><i class="fas fa-home"></i>&nbsp;Team Site</a>
            </li>
        </ul>

    </div>
</nav>
<div class="mb-3">
    <div class="row form-group">
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start" ToolTip="required" />
            <asp:TextBox runat="server" CssClass="form-control datepicker" ClientIDMode="Static" ID="txtStartDate" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End" ToolTip="required" />
            <asp:TextBox runat="server" CssClass="form-control datepicker" ClientIDMode="Static" ID="txtEndDate" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndDate"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
        </div>
    </div>
    <div class="row form-group">
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpStatus" Text="Status" />
            <asp:DropDownList ID="drpStatus" CssClass="form-control" runat="server" >
                 <asp:ListItem Value="ALL">All</asp:ListItem>
                <asp:ListItem Value="A">Active</asp:ListItem>
                <asp:ListItem Value="P">Pending</asp:ListItem>
                <asp:ListItem Value="N">Not Completed</asp:ListItem>
                <asp:ListItem Value="C">Completed</asp:ListItem>               
            </asp:DropDownList>
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpExtendedStatus" Text="Extended Status" />
            <asp:DropDownList ID="drpExtendedStatus" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Value="" Text="All" />
                <asp:ListItem Text="Admin Task Completed" />
                <asp:ListItem Text="Admin Review Needed" />
                <asp:ListItem Text="Amended Motion Due " />
                <asp:ListItem Text="Answer Brief Due" />
                <asp:ListItem Text="Assigned" />
                <asp:ListItem Text="Completed" />
                <asp:ListItem Text="EOT Filed" />
                <asp:ListItem Text="EOT Granted" />
                <asp:ListItem Text="Evidentiary Hearing Granted" />
                <asp:ListItem Text="Evidentiary Hearing Scheduled" />
                <asp:ListItem Text="Fee Order Issued" />
                <asp:ListItem Text="Final Order Due" />
                <asp:ListItem Text="Follow up needed" />
                <asp:ListItem Text="Initial Brief Due" />
                <asp:ListItem Text="Mandamus Petition Filed w/ 2nd" />
                <asp:ListItem Text="Motion Stricken With Leave to Amend" />
                <asp:ListItem Text="Motion Under Review" />
                <asp:ListItem Text="NOI II filed" />
                <asp:ListItem Text="NOI III filed" />
                <asp:ListItem Text="Non-Final Order Entered" />
                <asp:ListItem Text="Notice of Inquiry Filed" />
                <asp:ListItem Text="Order to Show Cause" />
                <asp:ListItem Text="Ordered Response" />
                <asp:ListItem Text="Post Conviction Counsel Appointed" />
                <asp:ListItem Text="Proposed Order Submitted" />
                <asp:ListItem Text="Ready for Disposition" />
                <asp:ListItem Text="Reply Brief (Optional)" />
                <asp:ListItem Text="State Ordered to Respond" />
                <asp:ListItem Text="State's Response Due" />
                <asp:ListItem Text="State's Response Filed" />
                <asp:ListItem Text="Transcripts Ordered" />
                <asp:ListItem Text="Other" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="row form-group">
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpAttorney" Text="Attorney" />
            <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control" aria-label="Select Attorney" ClientIDMode="Static">
            </asp:DropDownList>
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpRequestor" Text="Requestor" />
            <asp:DropDownList ID="drpRequestor" runat="server" AppendDataBoundItems="True" ClientIDMode="Static"
                DataTextField="RequestorName" DataValueField="RequestorName" CssClass="form-control">
            </asp:DropDownList>
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County" />
            <asp:DropDownList ID="drpCounty" runat="server" AppendDataBoundItems="True"
                DataTextField="County" DataValueField="County" CssClass="form-control" ClientIDMode="Static">
            </asp:DropDownList>
        </div>
    </div>
    <div class="mt-3">
        <div class="form-check form-switch mt-4">
            <asp:CheckBox ID="chkShowDetail" runat="server" Text="Show Detail" />
        </div>
    </div>
    <asp:Button ID="cmdSearch" OnClick="cmdSearch_Click" ClientIDMode="Static" runat="server" Text="Search" ToolTip="Search Court Counsel Records" CssClass="btn btn-primary" />
    <asp:HiddenField ID="hdSearchType" runat="server" ClientIDMode="Static" Value="0" />
</div>
<hr />
<asp:Literal ID="ltHistory" runat="server"></asp:Literal>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        $(".datepicker").datepicker();
        InitializeStatusDropDown();
        InitializeResponsibleDropDown();
        InitializeRequestedByDropDown();
    }
    function InitializeRequestedByDropDown() {
        var $select = $("#drpRequestor");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpRequestor option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    function InitializeResponsibleDropDown() {
        var $select = $("#drpAttorney");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpAttorney option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    function InitializeStatusDropDown() {
        var $select = $("#drpExtendedStatus");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpExtendedStatus option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive' data-pending='" + $(this).data("pending") + "'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option data-pending='" + $(this).data("pending") + "'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
</script>
