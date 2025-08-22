<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="navbar.ascx.cs" Inherits="tjc.Modules.jacs.Controls.navbar" %>
<asp:Panel ID="pnlAdminMenu" runat="server" ClientIDMode="Static" Visible="false">
    <nav class="sidebar collapse show" id="sidebarMenu">
        <div class="position-sticky">
            <ul class="nav flex-column">
                <li class="nav-item">
                    <a class="nav-link" id="lnkMain" href="<%=MainViewUrl %>">
                        <i class="fa-solid fa-gauge-high"></i>Dashboard
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="collapse" href="#authMenu" role="button" aria-expanded="false" aria-controls="authMenu">
                        <i class="fa-solid fa-people-group"></i>Authentication
                    </a>
                    <div class="collapse" id="authMenu">
                        <ul class="nav flex-column sub-menu">
                            <li class="nav-item"><a class="nav-link" id="lnkUser" href="<%=UserListUrl %>"><i class="fas fa-user"></i>Users</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkRole" href="<%=RoleListUrl %>"><i class="fas fa-id-badge"></i>Roles</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkPermission" href="<%=PermissionListUrl %>"><i class="fas fa-key"></i>Permissions</a></li>
                        </ul>
                    </div>

                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="collapse" href="#jacsMenu" role="button" aria-expanded="false" aria-controls="jacsMenu">
                        <i class="fas fa-user-gear"></i>JACS SA
                    </a>
                    <div class="collapse" id="jacsMenu">
                        <ul class="nav flex-column sub-menu">
                            <li class="nav-item"><a class="nav-link" id="lnkAttorney" href="<%=AttorneyListUrl %>"><i class="fas fa-circle-user"></i>Attorneys</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCategory" href="<%=CategoryListUrl %>"><i class="fas fa-bars-staggered"></i>Categories</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCounty" href="<%=CountyListUrl %>"><i class="fas fa-earth-americas"></i>Counties</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourt" href="<%=CourtListUrl %>"><i class="fas fa-building-columns"></i>Courts</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourtType" href="<%=CourtTypeListUrl %>"><i class="fas fa-tags"></i>Court types</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkCourtPermission" href="<%=CourtPermissionListUrl %>"><i class="fas fa-user-lock"></i>Court Permissions</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkDocketPrint" href="<%=DocketPrintUrl %>"><i class="fas fa-print"></i>Docket Print</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEvent" href="<%=EventListUrl %>"><i class="fas fa-user-clock"></i>Events</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEventStatus" href="<%=EventStatusListUrl %>"><i class="fas fa-sliders-h"></i>Event Statuses</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkEventType" href="<%=EventTypeListUrl %>"><i class="fas fa-tags"></i>Event Types</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkHoliday" href="<%=HolidayListUrl %>"><i class="fas fa-gifts"></i>Holidays</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkJudge" href="<%=JudgeListUrl %>"><i class="fas fa-gavel"></i>Judges</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkMotion" href="<%=MotionListUrl %>"><i class="fas fa-thumbtack"></i>Motions</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkTemplate" href="<%=TemplateListUrl %>"><i class="fas fa-object-ungroup"></i>Templates</a></li>
                            <li class="nav-item"><a class="nav-link" id="lnkTimeSlot" href="<%=TimeSlotListUrl %>"><i class="fas fa-clock"></i>Timeslots</a></li>
                        </ul>
                    </div>
                </li>
                <li class="nav-item"><a class="nav-link" id="lnkQuickRef" href="<%=QuickReferenceUrl %>"><i class="far fa-file-lines"></i>Quick Reference</a></li>
            </ul>
        </div>
    </nav>

</asp:Panel>
<asp:Panel ID="pnlUserMenu" runat="server" ClientIDMode="Static" Visible="false">
    <nav class="sidebar collapse show" id="sidebarMenu">
        <div class="position-sticky">
            <ul class="nav flex-column">
                <li class="nav-item">
                    <a class="nav-link" id="lnkMain" href="<%=MainViewUrl %>">
                        <i class="fa-solid fa-gauge-high"></i>Dashboard
                    </a>
                </li>
                <li class="nav-item"><a class="nav-link" id="lnkCourt" href="<%=CourtListUrl %>"><i class="fas fa-building-columns"></i>Courts</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkCourtPermission" href="<%=CourtPermissionListUrl %>"><i class="fas fa-calendar"></i>Active Calendar</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkDocketPrint" href="<%=DocketPrintUrl %>"><i class="fas fa-print"></i>Docket Print</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkEvent" href="<%=EventListUrl %>"><i class="fas fa-user-clock"></i>Events</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkTemplate" href="<%=TemplateListUrl %>"><i class="fas fa-object-ungroup"></i>Templates</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkTimeSlot" href="<%=TimeSlotListUrl %>"><i class="fas fa-clock"></i>Timeslots</a></li>
                <li class="nav-item"><a class="nav-link" id="lnkQuickRef" href="<%=QuickReferenceUrl %>"><i class="far fa-file-lines"></i>Quick Reference</a></li>
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
