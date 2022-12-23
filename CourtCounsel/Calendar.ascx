<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Calendar" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">

            <li class="nav-item">
                <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
            </li>
            <li class="active nav-item">
                <a class="nav-link" href="<%=EditUrl("calendar") %>"><i class="fas fa-calendar"></i>&nbsp;Event Calendar</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("library") %>"><i class="fas fa-folder-open"></i>&nbsp;Document Repository</a>
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

<asp:UpdatePanel ID="pnlUpdate" runat="server">

    <ContentTemplate>
        <div class="container-fluid">
            <asp:UpdateProgress ID="upProgress" runat="server">
                <ProgressTemplate>
                    <div class="modal-progress">
                        <div class="center-progress">
                            <img alt="" src="/images/loading.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:ListBox ID="lstUsers" runat="server" ClientIDMode="Static" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="lstUsers_SelectedIndexChanged"></asp:ListBox>
            <asp:Repeater ID="rptCalendar" runat="server">
                <HeaderTemplate>
                    <header>
                        <h4 class="display-4 mb-4 text-center">
                            <asp:LinkButton ID="cmdPreviousYear" ToolTip="Previous Year" runat="server" OnClick="cmdPreviousYear_Click"><i class="fas fa-angle-double-left"></i></asp:LinkButton>
                            <asp:LinkButton ID="cmdPreviousMonth" ToolTip="Previous Month" runat="server" OnClick="cmdPreviousMonth_Click"><i class="fas fa-angle-left"></i></asp:LinkButton>
                            <asp:Literal ID="ltHeader" runat="server" />
                            <asp:LinkButton ID="cmdNextMonth" runat="server" ToolTip="Next Month" OnClick="cmdNextMonth_Click"><i class="fas fa-angle-right"></i></asp:LinkButton>
                            <asp:LinkButton ID="cmdNextYear" runat="server" ToolTip="Next Year" OnClick="cmdNextYear_Click"><i class="fas fa-angle-double-right"></i></asp:LinkButton></h4>
                        <div class="row g-0 d-none d-sm-flex p-1 bg-dark">
                            <h5 class="col-sm p-1 text-center text-white">Sunday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Monday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Tuesday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Wednesday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Thursday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Friday</h5>
                            <h5 class="col-sm p-1 text-center text-white">Saturday</h5>
                        </div>
                    </header>
                    <div class="row g-0 border border-right-0 border-bottom-0">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="day col-sm p-2 border border-left-0 border-top-0 text-truncate <%#(bool)Eval("Muted")?"d-none d-sm-inline-block bg-light text-muted":"" %>">
                        <h5 class="row align-items-center">
                            <span class="date col-1"><%#Eval("Day") %></span>
                            <small class="col d-sm-none text-center text-muted"><%#Eval("DayOfWeek") %></small>
                            <span class="col-1"></span>
                        </h5>
                        <%#Eval("EventList") %>
                    </div>
                    <%#Eval("WeekBreak") %>
                </ItemTemplate>
                <FooterTemplate></div></FooterTemplate>
            </asp:Repeater>
            <div class="modal fade" id="tooltipModal" tabindex="-1" role="dialog" aria-labelledby="event-subject" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="event-subject">Title</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        </div>
                        <div id="event-body" class="modal-body">
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-tooltip" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Shared/scripts/jquery.sumoselect.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Shared/stylesheets/sumoselect.min.css" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        InitializeResponsibleDropDown();
        $("#lstUsers").SumoSelect({ selectAll: true,placeholder:'Select User(s)' });
        $(".event-item").on("dblclick", function (e) {
            var subject = $(this).data("subject");
            var body = $(this).data("body");
            var user = $(this).data("user");
            $("#event-subject").text(subject);
            $("#event-body").html(`<p>${body}</p><p><strong>Last Modified By:</strong> ${user}</p>`);
            $('#tooltipModal').modal("show");
        });
       <%-- $(".event-item").on("dblclick", function (e) {
            var assignmentId = $(this).data("assignmentid");
            window.location.href = "<%=EditUrl("logEdit") %>/aid/" + assignmentId;
        });--%>

    }
    function InitializeResponsibleDropDown() {
        var $select = $("#lstUsers");
        var currentSelection = $select.val();
        var optGroup;
        $("#lstUsers option").each(function () {
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
</script>
