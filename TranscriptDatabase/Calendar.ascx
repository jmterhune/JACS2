<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.Calendar" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#calendar" data-toggle="tab">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormListUrl%>">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=HearingListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="designation" class="tab-pane active">
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
                        <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control">
                            <asp:ListItem Text="< Select County >" Value="" />
                            <asp:ListItem Text="DeSoto" />
                            <asp:ListItem Text="Manatee" />
                            <asp:ListItem Text="Sarasota" />
                        </asp:DropDownList>
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
        </div>
    </div>
</div>
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
        $("#lstUsers").SumoSelect({ selectAll: true, placeholder: 'Select User(s)' });
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
    
</script>
