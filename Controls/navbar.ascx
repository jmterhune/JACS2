<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="navbar.ascx.cs" Inherits="tjc.Modules.jacs.Controls.navbar" %>
<asp:Panel ID="pnlAdminMenu" runat="server" ClientIDMode="Static" Visible="false">
    <nav class="sidebar collapse show" id="sidebarMenu">
        <div class="position-sticky">
            <ul class="nav flex-column">
                <li class="nav-item">
                    <a class="nav-link" id="lnkMain" href="<%=ModuleContext.MainViewUrl %>">
                        <i class="fa-solid fa-gauge-high"></i>Dashboard
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="collapse" href="#authMenu" role="button" aria-expanded="false" aria-controls="authMenu">
                        <i class="fa-solid fa-people-group"></i>Authentication
                    </a>
                    <div class="collapse" id="authMenu">
                        <ul class="nav flex-column sub-menu">
                            <li class="nav-item"><a class="nav-link" id="lnkUser" href="<%=ModuleContext.UserListUrl %>"><i class="fas fa-user"></i>Users</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkRole" href="<%=ModuleContext.RoleListUrl %>"><i class="fas fa-id-badge"></i>Roles</a></li>
                        </ul>
                    </div>

                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="collapse" href="#jacsMenu" role="button" aria-expanded="false" aria-controls="jacsMenu">
                        <i class="fas fa-user-gear"></i>JACS SA
                    </a>
                    <div class="collapse" id="jacsMenu">
                        <ul class="nav flex-column sub-menu">
                            <li class="nav-item"><a class="nav-link" id="lnkAttorney" href="<%=ModuleContext.AttorneyListUrl %>"><i class="fas fa-circle-user"></i>Attorneys</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourtroom" href="<%=ModuleContext.CourtroomListUrl %>"><i class="fas fa-people-roof"></i>Courtrooms</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCounty" href="<%=ModuleContext.CountyListUrl %>"><i class="fas fa-earth-americas"></i>Counties</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourt" href="<%=ModuleContext.CourtListUrl %>"><i class="fas fa-building-columns"></i>Courts</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourtType" href="<%=ModuleContext.CourtTypeListUrl %>"><i class="fas fa-tags"></i>Court types</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourtPermission" href="<%=ModuleContext.CourtPermissionListUrl %>"><i class="fas fa-user-lock"></i>Court Permissions</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkDocketPrint" href="<%=ModuleContext.DocketPrintUrl %>"><i class="fas fa-print"></i>Docket Print</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEvent" href="<%=ModuleContext.EventListUrl %>"><i class="fas fa-user-clock"></i>Events</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEventStatus" href="<%=ModuleContext.EventStatusListUrl %>"><i class="fas fa-sliders-h"></i>Event Statuses</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEventType" href="<%=ModuleContext.EventTypeListUrl %>"><i class="fas fa-tags"></i>Event Types</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkHoliday" href="<%=ModuleContext.HolidayListUrl %>"><i class="fas fa-gifts"></i>Holidays</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkJudge" href="<%=ModuleContext.JudgeListUrl %>"><i class="fas fa-gavel"></i>Judges</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkMotion" href="<%=ModuleContext.MotionListUrl %>"><i class="fas fa-thumbtack"></i>Motions</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkTemplate" href="<%=ModuleContext.TemplateListUrl %>"><i class="fas fa-object-ungroup"></i>Templates</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkTimeSlot" href="<%=ModuleContext.TimeSlotListUrl %>"><i class="fas fa-clock"></i>Timeslots</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkApiConfig" href="<%=ModuleContext.ApiConfigUrl %>"><i class="fas fa-arrow-right-arrow-left"></i>API Config</a></li>
                        </ul>
                    </div>
                </li>
                <li class="nav-item"><a class="nav-link" id="lnkQuickRef" href="<%=ModuleContext.QuickReferenceUrl %>"><i class="far fa-file-lines"></i>Quick Reference</a></li>
            </ul>
        </div>
    </nav>

</asp:Panel>
<asp:Panel ID="pnlUserMenu" runat="server" ClientIDMode="Static" Visible="false">
    <nav class="sidebar collapse show" id="sidebarMenu">
        <div class="position-sticky">
            <ul class="nav flex-column">
                <li class="nav-item">
                    <a class="nav-link" id="lnkMain" href="<%=ModuleContext.MainViewUrl %>">
                        <i class="fa-solid fa-gauge-high"></i>Dashboard
                    </a>
                </li>
                <li class="nav-item"><a class="nav-link" id="lnkCourt" href="<%=ModuleContext.CourtListUrl %>"><i class="fas fa-building-columns"></i>Courts</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkDocketPrint" href="<%=ModuleContext.DocketPrintUrl %>"><i class="fas fa-print"></i>Docket Print</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkEvent" href="<%=ModuleContext.EventListUrl %>"><i class="fas fa-user-clock"></i>Events</a></li>
                <li class="nav-item hidden"><a class="nav-link" id="lnkTemplate" href="<%=ModuleContext.TemplateListUrl %>"><i class="fas fa-object-ungroup"></i>Templates</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkTimeSlot" href="<%=ModuleContext.TimeSlotListUrl %>"><i class="fas fa-clock"></i>Timeslots</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkQuickRef" href="<%=ModuleContext.QuickReferenceUrl %>"><i class="far fa-file-lines"></i>Quick Reference</a></li>
            </ul>
        </div>
    </nav>

</asp:Panel>

<script src="/DesktopModules/tjc.Modules/Jacs/js/jacs.js"></script>
<script>
    (function ($, Sys) {
        $(document).ready(function () {
            setActiveLink("<%=ActiveLink %>");
        });
    }(jQuery, window.Sys));
</script>
