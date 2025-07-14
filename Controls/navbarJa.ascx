<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="navbarJa.ascx.cs" Inherits="tjc.Modules.jacs.Controls.navbar" %>
<nav class="sidebar collapse show" id="sidebarMenu">
    <div class="position-sticky">
        <ul class="nav flex-column">
            <li class="nav-item">
                <a class="nav-link" id="lnkMain" href="<%=MainViewUrl %>">
                    <i class="fa-solid fa-gauge-high"></i>Dashboard
                </a>
            </li>
            <li class="nav-item"><a class="nav-link" id="lnkCourt" href="<%=CourtListUrl %>"><i class="fas fa-building-columns"></i>Courts</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkCourtPermission" href="<%=CourtPermissionListUrl %>"><i class="fas fa-user-lock"></i>Court Permissions</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkDocketPrint" href="<%=DocketPrintUrl %>"><i class="fas fa-print"></i>Docket Print</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkEvent" href="<%=EventListUrl %>"><i class="fas fa-user-clock"></i>Events</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkTemplate" href="<%=TemplateListUrl %>"><i class="fas fa-object-ungroup"></i>Templates</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkTimeSlot" href="<%=TimeSlotListUrl %>"><i class="fas fa-clock"></i>Timeslots</a></li>
            <li class="nav-item"><a class="nav-link" id="lnkQuickRef" href="<%=QuickReferenceUrl %>"><i class="far fa-file-lines"></i>Quick Reference</a></li>
        </ul>
    </div>
</nav>
<script src="/DesktopModules/tjc.Modules/Jacs/js/jacs.js"></script>
<script>
    (function ($, Sys) {
        $(document).ready(function () {
            setActiveLink("<%=ActiveLink %>");
        });
    }(jQuery, window.Sys));
</script>
